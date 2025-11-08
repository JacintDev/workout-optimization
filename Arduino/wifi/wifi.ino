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

// MAX30105-t most nem használjuk ebben a szétbontásban, de maradhat a projektben
#include "MAX30105.h"

#include "config.h"
#include "state.h"
#include "wifi_portal.h"
#include "routes.h"
#include "ws_handlers.h"
#include "gyro.h"
#include "api.h"

//----------------------------------- PULSE
MAX30105 particleSensor;
const byte RATE_SIZE = 8; // Növelve 4-ről 8-ra a simább átlagért
byte rates[RATE_SIZE];
byte rateSpot = 0;
long lastBeat = 0;
float beatsPerMinute = 0;
int beatAvg = 0;

// Szűréshez és beat detektáláshoz
const int BUFFER_SIZE = 50;
long irBuffer[BUFFER_SIZE];
int bufferIndex = 0;
bool bufferFull = false;

long lastIRValue = 0;
bool risingEdge = false;
unsigned long lastBeatTime = 0;
const int MIN_BPM = 40;
const int MAX_BPM = 180;
const unsigned long DEBOUNCE_TIME = 300; // 300ms-re növelve a jobb szűrésért

// LED fényerő automatikus beállításához
byte currentLEDBrightness = 0x1F;
unsigned long lastAdjustTime = 0;
const unsigned long ADJUST_INTERVAL = 2000;

// Derivált küszöbök finomhangolása
const long DERIVATIVE_THRESHOLD_UP = 200;   // Növelve 100-ról 200-ra
const long DERIVATIVE_THRESHOLD_DOWN = -200; // Növelve -100-ról -200-ra

// Adaptive threshold a jobb peak detektáláshoz
long maxDerivative = 0;
long minDerivative = 0;
unsigned long lastThresholdUpdate = 0;

//-----------------

// ====== Globális példányok definíciói (state.h-hoz) ======
WebServer       server(80);
HTTPClient      http;
WebSocketsClient webSocket;
MPU6050         mpu(Wire);

String currentSSID, currentPassword;
bool   wifiConnected = false;
String authToken = "";
bool   isLoggedIn = false;

int    trainingId = 0;
bool   isActiveTraining = false;

bool   shouldSend = false;
bool   shouldSendPulse = false;

float  filteredGyroX = 0, filteredGyroY = 0, filteredGyroZ = 0;
float  filteredAccX  = 0, filteredAccY  = 0, filteredAccZ  = 0;

unsigned long lastSent = 0;
// =========================================================

void setup() {
Wire.begin(I2C_SDA, I2C_SCL);
  if (!particleSensor.begin(Wire, I2C_SPEED_FAST))
  {
    Serial.println("MAX30105 nem található! Ellenőrizd a bekötést.");
    Serial.println("SDA: pin 6, SCL: pin 7");
    while (1);
  }
  
  Serial.println("Szenzor beállítása...");
  
  // Konzervatív kezdő beállítások
  byte sampleAverage = 4;
  byte ledMode = 2;          // Red + IR
  int sampleRate = 100;      // 100 Hz
  int pulseWidth = 411;      // 411 us
  int adcRange = 4096;       // 4096
  
  particleSensor.setup(currentLEDBrightness, sampleAverage, ledMode, sampleRate, pulseWidth, adcRange);
  particleSensor.setPulseAmplitudeRed(currentLEDBrightness);
  particleSensor.setPulseAmplitudeGreen(0);
  
  Serial.println("Tedd az ujjadat a szenzorra egyenletes nyomással!");
  Serial.println("A rendszer automatikusan beállítja a fényerőt...");
  
  delay(1000);

  
  setCpuFrequencyMhz(80);
  Serial.begin(115200);

  if (!LittleFS.begin()) {
    Serial.println("❌ LittleFS indítása sikertelen!");
    // Nem térünk vissza, de jelezzük
  } else {
    Serial.println("✅ LittleFS indítása sikeres.");
  }

  startAP();          // Hotspot indul
  registerRoutes();   // HTTP endpointok
  server.begin();
  Serial.println("🌍 Webszerver elindult!");

  initGyro();         // I2C + MPU init
}
void adjustLEDBrightness(long irValue) {
  if (millis() - lastAdjustTime < ADJUST_INTERVAL) return;
  lastAdjustTime = millis();
  
  bool adjusted = false;
  
  if (irValue > 260000) {
    if (currentLEDBrightness > 0x10) {
      currentLEDBrightness -= 0x08;
      adjusted = true;
      Serial.println("Telítődés! LED csökkentve.");
    }
  }
  else if (irValue < 100000 && irValue > 50000) {
    if (currentLEDBrightness < 0x80) {
      currentLEDBrightness += 0x08;
      adjusted = true;
      Serial.println("Gyenge jel! LED növelve.");
    }
  }
  
  if (adjusted) {
    particleSensor.setPulseAmplitudeRed(currentLEDBrightness);
    Serial.print("Új LED fényerő: 0x");
    Serial.println(currentLEDBrightness, HEX);
    
    bufferFull = false;
    bufferIndex = 0;
    
    // Reset beat tracking
    for (int i = 0; i < RATE_SIZE; i++) rates[i] = 0;
    rateSpot = 0;
  }
}

long getSmoothedValue() {
  if (!bufferFull && bufferIndex < 8) return irBuffer[bufferIndex - 1];
  
  long sum = 0;
  int count = bufferFull ? BUFFER_SIZE : bufferIndex;
  int start = count > 8 ? count - 8 : 0; // 8 mintás mozgóátlag
  
  for (int i = start; i < count; i++) {
    sum += irBuffer[i];
  }
  return sum / (count - start);
}

long getDerivative() {
  if (bufferIndex < 3) return 0;
  
  // 3 pontos derivált a simább működésért
  int idx1, idx2, idx3;
  
  if (!bufferFull) {
    idx1 = bufferIndex - 1;
    idx2 = bufferIndex - 2;
    idx3 = bufferIndex - 3;
  } else {
    idx1 = (bufferIndex - 1 + BUFFER_SIZE) % BUFFER_SIZE;
    idx2 = (bufferIndex - 2 + BUFFER_SIZE) % BUFFER_SIZE;
    idx3 = (bufferIndex - 3 + BUFFER_SIZE) % BUFFER_SIZE;
  }
  
  // Súlyozott derivált
  long der = (irBuffer[idx1] - irBuffer[idx3]) / 2;
  
  // Adaptív küszöb frissítése
  if (millis() - lastThresholdUpdate > 1000) {
    maxDerivative = max((long)(maxDerivative * 0.9), der);
    minDerivative = min((long)(minDerivative * 0.9), der);
    lastThresholdUpdate = millis();
  } else {
    maxDerivative = max(maxDerivative, der);
    minDerivative = min(minDerivative, der);
  }
  
  return der;
}

bool isValidBPM(float bpm, float previousBPM) {
  // Ha ez az első mérés
  if (previousBPM == 0) return (bpm >= MIN_BPM && bpm <= MAX_BPM);
  
  // Maximum 20% eltérés az előző értéktől
  float maxChange = previousBPM * 0.20;
  return (bpm >= MIN_BPM && bpm <= MAX_BPM && 
          abs(bpm - previousBPM) < maxChange);
}


void BpmCalculate(){
  
  long irValue = particleSensor.getIR();
  
  adjustLEDBrightness(irValue);
  
  irBuffer[bufferIndex] = irValue;
  bufferIndex++;
  if (bufferIndex >= BUFFER_SIZE) {
    bufferIndex = 0;
    bufferFull = true;
  }
  
  if ((bufferFull || bufferIndex > 15) && irValue > 50000 && irValue < 260000) {
    long smoothed = getSmoothedValue();
    long derivative = getDerivative();
    
    unsigned long currentTime = millis();
    
    // Peak detektálás finomított küszöbökkel
    if (!risingEdge && derivative > DERIVATIVE_THRESHOLD_UP) {
      risingEdge = true;
    }
    else if (risingEdge && derivative < DERIVATIVE_THRESHOLD_DOWN) {
      risingEdge = false;
      
      if (currentTime - lastBeatTime > DEBOUNCE_TIME) {
        long delta = currentTime - lastBeatTime;
        
        if (lastBeatTime > 0 && delta < (60000 / MIN_BPM)) {
          float newBPM = 60000.0 / delta;
          
          // Validálás az előző értékkel
          if (isValidBPM(newBPM, beatsPerMinute)) {
            beatsPerMinute = newBPM;
            
            rates[rateSpot++] = (byte)beatsPerMinute;
            rateSpot %= RATE_SIZE;
            
            // Átlag számítás súlyozással (újabbak nagyobb súllyal)
            beatAvg = 0;
            int validSamples = 0;
            float weightSum = 0;
            
            for (int x = 0; x < RATE_SIZE; x++) {
              if (rates[x] > 0) {
                int idx = (rateSpot - 1 - x + RATE_SIZE) % RATE_SIZE;
                float weight = 1.0 + (x * 0.1); // Újabb értékek nagyobb súllyal
                beatAvg += rates[idx] * weight;
                weightSum += weight;
                validSamples++;
              }
            }
            
            if (validSamples > 0) {
              beatAvg = (int)(beatAvg / weightSum);
            }
            
            
          }
        }
        lastBeatTime = currentTime;
      }
    }
  }
 //sendPulseBpm(beatsPerMinute);
 Serial.println(beatsPerMinute,0);
//  // Kiírás
  Serial.print("IR=");
  Serial.print(irValue);
  Serial.print(" | Sim=");
  Serial.print(getSmoothedValue());
  Serial.print(" | Der=");
  Serial.print(getDerivative());
  Serial.print(" | BPM=");
  Serial.print(beatsPerMinute, 1);
  Serial.print(" | Átlag=");
  Serial.print(beatAvg);
  Serial.print(" | LED=0x");
  Serial.print(currentLEDBrightness, HEX);
  Serial.println();
//  
//  if (irValue < 50000) {
//    Serial.print(" [NINCS UJJ]");
//  } else if (irValue > 260000) {
//    Serial.print(" [TELÍTETT!]");
//  }
//  
//  Serial.println();
//  }
}
static void sendPulseBpm(float bpm) {
  if (bpm <= 0) return; // csak értelmes értéket küldjünk

  StaticJsonDocument<128> doc;
  doc["pulse"] = bpm;  // amit küldesz (ha kell, ide jöhet trainingId is)

  String body;
  serializeJson(doc, body);

  // Serial log → hogy pontosan lásd, mit küldünk a szervernek
  Serial.print("📤 Pulse JSON küldés előtt: ");
  Serial.println(body);

  // Végpont: ha a te API-d pl. /Pulse/Post akkor ez jó
  String url = "http://" + IP_ADDRESS + "/Pulse/Post";

  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  if (authToken.length()) {
    http.addHeader("Authorization", "Bearer " + authToken);
  }

  int code = http.POST(body);
  Serial.printf("✅ Pulse POST válaszkód: %d\n", code);

  http.end();
}

void loop() {
  server.handleClient();

  if (isLoggedIn) {
    webSocketLoop();

    // Gyro adat küldése 100ms-enként, ha engedélyezett
    if (shouldSend && (millis() - lastSent > GYRO_SEND_MS)) {
      checkActiveTraining();           // frissítjük trainingId-t
      String msg = buildGyroscopeJson();
      webSocket.sendTXT(msg);
      Serial.println("Küldve: " + msg);
      lastSent = millis();
    }

    if (shouldSendPulse) {
      // Itt jönne a pulzus adatgyűjtés / küldés
     BpmCalculate();
      delay(10);
    }
  }

  delay(10);
}
