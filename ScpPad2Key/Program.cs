using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScpPad2vJoy
{
    static class Program
    {
        const string VJOYKEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{8E31F76F-74C3-47F1-9550-E041EEDC5FBB}_is1";
        const string SCPKEY_PATH = @"SOFTWARE\Nefarius Software Solutions\ScpToolkit";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetupLoggin();
            Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentCulture.Name);

            //ClearFromOldInstall();

            string file = GetvJoyPath();
            SetDllDirectory(file);

            AppDomain.CurrentDomain.AssemblyResolve += ResolveSCP;

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
                bool is64 = Environment.Is64BitProcess;
                if (is64 == true)
                {
                    file = (string)_regKey.GetValue("DllX64Location");
                }
                else
                {
                    file = (string)_regKey.GetValue("DllX86Location");
                }
            }
            if (_regKey != null)
                _regKey.Close();
            if (_baseKey != null)
                _baseKey.Close();
            return file;
        }

        static string GetSCPPath()
        {
            string file = "";
            //Key is in 32bit regs
            RegistryKey _baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey _regKey = _baseKey.OpenSubKey(SCPKEY_PATH, false);
            if (_regKey != null)
            {
                file = (string)_regKey.GetValue("Path");
            }
            if (_regKey != null)
                _regKey.Close();
            if (_baseKey != null)
                _baseKey.Close();
            return file;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        //static void ClearFromOldInstall()
        //{
        //    //The dll search path searches the application directory 1st
        //    //before searching the directory specified in SetDllDirectory
        //    //To ensure we get the vJoyInterface that matches the installed
        //    //driver, we delete the vJoyInterface.dll that was bundled in 
        //    //older versions
        //    if (File.Exists("vJoyInterface.dll"))
        //    {
        //        File.Delete("vJoyInterface.dll");
        //    }
        //}

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

        private static Assembly ResolveSCP(object sender, ResolveEventArgs args)
        {
            AssemblyName aName = new AssemblyName(args.Name);

            string file = GetSCPPath();

            switch (aName.Name)
            {
                case "ScpControl":
                case "ScpControl.Shared":

                case "CSScriptLibrary":

                case "DBreeze":

                case "HidSharp":
                case "Libarius":
                case "log4net":
                case "MadMilkman.Ini":

                case "Mono.CSharp":
                case "Newtonsoft.Json":

                case "ReactiveSockets":
                case "System.Reactive.Core":
                case "System.Reactive.Interfaces":
                case "System.Reactive.Linq":
                case "System.Reactive.PlatformServices": 

                case "Trinet.Core.IO.Ntfs":
                case "WindowsInput":
                      
                    return Assembly.LoadFile(file + aName.Name + ".dll");

                case "System.Reactive.Debugger":
                    if (File.Exists(file + aName.Name + ".dll"))
                    {
                        return Assembly.LoadFile(file + aName.Name + ".dll");
                    }
                    else
                    {
                        return null;
                    }
                case "ScpPad2vJoy.resources":
                    return null;
            }
            
            return null;
        }
    }
}
