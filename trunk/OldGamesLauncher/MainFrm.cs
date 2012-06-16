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
        private int _idtowatch;
        private string _windir;
        private DropForm _dosdop;
        private HelpBrowser _hb;
        private GameType _filter;
        private FormWindowState _laststate;

        public MainFrm()
        {
            InitializeComponent();
            if (SystemCommands.IsAdministrator) this.Text += " [Administrator]";
            _windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            _idtowatch = -1;
            _dosdop = new DropForm();
            _dosdop.MainWindow = this;
            _filter = GameType.All;
        }

        private void BuildList()
        {
            GamesList.Items.Clear();
            GamesList.Groups.Clear();
            GamesList.ShowGroups = Settings.Default.GroupsVisible;
            GamesList.LargeImageList = Program.GameMan.Icons;
            GamesList.SmallImageList = Program.GameMan.Icons;
            GamesList.StateImageList = Program.GameMan.Icons;
            var games = Program.GameMan.Filter(_filter);
            foreach (var c in Program.GameMan.GetGameNameGroups())
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
            if ((_filter == GameType.All || _filter == GameType.Windows) && Settings.Default.GamesfolderVisible)
            {
                foreach (var wgame in Program.GameMan.WindowsGames)
                {
                    ListViewItem itm = new ListViewItem();
                    itm.Text = wgame.Key;
                    itm.Group = GamesList.Groups[wgame.Key[0].ToString()];
                    itm.ImageKey = wgame.Key;
                    GamesList.Items.Add(itm);
                }
            }

        }

        private void DoInstall(GameType whattoinstall)
        {
            WaitForInstall db = new WaitForInstall();
            db.EmulatorToInstall = whattoinstall;
            db.Show();
        }

        private void DoInstallAndRun(GameType whattoinstall, string Command, string param = null, bool cmdemu = false)
        {
            try
            {
                WaitForInstall db = new WaitForInstall();
                db.EmulatorToInstall = whattoinstall;
                db.Command = Command;
                db.Arguments = param;
                db.CommandWindowVisible = cmdemu;
                db.Show();
            }
            catch (ObjectDisposedException) { }
        }

        private void StartGame(bool force = false)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            GamesData d = Program.GameMan[selected];
            if (d == null)
            {
                var exepath = (from g in Program.GameMan.WindowsGames where g.Key == selected select g.Value).FirstOrDefault();
                if (exepath != null)
                {
                    if (!Settings.Default.DontwarnDirectDraw) MessageBox.Show("DirectDraw hacking for windows games folder games is not supported", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SystemCommands.RunCommand(exepath);
                }
                return;
            }
            bool test = SystemCommands.IsDosExe(d.GameExePath);
            string args = string.IsNullOrEmpty(d.CommandLinePars) ? null : d.CommandLinePars + " ";
            switch (d.GameType)
            {
                case GameType.DosBox:
                    DoInstallAndRun(GameType.DosBox, Program.FileMan.DosBoxExe, args + "\"" + d.GameExePath + "\"", true);
                    break;
                case GameType.ScummVm:
                    DoInstallAndRun(GameType.ScummVm, Program.FileMan.ScummVmExe, args + d.ScumGameId, true);
                    break;
                case GameType.Snes:
                    DoInstallAndRun(GameType.Snes, Program.FileMan.SnesExe, args + "\"" + d.GameExePath + "\"", false);
                    break;
                case GameType.Windows:
                    if (test)
                    {
                        MessageBox.Show("You are trying to run a DOS program as a Windows program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (d.DirectDraw == UsesDDraw.Unknown && !force)
                    {
                        FirstLaunch fl = new FirstLaunch();
                        fl.Data = d;
                        fl.CallerForm = this;
                        fl.Show();
                    }
                    else LaunchdDraw(d, force);
                    break;
            }
        }

        public void LaunchdDraw(GamesData d, bool force = false)
        {
            string gamedir = Path.GetDirectoryName(d.GameExePath);
            if ((!File.Exists(gamedir + "\\ddhack.cfg") && d.DirectDraw == UsesDDraw.True) || Settings.Default.ForceDirectDraw || force)
            {
                _idtowatch = SystemCommands.RunCommand(d.GameExePath, d.CommandLinePars);
                Thread.Sleep(1000);
                ProcessWatchTimer.Enabled = true;
                SystemCommands.KillExplorer();
            }
            else SystemCommands.RunCommand(d.GameExePath, d.CommandLinePars);
        }

        public void RunDosExe(string filename)
        {
            bool test = SystemCommands.IsDosExe(filename);
            if (!test)
            {
                MessageBox.Show("The file you droped is not a Dos executable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DoInstallAndRun(GameType.DosBox, Program.FileMan.DosBoxExe, "\"" + filename + "\"", true);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            var width = Math.Min(Screen.FromControl(this).WorkingArea.Width, Settings.Default.WindowSize.Width);
            var height = Math.Min(Screen.FromControl(this).WorkingArea.Height, Settings.Default.WindowSize.Height);
            this.Left = Settings.Default.Location.X < 0 || Settings.Default.Location.X > Screen.FromControl(this).WorkingArea.Width ? 0 : Settings.Default.Location.X;
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
            showGamesFolderContentsToolStripMenuItem.Checked = Settings.Default.GamesfolderVisible;

            Program.GameMan = new GamesManager();
            Program.FileMan = new FileManager();
            Program.GameMan.LoadDataFile(Program.FileMan.ConfigLocation);
            BuildList();
            if (Settings.Default.DropVisible) _dosdop.Show();
            steamToolStripMenuItem.Visible = Steam.IsSteamInstalled();
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
            Program.GameMan.SaveDataFile(Program.FileMan.ConfigLocation);
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
                    _filter = GameType.All;
                    break;
                case 1:
                    _filter = GameType.Windows;
                    break;
                case 2:
                    _filter = GameType.DosBox;
                    break;
                case 3:
                    _filter = GameType.ScummVm;
                    break;
                case 4:
                    _filter = GameType.Snes;
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
                Program.GameMan.AddGame(agf.GameName, agf.GamePath, agf.SelectedGameType, null, agf.Arguments);
                BuildList();
            }
        }

        private void addScumVmGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddScumGameForm asgf = new AddScumGameForm();
            if (asgf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Program.GameMan.AddGame(asgf.GameName, asgf.GamePath, GameType.ScummVm, asgf.GameId);
                Program.FileMan.AddScummGame(asgf.GameId, asgf.GameName, asgf.GamePath);
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
                startGameWithDirectDrawHackingToolStripMenuItem.Enabled = false;
            }
            else
            {
                var selected = GamesList.SelectedItems[0].Text;
                internetToolStripMenuItem.Enabled = true;
                openFolderToolStripMenuItem.Enabled = true;
                startGameToolStripMenuItem.Enabled = true;
                directDrawHackToolStripMenuItem.Enabled = false;
                EditToolStripMenuItem.Enabled = false;
                deleteGameToolStripMenuItem.Enabled = false;
                gameExePropertiesToolStripMenuItem.Enabled = false;
                startGameWithDirectDrawHackingToolStripMenuItem.Enabled = false;
                gameSettingsToolStripMenuItem.Enabled = SystemCommands.SetupExists(Program.GameMan.GetPathByName(selected));
                gameExePropertiesToolStripMenuItem.Enabled = IsWinGame(selected);
                if (Program.GameMan[selected] != null)
                {
                    directDrawHackToolStripMenuItem.Enabled = IsWinGame(selected);
                    deleteGameToolStripMenuItem.Enabled = IsWinGame(selected);
                    EditToolStripMenuItem.Enabled = IsWinGame(selected);
                    startGameWithDirectDrawHackingToolStripMenuItem.Enabled = !Settings.Default.ForceDirectDraw && IsWinGame(selected);
                }
            }
        }

        bool IsWinGame(string selected)
        {
            if (Program.GameMan[selected] == null) return true;
            return Program.GameMan[selected].GameType == GameType.Windows;
        }

        private void startGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            string path = Path.GetDirectoryName(Program.GameMan.GetPathByName(selected));
            if (string.IsNullOrEmpty(path)) path = Path.GetDirectoryName(Program.GameMan.WindowsGames[selected]);
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

            var dat = Program.GameMan[selected];
            int index = Program.GameMan.IndexOf(dat);

            switch (dat.GameType)
            {
                case GameType.DosBox:
                case GameType.Windows:
                case GameType.Snes:
                    AddGameForm ed = new AddGameForm();
                    ed.GameName = dat.GameName;
                    ed.GamePath = dat.GameExePath;
                    ed.Arguments = dat.CommandLinePars;
                    ed.SelectedGameType = dat.GameType;
                    if (ed.ShowDialog() == DialogResult.OK)
                    {
                        dat.GameName = ed.GameName;
                        dat.GameExePath = ed.GamePath;
                        dat.CommandLinePars = ed.Arguments;
                        dat.GameType = ed.SelectedGameType;
                        Program.GameMan[index] = dat;
                    }
                    break;
                case GameType.ScummVm:
                    AddScumGameForm esg = new AddScumGameForm();
                    esg.GameName = dat.GameName;
                    esg.GamePath = dat.GameExePath;
                    esg.GameId = dat.ScumGameId;
                    if (esg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        dat.GameName = esg.GameName;
                        dat.GameExePath = esg.GamePath;
                        dat.ScumGameId = esg.GameId;
                        Program.GameMan[index] = dat;
                    }
                    break;
            }
            Program.GameMan.RebuildIconIndex();
            BuildList();
        }

        private void deleteGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            var confirm = MessageBox.Show("Delete " + selected + "?", "Confirm action", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (confirm == System.Windows.Forms.DialogResult.Yes)
            {
                if (Program.GameMan[selected].GameType == GameType.ScummVm) Program.FileMan.RemoveScummGame(Program.GameMan[selected].ScumGameId);
                Program.GameMan.RemoveByName(selected);
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
                    DoInstallAndRun(GameType.DosBox, Program.FileMan.DosBoxPath + "\\DOSBox 0.74 Options.bat", null, false);
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\DOSBox 0.74 Options.bat");
                    break;
                case "openScreenshotsRecordingsToolStripMenuItem":
                    DoInstallAndRun(GameType.DosBox, Program.FileMan.DosBoxPath + "\\Screenshots & Recordings.bat", null, false);
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\Screenshots & Recordings.bat");
                    break;
                case "resetConfigurationToolStripMenuItem":
                    DoInstallAndRun(GameType.DosBox, Program.FileMan.DosBoxPath + "\\Reset Options.bat", null, false);
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\Reset Options.bat");
                    break;
                case "resetKeyMappingsToolStripMenuItem":
                    DoInstallAndRun(GameType.DosBox, Program.FileMan.DosBoxPath + "\\Reset KeyMapper.bat", null, false);
                    //SystemCommands.RunCommand(Program._fileman.DosBoxPath + "\\Reset KeyMapper.bat");
                    break;
            }
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
                case "installReinstallSnes9xToolStripMenuItem":
                    toinstall = "Snes9x";
                    break;
            }
            var res = MessageBox.Show(string.Format("Install/Reinstall {0}?", toinstall), string.Format("{0} Installer", toinstall), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                switch (s.Name)
                {
                    case "installReinstallDosBoxToolStripMenuItem":
                        if (Program.FileMan.IsEmulatorInstalled(GameType.DosBox)) Program.FileMan.DeleteEmulator(GameType.DosBox);
                        DoInstall(GameType.DosBox);
                        break;
                    case "installReinstallScummVmToolStripMenuItem":
                        if (Program.FileMan.IsEmulatorInstalled(GameType.ScummVm)) Program.FileMan.DeleteEmulator(GameType.ScummVm);
                        DoInstall(GameType.ScummVm);
                        break;
                    case "installReinstallSnes9xToolStripMenuItem":
                        if (Program.FileMan.IsEmulatorInstalled(GameType.Snes)) Program.FileMan.DeleteEmulator(GameType.Snes);
                        DoInstall(GameType.Snes);
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
                case "uninstallSnes9xToolStripMenuItem":
                    toinstall = "Snes9x";
                    break;
            }
            var res = MessageBox.Show(string.Format("Uninstall {0}?", toinstall), string.Format("{0} Installer", toinstall), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                switch (s.Name)
                {
                    case "uninstallDosBoxToolStripMenuItem":
                        if (Program.FileMan.DeleteEmulator(GameType.DosBox)) MessageBox.Show("DosBox Uninstalled", "DosBox Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "uninstallScummVmToolStripMenuItem":
                        if (Program.FileMan.DeleteEmulator(GameType.ScummVm)) MessageBox.Show("ScummVM Uninstalled", "ScummVM Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "uninstallSnes9xToolStripMenuItem":
                        if (Program.FileMan.DeleteEmulator(GameType.Snes)) MessageBox.Show("Snes9x Uninstalled", "ScummVM Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    _hb.LoadDocument("Old games launcher/Release notes.rtf");
                    break;
                case "licenseToolStripMenuItem":
                    _hb.LoadDocument("Old games launcher/License.rtf");
                    break;
                case "dosBoxReadmeToolStripMenuItem":
                    _hb.LoadDocument("Emulator related/DOSBox Manual.txt");
                    break;
                case "dosBoxKeysToolStripMenuItem":
                    _hb.LoadDocument("Emulator related/Dosbox keys.txt");
                    break;
                case "scummVmReadmeToolStripMenuItem":
                    _hb.LoadDocument("Emulator related/ScummVm Readme.txt");
                    break;
                case "snes9xReadmeToolStripMenuItem":
                    _hb.LoadDocument("Emulator related/SNES9x Readme.txt");
                    break;
                case "compatiblitySettingsToolStripMenuItem":
                    _hb.LoadDocument("Old games launcher/Changelog.rtf");
                    break;
            }
            _hb.Show();
        }

        private void gameExePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            string path = Program.GameMan.GetPathByName(selected);
            if (string.IsNullOrEmpty(path)) path = Program.GameMan.WindowsGames[selected];
            SystemCommands.ShowFileProperties(path);
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            var res = MessageBox.Show("Install Direct Draw hack to " + selected + "?", "DirectDraw Hack Installer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                string directory = Path.GetDirectoryName(Program.GameMan.GetPathByName(selected));
                Program.FileMan.InstallDirectDrawHack(directory);
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamesList.SelectedItems.Count < 1) return;
            var selected = GamesList.SelectedItems[0].Text;
            var res = MessageBox.Show("Uninstall Direct Draw hack from " + selected + "?", "DirectDraw Hack Installer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                string directory = Path.GetDirectoryName(Program.GameMan.GetPathByName(selected));
                if (Program.FileMan.DeleteDirectDrawHack(directory))
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
            string setup = SystemCommands.GetSetup(Program.GameMan.GetPathByName(selected));
            if (string.IsNullOrEmpty(setup)) return;
            RunDosExe(setup);
        }

        private void groupsVisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.GroupsVisible = groupsVisibleToolStripMenuItem.Checked;
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
            switch (s.Name)
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
                case "getSNESRomsToolStripMenuItem":
                    SystemCommands.OpenWebLocation("http://www.rom-world.com/dl.php?name=Super_Nintendo");
                    break;
            }
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Warning! This will delete all games in collection and can't be undone.\r\n" +
                                          "Are you sure you want to do this?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == System.Windows.Forms.DialogResult.Yes)
            {
                Program.GameMan.Clear();
                BuildList();
            }
        }

        private void StartEmulator(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            switch (s.Name)
            {
                case "startDosBoxToolSatripMenuItem":
                    DoInstallAndRun(GameType.DosBox, Program.FileMan.DosBoxExe, null, true);
                    break;
                case "startScummVMToolStripMenuItem":
                    DoInstallAndRun(GameType.ScummVm, Program.FileMan.ScummVmExe, null, true);
                    break;
                case "startSnes9xToolStripMenuItem":
                    DoInstallAndRun(GameType.Snes, Program.FileMan.SnesExe, null, false);
                    break;
            }

        }

        private void SteamMenu(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            switch (s.Name)
            {
                case "storeToolStripMenuItem":
                    Steam.DoSteamAction(Steam.SteamAction.OpenStore);
                    break;
                case "gamesToolStripMenuItem1":
                    Steam.DoSteamAction(Steam.SteamAction.OpenGames);
                    break;
                case "downloadsToolStripMenuItem":
                    Steam.DoSteamAction(Steam.SteamAction.OpenDownloads);
                    break;
                case "friendsToolStripMenuItem":
                    Steam.DoSteamAction(Steam.SteamAction.OpenFriendsList);
                    break;
                case "newsToolStripMenuItem":
                    Steam.DoSteamAction(Steam.SteamAction.OpenNews);
                    break;
                case "settingsToolStripMenuItem":
                    Steam.DoSteamAction(Steam.SteamAction.OpenSettings);
                    break;
            }
        }

        private void showGamesFolderContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.GamesfolderVisible = showGamesFolderContentsToolStripMenuItem.Checked;
            BuildList();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsForm ofm = new OptionsForm();
            if (ofm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                closeToTrayToolStripMenuItem.Checked = Settings.Default.CloseToTray;
                showEmulatorsConsoleToolStripMenuItem.Checked = Settings.Default.EmuConsoleVisible;
                showGamesFolderContentsToolStripMenuItem.Checked = Settings.Default.GamesfolderVisible;
                BuildList();
            }
        }

        private void startGameWithDirectDrawHackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartGame(true);
        }
    }
}
