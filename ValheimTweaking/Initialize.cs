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


namespace ValheimTweaking
{
    [BepInPlugin("org.bepinex.plugins.valheim_tweaking", "Valheim Tweaking", "0.1")]
    class ValheimTweaking : BaseUnityPlugin
    {
        public static Boolean isDebug = false;

        private static ValheimTweaking m_instance;
        public static ValheimTweaking instance => ValheimTweaking.m_instance;

        // Awake is called once when both the game and the plug-in are loaded
        void Awake() {
            ValheimTweaking.m_instance = this;

            Logger.LogInfo("Trying to load the configuration file \"" + Settings.getPath() + "\"");
            if (!Settings.exists()) {
                Logger.LogError("Error: File not found. Plugin not loaded.");
                return;
            }

            Logger.LogInfo("Configuration file found, loading configuration...");
            if (Settings.loadSettings() != true) {
                Logger.LogError("Error while loading configuration file");
                Logger.LogWarning("Please make sure \"" + Settings.getPath() + "\" was properly placed.");
                return;
            }
            
            Logger.LogInfo("Configuration file loaded succesfully.");

            if (Settings.isEnabled("Server")) {
                ServerPatches.patch();
            }
        }
        
        public static void LogDebug(object data) {
            ValheimTweaking.m_instance.Logger.LogDebug(data);
        }

        public static void LogError(object data) {
            ValheimTweaking.m_instance.Logger.LogError(data);
        }

        public static void LogFatal(object data) {
            ValheimTweaking.m_instance.Logger.LogFatal(data);
        }

        public static void LogInfo(object data) {
            ValheimTweaking.m_instance.Logger.LogInfo(data);
        }

        public static void LogMessage(object data) {
            ValheimTweaking.m_instance.Logger.LogMessage(data);
        }

        public static void LogWarning(object data) {
            ValheimTweaking.m_instance.Logger.LogWarning(data);
        }
    }
}
