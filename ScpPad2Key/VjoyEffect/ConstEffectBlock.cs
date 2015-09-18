﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vJoyInterfaceWrap;

namespace ScpPad2vJoy.VjoyEffect
{
    class ConstEffectBlock : EnverlopeCabableBaseEffectBlack
    {
        private vJoy.FFB_EFF_CONSTANT constEffect;

        public override void PrimaryEffectData(object eff)
        {
            base.PrimaryEffectData(eff);
            constEffect = (vJoy.FFB_EFF_CONSTANT)eff;
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
            return constEffect.Magnitude;
        }
 
    }
}
