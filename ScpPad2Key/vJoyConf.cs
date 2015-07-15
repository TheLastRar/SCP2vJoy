using System;
using System.Collections.Generic;
using Microsoft.Win32;

//TakenFrom https://github.com/Swizzy/vJoyConfNet/blob/master/vJoyConfNet/VJoyConf.cs

namespace ScpPad2vJoy
{
    public class VJoyConf
    {
        private const byte MaxButtons = 128;
        private const byte MinAxes = 8;
        private readonly RegistryKey _regKey;

        public VJoyConf()
        {
            _regKey = Registry.LocalMachine.CreateSubKey("SYSTEM");
            if (_regKey == null)
                throw new InvalidOperationException();
            _regKey = _regKey.CreateSubKey("CurrentControlSet");
            if (_regKey == null)
                throw new InvalidOperationException();
            _regKey = _regKey.CreateSubKey("services");
            if (_regKey == null)
                throw new InvalidOperationException();
            _regKey = _regKey.CreateSubKey("vjoy");
            if (_regKey == null)
                throw new InvalidOperationException();
            _regKey = _regKey.CreateSubKey("Parameters");
            if (_regKey == null)
                throw new InvalidOperationException();
        }

        public byte[] CreateHidReportDesc(byte reportID, bool[] axes, byte nPovHatsCont, byte nPovHatsDir, byte nButtons)
        {
            var ret = new List<byte>();

            #region Header + Collection 1

            ret.AddRange(new byte[] {
                0x05, 0x01, //USAGE_PAGE(Generic Desktop)
                0x15, 0x00, //LOGICAL_MINIMUM(0)
                0x09, 0x04, //USAGE(Joystick)
                0xA1, 0x01, //COLLECTION(Application)

                0x05, 0x01, //USAGE_PAGE(Generic Desktop)
                0x85, reportID, //REPORT_ID(x)
                0x09, 0x01, //USAGE(Pointer)
                0x15, 0x00, //LOGICAL_MINIMUM(0)
                0x26, 0xFF, 0x7F,//LOGICAL_MAXIMUM_2byte(32767)
                0x75, 0x20, //REPORT_SIZE(32)
                0x95, 0x01, //REPORT_COUNT(1)
                0xA1, 0x00  //COLLECTION(Physical)
            });
            #endregion

            #region Axes
            for (var i = 0; i < axes.Length; i++)
            { // Loop axes
                if (axes[i])
                {
                    ret.Add(0x09); //USAGE(X+offset):
                    ret.Add((byte)(0x30 + i));

                    ret.Add(0x81); ret.Add(0x02); //INPUT(Data,Var,Abs)
                }
                else
                {
                    ret.Add(0x81); ret.Add(0x01); //INPUT(Cnst,Ary,Abs)
                }
            }
            if (axes.Length < MinAxes)
            { // Assume the remaining axes are not implemented
                for (var i = 0; i < MinAxes - axes.Length; i++)
                {
                    ret.Add(0x81); ret.Add(0x01); //INPUT(Cnst,Ary,Abs)
                }
            }
            ret.Add(0xC0); // End collection
            #endregion

            #region POV
            if (nPovHatsDir > 0)
            {
                ret.AddRange(new byte[] {
                    0x15, 0x00, //LOGICAL_MINIMUM(0)
                    0x25, 0x03, //LOGICAL_MAXIMUM(3)
                    0x35, 0x00, //PHYSICAL_MINIMUM(0)
                    0x46, 0x0E, 0x01,//PHYSICAL_MAXIMUM_2byte(270)
                    0x65, 0x14, //UNIT (Eng Rot:Angular Pos)
                    //One 4-bit data  + 31 4-bit padding
                    0x75, 0x04, //REPORT_SIZE(4)
                    0x95, 0x01, //REPORT_COUNT(1)
                });

                // Insert 1-4 5-state POVs
                for (var i = 0; i < nPovHatsDir; i++)
                {
                    ret.AddRange(new byte[] {
                        0x09, 0x39, //USAGE(Hat switch)
                        0x81, 0x02  //INPUT(Data,Var,Abs)
                    });
                }
                //Insert 5-state POV place holders
                ret.AddRange(new byte[] {
                    0x95, (byte)(0x20 - nPovHatsDir), //REPORT_COUNT(31)
                    0x81, 0x01  //INPUT(Cnst,Ary,Abs)
                });

            }
            else if (nPovHatsCont > 0)
            {
                ret.AddRange(new byte[] {
                    0x15, 0x00, //LOGICAL_MINIMUM(0)
                    0x27, 0x3c, 0x8c, 0x00, 0x00, //LOGICAL_MAXIMUM_4byte(35900)
                    0x35, 0x00, //PHYSICAL_MINIMUM(0)
                    0x47, 0x3c, 0x8c, 0x00, 0x00,//PHYSICAL_MAXIMUM_4byte(35900)
                    0x65, 0x14, //UNIT (Eng Rot:Angular Pos)
                    // One 4-bit data  + 31 4-bit padding
                    0x75, 0x20, //REPORT_SIZE(32)
                    0x95, 0x01, //REPORT_COUNT(1)
                });
                // Insert 1-4 continuous POVs
                for (var i = 0; i < nPovHatsCont; i++)
                {
                    ret.AddRange(new byte[] {
                        0x09, 0x39, //USAGE(Hat switch)
                        0x81, 0x02  //INPUT(Data,Var,Abs)
                    });
                }
                // Insert 1-3 continuous POV place holders
                ret.AddRange(new byte[] {
                    0x95, (byte)(0x04-nPovHatsCont), //REPORT_COUNT(3)
                    0x81, 0x01  //INPUT(Cnst,Ary,Abs)
                });
            }
            else
            {
                // Sixteen 4-bit padding
                ret.AddRange(new byte[] {
                    0x75, 0x20, //REPORT_SIZE(32)
                    0x95, 0x04, //REPORT_COUNT(4)
                    0x81, 0x01, //INPUT(Cnst,Ary,Abs)
                });
            }
            #endregion

            #region Buttons

            ret.AddRange(new byte[] {
                0x05, 0x09, //USAGE_PAGE(Buttons)
                0x15, 0x00, //LOGICAL_MINIMUM(0)
                0x25, 0x01, //LOGICAL_MAXIMUM(0)
                0x55, 0x00, //UNIT_EXPONENT(0)
                0x65, 0x00, //UNIT (None)
                0x19, 0x01, //USAGE_MINIMUM(1)
                0x29, nButtons, //USAGE_MAXIMUM(nButtons)
                0x75, 0x01, //REPORT_SIZE(1)
                0x95, nButtons, //REPORT_COUNT(nButtons)
                0x81, 0x02 //INPUT(Data,Var,Abs)
            });
            // Padding, if there are less than 32 buttons
            if (nButtons < MaxButtons)
            {
                ret.AddRange(new byte[] {
                    0x75, (byte)(MaxButtons - nButtons), //REPORT_SIZE(x)
                    0x95, 0x01, //REPORT_COUNT(1)
                    0x81, 0x01, //INPUT(Cnst,Ary,Abs)
                });
            }
            ret.Add(0xC0); // End collection
            #endregion

            return ret.ToArray();
        }

        public void WriteHidReportDescToReg(int target, byte[] hidReportDesc) { WriteHidReportDescToReg(target, ref hidReportDesc); }

        public void WriteHidReportDescToReg(int target, ref byte[] hidReportDesc)
        {
            if (_regKey == null)
                throw new InvalidOperationException();
            var key = _regKey.CreateSubKey(string.Format("Device{0:D2}", target));
            if (key == null)
                throw new InvalidOperationException();
            key.SetValue("HidReportDesctiptor", hidReportDesc);
            key.SetValue("HidReportDesctiptorSize", hidReportDesc.Length);
            key.Close();
        }

        public void DeleteHidReportDescFromReg(int target)
        {
            if (_regKey == null)
                throw new InvalidOperationException();
            int max, i;
            if (target != 0 || target > 16)
                i = max = target;
            else
            {
                i = 1;
                max = 16;
            }
            for (; i <= max; i++)
            {
                RegistryKey targetKey = _regKey.OpenSubKey(string.Format("Device{0:D2}", i));
                if (targetKey == null)
                {
                    continue;
                }
                targetKey.Close();

                try
                {
                    _regKey.DeleteSubKey(string.Format("Device{0:D2}", i));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught Exception :" + e.Message + " @ " + e.StackTrace);
                }
            }
        }
    }
}
