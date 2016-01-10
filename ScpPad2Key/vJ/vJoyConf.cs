using Microsoft.Win32;
using ScpPad2vJoy.vJ.FFB;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//TakenFrom https://github.com/Swizzy/vJoyConfNet/blob/master/vJoyConfNet/VJoyConf.cs
//Taken from RC3 of vjoy 2.1.6
namespace ScpPad2vJoy.vJ
{
    public class VJoyConf
    {
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

        public byte[] CreateHidReportDesc(byte nButtons, bool[] axes, byte nPovHatsCont, byte nPovHatsDir, byte reportID, bool ffb, bool[] ffbEffects)
        {
            var ret = new List<byte>();

            #region Header + Collection 1

            ret.AddRange(new byte[] {
                HidToken.HIDP_GLOBAL_USAGE_PAGE_1,      //USAGE_PAGE(Generic Desktop)
                HidUsage.HID_USAGE_PAGE_GENERIC,
                HidToken.HIDP_GLOBAL_LOG_MIN_1,         //LOGICAL_MINIMUM(0)
                0x00,
                HidToken.HIDP_LOCAL_USAGE_1,            //USAGE(Joystick)
                HidUsage.HID_USAGE_GENERIC_JOYSTICK,
                HidToken.HIDP_MAIN_COLLECTION,          //COLLECTION(Application)
                HidToken.HIDP_MAIN_COLLECTION_APP,

                HidToken.HIDP_GLOBAL_USAGE_PAGE_1,      //USAGE_PAGE(Generic Desktop)
                HidUsage.HID_USAGE_PAGE_GENERIC,
                HidToken.HIDP_GLOBAL_REPORT_ID,         //REPORT_ID(x)
                reportID,
                HidToken.HIDP_LOCAL_USAGE_1,            //USAGE(Pointer)
                HidUsage.HID_USAGE_GENERIC_POINTER,
                HidToken.HIDP_GLOBAL_LOG_MIN_1,         //LOGICAL_MINIMUM(0)
                0x00,
                HidToken.HIDP_GLOBAL_LOG_MAX_2,         //LOGICAL_MAXIMUM_2byte(32767)
                0xFF, 0x7F,
                HidToken.HIDP_GLOBAL_REPORT_SIZE,       //REPORT_SIZE(32)
                0x20,
                HidToken.HIDP_GLOBAL_REPORT_COUNT_1,    //REPORT_COUNT(1)
                0x01,
                HidToken.HIDP_MAIN_COLLECTION,          //COLLECTION(Physical)
                0x00
            });
            #endregion

            #region Axes
            for (var i = 0; i < axes.Length; i++)
            { // Loop axes
                if (axes[i])
                {
                    ret.Add(HidToken.HIDP_LOCAL_USAGE_1);//USAGE(X+offset):
                    ret.Add((byte)(HidUsage.HID_USAGE_GENERIC_X + i));

                    ret.Add(HidToken.HIDP_MAIN_INPUT_1);//INPUT(Data,Var,Abs)
                    ret.Add(0x02);
                }
                else
                {
                    ret.Add(HidToken.HIDP_MAIN_INPUT_1);//INPUT(Cnst,Ary,Abs)
                    ret.Add(0x01);
                }
            }
            if (axes.Length < vJoyConstants.MAX_AXIS_COUNT)
            { // Assume the remaining axes are not implemented
                for (var i = 0; i < vJoyConstants.MAX_AXIS_COUNT - axes.Length; i++)
                {
                    ret.Add(HidToken.HIDP_MAIN_INPUT_1);//INPUT(Cnst,Ary,Abs)
                    ret.Add(0x01);
                }
            }
            ret.Add(HidToken.HIDP_MAIN_ENDCOLLECTION);  //End collection
            #endregion

            #region POV
            if (nPovHatsDir > 0)
            {
                ret.AddRange(new byte[] {
                    HidToken.HIDP_GLOBAL_LOG_MIN_1,     //LOGICAL_MINIMUM(0)
                    0x00,
                    HidToken.HIDP_GLOBAL_LOG_MAX_1,     //LOGICAL_MAXIMUM(3)
                    0x03,
                    HidToken.HIDP_GLOBAL_PHY_MIN_1,     //PHYSICAL_MINIMUM(0)
                    0x00,
                    HidToken.HIDP_GLOBAL_PHY_MAX_2,     //PHYSICAL_MAXIMUM_2byte(270)
                    0x0E, 0x01,
                    HidToken.HIDP_GLOBAL_UNIT_1,        //UNIT (Eng Rot:Angular Pos)
                    0x14, 
                    //One 4-bit data  + 31 4-bit padding
                    HidToken.HIDP_GLOBAL_REPORT_SIZE,   //REPORT_SIZE(4)
                    0x04,
                    HidToken.HIDP_GLOBAL_REPORT_COUNT_1,//REPORT_COUNT(1)
                    0x01,
                });

                // Insert 1-4 5-state POVs
                for (var i = 0; i < nPovHatsDir; i++)
                {
                    ret.AddRange(new byte[] {
                        HidToken.HIDP_LOCAL_USAGE_1,    //USAGE(Hat switch)
                        HidUsage.HID_USAGE_GENERIC_HATSWITCH,
                        HidToken.HIDP_MAIN_INPUT_1,     //INPUT(Data,Var,Abs)
                        0x02
                    });
                }
                //Insert 5-state POV place holders
                ret.AddRange(new byte[] {
                    HidToken.HIDP_GLOBAL_REPORT_COUNT_1,//REPORT_COUNT(31)
                    (byte)(0x20 - nPovHatsDir),
                    HidToken.HIDP_MAIN_INPUT_1,
                    0x01  //INPUT(Cnst,Ary,Abs)
                });

            }
            else if (nPovHatsCont > 0)
            {
                ret.AddRange(new byte[] {
                    HidToken.HIDP_GLOBAL_LOG_MIN_1,     //LOGICAL_MINIMUM(0)
                    0x00,
                    HidToken.HIDP_GLOBAL_LOG_MAX_4,     //LOGICAL_MAXIMUM_4byte(35900)
                    0x3c, 0x8c, 0x00, 0x00,
                    HidToken.HIDP_GLOBAL_PHY_MIN_1,     //PHYSICAL_MINIMUM(0)
                    0x00,
                    HidToken.HIDP_GLOBAL_PHY_MAX_4,     //PHYSICAL_MAXIMUM_4byte(35900)
                    0x3c, 0x8c, 0x00, 0x00,
                    HidToken.HIDP_GLOBAL_UNIT_1,        //UNIT (Eng Rot:Angular Pos)
                    0x14, 
                    //
                    HidToken.HIDP_GLOBAL_REPORT_SIZE,   //REPORT_SIZE(32)
                    0x20,
                    HidToken.HIDP_GLOBAL_REPORT_COUNT_1,//REPORT_COUNT(1)
                    0x01,
                });
                // Insert 1-4 continuous POVs
                for (var i = 0; i < nPovHatsCont; i++)
                {
                    ret.AddRange(new byte[] {
                        HidToken.HIDP_LOCAL_USAGE_1,    //USAGE(Hat switch)
                        HidUsage.HID_USAGE_GENERIC_HATSWITCH,
                        HidToken.HIDP_MAIN_INPUT_1,     //INPUT(Data,Var,Abs)
                        0x02
                    });
                }
                // Insert 1-3 continuous POV place holders
                ret.AddRange(new byte[] {
                    HidToken.HIDP_GLOBAL_REPORT_COUNT_1,//REPORT_COUNT(3)
                    (byte)(0x04-nPovHatsCont),
                    HidToken.HIDP_MAIN_INPUT_1,         //INPUT(Cnst,Ary,Abs)
                    0x01
                });
            }
            else
            {
                // Sixteen 4-bit padding
                ret.AddRange(new byte[] {
                    HidToken.HIDP_GLOBAL_REPORT_SIZE,   //REPORT_SIZE(32)
                    0x20,
                    HidToken.HIDP_GLOBAL_REPORT_COUNT_1,//REPORT_COUNT(4)
                    0x04,
                    HidToken.HIDP_MAIN_INPUT_1,         //INPUT(Cnst,Ary,Abs)
                    0x01,
                });
            }
            #endregion

            #region Buttons

            ret.AddRange(new byte[] {
                HidToken.HIDP_GLOBAL_USAGE_PAGE_1,      //USAGE_PAGE(Buttons)
                HidUsage.HID_USAGE_PAGE_BUTTON,
                HidToken.HIDP_GLOBAL_LOG_MIN_1,         //LOGICAL_MINIMUM(0)
                0x00,
                HidToken.HIDP_GLOBAL_LOG_MAX_1,         //LOGICAL_MAXIMUM(0)
                0x01,
                HidToken.HIDP_GLOBAL_UNIT_EXP_1,        //UNIT_EXPONENT(0)
                0x00,
                HidToken.HIDP_GLOBAL_UNIT_1,            //UNIT (None)
                0x00,
                HidToken.HIDP_LOCAL_USAGE_MIN_1,        //USAGE_MINIMUM(1)
                0x01,
                HidToken.HIDP_LOCAL_USAGE_MAX_1,        //USAGE_MAXIMUM(nButtons)
                nButtons,
                HidToken.HIDP_GLOBAL_REPORT_SIZE,       //REPORT_SIZE(1)
                0x01,
                HidToken.HIDP_GLOBAL_REPORT_COUNT_1,    //REPORT_COUNT(nButtons)
                nButtons,
                HidToken.HIDP_MAIN_INPUT_1,             //INPUT(Data,Var,Abs)
                0x02
            });
            // Padding, if there are less than 32 buttons
            if (nButtons < vJoyConstants.MAX_BUTTON_COUNT)
            {
                ret.AddRange(new byte[] {
                    HidToken.HIDP_GLOBAL_REPORT_SIZE,   //REPORT_SIZE(x)
                    (byte)(vJoyConstants.MAX_BUTTON_COUNT - nButtons),
                    HidToken.HIDP_GLOBAL_REPORT_COUNT_1,//REPORT_COUNT(1)
                    0x01,
                    HidToken.HIDP_MAIN_INPUT_1,         //INPUT(Cnst,Ary,Abs)
                    0x01,
                });
            }
            #endregion

            #region ForceFeedBack
            if (ffb)
            {
                CreateFfbDesc(ret, reportID);
                UInt16 mask = GetFfbEffectMask(ffbEffects);
                ModifyFfbEffectDesc(ret, mask);
            }
            #endregion

            ret.Add(HidToken.HIDP_MAIN_ENDCOLLECTION);  //End collection
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
                    Trace.WriteLine("Caught Exception :" + e.Message + " @ " + e.StackTrace);
                }
            }
        }

        private void CreateFfbDesc(List<byte> buffer, byte ReportId)
        {
            byte[] vars = {
                (byte)(HidReportDescFfb.HID_ID_STATE + 0x10 * ReportId),//  Report ID 2
                (byte)(FFBPType.PT_EFFREP + 0x10 * ReportId),           //  Report ID 1
                (byte)(FFBPType.PT_ENVREP + 0x10 * ReportId),           //  Report ID 2
                (byte)(FFBPType.PT_CONDREP + 0x10 * ReportId),          //  Report ID 3
                (byte)(FFBPType.PT_PRIDREP + 0x10 * ReportId),          //  Report ID 4
                (byte)(FFBPType.PT_CONSTREP + 0x10 * ReportId),         //  Report ID 5
                (byte)(FFBPType.PT_RAMPREP + 0x10 * ReportId),          //  Report ID 6
                (byte)(FFBPType.PT_CSTMREP + 0x10 * ReportId),          //  Report ID 7
                (byte)(FFBPType.PT_SMPLREP + 0x10 * ReportId),          //  Report ID 8
                (byte)(FFBPType.PT_EFOPREP + 0x10 * ReportId),          //  Report ID Ah (10d)
                (byte)(FFBPType.PT_BLKFRREP + 0x10 * ReportId),         //  Report ID Bh (11d)
                (byte)(FFBPType.PT_CTRLREP + 0x10 * ReportId),          //  Report ID Ch (12d)
                (byte)(FFBPType.PT_GAINREP + 0x10 * ReportId),          //  Report ID Dh (13d)
                (byte)(FFBPType.PT_SETCREP + 0x10 * ReportId),          //  Report ID Eh (14d)
                //Note Wrapper adds 0x10 to these values
                //But the config dosn't
                (byte)(FFBPType.PT_NEWEFREP - 0x10 + 0x10 * ReportId),  //  Report ID 1
                (byte)(FFBPType.PT_BLKLDREP - 0x10 + 0x10 * ReportId),  //  Report ID 2
                (byte)(FFBPType.PT_POOLREP - 0x10 + 0x10 * ReportId),   //  Report ID 3
            };
            // Replace the first byte of each sub vector with the corresponding varible (exclude first sub vector)
            // Append modified sub vector to buffer
            buffer.AddRange(HidReportDescFfb.FfbDescriptor[0]);
            for (uint i = 1; i < HidReportDescFfb.FfbDescriptor.Length; i++)
            {
                HidReportDescFfb.FfbDescriptor[i][0] = vars[i - 1];
                buffer.AddRange(HidReportDescFfb.FfbDescriptor[i]);
            }
        }

        UInt16 GetFfbEffectMask(bool[] cb)
        {
            UInt16 Mask = 0;
            foreach (bool Checked in cb)
            {
                //BOOL Checked = FALSE, Enabled = FALSE;
                //h = GetDlgItem(hDlgTab, effect);
                //if (Button_GetCheck(h) == BST_CHECKED)
                //    Checked = TRUE;
                //Enabled = IsWindowEnabled(h);
                if (Checked /*&& Enabled*/)
                    Mask |= 0x01;
                Mask = (UInt16)(Mask << 1);
            }

            Mask = (UInt16)(Mask >> 1);
            return Mask;
        }

        void ModifyFfbEffectDesc(List<byte> buffer, UInt16 Mask)
        {
            byte[] Effect = new byte[] { 0x26, 0x27, 0x30, 0x31, 0x32, 0x33, 0x34, 0x40, 0x41, 0x42, 0x43 };
            byte nEff = (byte)Effect.Length;

            // Search for sequence(0x09, 0x25, 0xA1, 0x02)
            for (int index = 0; index < buffer.Count; index++)
            {
                //byte i = buffer[index];
                if ((buffer[index] == 0x09) && (buffer[index + 1] == 0x25) && (buffer[index + 2] == 0xA1) && (buffer[index + 3] == 0x02))
                    // Sequence found - now replace by going over the effects 
                    for (byte e = 0; e < nEff; e++)
                    {
                        if (((Mask >> (nEff - e - 1)) & 0x01) != 0)
                            buffer[index + 5 + e * 2] = Effect[e];
                        else
                            buffer[index + 5 + e * 2] = 0x29;
                    };
                //buffer[index] = i;
            };
        }
    }
}
