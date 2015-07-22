using System;

using ScpControl;

using vJoyInterfaceWrap;
using ScpPad2vJoy.VjoyEffect;
using System.Diagnostics;

namespace ScpPad2vJoy
{
    class DXPadState
    {
        protected ScpProxy m_VibProxy = null;

        protected volatile VJoyPost vJPad;
        protected volatile PadSettings config;
        public DXPadState(VJoyPost parVJoyPad, PadSettings parConfig)
        {
            vJPad = parVJoyPad;
            config = parConfig;
            vJPad.VibrationCommand += Rumble;
        }

        //AxisAsButton
        private const byte HighTrigger = 193;
        private const byte LowTrigger = 32;

        public void Update(DsPacket Data)
        {
            lock (this)
            {
                uint dsID = (uint)Data.Detail.Pad + 1;
                switch (Data.Detail.Model)
                {
                    #region DS3
                    case DsModel.DS3:
                        {
                            #region Axis
                            vJPad.JoyAxis(config.axisL2, Data.Axis(Ds3Axis.L2), dsID);
                            vJPad.JoyAxis(config.axisR2, Data.Axis(Ds3Axis.R2), dsID);
                            SetAxis(Data.Axis(Ds3Axis.LX), config.axisLX, config.invertLX, dsID);
                            SetAxis(Data.Axis(Ds3Axis.LY), config.axisLY, config.invertLY, dsID);

                            SetAxis(Data.Axis(Ds3Axis.RX), config.axisRX, config.invertRX, dsID);
                            SetAxis(Data.Axis(Ds3Axis.RY), config.axisRY, config.invertRY, dsID);
                            #endregion
                            #region Buttons
                            SetButton(Data.Button(Ds3Button.Cross), config.cross, dsID);
                            SetButton(Data.Button(Ds3Button.Circle), config.circle, dsID);
                            SetButton(Data.Button(Ds3Button.Square), config.square, dsID);
                            SetButton(Data.Button(Ds3Button.Triangle), config.triangle, dsID);
                            SetButton(Data.Button(Ds3Button.L1), config.l1, dsID);
                            SetButton(Data.Button(Ds3Button.R1), config.r1, dsID);
                            SetButton(Data.Button(Ds3Button.L2), config.l2, dsID);
                            SetButton(Data.Button(Ds3Button.R2), config.r2, dsID);
                            SetButton(Data.Button(Ds3Button.Select), config.select_share, dsID);
                            SetButton(Data.Button(Ds3Button.Start), config.start_options, dsID);
                            SetButton(Data.Button(Ds3Button.L3), config.l3, dsID);
                            SetButton(Data.Button(Ds3Button.R3), config.r3, dsID);
                            SetButton(Data.Button(Ds3Button.PS), config.ps, dsID);
                            //Dpad as button
                            SetButton(Data.Button(Ds3Button.Up), config.up, dsID);
                            SetButton(Data.Button(Ds3Button.Down), config.down, dsID);
                            SetButton(Data.Button(Ds3Button.Left), config.left, dsID);
                            SetButton(Data.Button(Ds3Button.Right), config.right, dsID);
                            //AxisAsButton
                            SetAxisAsButton(Data.Axis(Ds3Axis.LX), config.aLRight, config.aLLeft, dsID);
                            SetAxisAsButton(Data.Axis(Ds3Axis.LY), config.aLDown, config.aLUp, dsID);
                            SetAxisAsButton(Data.Axis(Ds3Axis.RX), config.aRRight, config.aRLeft, dsID);
                            SetAxisAsButton(Data.Axis(Ds3Axis.RY), config.aRDown, config.aRUp, dsID);
                            #endregion
                            #region Dpad
                            //Dpad
                            Direction DSPov = Direction.None;
                            if (CheckDpadDs3(Data, config.pUp)) { DSPov = DSPov | Direction.Up; }
                            if (CheckDpadDs3(Data, config.pDown)) { DSPov = DSPov | Direction.Down; }
                            if (CheckDpadDs3(Data, config.pLeft)) { DSPov = DSPov | Direction.Left; }
                            if (CheckDpadDs3(Data, config.pRight)) { DSPov = DSPov | Direction.Right; }
                            //Axis as Dpad

                            if (CheckDpadDs3AxisAsButton(Data, config.aPUp)) { DSPov = DSPov | Direction.Up; }
                            if (CheckDpadDs3AxisAsButton(Data, config.aPDown)) { DSPov = DSPov | Direction.Down; }
                            if (CheckDpadDs3AxisAsButton(Data, config.aPLeft)) { DSPov = DSPov | Direction.Left; }
                            if (CheckDpadDs3AxisAsButton(Data, config.aPRight)) { DSPov = DSPov | Direction.Right; }

                            vJPad.JoyPov(DSPov, dsID);
                            #endregion
                        }
                        break;
                    #endregion
                    #region DS4
                    case DsModel.DS4:
                        {
                            #region Axis
                            vJPad.JoyAxis(config.axisL2, Data.Axis(Ds4Axis.L2), dsID);
                            vJPad.JoyAxis(config.axisR2, Data.Axis(Ds4Axis.R2), dsID);

                            SetAxis(Data.Axis(Ds4Axis.LX), config.axisLX, config.invertLX, dsID);
                            SetAxis(Data.Axis(Ds4Axis.LY), config.axisLY, config.invertLY, dsID);

                            SetAxis(Data.Axis(Ds4Axis.RX), config.axisRX, config.invertRX, dsID);
                            SetAxis(Data.Axis(Ds4Axis.RY), config.axisRY, config.invertRY, dsID);
                            #endregion
                            #region Buttons
                            SetButton(Data.Button(Ds4Button.Cross), config.cross, dsID);
                            SetButton(Data.Button(Ds4Button.Circle), config.circle, dsID);
                            SetButton(Data.Button(Ds4Button.Square), config.square, dsID);
                            SetButton(Data.Button(Ds4Button.Triangle), config.triangle, dsID);
                            SetButton(Data.Button(Ds4Button.L1), config.l1, dsID);
                            SetButton(Data.Button(Ds4Button.R1), config.r1, dsID);
                            SetButton(Data.Button(Ds4Button.L2), config.l2, dsID);
                            SetButton(Data.Button(Ds4Button.R2), config.r2, dsID);
                            SetButton(Data.Button(Ds4Button.Share), config.select_share, dsID);
                            SetButton(Data.Button(Ds4Button.Options), config.start_options, dsID);
                            SetButton(Data.Button(Ds4Button.L3), config.l3, dsID);
                            SetButton(Data.Button(Ds4Button.R3), config.r3, dsID);
                            SetButton(Data.Button(Ds4Button.PS), config.ps, dsID);
                            //Dpad as button
                            SetButton(Data.Button(Ds4Button.Up), config.up, dsID);
                            SetButton(Data.Button(Ds4Button.Down), config.down, dsID);
                            SetButton(Data.Button(Ds4Button.Left), config.left, dsID);
                            SetButton(Data.Button(Ds4Button.Right), config.right, dsID);
                            //AxisAsButton
                            SetAxisAsButton(Data.Axis(Ds4Axis.LX), config.aLRight, config.aLLeft, dsID);
                            SetAxisAsButton(Data.Axis(Ds4Axis.LY), config.aLDown, config.aLUp, dsID);
                            SetAxisAsButton(Data.Axis(Ds4Axis.RX), config.aRRight, config.aRLeft, dsID);
                            SetAxisAsButton(Data.Axis(Ds4Axis.RY), config.aRDown, config.aRUp, dsID);
                            #endregion
                            #region Dpad
                            //Dpad
                            Direction DSPov = Direction.None;
                            if (CheckDpadDs4(Data, config.pUp)) { DSPov = DSPov | Direction.Up; }
                            if (CheckDpadDs4(Data, config.pDown)) { DSPov = DSPov | Direction.Down; }
                            if (CheckDpadDs4(Data, config.pLeft)) { DSPov = DSPov | Direction.Left; }
                            if (CheckDpadDs4(Data, config.pRight)) { DSPov = DSPov | Direction.Right; }
                            //Axis as Dpad
                            if (CheckDpadDs4AxisAsButton(Data, config.aPUp)) { DSPov = DSPov | Direction.Up; }
                            if (CheckDpadDs4AxisAsButton(Data, config.aPDown)) { DSPov = DSPov | Direction.Down; }
                            if (CheckDpadDs4AxisAsButton(Data, config.aPLeft)) { DSPov = DSPov | Direction.Left; }
                            if (CheckDpadDs4AxisAsButton(Data, config.aPRight)) { DSPov = DSPov | Direction.Right; }

                            vJPad.JoyPov(DSPov, dsID);
                            #endregion
                        }
                        break;
                    #endregion
                }
                vJPad.JoySubmit(dsID);
            }
        }

        private void SetAxis(byte parAxisValue, HID_USAGES parTargetHID, bool parInverted, uint parDsID)
        {
            if (parInverted)
            {
                vJPad.JoyAxis(parTargetHID, 255 - parAxisValue, parDsID);
            }
            else
            {
                vJPad.JoyAxis(parTargetHID, parAxisValue, parDsID);
            }
        }
        private void SetAxisAsButton(byte parAxisValue, uint parHighId, uint parLowId, uint parDsID)
        {
            if (parAxisValue > HighTrigger)
            {
                vJPad.JoyButton(parHighId, true, parDsID);
                vJPad.JoyButton(parLowId, false, parDsID);
            }
            else if (parAxisValue < LowTrigger)
            {
                vJPad.JoyButton(parLowId, true, parDsID);
                vJPad.JoyButton(parHighId, false, parDsID);
            }
            else
            {
                vJPad.JoyButton(parHighId, false, parDsID); vJPad.JoyButton(parLowId, false, parDsID);
            }
        }
        private void SetButton(bool parDown, uint parButtonID, uint parDsID)
        { //Also deals with ButtonAsAixis
            if (parButtonID < 1000)
            {
                vJPad.JoyButton(parButtonID, parDown, parDsID);
            }
            else if (parButtonID < 2000)
            {
                //LO
                if (parDown)
                {
                    vJPad.JoyAxis((HID_USAGES)(parButtonID - 1000), 0, parDsID);
                }
            }
            else
            {
                //HI
                if (parDown)
                {
                    vJPad.JoyAxis((HID_USAGES)(parButtonID - 2000), 255, parDsID);
                }
            }
        }

        private bool CheckDpadDs3(DsPacket Data, DSButton parButton)
        {
            if (parButton.DS3 != Ds3Button.None)
            {
                return Data.Button(parButton.DS3);
            }
            else
            {
                return false;
            }
        }
        private bool CheckDpadDs4(DsPacket Data, DSButton parButton)
        {
            if (parButton.DS4 != Ds4Button.None)
            {
                return Data.Button(parButton.DS4);
            }
            else
            {
                return false;
            }
        }
        private bool CheckDpadDs3AxisAsButton(DsPacket Data, DSAxis parAxis)
        {
            if (parAxis.DS3 != Ds3Axis.None)
            {
                return CheckDpadAxisAsButton(Data.Axis(parAxis.DS3), parAxis.triggerHigh);
            }
            else
            {
                return false;
            }
        }
        private bool CheckDpadDs4AxisAsButton(DsPacket Data, DSAxis parAxis)
        {
            if (parAxis.DS4 != Ds4Axis.None)
            {
                return CheckDpadAxisAsButton(Data.Axis(parAxis.DS4), parAxis.triggerHigh);
            }
            else
            {
                return false;
            }
        }

        private bool CheckDpadAxisAsButton(byte parAxisValue, bool parTriggerHigh)
        {
            if (parTriggerHigh)
            {
                if (parAxisValue > HighTrigger)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (parAxisValue < LowTrigger)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public ScpProxy Proxy
        {
            get { return m_VibProxy; }
            set { m_VibProxy = value;}
        }
        protected void Rumble(uint parDsID, EffectReturnValue e)
        {
            if (m_VibProxy != null)
            {
                Proxy.Rumble((DsPadId)(parDsID - 1), ScaleLargeMotor(e.MotorLeft), 0); //large moter + small moter (Byte)e.MotorRight
                //Trace.WriteLine("Dev(" + parDsID + "), Vibration Left : " + e.MotorLeft);
            }
        }
        protected Byte ScaleLargeMotor(float parLevel)
        {
            //Reflect negative values
            if (parLevel < 0)
            {
                parLevel = -parLevel;
            }

            //Incomming range = 0-255
            const float IN_MAX = 255;
            const float IN_MIN = 0;
            const float IN_RANGE = IN_MAX - IN_MIN;
            //outgoing range = 0-255
            const float OUT_MAX = 255;
            const float OUT_MIN = 0;
            const float OUT_RANGE = OUT_MAX - OUT_MIN;

            //Clamp High Values
            if (parLevel > IN_MAX)
            {
                parLevel = IN_MAX;
            }

            float verIn = parLevel / IN_RANGE;
            float ret = (OUT_MIN + verIn * OUT_RANGE);

            return (Byte)ret;
        }
    }
}
