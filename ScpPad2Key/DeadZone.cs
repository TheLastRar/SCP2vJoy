using ScpPad2vJoy.vJ;
using System;
using vJoyInterfaceWrap;

namespace ScpPad2vJoy
{
    public enum AxisType
    {
        Stick = 1, //Neutral = 127.5
        Trigger = 2, //Neutral = 0
    }
    abstract class DeadZone
    {
        public abstract void Apply(ref vJoy.JoystickState state);
        protected int GetValue(ref vJoy.JoystickState joyReport, HID_USAGES hid)
        {
            switch (hid)
            {
                case HID_USAGES.HID_USAGE_X:
                    return joyReport.AxisX;
                case HID_USAGES.HID_USAGE_Y:
                    return joyReport.AxisY;
                case HID_USAGES.HID_USAGE_Z:
                    return joyReport.AxisZ;
                case HID_USAGES.HID_USAGE_RX:
                    return joyReport.AxisXRot;
                case HID_USAGES.HID_USAGE_RY:
                    return joyReport.AxisYRot;
                case HID_USAGES.HID_USAGE_RZ:
                    return joyReport.AxisZRot;
                case HID_USAGES.HID_USAGE_SL0:
                    return joyReport.Slider;
                case HID_USAGES.HID_USAGE_SL1:
                    return joyReport.Dial;
                default:
                    return 0;
            }
        }
        protected void SetValue(ref vJoy.JoystickState joyReport, HID_USAGES hid, int value)
        {
            switch (hid)
            {
                case HID_USAGES.HID_USAGE_X:
                    joyReport.AxisX = value;
                    break;
                case HID_USAGES.HID_USAGE_Y:
                    joyReport.AxisY = value;
                    break;
                case HID_USAGES.HID_USAGE_Z:
                    joyReport.AxisZ = value;
                    break;
                case HID_USAGES.HID_USAGE_RX:
                    joyReport.AxisXRot = value;
                    break;
                case HID_USAGES.HID_USAGE_RY:
                    joyReport.AxisYRot = value;
                    break;
                case HID_USAGES.HID_USAGE_RZ:
                    joyReport.AxisZRot = value;
                    break;
                case HID_USAGES.HID_USAGE_SL0:
                    joyReport.Slider = value;
                    break;
                case HID_USAGES.HID_USAGE_SL1:
                    joyReport.Dial = value;
                    break;
            }
        }
    }

    class AxialDeadZone : DeadZone
    {
        public double DeadZone; //0 to 1 

        public HID_USAGES Axis;

        public AxisType AType;

        public override void Apply(ref vJoy.JoystickState state)
        {
            double axis = 0;
            double maxValue = 0;
            switch (AType)
            {
                case AxisType.Stick:
                    axis = GetValue(ref state, Axis) - vJoyConstants.HALF_AXIS_VALUE - 0.5;
                    maxValue = vJoyConstants.HALF_AXIS_VALUE + 0.5;
                    break;
                case AxisType.Trigger:
                    axis = GetValue(ref state, Axis);
                    maxValue = vJoyConstants.MAX_AXIS_VALUE;
                    break;
            }

            double acturalDeadZone = maxValue * DeadZone;

            if (Math.Abs(axis) < acturalDeadZone)
            {
                axis = 0;
            }
            else
            {
                if (axis > 0)
                    axis = maxValue * ((axis - acturalDeadZone) / (maxValue - acturalDeadZone));
                else
                    axis = maxValue * ((axis + acturalDeadZone) / (maxValue - acturalDeadZone));
            }
            switch (AType)
            {
                case AxisType.Stick:
                    axis += vJoyConstants.HALF_AXIS_VALUE + 0.5;
                    break;
                case AxisType.Trigger:
                    break;
            }
            SetValue(ref state, Axis, (int)axis);
        }
    }
    class RadialDeadZone : DeadZone
    {
        public double DeadZone;

        public HID_USAGES AxisX;
        public HID_USAGES AxisY;

        public AxisType AType;

        public override void Apply(ref vJoy.JoystickState state)
        {
            double axisX = 0;
            double axisY = 0;

            double maxValue = 0;
            switch (AType)
            {
                case AxisType.Stick:
                    axisX = GetValue(ref state, AxisX) - vJoyConstants.HALF_AXIS_VALUE - 0.5;
                    axisY = GetValue(ref state, AxisY) - vJoyConstants.HALF_AXIS_VALUE - 0.5;
                    maxValue = vJoyConstants.HALF_AXIS_VALUE + 0.5;
                    break;
                case AxisType.Trigger:
                    axisX = GetValue(ref state, AxisX);
                    axisY = GetValue(ref state, AxisY);
                    maxValue = vJoyConstants.MAX_AXIS_VALUE;
                    break;
            }

            double acturalDeadZone = maxValue * DeadZone;

            double mag = Math.Sqrt(axisX * axisX + axisY * axisY);

            if (mag < acturalDeadZone)
            {
                axisX = 0;
                axisY = 0;
            }
            else
            {
                double normX = axisX / mag; //0-1
                double normY = axisY / mag; //0-1
                axisX = normX * maxValue * ((mag - acturalDeadZone) / (maxValue - acturalDeadZone));
                axisY = normY * maxValue * ((mag - acturalDeadZone) / (maxValue - acturalDeadZone));
            }
            switch (AType)
            {
                case AxisType.Stick:
                    axisX += vJoyConstants.HALF_AXIS_VALUE + 0.5;
                    axisY += vJoyConstants.HALF_AXIS_VALUE + 0.5;
                    break;
                case AxisType.Trigger:
                    break;
            }

            SetValue(ref state, AxisX, (int)axisX);
            SetValue(ref state, AxisY, (int)axisY);
        }
    }
}
