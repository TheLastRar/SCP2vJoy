using System;

namespace ScpPad2vJoy.vJ
{
    class vJoyConstants
    {
        //vJoy
        public const UInt32 MIN_VER = 0x216;
        //Axis
        public const byte MAX_AXIS_COUNT = 8;
        public const Int32 HALF_AXIS_VALUE = MAX_AXIS_VALUE / 2;
        public const Int32 MAX_AXIS_VALUE = 32767; //Max vJoy value (0-32767)
        //Buttons
        public const byte MAX_BUTTON_COUNT = 128;
        public const uint BUTTON_EX0 = 1;
        public const uint BUTTON_EX1 = 33;
        public const uint BUTTON_EX2 = 65;
        public const uint BUTTON_EX3 = 97;
        //Vib
        public const float EFFECT_MAX_VALUE = 10000;
        public const float EFFECT_MAX_GAIN = 255;
        public const ushort EFFECT_INF_DURATION = 0xFFFF;
        public const byte EFFECT_MAX_LOOP_COUNT = 255;
        public const byte EFFECT_NULL_TRIGGER_BTN = 255;
        public const double EFFECT_MAX_PHASE = 35999 + 1;

        public const double EFFECT_DIRECTION_MAX_VALUE = 255;
    }
}
