using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScpPad2vJoy.VjoyEffect
{
    class TrangleEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(Double phaseTime)
        {
            //Effect starts at +Magnitude
            //reaches -Magnitude by half phase
            //returns back up to +Magnitude by phase end
            Double riseRate = (Double)(4 * periodEffect.Magnitude) / (Double)periodEffect.Period;
            if (phaseTime < ((Double)periodEffect.Period / 2.0))
            {
                //+Mag to -Mag
                return (float)((Double)periodEffect.Magnitude - (phaseTime * riseRate));
            } 
            else 
            {
                //-Mag to +Mag
                phaseTime -= ((Double)periodEffect.Period / 2.0);
                return (float)((phaseTime * riseRate) - (Double)periodEffect.Magnitude);
            }
        }
    }
}
