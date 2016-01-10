using System;
using System.Diagnostics;
using vJoyInterfaceWrap;

namespace ScpPad2vJoy.vJ.FFB.Effect
{
    abstract class BaseEffectBlock
    {
        private bool m_isPaused = true;
        private byte maxLoopCount = 1;
        protected vJoy.FFB_EFF_REPORT m_ffbHeader;
        protected float gain = 1.0F;
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
                        Trace.WriteLine("Polar Direction with value for Y Not Supported");
                }
                else
                {
                    if (m_ffbHeader.DirY != 0 | m_ffbHeader.DirX != 0)
                        Trace.WriteLine("Cartesian Direction Not Supported");
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
                if (m_ffbHeader.SamplePrd > 1)
                {
                    Trace.WriteLine("Custom Sample Rate Not Supported");
                }
                gain = value.Gain / vJoyConstants.EFFECT_MAX_GAIN;
            }
        }
        public bool isPaused
        {
            get
            {
                return m_isPaused;
            }
        }
        #endregion

        public void Start(byte parLoopCount)
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
            if (!m_isPaused)
            {
                effectTimer.Start();
            }
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
            //Found someting that supports direction

            float DirXMult = 0;
            float DirYMult = 1;

            if (m_ffbHeader.Polar)
            {
                double angleRad = ((double)m_ffbHeader.Direction / vJoyConstants.EFFECT_DIRECTION_MAX_VALUE) * (2 * Math.PI);
                //Up(X) = 0/255
                //Right(Y) = ~63

                double h = 1.0;

                DirXMult = (float)(h * Math.Cos(angleRad));
                DirYMult = (float)(h * Math.Sin(angleRad));
            }
            //Trace.WriteLine("Writing Direction");
            //Trace.WriteLine(m_ffbHeader.Polar);
            //Trace.WriteLine(m_ffbHeader.DirX); //Also Direction
            //Trace.WriteLine(m_ffbHeader.DirY);

            //Trace.WriteLine(DirXMult);
            //Trace.WriteLine(DirYMult);

            return new EffectReturnValue(effValue * DirXMult, effValue * DirYMult);
        }
        protected abstract float ComputeEffect();

        private bool Tick()
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
        private bool OverTick()
        {
            long currRunTime = effectTimer.ElapsedMilliseconds;
            while (currRunTime > ((long)m_ffbHeader.Duration * (m_runLoops + 1)))
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
