using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OldGamesLauncher
{
    public partial class ArgumentInput : Form
    {
        public ArgumentInput()
        {
            InitializeComponent();
        }

        public string ArgumentText
        {
            get { return TbArguments.Text; }
            set { TbArguments.Text = value; }
        }
    }
}
