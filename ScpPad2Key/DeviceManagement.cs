using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
