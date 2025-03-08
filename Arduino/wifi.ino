#include <WiFi.h>
#include <WebServer.h>
#include <LittleFS.h>
#include <ArduinoJson.h>
#include <Wire.h>
#include <HTTPClient.h>
#include <MPU6050_tockn.h>

#define NUM_SAMPLES 3  

const char *apSSID = "ESP32_Setup";
const char *apPassword = "12345678";

WebServer server(80);
MPU6050 mpu(Wire);

const char* serverURL = "http://188.157.206.94/api/GyroscopeData";

float accX_samples[NUM_SAMPLES] = {0}, accY_samples[NUM_SAMPLES] = {0}, accZ_samples[NUM_SAMPLES] = {0};
float gyroX_samples[NUM_SAMPLES] = {0}, gyroY_samples[NUM_SAMPLES] = {0}, gyroZ_samples[NUM_SAMPLES] = {0};

bool wifiConnected = false;
String currentSSID, currentPassword;

void startAP() {
    WiFi.softAP(apSSID, apPassword);
    Serial.println("🔹 Hotspot indítva");
    Serial.println(WiFi.softAPIP());
}

void handleRoot() {
    File file = LittleFS.open("/index.html", "r");
    if (!file) {
        server.send(404, "text/plain", "❌ index.html nem található!");
        return;
    }
    server.streamFile(file, "text/html");
    file.close();
}

void handleScan() {
    int networks = WiFi.scanNetworks();
    DynamicJsonDocument doc(1024);
    JsonArray wifiList = doc.createNestedArray("networks");

    for (int i = 0; i < networks; i++) {
        JsonObject network = wifiList.createNestedObject();
        network["ssid"] = WiFi.SSID(i);
        network["rssi"] = WiFi.RSSI(i);
    }
    
    String response;
    serializeJson(doc, response);
    server.send(200, "application/json", response);
}

void handleConnect() {
    if (server.method() != HTTP_POST) {
        server.send(405, "text/plain", "❌ Csak POST kérés engedélyezett!");
        return;
    }

    DynamicJsonDocument doc(256);
    deserializeJson(doc, server.arg("plain"));
    currentSSID = doc["ssid"].as<String>();
    currentPassword = doc["password"].as<String>();

    WiFi.begin(currentSSID.c_str(), currentPassword.c_str());
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

void sendDataToAPI(float ax, float ay, float az, float gx, float gy, float gz) {
    if (!wifiConnected || WiFi.status() != WL_CONNECTED) {
        Serial.println("❌ Wi-Fi kapcsolat megszakadt!");
        wifiConnected = false;
        return;
    }

    HTTPClient http;
    http.begin(serverURL);
    http.addHeader("Content-Type", "application/json");

    StaticJsonDocument<200> jsonDoc;
    jsonDoc["AccelX"] = ax;
    jsonDoc["AccelY"] = ay;
    jsonDoc["AccelZ"] = az;
    jsonDoc["GyrosX"] = gx;
    jsonDoc["GyrosY"] = gy;
    jsonDoc["GyrosZ"] = gz;

    String jsonString;
    serializeJson(jsonDoc, jsonString);
    Serial.println("JSON adat küldése:");
    Serial.println(jsonString);

    int httpResponseCode = http.POST(jsonString);

    if (httpResponseCode > 0) {
        Serial.print("HTTP válaszkód: ");
        Serial.println(httpResponseCode);
    } else {
        Serial.print("HTTP kérés hiba: ");
        Serial.println(httpResponseCode);
    }
    http.end();
}

float smoothData(float *samples, float newValue) {
    float sum = 0;
    for (int i = 1; i < NUM_SAMPLES; i++) {
        samples[i - 1] = samples[i];  
        sum += samples[i - 1];
    }
    samples[NUM_SAMPLES - 1] = newValue;
    sum += newValue;
    return sum / NUM_SAMPLES;
}

void setup() {
    Serial.begin(115200);
    Wire.begin(6, 7);

    if (!LittleFS.begin()) {
        Serial.println("❌ LittleFS sikertelen!");
        return;
    }
    Serial.println("✅ LittleFS sikeresen csatlakoztatva.");

    startAP();
    
    server.on("/", handleRoot);
    server.on("/scan", handleScan);
    server.on("/connect", handleConnect);
    server.begin();
    Serial.println("🌍 Webszerver elindult!");

    mpu.begin();
    mpu.calcGyroOffsets(true);
}

void loop() {
    server.handleClient();
    if (wifiConnected && WiFi.status() == WL_CONNECTED) {
        mpu.update();

        float smoothAccX = smoothData(accX_samples, mpu.getAccX());
        float smoothAccY = smoothData(accY_samples, mpu.getAccY());
        float smoothAccZ = smoothData(accZ_samples, mpu.getAccZ());
        float smoothGyroX = smoothData(gyroX_samples, mpu.getGyroX());
        float smoothGyroY = smoothData(gyroY_samples, mpu.getGyroY());
        float smoothGyroZ = smoothData(gyroZ_samples, mpu.getGyroZ());

        sendDataToAPI(smoothAccX, smoothAccY, smoothAccZ, smoothGyroX, smoothGyroY, smoothGyroZ);
    }
    delay(30);
}
