#include "wifi_portal.h"
#include "state.h"
#include "config.h"
#include <WiFi.h>

void startAP() {
  WiFi.softAP(AP_SSID, AP_PASSWORD);
  Serial.println("🔹 Hotspot indítva");
  Serial.println(WiFi.softAPIP());
}

void stopConfigPortal() {
  server.stop();               // webszerver leáll
  WiFi.softAPdisconnect(true); // AP OFF
  WiFi.mode(WIFI_STA);         // csak STA
  Serial.println("Config portal OFF, STA only");
}
