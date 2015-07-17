using System;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using ScpControl;

using vJoyInterfaceWrap;

namespace ScpPad2vJoy 
{
    partial class ScpForm : Form 
    {
        protected DXPadState gps;

        protected VJoyPost vJP = new VJoyPost();
        protected X360_InputLocker dxLocker;
        protected String m_Active;
        protected PadSettings config;
        protected bool[] selectedPads = new bool[] {true,false,false,false};
        protected DeviceManagement devManLevel = DeviceManagement.vJoy_Config | DeviceManagement.vJoy_Device | DeviceManagement.Xinput_DX /*| DeviceManagement.Xinput_XI*/;

        public ScpForm() 
        {
            InitializeComponent();
        }

        protected void Form_Load(object sender, EventArgs e) 
        {
            Icon = Properties.Resources.Scp_All;
            //if DefaultConfig.txt exits in working directory, use that
            if (System.IO.File.Exists("Default_vjConfig.txt"))
            {
                string[] configfile = System.IO.File.ReadAllLines("Default_vjConfig.txt");
                config = new PadSettings(configfile);               
            }
            else
            {//No Default config, use our own
                config = new PadSettings(Properties.Resources.W3C.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
            }
            if (scpProxy.Load())
            {
                m_Active = scpProxy.Active;
            }

            //cbPad.SelectedIndex = (Int32) m_Pad;

            dxLocker = new X360_InputLocker(this);
        }

        protected void Form_Close(object sender, FormClosingEventArgs e) 
        {

            if (btnStop.Enabled)
            {
                btnStop_Click(sender, e);
            }
        }

        protected void Parse(object sender, DsPacket e) 
        {
            lock(this)
            {
                if (selectedPads[(int)e.Detail.Pad] && !(gps == null))
                {
                    gps.Update(e);
                }
            }
        }

        protected void onVib(object sender, EventArgs e)
        {
            scpProxy.Rumble(DsPadId.One, 100, 100);
        }

        protected void btnStart_Click(object sender, EventArgs e) 
        {
            if (scpProxy.Start())
            {
                cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = btnStart.Enabled = false;
                if (vJP.Start(selectedPads, config, devManLevel))
                {
                    gps = new DXPadState(vJP,config);
                    gps.Proxy = scpProxy;
                    if ((devManLevel & DeviceManagement.Xinput_XI) == DeviceManagement.Xinput_XI)
                    {
                        //SCP X360 Controller can only be disabled when
                        //the SCP service isn't running, so we have to
                        //restart the service (done in Lock_XI_Devices).
                        //However, ScpProxy dosn't like the service
                        //restart, so we need create a new instance.
                        scpProxy.Stop();
                        this.components.Remove(scpProxy);
                        dxLocker.Lock_XI_Devices();
                        scpProxy = new ScpProxy(this.components);
                        this.scpProxy.Packet += new System.EventHandler<ScpControl.DsPacket>(this.Parse);
                        if (!scpProxy.Start())
                        {
                            //error
                            MessageBox.Show(this, "Native Feed is not available", "ScpPad2vJoy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            vJP.Stop(selectedPads, devManLevel);
                            dxLocker.UnlockDevices();
                        }
                    }
                    if ((devManLevel & DeviceManagement.Xinput_DX) == DeviceManagement.Xinput_DX)
                    {
                        dxLocker.Lock_DX_Devices();
                    }

                    btnStop.Enabled = true;  
                }
                else
                {
                    scpProxy.Stop();
                    cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = btnStart.Enabled = true;
                    MessageBox.Show(this, "Vjoy pad is not available", "ScpPad2vJoy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show(this, "Native Feed is not available",  "ScpPad2vJoy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void btnStop_Click(object sender, EventArgs e) 
        {
            btnStop.Enabled = false;

            vJP.Stop(selectedPads, devManLevel);
            scpProxy.Stop();
            //Do this regardless as the input
            //locker only unlocks devices it has locked.
            dxLocker.UnlockDevices();

            cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = true;
            btnStart.Enabled = true;
        }

        private void cbP1_CheckedChanged(object sender, EventArgs e)
        {
            selectedPads[0] = cbP1.Checked;
        }

        private void cbP2_CheckedChanged(object sender, EventArgs e)
        {
            selectedPads[1] = cbP2.Checked;
        }

        private void cbP3_CheckedChanged(object sender, EventArgs e)
        {
            selectedPads[2] = cbP3.Checked;
        }

        private void cbP4_CheckedChanged(object sender, EventArgs e)
        {
            selectedPads[3] = cbP4.Checked;
        }

        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] configfile = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                config = new PadSettings(configfile);
            }
        }
    }
}
