#include "gyro.h"
#include "state.h"
#include "config.h"
#include <ArduinoJson.h>
#include <Wire.h>

void initGyro() {
  Wire.begin(I2C_SDA, I2C_SCL);
  mpu.begin();
  mpu.calcGyroOffsets(true);
}

String buildGyroscopeJson() {
  StaticJsonDocument<512> doc;

  mpu.update();

  float rawGyroX = mpu.getGyroX();
  float rawGyroY = mpu.getGyroY();
  float rawGyroZ = mpu.getGyroZ();

  float rawAccX = mpu.getAccX();
  float rawAccY = mpu.getAccY();
  float rawAccZ = mpu.getAccZ();

  // Exponenciális simítás
  filteredGyroX = ALPHA * rawGyroX + (1 - ALPHA) * filteredGyroX;
  filteredGyroY = ALPHA * rawGyroY + (1 - ALPHA) * filteredGyroY;
  filteredGyroZ = ALPHA * rawGyroZ + (1 - ALPHA) * filteredGyroZ;

  filteredAccX  = ALPHA * rawAccX  + (1 - ALPHA) * filteredAccX;
  filteredAccY  = ALPHA * rawAccY  + (1 - ALPHA) * filteredAccY;
  filteredAccZ  = ALPHA * rawAccZ  + (1 - ALPHA) * filteredAccZ;

  // Nyugalom küszöbök
  const float gyroThreshold = 0.5f;
  const float accThreshold  = 0.1f;

  if (abs(filteredGyroX) < gyroThreshold) filteredGyroX = 0;
  if (abs(filteredGyroY) < gyroThreshold) filteredGyroY = 0;
  if (abs(filteredGyroZ) < gyroThreshold) filteredGyroZ = 0;

  if (abs(filteredAccX)  < accThreshold)  filteredAccX  = 0;
  if (abs(filteredAccY)  < accThreshold)  filteredAccY  = 0;
  if (abs(filteredAccZ)  < accThreshold)  filteredAccZ  = 0;

  doc["accelX"] = filteredAccX;
  doc["accelY"] = filteredAccY;
  doc["accelZ"] = filteredAccZ;
  doc["gyrosX"] = filteredGyroX;
  doc["gyrosY"] = filteredGyroY;
  doc["gyrosZ"] = filteredGyroZ;
  doc["trainingId"] = trainingId;

  String body;
  serializeJson(doc, body);
  return body;
}
