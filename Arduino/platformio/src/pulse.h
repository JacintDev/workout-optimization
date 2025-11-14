#pragma once
#include <Arduino.h>
#include <Wire.h>
#include "MAX30105.h"

// A POST-hoz ezekre a globálisokra támaszkodunk:
class HTTPClient;
extern HTTPClient http;        // state.cpp-ben definiáld
extern String authToken;       // state.cpp-ben definiáld

namespace pulse {

// Inicializálás (I2C már fusson: Wire.begin(...))
bool begin(TwoWire& wire, uint32_t i2cSpeed = I2C_SPEED_FAST);

void setUpDefaultVariables();
// Hívd minden loop-ban, amikor shouldSendPulse == true
void update();

// Aktuális értékek lekérdezése
float currentBPM();     // utolsó érvényes BPM
int   averageBPM();     // mozgóátlag (RATE_SIZE-ből)

// Opcionális beállítások
void setLedBrightness(uint8_t red);
void setDebug(bool on);

} // namespace pulse
