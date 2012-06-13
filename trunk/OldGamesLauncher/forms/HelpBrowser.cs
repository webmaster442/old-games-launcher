using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OldGamesLauncher.Properties;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace OldGamesLauncher
{
    public partial class HelpBrowser : Form
    {
        public HelpBrowser()
        {
            InitializeComponent();
            ListItems();
        }

        private void ListItems()
        {
            TreeNode lastNode = null;
            string subPathAgg;
            string pathext;
            TvDocs.PathSeparator = "/";
            try
            {
                FileStream fs = File.OpenRead(Program._fileman.DocsPath);
                using (ZipInputStream zi = new ZipInputStream(fs))
                {
                    ZipEntry file;
                    while ((file = zi.GetNextEntry()) != null)
                    {
                        pathext = "Documents/" + file.Name;
                        subPathAgg = string.Empty;
                        foreach (string subPath in pathext.Split('/'))
                        {
                            if (string.IsNullOrEmpty(subPath)) continue;
                            subPathAgg += subPath + '/';
                            TreeNode[] nodes = TvDocs.Nodes.Find(subPathAgg, true);
                            if (nodes.Length == 0)
                            {
                                if (lastNode == null) lastNode = TvDocs.Nodes.Add(subPathAgg, subPath);
                                else lastNode = lastNode.Nodes.Add(subPathAgg, subPath);
                            }
                            else lastNode = nodes[0];
                        }
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Can't Get documentation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HelpBrowser_Load(object sender, EventArgs e)
        {
            TvDocs.ExpandAll();
        }

        private void TvDocs_DoubleClick(object sender, EventArgs e)
        {
            if (TvDocs.SelectedNode == null) return;
            string path = TvDocs.SelectedNode.FullPath.Replace("Documents/", "");
            LoadDocument(path);
        }

        public void LoadDocument(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            MemoryStream data = Program._fileman.GetDocumentContent(path);
            if (data == null)
            {
                MessageBox.Show("Document not found: " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (path.EndsWith(".txt")) RtbHelp.LoadFile(data, RichTextBoxStreamType.PlainText);
            else if (path.EndsWith(".rtf")) RtbHelp.LoadFile(data, RichTextBoxStreamType.RichText);
        }
    }

}

