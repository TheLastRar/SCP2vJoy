using System;
using System.Collections.Generic;
using System.Linq;
using ScpControl;

using vJoyInterfaceWrap;

namespace ScpPad2vJoy
{
    #region DS structs
    public struct DSButton
    {
        private Ds4Button m_DS4;
        private Ds3Button m_DS3;

        public DSButton(Ds3Button parDs3Button, Ds4Button parDs4Button)
        {
            m_DS3 = parDs3Button;
            m_DS4 = parDs4Button;
        }
        public Ds3Button DS3
        {
            get { return m_DS3; }
        }
        public Ds4Button DS4
        {
            get { return m_DS4; }
        }

    }
    public struct DSAxis
    {
        private Ds4Axis m_DS4;
        private Ds3Axis m_DS3;
        private bool m_TriggerHigh;
        public DSAxis(Ds3Axis parDs3Axis, Ds4Axis parDs4Axis,bool parTriggerHigh)
        {
            m_DS3 = parDs3Axis;
            m_DS4 = parDs4Axis;
            m_TriggerHigh = parTriggerHigh;
        }
        public Ds3Axis DS3
        {
            get { return m_DS3; }
        }
        public Ds4Axis DS4
        {
            get { return m_DS4; }
        }
        public bool triggerHigh
        {
            get { return m_TriggerHigh; }
        }
    }
#endregion
    public class PadSettings
    {
        protected bool[] m_EnabledAxis = new bool[] {false,false,false,false,false,false,false,false};
        protected byte m_nButtons = 0;
        protected bool m_dpad = false;
        //Axis
        protected HID_USAGES m_AxisL2 = 0;
        protected HID_USAGES m_AxisR2 = 0;

        protected HID_USAGES m_AxisLX = 0;
        protected bool m_InvertLX = false;//Invert axis is a global setting
        protected HID_USAGES m_AxisLY = 0;
        protected bool m_InvertLY = false;

        protected HID_USAGES m_AxisRX = 0;
        protected bool m_InvertRX = false;
        protected HID_USAGES m_AxisRY = 0;
        protected bool m_InvertRY = false;

        //Buttons
        protected uint m_Cross = 0;
        protected uint m_Circle = 0;
        protected uint m_Square = 0;
        protected uint m_Triangle = 0;
        protected uint m_L1 = 0;
        protected uint m_L2 = 0;
        protected uint m_R1 = 0;
        protected uint m_R2 = 0;
        protected uint m_Select_Share = 0;
        protected uint m_Start_Options = 0;
        protected uint m_L3 = 0;
        protected uint m_R3 = 0;
        protected uint m_PS = 0;

        protected uint m_Up = 0;
        protected uint m_Down = 0;
        protected uint m_Left = 0;
        protected uint m_Right = 0;

        //AxisAsButton
        protected uint m_aLUp = 0;
        protected uint m_aLDown = 0;
        protected uint m_aLLeft = 0;
        protected uint m_aLRight = 0;

        protected uint m_aRUp = 0;
        protected uint m_aRDown = 0;
        protected uint m_aRLeft = 0;
        protected uint m_aRRight = 0;
        //POV
        protected DSButton m_PUp;
        protected DSButton m_PDown;
        protected DSButton m_PLeft;
        protected DSButton m_PRight;
        //Axis as Button POV
        protected DSAxis m_aPUp;
        protected DSAxis m_aPDown;
        protected DSAxis m_aPLeft;
        protected DSAxis m_aPRight;

        public PadSettings(string[] SettingsFile)
        {
            SettingsFile = SettingsFile.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            SettingsFile = SettingsFile.Where(x => !x.StartsWith("#")).ToArray();
            for (int Line = 0; Line < SettingsFile.Length; Line++)
            {
                string[] Setting = SettingsFile[Line].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (Setting.Length == 2)
                {
                    switch (Setting[1])
                    {
                        #region Triggers
                        case "L2":
                            {
                                if (Setting[0].StartsWith("A"))//Using triggers as axis
                                {
                                    AssignAxis(ref m_AxisL2, Setting[0]);
                                }
                                else //Using Buttons (or Pov)
                                {
                                    AssignButton(ref m_L2, new DSButton(Ds3Button.L2, Ds4Button.L2), Setting[0]);
                                }
                            }
                            break;
                        case "R2":
                            {
                                if (Setting[0].StartsWith("A"))//Using triggers as axis
                                {
                                    AssignAxis(ref m_AxisR2, Setting[0]);
                                }
                                else //Using Buttons (or Pov)
                                {
                                    AssignButton(ref m_R2, new DSButton(Ds3Button.R2, Ds4Button.R2), Setting[0]);
                                }
                            }
                            break;
                        #endregion
                        #region Axis
                        case "-LX":
                            {
                                m_InvertLX = true;
                                goto case "LX";
                            }
                        case "LX":
                            {
                                AssignAxis(ref m_AxisLX, Setting[0]);
                            }
                            break;
                        case "-LY":
                            {
                                m_InvertLY = true;
                                goto case "LY";
                            }
                        case "LY":
                            {
                                AssignAxis(ref m_AxisLY, Setting[0]);
                            }
                            break;
                        case "-RX":
                            {
                                m_InvertRX = true;
                                goto case "RX";
                            }
                        case "RX":
                            {
                                AssignAxis(ref m_AxisRX, Setting[0]);
                            }
                            break;
                        case "-RY":
                            {
                                m_InvertRY = true;
                                goto case "RY";
                            }
                        case "RY":
                            {
                                AssignAxis(ref m_AxisRY, Setting[0]);
                            }
                            break;
                        #endregion
                        #region buttons
                        case "CROSS":
                            {
                                AssignButton(ref m_Cross, new DSButton(Ds3Button.Cross, Ds4Button.Cross), Setting[0]);
                            }
                            break;
                        case "CIRCLE":
                            {
                                AssignButton(ref m_Circle, new DSButton(Ds3Button.Circle, Ds4Button.Circle), Setting[0]);
                            }
                            break;
                        case "SQUARE":
                            {
                                AssignButton(ref m_Square, new DSButton(Ds3Button.Square, Ds4Button.Square), Setting[0]);
                            }
                            break;
                        case "TRIANGLE":
                            {
                                AssignButton(ref m_Triangle, new DSButton(Ds3Button.Triangle, Ds4Button.Triangle), Setting[0]);
                            }
                            break;
                        case "L1":
                            {
                                AssignButton(ref m_L1, new DSButton(Ds3Button.L1, Ds4Button.L1), Setting[0]);
                            }
                            break;
                        case "R1":
                            {
                                AssignButton(ref m_R1, new DSButton(Ds3Button.R1, Ds4Button.R1), Setting[0]);
                            }
                            break;
                        case "SELECT_SHARE":
                            {
                                AssignButton(ref m_Select_Share, new DSButton(Ds3Button.Select, Ds4Button.Share), Setting[0]);
                            }
                            break;
                        case "START_OPTIONS":
                            {
                                AssignButton(ref m_Start_Options, new DSButton(Ds3Button.Start, Ds4Button.Options), Setting[0]);
                            }
                            break;
                        case "L3":
                            {
                                AssignButton(ref m_L3, new DSButton(Ds3Button.L3, Ds4Button.L3), Setting[0]);
                            }
                            break;
                        case "R3":
                            {
                                AssignButton(ref m_R3, new DSButton(Ds3Button.R3, Ds4Button.R3), Setting[0]);
                            }
                            break;
                        case "PS":
                            {
                                AssignButton(ref m_PS, new DSButton(Ds3Button.PS, Ds4Button.PS), Setting[0]);
                            }
                            break;
                        case "UP":
                            {
                                AssignButton(ref m_Up, new DSButton(Ds3Button.Up, Ds4Button.Up), Setting[0]);
                            }
                            break;
                        case "DOWN":
                            {
                                AssignButton(ref m_Down, new DSButton(Ds3Button.Down, Ds4Button.Down), Setting[0]);
                            }
                            break;
                        case "LEFT":
                            {
                                AssignButton(ref m_Left, new DSButton(Ds3Button.Left, Ds4Button.Left), Setting[0]);
                            }
                            break;
                        case "RIGHT":
                            {
                                AssignButton(ref m_Right, new DSButton(Ds3Button.Right, Ds4Button.Right), Setting[0]);
                            }
                            break;
                        #endregion
                        #region AxisAsButton
                            //TODO: Enable AxisAsButtons to work on dpad
                        case "LS_UP":
                            {
                                AssignAxisButton(ref m_aLUp, new DSAxis(Ds3Axis.LY, Ds4Axis.LY,false), Setting[0]);
                            }
                            break;
                        case "LS_DOWN":
                            {
                                AssignAxisButton(ref m_aLDown, new DSAxis(Ds3Axis.LY, Ds4Axis.LY,true), Setting[0]);
                            }
                            break;
                        case "LS_LEFT":
                            {
                                AssignAxisButton(ref m_aLLeft, new DSAxis(Ds3Axis.LX, Ds4Axis.LX,false), Setting[0]);
                            }
                            break;
                        case "LS_RIGHT":
                            {
                                AssignAxisButton(ref m_aLLeft, new DSAxis(Ds3Axis.LX, Ds4Axis.LX,true), Setting[0]);
                            }
                            break;
                        case "RS_UP":
                            {
                                AssignAxisButton(ref m_aRUp, new DSAxis(Ds3Axis.RY, Ds4Axis.RY,false), Setting[0]);
                            }
                            break;
                        case "RS_DOWN":
                            {
                                AssignAxisButton(ref m_aRDown, new DSAxis(Ds3Axis.RY, Ds4Axis.RY,true), Setting[0]);
                            }
                            break;
                        case "RS_LEFT":
                            {
                                AssignAxisButton(ref m_aRLeft, new DSAxis(Ds3Axis.RX, Ds4Axis.RX,false), Setting[0]);
                            }
                            break;
                        case "RS_RIGHT":
                            {
                                AssignAxisButton(ref m_aRRight, new DSAxis(Ds3Axis.RX, Ds4Axis.RX,true), Setting[0]);
                            }
                            break;
                        #endregion
                    }
                }
            }
        }

        protected void AssignAxis(ref HID_USAGES parSourceAxis, string parTargetAxis)
        {
            switch (parTargetAxis)
            {
                case "AX":
                    {
                        m_EnabledAxis[0] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_X;
                    }
                    break;
                    case "AY":
                    {
                        m_EnabledAxis[1] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_Y;
                    }
                    break;
                    case "AZ":
                    {
                        m_EnabledAxis[2] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_Z;
                    }
                    break;
                    case "ARX":
                    {
                        m_EnabledAxis[3] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_RX;
                    }
                    break;
                    case "ARY":
                    {
                        m_EnabledAxis[4] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_RY;
                    }
                    break;
                    case "ARZ":
                    {
                        m_EnabledAxis[5] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_RZ;
                    }
                    break;
                    case "ASL0":
                    {
                        m_EnabledAxis[6] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_SL0;
                    }
                    break;
                    case "ASL1":
                    {
                        m_EnabledAxis[7] = true;
                        parSourceAxis = HID_USAGES.HID_USAGE_SL1;
                    }
                    break;
            }

        }

        private void AssignButton(ref uint parSourceButton, DSButton parSourceID, string parTargetButton)
        {
            if (parTargetButton.StartsWith("B"))
            {
                //used for button
                uint buttonid = uint.Parse(parTargetButton.Remove(0, 1));
                if (buttonid > m_nButtons) { m_nButtons = (byte)buttonid; }
                parSourceButton = buttonid;
            }
            else if (parTargetButton.StartsWith("P"))
            {
                //used for POV
                m_dpad = true;
                switch (parTargetButton)
                {
                    case "PU":
                        {
                            m_PUp = parSourceID;
                        }
                        break;
                    case "PD":
                        {
                            m_PDown = parSourceID;
                        }
                        break;
                    case "PL":
                        {
                            m_PLeft = parSourceID;
                        }
                        break;
                    case "PR":
                        {
                            m_PRight = parSourceID;
                        }
                        break;
                }
            } 
            else if (parTargetButton.StartsWith("A"))
            {
                //Button As Axis
                uint axisCode = 0;
                string[] Setting = parTargetButton.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (Setting[1] == "HI")
                {
                    axisCode = 2000;
                }
                else
                {
                    axisCode = 1000;
                }
                HID_USAGES TargetAxis = 0;
                AssignAxis(ref TargetAxis, Setting[0]);
                axisCode += (uint)TargetAxis;
                parSourceButton = axisCode;
            }

        }
        private void AssignAxisButton(ref uint parSourceButton, DSAxis parSourceID, string parTargetButton)
        {
            if (parTargetButton.StartsWith("B"))
            {
                //used for button
                uint buttonid = uint.Parse(parTargetButton.Remove(0, 1));
                if (buttonid > m_nButtons) { m_nButtons = (byte)buttonid; }
                parSourceButton = buttonid;
            }
            else if (parTargetButton.StartsWith("P"))
            {
                //used for POV
                m_dpad = true;
                switch (parTargetButton)
                {
                    case "PU":
                        {
                            m_aPUp = parSourceID;
                        }
                        break;
                    case "PD":
                        {
                            m_aPDown = parSourceID;
                        }
                        break;
                    case "PL":
                        {
                            m_aPLeft = parSourceID;
                        }
                        break;
                    case "PR":
                        {
                            m_aPRight = parSourceID;
                        }
                        break;
                }
            }

        }   

#region Getters
        //Pad details
        public bool[] enabledAxis { get { return m_EnabledAxis; } }
        public byte nButtons { get { return m_nButtons; } }
        public bool dpad { get { return m_dpad; } }

        //Axis
        public HID_USAGES axisL2 { get { return m_AxisL2; } }
        public HID_USAGES axisR2 { get { return m_AxisR2; } }

        public HID_USAGES axisLX { get { return m_AxisLX; } }
        public bool invertLX { get { return m_InvertLX; } }
        public HID_USAGES axisLY { get { return m_AxisLY; } }
        public bool invertLY { get { return m_InvertLY; } }

        public HID_USAGES axisRX { get { return m_AxisRX; } }
        public bool invertRX { get { return m_InvertRX; } }
        public HID_USAGES axisRY { get { return m_AxisRY; } }
        public bool invertRY { get { return m_InvertRY; } }

        //Buttons
        public uint cross { get { return m_Cross; } }
        public uint circle { get { return m_Circle; } }
        public uint square { get { return m_Square; } }
        public uint triangle { get { return m_Triangle; } }
        public uint l1 { get { return m_L1; } }
        public uint r1 { get { return m_R1; } }
        public uint l2 { get { return m_L2; } }
        public uint r2 { get { return m_R2; } }
        public uint select_share { get { return m_Select_Share; } }
        public uint start_options { get { return m_Start_Options; } }
        public uint l3 { get { return m_L3; } }
        public uint r3 { get { return m_R3; } }
        public uint ps { get { return m_PS; } }

        public uint up { get { return m_Up; } }
        public uint down { get { return m_Down; } }
        public uint left { get { return m_Left; } }
        public uint right { get { return m_Right; } }

        //AxisAsButtons
        public uint aLUp { get { return m_aLUp; } }
        public uint aLDown { get { return m_aLDown; } }
        public uint aLLeft { get { return m_aLLeft; } }
        public uint aLRight { get { return m_aLRight; } }

        public uint aRUp { get { return m_aRUp; } }
        public uint aRDown { get { return m_aRDown; } }
        public uint aRLeft { get { return m_aRLeft; } }
        public uint aRRight { get { return m_aRRight; } }
        //pov
        public DSButton pUp { get { return m_PUp; } }
        public DSButton pDown { get { return m_PDown; } }
        public DSButton pLeft { get { return m_PLeft; } }
        public DSButton pRight { get { return m_PRight; } }
        //Axis As Button Pov
        public DSAxis aPUp { get { return m_aPUp; } }
        public DSAxis aPDown { get { return m_aPDown; } }
        public DSAxis aPLeft { get { return m_aPLeft; } }
        public DSAxis aPRight { get { return m_aPRight; } }
#endregion
    }
}
