using System;
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
        protected UInt32 vJoyVersion = 0;

        //vJoy
        protected const UInt32 MIN_VER = 0x205;
        //Axis
        protected const Int32 MAX_VJOY_AXIS = 32767; //Max vJoy value (0-32767)
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
                uint id = GetvjFromDS(parDSid);
                joystick.SetBtn(parDown, id, parButtonID);
            }
        }

        public void JoyAxis(HID_USAGES parAxis, Int32 parValue, uint parDSid)
        {
            if (parAxis != 0)
            {
                uint id = GetvjFromDS(parDSid);
                //bunch of offset correction nonsense, may need reevaluation
                //Seems to be very close, Constant offset of 0.5 seems to be present
                //Also Scaled values cannot get within 64 of vJoy limits
                //this gives pratical limits of (64-32704)
                joystick.SetAxis((int)((parValue + 1) * AXIS_SCALE) - AXIS_SCALE_OFFSET, id, parAxis);
            }
        }

        public void JoyPov(Direction parPOV, uint parDSid)
        {
            uint id = GetvjFromDS(parDSid);
            switch (parPOV)
            {
                case Direction.UpLeft:
                    joystick.SetContPov(31500, id, 1);
                    break;
                case Direction.UpRight:
                    joystick.SetContPov(4500, id, 1);
                    break;
                case Direction.DownLeft:
                    joystick.SetContPov(22500, id, 1);
                    break;
                case Direction.DownRight:
                    joystick.SetContPov(13500, id, 1);
                    break;
                case Direction.Up:
                    joystick.SetContPov(0, id, 1);
                    break;
                case Direction.Down:
                    joystick.SetContPov(18000, id, 1);
                    break;
                case Direction.Left:
                    joystick.SetContPov(27000, id, 1);
                    break;
                case Direction.Right:
                    joystick.SetContPov(9000, id, 1);
                    break;
                default:
                    joystick.SetContPov(-1, id, 1);
                    break;
            }
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
