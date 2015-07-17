﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Management;

using DisableDevice;

//using ScpControl;

using vJoyInterfaceWrap;
using System.Runtime.InteropServices;
using ScpPad2vJoy.VjoyEffect;
using System.Diagnostics;

namespace ScpPad2vJoy
{
    class VJoyPost
    {
        protected vJoy joystick;
        protected vJoyVibrate vibrationCore;
        protected vJoy.JoystickState[] joyReport = new vJoy.JoystickState[4];
        protected UInt32 vJoyVersion = 0;

        //vJoy
        protected const UInt32 MIN_VER = 0x205;
        //Axis
        protected const Int32 MAX_VJOY_AXIS = 32767; //Max vJoy value (0-32767)
        protected const Int32 HALF_VJOY_AXIS = MAX_VJOY_AXIS / 2;
        protected const Int32 MAX_SCP_AXIS = 255; //Max SCP value (0-255)
        protected const Int32 AXIS_SCALE = (MAX_VJOY_AXIS + 1) / (MAX_SCP_AXIS+1); //(+1 the max values to give interger scale)
        protected const Int32 AXIS_SCALE_OFFSET = AXIS_SCALE / 2; //Offset needs to be applied to account for us (adding 1 the max values)

        //Events
        public event vJoyVibrate.VibrationEventHandler VibrationCommand;
        private void VibbEventProxy(uint parvjid, EffectReturnValue e)
        {
            if (VibrationCommand == null)
                return;
            VibrationCommand(GetDSFromvj(parvjid), e);
        }

        public bool Start(bool[] parSelectedPads, PadSettings config, DeviceManagement devManLevel)
        {

            //Setup vJoy
            if ((devManLevel & DeviceManagement.vJoy_Config) == DeviceManagement.vJoy_Config)
            {
                EnableVJoy(false);
                SetupVjoy(parSelectedPads, config);
                EnableVJoy(true);
            }
            else if ((devManLevel & DeviceManagement.vJoy_Device) == DeviceManagement.vJoy_Device)
            {
                EnableVJoy(true);
            }

            joystick = new vJoy();

            if (!joystick.vJoyEnabled())
            {
                Trace.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }
            else
            {
                Trace.WriteLine(String.Format("Vendor : {0}\nProduct: {1}\nVersion: {2}\n",
                joystick.GetvJoyManufacturerString(),
                joystick.GetvJoyProductString(),
                joystick.GetvJoySerialNumberString()));

                // Test if DLL matches the driver
                UInt32 DllVer = 0, DrvVer = 0;
                bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
                if (match)
                {
                    Trace.WriteLine(String.Format("Version of Driver Matches DLL Version ({0:X})", DllVer));
                    Trace.WriteLine(String.Format("Version of vJoyInterfaceWrap.dll is ({0})", 
                        typeof(vJoy).Assembly.GetName().Version));
                    Trace.WriteLine(String.Format("Version of ScpControl.dll is ({0})\n",
                        typeof(ScpControl.ScpProxy).Assembly.GetName().Version));
                }
                else
                {
                    Trace.WriteLine(String.Format("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer));
                    Stop(parSelectedPads, devManLevel);
                    return false;
                }
                //MinVersion Check
                vJoyVersion = DrvVer;
                if (vJoyVersion < MIN_VER)
                {
                    Trace.WriteLine("vJoy version less than required: Aborting\n");
                    Stop(parSelectedPads, devManLevel);
                    return false;
                }
            }

            for (uint dsID = 1; dsID <= 4; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID);

                    // Acquire the target
                    VjdStat status = joystick.GetVJDStatus(id);
                    if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
                    {
                        Trace.WriteLine(String.Format("Failed to acquire vJoy device number {0}.", id));
                        Stop(parSelectedPads, devManLevel);
                        return false;
                    }
                    else
                    {
                        Trace.WriteLine(String.Format("Acquired: vJoy device number {0}.", id));
                    }
                    Trace.WriteLine(String.Format("Buttons : {0}.", joystick.GetVJDButtonNumber(id)));
                    Trace.WriteLine(String.Format("DiscPov : {0}.", joystick.GetVJDDiscPovNumber(id)));
                    Trace.WriteLine(String.Format("ContPov : {0}.", joystick.GetVJDContPovNumber(id)));
                    //FFB
                    if (vJoyVersion >= 0x215)
                    {
                        vibrationCore = new vJoyVibrate(joystick);
                        vibrationCore.FfbInterface(dsID);
                        vibrationCore.VibrationCommand += VibbEventProxy;
                    }
                    // Reset this device to default values
                    joystick.ResetVJD(id);
                    //Set Axis to mid value
                    joyReport[dsID - 1].AxisX = HALF_VJOY_AXIS;
                    joyReport[dsID - 1].AxisY = HALF_VJOY_AXIS;
                    joyReport[dsID - 1].AxisZ = HALF_VJOY_AXIS;
                    joyReport[dsID - 1].AxisXRot = HALF_VJOY_AXIS;
                    joyReport[dsID - 1].AxisYRot = HALF_VJOY_AXIS;
                    joyReport[dsID - 1].AxisZRot = HALF_VJOY_AXIS;
                    joyReport[dsID - 1].Slider = HALF_VJOY_AXIS;
                    joyReport[dsID - 1].Dial = HALF_VJOY_AXIS;
                }
            }
            return true;
        }

        public void Stop(bool[] parSelectedPads, DeviceManagement devManLevel)
        {
            for (uint dsID=1; dsID <= 4; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID);
                    try
                    {
                        if (vJoyVersion >= 0x215)
                        {
                            vibrationCore.FfbStop(id);
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
                    if (parButtonID < 33) //1-32
                    {
                        joyReport[parDSid - 1].Buttons |= (uint)(0x1 << (int)(parButtonID - 1));
                    }
                    else if (parButtonID < 65) //33-64
                    {
                        joyReport[parDSid - 1].ButtonsEx1 |= (uint)(0x1 << (int)(parButtonID - 33));
                    }
                    else if (parButtonID < 97) //65-96
                    {
                        joyReport[parDSid - 1].ButtonsEx2 |= (uint)(0x1 << (int)(parButtonID - 65));
                    }
                    else //97-128
                    {
                        joyReport[parDSid - 1].ButtonsEx3 |= (uint)(0x1 << (int)(parButtonID - 97));
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
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                case Direction.UpRight:
                    value = 4500;
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                case Direction.DownLeft:
                    value = 22500;
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                case Direction.DownRight:
                    value = 13500;
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                case Direction.Up:
                    value = 0;
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                case Direction.Down:
                    value = 18000;
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                case Direction.Left:
                    value = 27000;
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                case Direction.Right:
                    value = 9000;
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
                default:
                    value = unchecked((uint)-1);
                    joyReport[parDSid - 1].bHats = value;
                    //joyReport[parDSid - 1].bHatsEx1 = value;
                    //joyReport[parDSid - 1].bHatsEx2 = value;
                    //joyReport[parDSid - 1].bHatsEx3 = value;
                    break;
            }
        }

        public void JoySubmit(uint parDSid)
        {
            uint id = GetvjFromDS(parDSid);
            joystick.UpdateVJD(id, ref joyReport[parDSid-1]);
            joyReport[parDSid - 1].Buttons = 0;
            joyReport[parDSid - 1].ButtonsEx1 = 0;
            joyReport[parDSid - 1].ButtonsEx2 = 0;
            joyReport[parDSid - 1].ButtonsEx3 = 0;
            //Set Axis to mid value
            //Needed as ButtonsAsAxis don't reset to axis
            joyReport[parDSid - 1].AxisX = HALF_VJOY_AXIS;
            joyReport[parDSid - 1].AxisY = HALF_VJOY_AXIS;
            joyReport[parDSid - 1].AxisZ = HALF_VJOY_AXIS;
            joyReport[parDSid - 1].AxisXRot = HALF_VJOY_AXIS;
            joyReport[parDSid - 1].AxisYRot = HALF_VJOY_AXIS;
            joyReport[parDSid - 1].AxisZRot = HALF_VJOY_AXIS;
            joyReport[parDSid - 1].Slider = HALF_VJOY_AXIS;
            joyReport[parDSid - 1].Dial = HALF_VJOY_AXIS;
        }

        public void EnableVJoy(bool enable)
        {
            string strClassGUID = "{745a17a0-74d3-11d0-b6fe-00a0c90f57da}";
            Guid ClassGUID = new Guid(strClassGUID);
            //find all vjoy pads and enable them
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver WHERE ClassGuid = '" + strClassGUID + "' AND DeviceName = 'vJoy Device'");

            ManagementObjectCollection objCollection = objSearcher.Get();

            //We expect only one vJoy Device to be returned
            string objdeviceid = "";
            foreach (ManagementObject cobj in objCollection)
            {
                //string info = String.Format("Device='{0}',Manufacturer='{1}',DriverVersion='{2}' ", cobj["HardWareID"], cobj["DeviceID"], cobj["DeviceName"]);
                //Trace.Out.WriteLine(info);
                objdeviceid = (string)cobj["DeviceID"];
                break;
            }
            if (objdeviceid !="")
            {
                DeviceHelper.SetDeviceEnabled(ClassGUID, objdeviceid, enable);
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
            for (uint dsID = 1; dsID <= 4; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID);
                    byte[] PadConfig = VJC.CreateHidReportDesc((byte)id, config.enabledAxis, dpads, 0, config.nButtons);
                    VJC.WriteHidReportDescToReg((int)id, PadConfig);
                }
            }
        }
    }
}
