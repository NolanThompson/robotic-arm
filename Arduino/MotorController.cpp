#include "MotorController.h"
#include <Arduino.h>

MotorController::MotorController(int forwardPin, int reversePin) {
    this->forwardPin = forwardPin;
    this->reversePin = reversePin;
    pinMode(forwardPin, OUTPUT);
    pinMode(reversePin, OUTPUT);
}

void MotorController::setSpeed(float speed) {
    // Map the speed value to the PWM range
    int pwmValue = mapSpeedToPWM(speed);
    
    // Set PWM output based on the speed value
    if (speed > 0) {
        analogWrite(reversePin, 0);
        analogWrite(forwardPin, pwmValue);
    } else if (speed < 0) {
        analogWrite(forwardPin, 0);
        analogWrite(reversePin, pwmValue); // Make sure it's positive
    } else {
        // Stop the motor
        analogWrite(forwardPin, 0);
        analogWrite(reversePin, 0);
    }
}

int MotorController::mapSpeedToPWM(float speed) {
    // Map the speed range (-1 to 1) to the PWM range (0 to 255)
    int pwmValue = constrain(int(abs(speed) * 255), 0, 255);
    return pwmValue;
}
