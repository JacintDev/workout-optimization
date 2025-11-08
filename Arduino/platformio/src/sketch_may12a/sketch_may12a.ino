#include <Wire.h>
#include "MAX30100_PulseOximeter.h"

#define REPORTING_PERIOD_MS 2000  // 2 másodpercenként írunk ki új értéket

PulseOximeter pox;
uint32_t tsLastReport = 0;

void onBeatDetected() {
  Serial.println("♥ Beat!");
}

void setup() {
  Serial.begin(115200);
  Wire.begin(6, 7); // SDA, SCL

  Serial.println("Initializing pulse oximeter...");
  if (!pox.begin()) {
    Serial.println("FAILED to initialize pulse oximeter");
    while (1);
  } else {
    Serial.println("Pulse oximeter initialized");
  }

  pox.setIRLedCurrent(MAX30100_LED_CURR_7_6MA); // kisebb áram = kevesebb zaj
  pox.setOnBeatDetectedCallback(onBeatDetected);
}

void loop() {
  pox.update();  // kötelező minden ciklusban meghívni!

  if (millis() - tsLastReport > REPORTING_PERIOD_MS) {
    tsLastReport = millis();
    
    float hr = pox.getHeartRate();
    float spo2 = pox.getSpO2();

    Serial.print("Heart rate: ");
    Serial.print(hr);
    Serial.print(" bpm / SpO2: ");
    Serial.print(spo2);
    Serial.println(" %");
  }
}
