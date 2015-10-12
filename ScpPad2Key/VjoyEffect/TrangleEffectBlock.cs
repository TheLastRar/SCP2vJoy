using System;

namespace ScpPad2vJoy.VjoyEffect
{
    class TrangleEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(Double phaseTime)
        {
            //Effect starts at +Magnitude
            //reaches -Magnitude by half phase
            //returns back up to +Magnitude by phase end
            Double riseRate = (4.0) / (Double)periodEffect.Period;
            if (phaseTime < ((Double)periodEffect.Period / 2.0))
            {
                //+Mag to -Mag
                return (float)(1.0 - (phaseTime * riseRate));
            }
            else
            {
                //-Mag to +Mag
                phaseTime -= ((Double)periodEffect.Period / 2.0);
                return (float)((phaseTime * riseRate) - 1.0);
            }
        }
    }
}
