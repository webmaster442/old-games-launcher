using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.IconLib;
using System.Linq;

namespace OldGamesLauncher
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SHELLEXECUTEINFO
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    internal static class SystemCommands
    {
        private static string[] setups;

        static SystemCommands()
        {
            setups = new string[] { "setup.exe", "setup.com" };
        }

        public static int RunCommand(string cmd, string args = "", bool cmdemu = false)
        {
            Process Command = new Process();
            Command.StartInfo.UseShellExecute = true;
            Command.StartInfo.FileName = cmd;

            string dir = Path.GetDirectoryName(cmd);
            if (dir.Length < 1) dir = Path.GetDirectoryName(Application.ExecutablePath);

            Command.StartInfo.WorkingDirectory = dir;
            if (!Properties.Settings.Default.EmuConsoleVisible && cmdemu) Command.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Command.StartInfo.Arguments = args;
            Command.Start();
            return Command.Id;
        }

        public static bool ExplorerIsRunning()
        {
            try
            {
                Process[] results = Process.GetProcessesByName("explorer");
                return results.Length > 0;
            }
            catch (Exception) { return false; }
        }

        public static void KillExplorer()
        {
            RunCommand("taskkill", "/F /IM Explorer.exe");
        }

        public static bool Elevate()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = Application.ExecutablePath;
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (Win32Exception) { return false; }
        }

        public static Icon GetThumbnail(string filepath)
        {
            MultiIcon mi = new MultiIcon();
            mi.Load(filepath);
            SingleIcon icon = mi[0];
            var query = from i in icon where i.Image.Width == 48 && i.Image.Height == 48 orderby i.PixelFormat descending select i.Icon;
            Icon ret = query.FirstOrDefault();
            if (ret == null)
                ret = (from i in icon where i.Image.Width == 32 && i.Image.Height == 32 orderby i.PixelFormat descending select i.Icon).FirstOrDefault();
            return ret;
        }

        public static bool IsAdministrator
        {
            get
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static void OpenWebLocation(string Location)
        {
            try
            {
                Process.Start(Location);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        public static void ShowFileProperties(string Filename)
        {
            const int SW_SHOW = 5;
            const uint SEE_MASK_INVOKEIDLIST = 12;
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            ShellExecuteEx(ref info);
        }

        public static bool IsDosExe(string filename)
        {
            if (filename.EndsWith(".com")) return true;
            else if (filename.EndsWith(".cmd")) return false;
            else if (filename.EndsWith(".bat")) return true;
            try
            {
                _3DViewerControls.Data.PeHeaderReader pe = new _3DViewerControls.Data.PeHeaderReader(filename);
                const ushort IMAGE_FILE_MACHINE_I386 = 0x014c;
                const ushort IMAGE_FILE_MACHINE_IA64 = 0x0200;
                const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;
                switch (pe.FileHeader.Machine)
                {
                    case IMAGE_FILE_MACHINE_I386:
                    case IMAGE_FILE_MACHINE_IA64:
                    case IMAGE_FILE_MACHINE_AMD64:
                        return false;
                    default:
                        return true;
                }
            }
            catch (Exception) { return true; }
        }

        public static bool SetupExists(string gameexe)
        {
            string directory = Path.GetDirectoryName(gameexe);
            foreach (var setup in setups)
            {
                if (File.Exists(directory + "\\" + setup)) return true;
            }
            return false;
        }

        public static string GetSetup(string gameexe)
        {
            string directory = Path.GetDirectoryName(gameexe);
            foreach (var setup in setups)
            {
                if (File.Exists(directory + "\\" + setup)) return directory + "\\" + setup;
            }
            return null;
        }

        public static bool DirectDrawTest(string exepath)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.Arguments = "/c findstr -i .dll \"" + exepath + "\" | more | findstr -i .dll | more";
            p.Start();
            string[] output = p.StandardOutput.ReadToEnd().ToLower().Split('\n');
            p.WaitForExit();
            foreach (var dll in output)
            {
                if (dll.StartsWith("ddraw.dll")) return true;
            }
            return false;
        }
    }
}