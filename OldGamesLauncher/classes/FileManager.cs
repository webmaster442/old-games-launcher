using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace OldGamesLauncher
{
    internal class FileManager
    {
        private string _profileDir;
        private string _storageroot;
        private string _appdir;
        private string _appdataloc;
        private IniParser _iniparser;

        public FileManager()
        {
            _profileDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(_profileDir + "oldgameslauncher")) Directory.CreateDirectory(_profileDir + "\\oldgameslauncher");
            _storageroot = _profileDir + "\\oldgameslauncher\\";
            _appdir = Path.GetDirectoryName(Application.ExecutablePath);
            _appdataloc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _iniparser = new IniParser(ScummAppData + "scummvm.ini");
        }

        /// <summary>
        /// Old Game Launcher main game database
        /// </summary>
        public string ConfigLocation
        {
            get { return _storageroot + "oldgamesstarter.xml"; }
        }

        /// <summary>
        /// Scum VM Application data folder
        /// </summary>
        public string ScummAppData
        {
            get { return _appdataloc + "\\scummvm\\"; }
        }

        /// <summary>
        /// DosBox exe path
        /// </summary>
        public string DosBoxExe
        {
            get { return _storageroot + "dosbox\\dosbox.exe"; }
        }

        /// <summary>
        /// ScummVm Exe path
        /// </summary>
        public string ScummVmExe
        {
            get { return _storageroot + "scummvm\\scummvm.exe"; }
        }

        /// <summary>
        /// Snes9x exe path
        /// </summary>
        public string SnesExe
        {
            get { return _storageroot + "snes9x\\snes9x.exe"; }
        }

        /// <summary>
        /// ScummVM path
        /// </summary>
        public string ScummVmPath
        {
            get { return _storageroot + "scummvm"; }
        }

        /// <summary>
        /// DosBox path
        /// </summary>
        public string DosBoxPath
        {
            get { return _storageroot + "dosbox"; }
        }

        public string Snes9xPath
        {
            get { return _storageroot + "snes9x"; }
        }

        public bool IsEmulatorInstalled(GameType type)
        {
            switch (type)
            {
                case GameType.DosBox:
                    if (Directory.Exists(DosBoxPath)) return File.Exists(DosBoxExe);
                    else return false;
                case GameType.ScummVm:
                    if (Directory.Exists(ScummVmPath)) return File.Exists(ScummVmExe);
                    else return false;
                case GameType.Snes:
                    if (Directory.Exists(Snes9xPath)) return File.Exists(SnesExe);
                    else return false;
                default:
                    return false;
            }
        }

        private static string CreateFilenameFromUri(Uri uri)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            StringBuilder sb = new StringBuilder(uri.OriginalString.Length);
            foreach (char c in uri.OriginalString)
            {
                sb.Append(Array.IndexOf(invalidChars, c) < 0 ? c : '_');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Installes DirectDrawHack to a directory
        /// </summary>
        /// <param name="targetdir">The directory, where DirectDrawHack sould be installed</param>
        public void InstallDirectDrawHack(string targetdir)
        {
            try
            {
                FileStream ms = File.OpenRead(_appdir + "\\Zips\\ddhack.zip");
                string target;
                using (ZipInputStream zi = new ZipInputStream(ms))
                {
                    ZipEntry file;
                    while ((file = zi.GetNextEntry()) != null)
                    {
                        target = Path.Combine(targetdir, file.Name.Replace('/', '\\'));

                        if (file.IsDirectory) Directory.CreateDirectory(target);
                        else
                        {
                            using (FileStream fs = File.Create(target))
                            {
                                int size;
                                byte[] data = new byte[2048];
                                while ((size = zi.Read(data, 0, data.Length)) > 0)
                                {
                                    fs.Write(data, 0, size);
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("Direct Draw Hack Installed", "Direct Draw Hack Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Direct Draw Hack Install failed.\r\n" + ex.Message, "Direct Draw Hack Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Deletes DirectDrawHack files from a directory
        /// </summary>
        /// <param name="target">The directory containing DirectDrawHack</param>
        /// <returns>true, if delete was succesfull, false, if an error occured</returns>
        public bool DeleteDirectDrawHack(string target)
        {
            string[] files = new string[] { "\\ddhack.cfg", "\\ddhack.txt", "\\ddraw.dll" };
            try
            {
                foreach (var f in files)
                {
                    if (File.Exists(target + f)) File.Delete(target + f);
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public void InstallEmulator(GameType type)
        {
            FileStream ms = null;
            string basedir = null;
            string target = null;
            try
            {
                switch (type)
                {
                    case GameType.DosBox:
                        if (!Directory.Exists(this.DosBoxPath)) Directory.CreateDirectory(this.DosBoxPath);
                        ms = File.OpenRead(_appdir + "\\Zips\\dosbox.zip");
                        basedir = DosBoxPath;
                        break;
                    case GameType.ScummVm:
                        if (!Directory.Exists(this.ScummVmPath)) Directory.CreateDirectory(this.ScummVmPath);
                        ms = File.OpenRead(_appdir + "\\Zips\\scummvm.zip");
                        basedir = ScummVmPath;
                        break;
                    case GameType.Snes:
                        if (!Directory.Exists(this.Snes9xPath)) Directory.CreateDirectory(this.Snes9xPath);
                        ms = File.OpenRead(_appdir + "\\Zips\\snes9x.zip");
                        basedir = Snes9xPath;
                        break;
                }
                using (ZipInputStream zi = new ZipInputStream(ms))
                {
                    ZipEntry file;
                    while ((file = zi.GetNextEntry()) != null)
                    {
                        target = Path.Combine(basedir, file.Name.Replace('/', '\\'));

                        if (file.IsDirectory) Directory.CreateDirectory(target);
                        else
                        {
                            using (FileStream fs = File.Create(target))
                            {
                                int size;
                                byte[] data = new byte[2048];
                                while ((size = zi.Read(data, 0, data.Length)) > 0)
                                {
                                    fs.Write(data, 0, size);
                                }
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(string.Format("{0} Install failed.\r\n", type) + ex.Message, "Emulator Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Uninstalls DosBox
        /// </summary>
        /// <returns>true, if uninstall was succesfull, false, if not</returns>
        public bool DeleteEmulator(GameType type)
        {
            string workdir = null;
            if (!IsEmulatorInstalled(type)) return true;
            switch (type)
            {
                case GameType.DosBox:
                    workdir = DosBoxPath;
                    break;
                case GameType.ScummVm:
                    workdir = ScummVmPath;
                    break;
                case GameType.Snes:
                    workdir = Snes9xPath;
                    break;
            }
            try
            {
                string[] Files = Directory.GetFiles(workdir, "*.*", SearchOption.AllDirectories);
                foreach (var file in Files) File.Delete(file);

                string[] dirs = Directory.GetDirectories(workdir, "*.*", SearchOption.AllDirectories);
                foreach (var dir in dirs) Directory.Delete(dir);

                Directory.Delete(workdir);
                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Uninstall failed. Reason: " + ex.Message, "Uninstall error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Gets a document file as a memorystream
        /// </summary>
        /// <param name="fname">File to get inside docs.zip</param>
        /// <returns>The file as a memorystream</returns>
        public MemoryStream GetDocumentContent(string fname)
        {
            try
            {
                string docs = _appdir + "\\Zips\\docs.zip";
                MemoryStream outp = new MemoryStream();
                FileStream fs = File.OpenRead(docs);
                using (ZipInputStream zi = new ZipInputStream(fs))
                {
                    ZipEntry file;
                    while ((file = zi.GetNextEntry()) != null)
                    {
                        if (file.Name == fname)
                        {
                            int size;
                            byte[] data = new byte[2048];
                            while ((size = zi.Read(data, 0, data.Length)) > 0) outp.Write(data, 0, size);
                            outp.Seek(0, SeekOrigin.Begin);
                            return outp;
                        }
                    }
                }
                return null;
            }
            catch (IOException)
            {
                MessageBox.Show("Can't Get documentation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Registers a Scum
        /// </summary>
        /// <param name="GameId">Scumm Game ID</param>
        /// <param name="description">Game Name (description)</param>
        /// <param name="path">Game Path</param>
        public void AddScummGame(string GameId, string description, string path)
        {
            _iniparser.Load();
            _iniparser[GameId, "gameid"] = GameId;
            _iniparser[GameId, "description"] = description;
            _iniparser[GameId, "path"] = path;
            _iniparser.SaveSettings();
        }

        /// <summary>
        /// Deletes a SummVm Game
        /// </summary>
        /// <param name="GameId">Scumm Game Id</param>
        public void RemoveScummGame(string GameId)
        {
            _iniparser.Load();
            _iniparser.DeleteSection(GameId);
            _iniparser.SaveSettings();
        }
    }
}