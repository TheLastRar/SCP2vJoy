using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace ScpPad2vJoy
{
    static class Program
    {
        const string VJOYKEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{8E31F76F-74C3-47F1-9550-E041EEDC5FBB}_is1";
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetupLoggin();
            string file = GetvJoyPath();

            //ClearFromOldInstall();
            SetDllDirectory(file);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScpForm());
        }

        static string GetvJoyPath()
        {
            string file = "";
            RegistryKey _baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey _regKey = _baseKey.OpenSubKey(VJOYKEY_PATH, false);
            if (_regKey != null)
            {
                
                Boolean is64 = Environment.Is64BitProcess;
                if (is64 == true)
                {
                    file = (string)_regKey.GetValue("DllX64Location");
                }
                else
                {
                    file = (string)_regKey.GetValue("DllX86Location");
                }
            }
            _regKey.Close();
            _baseKey.Close();
            return file;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        static void ClearFromOldInstall()
        {
            //The dll search path searches the application directory 1st
            //before searching the directory specified in SetDllDirectory
            //To ensure we get the vJoyInterface that matches the installed
            //driver, we delete the vJoyInterface.dll that was bundled in 
            //older versions
            if (File.Exists("vJoyInterface.dll"))
            {
                File.Delete("vJoyInterface.dll");
            }
        }

        static void SetupLoggin()
        {
            if (File.Exists("Pad2vJoy.log"))
            {
                File.Delete("Pad2vJoy.log");
            }

            Trace.Listeners.Clear();

            TextWriterTraceListener twtl = new TextWriterTraceListener("Pad2vJoy.log");
            twtl.Writer.NewLine = "\n";
            twtl.Name = "TextLogger";
            twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;

            ConsoleTraceListener ctl = new ConsoleTraceListener(false);
            ctl.TraceOutputOptions = TraceOptions.DateTime;

            //Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;

            Trace.WriteLine("Trace Active");
        }
    }
}
