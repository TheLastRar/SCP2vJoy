
namespace ScpPad2vJoy.vJ.FFB.Effect.Periodic
{
    class TrangleEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(double phaseTime)
        {
            //Effect starts at +Magnitude
            //reaches -Magnitude by half phase
            //returns back up to +Magnitude by phase end
            double riseRate = (4.0) / (double)periodEffect.Period;
            if (phaseTime < ((double)periodEffect.Period / 2.0))
            {
                //+Mag to -Mag
                return (float)(1.0 - (phaseTime * riseRate));
            }
            else
            {
                //-Mag to +Mag
                phaseTime -= ((double)periodEffect.Period / 2.0);
                return (float)((phaseTime * riseRate) - 1.0);
            }
        }
    }
}
