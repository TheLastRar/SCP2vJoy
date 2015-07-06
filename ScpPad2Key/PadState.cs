﻿using System;

using ScpControl;

using vJoyInterfaceWrap;

namespace ScpPad2vJoy
{
    public class DXPadState
    {
        protected ScpProxy m_VibProxy = null;

        protected volatile VJoyPost vJPad;
        protected volatile PadSettings config;
        public DXPadState(VJoyPost parVJoyPad, PadSettings parConfig)
        {
            vJPad = parVJoyPad;
            config = parConfig;
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
                            vJPad.JoyButton(config.cross, Data.Button(Ds3Button.Cross), dsID);
                            vJPad.JoyButton(config.circle, Data.Button(Ds3Button.Circle), dsID);
                            vJPad.JoyButton(config.square, Data.Button(Ds3Button.Square), dsID);
                            vJPad.JoyButton(config.triangle, Data.Button(Ds3Button.Triangle), dsID);
                            vJPad.JoyButton(config.l1, Data.Button(Ds3Button.L1), dsID);
                            vJPad.JoyButton(config.r1, Data.Button(Ds3Button.R1), dsID);
                            vJPad.JoyButton(config.l2, Data.Button(Ds3Button.L2), dsID);
                            vJPad.JoyButton(config.r2, Data.Button(Ds3Button.R2), dsID);
                            vJPad.JoyButton(config.select_share, Data.Button(Ds3Button.Select), dsID);
                            vJPad.JoyButton(config.start_options, Data.Button(Ds3Button.Start), dsID);
                            vJPad.JoyButton(config.l3, Data.Button(Ds3Button.L3), dsID);
                            vJPad.JoyButton(config.r3, Data.Button(Ds3Button.R3), dsID);
                            vJPad.JoyButton(config.ps, Data.Button(Ds3Button.PS), dsID);
                            //Dpad as button
                            vJPad.JoyButton(config.up, Data.Button(Ds3Button.Up), dsID);
                            vJPad.JoyButton(config.down, Data.Button(Ds3Button.Down), dsID);
                            vJPad.JoyButton(config.left, Data.Button(Ds3Button.Left), dsID);
                            vJPad.JoyButton(config.right, Data.Button(Ds3Button.Right), dsID);
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
                            vJPad.JoyButton(config.cross, Data.Button(Ds4Button.Cross), dsID);
                            vJPad.JoyButton(config.circle, Data.Button(Ds4Button.Circle), dsID);
                            vJPad.JoyButton(config.square, Data.Button(Ds4Button.Square), dsID);
                            vJPad.JoyButton(config.triangle, Data.Button(Ds4Button.Triangle), dsID);
                            vJPad.JoyButton(config.l1, Data.Button(Ds4Button.L1), dsID);
                            vJPad.JoyButton(config.r1, Data.Button(Ds4Button.R1), dsID);
                            vJPad.JoyButton(config.l2, Data.Button(Ds4Button.L2), dsID);
                            vJPad.JoyButton(config.r2, Data.Button(Ds4Button.R2), dsID);
                            vJPad.JoyButton(config.select_share, Data.Button(Ds4Button.Share), dsID);
                            vJPad.JoyButton(config.start_options, Data.Button(Ds4Button.Options), dsID);
                            vJPad.JoyButton(config.l3, Data.Button(Ds4Button.L3), dsID);
                            vJPad.JoyButton(config.r3, Data.Button(Ds4Button.R3), dsID);
                            vJPad.JoyButton(config.ps, Data.Button(Ds4Button.PS), dsID);
                            //Dpad as button
                            vJPad.JoyButton(config.up, Data.Button(Ds4Button.Up), dsID);
                            vJPad.JoyButton(config.down, Data.Button(Ds4Button.Down), dsID);
                            vJPad.JoyButton(config.left, Data.Button(Ds4Button.Left), dsID);
                            vJPad.JoyButton(config.right, Data.Button(Ds4Button.Right), dsID);
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
            }
        }

        private void SetAxis(byte parAxisValue, HID_USAGES parTargetHID,bool parInverted, uint parDsID)
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

        private bool CheckDpadAxisAsButton(byte parAxisValue,bool parTriggerHigh)
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
        //protected void Rumble(Byte Large, Byte Small)
        //{
        //    if (Proxy != null)
        //    {
        //        Proxy.Rumble(Pad, Large, Small); //large moter + small moter
        //    }
        //}
    }
}
