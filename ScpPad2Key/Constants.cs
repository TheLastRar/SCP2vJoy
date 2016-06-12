using System;

namespace ScpPad2vJoy
{
    class SCPConstants
    {
        //SCP
        public const int MAX_XINPUT_DEVICES = 4;
        //Axis
        public const int MAX_SCP_AXIS = 255; //Max SCP Stick/Pressure value (0-255)
        public const int MAX_ACCEL_AXIS = 1023; //Max Accel value (DS3) (0-255)
        //Vib
        public const float EFFECT_MAX_VALUE = 255;
    }
}
