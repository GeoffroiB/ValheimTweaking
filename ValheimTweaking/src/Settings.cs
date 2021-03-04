using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using Unity;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Runtime;
using IniParser;
using IniParser.Model;
using HarmonyLib;
using System.Globalization;
using Steamworks;
using System.Net;

// Todo, better error handling

namespace ValheimTweaking
{
    class Settings
    {
        public static IniData Config { get; set; }

        private static string ConfigPath = Path.GetDirectoryName(Paths.BepInExConfigPath) + "/tweaking.ini";

        public static string getPath() {
            return ConfigPath;
        }

        public static bool exists() {
            return File.Exists(ConfigPath);
        }

        public static bool loadSettings() {
            if (!Settings.exists()) {
                return false;
            }
            Config = (new FileIniDataParser()).ReadFile(ConfigPath);
            return true;
        }

        public static bool isEnabled(string section) {
            return Boolean.Parse(Config[section]["enabled"]);
        }

        public static bool getBool(string section, string name) {
            return Config[section][name].ToLower() == "true";
        }

        public static string getString(string section, string name) {
            return Config[section][name];
        }

        public static int getInt(string section, string name) {
            return int.Parse(Config[section][name]);
        }

        public static float getFloat(string section, string name) {
            return float.Parse(Config[section][name], CultureInfo.InvariantCulture.NumberFormat);
        }

        public static KeyCode getHotkey(string name) {
            try {
                return (KeyCode)System.Enum.Parse(typeof(KeyCode), Config["Hotkeys"][name]);
            } catch (Exception e) {
                return KeyCode.None;
            }
        }

        //public static Boolean isNewVersionAvailable(string version) {
        //    WebClient client = new WebClient();
        //    client.Headers.Add("User-Agent: V+ Server");

        //    string reply = client.DownloadString(ValheimTweakingPlugin.ApiRepository);

        //    string newestVersion = reply.Split(new[] { "," }, StringSplitOptions.None)[0].Trim().Replace("\"", "").Replace("[{name:", "");
        //    try {
        //        if (newestVersion != version) {
        //            return true;
        //        }
        //    } catch (Exception e) {
        //        Debug.Log("The newest version could not be determined.");
        //        return false;
        //    }

        //    return false;
        //}

        public class GithubTagInfo
        {
            public string name { get; set; }
            public DateTimeOffset Date { get; set; }
            public string zipball_url { get; set; }
            public string tarball_url { get; set; }
            public Dictionary<string, commitHashes> Hashes { get; set; }
            public string node_id { get; set; }
        }

        public class commitHashes
        {
            public int sha { get; set; }
            public int url { get; set; }
        }
    }
}
