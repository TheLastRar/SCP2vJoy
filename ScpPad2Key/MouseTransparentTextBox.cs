using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScpPad2vJoy
{
    // Thanks http://stackoverflow.com/a/28807364
    class MouseTransparentTextBox : TextBox
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x020A: // WM_MOUSEWHEEL
                case 0x020E: // WM_MOUSEHWHEEL
                    if (this.ScrollBars == ScrollBars.None && this.Parent != null)
                    {
                        //m.HWnd = this.Parent.Handle; // forward this to your parent
                        SendMessage(this.Parent.Handle, m.Msg, m.WParam, m.LParam);
                    }
                    //base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
