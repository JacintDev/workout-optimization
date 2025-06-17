#include <WiFi.h>
#include <WebServer.h>
#include <Wire.h>
#include <LittleFS.h>
#include <ArduinoJson.h>
#include <HTTPClient.h>
#include "esp_system.h"
#include <MPU6050_tockn.h>
#include <WebSocketsClient.h>


// Wi-Fi AP beállítások
const char *apSSID = "ESP32_Setup";
const char *apPassword = "12345678";
String ip_address="84.3.231.158";

// Webszerver példány
WebServer server(80);
//Gyroscope
MPU6050 mpu(Wire);
float filteredGyroX = 0, filteredGyroY = 0, filteredGyroZ = 0;
float filteredAccX = 0, filteredAccY = 0, filteredAccZ = 0;
float alpha = 0.3;  // szűrés mértéke (0.0 - 1.0)

//websocket
WebSocketsClient webSocket;
bool shouldSend = false;


void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {
  switch(type) {
    case WStype_CONNECTED:
      Serial.println("WebSocket connected!");
      break;
    case WStype_DISCONNECTED:
      Serial.println("WebSocket disconnected!");
      shouldSend = false;
      break;
    case WStype_TEXT:
      Serial.printf("[Server]: %s\n", payload);

      if (strcmp((char*)payload, "start") == 0) {
        shouldSend = true;
        mpu.calcGyroOffsets(true);
        Serial.println(">> Indul az adatküldés");
      } else if (strcmp((char*)payload, "stop") == 0) {
        shouldSend = false;
        Serial.println(">> Leáll az adatküldés");
      }
      break;
  }
}


String currentSSID, currentPassword;
bool wifiConnected = false;
String authToken="";
bool isLoggedIn = false;
HTTPClient http;
int trainingId=0;
bool isActiveTraining = false;

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
    
    http.begin("http://"+ip_address+"/Auth/Login");  // Cél API cím
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
            isLoggedIn = true;
              webSocket.begin(ip_address, 80, "/Websocket/connect"); // vagy IP cím
              webSocket.onEvent(webSocketEvent);
              webSocket.setReconnectInterval(5000); // újracsatlakozás, ha kell
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
    setCpuFrequencyMhz(80);
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

    //Gyroscope init
    Wire.begin(6, 7);
    mpu.begin();
    mpu.calcGyroOffsets(true);

    
  //webSocket.begin(ip_address, 80, "/Websocket/connect"); // vagy IP cím
  //webSocket.onEvent(webSocketEvent);
  //webSocket.setReconnectInterval(5000); // újracsatlakozás, ha kell
}

void IsActiveRequest(){
    http.begin("http://"+ip_address+"/Training/GetActiveTraining");  // Cél API cím
    http.addHeader("Content-Type", "application/json");
    http.addHeader("Authorization", "Bearer " + authToken);  // Token hozzáadása a kéréshez
    int httpResponseCode = http.GET();  // GET kérés küldése
    if (httpResponseCode > 0) {
        String apiResponse = http.getString();
        Serial.println("🔹 API válasz:");
        Serial.println(apiResponse);
        if(httpResponseCode == 200) {
        
            //itt kiszedjük a token-t a válaszból
            DynamicJsonDocument doc(256);
            deserializeJson(doc, apiResponse);
            trainingId = doc["trainingId"].as<int>();
            isActiveTraining = true;
            
        } else {
           Serial.println("Nincs aktív training!");
           isActiveTraining = false;
        }
    } else {
        Serial.println("❌ API hívási hiba!");
        Serial.println(http.errorToString(httpResponseCode));
    }

    http.end();
}


String sendGyroscopeData(){
//    http.begin("http://"+ip_address+"/api/GyroscopeData");  // Cél API cím
//    http.addHeader("Content-Type", "application/json");
//    http.addHeader("Authorization", "Bearer " + authToken);  // Token hozzáadása a kéréshez

    // JSON dokumentum létrehozása
    DynamicJsonDocument doc(512);
    mpu.update();

//Szűrés
    float rawGyroX = mpu.getGyroX();
    float rawGyroY = mpu.getGyroY();
    float rawGyroZ = mpu.getGyroZ();

    float rawAccX = mpu.getAccX();
    float rawAccY = mpu.getAccY();
    float rawAccZ = mpu.getAccZ();

    filteredGyroX = alpha * rawGyroX + (1 - alpha) * filteredGyroX;
    filteredGyroY = alpha * rawGyroY + (1 - alpha) * filteredGyroY;
    filteredGyroZ = alpha * rawGyroZ + (1 - alpha) * filteredGyroZ;

    filteredAccX = alpha * rawAccX + (1 - alpha) * filteredAccX;
    filteredAccY = alpha * rawAccY + (1 - alpha) * filteredAccY;
    filteredAccZ = alpha * rawAccZ + (1 - alpha) * filteredAccZ;

    // 🔒 Statikus detektálás küszöbérték
    float gyroThreshold = 0.5;
    float accThreshold = 0.1;

    // Ha a mozgás kisebb, mint a küszöb – tekintsd nyugalomnak
    if (abs(filteredGyroX) < gyroThreshold) filteredGyroX = 0;
    if (abs(filteredGyroY) < gyroThreshold) filteredGyroY = 0;
    if (abs(filteredGyroZ) < gyroThreshold) filteredGyroZ = 0;

    if (abs(filteredAccX) < accThreshold) filteredAccX = 0;
    if (abs(filteredAccY) < accThreshold) filteredAccY = 0;
    if (abs(filteredAccZ) < accThreshold) filteredAccZ = 0;



    
    doc["accelX"] = filteredAccX;
    doc["accelY"] = filteredAccY;
    doc["accelZ"] = filteredAccZ;
    doc["gyrosX"] = filteredGyroX;
    doc["gyrosY"] = filteredGyroY;
    doc["gyrosZ"] = filteredGyroZ;




//    doc["accelX"] = 0;
//    doc["accelY"] = 0;
//    doc["accelZ"] = 0;
//    doc["gyrosX"] = 0;
//    doc["gyrosY"] = 0;
//    doc["gyrosZ"] = 0;
    doc["trainingId"] = trainingId;

    // JSON string létrehozása
    String requestBody;
    serializeJson(doc, requestBody);
    Serial.println(requestBody);
    return requestBody;

//    // POST kérés küldése
//    int httpResponseCode = http.POST(requestBody);
//
//    if (httpResponseCode > 0) {
//        String apiResponse = http.getString();
//        Serial.println("🔹 API válasz:");
//        Serial.println(apiResponse);
//    } else {
//        Serial.println("❌ API hívási hiba!");
//        Serial.println(http.errorToString(httpResponseCode));
//    }
//
//    http.end();
}



unsigned long lastActiveCheckTime = 0;
unsigned long lastGyroSendTime = 0;
unsigned long lastSent = 0;

void loop() {
    server.handleClient();  // klienskérések kezelése

if(isLoggedIn){
 webSocket.loop();

  if (shouldSend && millis() - lastSent > 100) {
    IsActiveRequest();
    
    String msg = sendGyroscopeData();
    webSocket.sendTXT(msg);
    Serial.println("Küldve: " + msg);
    lastSent = millis();
  }
}
delay(10);

//    unsigned long now = millis();

//    if (isLoggedIn) {
//        // 500 ms-onként aktív tréning lekérdezése
//        if (now - lastActiveCheckTime >= 1000) {
//            IsActiveRequest();  // ez állítja be az isActiveTraining változót
//            lastActiveCheckTime = now;
//            uint8_t temp_farenheit = temperatureRead();
//            Serial.println(temp_farenheit);  // kb. 40–70 °C lehet
//        }
//
//        // Ha van aktív tréning, 100 ms-onként küldjön giroszkóp adatokat
//        if (isActiveTraining && now - lastGyroSendTime >= 100) {
//            sendGyroscopeData();
//            lastGyroSendTime = now;
//        }
//    }
//    delay(10);
}
