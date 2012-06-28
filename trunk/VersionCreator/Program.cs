using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionHandling;
using System.IO;
using System.Windows.Forms;

namespace VersionCreator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            PackVersion ver = null;
            sfd.Filter = "*.xml|xml files";
            sfd.Title = "Select output file";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DialogResult res = MessageBox.Show("Automaticaly create versioninfo based on current date?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes) ver = new PackVersion(DateTime.Now);
                else
                {
                    VersionSet s = new VersionSet();
                    if (s.ShowDialog() == DialogResult.OK)
                    {
                        ver = new PackVersion(s.SelectedDate);
                    }
                }
                if (ver != null)
                {
                    try
                    {
                        VersionHandler.CreateVersionXml(sfd.FileName, ver);
                        MessageBox.Show("Version xml successfully saved", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
