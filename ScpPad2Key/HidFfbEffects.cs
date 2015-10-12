
namespace ScpPad2vJoy
{
    class HidFfbEffects
    {
        public const byte HID_USAGE_CONST = 0x26; //    Usage ET Constant Force
        public const byte HID_USAGE_RAMP = 0x27;  //    Usage ET Ramp
        public const byte HID_USAGE_SQUR = 0x30;  //    Usage ET Square
        public const byte HID_USAGE_SINE = 0x31;  //    Usage ET Sine
        public const byte HID_USAGE_TRNG = 0x32;  //    Usage ET Triangle
        public const byte HID_USAGE_STUP = 0x33;  //    Usage ET Sawtooth Up
        public const byte HID_USAGE_STDN = 0x34;  //    Usage ET Sawtooth Down
        public const byte HID_USAGE_SPRNG = 0x40; //    Usage ET Spring
        public const byte HID_USAGE_DMPR = 0x41;  //    Usage ET Damper
        public const byte HID_USAGE_INRT = 0x42;  //    Usage ET Inertia
        public const byte HID_USAGE_FRIC = 0x43;  //    Usage ET Friction
    }
}
