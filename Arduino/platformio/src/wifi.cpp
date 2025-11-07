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

#include "MAX30105.h"

#include "config.h"
#include "state.h"
#include "wifi_portal.h"
#include "routes.h"
#include "ws_handlers.h"
#include "gyro.h"
#include "api.h"

//----------------------------------- PULSE (JAVÍTOTT)
MAX30105 particleSensor;
const byte RATE_SIZE = 8;
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
const unsigned long DEBOUNCE_TIME = 300;

// LED fényerő automatikus beállításához
byte currentLEDBrightness = 0x1F;
unsigned long lastAdjustTime = 0;
const unsigned long ADJUST_INTERVAL = 2000;

// *** JAVÍTOTT: Adaptív küszöbök ***
long adaptiveThresholdUp = 100;
long adaptiveThresholdDown = -100;
long maxDerivative = 0;
long minDerivative = 0;
unsigned long lastThresholdUpdate = 0;

// *** ÚJ: Validációs számláló ***
int validBeatCount = 0;

//-----------------

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
  Wire.begin(I2C_SDA, I2C_SCL);

  if (!particleSensor.begin(Wire, I2C_SPEED_FAST))
  {
    Serial.println("MAX30105 nem található! Ellenőrizd a bekötést.");
    Serial.println("SDA: pin 6, SCL: pin 7");
    while (1)
      ;
  }

  Serial.println("Szenzor beállítása...");

  byte sampleAverage = 4;
  byte ledMode = 2;
  int sampleRate = 100;
  int pulseWidth = 411;
  int adcRange = 4096;

  particleSensor.setup(currentLEDBrightness, sampleAverage, ledMode, sampleRate, pulseWidth, adcRange);
  particleSensor.setPulseAmplitudeRed(currentLEDBrightness);
  particleSensor.setPulseAmplitudeGreen(0);

  Serial.println("Tedd az ujjadat a szenzorra egyenletes nyomással!");
  Serial.println("A rendszer automatikusan beállítja a fényerőt...");

  delay(1000);

  setCpuFrequencyMhz(80);
  Serial.begin(115200);

  if (!LittleFS.begin())
  {
    Serial.println("❌ LittleFS indítása sikertelen!");
  }
  else
  {
    Serial.println("✅ LittleFS indítása sikeres.");
  }

  startAP();
  registerRoutes();
  server.begin();
  Serial.println("🌍 Webszerver elindult!");

  initGyro();
}

void adjustLEDBrightness(long irValue)
{
  if (millis() - lastAdjustTime < ADJUST_INTERVAL)
    return;
  lastAdjustTime = millis();

  bool adjusted = false;

  if (irValue > 260000)
  {
    if (currentLEDBrightness > 0x10)
    {
      currentLEDBrightness -= 0x08;
      adjusted = true;
      Serial.println("Telítődés! LED csökkentve.");
    }
  }
  else if (irValue < 100000 && irValue > 50000)
  {
    if (currentLEDBrightness < 0x80)
    {
      currentLEDBrightness += 0x08;
      adjusted = true;
      Serial.println("Gyenge jel! LED növelve.");
    }
  }

  if (adjusted)
  {
    particleSensor.setPulseAmplitudeRed(currentLEDBrightness);
    Serial.print("Új LED fényerő: 0x");
    Serial.println(currentLEDBrightness, HEX);

    bufferFull = false;
    bufferIndex = 0;
    validBeatCount = 0; // *** ÚJ ***

    for (int i = 0; i < RATE_SIZE; i++)
      rates[i] = 0;
    rateSpot = 0;
  }
}

// *** JAVÍTOTT: Működő simítás ***
long getSmoothedValue()
{
  if (!bufferFull && bufferIndex < 8)
  {
    return bufferIndex > 0 ? irBuffer[bufferIndex - 1] : 0;
  }

  long sum = 0;
  int count = bufferFull ? BUFFER_SIZE : bufferIndex;
  int windowSize = 8;
  int start = count > windowSize ? count - windowSize : 0;

  for (int i = start; i < count; i++)
  {
    int idx = i % BUFFER_SIZE;
    sum += irBuffer[idx];
  }

  int samples = count - start;
  return samples > 0 ? sum / samples : 0;
}

// *** JAVÍTOTT: Adaptív küszöbökkel ***
long getDerivative()
{
  if (bufferIndex < 3)
    return 0;

  int idx1, idx2, idx3;

  if (!bufferFull)
  {
    if (bufferIndex < 3)
      return 0;
    idx1 = bufferIndex - 1;
    idx2 = bufferIndex - 2;
    idx3 = bufferIndex - 3;
  }
  else
  {
    idx1 = (bufferIndex - 1 + BUFFER_SIZE) % BUFFER_SIZE;
    idx2 = (bufferIndex - 2 + BUFFER_SIZE) % BUFFER_SIZE;
    idx3 = (bufferIndex - 3 + BUFFER_SIZE) % BUFFER_SIZE;
  }

  long der = (irBuffer[idx1] - irBuffer[idx3]) / 2;

  // *** ÚJ: Adaptív küszöb frissítése 5 másodpercenként ***
  if (millis() - lastThresholdUpdate > 5000)
  {
    if (maxDerivative > 50)
    {
      adaptiveThresholdUp = maxDerivative * 0.4;
    }
    if (minDerivative < -50)
    {
      adaptiveThresholdDown = minDerivative * 0.4;
    }

    adaptiveThresholdUp = constrain(adaptiveThresholdUp, 80, 300);
    adaptiveThresholdDown = constrain(adaptiveThresholdDown, -300, -80);

    Serial.print("Új küszöbök: UP=");
    Serial.print(adaptiveThresholdUp);
    Serial.print(" DOWN=");
    Serial.println(adaptiveThresholdDown);

    maxDerivative = 0;
    minDerivative = 0;
    lastThresholdUpdate = millis();
  }
  else
  {
    maxDerivative = max(maxDerivative, der);
    minDerivative = min(minDerivative, der);
  }

  return der;
}

// *** JAVÍTOTT: Puhább validáció ***
bool isValidBPM(float bpm)
{
  // Első 3 beat: csak alapvető tartomány ellenőrzés
  if (validBeatCount < 3)
  {
    return (bpm >= MIN_BPM && bpm <= MAX_BPM);
  }

  if (beatsPerMinute == 0)
    return (bpm >= MIN_BPM && bpm <= MAX_BPM);

  // 30% eltérés megengedett (eredetileg 20% volt - túl szigorú!)
  float maxChange = beatsPerMinute * 0.30;
  return (bpm >= MIN_BPM && bpm <= MAX_BPM &&
          abs(bpm - beatsPerMinute) < maxChange);
}
int Timer = 0;
void sendPulseBpm(float bpm, long irValue)
{
  if (Timer<10)
  {
    Timer++;
    return;
  }
  
  if (bpm <= 0)
    return;
  if (irValue < 50000)
  {
    return;
  }
  else if (irValue > 260000)
  {
    return;
  }
  StaticJsonDocument<128> doc;
  doc["pulse"] = bpm;

  String body;
  serializeJson(doc, body);

  Serial.print("📤 Pulse JSON küldés előtt: ");
  Serial.println(body);

  String url = "http://" + IP_ADDRESS + "/Pulse/Post";

  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  if (authToken.length())
  {
    http.addHeader("Authorization", "Bearer " + authToken);
  }

  int code = http.POST(body);
  Serial.printf("✅ Pulse POST válaszkód: %d\n", code);

  http.end();
  Timer = 0;
}

void BpmCalculate()
{
  long irValue = particleSensor.getIR();

  adjustLEDBrightness(irValue);

  irBuffer[bufferIndex] = irValue;
  bufferIndex++;
  if (bufferIndex >= BUFFER_SIZE)
  {
    bufferIndex = 0;
    bufferFull = true;
  }

  if ((bufferFull || bufferIndex > 15) && irValue > 50000 && irValue < 260000)
  {
    long smoothed = getSmoothedValue();
    long derivative = getDerivative();

    unsigned long currentTime = millis();

    // *** JAVÍTOTT: Adaptív küszöbökkel ***
    if (!risingEdge && derivative > adaptiveThresholdUp)
    {
      risingEdge = true;
      Serial.print(" [EMELKEDIK]");
    }
    else if (risingEdge && derivative < adaptiveThresholdDown)
    {
      risingEdge = false;
      Serial.print(" [CSÖKKEN → BEAT?]");

      if (currentTime - lastBeatTime > DEBOUNCE_TIME)
      {
        long delta = currentTime - lastBeatTime;

        if (lastBeatTime > 0 && delta < (60000 / MIN_BPM))
        {
          float newBPM = 60000.0 / delta;

          // *** JAVÍTOTT: Új validáció ***
          if (isValidBPM(newBPM))
          {
            beatsPerMinute = newBPM;
            validBeatCount++; // *** ÚJ ***

            rates[rateSpot++] = (byte)beatsPerMinute;
            rateSpot %= RATE_SIZE;

            // Átlag számítás
            beatAvg = 0;
            int validSamples = 0;

            for (int x = 0; x < RATE_SIZE; x++)
            {
              if (rates[x] > 0)
              {
                beatAvg += rates[x];
                validSamples++;
              }
            }

            if (validSamples > 0)
            {
              beatAvg = beatAvg / validSamples;
            }

            Serial.print(" 💓 ÉRVÉNYES BEAT! Delta=");
            Serial.print(delta);
            Serial.print("ms");
          }
          else
          {
            Serial.print(" [INVALID: ");
            Serial.print(newBPM, 1);
            Serial.print(" BPM]");
          }
        }
        lastBeatTime = currentTime;
      }
      else
      {
        Serial.print(" [DEBOUNCE]");
      }
    }
  }

  // Kiírás
  // Serial.print("IR=");
  // Serial.print(irValue);
  // Serial.print(" | Sim=");
  // Serial.print(getSmoothedValue());
  // Serial.print(" | Der=");
  // Serial.print(getDerivative());
  // Serial.print(" | THR=");
  // Serial.print(adaptiveThresholdUp);
  // Serial.print("/");
  // Serial.print(adaptiveThresholdDown);
  // Serial.print(" | BPM=");
  // Serial.print(beatsPerMinute, 1);
  // Serial.print(" | Átlag=");
  // Serial.print(beatAvg);
  // Serial.print(" | LED=0x");
  // Serial.print(currentLEDBrightness, HEX);

  sendPulseBpm(beatsPerMinute, irValue);
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

    if (shouldSendPulse)
    {
      BpmCalculate();
      delay(10);
    }
  }

  delay(10);
}