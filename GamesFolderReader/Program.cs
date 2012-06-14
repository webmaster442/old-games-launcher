using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace GamesFolderReader
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        static void Main(string[] args)
        {
            RegistryKey games = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\GameUX\Games", false);
            RegistryKey subk = null;
            foreach (var subkey in games.GetSubKeyNames())
            {
                subk = games.OpenSubKey(subkey);
                Console.WriteLine(subk.GetValue("ConfigGDFBinaryPath").ToString());
                Console.WriteLine(DecodeName(subk.GetValue("DisplayName").ToString()));
            }
            Console.ReadLine();
        }

        private static string DecodeName(string input)
        {
            if (!input.StartsWith("@")) return input;
            string[] dllindex = input.Replace("@", "").Split(',');
            IntPtr dll = LoadLibrary(dllindex[0]);
            StringBuilder dec = new StringBuilder(256);
            LoadString(dll, Convert.ToUInt32(dllindex[1].Replace("-", "")), dec, dec.Capacity);
            FreeLibrary(dll);
            return dec.ToString();
        }
    }
}
//OFTWARE\Microsoft\Windows\CurrentVersion\GameUX