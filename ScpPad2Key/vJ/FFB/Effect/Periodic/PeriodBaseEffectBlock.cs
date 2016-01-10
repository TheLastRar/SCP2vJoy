using System;
using vJoyInterfaceWrap;

namespace ScpPad2vJoy.vJ.FFB.Effect.Periodic
{
    abstract class PeriodBaseEffectBlock : EnverlopeCabableBaseEffectBlock
    {
        protected vJoy.FFB_EFF_PERIOD periodEffect;
        public override void PrimaryEffectData(object eff)
        {
            base.PrimaryEffectData(eff);
            periodEffect = (vJoy.FFB_EFF_PERIOD)eff;
        }
        protected override sealed float ComputeEffect()
        {
            //Enverlope is applied before offset is
            //according to the documents(?)
            float UnitForce = (ApplyEnverlope() * gain);

            //Phase offset as a unit of time
            double effectTime = ((double)periodEffect.Phase / vJoyConstants.EFFECT_MAX_PHASE) * periodEffect.Period;
            //When current cycle started
            long effectBaseTime = runTime / (long)periodEffect.Period;
            //Phase offset + time since current cycle started
            effectTime += runTime - (effectBaseTime * (long)periodEffect.Period);
            //wrap around phase time
            //incase of large Phase + 
            //large time since current cycle started
            if (effectTime > periodEffect.Period)
            {
                effectTime -= (double)periodEffect.Period;
            }
            //Get Periodic effect value
            UnitForce *= ComputePeriodicEffectMult(effectTime);
            //Offset is a signed value
            float offsetValue = (float)(periodEffect.Offset);
            UnitForce += offsetValue;
            return UnitForce;
        }
        protected abstract float ComputePeriodicEffectMult(double phaseTime);

        protected override Int32 Magnitude()
        {
            return (Int32)periodEffect.Magnitude;
        }
    }
}
