
namespace ScpPad2vJoy.vJ.FFB.Effect
{
    struct EffectReturnValue
    {
        public float MotorLeft;
        public float MotorRight;
        public EffectReturnValue(float Left, float Right)
        {
            MotorLeft = Left;
            MotorRight = Right;
        }
        public static EffectReturnValue operator +(EffectReturnValue c1, EffectReturnValue c2)
        {
            return new EffectReturnValue(c1.MotorLeft + c2.MotorLeft, c1.MotorRight + c2.MotorRight);
        }
    }
}
