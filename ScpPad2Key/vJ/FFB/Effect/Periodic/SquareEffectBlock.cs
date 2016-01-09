using System;

namespace ScpPad2vJoy.vJ.FFB.Effect.Periodic
{
    class SquareEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(Double phaseTime)
        {
            //Effect starts at +Magnitude
            //becomes -Magnitude by half phase
            if (phaseTime < ((Double)periodEffect.Period / 2.0))
            {
                //+Mag
                return 1;
            }
            else
            {
                //-Mag
                return -1.0f;
            }
        }
    }
}
