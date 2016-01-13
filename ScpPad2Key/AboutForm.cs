using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ScpPad2vJoy
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            //this.MouseWheel += new MouseEventHandler(Panel_MouseWheel);
            DoAboutPage();
            DoContConfigPage();
            DoDSParamPage();
            DovJoyParamPage();
            DoExtraOpPage();
            //Resize
            int neededTabWidth = tableEdgePadding * 2 + parmWidth + descWidth + typeWidth + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            int neededtabControlWidth = neededTabWidth + 8;
            tabControl1.Width = neededtabControlWidth;
            int neededClientWidth = neededtabControlWidth + 13 * 2;
            this.ClientSize = new Size(neededClientWidth, this.ClientSize.Height);

            Icon = Properties.Resources.Scp_All;
        }

        public void DoAboutPage()
        {
            AssemblyName aName = Assembly.GetExecutingAssembly().GetName();
            string aboutName = aName.Name;
            string aboutVersion = aName.Version.ToString();
            string aboutDesc1 = "This program allows you to map your DualShock3/4 controller (using SCP Drivers) to a";
            string aboutDesc2 = "Dinput device via vJoy.";
            //string aboutMinvJoy = "The minimum required version of vJoy is: " + SharedConstants.VJOY_MIN_VERSION_STR;
            string aboutCredit = "Developed by Ge-Force, based of the GTA Mapper from Scarlet.Crush.";
            string aboutSupport = "Support provided on the PCSX2 Fourms: " + aboutName + ", SCP Driver.";
            string aboutSupportvJoyMapper = "http://forums.pcsx2.net/Thread-Dinput-Wrapper-for-SCP-Driver";
            string aboutSupportSCP = "http://forums.pcsx2.net/Thread-XInput-Wrapper-for-DS3-and-Play-com-USB-Dual-DS2-Controller";
            //I used these to help me UI
            labelSpacer1.Text = "";
            labelSpacer2.Text = "";
            labelSpacer3.Text = "";

            labelAboutName.Text = aboutName;
            labelVersion.Text = aboutVersion;
            labelDesc1.Text = aboutDesc1;
            labelDesc2.Text = aboutDesc2;
            //labelMinvJoy.Text = aboutMinvJoy;
            labelCredit.Text = aboutCredit;
            linkSupport.Text = aboutSupport;
            linkSupport.Links.Add(38, aboutName.Length, aboutSupportvJoyMapper);
            linkSupport.Links.Add(38 + aboutName.Length + 2, 10, aboutSupportSCP);
        }
        public void DoContConfigPage()
        {
            //TODO
            string line1 = "This program features a default mapping that should prove adaquate for most games.";
            //Space
            string line2 = "If you want a custom mapping, you can load custom mappings via \"Load Config\"";
            string line3 = "or you can have a custom mapping be automatically loaded by naming a file";
            string line4 = "\"Default_vjConfig.txt\" and placing it in the same directory as this program.";
            //Space
            string line5 = "A mapping profile is a simple text file that can be created and modified in a text editor";
            string line6 = "of your choice.";

            string line7 = "A button or axis is mapped like so: [vJoy input]=[DualShock input], i.e. to map the";
            string line8 = "circle button to button 2, you would do \"B2=CIRCLE\", you can also assign the square";
            string line9 = "button to Dpad left by doing \"PL=SQUARE\".";

            string line10 = "Mappings can only be made bettween inputs of the same type, buttons can only be";
            string line11 = "mapped to buttons and axes can only be mapped to axes. If you do want to map an";
            string line12 = "axis to a button or vise versa, there are additional options that allow that i.e. \"LS_UP\"";
            string line13 = "(Left stick up) can be mapped to a button like so .\"B3=LS_UP\"";
            label1.Text = line1;
            label2.Text = "";
            label3.Text = line2;
            label4.Text = line3;
            label5.Text = line4;
            label6.Text = "";
            label7.Text = line5;
            label8.Text = line6;
            label9.Text = "";
            label10.Text = line7;
            label11.Text = line8;
            label12.Text = line9;
            label13.Text = "";
            label14.Text = line10;
            label15.Text = line11;
            label16.Text = line12;
            label17.Text = line13;
        }
        public void DoDSParamPage()
        {
            string[] dsTable = Properties.Resources.DSButtons.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            DrawTable(dsTable, tabDSParam);
        }
        public void DovJoyParamPage()
        {
            string[] vJTable = Properties.Resources.vJoyButtons.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            DrawTable(vJTable, tabvJoyParam);
        }

        public void DoExtraOpPage()
        {
            string[] ExTable = Properties.Resources.ExtraParams.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            DrawTable(ExTable, tabExtra, false);
        }

        const int tableEdgePadding = 4;
        const int parmWidth = 100;
        const int descWidth = 225;
        const int typeWidth = 75;
        private void DrawTable(string[] parData, TabPage parDestination, bool par3Tabs = true)
        {
            const int textBoxHeight = 20;
            TextBox pastDescTextBox = null;
            int currentY = 7 + 20;
            int startX = tableEdgePadding;
            //Create Headers
            CreateHeader(startX, currentY - 20, "Option", parDestination);
            CreateHeader(startX + parmWidth, currentY - 20, "Description", parDestination);
            if (par3Tabs)
            {
                CreateHeader(startX + parmWidth + descWidth, currentY - 20, "Type", parDestination);
            }
            //Create Table
            foreach (string line in parData)
            {
                List<string> entry = PraseCSVLine(line);
                if (entry.Count == 1)
                    continue;
                CreateTextBox(startX, currentY, parmWidth, entry[0], parDestination);
                if (entry[1] != "")
                {
                    if (par3Tabs)
                    {
                        pastDescTextBox = CreateTextBox(startX + parmWidth, currentY, descWidth, entry[1], parDestination);
                    }
                    else
                    {
                        pastDescTextBox = CreateTextBox(startX + parmWidth, currentY, descWidth + typeWidth, entry[1], parDestination);
                    }
                    pastDescTextBox.Multiline = true;
                    pastDescTextBox.WordWrap = true;
                    pastDescTextBox.TextAlign = HorizontalAlignment.Center;
                }
                else
                {
                    pastDescTextBox.Height += textBoxHeight;
                }
                if (par3Tabs)
                {
                    CreateTextBox(startX + parmWidth + descWidth, currentY, typeWidth, entry[2], parDestination);
                }
                currentY += textBoxHeight;
            }
        }

        private List<string> PraseCSVLine(string line)
        {
            List<string> entry = new List<string>();
            string temp = "";
            const char Comma = ',';
            const char DoubleQuote = '"';
            bool isEscaped = false;
            for (int i = 0; i < line.Length; i++)
            {
                //Comma
                if (line[i] == Comma & isEscaped == false)
                {
                    entry.Add(temp);
                    temp = "";
                    continue;
                }
                //DoubleQuote
                if (line[i] == DoubleQuote)
                {
                    if (i + 1 < line.Length && line[i + 1] == DoubleQuote)
                    {
                        //Escaped DoubleQuote
                        i += 1;
                        temp += DoubleQuote;
                        continue;
                    }
                    //We are abount to escape (or unescape) commas
                    isEscaped = !isEscaped;
                    continue;
                }
                //Normal Char
                temp += line[i];
            }
            entry.Add(temp);
            return entry;
        }

        private TextBox CreateTextBox(int x, int y, int width, string text, TabPage parDestination)
        {
            MouseTransparentTextBox boxParam = new MouseTransparentTextBox();
            boxParam.ReadOnly = true;
            boxParam.Width = width;
            boxParam.Text = text.Replace("{NL}", Environment.NewLine);
            boxParam.Location = new Point(x, y);
            boxParam.Cursor = this.Cursor;
            parDestination.Controls.Add(boxParam);
            return boxParam;
        }
        private void CreateHeader(int x, int y, string text, TabPage parDestination)
        {
            Label paramHeader = new Label();
            paramHeader.Text = text;
            paramHeader.Height = 13;
            paramHeader.Width = typeWidth;
            paramHeader.Location = new Point(x, y);
            parDestination.Controls.Add(paramHeader);
        }

        private void linkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
