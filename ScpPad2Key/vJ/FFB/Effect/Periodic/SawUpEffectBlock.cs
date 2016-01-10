
namespace ScpPad2vJoy.vJ.FFB.Effect.Periodic
{
    class SawUpEffectBlock : PeriodBaseEffectBlock
    {
        protected override float ComputePeriodicEffectMult(double phaseTime)
        {
            //Effect starts at -Magnitude
            //And rises over the whole period
            //to + Magnitude
            double riseRate = (2.0) / (double)periodEffect.Period;
            return (float)((phaseTime * riseRate) - 1.0);
        }
    }
}
