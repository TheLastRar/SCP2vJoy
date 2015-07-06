using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpDX.DirectInput;
using DisableDevice;

namespace ScpPad2vJoy
{
    public class DXinputLocker
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

        public DXinputLocker(Control parParentForm)
        {
            parentForm = parParentForm;
        }
        public void LockDevices()//Disable all dinput devices for xinput controllers
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
                    lockedDevices.Add(new DeviceID(deviceGUID,devicePath));

                    DeviceHelper.SetDeviceEnabled(deviceGUID, devicePath, false);
                }
            }
            
        }
        public void UnlockDevices() //re-enable them
        {
            foreach (DeviceID deviceid in lockedDevices)
            {
                DeviceHelper.SetDeviceEnabled(deviceid.classGUID, deviceid.instancePath, true);
            }
            lockedDevices.Clear();
        }
    }
}
