using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace VersionHandling
{
    public static class VersionHandler
    {
        public static void CreateVersionXml(string path, PackVersion v)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PackVersion));
            FileStream target = File.Create(path);
            xs.Serialize(target, v);
            target.Close();
        }

        public static bool IsNewerVersion(string OldVersionXML, Stream NewVersionXML)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PackVersion));
            if (!File.Exists(OldVersionXML)) return false;
            FileStream old = File.OpenRead(OldVersionXML);
            PackVersion _old, _new;
            _old = (PackVersion)xs.Deserialize(old);
            _new = (PackVersion)xs.Deserialize(NewVersionXML);
            old.Close();
            return _new > _old;
        }
    }
}
