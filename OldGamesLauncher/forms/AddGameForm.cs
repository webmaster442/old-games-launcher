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
        private bool Warn;

        public AddGameForm()
        {
            InitializeComponent();
            Warn = false;
            CbGameType.SelectedIndex = 0;
            OpenExeDialog.Filter = Properties.Resources.FileTypes.Replace("\r\n", "|");
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
                if (OpenExeDialog.FilterIndex == 0)
                {
                    if (SystemCommands.IsDosExe(OpenExeDialog.FileName)) CbGameType.SelectedIndex = 1;
                    else CbGameType.SelectedIndex = 0;
                }
                else CbGameType.SelectedIndex = 2;
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
                MessageBox.Show("Select Game path");
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

        public string Arguments
        {
            get { return TbArgs.Text; }
            set { TbArgs.Text = value; }
        }

        public GameType SelectedGameType
        {
            get
            {
                switch (CbGameType.SelectedIndex)
                {
                    case 0:
                        return GameType.Windows;
                    case 1:
                        return GameType.DosBox;
                    case 2:
                        return GameType.Snes;
                    default:
                        return GameType.Windows;
                }
            }
            set
            {
                switch (value)
                {
                    case GameType.Windows:
                        CbGameType.SelectedIndex = 0;
                        break;
                    case GameType.DosBox:
                        CbGameType.SelectedIndex = 1;
                        break;
                    case GameType.Snes:
                        CbGameType.SelectedIndex = 2;
                        break;
                }
            }
        }

        private void CbGameType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Warn)
            { 
                Warn = true;
                return;
            }
            else if (SystemCommands.IsDosExe(TbGamePath.Text) && SelectedGameType != GameType.DosBox && !string.IsNullOrEmpty(TbGamePath.Text))
            {
                var result = MessageBox.Show("The Game seems to be a dos game. With wrong configuration, the game won't start.\r\n" +
                             "Do you want to set correct settings now?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes) CbGameType.SelectedIndex = 1;
            }

        }
    }
}
