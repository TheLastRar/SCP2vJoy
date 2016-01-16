using ScpControl;
using ScpControl.Shared.Core;
using ScpPad2vJoy.vJ;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ScpPad2vJoy
{
    partial class ScpForm : Form
    {
        protected DXPadState gps;

        protected vJoyPost vJP = new vJoyPost();
        protected X360_InputLocker dxLocker;

        protected PadSettings config;
        protected bool[] selectedPads = new bool[] { true, false, false, false };
        protected DeviceManagement devManLevel = DeviceManagement.vJoy_Config | DeviceManagement.vJoy_Device | DeviceManagement.Xinput_DX /*| DeviceManagement.Xinput_XI*/;

        protected bool runParser = false;

        public ScpForm()
        {
            //try
            //{
                InitializeComponent();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("HELP");
            //}
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            Text += ver.Major + "." + ver.Minor;
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

            //cbPad.SelectedIndex = (Int32) m_Pad;

            dxLocker = new X360_InputLocker(this);

            if (!FormLoadScp())
            {
                MessageBox.Show(this, "Native Feed is not available", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }
        }

        protected void Form_Close(object sender, FormClosingEventArgs e)
        {
            if (btnStop.Enabled)
            {
                btnStop_Click(sender, e);
            }
            FormCloseScp();
        }

        protected void Parse(object sender, ScpHidReport e)
        {
            lock (this)
            {
                if (runParser & selectedPads[(int)e.PadId])
                {
                    gps.Update(e);
                }
            }
        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = btnStart.Enabled = cbVib.Enabled = false;
            config.ffb = cbVib.Checked;

            if (vJP.Start(selectedPads, config, devManLevel))
            {
                gps = new DXPadState(vJP, config);
                gps.Proxy = scpProxy;
                if ((devManLevel & DeviceManagement.Xinput_XI) == DeviceManagement.Xinput_XI)
                {
                    //TODO
                    //Is this even doable with the bugged ScpProxy.Stop()?

                    //    //SCP X360 Controller can only be disabled when
                    //    //the SCP service isn't running, so we have to
                    //    //restart the service (done in Lock_XI_Devices).
                    //    //However, ScpProxy dosn't like the service
                    //    //restart, so we need create a new instance.
                    //    dxLocker.Lock_XI_Devices();
                    //    //TODO Reinit ScpProxy (if needed)
                    //    //
                    //    gps.Proxy = scpProxy;
                }
                if ((devManLevel & DeviceManagement.Xinput_DX) == DeviceManagement.Xinput_DX)
                {
                    dxLocker.Lock_DX_Devices();
                }
                if (StandardScpStart())
                {
                    btnStop.Enabled = true;
                }
                else
                {
                    dxLocker.UnlockDevices();
                    vJP.Stop(selectedPads, devManLevel);

                    cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = btnStart.Enabled = cbVib.Enabled = true;
                    MessageBox.Show(this, "Native Feed is not available", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = btnStart.Enabled = cbVib.Enabled = true;
                MessageBox.Show(this, "Vjoy pad is not available", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            vJP.Stop(selectedPads, devManLevel);
            //Can't restart proxy after stop (now)
            StandardScpStop();
            //Do this regardless as the input
            //locker only unlocks devices it has locked.
            dxLocker.UnlockDevices();

            cbP1.Enabled = cbP2.Enabled = cbP3.Enabled = cbP4.Enabled = btnLoadConfig.Enabled = cbVib.Enabled = true;
            btnStart.Enabled = true;
        }

        #region ScpStartStop
        //ScpProxy.Stop() is buggy
        //So we start the proxy on form load
        //and have it run for the life of the program 
        protected bool FormLoadScp()
        {
            return scpProxy.Start();
        }
        protected bool StandardScpStart()
        {
            //scpProxy.Start()
            runParser = true;
            return true;
        }

        protected void StandardScpStop()
        {
            //scpProxy.Stop();
            //components.Remove(scpProxy);
            //scpProxy.Dispose();
            //scpProxy = new ScpProxy(components);
            //scpProxy.NativeFeedReceived += Parse;
            runParser = false;
        }
        protected void FormCloseScp()
        {
            scpProxy.Stop();
        }
        #endregion

        #region PadSelect
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
        #endregion

        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] configfile = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                config = new PadSettings(configfile);
            }
        }

        private void Help_Click(object sender, EventArgs e)
        {
            AboutForm abt = new AboutForm();
            abt.Show();
        }
    }
}
