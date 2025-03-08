#include <WiFi.h>
#include <WebServer.h>
#include <SPIFFS.h>  // SPIFFS fájlrendszer

const char *ap_ssid = "ESP32_Hotspot";
const char *ap_password = "";

WebServer server(80);

void setup() {
    Serial.begin(115200);

    // SPIFFS inicializálása
    if (!SPIFFS.begin(true)) {
        Serial.println("Hiba a SPIFFS indításakor!");
        return;
    }

    WiFi.mode(WIFI_AP);
    WiFi.softAP(ap_ssid, ap_password);
    Serial.print("Hotspot fut: ");
    Serial.println(WiFi.softAPIP());

    server.on("/", HTTP_GET, handleRoot);
    server.on("/scan", HTTP_GET, handleWiFiScan);
    server.on("/connect", HTTP_POST, handleWiFiConnect);
    server.begin();
}

void loop() {
    server.handleClient();
}

// Weboldal betöltése SPIFFS-ből
void handleRoot() {
    File file = SPIFFS.open("/index.html", "r");
    if (!file) {
        server.send(500, "text/plain", "Hiba: Nem található az index.html");
        return;
    }
    String page = file.readString();
    server.send(200, "text/html", page);
    file.close();
}

// Wi-Fi hálózatok keresése
void handleWiFiScan() {
    int n = WiFi.scanNetworks();
    String list = "<ul>";
    for (int i = 0; i < n; i++) {
        list += "<li><button onclick=\"connect('" + WiFi.SSID(i) + "')\">" + WiFi.SSID(i) + " (" + String(WiFi.RSSI(i)) + " dBm)</button></li>";
    }
    list += "</ul>";
    server.send(200, "text/html", list);
}

// Wi-Fi csatlakozás
void handleWiFiConnect() {
    String body = server.arg("plain");
    body.replace("\"", "");
    body.replace("{", "");
    body.replace("}", "");
    body.replace("ssid:", "");
    body.replace("password:", "");

    int sep = body.indexOf(",");
    String ssid = body.substring(0, sep);
    String password = body.substring(sep + 1);

    Serial.print("Csatlakozás a következő hálózatra: ");
    Serial.println(ssid);

    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid.c_str(), password.c_str());

    int timeout = 10;
    while (WiFi.status() != WL_CONNECTED && timeout > 0) {
        delay(1000);
        Serial.print(".");
        timeout--;
    }

    if (WiFi.status() == WL_CONNECTED) {
        Serial.println("\nSikeres csatlakozás!");
        Serial.print("IP cím: ");
        Serial.println(WiFi.localIP());
        server.send(200, "text/plain", "Sikeres Wi-Fi csatlakozás: " + ssid);
    } else {
        Serial.println("\nCsatlakozás sikertelen!");
        server.send(400, "text/plain", "Sikertelen csatlakozás!");
    }
}
