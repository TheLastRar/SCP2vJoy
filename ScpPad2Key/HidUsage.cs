
namespace ScpPad2vJoy
{
    class HidUsage
    {
        //
        // Usage Pages
        //

        //typedef USHORT USAGE, *PUSAGE;

        public const byte HID_USAGE_PAGE_UNDEFINED = (0x00);
        public const byte HID_USAGE_PAGE_GENERIC = (0x01);
        public const byte HID_USAGE_PAGE_SIMULATION = (0x02);
        public const byte HID_USAGE_PAGE_VR = (0x03);
        public const byte HID_USAGE_PAGE_SPORT = (0x04);
        public const byte HID_USAGE_PAGE_GAME = (0x05);
        public const byte HID_USAGE_PAGE_KEYBOARD = (0x07);
        public const byte HID_USAGE_PAGE_LED = (0x08);
        public const byte HID_USAGE_PAGE_BUTTON = (0x09);
        public const byte HID_USAGE_PAGE_ORDINAL = (0x0A);
        public const byte HID_USAGE_PAGE_TELEPHONY = (0x0B);
        public const byte HID_USAGE_PAGE_CONSUMER = (0x0C);
        public const byte HID_USAGE_PAGE_DIGITIZER = (0x0D);
        public const byte HID_USAGE_PAGE_UNICODE = (0x10);
        public const byte HID_USAGE_PAGE_ALPHANUMERIC = (0x14);

        //
        // Microsoft Vendor Specific Usage Pages
        //
        public const ushort HID_USAGE_PAGE_MICROSOFT_BLUETOOTH_HANDSFREE = (0xFFF3);

        //
        // Usages from Generic Desktop Page (0x01)
        //

        public const byte HID_USAGE_GENERIC_POINTER = (0x01);
        public const byte HID_USAGE_GENERIC_MOUSE = (0x02);
        public const byte HID_USAGE_GENERIC_JOYSTICK = (0x04);
        public const byte HID_USAGE_GENERIC_GAMEPAD = (0x05);
        public const byte HID_USAGE_GENERIC_KEYBOARD = (0x06);
        public const byte HID_USAGE_GENERIC_KEYPAD = (0x07);
        public const byte HID_USAGE_GENERIC_SYSTEM_CTL = (0x80);

        public const byte HID_USAGE_GENERIC_X = (0x30);
        public const byte HID_USAGE_GENERIC_Y = (0x31);
        public const byte HID_USAGE_GENERIC_Z = (0x32);
        public const byte HID_USAGE_GENERIC_RX = (0x33);
        public const byte HID_USAGE_GENERIC_RY = (0x34);
        public const byte HID_USAGE_GENERIC_RZ = (0x35);
        public const byte HID_USAGE_GENERIC_SLIDER = (0x36);
        public const byte HID_USAGE_GENERIC_DIAL = (0x37);
        public const byte HID_USAGE_GENERIC_WHEEL = (0x38);
        public const byte HID_USAGE_GENERIC_HATSWITCH = (0x39);
        public const byte HID_USAGE_GENERIC_COUNTED_BUFFER = (0x3A);
        public const byte HID_USAGE_GENERIC_BYTE_COUNT = (0x3B);
        public const byte HID_USAGE_GENERIC_MOTION_WAKEUP = (0x3C);
        public const byte HID_USAGE_GENERIC_VX = (0x40);
        public const byte HID_USAGE_GENERIC_VY = (0x41);
        public const byte HID_USAGE_GENERIC_VZ = (0x42);
        public const byte HID_USAGE_GENERIC_VBRX = (0x43);
        public const byte HID_USAGE_GENERIC_VBRY = (0x44);
        public const byte HID_USAGE_GENERIC_VBRZ = (0x45);
        public const byte HID_USAGE_GENERIC_VNO = (0x46);
        public const byte HID_USAGE_GENERIC_SYSCTL_POWER = (0x81);
        public const byte HID_USAGE_GENERIC_SYSCTL_SLEEP = (0x82);
        public const byte HID_USAGE_GENERIC_SYSCTL_WAKE = (0x83);
        public const byte HID_USAGE_GENERIC_SYSCTL_CONTEXT_MENU = (0x84);
        public const byte HID_USAGE_GENERIC_SYSCTL_MAIN_MENU = (0x85);
        public const byte HID_USAGE_GENERIC_SYSCTL_APP_MENU = (0x86);
        public const byte HID_USAGE_GENERIC_SYSCTL_HELP_MENU = (0x87);
        public const byte HID_USAGE_GENERIC_SYSCTL_MENU_EXIT = (0x88);
        public const byte HID_USAGE_GENERIC_SYSCTL_MENU_SELECT = (0x89);
        public const byte HID_USAGE_GENERIC_SYSCTL_MENU_RIGHT = (0x8A);
        public const byte HID_USAGE_GENERIC_SYSCTL_MENU_LEFT = (0x8B);
        public const byte HID_USAGE_GENERIC_SYSCTL_MENU_UP = (0x8C);
        public const byte HID_USAGE_GENERIC_SYSCTL_MENU_DOWN = (0x8D);

        //
        // Usages from Simulation Controls Page (0x02)
        //

        public const byte HID_USAGE_SIMULATION_RUDDER = (0xBA);
        public const byte HID_USAGE_SIMULATION_THROTTLE = (0xBB);

        //
        // Virtual Reality Controls Page (0x03)
        //


        //
        // Sport Controls Page (0x04)
        //


        //
        // Game Controls Page (0x05)
        //


        //
        // Keyboard/Keypad Page (0x07)
        //

        // Error "keys"
        public const byte HID_USAGE_KEYBOARD_NOEVENT = (0x00);
        public const byte HID_USAGE_KEYBOARD_ROLLOVER = (0x01);
        public const byte HID_USAGE_KEYBOARD_POSTFAIL = (0x02);
        public const byte HID_USAGE_KEYBOARD_UNDEFINED = (0x03);

        // Letters
        public const byte HID_USAGE_KEYBOARD_aA = (0x04);
        public const byte HID_USAGE_KEYBOARD_zZ = (0x1D);
        // Numbers
        public const byte HID_USAGE_KEYBOARD_ONE = (0x1E);
        public const byte HID_USAGE_KEYBOARD_ZERO = (0x27);
        // Modifier Keys
        public const byte HID_USAGE_KEYBOARD_LCTRL = (0xE0);
        public const byte HID_USAGE_KEYBOARD_LSHFT = (0xE1);
        public const byte HID_USAGE_KEYBOARD_LALT = (0xE2);
        public const byte HID_USAGE_KEYBOARD_LGUI = (0xE3);
        public const byte HID_USAGE_KEYBOARD_RCTRL = (0xE4);
        public const byte HID_USAGE_KEYBOARD_RSHFT = (0xE5);
        public const byte HID_USAGE_KEYBOARD_RALT = (0xE6);
        public const byte HID_USAGE_KEYBOARD_RGUI = (0xE7);
        public const byte HID_USAGE_KEYBOARD_SCROLL_LOCK = (0x47);
        public const byte HID_USAGE_KEYBOARD_NUM_LOCK = (0x53);
        public const byte HID_USAGE_KEYBOARD_CAPS_LOCK = (0x39);
        // Funtion keys
        public const byte HID_USAGE_KEYBOARD_F1 = (0x3A);
        public const byte HID_USAGE_KEYBOARD_F12 = (0x45);

        public const byte HID_USAGE_KEYBOARD_RETURN = (0x28);
        public const byte HID_USAGE_KEYBOARD_ESCAPE = (0x29);
        public const byte HID_USAGE_KEYBOARD_DELETE = (0x2A);

        public const byte HID_USAGE_KEYBOARD_PRINT_SCREEN = (0x46);

        // and hundreds more...

        //
        // LED Page (0x08)
        //

        public const byte HID_USAGE_LED_NUM_LOCK = (0x01);
        public const byte HID_USAGE_LED_CAPS_LOCK = (0x02);
        public const byte HID_USAGE_LED_SCROLL_LOCK = (0x03);
        public const byte HID_USAGE_LED_COMPOSE = (0x04);
        public const byte HID_USAGE_LED_KANA = (0x05);
        public const byte HID_USAGE_LED_POWER = (0x06);
        public const byte HID_USAGE_LED_SHIFT = (0x07);
        public const byte HID_USAGE_LED_DO_NOT_DISTURB = (0x08);
        public const byte HID_USAGE_LED_MUTE = (0x09);
        public const byte HID_USAGE_LED_TONE_ENABLE = (0x0A);
        public const byte HID_USAGE_LED_HIGH_CUT_FILTER = (0x0B);
        public const byte HID_USAGE_LED_LOW_CUT_FILTER = (0x0C);
        public const byte HID_USAGE_LED_EQUALIZER_ENABLE = (0x0D);
        public const byte HID_USAGE_LED_SOUND_FIELD_ON = (0x0E);
        public const byte HID_USAGE_LED_SURROUND_FIELD_ON = (0x0F);
        public const byte HID_USAGE_LED_REPEAT = (0x10);
        public const byte HID_USAGE_LED_STEREO = (0x11);
        public const byte HID_USAGE_LED_SAMPLING_RATE_DETECT = (0x12);
        public const byte HID_USAGE_LED_SPINNING = (0x13);
        public const byte HID_USAGE_LED_CAV = (0x14);
        public const byte HID_USAGE_LED_CLV = (0x15);
        public const byte HID_USAGE_LED_RECORDING_FORMAT_DET = (0x16);
        public const byte HID_USAGE_LED_OFF_HOOK = (0x17);
        public const byte HID_USAGE_LED_RING = (0x18);
        public const byte HID_USAGE_LED_MESSAGE_WAITING = (0x19);
        public const byte HID_USAGE_LED_DATA_MODE = (0x1A);
        public const byte HID_USAGE_LED_BATTERY_OPERATION = (0x1B);
        public const byte HID_USAGE_LED_BATTERY_OK = (0x1C);
        public const byte HID_USAGE_LED_BATTERY_LOW = (0x1D);
        public const byte HID_USAGE_LED_SPEAKER = (0x1E);
        public const byte HID_USAGE_LED_HEAD_SET = (0x1F);
        public const byte HID_USAGE_LED_HOLD = (0x20);
        public const byte HID_USAGE_LED_MICROPHONE = (0x21);
        public const byte HID_USAGE_LED_COVERAGE = (0x22);
        public const byte HID_USAGE_LED_NIGHT_MODE = (0x23);
        public const byte HID_USAGE_LED_SEND_CALLS = (0x24);
        public const byte HID_USAGE_LED_CALL_PICKUP = (0x25);
        public const byte HID_USAGE_LED_CONFERENCE = (0x26);
        public const byte HID_USAGE_LED_STAND_BY = (0x27);
        public const byte HID_USAGE_LED_CAMERA_ON = (0x28);
        public const byte HID_USAGE_LED_CAMERA_OFF = (0x29);
        public const byte HID_USAGE_LED_ON_LINE = (0x2A);
        public const byte HID_USAGE_LED_OFF_LINE = (0x2B);
        public const byte HID_USAGE_LED_BUSY = (0x2C);
        public const byte HID_USAGE_LED_READY = (0x2D);
        public const byte HID_USAGE_LED_PAPER_OUT = (0x2E);
        public const byte HID_USAGE_LED_PAPER_JAM = (0x2F);
        public const byte HID_USAGE_LED_REMOTE = (0x30);
        public const byte HID_USAGE_LED_FORWARD = (0x31);
        public const byte HID_USAGE_LED_REVERSE = (0x32);
        public const byte HID_USAGE_LED_STOP = (0x33);
        public const byte HID_USAGE_LED_REWIND = (0x34);
        public const byte HID_USAGE_LED_FAST_FORWARD = (0x35);
        public const byte HID_USAGE_LED_PLAY = (0x36);
        public const byte HID_USAGE_LED_PAUSE = (0x37);
        public const byte HID_USAGE_LED_RECORD = (0x38);
        public const byte HID_USAGE_LED_ERROR = (0x39);
        public const byte HID_USAGE_LED_SELECTED_INDICATOR = (0x3A);
        public const byte HID_USAGE_LED_IN_USE_INDICATOR = (0x3B);
        public const byte HID_USAGE_LED_MULTI_MODE_INDICATOR = (0x3C);
        public const byte HID_USAGE_LED_INDICATOR_ON = (0x3D);
        public const byte HID_USAGE_LED_INDICATOR_FLASH = (0x3E);
        public const byte HID_USAGE_LED_INDICATOR_SLOW_BLINK = (0x3F);
        public const byte HID_USAGE_LED_INDICATOR_FAST_BLINK = (0x40);
        public const byte HID_USAGE_LED_INDICATOR_OFF = (0x41);
        public const byte HID_USAGE_LED_FLASH_ON_TIME = (0x42);
        public const byte HID_USAGE_LED_SLOW_BLINK_ON_TIME = (0x43);
        public const byte HID_USAGE_LED_SLOW_BLINK_OFF_TIME = (0x44);
        public const byte HID_USAGE_LED_FAST_BLINK_ON_TIME = (0x45);
        public const byte HID_USAGE_LED_FAST_BLINK_OFF_TIME = (0x46);
        public const byte HID_USAGE_LED_INDICATOR_COLOR = (0x47);
        public const byte HID_USAGE_LED_RED = (0x48);
        public const byte HID_USAGE_LED_GREEN = (0x49);
        public const byte HID_USAGE_LED_AMBER = (0x4A);
        public const byte HID_USAGE_LED_GENERIC_INDICATOR = (0x4B);

        //
        //  Button Page (0x09)
        //
        //  There is no need to label these usages.
        //


        //
        //  Ordinal Page (0x0A)
        //
        //  There is no need to label these usages.
        //


        //
        //  Telephony Device Page (0x0B)
        //

        public const byte HID_USAGE_TELEPHONY_PHONE = (0x01);
        public const byte HID_USAGE_TELEPHONY_ANSWERING_MACHINE = (0x02);
        public const byte HID_USAGE_TELEPHONY_MESSAGE_CONTROLS = (0x03);
        public const byte HID_USAGE_TELEPHONY_HANDSET = (0x04);
        public const byte HID_USAGE_TELEPHONY_HEADSET = (0x05);
        public const byte HID_USAGE_TELEPHONY_KEYPAD = (0x06);
        public const byte HID_USAGE_TELEPHONY_PROGRAMMABLE_BUTTON = (0x07);
        public const byte HID_USAGE_TELEPHONY_REDIAL = (0x24);
        public const byte HID_USAGE_TELEPHONY_TRANSFER = (0x25);
        public const byte HID_USAGE_TELEPHONY_DROP = (0x26);
        public const byte HID_USAGE_TELEPHONY_LINE = (0x2A);
        public const byte HID_USAGE_TELEPHONY_RING_ENABLE = (0x2D);
        public const byte HID_USAGE_TELEPHONY_SEND = (0x31);
        public const byte HID_USAGE_TELEPHONY_KEYPAD_0 = (0xB0);
        public const byte HID_USAGE_TELEPHONY_KEYPAD_D = (0xBF);
        public const byte HID_USAGE_TELEPHONY_HOST_AVAILABLE = (0xF1);


        //
        // Microsoft Bluetooth Handsfree Page (0xFFF3)
        //
        public const byte HID_USAGE_MS_BTH_HF_DIALNUMBER = (0x21);
        public const byte HID_USAGE_MS_BTH_HF_DIALMEMORY = (0x22);


        //
        // and others...
        //

        public const byte HID_USAGE_CONSUMERCTRL = (0x01);
        public const byte HID_USAGE_DIGITIZER_PEN = (0x02);
        public const byte HID_USAGE_DIGITIZER_IN_RANGE = (0x32);
        public const byte HID_USAGE_DIGITIZER_TIP_SWITCH = (0x42);
        public const byte HID_USAGE_DIGITIZER_BARREL_SWITCH = (0x44);
    }
}
