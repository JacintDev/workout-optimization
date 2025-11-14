#include "ws_handlers.h"
#include "state.h"
#include "config.h"
#include "wifi_portal.h"
#include <Arduino.h>
#include "pulse.h"

static void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {
  switch (type) {
    case WStype_CONNECTED:
      Serial.println("WebSocket connected!");
      break;
    case WStype_DISCONNECTED:
      Serial.println("WebSocket disconnected!");
      shouldSend = false;
      break;
    case WStype_TEXT: {
      Serial.printf("[Server]: %s\n", payload);
      if (strcmp((char*)payload, "start") == 0) {
        shouldSend = true;
        mpu.calcGyroOffsets(true);
        Serial.println(">> Indul az adatküldés");
      } else if (strcmp((char*)payload, "stop") == 0) {
        shouldSend = false;
        Serial.println(">> Leáll az adatküldés");
      } else if (strcmp((char*)payload, "startPulseDataSending") == 0) {
        shouldSendPulse = true;
        pulse::setUpDefaultVariables();
        Serial.println(">> Indul a pulzus adatküldés");
      } else if (strcmp((char*)payload, "stopPulseDataSending") == 0) {
        shouldSendPulse = false;
        Serial.println(">> Leáll a pulzus adatküldés");
      }
      break;
    }
    default: break;
  }
}

void setupWebSocket() {
  webSocket.begin(IP_ADDRESS.c_str(), 80, "/Websocket/connect");
  webSocket.onEvent(webSocketEvent);
  webSocket.setReconnectInterval(5000);
}

void webSocketLoop() {
  webSocket.loop();
}
