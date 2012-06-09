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
    public partial class HelpBrowser : Form
    {
        public HelpBrowser()
        {
            InitializeComponent();
        }

        private void TabSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            RtbHelp.Clear();
            switch (TabSelector.SelectedIndex)
            {
                case 0:
                    RtbHelp.LoadFile(Program._fileman.GetDocumentContent("Notes.rtf"), RichTextBoxStreamType.RichText);
                    break;
                case 1:
                    RtbHelp.LoadFile(Program._fileman.GetDocumentContent("license.rtf"), RichTextBoxStreamType.RichText);
                    break;
                case 2:
                    RtbHelp.LoadFile(Program._fileman.GetDocumentContent("DOSBoxManual.txt"), RichTextBoxStreamType.PlainText);
                    break;
                case 3:
                    RtbHelp.LoadFile(Program._fileman.GetDocumentContent("compatibilitysettings.rtf"), RichTextBoxStreamType.RichText);
                    break;
                case 4:
                    RtbHelp.LoadFile(Program._fileman.GetDocumentContent("dosboxkeys.txt"), RichTextBoxStreamType.PlainText);
                    break;
            }
        }

        public int DocIndex
        {
            get { return TabSelector.SelectedIndex; }
            set 
            {
                TabSelector.SelectedIndex = value;
                TabSelector_SelectedIndexChanged(null, null);
            }
        }
    }
}
