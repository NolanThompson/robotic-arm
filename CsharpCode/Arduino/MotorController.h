#ifndef MOTORCONTROLLER_H
#define MOTORCONTROLLER_H

class MotorController {
private:
    int forwardPin;
    int reversePin;

public:
    MotorController(int forwardPin, int reversePin);
    void setSpeed(float speed);

private:
    int mapSpeedToPWM(float speed);
};

#endif
