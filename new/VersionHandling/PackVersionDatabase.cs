using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace VersionHandling
{
    public class PackVersionDatabase
    {
        Dictionary<string, PackVersion> _dic;

        public PackVersionDatabase()
        {
            _dic = new Dictionary<string, PackVersion>();
        }

        public void SaveToFile(string target)
        {
            XmlSerializer xs = new XmlSerializer(typeof(KeyValuePair<string, PackVersion>[]));
            List<KeyValuePair<string, PackVersion>> serial = new List<KeyValuePair<string, PackVersion>>(_dic.Keys.Count);
            foreach (var item in _dic) serial.Add(item);
            FileStream fs = File.Create(target + ".new");
            xs.Serialize(fs, serial.ToArray());
            fs.Close();
            File.Move(target + ".new", target);
        }

        public void LoidFromFile(string Source)
        {
            XmlSerializer xs = new XmlSerializer(typeof(KeyValuePair<string, PackVersion>[]));
            FileStream fs = File.OpenRead(Source);
            KeyValuePair<string, PackVersion>[] des = (KeyValuePair<string, PackVersion>[])xs.Deserialize(fs);
            fs.Close();
            foreach (var item in des) _dic.Add(item.Key, item.Value);
        }

        public void Add(string s, PackVersion ver)
        {
            if (_dic.ContainsKey(s)) _dic[s] = ver;
            else _dic.Add(s, ver);
        }

        public void Remove(string s)
        {
            _dic.Remove(s);
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public PackVersion this[string key]
        {
            get { return _dic[key]; }
            set { _dic[key] = value; }
        }
    }
}
