using System;

namespace ScpPad2vJoy.vJ.FFB.Effect.Periodic
{
    class SineEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(Double phaseTime)
        {
            return (float)Math.Sin(((2.0 * Math.PI) / (Double)periodEffect.Period) * phaseTime);
        }
    }
}
