/*
  Coffee Level Tracker 
  -------------------------------
  What it does:
  - Reads a force sensor (FSR) as a proxy for "coffee level" (analog A0)
  - Reads temperature/humidity via DHT11 (pin 2)
  - Reads 5x QTR sensor values (pins are defined in config below)
  - Sends telemetry to an HTTP endpoint over Ethernet (DHCP)
  - Parses a simple server response format and shows 2 lines on an I2C LCD


*/

#include <SPI.h>
#include <Ethernet.h>
#include <LiquidCrystal_I2C_AvrI2C.h>
#include "DHT.h"
#include <QTRSensors.h>
#include <math.h>  // isnan/isfinite

// -----------------------------
// Hardware pins / sensors
// -----------------------------
static constexpr uint8_t LED_GREEN_PIN = 4;
static constexpr uint8_t RESET_PIN     = 7;

static constexpr uint8_t FSR_ANALOG_PIN = A0;

static constexpr uint8_t DHT_PIN  = 2;
static constexpr uint8_t DHT_TYPE = DHT11;

// QTR config
static constexpr uint8_t NUM_SENSORS = 5;
static constexpr uint8_t NUM_SAMPLES_PER_SENSOR = 4;

// NOTE: Original code used {8,9,10,11,12}. QTRSensorsAnalog expects analog-capable pins.
// If you're on a board where 8..12 are NOT analog pins, change this to {A1, A2, ...} etc.
static const uint8_t QTR_PINS[NUM_SENSORS] = {8, 9, 10, 11, 12};
QTRSensorsAnalog qtra((unsigned char*)QTR_PINS, NUM_SENSORS, NUM_SAMPLES_PER_SENSOR);
unsigned int qtrValues[NUM_SENSORS] = {0};

// LCD
LiquidCrystal_I2C_AvrI2C lcd(0x3F, 16, 2);

// Sensors
DHT dht(DHT_PIN, DHT_TYPE);

// -----------------------------
// Network / server config
// -----------------------------
static const byte MAC_ADDR[6] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };

// Server you call via HTTP
static const char* SERVER_HOST = "10.222.2.121";
static constexpr uint16_t SERVER_PORT = 80;

// Path format:
//   /arduino/insertcoffee?level=<int>&temp=<float>&humidity=<int>&ir0=<...>&ir1=<...>...
// Adjust if your server differs.
static const char* SERVER_PATH = "/arduino/insertcoffee";

EthernetClient client;

// -----------------------------
// Timing
// -----------------------------
// Sampling interval: how often we read analog FSR and accept incoming server bytes.
static constexpr unsigned long SAMPLE_INTERVAL_MS = 100;

// Send interval: how often we calculate average + send to server.
static constexpr unsigned long SEND_INTERVAL_MS = 60UL * 1000UL;  // 60 seconds

// LCD refresh interval (display last known server response)
static constexpr unsigned long LCD_INTERVAL_MS = 5UL * 1000UL;   // 5 seconds

// -----------------------------
// Runtime state
// -----------------------------
static bool hasIP = false;

static unsigned long lastSampleMs = 0;
static unsigned long windowStartMs = 0;
static unsigned long lastSendMs = 0;
static unsigned long lastLcdMs = 0;

static uint32_t fsrSum = 0;
static uint32_t fsrSamples = 0;

static float lastHumidity = 0.0f;
static float lastTempC = 0.0f;

static char serverRxBuf[512];   // raw bytes from server (best-effort)
static size_t serverRxLen = 0;

static char lastGoodPayload[256]; // last parsed payload (for LCD display)
static char lcdLine1[17] = {0};
static char lcdLine2[17] = {0};

// -----------------------------
// Utilities
// -----------------------------
static void hardResetBoard() {
  // External reset pin (wired to board reset circuit).
  // If you don't have this wired, replace with watchdog or just while(true){}.
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Resetting...");
  delay(1500);

  digitalWrite(LED_GREEN_PIN, HIGH);
  delay(250);
  digitalWrite(LED_GREEN_PIN, LOW);
  delay(250);

  digitalWrite(RESET_PIN, LOW);  // trigger external reset
  delay(1000);
}

static void lcdPrintCentered(uint8_t row, const char* text) {
  // Simple helper: left-justify (LCDs are small; keep it simple)
  lcd.setCursor(0, row);
  for (uint8_t i = 0; i < 16; i++) lcd.print(' ');
  lcd.setCursor(0, row);
  lcd.print(text);
}

static void displayIP() {
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("IP:");
  lcd.setCursor(3, 0);
  lcd.print(Ethernet.localIP());
}

// Parse server response format:
// We expect something like:  "s;<line1>m;<line2>f;"
// Example: "s;Hello m;World f;"
static bool parseLcdPayload(const char* src, char* out1, size_t out1sz, char* out2, size_t out2sz) {
  if (!src) return false;
  const char* s = strstr(src, "s;");
  const char* m = strstr(src, "m;");
  const char* f = strstr(src, "f;");
  if (!s || !m || !f) return false;
  if (!(s < m && m < f)) return false;

  s += 2; // after "s;"
  m += 2; // after "m;"

  size_t len1 = (size_t)(m - 2 - s);
  size_t len2 = (size_t)(f - 2 - m);

  // clamp to LCD width (16) and buffer sizes
  if (len1 >= out1sz) len1 = out1sz - 1;
  if (len2 >= out2sz) len2 = out2sz - 1;

  // also clamp to 16 chars for LCD
  if (len1 > 16) len1 = 16;
  if (len2 > 16) len2 = 16;

  memcpy(out1, s, len1); out1[len1] = '\0';
  memcpy(out2, m, len2); out2[len2] = '\0';

  return true;
}

static void updateLcdFromLastPayload() {
  // prefer freshly received bytes; otherwise show lastGoodPayload
  char l1[17] = {0};
  char l2[17] = {0};

  if (lastGoodPayload[0] == '\0') return;

  if (parseLcdPayload(lastGoodPayload, l1, sizeof(l1), l2, sizeof(l2))) {
    // only redraw if changed (reduces flicker)
    if (strncmp(l1, lcdLine1, 16) != 0 || strncmp(l2, lcdLine2, 16) != 0) {
      strncpy(lcdLine1, l1, 16); lcdLine1[16] = '\0';
      strncpy(lcdLine2, l2, 16); lcdLine2[16] = '\0';

      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print(lcdLine1);
      lcd.setCursor(0, 1);
      lcd.print(lcdLine2);
    }
  }
}

// -----------------------------
// Ethernet / client handling
// -----------------------------
static void initEthernetOrReset() {
  Serial.println("Initializing Ethernet (DHCP)...");
  if (Ethernet.begin((byte*)MAC_ADDR) == 0) {
    Serial.println("DHCP failed (attempt 1). Retrying...");
    digitalWrite(LED_GREEN_PIN, HIGH);
    delay(300);
    digitalWrite(LED_GREEN_PIN, LOW);
    delay(700);

    if (Ethernet.begin((byte*)MAC_ADDR) == 0) {
      Serial.println("DHCP failed (attempt 2). Resetting...");
      hasIP = false;
      hardResetBoard();
      return;
    }
  }

  hasIP = true;
  Serial.print("IP: ");
  Serial.println(Ethernet.localIP());
  displayIP();
}

static bool ensureConnected() {
  if (client.connected()) return true;

  Serial.print("Connecting to server ");
  Serial.print(SERVER_HOST);
  Serial.print(":");
  Serial.println(SERVER_PORT);

  if (client.connect(SERVER_HOST, SERVER_PORT)) {
    Serial.println("Connected.");
    return true;
  }

  Serial.println("Connection failed.");
  displayIP();
  lcd.setCursor(0, 1);
  lcd.print("CONN FAILED     ");
  client.stop();
  delay(600);
  return false;
}

static void readServerBytesNonBlocking() {
  while (client.available()) {
    int b = client.read();
    if (b < 0) break;

    if (serverRxLen < sizeof(serverRxBuf) - 1) {
      serverRxBuf[serverRxLen++] = (char)b;
      serverRxBuf[serverRxLen] = '\0';
    } else {
      // buffer full; keep last part by shifting (simple strategy)
      memmove(serverRxBuf, serverRxBuf + 1, sizeof(serverRxBuf) - 2);
      serverRxBuf[sizeof(serverRxBuf) - 2] = (char)b;
      serverRxBuf[sizeof(serverRxBuf) - 1] = '\0';
      serverRxLen = sizeof(serverRxBuf) - 1;
    }
  }

  // If we detect our markers, store a trimmed payload snapshot for LCD parsing.
  // (We keep it simple: store the entire buffer last part.)
  if (strstr(serverRxBuf, "s;") && strstr(serverRxBuf, "m;") && strstr(serverRxBuf, "f;")) {
    strncpy(lastGoodPayload, serverRxBuf, sizeof(lastGoodPayload) - 1);
    lastGoodPayload[sizeof(lastGoodPayload) - 1] = '\0';
    // reset rx buffer to avoid infinite growth / re-parsing stale data
    serverRxLen = 0;
    serverRxBuf[0] = '\0';
  }
}

// -----------------------------
// Measurements
// -----------------------------
static void readSensors() {
  // DHT can return NAN
  float h = dht.readHumidity();
  float t = dht.readTemperature();

  if (!isnan(h) && isfinite(h) && h >= 0.0f && h <= 100.0f) lastHumidity = h;
  if (!isnan(t) && isfinite(t) && t >= -30.0f && t <= 100.0f) lastTempC = t;

  // QTR sensors
  qtra.read(qtrValues);
}

static int computeCoffeeLevel() {
  // Average FSR over window; clamp negative / invalid
  if (fsrSamples == 0) return 0;
  float avg = (float)fsrSum / (float)fsrSamples;
  if (isnan(avg) || !isfinite(avg) || avg < 0.0f) avg = 0.0f;
  // Original code sent int(level)
  return (int)(avg + 0.5f);
}

// -----------------------------
// HTTP request
// -----------------------------
static void sendTelemetry() {
  if (!ensureConnected()) return;

  readSensors();
  int level = computeCoffeeLevel();

  // Build query string safely (avoid huge dynamic String concatenation)
  // Example:
  // GET /arduino/insertcoffee?level=123&temp=23.4&humidity=41&ir0=...&ir1=... HTTP/1.1
  char query[256];
  int n = snprintf(query, sizeof(query),
                   "GET %s?level=%d&temp=%.1f&humidity=%d",
                   SERVER_PATH,
                   level,
                   (double)lastTempC,
                   (int)(lastHumidity + 0.5f));
  if (n < 0 || (size_t)n >= sizeof(query)) {
    Serial.println("Query buffer overflow. Skipping send.");
    return;
  }

  // append IR/QTR readings
  // Format: &ir0=123&ir1=456...
  for (uint8_t i = 0; i < NUM_SENSORS; i++) {
    char tail[24];
    int tn = snprintf(tail, sizeof(tail), "&ir%u=%u", (unsigned)i, (unsigned)qtrValues[i]);
    if (tn < 0) continue;

    size_t curLen = strlen(query);
    if (curLen + (size_t)tn + 1 >= sizeof(query)) break;
    strcat(query, tail);
  }

  client.println(query);
  client.println(" HTTP/1.1");
  client.print("Host: ");
  client.println(SERVER_HOST);
  client.println("Connection: keep-alive");
  client.println();

  Serial.println(query);

  // After send: reset measurement window
  fsrSum = 0;
  fsrSamples = 0;
  windowStartMs = millis();
}

// -----------------------------
// Setup / Loop
// -----------------------------
void setup() {
  // Pin init first
  pinMode(LED_GREEN_PIN, OUTPUT);
  pinMode(RESET_PIN, OUTPUT);
  digitalWrite(RESET_PIN, HIGH);  // default high (inactive)
  digitalWrite(LED_GREEN_PIN, LOW);

  Serial.begin(9600);
  delay(200);

  lcd.begin();
  lcd.backlight();
  lcdPrintCentered(0, "Starting...");
  lcdPrintCentered(1, "Coffee Tracker");

  dht.begin();

  // Ethernet init
  digitalWrite(LED_GREEN_PIN, HIGH);
  delay(500);
  digitalWrite(LED_GREEN_PIN, LOW);

  initEthernetOrReset();
  displayIP();

  // Initialize timers
  unsigned long now = millis();
  lastSampleMs = now;
  windowStartMs = now;
  lastSendMs = now;
  lastLcdMs = now;

  // Blink once to show "ready"
  digitalWrite(LED_GREEN_PIN, HIGH);
  delay(200);
  digitalWrite(LED_GREEN_PIN, LOW);
}

void loop() {
  const unsigned long now = millis();

  // 1) Sample (FSR + incoming bytes) at fixed interval
  if (now - lastSampleMs >= SAMPLE_INTERVAL_MS) {
    lastSampleMs = now;

    // Read any server response bytes (non-blocking)
    readServerBytesNonBlocking();

    // Sample FSR
    int fsr = analogRead(FSR_ANALOG_PIN);
    fsrSum += (uint32_t)fsr;
    fsrSamples++;

    // Safety: if DHCP never got an IP, reset after a while
    if (!hasIP && (now > 60UL * 1000UL)) {
      hardResetBoard();
    }
  }

  // 2) Send telemetry periodically
  if (now - lastSendMs >= SEND_INTERVAL_MS) {
    lastSendMs = now;
    if (hasIP) {
      sendTelemetry();
    } else {
      // attempt to re-init ethernet
      initEthernetOrReset();
    }
  }

  // 3) LCD refresh
  if (now - lastLcdMs >= LCD_INTERVAL_MS) {
    lastLcdMs = now;
    updateLcdFromLastPayload();
  }

  // 4) Keep Ethernet stack alive on some boards/shields
  Ethernet.maintain();
}


