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
    public partial class AddScumGameForm : Form
    {
        private List<string> _keys, _values;

        public AddScumGameForm()
        {
            InitializeComponent();
            _keys = new List<string>();
            _values = new List<string>();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TbGamePath.Text))
                {
                    if (Directory.Exists(TbGamePath.Text)) folderBrowserDialog1.SelectedPath = TbGamePath.Text;
                }
            }
            catch (IOException) { }
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TbGamePath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void AddScumGame_Load(object sender, EventArgs e)
        {
            string[] rows = Properties.Resources.scumgames.Split('\n');
            foreach (var row in rows)
            {
                string[] it = row.Split('|');
                if (it.Length > 1)
                {
                    //_items.Add(it[0], it[1]);
                    _keys.Add(it[0]);
                    _values.Add(it[1]);
                    LbGame.Items.Add(it[1]);
                }
                else
                {
                    _keys.Add(it[0]);
                    _values.Add(it[0]);
                    LbGame.Items.Add(it[0]);
                }
            }
        }

        public string GameName
        {
            get 
            {
                int index = LbGame.SelectedIndex;
                return _values[index];
            }
            set
            {
                int index = _values.IndexOf(value);
                LbGame.SelectedIndex = index;
            }
        }

        public string GamePath
        {
            get { return TbGamePath.Text; }
            set { TbGamePath.Text = value; }
        }

        public string GameId
        {
            get
            {
                int index = LbGame.SelectedIndex;
                return _keys[index];
            }
            set
            {
                int index = _keys.IndexOf(value);
                LbGame.SelectedIndex = index;
            }
        }
    }
}
