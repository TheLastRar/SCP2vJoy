using ScpPad2vJoy.vJ.FFB;
using ScpPad2vJoy.vJ.FFB.Effect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using ScpControl;

using vJoyInterfaceWrap;

namespace ScpPad2vJoy.vJ
{
    class vJoyInstall
    {
        //See wrapper.h in vJoy sourcecode for full definitions
        [DllImport("vJoyInstall.dll", EntryPoint = "enable")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Enable(UInt16 Revision);
        [DllImport("vJoyInstall.dll", EntryPoint = "disable")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Disable(UInt16 Revision);
        [DllImport("vJoyInstall.dll", EntryPoint = "refresh_vjoy")]
        public static extern void RefreshvJoy();
    }

    class vJoyPost
    {
        protected vJoy joystick;
        protected vJoyVibrate vibrationCore;
        protected vJoy.JoystickState[] joyReport = new vJoy.JoystickState[SCPConstants.MAX_XINPUT_DEVICES];
        protected UInt32 vJoyVersion = 0;

        protected const Int32 AXIS_SCALE = (vJoyConstants.MAX_AXIS_VALUE + 1) / (SCPConstants.MAX_SCP_AXIS + 1); //(+1 the max values to give interger scale)
        protected const Int32 AXIS_SCALE_OFFSET = AXIS_SCALE / 2; //Offset needs to be applied to account for us adding 1 the max values

        //Events
        public event vJoyVibrate.VibrationEventHandler VibrationCommand;
        private void VibEventProxy(uint parvjid, EffectReturnValue e)
        {
            if (VibrationCommand == null)
                return;
            VibrationCommand(GetDSFromvj(parvjid), e);
        }

        public bool Start(bool[] parSelectedPads, PadSettings config, DeviceManagement devManLevel)
        {
            //Setup vJoy
            //Perform device enable/disable based on dll version
            //EnableVJoy needs to know which version of vJoy we are running
            joystick = new vJoy();
            UInt32 DllVer = 0, DrvVer = 0;
            joystick.DriverMatch(ref DllVer, ref DrvVer);
            //MIN Version Check 1
            vJoyVersion = DllVer;
            if (vJoyVersion < vJoyConstants.MIN_VER)
            {
                Trace.WriteLine("vJoy version less than required: Aborting\n");
                Stop(parSelectedPads, devManLevel);
                return false;
            }

            if ((devManLevel & DeviceManagement.vJoy_Config) == DeviceManagement.vJoy_Config)
            {
                EnableVJoy(false);
                SetupVjoy(parSelectedPads, config);
                vJoyInstall.RefreshvJoy(); //do it like vJConfig does (needed in 2.1.6)
                EnableVJoy(true);
            }
            else if ((devManLevel & DeviceManagement.vJoy_Device) == DeviceManagement.vJoy_Device)
            {
                EnableVJoy(true);
            }

            if (!joystick.vJoyEnabled())
            {
                Trace.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }
            else
            {
                Trace.WriteLine(string.Format("Vendor : {0}\nProduct: {1}\nVersion: {2}\n",
                joystick.GetvJoyManufacturerString(),
                joystick.GetvJoyProductString(),
                joystick.GetvJoySerialNumberString()));

                // Test if DLL matches the driver
                bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
                if (match)
                {
                    Trace.WriteLine(string.Format("Version of Driver Matches DLL Version ({0:X})", DllVer));
                    Trace.WriteLine(string.Format("Version of vJoyInterfaceWrap.dll is ({0})",
                        typeof(vJoy).Assembly.GetName().Version));
                    Trace.WriteLine(string.Format("Version of ScpControl.dll is ({0})\n",
                        typeof(ScpControl.ScpProxy).Assembly.GetName().Version));
                }
                else
                {
                    Trace.WriteLine(string.Format("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer));
                    Stop(parSelectedPads, devManLevel);
                    return false;
                }
                //MinVersion Check
                vJoyVersion = DrvVer;
                if (vJoyVersion < vJoyConstants.MIN_VER)
                {
                    Trace.WriteLine("vJoy version less than required: Aborting\n");
                    Stop(parSelectedPads, devManLevel);
                    return false;
                }
            }

            for (uint dsID = 1; dsID <= SCPConstants.MAX_XINPUT_DEVICES; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID);

                    // Acquire the target
                    VjdStat status = joystick.GetVJDStatus(id);
                    if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
                    {
                        Trace.WriteLine(string.Format("Failed to acquire vJoy device number {0}.", id));
                        Stop(parSelectedPads, devManLevel);
                        return false;
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("Acquired vJoy device number {0}.", id));
                    }
                    Trace.WriteLine(string.Format("Buttons : {0}.", joystick.GetVJDButtonNumber(id)));
                    Trace.WriteLine(string.Format("DiscPov : {0}.", joystick.GetVJDDiscPovNumber(id)));
                    Trace.WriteLine(string.Format("ContPov : {0}.", joystick.GetVJDContPovNumber(id)));
                    //FFB
                    if (config.ffb)
                    {
                        vibrationCore = new vJoyVibrate(joystick);
                        vibrationCore.FfbInterface(dsID);
                        vibrationCore.VibrationCommand += VibEventProxy;
                    }
                    // Reset this device to default values
                    joystick.ResetVJD(id);
                    //Set Axis to mid value
                    joyReport[dsID - 1].AxisX = vJoyConstants.HALF_AXIS_VALUE;
                    joyReport[dsID - 1].AxisY = vJoyConstants.HALF_AXIS_VALUE;
                    joyReport[dsID - 1].AxisZ = vJoyConstants.HALF_AXIS_VALUE;
                    joyReport[dsID - 1].AxisXRot = vJoyConstants.HALF_AXIS_VALUE;
                    joyReport[dsID - 1].AxisYRot = vJoyConstants.HALF_AXIS_VALUE;
                    joyReport[dsID - 1].AxisZRot = vJoyConstants.HALF_AXIS_VALUE;
                    joyReport[dsID - 1].Slider = vJoyConstants.HALF_AXIS_VALUE;
                    joyReport[dsID - 1].Dial = vJoyConstants.HALF_AXIS_VALUE;
                }
            }
            return true;
        }

        public void Stop(bool[] parSelectedPads, DeviceManagement devManLevel)
        {
            for (uint dsID = 1; dsID <= SCPConstants.MAX_XINPUT_DEVICES; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID);
                    try
                    {
                        if (vibrationCore != null)
                        {
                            vibrationCore.FfbStop(id);
                            vibrationCore = null;
                        }
                        joystick.RelinquishVJD(id);
                    }
                    catch
                    {
                        //if this ever produces an error
                    }
                }
            }
            if ((devManLevel & DeviceManagement.vJoy_Device) == DeviceManagement.vJoy_Device)
            {
                EnableVJoy(false);
            }
        }

        public uint GetvjFromDS(uint parDSid)
        {
            return parDSid;
        }
        public uint GetDSFromvj(uint parvjid)
        {
            return parvjid;
        }

        public void JoyButton(uint parButtonID, bool parDown, uint parDSid)
        {
            if (parButtonID != 0)
            {
                if (parDown)
                {
                    if (parButtonID < vJoyConstants.BUTTON_EX1) //1-32
                    {
                        joyReport[parDSid - 1].Buttons |= (uint)(0x1 << (int)(parButtonID - vJoyConstants.BUTTON_EX0));
                    }
                    else if (parButtonID < vJoyConstants.BUTTON_EX2) //33-64
                    {
                        joyReport[parDSid - 1].ButtonsEx1 |= (uint)(0x1 << (int)(parButtonID - vJoyConstants.BUTTON_EX1));
                    }
                    else if (parButtonID < vJoyConstants.BUTTON_EX3) //65-96
                    {
                        joyReport[parDSid - 1].ButtonsEx2 |= (uint)(0x1 << (int)(parButtonID - vJoyConstants.BUTTON_EX2));
                    }
                    else //97-128
                    {
                        joyReport[parDSid - 1].ButtonsEx3 |= (uint)(0x1 << (int)(parButtonID - vJoyConstants.BUTTON_EX3));
                    }
                }
            }
        }

        public void JoyAxis(HID_USAGES parAxis, Int32 parValue, uint parDSid)
        {
            if (parAxis != 0)
            {
                //bunch of offset correction nonsense, may need reevaluation
                //Seems to be very close, Constant offset of 0.5 seems to be present
                //Also Scaled values cannot get within 64 of vJoy limits
                //this gives pratical limits of (64-32704)
                int JoyValue = (int)((parValue + 1) * AXIS_SCALE) - AXIS_SCALE_OFFSET;

                switch (parAxis)
                {
                    case HID_USAGES.HID_USAGE_X:
                        joyReport[parDSid - 1].AxisX = JoyValue;
                        break;
                    case HID_USAGES.HID_USAGE_Y:
                        joyReport[parDSid - 1].AxisY = JoyValue;
                        break;
                    case HID_USAGES.HID_USAGE_Z:
                        joyReport[parDSid - 1].AxisZ = JoyValue;
                        break;
                    case HID_USAGES.HID_USAGE_RX:
                        joyReport[parDSid - 1].AxisXRot = JoyValue;
                        break;
                    case HID_USAGES.HID_USAGE_RY:
                        joyReport[parDSid - 1].AxisYRot = JoyValue;
                        break;
                    case HID_USAGES.HID_USAGE_RZ:
                        joyReport[parDSid - 1].AxisZRot = JoyValue;
                        break;
                    case HID_USAGES.HID_USAGE_SL0:
                        joyReport[parDSid - 1].Slider = JoyValue;
                        break;
                    case HID_USAGES.HID_USAGE_SL1:
                        joyReport[parDSid - 1].Dial = JoyValue;
                        break;
                }
            }
        }

        public void JoyPov(Direction parPOV, uint parDSid)
        {
            uint value = 0;
            switch (parPOV)
            {
                case Direction.UpLeft:
                    value = 31500;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                case Direction.UpRight:
                    value = 4500;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                case Direction.DownLeft:
                    value = 22500;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                case Direction.DownRight:
                    value = 13500;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                case Direction.Up:
                    value = 0;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                case Direction.Down:
                    value = 18000;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                case Direction.Left:
                    value = 27000;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                case Direction.Right:
                    value = 9000;
                    joyReport[parDSid - 1].bHats = value;
                    break;
                default:
                    value = unchecked((uint)-1);
                    joyReport[parDSid - 1].bHats = value;
                    break;
            }
        }

        public void ApplyDeadzone(List<DeadZone> parDeadzones, uint parDSid)
        {
            uint id = GetvjFromDS(parDSid);

            foreach (DeadZone dz in parDeadzones)
            {
                dz.Apply(ref joyReport[id - 1]);
            }
        }

        public void JoySubmit(uint parDSid)
        {
            uint id = GetvjFromDS(parDSid);

            joystick.UpdateVJD(id, ref joyReport[parDSid - 1]);
            joyReport[id - 1].Buttons = 0;
            joyReport[id - 1].ButtonsEx1 = 0;
            joyReport[id - 1].ButtonsEx2 = 0;
            joyReport[id - 1].ButtonsEx3 = 0;
            //Set Axis to mid value
            //Needed as ButtonsAsAxis don't reset to axis
            joyReport[id - 1].AxisX = vJoyConstants.HALF_AXIS_VALUE;
            joyReport[id - 1].AxisY = vJoyConstants.HALF_AXIS_VALUE;
            joyReport[id - 1].AxisZ = vJoyConstants.HALF_AXIS_VALUE;
            joyReport[id - 1].AxisXRot = vJoyConstants.HALF_AXIS_VALUE;
            joyReport[id - 1].AxisYRot = vJoyConstants.HALF_AXIS_VALUE;
            joyReport[id - 1].AxisZRot = vJoyConstants.HALF_AXIS_VALUE;
            joyReport[id - 1].Slider = vJoyConstants.HALF_AXIS_VALUE;
            joyReport[id - 1].Dial = vJoyConstants.HALF_AXIS_VALUE;
        }

        public void EnableVJoy(bool enable)
        {
            {
                //Version 2.0.5 does not have these functions
                if (enable)
                {
                    //Don't have easy way of getting driver version
                    vJoyInstall.Enable(0);
                }
                else
                {
                    vJoyInstall.Disable((UInt16)joystick.GetvJoyVersion());
                }
            }
        }

        public void SetupVjoy(bool[] parSelectedPads, PadSettings config)
        {
            VJoyConf VJC = new VJoyConf();
            //Clear out old configs
            VJC.DeleteHidReportDescFromReg(0); //TODO, Respect DevManagement levels
            //create needed pads
            byte dpads = 0;
            if (config.dpad) { dpads = 1; }
            for (uint dsID = 1; dsID <= SCPConstants.MAX_XINPUT_DEVICES; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID); //
                    //byte[] PadConfig = VJC.CreateHidReportDesc(config.nButtons, config.enabledAxis, dpads, 0,(byte)id,
                    //    false, new bool [] {false,false,false,false,false,false,false,false,false,false,false});
                    byte[] PadConfig = VJC.CreateHidReportDesc(config.nButtons, config.enabledAxis, dpads, 0, (byte)id,
                        config.ffb, //enable vibration
                        new bool[] {
                            true, //Const
                            true, //Ramp
                            true, //Square Wave
                            true, //Sine Wave
                            true, //Tri Wave
                            true, //SawUp Wave
                            true, //SawDown wave
                            false, //Spring
                            false, //Damper
                            false, //Inertia
                            false  //Friction
                        });
                    VJC.WriteHidReportDescToReg((int)id, PadConfig);
                }
            }
        }
    }
}
