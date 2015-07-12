using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScpPad2vJoy.VjoyEffect
{
    class SineEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(Double phaseTime)
        {
            return (float)Math.Sin(((2.0 * Math.PI) / (Double)periodEffect.Period) * phaseTime);
        }
    }
}
