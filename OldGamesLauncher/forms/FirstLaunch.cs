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
    public partial class FirstLaunch : Form
    {
        private Thread _t;

        public FirstLaunch()
        {
            InitializeComponent();
        }

        public MainFrm CallerForm { get; set; }
        public GamesData Data { get; set; }

        private void FirstLaunch_Load(object sender, EventArgs e)
        {
            _t = new Thread(delegate()
            {
                int index = Program._manager.IndexOf(Data);
                bool check = SystemCommands.DirectDrawTest(Data.GameExePath);
                if (check) Data.DirectDraw = UsesDDraw.True;
                else Data.DirectDraw = UsesDDraw.False;
                Program._manager[index] = Data;
                if (this.InvokeRequired) this.Invoke((Action)delegate 
                {
                    CallerForm.LaunchdDraw(Data);
                    this.Close();
                });
            });
            _t.Start();
        }
    }
}
