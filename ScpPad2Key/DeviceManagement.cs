using System;

namespace ScpPad2vJoy
{
    [Flags]
    public enum DeviceManagement
    {
        None = 0,
        Xinput_DX = 1,
        Xinput_XI = 8,
        //Xinput_ALL = Xinput_DX | Xinput_XI,
        vJoy_Config = 2,
        vJoy_Device = 4,
        //next value is 16
    }
}
