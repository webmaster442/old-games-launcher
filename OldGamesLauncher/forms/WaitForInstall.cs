using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace OldGamesLauncher
{
    public partial class WaitForInstall : Form
    {
        public WaitForInstall()
        {
            InitializeComponent();
        }

        private Thread _t;

        public Install WhatToInstall { get; set; }

        public string Command { get; set; }
        public string Arguments { get; set; }

        private bool AfterInstallCmd
        {
            get { return !string.IsNullOrEmpty(Command); }
        }

        public enum Install
        {
            Dosbox, ScummVm
        }

        private void WaitForDosBoxInstall_Load(object sender, EventArgs e)
        {
            switch (WhatToInstall)
            {
                case Install.Dosbox:
                    label1.Text = "Installing DosBox, Please Wait...";
                    if (!Program._fileman.IsDosboxInstalled())
                    {
                        _t = new Thread(delegate()
                        {
                            Program._fileman.InstallDosDox();
                            if (this.InvokeRequired) this.Invoke((Action)delegate { this.Close(); });
                        });
                        _t.Start();
                    }
                    else this.Close();
                    break;
                case Install.ScummVm:
                    label1.Text = "Installing ScumVM, Please Wait...";
                    if (!Program._fileman.IsScummVmInstalled())
                    {
                        _t = new Thread(delegate()
                        {
                            Program._fileman.InstallScummVm();
                            if (this.InvokeRequired) this.Invoke((Action)delegate { this.Close(); });
                        });
                        _t.Start();
                    }
                    else this.Close();
                    break;
            }
        }

        private void WaitForInstall_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AfterInstallCmd) SystemCommands.RunCommand(Command, Arguments);
            e.Cancel = false;
        }
    }
}
