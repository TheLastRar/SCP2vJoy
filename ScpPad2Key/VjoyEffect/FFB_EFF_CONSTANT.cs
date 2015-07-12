using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ScpPad2vJoy.VjoyEffect
{
    [StructLayout(LayoutKind.Explicit)]
    struct FFB_EFF_CONSTANT
    {
        [FieldOffset(0)]
        public Byte EffectBlockIndex;
        [FieldOffset(1)]
        public Byte Magnitude;
    }
}
