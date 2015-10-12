using DisableDevice;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Management;
using System.ServiceProcess;
using System.Windows.Forms;

namespace ScpPad2vJoy
{
    public class X360_InputLocker
    {
        private Control parentForm;
        protected List<DeviceID> lockedDevices = new List<DeviceID>();
        public struct DeviceID
        {
            public Guid classGUID;
            public string instancePath;
            public DeviceID(Guid parClassGUID, string parInstacePath)
            {
                classGUID = parClassGUID;
                instancePath = parInstacePath;
            }
        }
        const string XINPUT_NAME = "Controller (XBOX 360 For Windows)";

        public X360_InputLocker(Control parParentForm)
        {
            parentForm = parParentForm;
        }
        public void Lock_XI_Devices()//Disable all Xinput controllers presented by SCP
        {
            string strClassGUID = "{d61ca365-5af4-4486-998b-9db4734c6ca3}";
            Guid ClassGUID = new Guid(strClassGUID);
            //find all "Xbox 360 Controller for Windows" pads and disable them
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver WHERE ClassGuid = '" + strClassGUID + "' AND DeviceName = 'Xbox 360 Controller for Windows'");

            ManagementObjectCollection objCollection = objSearcher.Get();

            List<String> objdeviceids = new List<String>();
            foreach (ManagementObject cobj in objCollection)
            {
                string Local = (string)cobj["Location"];
                //Dealing with X360 controllers outside of
                //SCP is untested and might cause a crash
                if (Local.Contains("SCP Virtual X360 Bus"))
                    objdeviceids.Add((string)cobj["DeviceID"]);
            }

            //Stop Service to allow us to disable needed X360 controllers
            //Stopping the service effectivly unplugs the emulated X360
            //controller, but we can still disable and re-enable it
            //If we do this while the service is running, the program will
            //crash
            ServiceController sc = new ServiceController("SCP DSx Service");
            sc.Stop();
            sc.WaitForStatus(ServiceControllerStatus.Stopped);

            foreach (String objdeviceid in objdeviceids)
            {
                if (objdeviceid != "")
                {
                    lockedDevices.Add(new DeviceID(ClassGUID, objdeviceid));
                    DeviceHelper.SetDeviceEnabled(ClassGUID, objdeviceid, false, false);
                }
            }

            //Restart service
            sc.Start();
            sc.WaitForStatus(ServiceControllerStatus.Running);
        }
        public void Lock_DX_Devices()//Disable all dinput devices for xinput controllers
        {
            var directInput = new DirectInput();

            IList<DeviceInstance> devicelist = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance cdevice in devicelist)
            {
                if (cdevice.InstanceName == XINPUT_NAME)
                {
                    var joystick = new Joystick(directInput, cdevice.InstanceGuid);
                    joystick.Acquire();
                    Guid deviceGUID = joystick.Properties.ClassGuid;
                    string devicePath = joystick.Properties.InterfacePath;
                    joystick.Unacquire();
                    string[] dpstlit = devicePath.Split('#');
                    devicePath = @"HID\" + dpstlit[1].ToUpper() + @"\" + dpstlit[2].ToUpper();
                    lockedDevices.Add(new DeviceID(deviceGUID, devicePath));

                    DeviceHelper.SetDeviceEnabled(deviceGUID, devicePath, false);
                }
            }

        }
        public void UnlockDevices() //re-enable them
        {
            foreach (DeviceID deviceid in lockedDevices)
            {
                DeviceHelper.SetDeviceEnabled(deviceid.classGUID, deviceid.instancePath, true, false);
            }
            lockedDevices.Clear();
        }
    }
}
