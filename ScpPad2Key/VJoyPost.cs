using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Management;

using DisableDevice;

using ScpControl;

using vJoyInterfaceWrap;
using System.Runtime.InteropServices;

namespace ScpPad2vJoy
{
    public class VJoyPost
    {
        protected vJoy joystick;

        //Axis
        protected const Int32 MAX_AXIS = 32767; //Max DX value
        protected const Int32 AXIS_SCALE = (MAX_AXIS + 1) / 256; //Scp gives values from 0-255 (which we +1)

        public bool Start(bool[] parSelectedPads,PadSettings config)
        {
            EnableVJoy(false);
            SetupVjoy(parSelectedPads, config);
            EnableVJoy(true);

            joystick = new vJoy();

            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }
            else
            {
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n",
                joystick.GetvJoyManufacturerString(),
                joystick.GetvJoyProductString(),
                joystick.GetvJoySerialNumberString());

                // Test if DLL matches the driver
                UInt32 DllVer = 0, DrvVer = 0;
                bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
                if (match)
                    Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
                else
                    Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n",
                    DrvVer, DllVer);
            }

            for (uint dsID = 1; dsID <= 4; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID);

                    ///// Write access to vJoy Device - Basic
                    // Acquire the target
                    VjdStat status = joystick.GetVJDStatus(id);
                    if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
                    {
                        Console.WriteLine(String.Format("Failed to acquire vJoy device number {0}.", id));
                        Stop(parSelectedPads);
                        return false;
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Acquired: vJoy device number {0}.", id));
                    }
                    Console.WriteLine(String.Format("Buttons : {0}.", joystick.GetVJDButtonNumber(id)));
                    Console.WriteLine(String.Format("DiscPov : {0}.", joystick.GetVJDDiscPovNumber(id)));
                    Console.WriteLine(String.Format("ContPov : {0}.", joystick.GetVJDContPovNumber(id)));
                    //Console.WriteLine(String.Format("X Axis  : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X)));
                    //Console.WriteLine(String.Format("Y Axis  : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y)));
                    //Console.WriteLine(String.Format("Z Axis  : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z)));
                    //Console.WriteLine(String.Format("RX Axis : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX)));
                    //Console.WriteLine(String.Format("RY Axis : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RY)));
                    //Console.WriteLine(String.Format("RZ Axis : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ)));
                    //Console.WriteLine(String.Format("Slider0 : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_SL0)));
                    //Console.WriteLine(String.Format("Slider1 : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_SL1)));
                    //Console.WriteLine(String.Format("Wheel   : {0}.", joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_WHL)));
                    // Reset this device to default values
                    joystick.ResetVJD(id);
                }
            }
            return true;
        }

        public void Stop(bool[] parSelectedPads)
        {
            for (uint dsID=1; dsID <= 4; dsID++)
            {
                if (parSelectedPads[dsID - 1])
                {
                    uint id = GetvjFromDS(dsID);
                    try
                    {
                        joystick.RelinquishVJD(id);

                    }
                    catch
                    {
                        //if this ever produces an error
                    }
                }
            }
            EnableVJoy(false);
        }

        public uint GetvjFromDS(uint parDSid)
        {
            return parDSid;
        }

        public void JoyButton(uint parButtonID, bool parDown,uint parDSid)
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
                joystick.SetAxis((int)((parValue + 1) * AXIS_SCALE) - 64, id, parAxis);
            }
        }

        public void JoyPov(Direction parPOV,uint parDSid)
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
                //Console.Out.WriteLine(info);
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
            VJC.DeleteHidReportDescFromReg(0);
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

        //public void FfbInterface(object dialog, uint parDSid)
        //{
        //    // Start FFB Mechanism
        //    if (!joystick.Ff))
        //    throw new Exception(String.Format("Cannot start Forcefeedback on device {0}", id));
        //    // Convert Form to pointer and pass it as user data to the callback function
        //    GCHandle h = GCHandle.Alloc(dialog);
        //    IntPtr parameter = (IntPtr)h;
        //    // Register the callback function & pass the dialog box object
        //    joystick.FfbRegisterGenCB(OnEffectObj, dialog);
        //}
    }
}
