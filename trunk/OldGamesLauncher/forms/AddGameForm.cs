using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OldGamesLauncher
{
    public partial class AddGameForm : Form
    {
        public AddGameForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TbGamePath.Text))
                {
                    string dir = Path.GetDirectoryName(TbGamePath.Text);
                    if (Directory.Exists(dir)) OpenExeDialog.InitialDirectory = dir;
                }
            }
            catch (IOException) { }
            if (OpenExeDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.TbGamePath.Text = OpenExeDialog.FileName;
                this.CbDosExe.Checked = SystemCommands.IsDosExe(OpenExeDialog.FileName);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            bool ok = true;
            if (TbGameName.Text.Length < 1)
            {
                MessageBox.Show("Enter Game name");
                ok = false;
            }
            if (TbGamePath.Text.Length < 1)
            {
                MessageBox.Show("Select Game Exe");
                ok = false;
            }
            if (ok) this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public string GameName
        {
            get { return TbGameName.Text; }
            set { TbGameName.Text = value; }
        }

        public string GamePath
        {
            get { return TbGamePath.Text; }
            set { TbGamePath.Text = value; }
        }

        public bool IsDosExe
        {
            get { return CbDosExe.Checked; }
            set { CbDosExe.Checked = value; }
        }

        private void CbDosExe_CheckedChanged(object sender, EventArgs e)
        {
            if (!CbDosExe.Checked && SystemCommands.IsDosExe(TbGamePath.Text))
            {
                var result = MessageBox.Show("The Game seems to be a dos game. With wrong configuration, the game won't start.\r\n" +
                                             "Do you want to set correct settings now?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes) CbDosExe.Checked = true;
            }
        }
    }
}
