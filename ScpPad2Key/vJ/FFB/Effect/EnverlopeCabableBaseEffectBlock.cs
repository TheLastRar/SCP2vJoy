using System;
using System.Diagnostics;
using vJoyInterfaceWrap;

namespace ScpPad2vJoy.vJ.FFB.Effect
{
    abstract class EnverlopeCabableBaseEffectBlock : BaseEffectBlock
    {
        //Enverlope
        private vJoy.FFB_EFF_ENVLP envEffect;
        private bool hasEnvelope = false;

        public override void PrimaryEffectData(object eff)
        {
            if (hasEnvelope == true)
            {
                Trace.WriteLine("Effect Redownload(?) Clearing Enverlope");
                hasEnvelope = false;
            }
        }
        public override sealed void SecondaryEffectData(object eff)
        {
            Trace.WriteLine("Got Enverlope");
            envEffect = (vJoy.FFB_EFF_ENVLP)eff;
            hasEnvelope = true;
        }

        //Sustain level
        protected abstract Int32 Magnitude();

        //Attack and fade level specified by envEffect
        //Sustain level specified by unmodified effect Peak Magnitude
        //(Excluding applyied offset(?))
        protected float ApplyEnverlope()
        {
            if (hasEnvelope == false)
            {
                return Magnitude();
            }
            float fadeStart = (m_ffbHeader.Duration - envEffect.FadeTime);
            if (envEffect.AttackTime <= runTime &
                (runTime <= fadeStart | m_ffbHeader.Duration == vJoyConstants.EFFECT_INF_DURATION))
            {
                return Magnitude();
            }
            float DeltaLevel = 0.0f;
            float percentTime = 1.0f;
            if (runTime < envEffect.AttackTime)
            {
                DeltaLevel = envEffect.AttackLevel - Magnitude();
                percentTime = (float)(envEffect.AttackTime - runTime) / (float)envEffect.AttackTime;
            }
            else if (m_ffbHeader.Duration != vJoyConstants.EFFECT_INF_DURATION)
            {
                DeltaLevel = envEffect.FadeLevel - Magnitude();
                percentTime = (float)(runTime - fadeStart) / (float)envEffect.FadeTime;
            }
            return Magnitude() + (DeltaLevel * percentTime);
        }
    }
}
