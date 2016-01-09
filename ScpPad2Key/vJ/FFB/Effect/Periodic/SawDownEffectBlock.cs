using System;

namespace ScpPad2vJoy.vJ.FFB.Effect.Periodic
{
    class SawDownEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(Double phaseTime)
        {
            //Effect starts at +Magnitude
            //And rises over the whole period
            //to - Magnitude
            Double riseRate = -(2.0) / (Double)periodEffect.Period;
            return (float)((phaseTime * riseRate) + 1.0);
        }
    }
}
