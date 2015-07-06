using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScpPad2vJoy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Boolean is64 = Environment.Is64BitProcess;
            string file = "";
            if (is64 == true)
            {
                file = "x64\\vJoyInterface.dll";
            } 
            else
            {
                file = "x86\\vJoyInterface.dll";
            }
            if (!IsRightFile(file))
            {
                System.IO.File.Copy(file, "vJoyInterface.dll", true);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScpForm());
        }

        static Boolean IsRightFile(string file)
        {
            //check file exists
            if (!System.IO.File.Exists("vJoyInterface.dll"))
            {
                return false;
            }

            //check file same size
            System.IO.FileInfo fi_existing = new System.IO.FileInfo("vJoyInterface.dll");
            System.IO.FileInfo fi_using = new System.IO.FileInfo(file);
            if (fi_existing.Length != fi_using.Length)
            {
                return false;
            }

            //check file same data
            byte[] by_existing = System.IO.File.ReadAllBytes("vJoyInterface.dll");
            byte[] by_using = System.IO.File.ReadAllBytes(file);
            if (!by_existing.SequenceEqual(by_using))
            {
                return false;
            }
            return true;
        }
    }
}
