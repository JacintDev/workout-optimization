#include <Arduino.h>
#include <WiFi.h>
#include <WebServer.h>
#include <Wire.h>
#include <LittleFS.h>
#include <ArduinoJson.h>
#include <HTTPClient.h>
#include <MPU6050_tockn.h>
#include <WebSocketsClient.h>
#include "esp_system.h"

#include "config.h"
#include "state.h"
#include "wifi_portal.h"
#include "routes.h"
#include "ws_handlers.h"
#include "gyro.h"
#include "api.h"
#include "pulse.h"

// ====== Globális példányok definíciói (state.h-hoz) ======
WebServer server(80);
HTTPClient http;
WebSocketsClient webSocket;
MPU6050 mpu(Wire);

String currentSSID, currentPassword;
bool wifiConnected = false;
String authToken = "";
bool isLoggedIn = false;

int trainingId = 0;
bool isActiveTraining = false;

bool shouldSend = false;
bool shouldSendPulse = false;

float filteredGyroX = 0, filteredGyroY = 0, filteredGyroZ = 0;
float filteredAccX = 0, filteredAccY = 0, filteredAccZ = 0;

unsigned long lastSent = 0;
// =========================================================

void setup()
{
  Serial.begin(115200);
  setCpuFrequencyMhz(80);

  Wire.begin(I2C_SDA, I2C_SCL);

  if (!LittleFS.begin())
    Serial.println("❌ LittleFS indítása sikertelen!");
  else
    Serial.println("✅ LittleFS indítása sikeres.");

  startAP();
  registerRoutes();
  server.begin();
  Serial.println("🌍 Webszerver elindult!");

  initGyro();

  // --- PULSE modul ---
  if (!pulse::begin(Wire, I2C_SPEED_FAST)) {
    Serial.println("Pulse modul: MAX30105 nincs jelen vagy init hiba.");
  } else {
    pulse::setDebug(false); // opcionális
  }
}

void loop()
{
  server.handleClient();

  if (isLoggedIn)
  {
    webSocketLoop();

    // Gyro adat küldése 100ms-enként, ha engedélyezett
    if (shouldSend && (millis() - lastSent > GYRO_SEND_MS))
    {
      checkActiveTraining();
      String msg = buildGyroscopeJson();
      webSocket.sendTXT(msg);
      Serial.println("Küldve: " + msg);
      lastSent = millis();
    }

    // Pulzusmérés – CSAK, ha a WS engedélyezte
    if (shouldSendPulse)
    {
      pulse::update();   // no-op nincs, mert csak akkor hívjuk, ha tényleg kell
    }
  }

  delay(10);
}
