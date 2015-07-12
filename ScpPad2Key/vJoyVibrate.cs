using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using vJoyInterfaceWrap;
using ScpPad2vJoy.VjoyEffect;
using System.Diagnostics;

namespace ScpPad2vJoy
{
    class vJoyVibrate
    {
        protected vJoy joystick;
        object vibThreadSentry = new object();
        protected FFBDevice[] devices = new FFBDevice[4];

        //Events
        public event VibrationEventHandler VibrationCommand;
        public delegate void VibrationEventHandler(uint id, EffectReturnValue e);

        public vJoyVibrate(vJoy parVJoy)
        {
            joystick = parVJoy;
            for (int x = 1; x <= 4; x++)
            {
                devices[x - 1] = new FFBDevice();
                devices[x - 1].ClearBlocks();
            }
        }

        private vJoy.FfbCbFunc OnEffectObj = null;
        //private EffectBlock effectBlockNew = null; //Block to be loaded
        private void func_OnEffectObj(IntPtr data, object userData)
        {

            //Trace.WriteLine("");
            const int ERROR_SUCCESS = 0;
            //Trace.WriteLine(String.Format("Got FFB Packet"));
            int id = -1;
            FFBPType type = (FFBPType)0;
            Int32 effectBlockIndex = -1;
            if (joystick.Ffb_h_DeviceID(data, ref id) != ERROR_SUCCESS)
            {
                Trace.WriteLine("Error: Unable to read DevID");
                return;
            }
            if (joystick.Ffb_h_Type(data, ref type) != ERROR_SUCCESS)
            {
                Trace.WriteLine("Error: Unable to read FFB type");
                return;
            }
            if (joystick.Ffb_h_EBI(data, ref effectBlockIndex) != ERROR_SUCCESS)
            { effectBlockIndex = -1; }

            FFBDevice srcDevice = devices[id - 1];

            //Trace.WriteLine(String.Format("EBI : {0}", effectBlockIndex));
            //Read Based on Type
            switch (type)
            {
                case FFBPType.PT_EFFREP:
                    vJoy.FFB_EFF_CONST effectParam = new vJoy.FFB_EFF_CONST();
                    if (joystick.Ffb_h_Eff_Const(data, ref effectParam) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Effect Param");
                        return;
                    }
                    Trace.WriteLine(String.Format("Got Effect Param on EBI : {0}", effectBlockIndex));
                    //Trace.WriteLine(String.Format("EffectType: {0}", effectParam.EffectType.ToString()));
                    Trace.WriteLine(String.Format("Duration  : {0}", effectParam.Duration));
                    Trace.WriteLine(String.Format("TrigerRpt : {0}", effectParam.TrigerRpt));
                    Trace.WriteLine(String.Format("SamplePrd : {0}", effectParam.SamplePrd));
                    Trace.WriteLine(String.Format("Gain      : {0}", effectParam.Gain));
                    Trace.WriteLine(String.Format("TrigerBtn : {0}", effectParam.TrigerBtn));
                    Trace.WriteLine(String.Format("Polar     : {0}", effectParam.Polar));
                    Trace.WriteLine(String.Format("Dir1      : {0}", effectParam.DirX));
                    Trace.WriteLine(String.Format("Dir2      : {0}", effectParam.DirY));
                    byte[] rawP = new byte[0];
                    int len = 0;
                    uint transfertype = 0;
                    joystick.Ffb_h_Packet(data, ref transfertype, ref len, ref rawP);
                    Trace.WriteLine(String.Format("TypeSpecificBlockOffset1 : {0}", BitConverter.ToUInt16(rawP, len - 4)));
                    Trace.WriteLine(String.Format("TypeSpecificBlockOffset2 : {0}", BitConverter.ToUInt16(rawP, len - 2)));
                    srcDevice.EffectBlocks[(Byte)effectBlockIndex].ffbHeader = effectParam;
                    break;

                case FFBPType.PT_ENVREP:
                    vJoy.FFB_EFF_ENVLP envEffect = new vJoy.FFB_EFF_ENVLP();
                    if (joystick.Ffb_h_Eff_Envlp(data, ref envEffect) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Envelope Effect");
                        return;
                    }
                    srcDevice.EffectBlocks[(Byte)effectBlockIndex].SecondaryEffectData(envEffect);
                    Trace.WriteLine(String.Format("Got Envelope Effect on EBI: {0}", effectBlockIndex));
                    Trace.WriteLine(String.Format("Start : {0}", envEffect.AttackLevel));
                    Trace.WriteLine(String.Format("STime : {0}", envEffect.AttackTime));
                    Trace.WriteLine(String.Format("End   : {0}", envEffect.FadeLevel));
                    Trace.WriteLine(String.Format("ETime : {0}", envEffect.FadeTime));
                    break;

                case FFBPType.PT_CONDREP:
                    vJoy.FFB_EFF_COND condEffect = new vJoy.FFB_EFF_COND();
                    if (joystick.Ffb_h_Eff_Cond(data, ref condEffect) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Conditional Effect");
                        return;
                    }
                    Trace.WriteLine(String.Format("Got Conditional Effect on EBI (Not Supported): {0}", effectBlockIndex));
                    break;

                case FFBPType.PT_PRIDREP:
                    vJoy.FFB_EFF_PERIOD perEffect = new vJoy.FFB_EFF_PERIOD();
                    if (joystick.Ffb_h_Eff_Period(data, ref perEffect) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Periodic Effect");
                        return;
                    }
                    Trace.WriteLine(String.Format("Got Periodic Effect on EBI (Not Supported): {0}", effectBlockIndex));
                    Trace.WriteLine(String.Format("Magnitude : {0}", perEffect.Magnitude));
                    Trace.WriteLine(String.Format("Offset    : {0}", (SByte)perEffect.Offset));
                    Trace.WriteLine(String.Format("Phase     : {0}", perEffect.Phase));
                    Trace.WriteLine(String.Format("Period    : {0}", perEffect.Period));
                    srcDevice.EffectBlocks[(Byte)effectBlockIndex].PrimaryEffectData(perEffect);
                    break;

                case FFBPType.PT_CONSTREP:
                    FFB_EFF_CONSTANT constEffect = new FFB_EFF_CONSTANT();
                    if (Ffb_h_Eff_Constant(data, ref constEffect) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Constant Effect");
                        return;
                    }
                    Trace.WriteLine(String.Format("Got Const Effect on EBI : {0}", effectBlockIndex));
                    Trace.WriteLine(String.Format("Magnitude : {0}", constEffect.Magnitude));
                    srcDevice.EffectBlocks[(Byte)effectBlockIndex].PrimaryEffectData(constEffect);
                    break;

                case FFBPType.PT_RAMPREP:
                    vJoy.FFB_EFF_RAMP rampEffect = new vJoy.FFB_EFF_RAMP();
                    if (joystick.Ffb_h_Eff_Ramp(data, ref rampEffect) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Ramp Effect");
                        return;
                    }
                    Trace.WriteLine(String.Format("Got Ramp Effect on EBI (Not Supported): {0}", effectBlockIndex));
                    Trace.WriteLine(String.Format("Start : {0}", rampEffect.Start));
                    Trace.WriteLine(String.Format("End   : {0}", rampEffect.End));
                    break;

                case FFBPType.PT_CSTMREP:
                    Trace.WriteLine("Error: Unkown command");
                    break;

                case FFBPType.PT_SMPLREP:
                    Trace.WriteLine("Error: Unkown command");
                    break;

                case FFBPType.PT_EFOPREP:
                    vJoy.FFB_EFF_OP effect_OP = new vJoy.FFB_EFF_OP();
                    if (joystick.Ffb_h_EffOp(data, ref effect_OP) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Effect OP");
                        return;
                    }
                    Trace.WriteLine(String.Format("Effect Command : {0}, Loops : {1}, EBI : {2} ", effect_OP.EffectOp.ToString(), effect_OP.LoopCount, effectBlockIndex));
                    switch (effect_OP.EffectOp)
                    {
                        case FFBOP.EFF_START:
                            lock (vibThreadSentry)
                            {
                                //Trace.WriteLine("Effect Start");
                                srcDevice.EffectBlocks[(byte)effectBlockIndex].Start(effect_OP.LoopCount);
                            }
                            break;
                        case FFBOP.EFF_STOP:
                            lock (vibThreadSentry)
                            {
                                //Trace.WriteLine("Effect Stop");
                                srcDevice.EffectBlocks[(byte)effectBlockIndex].Stop();
                            }
                            break;
                        default:
                            Trace.WriteLine("Error: Unkown command");
                            break;
                    }
                    break;
                    
                case FFBPType.PT_BLKFRREP:
                    Trace.WriteLine("Free Block");
                    lock (vibThreadSentry)
                    {
                        srcDevice.RemoveBlock((Byte)effectBlockIndex);
                    }
                    break;

                case FFBPType.PT_CTRLREP:
                    FFB_CTRL control = (FFB_CTRL)0;
                    if (joystick.Ffb_h_DevCtrl(data, ref control) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Dev Control");
                        return;
                    }
                    switch (control)
                    {
                        case FFB_CTRL.CTRL_ENACT:
                            Trace.WriteLine("Enable all actuators (Not Supported)");
                            break;
                        case FFB_CTRL.CTRL_DISACT:
                            Trace.WriteLine("Disable all actuators (Not Supported)");
                            break;
                        case FFB_CTRL.CTRL_STOPALL:
                            Trace.WriteLine("Stop All Effects");
                            //Set All To Zero + Pause
                            foreach (BaseEffectBlock peffect in srcDevice.EffectBlocks.Values)
                            {
                                peffect.Stop();
                            }
                            break;
                        case FFB_CTRL.CTRL_DEVRST:
                            Trace.WriteLine("Reset Device");
                            lock (vibThreadSentry)
                            {
                                srcDevice.ClearBlocks();
                                srcDevice.DevicePaused = false;
                            }
                            break;
                        case FFB_CTRL.CTRL_DEVPAUSE:
                            Trace.WriteLine("Pause all effects");
                            foreach (BaseEffectBlock peffect in srcDevice.EffectBlocks.Values)
                            {
                                peffect.DevPause();
                            }
                            srcDevice.DevicePaused = true;
                            break;
                        case FFB_CTRL.CTRL_DEVCONT:
                            Trace.WriteLine("Resume all effects paused by DevPause");
                            foreach (BaseEffectBlock peffect in srcDevice.EffectBlocks.Values)
                            {
                                peffect.DevPause();
                            }
                            srcDevice.DevicePaused = false;
                            break;
                        default:
                            Trace.WriteLine("Unkown Command");
                            break;
                    }
                    break;

                case FFBPType.PT_GAINREP:
                    byte gain = 1;
                    if (joystick.Ffb_h_DevGain(data, ref gain) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read Dev Gain Command");
                        return;
                    }
                    Trace.WriteLine((String.Format("Gain Set : {0}", gain)));
                    srcDevice.DeviceGain = (Single)gain / 255.0F;
                    break;

                case FFBPType.PT_SETCREP:
                    Trace.WriteLine("Error: Unkown command");
                    break;

                //Feature
                case FFBPType.PT_NEWEFREP:
                    FFBEType nextEffect = (FFBEType)0;
                    if (joystick.Ffb_h_EffNew(data, ref nextEffect) != ERROR_SUCCESS)
                    {
                        Trace.WriteLine("Error: Unable read new effect");
                        return;
                    }
                    Trace.WriteLine((String.Format("Incomming Effect : {0}", nextEffect.ToString())));
                    //Add + Load Block
                    byte free_ebi = srcDevice.NextKey();
                    if (free_ebi != 0)
                    {
                        BaseEffectBlock newBlock;
                        switch (nextEffect)
                        {
                                //ET_NONE
                            case FFBEType.ET_CONST:
                                newBlock = new ConstEffectBlock();
                                break;
                                //ET_RAMP
                            case FFBEType.ET_SQR:
                                newBlock = new SquareEffectBlock();
                                break;
                            case FFBEType.ET_SINE:
                                newBlock = new SineEffectBlock();
                                break;
                            case FFBEType.ET_TRNGL:
                                newBlock = new TrangleEffectBlock();
                                break;
                            case FFBEType.ET_STUP:
                                newBlock = new SawUpEffectBlock();
                                break;
                            case FFBEType.ET_STDN:
                                newBlock = new SawDownEffectBlock();
                                break;
                                //Conditinal effects
                                //ET_CSTM
                            default:
                                Trace.WriteLine("Unkown Effect");
                                newBlock = null;//new BaseEffectBlock();
                                break;
                        }
                        if (newBlock != null)
                        {
                            newBlock.m_effectType = nextEffect;
                            //TODO, When Blockload support arrives
                            //Do AddBlock/ebi selection @ Blockload
                            lock (vibThreadSentry)
                            {
                                srcDevice.AddBlock(free_ebi, newBlock);
                            }
                        }
                    }
                    break;

                case FFBPType.PT_BLKLDREP:
                    Trace.WriteLine("BULK LOAD (Not Supported)");
                    break;

                case FFBPType.PT_POOLREP:
                    Trace.WriteLine("Error: Unkown command");
                    break;
                default:
                    Trace.WriteLine("Error: Unkown command");
                    break;
            }
        }

        //Start/Stop FFB
        int RefCounter = 0;
        public void FfbInterface(uint id)
        {
            // Start FFB Mechanism
            if (!joystick.FfbStart(id))
                throw new Exception(String.Format("Cannot start Forcefeedback on device {0}", id));
            // Register the callback function & pass the dialog box object
            OnEffectObj = new vJoy.FfbCbFunc(func_OnEffectObj);
            joystick.FfbRegisterGenCB(OnEffectObj, joystick);
            if (RefCounter == 0)
            {
                HaltVibThread = false;
                System.Threading.Thread eTh = new System.Threading.Thread(EffectThread);
                eTh.Start();
            }
            RefCounter += 1;
            devices[id - 1].DeviceActive = true;
        }
        internal void FfbStop(uint id)
        {
            joystick.FfbStop(id);
            RefCounter -= 1;
            if (RefCounter==0)
                HaltVibThread = true;
            devices[id - 1].timer.Stop();
            devices[id - 1].DeviceActive = false;
        }
        volatile private Boolean HaltVibThread = false;
        private void EffectThread()
        {
            while (HaltVibThread == false)
            {
                for (int x = 0; x < 4; x++)
                {
                    FFBDevice srcDevice = devices[x];
                    if (!srcDevice.DeviceActive)
                    {
                        //Don't send VibCommands to from
                        //inactive devices
                        continue;
                    }
                    EffectReturnValue VibValues = new EffectReturnValue(0, 0);
                    if (!srcDevice.DevicePaused)
                    {
                        List<BaseEffectBlock> EffectsCopy = new List<BaseEffectBlock>();
                        lock (vibThreadSentry)
                        {
                            foreach (BaseEffectBlock eff in srcDevice.EffectBlocks.Values)
                            {
                                EffectsCopy.Add(eff);
                            }
                        }
                        //Appy Gain before or after summation?
                        foreach (BaseEffectBlock eff in EffectsCopy)
                        {
                            if (eff.isPaused)
                            {
                                continue;
                            }
                            VibValues += eff.Effect();
                        }
                    }

                    VibValues.MotorLeft = (VibValues.MotorLeft * srcDevice.DeviceGain);
                    VibValues.MotorRight = (VibValues.MotorRight * srcDevice.DeviceGain);
                    if (VibrationCommand != null)
                        VibrationCommand((uint)(x + 1), VibValues);
                }
                System.Threading.Thread.Sleep(5);
            }
        }

        public UInt32 Ffb_h_Eff_Constant(IntPtr data, ref FFB_EFF_CONSTANT Effect)
        {
            UInt32 command = 0;
            Int32 len = 0;
            byte[] rawData = new byte[0];
            UInt32 ret = joystick.Ffb_h_Packet(data, ref command, ref len, ref rawData);
            int index = 0;
            ret = ret | joystick.Ffb_h_EBI(data, ref index);
            Effect.EffectBlockIndex = (byte)index;
            Effect.Magnitude = rawData[2];
            return ret;
        }
    }
}
