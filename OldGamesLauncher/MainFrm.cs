using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OldGamesLauncher.Properties;
using System.Threading;
using System.Diagnostics;

namespace OldGamesLauncher
{
    public partial class MainFrm : Form
    {
        private GamesManager _manager;
        private int _idtowatch;
        private string _windir;
        private DropForm _dosdop;
        private HelpBrowser _hb;
        private GamesManager.GameType _filter;
        private FormWindowState _laststate;

        public MainFrm()
        {
            InitializeComponent();
            if (SystemCommands.IsAdministrator) this.Text += " [Administrator]";
            _windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            _idtowatch = -1;
            _dosdop = new DropForm();
            _dosdop.MainWindow = this;
            _filter = GamesManager.GameType.All;
        }

        private void BuildList()
        {
            GamesList.Items.Clear();
            GamesList.Groups.Clear();
            GamesList.LargeImageList = _manager.Icons;
            GamesList.SmallImageList = _manager.Icons;
            GamesList.StateImageList = _manager.Icons;
            var games = _manager.Filter(_filter);
            foreach (var c in _manager.GetGameNameGroups())
            {
                GamesList.Groups.Add(string.Format("{0}", c), string.Format("{0}", c));
            }
            foreach (var game in games)
            {
                ListViewItem itm = new ListViewItem();
                itm.Text = game.GameName;
                itm.ImageKey = game.GameName;
                itm.Group = GamesList.Groups[game.GameName[0].ToString()];
                GamesList.Items.Add(itm);

            }

        }

        private void DoInstall(WaitForInstall.Install whattoinstall)
        {
            WaitForInstall db = new WaitForInstall();
            db.WhatToInstall = whattoinstall;
            db.Show();
        }

        private void DoInstallAndRun(WaitForInstall.Install whattoinstall, string Command, string param = null)
        {
            try
            {
                WaitForInstall db = new WaitForInstall();
                db.WhatToInstall = whattoinstall;
                db.Command = Command;
                db.Arguments = param;
                db.Show();
            }
            catch (ObjectDisposedException) { }
        }

        private void StartGame()
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            GamesData d = _manager[selected];
            bool test = SystemCommands.IsDosExe(d.GameExePath);
            if (d.isScummGame() && d.isDosboxGame)
            {
                MessageBox.Show("Game Database error. Can't decide if game is ScummVm Game or Dos game.\r\n"+
                                "Please delete game, and re add to program, with correct settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (d.isDosboxGame) DoInstallAndRun(WaitForInstall.Install.Dosbox, Program._fileman.DosBoxExe, "\"" + d.GameExePath + "\"");
            else if (d.isScummGame()) DoInstallAndRun(WaitForInstall.Install.ScummVm, Program._fileman.ScummVmExe, d.ScumGameId);
            else
            {
                if (test)
                {
                    MessageBox.Show("You are trying to run a DOS program as a Windows program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _idtowatch = SystemCommands.RunCommand(d.GameExePath);
                Thread.Sleep(1000);
                ProcessWatchTimer.Enabled = true;
                SystemCommands.KillExplorer();
            }
        }

        public void RunDosExe(string filename)
        {
            bool test = SystemCommands.IsDosExe(filename);
            if (!test)
            {
                MessageBox.Show("The file you droped is not a Dos executable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DoInstallAndRun(WaitForInstall.Install.Dosbox, Program._fileman.DosBoxExe, "\"" + filename + "\"");
            //DoInstall(WaitForInstall.Install.Dosbox, true, "\"" + filename + "\"");
            //SystemCommands.RunCommand(Program._fileman.DosBoxExe, "\"" + filename + "\"");
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            var width = Math.Min(Screen.FromControl(this).WorkingArea.Width, Settings.Default.WindowSize.Width);
            var height = Math.Min(Screen.FromControl(this).WorkingArea.Height, Settings.Default.WindowSize.Height);
            this.Left = Settings.Default.Location.X < 0 || Settings.Default.Location.X > Screen.FromControl(this).WorkingArea.Width ?  0 : Settings.Default.Location.X;
            this.Top = Settings.Default.Location.Y < 0 || Settings.Default.Location.Y > Screen.FromControl(this).WorkingArea.Height ? 0 : Settings.Default.Location.Y;
            if (width != 0 && height != 0)
            {
                this.Width = width;
                this.Height = height;
            }
            dosExeDropformToolStripMenuItem.Checked = Settings.Default.DropVisible;
            groupsVisibleToolStripMenuItem.Checked = Settings.Default.GroupsVisible;
            showEmulatorsConsoleToolStripMenuItem.Checked = Settings.Default.EmuConsoleVisible;
            closeToTrayToolStripMenuItem.Checked = Settings.Default.CloseToTray;

            _manager = new GamesManager();
            Program._fileman = new FileManager();
            _manager.LoadDataFile(Program._fileman.ConfigLocation);
            BuildList();
            if (Settings.Default.DropVisible) _dosdop.Show();
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.Default.CloseToTray)
            {
                minimizeToTrayToolStripMenuItem_Click(sender, new EventArgs());
                e.Cancel = true;
                return;
            }
            Size s = new System.Drawing.Size(this.Width, this.Height);
            Settings.Default.WindowSize = s;
            _manager.SaveDataFile(Program._fileman.ConfigLocation);
            Settings.Default.DropVisible = dosExeDropformToolStripMenuItem.Checked;
            Settings.Default.GroupsVisible = groupsVisibleToolStripMenuItem.Checked;
            Settings.Default.EmuConsoleVisible = showEmulatorsConsoleToolStripMenuItem.Checked;
            Settings.Default.CloseToTray = closeToTrayToolStripMenuItem.Checked;
            Settings.Default.Save();
            e.Cancel = false;
        }

        private void ProcessWatchTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var proc = Process.GetProcessById(_idtowatch);
                if (proc == null)
                {

                    SystemCommands.RunCommand(_windir + "\\explorer.exe");
                    _idtowatch = -1;
                    ProcessWatchTimer.Enabled = false;
                }
            }
            catch
            {
                SystemCommands.RunCommand(_windir + "\\explorer.exe");
                _idtowatch = -1;
                ProcessWatchTimer.Enabled = false;
            }
        }

        private void GamesList_DoubleClick(object sender, EventArgs e)
        {
            StartGame();
        }

        private void FilterSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (FilterSelector.SelectedIndex)
            {
                case 0:
                    _filter = GamesManager.GameType.All;
                    break;
                case 1:
                    _filter = GamesManager.GameType.Windows;
                    break;
                case 2:
                    _filter = GamesManager.GameType.Dos;
                    break;
                case 3:
                    _filter = GamesManager.GameType.Scumm;
                    break;
            }
            BuildList();
        }

        #region MenuItems
        private void addGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddGameForm agf = new AddGameForm();
            if (agf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _manager.AddGame(agf.GameName, agf.GamePath, agf.IsDosExe);
                BuildList();
            }
        }

        private void addScumVmGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddScumGameForm asgf = new AddScumGameForm();
            if (asgf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _manager.AddGame(asgf.GameName, asgf.GamePath, false, asgf.GameId);
                Program._fileman.AddScummGame(asgf.GameId, asgf.GameName, asgf.GamePath);
                BuildList();
            }
        }

        private void SetViewType(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            switch (s.Name)
            {
                case "largeIconsToolStripMenuItem":
                    GamesList.View = View.LargeIcon;
                    break;
                case "smallIconsToolStripMenuItem":
                    GamesList.View = View.SmallIcon;
                    break;
                case "listToolStripMenuItem":
                    GamesList.View = View.List;
                    break;
                case "tileToolStripMenuItem":
                    GamesList.View = View.Tile;
                    break;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void restartExplorerexeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemCommands.KillExplorer();
            Thread.Sleep(250);
            SystemCommands.RunCommand(_windir + "\\explorer.exe");
        }

        private void restartAsAdministratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SystemCommands.Elevate()) Application.Exit();
        }

        #endregion


        #region Context menu

        private void ListContext_Opening(object sender, CancelEventArgs e)
        {
            if (GamesList.SelectedIndices.Count < 1)
            {
                internetToolStripMenuItem.Enabled = false;
                deleteGameToolStripMenuItem.Enabled = false;
                EditToolStripMenuItem.Enabled = false;
                startGameToolStripMenuItem.Enabled = false;
                openFolderToolStripMenuItem.Enabled = false;
                gameExePropertiesToolStripMenuItem.Enabled = false;
                directDrawHackToolStripMenuItem.Enabled = false;
                gameSettingsToolStripMenuItem.Enabled = false;
            }
            else
            {
                var selected = GamesList.SelectedItems[0].Text;
                bool scummgame = _manager[selected].isScummGame();
                bool dosexe = _manager[selected].isDosboxGame;
                internetToolStripMenuItem.Enabled = true;
                deleteGameToolStripMenuItem.Enabled = true;
                EditToolStripMenuItem.Enabled = true;
                openFolderToolStripMenuItem.Enabled = true;
                startGameToolStripMenuItem.Enabled = true;
                gameSettingsToolStripMenuItem.Enabled = SystemCommands.SetupExists(_manager.GetPathByName(selected));
                if (!dosexe & !scummgame)
                {
                    gameExePropertiesToolStripMenuItem.Enabled = true;
                    directDrawHackToolStripMenuItem.Enabled = true;
                    gameSettingsToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void startGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            string path = Path.GetDirectoryName(_manager.GetPathByName(selected));
            SystemCommands.RunCommand("explorer.exe", path);
        }

        private void InternetMenuItemClick(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            switch (s.Name)
            {
                case "googleToolStripMenuItem":
                    SystemCommands.OpenWebLocation("https://www.google.hu/search?q=" + selected);
                    break;
                case "wikipediaLookupToolStripMenuItem":
                    SystemCommands.OpenWebLocation("http://en.wikipedia.org/wiki/" + selected);
                    break;
                case "searchCheatsToolStripMenuItem":
                    SystemCommands.OpenWebLocation("https://www.google.hu/search?q=" + selected + " cheats");
                    break;
            }
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;

            var dat = _manager[selected];
            int index = _manager.IndexOf(dat);

            if (!dat.isScummGame())
            {
                AddGameForm ed = new AddGameForm();
                ed.GameName = dat.GameName;
                ed.GamePath = dat.GameExePath;
                ed.IsDosExe = dat.isDosboxGame;
                if (ed.ShowDialog() == DialogResult.OK)
                {
                    dat.GameName = ed.GameName;
                    dat.GameExePath = ed.GamePath;
                    dat.isDosboxGame = ed.IsDosExe;
                    _manager[index] = dat;
                }
            }
            else
            {
                AddScumGameForm esg = new AddScumGameForm();
                esg.GameName = dat.GameName;
                esg.GamePath = dat.GameExePath;
                esg.GameId = dat.ScumGameId;
                if (esg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dat.GameName = esg.GameName;
                    dat.GameExePath = esg.GamePath;
                    dat.ScumGameId = esg.GameId;
                    _manager[index] = dat;
                }
            }
            _manager.RebuildIconIndex();
            BuildList();
        }

        private void deleteGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            var confirm = MessageBox.Show("Delete " + selected + "?", "Confirm action", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (confirm == System.Windows.Forms.DialogResult.Yes)
            {
                if (_manager[selected].isScummGame()) Program._fileman.RemoveScummGame(_manager[selected].ScumGameId);
                _manager.RemoveByName(selected);
                BuildList();
            }
        }

        private void DosBoxManage(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            //DoInstall(WaitForInstall.Install.Dosbox, false);
            switch (s.Name)
            {
                case "editConfigurationToolStripMenuItem":
                    DoInstallAndRun(WaitForInstall.Install.Dosbox, Program._fileman.DosBoxPath + "\\DOSBox 0.74 Options.bat");
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\DOSBox 0.74 Options.bat");
                    break;
                case "openScreenshotsRecordingsToolStripMenuItem":
                    DoInstallAndRun(WaitForInstall.Install.Dosbox, Program._fileman.DosBoxPath + "\\Screenshots & Recordings.bat");
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\Screenshots & Recordings.bat");
                    break;
                case "resetConfigurationToolStripMenuItem":
                    DoInstallAndRun(WaitForInstall.Install.Dosbox, Program._fileman.DosBoxPath + "\\Reset Options.bat");
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\Reset Options.bat");
                    break;
                case "resetKeyMappingsToolStripMenuItem":
                    DoInstallAndRun(WaitForInstall.Install.Dosbox, Program._fileman.DosBoxPath + "\\Reset KeyMapper.bat");
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\Reset KeyMapper.bat");
                    break;
            }
        }

        private void startDosBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Program._fileman.DosBoxPath
            DoInstallAndRun(WaitForInstall.Install.Dosbox, Program._fileman.DosBoxExe);
            //DoInstall(WaitForInstall.Install.Dosbox, true);
            
        }

        private void startScummVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoInstallAndRun(WaitForInstall.Install.ScummVm, Program._fileman.ScummVmExe);
            //DoInstall(WaitForInstall.Install.ScummVm, true);
            
        }

        private void InstallEmulator(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            string toinstall = null;
            switch (s.Name)
            {
                case "installReinstallDosBoxToolStripMenuItem":
                    toinstall = "DosBox";
                    break;
                case "installReinstallScummVmToolStripMenuItem":
                    toinstall = "ScummVM";
                    break;
            }
            var res = MessageBox.Show(string.Format("Install/Reinstall {0}?", toinstall), string.Format("{0} Installer", toinstall), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                switch (s.Name)
                {
                    case "installReinstallDosBoxToolStripMenuItem":
                        if (Program._fileman.IsDosboxInstalled()) Program._fileman.DeleteDosBox();
                        DoInstall(WaitForInstall.Install.Dosbox);
                        break;
                    case "installReinstallScummVmToolStripMenuItem":
                        if (Program._fileman.IsDosboxInstalled()) Program._fileman.DeleteScummVm();
                        DoInstall(WaitForInstall.Install.ScummVm);
                        break;
                }
            }
        }

        private void UninstallEmulator(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            string toinstall = null;
            switch (s.Name)
            {
                case "uninstallDosBoxToolStripMenuItem":
                    toinstall = "DosBox";
                    break;
                case "uninstallScummVmToolStripMenuItem":
                    toinstall = "ScummVM";
                    break;
            }
            var res = MessageBox.Show(string.Format("Uninstall {0}?", toinstall), string.Format("{0} Installer", toinstall), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                switch (s.Name)
                {
                    case "uninstallDosBoxToolStripMenuItem":
                        if (Program._fileman.DeleteDosBox()) MessageBox.Show("DosBox Uninstalled", "DosBox Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "uninstallScummVmToolStripMenuItem":
                        if (Program._fileman.DeleteScummVm()) MessageBox.Show("ScummVM Uninstalled", "ScummVM Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
        }

        private void HelpHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            _hb = new HelpBrowser();
            switch (s.Name)
            {
                case "releaseNotesToolStripMenuItem":
                    _hb.DocIndex = 0;
                    break;
                case "licenseToolStripMenuItem":
                    _hb.DocIndex = 1;
                    break;
                case "dosBoxReadmeToolStripMenuItem":
                    _hb.DocIndex = 2;
                    break;
                case "compatiblitySettingsToolStripMenuItem":
                    _hb.DocIndex = 3;
                    break;
                case "dosBoxKeysToolStripMenuItem":
                    _hb.DocIndex = 4;
                    break;
            }
            _hb.Show();
        }

        private void gameExePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            SystemCommands.ShowFileProperties(_manager.GetPathByName(selected));
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            var res = MessageBox.Show("Install Direct Draw hack to " + selected + "?", "DirectDraw Hack Installer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                string directory = Path.GetDirectoryName(_manager.GetPathByName(selected));
                Program._fileman.InstallDirectDrawHack(directory);
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            var res = MessageBox.Show("Uninstall Direct Draw hack from " + selected + "?", "DirectDraw Hack Installer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                string directory = Path.GetDirectoryName(_manager.GetPathByName(selected));
                if (Program._fileman.DeleteDirectDrawHack(directory))
                {
                    MessageBox.Show("Direct Draw hack uninstalled", "DirectDraw Hack Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void dosExeDropformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dosExeDropformToolStripMenuItem.Checked) _dosdop.Show();
            else _dosdop.Hide();
        }

        private void gameSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            string setup = SystemCommands.GetSetup(_manager.GetPathByName(selected));
            if (string.IsNullOrEmpty(setup)) return;
            RunDosExe(setup);
        }

        private void groupsVisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GamesList.ShowGroups = groupsVisibleToolStripMenuItem.Checked;
        }
        #endregion

        private void minimizeToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _laststate = this.WindowState;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            this.ShowInTaskbar = false;
        }

        private void Tray_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized || this.Visible == false)
            {
                this.Visible = true;
                this.ShowInTaskbar = true;
                this.WindowState = _laststate;
                this.BringToFront();
                this.Activate();
            }
            else minimizeToTrayToolStripMenuItem_Click(sender, e);
        }

        private void showEmulatorsConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.EmuConsoleVisible = showEmulatorsConsoleToolStripMenuItem.Checked;
        }

        private void closeToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.CloseToTray = closeToTrayToolStripMenuItem.Checked;
        }

        private void GetGamesMenu(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            switch(s.Name)
            {
                case "getDosDemosToolStripMenuItem":
                    SystemCommands.OpenWebLocation("http://www.dosgamesarchive.com/");
                    break;
                case "getScumVMDemosToolStripMenuItem":
                    SystemCommands.OpenWebLocation("http://www.scummvm.org/demos/");
                    break;
                case "visitGOGcomToolStripMenuItem":
                    SystemCommands.OpenWebLocation("http://www.gog.com/");
                    break;
            }
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Warning! This will delete all games in collection and can't be undone.\r\n" +
                                          "Are you sure you want to do this?", "Warning", MessageBoxButtons.YesNo,  MessageBoxIcon.Warning);
            if (confirm == System.Windows.Forms.DialogResult.Yes)
            {
                _manager.Clear();
                BuildList();
            }
        }
    }
}
