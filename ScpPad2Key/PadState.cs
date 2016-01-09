using ScpControl;
using ScpPad2vJoy.vJ;
using ScpControl.Profiler;
using ScpPad2vJoy.vJ.FFB.Effect;
using System;

namespace ScpPad2vJoy
{
    class DXPadState
    {
        protected ScpProxy m_VibProxy = null;

        protected volatile vJoyPost vJPad;
        protected volatile PadSettings config;
        public DXPadState(vJoyPost parVJoyPad, PadSettings parConfig)
        {
            vJPad = parVJoyPad;
            config = parConfig;
            vJPad.VibrationCommand += Rumble;
        }

        //AxisAsButton
        private const byte HighTrigger = 193;
        private const byte LowTrigger = 32;

        public void Update(ScpHidReport Data)
        {
            lock (this)
            {
                uint dsID = (uint)Data.PadId + 1;
                switch (Data.Model)
                {
                    #region DS3
                    case ScpControl.ScpCore.DsModel.DS3:
                        {
                            #region Gryo
                            //Data.Orientation
                            //Data.Motion
                            #endregion
                            #region Axis
                            vJPad.JoyAxis(config.axisL2, Data[Ds3Axis.L2].Value, dsID);
                            vJPad.JoyAxis(config.axisR2, Data[Ds3Axis.R2].Value, dsID);
                            SetAxis(Data[Ds3Axis.Lx].Value, config.axisLX, config.invertLX, dsID);
                            SetAxis(Data[Ds3Axis.Ly].Value, config.axisLY, config.invertLY, dsID);

                            SetAxis(Data[Ds3Axis.Rx].Value, config.axisRX, config.invertRX, dsID);
                            SetAxis(Data[Ds3Axis.Ry].Value, config.axisRY, config.invertRY, dsID);
                            #endregion
                            #region Buttons
                            SetButton(Data[Ds3Button.Cross].IsPressed, config.cross, dsID);
                            SetButton(Data[Ds3Button.Circle].IsPressed, config.circle, dsID);
                            SetButton(Data[Ds3Button.Square].IsPressed, config.square, dsID);
                            SetButton(Data[Ds3Button.Triangle].IsPressed, config.triangle, dsID);
                            SetButton(Data[Ds3Button.L1].IsPressed, config.l1, dsID);
                            SetButton(Data[Ds3Button.R1].IsPressed, config.r1, dsID);
                            SetButton(Data[Ds3Button.L2].IsPressed, config.l2, dsID);
                            SetButton(Data[Ds3Button.R2].IsPressed, config.r2, dsID);
                            SetButton(Data[Ds3Button.Select].IsPressed, config.select_share, dsID);
                            SetButton(Data[Ds3Button.Start].IsPressed, config.start_options, dsID);
                            SetButton(Data[Ds3Button.L3].IsPressed, config.l3, dsID);
                            SetButton(Data[Ds3Button.R3].IsPressed, config.r3, dsID);
                            SetButton(Data[Ds3Button.Ps].IsPressed, config.ps, dsID);
                            //Dpad as button
                            SetButton(Data[Ds3Button.Up].IsPressed, config.up, dsID);
                            SetButton(Data[Ds3Button.Down].IsPressed, config.down, dsID);
                            SetButton(Data[Ds3Button.Left].IsPressed, config.left, dsID);
                            SetButton(Data[Ds3Button.Right].IsPressed, config.right, dsID);
                            //AxisAsButton
                            SetAxisAsButton(Data[Ds3Axis.Lx].Value, config.aLRight, config.aLLeft, dsID);
                            SetAxisAsButton(Data[Ds3Axis.Ly].Value, config.aLDown, config.aLUp, dsID);
                            SetAxisAsButton(Data[Ds3Axis.Rx].Value, config.aRRight, config.aRLeft, dsID);
                            SetAxisAsButton(Data[Ds3Axis.Ry].Value, config.aRDown, config.aRUp, dsID);
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
                    case ScpControl.ScpCore.DsModel.DS4:
                        {
                            #region ThouchPad
                            //These are Int values
                            //What is the range?
                            //Also these have ID values, which mean what?
                            DsTrackPadTouch tp0 = Data.TrackPadTouch0;
                            DsTrackPadTouch tp1 = Data.TrackPadTouch1;
                            #endregion
                            #region Gryo
                            //Data.Orientation
                            //Data.Motion
                            #endregion
                            #region Axis
                            vJPad.JoyAxis(config.axisL2, Data[Ds4Axis.L2].Value, dsID);
                            vJPad.JoyAxis(config.axisR2, Data[Ds4Axis.R2].Value, dsID);
                            SetAxis(Data[Ds4Axis.Lx].Value, config.axisLX, config.invertLX, dsID);
                            SetAxis(Data[Ds4Axis.Ly].Value, config.axisLY, config.invertLY, dsID);

                            SetAxis(Data[Ds4Axis.Rx].Value, config.axisRX, config.invertRX, dsID);
                            SetAxis(Data[Ds4Axis.Ry].Value, config.axisRY, config.invertRY, dsID);
                            #endregion
                            #region Buttons
                            SetButton(Data[Ds4Button.Cross].IsPressed, config.cross, dsID);
                            SetButton(Data[Ds4Button.Circle].IsPressed, config.circle, dsID);
                            SetButton(Data[Ds4Button.Square].IsPressed, config.square, dsID);
                            SetButton(Data[Ds4Button.Triangle].IsPressed, config.triangle, dsID);
                            SetButton(Data[Ds4Button.L1].IsPressed, config.l1, dsID);
                            SetButton(Data[Ds4Button.R1].IsPressed, config.r1, dsID);
                            SetButton(Data[Ds4Button.L2].IsPressed, config.l2, dsID);
                            SetButton(Data[Ds4Button.R2].IsPressed, config.r2, dsID);
                            SetButton(Data[Ds4Button.Share].IsPressed, config.select_share, dsID);
                            SetButton(Data[Ds4Button.Options].IsPressed, config.start_options, dsID);
                            SetButton(Data[Ds4Button.L3].IsPressed, config.l3, dsID);
                            SetButton(Data[Ds4Button.R3].IsPressed, config.r3, dsID);
                            SetButton(Data[Ds4Button.Ps].IsPressed, config.ps, dsID);
                            //Dpad as button
                            SetButton(Data[Ds4Button.Up].IsPressed, config.up, dsID);
                            SetButton(Data[Ds4Button.Down].IsPressed, config.down, dsID);
                            SetButton(Data[Ds4Button.Left].IsPressed, config.left, dsID);
                            SetButton(Data[Ds4Button.Right].IsPressed, config.right, dsID);
                            //AxisAsButton
                            SetAxisAsButton(Data[Ds4Axis.Lx].Value, config.aLRight, config.aLLeft, dsID);
                            SetAxisAsButton(Data[Ds4Axis.Ly].Value, config.aLDown, config.aLUp, dsID);
                            SetAxisAsButton(Data[Ds4Axis.Rx].Value, config.aRRight, config.aRLeft, dsID);
                            SetAxisAsButton(Data[Ds4Axis.Ry].Value, config.aRDown, config.aRUp, dsID);
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
                //List<DeadZone> DeadZones = new List<DeadZone>();
                //RadialDeadZone adz = new RadialDeadZone();
                //adz.AxisX = HID_USAGES.HID_USAGE_X;
                //adz.AxisTypeX = AxisType.Stick;
                //adz.AxisY = HID_USAGES.HID_USAGE_Y;
                //adz.AxisTypeY = AxisType.Stick;
                //adz.DeadZone = 0.1;

                //DeadZones.Add(adz);
                //vJPad.ApplyDeadzone(DeadZones, dsID);
                vJPad.JoySubmit(dsID);
            }
        }

        private void SetAxis(byte parAxisValue, HID_USAGES parTargetHID, bool parInverted, uint parDsID)
        {
            if (parInverted)
            {
                vJPad.JoyAxis(parTargetHID, SCPConstants.MAX_SCP_AXIS - parAxisValue, parDsID);
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
                    vJPad.JoyAxis((HID_USAGES)(parButtonID - 2000), SCPConstants.MAX_SCP_AXIS, parDsID);
                }
            }
        }

        private bool CheckDpadDs3(ScpHidReport Data, DSButton parButton)
        {
            if (parButton.DS3 != Ds3Button.None)
            {
                return Data[parButton.DS3].IsPressed;
            }
            else
            {
                return false;
            }
        }
        private bool CheckDpadDs4(ScpHidReport Data, DSButton parButton)
        {
            if (parButton.DS4 != Ds4Button.None)
            {
                return Data[parButton.DS4].IsPressed;
            }
            else
            {
                return false;
            }
        }
        private bool CheckDpadDs3AxisAsButton(ScpHidReport Data, DSAxis parAxis)
        {
            if (parAxis.DS3 != Ds3Axis.None)
            {
                return CheckDpadAxisAsButton(Data[parAxis.DS3].Value, parAxis.triggerHigh);
            }
            else
            {
                return false;
            }
        }
        private bool CheckDpadDs4AxisAsButton(ScpHidReport Data, DSAxis parAxis)
        {
            if (parAxis.DS4 != Ds4Axis.None)
            {
                return CheckDpadAxisAsButton(Data[parAxis.DS4].Value, parAxis.triggerHigh);
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
            set { m_VibProxy = value; }
        }
        protected void Rumble(uint parDsID, EffectReturnValue e)
        {
            if (m_VibProxy != null)
            {
                Proxy.Rumble((ScpControl.ScpCore.DsPadId)(parDsID - 1), ScaleLargeMotor(e.MotorLeft), ScaleSmallMotor(e.MotorRight)); //large moter + small moter
                //Trace.WriteLine("Dev(" + parDsID + "), Vibration Left : " + ScaleLargeMotor(e.MotorLeft));
            }
        }
        protected byte ScaleLargeMotor(float parLevel)
        {
            //Reflect negative values
            if (parLevel < 0)
            {
                parLevel = -parLevel;
            }

            const float OUT_MAX = SCPConstants.EFFECT_MAX_VALUE;
            const float OUT_MIN = 50;
            const float OUT_RANGE = OUT_MAX - OUT_MIN;

            //Clamp High Values
            if (parLevel > vJoyConstants.EFFECT_MAX_VALUE)
            {
                parLevel = vJoyConstants.EFFECT_MAX_VALUE;
            }

            float ret;
            if (parLevel < 1.0)
            {
                //Give near zero values a truly zero
                //vibration
                //instead of a value of 50
                ret = 0;
            } 
            else
            {
                float verIn = parLevel / vJoyConstants.EFFECT_MAX_VALUE;
                ret = (OUT_MIN + verIn * OUT_RANGE);
            }


            return (Byte)ret;
        }
        protected byte ScaleSmallMotor(float parLevel)
        {
            //return ScaleLargeMotor(parLevel);
            //Reflect negative values
            if (parLevel < 0)
            {
                parLevel = -parLevel;
            }

            const float OUT_MAX = SCPConstants.EFFECT_MAX_VALUE;
            const float OUT_MIN = 0;
            const float OUT_RANGE = OUT_MAX - OUT_MIN;

            //Clamp High Values
            if (parLevel > vJoyConstants.EFFECT_MAX_VALUE)
            {
                parLevel = vJoyConstants.EFFECT_MAX_VALUE;
            }
            //Small moter is V sensitive
            //Remove lower values to compensate
            if (parLevel < 2000)
            {
                parLevel = 0;
            }

            float verIn = parLevel / vJoyConstants.EFFECT_MAX_VALUE;
            float ret = (OUT_MIN + verIn * OUT_RANGE);

            return (Byte)ret;
        }
    }
}
