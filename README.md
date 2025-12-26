# Coffee Level Tracker (IoT Training Demo) ☕📈

An IoT demo project I built for internal training: measure coffee level (plus basic ambient telemetry) on an Arduino + Ethernet setup and visualize it on a web UI.

This is a **portfolio / educational** project (not a commercial product).

---

## What it does

The Arduino device:
- Reads a **force sensor (FSR)** on `A0` and computes an averaged “level” value
- Reads **temperature & humidity** from a **DHT11** sensor
- Reads **5-channel IR reflectance sensors** (QTR) for additional signals (useful for curved pots / multi-point detection)
- Sends all values to a backend via **HTTP GET** over **Ethernet (DHCP)**

The backend/web UI can:
- Store & visualize level data
- Return simple display messages that are rendered on a **16x2 I2C LCD** on the device

---

## Architecture (end-to-end flow)

1) Arduino samples sensors continuously and aggregates FSR readings  
2) Every ~60 loops (~6 seconds with `delay(100)`) it:
   - calculates `fsrAvr = fsrReading / sayac`
   - reads DHT11 (temp/humidity)
   - reads QTR IR array values
3) Arduino performs an HTTP request:

`GET /arduino/insertcoffee?level=<fsr>&temp=<t>&humidity=<h>&ir0=<v0>&ir1=<v1>...`

4) Arduino parses the HTTP response for LCD text markers and prints them to the 16x2 display.

---

## Hardware

**Core**
- Arduino (with enough digital pins; UNO works)
- Ethernet Shield (DHCP)
- FSR sensor on `A0`
- DHT11 on digital `D2`
- 5x IR reflectance sensors via **QTRSensorsAnalog** on digital pins `{8,9,10,11,12}` (as in code)
- 16x2 I2C LCD (address `0x3f`)
- Status LED on `D4`
- Reset control pin on `D7` (used to reset on repeated failures)

**Notes**
- “Coffee level” here is inferred from sensor signals; exact accuracy depends on mounting, pot material, and calibration.

---

## Arduino code highlights (based on the sketch)

### Network
- Uses `Ethernet.begin(mac)` (DHCP)
- Sends telemetry to `host = "10.222.2.121"` over port `80`

### Sensor payload
- `level`: integer of averaged FSR value (`(int)fsrAvr`)
- `temp`: DHT11 temperature
- `humidity`: DHT11 humidity (cast to int)
- `ir0..ir4`: QTR sensor array values

### LCD response format
The device looks for markers in the HTTP response:
- `s; ... m; ... f;`
and prints the substrings between:
- line1 = between `s;` and `m;`
- line2 = between `m;` and `f;`

> This allows the server/UI to push short status messages to the device.

---

## Backend endpoint (expected)

Arduino calls:

`/arduino/insertcoffee`

with query parameters:
- `level`
- `temp`
- `humidity`
- `ir0..ir4`

**Example request**
```
GET /arduino/insertcoffee?level=312&temp=24.0&humidity=41&ir0=800&ir1=790&ir2=775&ir3=760&ir4=740 HTTP/1.1
Host: 10.222.2.121
```

---

## How to run (quick start)

### 1) Arduino
1. Install required Arduino libraries (see below)
2. Update `host` IP if needed
3. Upload the sketch
4. Open Serial Monitor (9600) for logs
5. Verify DHCP prints an IP address and requests are being sent

### 2) Web UI / Server
- Ensure your server listens on `http://<host>/arduino/insertcoffee`
- Return a response body that optionally includes `s;...m;...f;` markers for LCD updates

---

## Dependencies (Arduino libraries)
The sketch uses:
- `Ethernet`
- `LiquidCrystal_I2C_AvrI2C`
- `DHT` (DHT11)
- `NewPing` (used for timer callback)
- `QTRSensors`

---

## Training / educational goals

This repo is useful as an internal IoT training example because it covers:
- Multi-sensor sampling + signal smoothing (averaging)
- Ethernet connectivity (DHCP) and simple HTTP telemetry
- Basic device UI (LCD) driven by server messages
- Practical “real-world messiness” (curved surfaces, noisy sensors, calibration)

---

## Safety & notes
- Demo/training project: not designed for safety-critical use.
- If you handle mains power near a coffee machine, use proper insulation and enclosures.
- Do not commit secrets or internal IP addresses if publishing publicly (use config templates).

---

## Project status
This repository is kept as a learning artifact. I may archive it once documentation is complete.


![alt text](https://github.com/taskma/Coffe_Level_Track/blob/master/coffelevel.PNG)

![alt text](https://github.com/taskma/Coffe_Level_Track/blob/master/equipments.PNG)

Special algorithm can detect coffee level even in curved coffepots

![alt text](https://github.com/taskma/Coffe_Level_Track/blob/master/coffe_machine.jpg)
