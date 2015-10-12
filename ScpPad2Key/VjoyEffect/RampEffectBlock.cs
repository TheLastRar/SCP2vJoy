using System;
using vJoyInterfaceWrap;

namespace ScpPad2vJoy.VjoyEffect
{
    class RampEffectBlock : EnverlopeCabableBaseEffectBlack
    {
        private vJoy.FFB_EFF_RAMP rampEffect;

        public override void PrimaryEffectData(object eff)
        {
            base.PrimaryEffectData(eff);
            rampEffect = (vJoy.FFB_EFF_RAMP)eff;
        }

        protected override float ComputeEffect()
        {
            float UnitForce = (ApplyEnverlope() * gain);
            //Should be in the base method
            //then ComputeEffect() should return a float
            return UnitForce;
        }
        protected override Int32 Magnitude()
        {
            if (m_ffbHeader.Duration != vJoyConstants.EFFECT_INF_DURATION)
            {
                float unitTime = (float)m_ffbHeader.Duration / (float)m_runTime;
                Int32 delta = rampEffect.End - rampEffect.Start;
                return rampEffect.Start + (Int32)(delta * unitTime);
            }
            else
            {
                return rampEffect.Start;
            }
        }

    }
}
