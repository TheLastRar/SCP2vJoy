using System.Diagnostics;

namespace ScpPad2vJoy.vJ.FFB.Effect
{
    class NullEffectBlock : BaseEffectBlock
    {
        public override void PrimaryEffectData(object eff)
        {
            Trace.WriteLine("Unkown Effect Param1 download");
        }
        public override void SecondaryEffectData(object eff)
        {
            Trace.WriteLine("Unkown Effect Param2 download");
        }
        protected override float ComputeEffect()
        {
            return 0;
        }
    }
}
