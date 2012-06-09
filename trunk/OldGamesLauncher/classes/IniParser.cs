using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace OldGamesLauncher
{
    public class IniParser
    {
        private Dictionary<SectionPair, string> keyPairs = new Dictionary<SectionPair, string>();
        private string _iniFilePath;

        private struct SectionPair
        {
            public string Section;
            public string Key;
        }

        public void Load()
        {
            TextReader iniFile = null;
            string strLine = null;
            string currentRoot = null;
            string[] keyPair = null;

            if (File.Exists(_iniFilePath))
            {
                try
                {
                    iniFile = new StreamReader(_iniFilePath);

                    strLine = iniFile.ReadLine();

                    while (strLine != null)
                    {
                        strLine = strLine.Trim().ToUpper();

                        if (strLine != "")
                        {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                            {
                                currentRoot = strLine.Substring(1, strLine.Length - 2);
                            }
                            else if (strLine.StartsWith("'")) { }
                            else
                            {
                                keyPair = strLine.Split(new char[] { '=' }, 2);

                                SectionPair sectionPair;
                                string value = null;

                                if (currentRoot == null) currentRoot = "ROOT";

                                sectionPair.Section = currentRoot;
                                sectionPair.Key = keyPair[0];

                                if (keyPair.Length > 1) value = keyPair[1];

                                keyPairs.Add(sectionPair, value);
                            }
                        }
                        strLine = iniFile.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (iniFile != null) iniFile.Close();
                }
            }
            else File.WriteAllText(_iniFilePath, "");
        }

        /// <summary>
        /// Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name="iniPath">Full path to INI file.</param>
        public IniParser(string iniPath)
        {
            _iniFilePath = iniPath;
            Load();
        }

        /// <summary>
        /// Returns the value for the given section, key pair.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="settingName">Key name.</param>
        public string GetSetting(string sectionName, string settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            return (string)keyPairs[sectionPair];
        }

        public string this[string Section, string Key]
        {
            get { return GetSetting(Section, Key); }
            set { AddSetting(Section, Key, value); }
        }

        /// <summary>
        /// Enumerates all lines for given section.
        /// </summary>
        /// <param name="sectionName">Section to enum.</param>
        public string[] EnumSection(string sectionName)
        {
            List<string> tmpArray = new List<string>();

            foreach (SectionPair pair in keyPairs.Keys)
            {
                if (pair.Section == sectionName.ToUpper()) tmpArray.Add(pair.Key);
            }
            return tmpArray.ToArray();
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        /// <param name="settingValue">Value of key.</param>
        public void AddSetting(string sectionName, string settingName, string settingValue)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            if (keyPairs.ContainsKey(sectionPair))
                keyPairs.Remove(sectionPair);

            keyPairs.Add(sectionPair, settingValue);
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved with a null value.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void AddSetting(string sectionName, string settingName)
        {
            AddSetting(sectionName, settingName, null);
        }

        /// <summary>
        /// Remove a setting.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void DeleteSetting(string sectionName, string settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            if (keyPairs.ContainsKey(sectionPair)) keyPairs.Remove(sectionPair);
        }

        public void DeleteSection(string sectionName)
        {
            foreach (var item in keyPairs.Keys)
            {
                if (item.Section == sectionName.ToUpper()) keyPairs.Remove(item);
            }
        }

        /// <summary>
        /// Save settings to new file.
        /// </summary>
        /// <param name="newFilePath">New file path.</param>
        public void SaveSettings(string newFilePath)
        {
            ArrayList sections = new ArrayList();
            string tmpValue = "";
            string strToSave = "";

            foreach (SectionPair sectionPair in keyPairs.Keys)
            {
                if (!sections.Contains(sectionPair.Section))
                    sections.Add(sectionPair.Section);
            }

            foreach (string section in sections)
            {
                strToSave += ("[" + section + "]\r\n");

                foreach (SectionPair sectionPair in keyPairs.Keys)
                {
                    if (sectionPair.Section == section)
                    {
                        tmpValue = (string)keyPairs[sectionPair];

                        if (tmpValue != null)
                            tmpValue = "=" + tmpValue;

                        strToSave += (sectionPair.Key + tmpValue + "\r\n");
                    }
                }

                strToSave += "\r\n";
            }

            try
            {
                TextWriter tw = new StreamWriter(newFilePath);
                tw.Write(strToSave);
                tw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save settings back to ini file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(_iniFilePath);
        }
    }
}