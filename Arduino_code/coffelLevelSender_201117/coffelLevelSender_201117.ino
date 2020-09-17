#include <SPI.h>
#include <Ethernet.h>
#include <LiquidCrystal_I2C_AvrI2C.h>
#include "DHT.h"
#include <NewPing.h>
#include <QTRSensors.h>

#define NUM_SENSORS             5  // number of sensors used
#define NUM_SAMPLES_PER_SENSOR  4  // average 4 analog samples per sensor reading
#define EMITTER_PIN             3  // emitter is controlled by digital pin 2
#define LedGreenPIN             4 
#define resetPin                7


// sensors 0 through 5 are connected to analog inputs 0 through 5, respectively 
 QTRSensorsAnalog qtra((unsigned char[]) {8, 9, 10, 11, 12},
  NUM_SENSORS, NUM_SAMPLES_PER_SENSOR);
unsigned int sensorValues[NUM_SENSORS];

#define DHTPIN 2
#define DHTTYPE DHT11   // DHT 11
#define MAX_TIMER  20

LiquidCrystal_I2C_AvrI2C lcd(0x3f, 16, 2);

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED }; const char* host = "10.222.2.121"; int sayac = 0; bool hasIP = false;

EthernetClient client;
int fsrAnalogPin = 0; // FSR is connected to analog 0
float fsrReading;      // the analog reading from the FSR resistor divider
DHT dht(DHTPIN, DHTTYPE);
int timerCount;
bool relayMode = false;
String serverResponse = "";
String lastResponse = "";

void setup() {
  digitalWrite(resetPin, HIGH);
  delay(200);
  Serial.begin(9600);
  pinMode(LedGreenPIN, OUTPUT);
  pinMode(resetPin, OUTPUT);    
  lcd.begin();
  dht.begin();
  lcd.backlight();
  timerCount = MAX_TIMER;
  NewPing::timer_ms(5000, writeLcdDisplay);
  while (!Serial) {
  }

  digitalWrite(LedGreenPIN, HIGH);
  delay(5000);
  digitalWrite(LedGreenPIN, LOW);
  
  if (Ethernet.begin(mac) == 0) {
    digitalWrite(LedGreenPIN, HIGH);
    Serial.println("Failed to configure Ethernet using DHCP");
    delay(3000);
    digitalWrite(LedGreenPIN, LOW);    // turn the LED off by making the voltage LOW
     Serial.println("Try again Connecting");
    if (Ethernet.begin(mac) == 0) {
      Serial.println("Failed to configure Ethernet using DHCP");
      Serial.println("Reseting....");
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("Reseting...");
      delay(5000);
      digitalWrite(resetPin, LOW);
    }
  }
  // give the Ethernet shield a second to initialize:
  digitalWrite(LedGreenPIN, HIGH);
  Serial.println("IP:");
  Serial.println(Ethernet.localIP());
  hasIP = true;
  Serial.println("connecting...");
  displayLcdIP();
  delay(2000);
  // if you get a connection, report back via serial:

}



double fsrAvr = 0;
float hum = 0;
float temp = 0;

void loop() {

  loopProccess();

  if (sayac < 60) {
    return;
  }

  measureAndCalculate();
  connectionControl();
  clientSendMessage();

}

void loopProccess() {
  sayac += 1;
  if (client.available()) {
    String line = client.readStringUntil('\r');
    Serial.print(line);
    serverResponse += line;
  }
  int fsr = analogRead(fsrAnalogPin);
  fsrReading = fsr + fsrReading;
  delay(100);
}

void connectionControl()
{
  if (!client.connected()) {
    if (client.connect(host, 80)) {
      Serial.println("connected");
    }
    else {
      // if you didn't get a connection to the server:
      Serial.println("connection failed"); Serial.println("IP:");
      Serial.println(Ethernet.localIP());
      displayLcdIP();
      lcd.setCursor(0, 1);
      lcd.print("CON FAILED");
      client.stop(); 
      delay(1500);
      return;
    }
  }
}

void displayLcdIP() {
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("ip:");
  lcd.setCursor(3, 0);
  lcd.print(Ethernet.localIP());
}



void measureAndCalculate()
{
  hum = dht.readHumidity();
  temp = dht.readTemperature();
  if (!isFloat(hum) || hum > 100 || hum < 0) {
    hum = 0;
  }
  if (!isFloat(temp) || temp > 100 || hum < -30) {
    temp = 0;
  }

 //IR SENSOR
  qtra.read(sensorValues);

  Serial.print("**********Analog reading fsrReading= ");
  Serial.println(fsrReading);
  Serial.print("Analog reading sayac= ");
  Serial.println(sayac);

  float dblFsrRead = (float)fsrReading;
  float dblSayac = (float)sayac;
  Serial.print(" dblFsrRead= ");
  Serial.println(dblFsrRead);

  Serial.print(" dblSayac= ");
  Serial.println(dblSayac);

  fsrAvr = dblFsrRead / dblSayac;
   if (!isFloat(fsrAvr) || fsrAvr < 0) {
    fsrAvr = 0;
  }
  Serial.print(" AVR= ");
  Serial.println(fsrAvr);
  fsrReading = 0;
  sayac = 0;
  Serial.println("********************************sayac sifirlandi"); }

void clientSendMessage() {
  String data = String((int)fsrAvr);
  String irData;
   for (unsigned char i = 0; i < NUM_SENSORS; i++)
  {
    irData += "&ir" + String(i) + "=" +  String(sensorValues[i]);
    Serial.print(sensorValues[i]);
    Serial.print('\t'); // tab to format the raw data into columns in the Serial monitor
  }
  
  client.print("GET /arduino/insertcoffee?level=");
  Serial.print("GET /arduino/insertcoffee?level=");
  client.print(data + "&temp=" + String(temp) + "&humidity=" + String((int)hum) + irData);
  Serial.print(data + "&temp=" + String(temp) + "&humidity=" + String((int)hum) + irData);

  client.println(" HTTP/1.1");
  client.println("Host: 10.222.2.121");
  client.println();
  Serial.println();
  lastResponse = serverResponse;
  serverResponse = "";
}

int loopwriting = 0;
//LCD
void writeLcdDisplay() {
  
    //loop Process
   if (loopwriting != 0 && loopwriting % 6 == 0)
   {
    Serial.println("geldi");
    if(!hasIP){
      Serial.println("Reseting....");
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("Reseting...");
      delay(5000);
      digitalWrite(resetPin, LOW);
    }
   }
  loopwriting += 1;
  if (loopwriting == 100)
  {
    loopwriting = 0;
  }
  //
  //LCD Processs

 String response = "";
  if(serverResponse != "" )
  {
    response = serverResponse;
  }
  else if(lastResponse != ""){
    response = lastResponse;
  }

  if(response != ""){
    int ind1 = serverResponse.indexOf("s;");
    int ind2 = serverResponse.indexOf("m;");
    int ind3 = serverResponse.indexOf("f;");
    int lcd1StartInd = ind1 + 2;
    int lcd1FinishInd = ind2;
    int lcd2StartInd = ind2 + 2;
    int lcd2FinishInd = ind3;
    
     
    if(ind3> ind2 && ind2 > ind1 && lcd1StartInd > 0 && lcd1FinishInd > 0 && lcd2StartInd > 0 && lcd2FinishInd >0){
      String lcdStr1 = serverResponse.substring(lcd1StartInd, lcd1FinishInd);
      String lcdStr2 = serverResponse.substring(lcd2StartInd, lcd2FinishInd);
        lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print(lcdStr1);
      lcd.setCursor(0, 1);
      lcd.print(lcdStr2);
    }
  }
}

boolean isFloat(float flt) {
  String tString = String(flt);
  String tBuf;
  boolean decPt = false;
  
  if(tString.charAt(0) == '+' || tString.charAt(0) == '-') tBuf = &tString[1];
  else tBuf = tString;  

  for(int x=0;x<tBuf.length();x++)
  {
    if(tBuf.charAt(x) == '.') {
      if(decPt) return false;
      else decPt = true;  
    }    
    else if(tBuf.charAt(x) < '0' || tBuf.charAt(x) > '9') return false;
  }
  return true;
}


