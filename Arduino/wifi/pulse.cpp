#include "pulse.h"
#include <Wire.h>
#include "MAX30105.h"

// ======= EREDETI KÓD BECOMPOSZOLVA =======

// ESP32 I2C pin definíciók
#define I2C_SDA 6
#define I2C_SCL 7

static MAX30105 particleSensor;

static const byte RATE_SIZE = 8; // Növelve 4-ről 8-ra a simább átlagért
static byte rates[RATE_SIZE];
static byte rateSpot = 0;
static long lastBeat = 0;
static float beatsPerMinute = 0;
static int beatAvg = 0;

// Szűréshez és beat detektáláshoz
static const int BUFFER_SIZE = 50;
static long irBuffer[BUFFER_SIZE];
static int bufferIndex = 0;
static bool bufferFull = false;

static long lastIRValue = 0;
static bool risingEdge = false;
static unsigned long lastBeatTime = 0;
static const int MIN_BPM = 40;
static const int MAX_BPM = 180;
static const unsigned long DEBOUNCE_TIME = 300; // 300ms-re növelve a jobb szűrésért

// LED fényerő automatikus beállításához
static byte currentLEDBrightness = 0x1F;
static unsigned long lastAdjustTime = 0;
static const unsigned long ADJUST_INTERVAL = 2000;

// Derivált küszöbök finomhangolása
static const long DERIVATIVE_THRESHOLD_UP   = 200;   // Növelve 100-ról 200-ra
static const long DERIVATIVE_THRESHOLD_DOWN = -200;  // Növelve -100-ról -200-ra

// Adaptive threshold a jobb peak detektáláshoz
static long maxDerivative = 0;
static long minDerivative = 0;
static unsigned long lastThresholdUpdate = 0;

// ---------- EREDETI FUNKCIÓK STATIKUSKÉNT ----------

static void adjustLEDBrightness(long irValue) {
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

static long getSmoothedValue() {
  // EREDETI LOGIKA + apró védelem: ha még túl kevés minta, vegyük az utolsót biztonságosan
  if (!bufferFull && bufferIndex < 8) {
    int safeIdx = bufferIndex > 0 ? bufferIndex - 1 : 0;
    return irBuffer[safeIdx];
  }

  long sum = 0;
  int count = bufferFull ? BUFFER_SIZE : bufferIndex;
  int start = count > 8 ? count - 8 : 0; // 8 mintás mozgóátlag

  for (int i = start; i < count; i++) {
    sum += irBuffer[i];
  }
  int denom = (count - start);
  return denom > 0 ? (sum / denom) : 0;
}

static long getDerivative() {
  if (bufferIndex < 3 && !bufferFull) return 0;

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

static bool isValidBPM(float bpm, float previousBPM) {
  // Ha ez az első mérés
  if (previousBPM == 0) return (bpm >= MIN_BPM && bpm <= MAX_BPM);

  // Maximum 20% eltérés az előző értéktől
  float maxChange = previousBPM * 0.20f;
  return (bpm >= MIN_BPM && bpm <= MAX_BPM && fabs(bpm - previousBPM) < maxChange);
}

// ---------- NYILVÁNOS FÜGGVÉNYEK (INTERFÉSZ) ----------

void pulseSetup() {
  Serial.begin(115200);
  Serial.println("Inicializálás...");

  // ESP32 I2C inicializálás egyedi pinekkel
  Wire.begin(I2C_SDA, I2C_SCL);

  if (!particleSensor.begin(Wire, I2C_SPEED_FAST)) {
    Serial.println("MAX30105 nem található! Ellenőrizd a bekötést.");
    Serial.println("SDA: pin 6, SCL: pin 7");
    while (1) { delay(1000); }
  }

  Serial.println("Szenzor beállítása...");

  // Konzervatív kezdő beállítások
  byte sampleAverage = 4;
  byte ledMode = 2;          // Red + IR
  int  sampleRate = 100;     // 100 Hz
  int  pulseWidth = 411;     // 411 us
  int  adcRange = 4096;      // 4096

  particleSensor.setup(currentLEDBrightness, sampleAverage, ledMode, sampleRate, pulseWidth, adcRange);
  particleSensor.setPulseAmplitudeRed(currentLEDBrightness);
  particleSensor.setPulseAmplitudeGreen(0);

  Serial.println("Tedd az ujjadat a szenzorra egyenletes nyomással!");
  Serial.println("A rendszer automatikusan beállítja a fényerőt...");

  delay(1000);
}

void pulseLoop() {
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
    } else if (risingEdge && derivative < DERIVATIVE_THRESHOLD_DOWN) {
      risingEdge = false;

      if (currentTime - lastBeatTime > DEBOUNCE_TIME) {
        long delta = currentTime - lastBeatTime;

        if (lastBeatTime > 0 && delta < (60000 / MIN_BPM)) {
          float newBPM = 60000.0f / delta;

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
                float weight = 1.0f + (x * 0.1f); // Újabb értékek nagyobb súllyal
                beatAvg += rates[idx] * weight;
                weightSum += weight;
                validSamples++;
              }
            }

            if (validSamples > 0) {
              beatAvg = (int)(beatAvg / weightSum);
            }

            Serial.print("💓 BEAT! ");
          }
        }
        lastBeatTime = currentTime;
      }
    }
  }

  // Kiírás
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

  if (irValue < 50000) {
    Serial.print(" [NINCS UJJ]");
  } else if (irValue > 260000) {
    Serial.print(" [TELÍTETT!]");
  }

  Serial.println();

  delay(20);
}
