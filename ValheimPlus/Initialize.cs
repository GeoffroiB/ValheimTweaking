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
    class ValheimTweakingPlugin : BaseUnityPlugin
    {
        // DO NOT REMOVE MY CREDITS
        //public static string Author = "Kevin 'nx' J.";
        //public static string Website = "http://n-x.xyz";
        //public static string Discord = "nx#8830";
        //public static string Repository = "https://github.com/nxPublic/ValheimPlus";
        //public static string ApiRepository = "https://api.github.com/repos/nxPublic/valheimPlus/tags";

        public static Boolean isDebug = false;

        private static ValheimTweakingPlugin m_instance;
        public static ValheimTweakingPlugin instance => ValheimTweakingPlugin.m_instance;

        // Awake is called once when both the game and the plug-in are loaded
        void Awake() {
            ValheimTweakingPlugin.m_instance = this;

            Logger.LogInfo("Trying to load the configuration file \"" + Settings.getPath() + "\"");
            if (!Settings.exists()) {
                Logger.LogError("Error: File not found. Plugin not loaded.");
                return;
            }

            Logger.LogInfo("Configuration file found, loading configuration.");
            if (Settings.loadSettings() != true) {
                Logger.LogError("Error while loading configuration file");
                return;
            }
            
            Logger.LogInfo("Configuration file loaded succesfully.");

            if (Settings.isEnabled("Server")) {
                ValheimTweaking.ServerPatches.patch();
            }
            
            // (new Harmony("mod.valheim_tweaking")).PatchAll();

            //if (Settings.isNewVersionAvailable("0.6")) {
            //    Logger.LogError("There is a newer version available of ValheimPlus.");
            //    Logger.LogWarning("Please visit " + ValheimTweakingPlugin.Repository + ".");
            //    return;
            //}
            //Logger.LogInfo("ValheimTweaking is up to date.");
        }

        public void LogInfo(object data) {
            Logger.LogInfo(data);
        }
    }
}
