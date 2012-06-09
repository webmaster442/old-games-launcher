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

        public GameType EmulatorToInstall { get; set; }

        public string Command { get; set; }
        public string Arguments { get; set; }

        private bool AfterInstallCmd
        {
            get { return !string.IsNullOrEmpty(Command); }
        }

        private void WaitForDosBoxInstall_Load(object sender, EventArgs e)
        {
            label1.Text = string.Format("Installing {0}, Please Wait...", EmulatorToInstall);
            if (!Program._fileman.IsEmulatorInstalled(EmulatorToInstall))
            {
                _t = new Thread(delegate()
                {
                    Program._fileman.InstallEmulator(EmulatorToInstall);
                    if (this.InvokeRequired) this.Invoke((Action)delegate { this.Close(); });
                });
                _t.Start();
            }
            else this.Close();

        }

        private void WaitForInstall_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AfterInstallCmd) SystemCommands.RunCommand(Command, Arguments);
            e.Cancel = false;
        }
    }
}
