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
    // Server side only
    // [HarmonyPatch(typeof(FejdStartup), "IsPublicPasswordValid")]
    public static class ChangeServerPasswordBehavior
    {
        private static void Postfix(ref Boolean __result) { // Set after awake function
            if (!Settings.isEnabled("Server")) {
                return;
            }

            if (Settings.getBool("Server", "disableServerPassword")) {
                __result = true;
            }
        }
    }

    // Server side only
    // [HarmonyPatch(typeof(FejdStartup), "Update")]
    public static class EveryTickLog
    {
        private static void Postfix() { // Set after awake function
            if (!Settings.isEnabled("Server")) {
                return;
            }

            List<ZNet.PlayerInfo> public_players = new List<ZNet.PlayerInfo>();

            if (ZNet.instance == null) {
                return;
            }

            hookZNet.GetOtherPublicPlayers(ZNet.instance, public_players);

            Debug.Log(public_players.Count());

        }
    }

    // Server side only
    // [HarmonyPatch(typeof(SteamGameServer), "SetMaxPlayerCount")]
    public static class ChangeSteamServerMaxPlayerCount
    {
        private static void Prefix(ref int cPlayersMax) {
            if (Settings.isEnabled("Server")) {
                int maxPlayers = Settings.getInt("Server", "maxPlayers");
                if (maxPlayers >= 1) {
                    cPlayersMax = maxPlayers;
                }
            }
        }
    }

    // Server side and client side
    // [HarmonyPatch(typeof(ZNet), "Awake")]
    public static class ChangeGameServerVariables
    {
        private static void Postfix(ref ZNet __instance) {
            if (Settings.isEnabled("Server")) {
                int maxPlayers = Settings.getInt("Server", "maxPlayers");
                if (maxPlayers >= 1) {
                    // Set Server Instance Max Players
                    __instance.m_serverPlayerLimit = maxPlayers;
                }
            }
        }
    }

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
