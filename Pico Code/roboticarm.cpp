#include <stdio.h>
#include <string.h>
#include "pico/stdlib.h"
#include <hardware/pwm.h>
#include "hardware/clocks.h"
#include "pico/multicore.h"


class MotorController {
private:
    int forwardPin;
    int reversePin;
    int cycles;
    uint slice_num;

public:
    MotorController(int forwardPin, int reversePin);
    void setSpeed(float speed);

private:
    int mapSpeedToPWM(float speed);
};

MotorController::MotorController(int forwardPin, int reversePin) {
    this->forwardPin = forwardPin;
    this->reversePin = reversePin;
    gpio_set_function(forwardPin, GPIO_FUNC_PWM);
    gpio_set_function(reversePin, GPIO_FUNC_PWM);

    slice_num = pwm_gpio_to_slice_num(forwardPin);
    pwm_set_wrap(slice_num,cycles-1);

    pwm_set_chan_level(slice_num, PWM_CHAN_A, 0);
    pwm_set_chan_level(slice_num, PWM_CHAN_B, 0);

    pwm_set_enabled(slice_num, true);
    cycles = 133000000;
}

void MotorController::setSpeed(float speed) {
    // Map the speed value to the PWM range
    int pwmValue = mapSpeedToPWM(speed);
    
    // Set PWM output based on the speed value
    if (speed > 0) 
    {
        pwm_set_chan_level(slice_num, PWM_CHAN_B, 0);
        pwm_set_chan_level(slice_num, PWM_CHAN_A, pwmValue);
    } else if (speed < 0) {
        pwm_set_chan_level(slice_num, PWM_CHAN_A, 0);
        pwm_set_chan_level(slice_num, PWM_CHAN_B, pwmValue);
    } else {
        // Stop the motor
        pwm_set_chan_level(slice_num, PWM_CHAN_A, 0);
        pwm_set_chan_level(slice_num, PWM_CHAN_B, 0);
    }
}

int MotorController::mapSpeedToPWM(float speed) {
    if(speed < 0)
        return -1*speed*cycles;
    else
        return speed*cycles;

}


const int LED_PIN = 25;
const int RELAY_PIN = 14;

const int encoder_pins[] = {11,13,15}; //left,forward,up

MotorController motorLeftRight(16,17);
MotorController motorUpDown(18,19);
MotorController motorForwardBackward(20,21);

const int CYCLES = 1024;

float measure_duty_cycle(uint gpio) {
    // Only the PWM B pins can be used as inputs.
    assert(pwm_gpio_to_channel(gpio) == PWM_CHAN_B);
    uint slice_num = pwm_gpio_to_slice_num(gpio);

    // Count once for every 100 cycles the PWM B input is high
    pwm_config cfg = pwm_get_default_config();
    pwm_config_set_clkdiv_mode(&cfg, PWM_DIV_B_HIGH);
    pwm_config_set_clkdiv(&cfg, CYCLES);
    pwm_init(slice_num, &cfg, false);
    gpio_set_function(gpio, GPIO_FUNC_PWM);

    pwm_set_enabled(slice_num, true);
    sleep_ms(10);
    pwm_set_enabled(slice_num, false);
    float counting_rate = clock_get_hz(clk_sys) / CYCLES;
    float max_possible_count = counting_rate * 0.039;
    float duty_cycle = pwm_get_counter(slice_num) / max_possible_count;
    return duty_cycle * 360.0f;
}

void ReadPWM()
{
    while(true)
        for(int i = 0; i < 3; i++)
        {
            printf("E %d %f\n",i,measure_duty_cycle(encoder_pins[i]));
        }
}


int main() 
{
    stdio_init_all();
    char identifier[20];
    float value;
    gpio_init(RELAY_PIN);
    gpio_set_dir(RELAY_PIN, GPIO_OUT);
    multicore_launch_core1(ReadPWM);

    while (true) 
    {
        scanf("%s %f", identifier, &value);
        
        if (strncmp(identifier, "left", 4) == 0) 
        {
            motorLeftRight.setSpeed(value);
            printf("Motor left/right set to speed %f\n", value);
        } 
        else if (strncmp(identifier, "right", 5) == 0) 
        {
            motorLeftRight.setSpeed(-value); // Reverse speed for right motor
            printf("Motor left/right set to speed %f\n", -value);
        } 
        else if (strncmp(identifier, "up", 2) == 0) 
        {
            motorUpDown.setSpeed(value);
            printf("Motor up/down set to speed %f\n", value);
        } 
        else if (strncmp(identifier, "down", 4) == 0) 
        {
            motorUpDown.setSpeed(-value); // Reverse speed for down motor
            printf("Motor up/down set to speed %f\n", -value);
        } 
        else if (strncmp(identifier, "forward", 7) == 0)  
        {
            motorForwardBackward.setSpeed(value);
            printf("Motor forward/backward set to speed %f\n", value);
        } 
        else if (strncmp(identifier, "backward", 8) == 0)   
        {
            motorForwardBackward.setSpeed(-value); // Reverse speed for backward motor
            printf("Motor forward/backward set to speed %f\n", -value);
        }
        else if (strncmp(identifier, "stop", 4) == 0) 
        {
            motorForwardBackward.setSpeed(0);
            motorLeftRight.setSpeed(0);
            motorUpDown.setSpeed(0);
            printf("Stop all motors\n");
        }
        else if (strncmp(identifier, "relay", 5) == 0) 
        {
            static bool relay = false;
            relay = !relay;

            if(relay)
            {
                gpio_put(RELAY_PIN, 1);
                printf("Suction ON\n");
            }
            else
            {
                gpio_put(RELAY_PIN, 0);
                printf("Suction OFF\n");
            }
        }
        else
        {
            printf("error\n");
        }
    }
    return 0;
}
