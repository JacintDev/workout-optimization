#include <WiFi.h>
#include <WebServer.h>
#include <LittleFS.h>
#include <ArduinoJson.h>
#include <HTTPClient.h>

// Wi-Fi AP beállítások
const char *apSSID = "ESP32_Setup";
const char *apPassword = "12345678";

// Webszerver példány
WebServer server(80);

String currentSSID, currentPassword;
bool wifiConnected = false;
String authToken="";

//Hotspot indítás
void startAP() {
    WiFi.softAP(apSSID, apPassword);
    Serial.println("🔹 Hotspot indítva");
    Serial.println(WiFi.softAPIP());
}

//Betölt az index.html ha létezik, amint a hotspotra megyünk a böngészőben
void handleRoot() {
    if (!LittleFS.exists("/index.html")) {
        server.send(404, "text/plain", "❌ index.html nem található!");
        return;
    }

    File file = LittleFS.open("/index.html", "r");
    server.streamFile(file, "text/html");
    file.close();
}

//Wifi scannelése
void handleScan(){
    int networks= WiFi.scanNetworks();
    DynamicJsonDocument doc(1024);
    JsonArray wifiList = doc.createNestedArray("networks");
    for (int i = 0; i < networks; i++) {
        JsonObject network = wifiList.createNestedObject();
        network["ssid"] = WiFi.SSID(i);
    }

    String response;
    serializeJson(doc, response);
    server.send(200, "application/json", response);
}

void handleConnect(){
    if (server.method() != HTTP_POST) {
        server.send(405, "text/plain", "❌ Csak POST kérés engedélyezett!");
        return;
    }

    //JSONből kiszedés
    DynamicJsonDocument doc(256);
    deserializeJson(doc, server.arg("plain"));
    currentSSID = doc["ssid"].as<String>();
    currentPassword = doc["password"].as<String>();

    //Csatlakozás próbálása
    WiFi.begin(currentSSID.c_str(), currentPassword.c_str());
    //20-s próba
    int timeout = 20;
    while (WiFi.status() != WL_CONNECTED && timeout > 0) {
        delay(500);
        Serial.print(".");
        timeout--;
    }

    if (WiFi.status() == WL_CONNECTED) {
        wifiConnected = true;
        server.send(200, "text/plain", "✅ Sikeres csatlakozás: " + currentSSID);
    } else {
        wifiConnected = false;
        server.send(400, "text/plain", "❌ Sikertelen csatlakozás!");
    }

}



//Login API hívás
//itt küldjük el a json-t a szervernek, és várjuk a választ
void handleLogin() {
    if (server.method() != HTTP_POST) {
        server.send(405, "text/plain", "Csak POST kérés engedélyezett!");
        return;
    }
    //A JSON amit kapunk frontendtől
    String receivedJson = server.arg("plain");
    Serial.println("🔹 Megkapott JSON:");
    Serial.println(receivedJson);

    //Tovább küldjül a távoli API-nak
    HTTPClient http;
    http.begin("http://188.157.217.41/Auth/Login");  // Cél API cím
    http.addHeader("Content-Type", "application/json");

    int httpResponseCode = http.POST(receivedJson);  // Továbbküldjük a JSON-t a frontendnek

    if (httpResponseCode > 0) {
        String apiResponse = http.getString();
        Serial.println("🔹 API válasz:");
        Serial.println(apiResponse);
        if(httpResponseCode == 200) {
            server.send(200, "text/plain", "Sikeres bejelentkezés!");  // Válasz küldése a kliensnek

            //itt kiszedjük a token-t a válaszból
            DynamicJsonDocument doc(256);
            deserializeJson(doc, apiResponse);
            authToken = doc["token"].as<String>();
            Serial.println("🔹 Token: " + authToken);
        } else {
            server.send(httpResponseCode, "text/plain", "Hibás felhasználónév vagy jelszó!");
        }
    } else {
        Serial.println("❌ API hívási hiba!");
        Serial.println(http.errorToString(httpResponseCode));
    }

    http.end();

}



//Setup, amikor indul az esp32
void setup() {
    Serial.begin(115200);
    //littlefs betöltése
    if (!LittleFS.begin()) {
        Serial.println("❌ LittleFS indítása sikertelen!");
        return;
    }
    Serial.println("✅ LittleFS indítása sikeres.");
    //Hotspot indítása
    startAP();

    //várjuk a kéréseket a /-re
    server.on("/", handleRoot);
    //Wifi scannelés
    server.on("/scan", handleScan);

    //Wifire csatlakozás json adatból
    server.on("/connect", handleConnect);

    //Login API hívás
    server.on("/login", handleLogin);


    //így tölti be a style.csst, és a js-t
    server.on("/style.css", []() {
    File file = LittleFS.open("/style.css", "r");
    server.streamFile(file, "text/css");
    file.close();
    });
    server.on("/app.js", []() {
    File file = LittleFS.open("/app.js", "r");
    server.streamFile(file, "application/javascript");
    file.close();
    });

    
    server.begin();
    Serial.println("🌍 Webszerver elindult!");
}

void loop() {
    //várjuk a kéréseket
    server.handleClient();
}
