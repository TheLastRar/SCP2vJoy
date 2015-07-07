using System;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using ScpControl;

using vJoyInterfaceWrap;

namespace ScpPad2vJoy 
{
    public partial class ScpForm : Form 
    {
        protected DXPadState gps;

        protected VJoyPost vJP = new VJoyPost();
        protected DXinputLocker dxLocker;
        protected String m_Active;
        protected PadSettings config;
        protected bool[] selectedPads = new bool[] {true,false,false,false};
        protected DeviceManagement devManLevel = DeviceManagement.vJoy_Config | DeviceManagement.vJoy_Device | DeviceManagement.Xinput_DX;

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

            dxLocker = new DXinputLocker(this);
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

        protected void btnStart_Click(object sender, EventArgs e) 
        {
            if (scpProxy.Start())
            {
                cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = btnStart.Enabled = false;
                if (vJP.Start(selectedPads, config, devManLevel))
                {
                    gps = new DXPadState(vJP,config);
                    if ((devManLevel & DeviceManagement.Xinput_DX) == DeviceManagement.Xinput_DX)
                    {
                        dxLocker.LockDevices();
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

            scpProxy.Stop();
            vJP.Stop(selectedPads, devManLevel);
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
