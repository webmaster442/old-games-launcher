using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OldGamesLauncher.Properties;

namespace OldGamesLauncher
{
    public partial class DropForm : Form
    {
        public DropForm()
        {
            InitializeComponent();
        }

        public MainFrm MainWindow { get; set; }

        private void DropForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        private void DropForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        private void DropForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array a = (Array)e.Data.GetData(DataFormats.FileDrop);

                if (a != null)
                {
                    // Extract string from first array element
                    // (ignore all files except first if number of files are dropped).
                    string s = a.GetValue(0).ToString();
                    string arg = null;
                    if (MainWindow != null)
                    {
                        if (CbArgumentPass.Checked)
                        {
                            ArgumentInput ai = new ArgumentInput();
                            if (ai.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                arg = ai.ArgumentText;
                            }
                        }
                        if (!string.IsNullOrEmpty(arg)) MainWindow.RunDosExe(s + " " + arg);
                        else MainWindow.RunDosExe(s);
                    }
                    this.Activate();        // in the case Explorer overlaps this form
                }
            }
            catch (Exception) { }
        }
    }
}
