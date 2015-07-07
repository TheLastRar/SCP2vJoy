using System;

namespace ScpPad2vJoy
{
    [Flags]
    public enum DeviceManagement
    {
        None = 0,
        Xinput_DX = 1,
        vJoy_Config = 2,
        vJoy_Device = 4,
    }
}
