#include "pulse.h"
#include <ArduinoJson.h>
#include <HTTPClient.h>
#include "config.h"
#include "state.h"

namespace {

MAX30105 particleSensor;

// ---- Belső állapot ----
constexpr byte RATE_SIZE = 8;
byte rates[RATE_SIZE] = {0};
byte rateSpot = 0;

long lastBeatTime = 0;
float g_bpm = 0.f;
int   g_avg = 0;

constexpr int BUFFER_SIZE = 50;
long irBuffer[BUFFER_SIZE];
int  bufferIndex = 0;
bool bufferFull  = false;

byte currentLEDBrightness = 0x1F;
unsigned long lastAdjustTime = 0;
constexpr unsigned long ADJUST_INTERVAL = 2000;

long maxDerivative = 0;
long minDerivative = 0;
unsigned long lastThresholdUpdate = 0;
long adaptiveThresholdUp = 100;
long adaptiveThresholdDown = -100;

int  validBeatCount = 0;

constexpr int MIN_BPM = 40;
constexpr int MAX_BPM = 180;
constexpr unsigned long DEBOUNCE_TIME = 300;

bool debugOn = false;
int postTimer = 0;

inline void dlog(const String& s){ if (debugOn) Serial.println(s); }

void adjustLEDBrightness(long irValue) {
  if (millis() - lastAdjustTime < ADJUST_INTERVAL) return;
  lastAdjustTime = millis();

  bool adjusted = false;
  if (irValue > 260000) {
    if (currentLEDBrightness > 0x10) {
      currentLEDBrightness -= 0x08;
      adjusted = true;
      dlog("Telítődés! LED csökkentve.");
    }
  } else if (irValue < 100000 && irValue > 50000) {
    if (currentLEDBrightness < 0x80) {
      currentLEDBrightness += 0x08;
      adjusted = true;
      dlog("Gyenge jel! LED növelve.");
    }
  }

  if (adjusted) {
    particleSensor.setPulseAmplitudeRed(currentLEDBrightness);
    if (debugOn) {
      Serial.print("Új LED fényerő: 0x");
      Serial.println(currentLEDBrightness, HEX);
    }
    bufferFull = false;
    bufferIndex = 0;
    validBeatCount = 0;
    memset(rates, 0, sizeof(rates));
    rateSpot = 0;
  }
}

long getSmoothedValue() {
  if (!bufferFull && bufferIndex < 8) {
    return bufferIndex > 0 ? irBuffer[bufferIndex - 1] : 0;
  }
  long sum = 0;
  const int count = bufferFull ? BUFFER_SIZE : bufferIndex;
  const int windowSize = 8;
  const int start = count > windowSize ? count - windowSize : 0;

  for (int i = start; i < count; ++i) {
    const int idx = i % BUFFER_SIZE;
    sum += irBuffer[idx];
  }
  const int samples = count - start;
  return samples > 0 ? sum / samples : 0;
}

long getDerivative() {
  if (bufferIndex < 3) return 0;

  int idx1, idx2, idx3;
  if (!bufferFull) {
    if (bufferIndex < 3) return 0;
    idx1 = bufferIndex - 1;
    idx2 = bufferIndex - 2;
    idx3 = bufferIndex - 3;
  } else {
    idx1 = (bufferIndex - 1 + BUFFER_SIZE) % BUFFER_SIZE;
    idx2 = (bufferIndex - 2 + BUFFER_SIZE) % BUFFER_SIZE;
    idx3 = (bufferIndex - 3 + BUFFER_SIZE) % BUFFER_SIZE;
  }

  long der = (irBuffer[idx1] - irBuffer[idx3]) / 2;

  // Adaptív küszöb frissítés 5 mp-enként
  if (millis() - lastThresholdUpdate > 5000) {
    if (maxDerivative > 50)  adaptiveThresholdUp   = maxDerivative * 0.4;
    if (minDerivative < -50) adaptiveThresholdDown = minDerivative * 0.4;

    adaptiveThresholdUp   = constrain(adaptiveThresholdUp,   80,  300);
    adaptiveThresholdDown = constrain(adaptiveThresholdDown, -300, -80);

    if (debugOn) {
      Serial.print("Új küszöbök: UP=");
      Serial.print(adaptiveThresholdUp);
      Serial.print(" DOWN=");
      Serial.println(adaptiveThresholdDown);
    }

    maxDerivative = 0;
    minDerivative = 0;
    lastThresholdUpdate = millis();
  } else {
    maxDerivative = max(maxDerivative, der);
    minDerivative = min(minDerivative, der);
  }
  return der;
}

bool isValidBPM(float bpm) {
  if (validBeatCount < 3) return (bpm >= MIN_BPM && bpm <= MAX_BPM);
  if (g_bpm == 0)         return (bpm >= MIN_BPM && bpm <= MAX_BPM);
  const float maxChange = g_bpm * 0.30f; // megengedőbb
  return (bpm >= MIN_BPM && bpm <= MAX_BPM && fabs(bpm - g_bpm) < maxChange);
}

void sendPulseBpm(float bpm, long irValue) {
  if (postTimer < 10) { postTimer++; return; }  // egyszerű rate limit
  if (bpm <= 0) return;
  if (irValue < 50000) return;
  if (irValue > 260000) return;

  StaticJsonDocument<128> doc;
  doc["pulse"] = bpm;

  String body;
  serializeJson(doc, body);

  if (debugOn) {
    Serial.print("📤 Pulse JSON küldés előtt: ");
    Serial.println(body);
  }

  String url = String("http://") + IP_ADDRESS + "/Pulse/Post";
  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  if (authToken.length()) {
    http.addHeader("Authorization", "Bearer " + authToken);
  }

  int code = http.POST(body);
  if (debugOn) {
    Serial.printf("✅ Pulse POST válaszkód: %d\n", code);
  }
  http.end();
  postTimer = 0;
}

} // namespace

namespace pulse {

bool begin(TwoWire& wire, uint32_t i2cSpeed) {
  if (!particleSensor.begin(wire, i2cSpeed)) {
    Serial.println("MAX30105 nem található! Ellenőrizd a bekötést.");
    Serial.println("SDA: pin 6, SCL: pin 7");
    return false;
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
  return true;
}

void update() {
  // FIGYELEM: ezt csak akkor hívd, ha shouldSendPulse == true
  long irValue = particleSensor.getIR();

  adjustLEDBrightness(irValue);

  irBuffer[bufferIndex] = irValue;
  bufferIndex++;
  if (bufferIndex >= BUFFER_SIZE) {
    bufferIndex = 0;
    bufferFull = true;
  }

  static bool risingEdge = false;

  if ((bufferFull || bufferIndex > 15) && irValue > 50000 && irValue < 260000) {
    (void)getSmoothedValue(); // jelenleg csak a deriváltat használjuk
    long derivative  = getDerivative();
    unsigned long now = millis();

    if (!risingEdge && derivative > adaptiveThresholdUp) {
      risingEdge = true;
      if (debugOn) Serial.print(" [EMELKEDIK]");
    } else if (risingEdge && derivative < adaptiveThresholdDown) {
      risingEdge = false;
      if (debugOn) Serial.print(" [CSÖKKEN → BEAT?]");

      if (now - lastBeatTime > DEBOUNCE_TIME) {
        long delta = now - lastBeatTime;

        if (lastBeatTime > 0 && delta < (60000 / MIN_BPM)) {
          float newBPM = 60000.0f / float(delta);

          if (isValidBPM(newBPM)) {
            g_bpm = newBPM;
            validBeatCount++;

            rates[rateSpot++] = (byte)g_bpm;
            rateSpot %= RATE_SIZE;

            g_avg = 0;
            int valid = 0;
            for (byte x = 0; x < RATE_SIZE; x++) {
              if (rates[x] > 0) { g_avg += rates[x]; valid++; }
            }
            if (valid > 0) g_avg /= valid;

            if (debugOn) {
              Serial.print(" 💓 ÉRVÉNYES BEAT! Delta=");
              Serial.print(delta);
              Serial.print("ms");
            }
          } else if (debugOn) {
            Serial.print(" [INVALID: ");
            Serial.print(newBPM, 1);
            Serial.print(" BPM]");
          }
        }
        lastBeatTime = now;
      } else {
        if (debugOn) Serial.print(" [DEBOUNCE]");
      }
    }
  }

  // HTTP POST (rate-limitelve)
  sendPulseBpm(g_bpm, irValue);
}

float currentBPM() { return g_bpm; }
int   averageBPM() { return g_avg; }

void setLedBrightness(uint8_t red) {
  currentLEDBrightness = red;
  particleSensor.setPulseAmplitudeRed(currentLEDBrightness);
}

void setDebug(bool on) { debugOn = on; }

} // namespace pulse
