using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vJoyInterfaceWrap;
using System.Diagnostics;

namespace ScpPad2vJoy.VjoyEffect
{
    abstract class BaseEffectBlock
    { 
        private Boolean m_isPaused = true;
        private Byte loopCount = 1;
        protected vJoy.FFB_EFF_CONST m_ffbHeader;
        protected Single gain = 1.0F;
        public FFBEType m_effectType;

        protected long m_runTime = 0;
        private Stopwatch effectTimer = new Stopwatch();

        #region "Props"
        protected long runTime
        {
            get
            {
                return m_runTime;
            }
        }
        public vJoy.FFB_EFF_CONST ffbHeader
        {
            get { return m_ffbHeader; }
            set
            {
                m_ffbHeader = value;
                if (m_ffbHeader.Polar == true)
                {
                    if (m_ffbHeader.DirY > 0)
                        Trace.WriteLine("Direction Not Supported");
                }
                else
                {
                    if (m_ffbHeader.DirY != 0 | m_ffbHeader.DirX != 0)
                        Trace.WriteLine("Direction Not Supported");
                }
                if (m_ffbHeader.EffectType != m_effectType)
                {
                    Trace.WriteLine("WARN: Effect Type Missmatch");
                    Trace.WriteLine("WARN: Effect Change not applied");
                }
                if (m_ffbHeader.TrigerBtn != 255)
                {
                    Trace.WriteLine("TRIGGER BUTTON NOT SUPPORTED");
                }
                gain = value.Gain / 255.0F;
            }
        }
        public Boolean isPaused
        {
            get
            {
                return m_isPaused;
            }
        }
        #endregion

        public void Start(Byte parLoopCount)
        {
            loopCount = (Byte)(parLoopCount - 1);
            effectTimer.Restart();
            m_runTime = 0;
            m_isPaused = false;
        }
        public void Stop()
        {
            effectTimer.Stop();
            m_isPaused = true;
        }

        //Keep time in check during
        //Device pauses
        public void DevPause()
        {
            effectTimer.Stop();
        }
        public void DevResume()
        {
            effectTimer.Start();
        }

        public abstract void PrimaryEffectData(object eff);
        public abstract void SecondaryEffectData(object eff);

        public EffectReturnValue Effect()
        {
            if (!Tick())
            {
                return new EffectReturnValue(0, 0);
            }
            //Handle Direction (?)
            float effValue = ComputeEffect();
            return new EffectReturnValue(effValue, 0);
        }
        protected abstract float ComputeEffect();

        private Boolean Tick()
        {
            m_runTime += effectTimer.ElapsedMilliseconds;
            effectTimer.Restart();
            if (m_ffbHeader.Duration != 0xFFFF)
            {
                return OverTick();
            }
            return true;
        }

        private Boolean OverTick() //Check if we run though multiple iterations in a timestep
        {
            while (runTime > m_ffbHeader.Duration)
            {
                if (loopCount == 0)
                {
                    m_isPaused = true;
                    return false;
                }
                m_runTime = m_runTime - m_ffbHeader.Duration;
                if (loopCount != 254)
                {
                    loopCount -= 1;
                }
            }
            return true;
        }
    }
}
