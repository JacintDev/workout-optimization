#include <WiFi.h>
#include <WebServer.h>
#include <LittleFS.h>
#include <ArduinoJson.h>

// Wi-Fi AP beállítások
const char *apSSID = "ESP32_Setup";
const char *apPassword = "12345678";

// Webszerver példány
WebServer server(80);

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
