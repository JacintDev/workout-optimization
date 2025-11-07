#include "routes.h"
#include "state.h"
#include "config.h"
#include "wifi_portal.h"
#include "ws_handlers.h"
#include <LittleFS.h>
#include <WiFi.h>
#include <ArduinoJson.h>

static void handleRoot() {
  if (!LittleFS.exists("/index.html")) {
    server.send(404, "text/plain", "❌ index.html nem található!");
    return;
  }
  File file = LittleFS.open("/index.html", "r");
  server.streamFile(file, "text/html");
  file.close();
}

static void handleScan() {
  int networks = WiFi.scanNetworks();
  StaticJsonDocument<1024> doc;
  JsonArray wifiList = doc.createNestedArray("networks");
  for (int i = 0; i < networks; i++) {
    JsonObject net = wifiList.createNestedObject();
    net["ssid"] = WiFi.SSID(i);
  }
  String response;
  serializeJson(doc, response);
  server.send(200, "application/json", response);
}

static void handleConnect() {
  if (server.method() != HTTP_POST) {
    server.send(405, "text/plain", "❌ Csak POST kérés engedélyezett!");
    return;
  }
  StaticJsonDocument<256> doc;
  deserializeJson(doc, server.arg("plain"));
  currentSSID    = doc["ssid"].as<String>();
  currentPassword= doc["password"].as<String>();

  WiFi.begin(currentSSID.c_str(), currentPassword.c_str());
  int timeout = 20;
  while (WiFi.status() != WL_CONNECTED && timeout-- > 0) {
    delay(500);
    Serial.print(".");
  }
  if (WiFi.status() == WL_CONNECTED) {
    wifiConnected = true;
    server.send(200, "text/plain", "✅ Sikeres csatlakozás: " + currentSSID);
  } else {
    wifiConnected = false;
    server.send(400, "text/plain", "❌ Sikertelen csatlakozás!");
  }
}

static void handleLogin() {
  if (server.method() != HTTP_POST) {
    server.send(405, "text/plain", "Csak POST kérés engedélyezett!");
    return;
  }
  String receivedJson = server.arg("plain");
  Serial.println("🔹 Megkapott JSON:");
  Serial.println(receivedJson);

  http.begin("http://" + IP_ADDRESS + "/Auth/Login");
  http.addHeader("Content-Type", "application/json");
  int code = http.POST(receivedJson);

  if (code > 0) {
    String apiResponse = http.getString();
    Serial.println("🔹 API válasz:");
    Serial.println(apiResponse);
    if (code == 200) {
      server.send(200, "text/plain", "Sikeres bejelentkezés!");

      StaticJsonDocument<256> doc;
      deserializeJson(doc, apiResponse);
      authToken = doc["token"].as<String>();
      Serial.println("🔹 Token: " + authToken);
      isLoggedIn = true;

      setupWebSocket();   // WS init
      stopConfigPortal(); // config portal OFF
    } else {
      server.send(code, "text/plain", "Hibás felhasználónév vagy jelszó!");
    }
  } else {
    Serial.println("❌ API hívási hiba!");
    Serial.println(http.errorToString(code));
  }
  http.end();
}

void registerRoutes() {
  server.on("/", handleRoot);
  server.on("/scan", handleScan);
  server.on("/connect", handleConnect);
  server.on("/login", handleLogin);

  // Statikus fájlok
  server.on("/style.css", [](){
    File f = LittleFS.open("/style.css", "r");
    server.streamFile(f, "text/css");
    f.close();
  });
  server.on("/app.js", [](){
    File f = LittleFS.open("/app.js", "r");
    server.streamFile(f, "application/javascript");
    f.close();
  });
}
