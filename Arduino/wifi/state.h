#pragma once
#include <WebServer.h>
#include <HTTPClient.h>
#include <WebSocketsClient.h>
#include <MPU6050_tockn.h>

// Globális eszközök (extern, hogy több modul lássa)
extern WebServer server;           // 80-as port
extern HTTPClient http;
extern WebSocketsClient webSocket;
extern MPU6050 mpu;

// Állapotváltozók
extern String currentSSID;
extern String currentPassword;

extern bool wifiConnected;
extern String authToken;
extern bool isLoggedIn;

extern int trainingId;
extern bool isActiveTraining;

extern bool shouldSend;         // WS-vezérelt adatküldés (gyro)
extern bool shouldSendPulse;    // WS-vezérelt pulzus-küldés

// Szűrt szenzor értékek (opcionális, hogy több modulból is lehessen látni)
extern float filteredGyroX, filteredGyroY, filteredGyroZ;
extern float filteredAccX,  filteredAccY,  filteredAccZ;

// Időzítéshez
extern unsigned long lastSent;
