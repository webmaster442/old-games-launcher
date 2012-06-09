using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OldGamesLauncher
{
    [Serializable]
    public class GamesData
    {
        public string GameName { get; set; }
        public string GameExePath { get; set; }
        public bool isDosboxGame { get; set; }
        public string ScumGameId { get; set; }

        public GamesData() { }

        public GamesData(string Name, string Exepath)
        {
            GameName = Name;
            GameExePath = Exepath;
        }

        public override int GetHashCode()
        {
            return GameName.GetHashCode() ^ GameExePath.GetHashCode() ^ isDosboxGame.GetHashCode();
        }

        public override string ToString()
        {
            return GameName;
        }

        public bool isScumGame()
        {
            return !string.IsNullOrEmpty(ScumGameId);
        }
    }

    /// <summary>
    /// Games manager class
    /// </summary>
    internal class GamesManager : IEnumerable<GamesData>, ICollection<GamesData>
    {
        private List<GamesData> _games;
        private List<string> _gamestartletters;
        private ImageList _images;

        public enum GameType
        {
            All, Windows, Dos, Scum
        }

        public GamesManager()
        {
            _games = new List<GamesData>();
            _gamestartletters = new List<string>();
            _images = new ImageList();
            _images.ImageSize = new System.Drawing.Size(32, 32);
        }

        /// <summary>
        /// Rebuilds Icon index of the games
        /// </summary>
        public void RebuildIconIndex()
        {
            if (_images != null)
            {
                _images.Dispose();
                _images = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            _images = new ImageList();
            _images.ImageSize = new System.Drawing.Size(32, 32);
            foreach (var e in _games)
            {
                if (e.isDosboxGame) _images.Images.Add(e.GameName, Properties.Resources.dosicon);
                else if (e.isScumGame()) _images.Images.Add(e.GameName, Properties.Resources.scumicon);
                else _images.Images.Add(e.GameName, SystemCommands.GetIconOfExe(e.GameExePath));
                 
            }
        }

        /// <summary>
        /// Loads a games data file
        /// </summary>
        /// <param name="str">Games datafile to be laoded</param>
        public void LoadDataFile(string str)
        {
            XmlSerializer xs = new XmlSerializer(typeof(GamesData[]));
            if (!File.Exists(str)) return;
            try
            {
                GamesData[] readbuff;
                using (FileStream fi = File.OpenRead(str))
                {
                    readbuff = (GamesData[])xs.Deserialize(fi);
                    _games.Clear();
                    _games.AddRange(readbuff);
                    RebuildIconIndex();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the games data file
        /// </summary>
        /// <param name="dest">target to write to</param>
        public void SaveDataFile(string dest)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(GamesData[]));
                string newdest = dest + ".new";
                if (File.Exists(newdest)) File.Delete(newdest);
                FileStream target = File.OpenWrite(newdest);
                xs.Serialize(target, _games.ToArray());
                target.Close();
                if (File.Exists(dest)) File.Delete(dest);
                File.Move(newdest, dest);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public IEnumerator<GamesData> GetEnumerator()
        {
            return _games.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _games.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets a Game data by it's index in the database
        /// </summary>
        /// <param name="i">Game index</param>
        /// <returns>A Game data</returns>
        public GamesData this[int i]
        {
            get { return _games[i]; }
            set { _games[i] = value; }
        }

        /// <summary>
        /// Images of the games
        /// </summary>
        public ImageList Icons
        {
            get { return _images; }
        }

        /// <summary>
        /// returns Game group headers
        /// </summary>
        /// <returns></returns>
        public char[] GetGameNameGroups()
        {
            var query = from g in _games group g by g.GameName[0] into List select List.Key;
            return query.ToArray();
        }

        /// <summary>
        /// Adds a game to the collection
        /// </summary>
        /// <param name="Name">Game name</param>
        /// <param name="ExePath">Game exe path</param>
        public void AddGame(string Name, string ExePath, bool isDos, string scumid = null)
        {
            GamesData d = new GamesData();
            d.GameName = Name;
            d.GameExePath = ExePath;
            d.isDosboxGame = isDos;
            d.ScumGameId = scumid;
            _games.Add(d);
            RebuildIconIndex();
        }

        /// <summary>
        /// Returns a game exe path by game name
        /// </summary>
        /// <param name="Name">Name to search for</param>
        /// <returns>the game's exe path</returns>
        public string GetPathByName(string Name)
        {
            var q = from i in _games where i.GameName == Name select i.GameExePath;
            return q.FirstOrDefault();
        }

        /// <summary>
        /// Returns game data by the game name
        /// </summary>
        /// <param name="Name">Name to searh for</param>
        /// <returns>Associated gamedata to neame</returns>
        public GamesData GetGameDataByName(string Name)
        {
            var q = from i in _games where i.GameName == Name select i;
            return q.FirstOrDefault();
        }

        public int IndexOf(GamesData d)
        {
            return _games.IndexOf(d);
        }

        public void RemoveByName(string name)
        {
            foreach (var data in _games)
            {
                if (data.GameName == name)
                {
                    _games.Remove(data);
                    RebuildIconIndex();
                    break;
                }
            }
        }

        public List<GamesData> Filter(GameType Filter)
        {
            switch (Filter)
            {
                case GameType.All:
                    return (from l in _games orderby l.GameName select l).ToList();
                case GameType.Dos:
                    return (from i in _games where i.isDosboxGame == true orderby i.GameName select i).ToList();
                case GameType.Scum:
                    return (from j in _games where j.isScumGame() == true orderby j.GameName select j).ToList();
                case GameType.Windows:
                    return (from k in _games where k.isScumGame() == false && k.isDosboxGame == false orderby k.GameName select k).ToList();
                default:
                    return null;
            }
        }

        public void Add(GamesData item)
        {
            _games.Add(item);
            RebuildIconIndex();
        }

        public void Clear()
        {
            _games.Clear();
            RebuildIconIndex();
        }

        public bool Contains(GamesData item)
        {
            return _games.Contains(item);
        }

        public void CopyTo(GamesData[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _games.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(GamesData item)
        {
            return _games.Remove(item);
        }
    }
}
