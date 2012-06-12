using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace OldGamesLauncher
{
    public static class Steam
    {
        public enum SteamAction
        {
            OpenDownloads,
            OpenFriendsList,
            OpenFriends,
            OpenGames,
            OpenNews,
            OpenSettings,
            OpenStore
        }


        public static bool IsSteamInstalled()
        {
            try
            {
                RegistryKey reg = Registry.CurrentUser;
                reg = reg.OpenSubKey(@"Software\Valve\Steam", false);
                object val = reg.GetValue("SteamExe", null, RegistryValueOptions.None);
                if (val != null) return true;
                return false;
            }
            catch (Exception) { return false; }
        }

        public static void DoSteamAction(SteamAction Action)
        {
            string cmd = null;
            switch (Action)
            {
                case SteamAction.OpenDownloads:
                    cmd = "steam://open/downloads";
                    break;
                case SteamAction.OpenFriends:
                    cmd = "steam://friends";
                    break;
                case SteamAction.OpenFriendsList:
                    cmd = "steam://open/friends";
                    break;
                case SteamAction.OpenGames:
                    cmd = "steam://open/games";
                    break;
                case SteamAction.OpenNews:
                    cmd = "steam://open/news";
                    break;
                case SteamAction.OpenSettings:
                    cmd = "steam://open/settings";
                    break;
                case SteamAction.OpenStore:
                    cmd = "steam://store";
                    break;
            }
            if (cmd != null) Process.Start(cmd);
        }
    }
}
