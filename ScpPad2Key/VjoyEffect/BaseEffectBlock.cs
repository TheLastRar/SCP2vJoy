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
        private Byte maxLoopCount = 1;
        protected vJoy.FFB_EFF_REPORT m_ffbHeader;
        protected Single gain = 1.0F;
        public FFBEType m_effectType;

        protected long m_runTime = 0;
        private long m_runLoops = 0;
        private Stopwatch effectTimer = new Stopwatch();

        #region "Props"
        protected long runTime
        {
            get
            {
                return m_runTime;
            }
        }
        public vJoy.FFB_EFF_REPORT ffbHeader
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
                if (m_ffbHeader.TrigerBtn != vJoyConstants.EFFECT_NULL_TRIGGER_BTN)
                {
                    Trace.WriteLine("TRIGGER BUTTON NOT SUPPORTED");
                }
                gain = value.Gain / vJoyConstants.EFFECT_MAX_GAIN;
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
            maxLoopCount = parLoopCount;
            effectTimer.Restart();
            m_runTime = 0;
            m_runLoops = 0;
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
            if (m_ffbHeader.Duration != vJoyConstants.EFFECT_INF_DURATION)
            {
                return OverTick();
            }
            else
            {
                m_runTime = effectTimer.ElapsedMilliseconds;
                return true;
            }
        }

        //Check if we have compleated a loop
        //Set runtime to amount of time spent
        //in current loop
        private Boolean OverTick()
        {
            long currRunTime = effectTimer.ElapsedMilliseconds;
            while (currRunTime > ((long)m_ffbHeader.Duration * (m_runLoops+1)))
            {
                m_runLoops += 1;
                if (maxLoopCount != vJoyConstants.EFFECT_MAX_LOOP_COUNT && maxLoopCount - m_runLoops == 0)
                {
                    m_isPaused = true;
                    return false;
                }
            }
            m_runTime = currRunTime - ((long)m_ffbHeader.Duration * m_runLoops);
            return true;
        }
    }
}
