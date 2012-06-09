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
        /// ScumVm Exe path
        /// </summary>
        public string ScummVmExe
        {
            get { return _storageroot + "scummvm\\scummvm.exe"; }
        }

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

        /// <summary>
        /// Checks if dosbox is installed or not
        /// </summary>
        /// <returns>true, if installed, false, if not</returns>
        public bool IsDosboxInstalled()
        {
            if (Directory.Exists(_storageroot + "dosbox"))
            {
                if (File.Exists(_storageroot + "dosbox\\dosbox.exe")) return true;
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Checks if ScummVm is installed or not
        /// </summary>
        /// <returns>true, if installed, false, if not</returns>
        public bool IsScummVmInstalled()
        {
            if (Directory.Exists(_storageroot + "scummvm"))
            {
                if (File.Exists(_storageroot + "scummvm\\scummvm.exe")) return true;
                else return false;
            }
            else return false;
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

        /// <summary>
        /// Installs dosbox
        /// </summary>
        public void InstallDosDox()
        {
            try
            {
                if (!Directory.Exists(this.DosBoxPath)) Directory.CreateDirectory(this.DosBoxPath);
                FileStream ms = File.OpenRead(_appdir + "\\Zips\\dosbox.zip");
                string basedir = _storageroot + "dosbox";
                string target;
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
                MessageBox.Show("DosBox Install failed.\r\n" + ex.Message, "DosBox Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void InstallScummVm()
        {
            try
            {
                if (!Directory.Exists(this.ScummVmPath)) Directory.CreateDirectory(this.ScummVmPath);
                FileStream ms = File.OpenRead(_appdir + "\\Zips\\scummvm.zip");
                string basedir = _storageroot + "scummvm";
                string target;
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
                MessageBox.Show("ScummVm Install failed.\r\n" + ex.Message, "ScummVm Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Uninstalls DosBox
        /// </summary>
        /// <returns>true, if uninstall was succesfull, false, if not</returns>
        public bool DeleteDosBox()
        {
            if (!IsDosboxInstalled()) return true;
            try
            {
                string[] Files = Directory.GetFiles(_storageroot + "dosbox", "*.*", SearchOption.AllDirectories);
                foreach (var file in Files)
                {
                    File.Delete(file);
                }
                string[] dirs = Directory.GetDirectories(_storageroot + "dosbox", "*.*", SearchOption.AllDirectories);
                foreach (var dir in dirs)
                {
                    Directory.Delete(dir);
                }
                Directory.Delete(_storageroot + "dosbox");
                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Uninstall failed. Reason: " + ex.Message, "Uninstall error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Deletes ScumVm
        /// </summary>
        /// <returns>true, if uninstall was succesfull, false, if not</returns>
        public bool DeleteScummVm()
        {
            if (!IsScummVmInstalled()) return true;
            try
            {
                string[] Files = Directory.GetFiles(_storageroot + "scummvm", "*.*", SearchOption.AllDirectories);
                foreach (var file in Files)
                {
                    File.Delete(file);
                }
                string[] dirs = Directory.GetDirectories(_storageroot + "scummvm", "*.*", SearchOption.AllDirectories);
                foreach (var dir in dirs)
                {
                    Directory.Delete(dir);
                }
                Directory.Delete(_storageroot + "scummvm");
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

        public void AddScumGame(string GameId, string description, string path)
        {
            _iniparser.Load();
            _iniparser[GameId, "gameid"] = GameId;
            _iniparser[GameId, "description"] = description;
            _iniparser[GameId, "path"] = path;
            _iniparser.SaveSettings();
        }

        public void RemoveScumGame(string GameId)
        {
            _iniparser.Load();
            _iniparser.DeleteSection(GameId);
            _iniparser.SaveSettings();
        }
    }
}