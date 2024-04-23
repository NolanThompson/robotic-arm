#include "MotorController.h"

MotorController motorLeft(16, 17);    // Pins for left motor controller
MotorController motorUpDown(18, 19);  // Pins for up/down motor controller
MotorController motorForwardBackward(20, 21);  // Pins for forward/backward motor controller

bool relay = false;
int relayPin = 14;

//encoder 11 12 13
const int encoderPin1 = 11;
const int encoderPin2 = 12;
const int encoderPin3 = 13;

volatile unsigned long pulseStart1;
volatile unsigned long pulseEnd1;
volatile unsigned long pulseStart2;
volatile unsigned long pulseEnd2;
volatile unsigned long pulseStart3;
volatile unsigned long pulseEnd3;

bool encoder1R;
bool encoder2R;
bool encoder3R;

volatile unsigned long lastEncoder1Message = 0;
volatile unsigned long lastEncoder2Message = 0;
volatile unsigned long lastEncoder3Message = 0;

const unsigned long interval = 1000; // 1 second


void setup() {
  Serial.begin(115200);

  pinMode(relayPin, OUTPUT);
  // Set initial relay state
  digitalWrite(relayPin, relay ? HIGH : LOW);

  pinMode(encoderPin1, INPUT);
  pinMode(encoderPin2, INPUT);
  pinMode(encoderPin3, INPUT);

  attachInterrupt(digitalPinToInterrupt(encoderPin1), encoderISR1, CHANGE);
  attachInterrupt(digitalPinToInterrupt(encoderPin2), encoderISR2, CHANGE);
  attachInterrupt(digitalPinToInterrupt(encoderPin3), encoderISR3, CHANGE);
}

void loop() {
  if (Serial.available() > 0) 
  {
    String input = Serial.readStringUntil('\n');
    if (input.startsWith("left")) {
      float speed = input.substring(5).toFloat();
      motorLeft.setSpeed(speed);
      Serial.println("Motor left/right set to speed " + String(speed));
    } else if (input.startsWith("right")) {
      float speed = -input.substring(6).toFloat(); // Reverse speed for right motor
      motorLeft.setSpeed(speed);
      Serial.println("Motor left/right set to speed " + String(speed));
    } else if (input.startsWith("up")) {
      float speed = input.substring(3).toFloat();
      motorUpDown.setSpeed(speed);
      Serial.println("Motor up/down set to speed " + String(speed));
    } else if (input.startsWith("down")) {
      float speed = -input.substring(5).toFloat(); // Reverse speed for down motor
      motorUpDown.setSpeed(speed);
      Serial.println("Motor up/down set to speed " + String(speed));
    } else if (input.startsWith("forward")) {
      float speed = input.substring(8).toFloat();
      motorForwardBackward.setSpeed(speed);
      Serial.println("Motor forward/backward set to speed " + String(speed));
    } else if (input.startsWith("backward")) {
      float speed = -input.substring(9).toFloat(); // Reverse speed for backward motor
      motorForwardBackward.setSpeed(speed);
      Serial.println("Motor forward/backward set to speed " + String(speed));
    }
    else if (input.startsWith("stop")) {
      motorForwardBackward.setSpeed(0);
      motorLeft.setSpeed(0);
      motorUpDown.setSpeed(0);
      Serial.println("Stop all motors");
    }
    else if (input.startsWith("relay")) {
      relay = !relay;

      digitalWrite(relayPin, relay ? HIGH : LOW);


      if(relay)
        Serial.println("Suction ON");
      else
        Serial.println("sunction OFF");
    }
    else
    {
      Serial.print("error");
    }
  }

    if(encoder1R)
    {
      int duration = pulseEnd1-pulseStart1;
      if(duration < 0)
      {
        Serial.println("coder 1 miss");
      }
      else
      {
        float angle = (float)duration / 1025.0 * 360.0;
        Serial.println("E 1 " + String(angle));
      }
      encoder1R = false;
    }

    if(encoder2R)
    {
      int duration = pulseEnd2-pulseStart2;
      if(duration < 0)
      {
        Serial.println("coder 2 miss");
      }
      else
      {
        float angle = (float)duration / 1025.0 * 360.0;
        Serial.println("E 2 " + String(angle));
      }
      encoder2R = false;
    }
    if(encoder3R)
    {
      int duration = pulseEnd3-pulseStart3;
      if(duration < 0)
      {
        Serial.println("coder 3 miss");
      }
      else
      {
        float angle = (float)duration / 1025.0 * 360.0;
        Serial.println("E 3 " + String(angle));
      }
      encoder3R = false;
    }
  
}
  void encoderISR1() {
  if (digitalRead(encoderPin1) == HIGH) {
    pulseStart1 = micros();
  } else {
    pulseEnd1 = micros();
    encoder1R = true;
  }
}

void encoderISR2() {
  if (digitalRead(encoderPin2) == HIGH) {
    pulseStart2 = micros();
  } else {
    pulseEnd2 = micros();
    encoder2R = true;
  }
}

void encoderISR3() {
  if (digitalRead(encoderPin3) == HIGH) {
    pulseStart3 = micros();
  } else {
    pulseEnd3 = micros();
    encoder3R = true;
  }
}

