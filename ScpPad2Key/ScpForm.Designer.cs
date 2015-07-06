namespace ScpPad2vJoy
{
    partial class ScpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.scpProxy = new ScpControl.ScpProxy(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.gbX = new System.Windows.Forms.GroupBox();
            this.cbP4 = new System.Windows.Forms.CheckBox();
            this.cbP3 = new System.Windows.Forms.CheckBox();
            this.cbP2 = new System.Windows.Forms.CheckBox();
            this.cbP1 = new System.Windows.Forms.CheckBox();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.gbX.SuspendLayout();
            this.SuspendLayout();
            // 
            // scpProxy
            // 
            this.scpProxy.Packet += new System.EventHandler<ScpControl.DsPacket>(this.Parse);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(217, 76);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(136, 76);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gbX
            // 
            this.gbX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbX.Controls.Add(this.cbP4);
            this.gbX.Controls.Add(this.cbP3);
            this.gbX.Controls.Add(this.cbP2);
            this.gbX.Controls.Add(this.cbP1);
            this.gbX.Location = new System.Drawing.Point(12, 12);
            this.gbX.Name = "gbX";
            this.gbX.Size = new System.Drawing.Size(278, 49);
            this.gbX.TabIndex = 10;
            this.gbX.TabStop = false;
            this.gbX.Text = "Capture Pads";
            // 
            // cbP4
            // 
            this.cbP4.AutoSize = true;
            this.cbP4.Location = new System.Drawing.Point(207, 19);
            this.cbP4.Name = "cbP4";
            this.cbP4.Size = new System.Drawing.Size(61, 17);
            this.cbP4.TabIndex = 3;
            this.cbP4.Text = "Pad #4";
            this.cbP4.UseVisualStyleBackColor = true;
            this.cbP4.CheckedChanged += new System.EventHandler(this.cbP4_CheckedChanged);
            // 
            // cbP3
            // 
            this.cbP3.AutoSize = true;
            this.cbP3.Location = new System.Drawing.Point(140, 19);
            this.cbP3.Name = "cbP3";
            this.cbP3.Size = new System.Drawing.Size(61, 17);
            this.cbP3.TabIndex = 2;
            this.cbP3.Text = "Pad #3";
            this.cbP3.UseVisualStyleBackColor = true;
            this.cbP3.CheckedChanged += new System.EventHandler(this.cbP3_CheckedChanged);
            // 
            // cbP2
            // 
            this.cbP2.AutoSize = true;
            this.cbP2.Location = new System.Drawing.Point(73, 19);
            this.cbP2.Name = "cbP2";
            this.cbP2.Size = new System.Drawing.Size(61, 17);
            this.cbP2.TabIndex = 1;
            this.cbP2.Text = "Pad #2";
            this.cbP2.UseVisualStyleBackColor = true;
            this.cbP2.CheckedChanged += new System.EventHandler(this.cbP2_CheckedChanged);
            // 
            // cbP1
            // 
            this.cbP1.AutoSize = true;
            this.cbP1.Checked = true;
            this.cbP1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbP1.Location = new System.Drawing.Point(6, 19);
            this.cbP1.Name = "cbP1";
            this.cbP1.Size = new System.Drawing.Size(61, 17);
            this.cbP1.TabIndex = 0;
            this.cbP1.Text = "Pad #1";
            this.cbP1.UseVisualStyleBackColor = true;
            this.cbP1.CheckedChanged += new System.EventHandler(this.cbP1_CheckedChanged);
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadConfig.Location = new System.Drawing.Point(55, 76);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(75, 23);
            this.btnLoadConfig.TabIndex = 11;
            this.btnLoadConfig.Text = "Load Config";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "Config.txt";
            // 
            // ScpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 112);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.gbX);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 360);
            this.MinimumSize = new System.Drawing.Size(320, 38);
            this.Name = "ScpForm";
            this.Text = "Vjoy Mapper - v1.2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Close);
            this.Load += new System.EventHandler(this.Form_Load);
            this.gbX.ResumeLayout(false);
            this.gbX.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ScpControl.ScpProxy scpProxy;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox gbX;
        private System.Windows.Forms.CheckBox cbP4;
        private System.Windows.Forms.CheckBox cbP3;
        private System.Windows.Forms.CheckBox cbP2;
        private System.Windows.Forms.CheckBox cbP1;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

