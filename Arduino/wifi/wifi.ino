#include <WiFi.h>
#include <WebServer.h>
#include <LittleFS.h>

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

//Betölt az index.html ha létezik, amint a hotspotra megyünk
void handleRoot() {
    if (!LittleFS.exists("/index.html")) {
        server.send(404, "text/plain", "❌ index.html nem található!");
        return;
    }

    File file = LittleFS.open("/index.html", "r");
    server.streamFile(file, "text/html");
    file.close();
}

void setup() {
    Serial.begin(115200);

    if (!LittleFS.begin()) {
        Serial.println("❌ LittleFS indítása sikertelen!");
        return;
    }
    Serial.println("✅ LittleFS indítása sikeres.");

    startAP();

    server.on("/", handleRoot);
    server.begin();
    Serial.println("🌍 Webszerver elindult!");
}

void loop() {
    server.handleClient();
}
