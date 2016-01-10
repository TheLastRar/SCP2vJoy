using System;

namespace ScpPad2vJoy.vJ.FFB.Effect.Periodic
{
    class SineEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(double phaseTime)
        {
            return (float)Math.Sin(((2.0 * Math.PI) / (double)periodEffect.Period) * phaseTime);
        }
    }
}
