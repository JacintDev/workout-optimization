#pragma once
#include <Arduino.h>

// -- Általános beállítások --
static const char* AP_SSID     = "ESP32_Setup";
static const char* AP_PASSWORD = "12345678";

// -- Szerver IP / host --
static String IP_ADDRESS = "46.139.216.110";

// -- MPU6050 I2C lábak (ESP32) --
static const int I2C_SDA = 6;
static const int I2C_SCL = 7;

// -- Szűrés paraméter --
static constexpr float ALPHA = 0.3f;

// -- Küldési időzítés --
static const unsigned long GYRO_SEND_MS = 100;
