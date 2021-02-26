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
    public class ServerPatches
    {
        public const string HARMONY_ID = "org.bepinex.plugins.valheim_tweaking.server_patches";

        private static Harmony harmony;
        private static bool m_isServer = false;

        public static void patch() {
            harmony = new Harmony(HARMONY_ID);

            harmony.Patch(
                AccessTools.Method(typeof(ZNet), "Awake"),
                null,
                new HarmonyMethod(typeof(Patch_ZNetAwake).GetMethod("postfix"))
            );

            harmony.Patch(
                AccessTools.Method(typeof(ZNet), "Update"),
                null,
                new HarmonyMethod(typeof(Patch_ZNetAwake).GetMethod("postfix"))
            );
        }

        public static class Patch_ZNetAwake
        {
            public const int DEFAULT_dataPerSec = 61440;
            public const int OVERRIDE_dataPerSec_ratio = 2;

            public static void postfix(ZNet __instance) {
                ServerPatches.m_isServer = __instance.IsServer();

                if (__instance.m_zdoMan.m_dataPerSec == DEFAULT_dataPerSec) {
                    __instance.m_zdoMan.m_dataPerSec *= OVERRIDE_dataPerSec_ratio;
                }

                ValheimTweaking.LogInfo("m_dataPerSec overriden to " + __instance.m_zdoMan.m_dataPerSec);
            }
        }

        public static class Patch_SpamPeers
        {
            public static void postfix() {
                if (ServerPatches.m_isServer) {
                    foreach (ZNetPeer peer in ZNet.instance.m_peers) {
                        ZNet.instance.RemotePrint(peer.m_rpc, "rawr Xd ;)");
                    }
                }
            }
        }
    }
}
