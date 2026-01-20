#include "api.h"
#include "state.h"
#include "config.h"
#include <ArduinoJson.h>

void checkActiveTraining() {
  http.begin("http://" + IP_ADDRESS + ":" + PORT + "/Training/GetActiveTraining");
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + authToken);

  int code = http.GET();
  if (code > 0) {
    String resp = http.getString();
    Serial.println("🔹 API válasz:");
    Serial.println(resp);
    if (code == 200) {
      StaticJsonDocument<256> doc;
      deserializeJson(doc, resp);
      trainingId = doc["trainingId"].as<int>();
      isActiveTraining = true;
    } else {
      Serial.println("Nincs aktív training!");
      isActiveTraining = false;
    }
  } else {
    Serial.println("❌ API hívási hiba!");
    Serial.println(http.errorToString(code));
  }
  http.end();
}
