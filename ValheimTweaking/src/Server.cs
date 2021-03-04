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
        private const bool PATCH_ALL = false;

        private static HarmonyMethod patchify(System.Type patch_type, string method_name) {
            return method_name != null ? new HarmonyMethod(patch_type.GetMethod(method_name)) : null;
        }

        public static void patch() {
            harmony = new Harmony(HARMONY_ID);

            Patch_ZNetAwake.patch(ref harmony);
            Patch_Character.patch(ref harmony);
            //Patch_ZDOMan.patch(ref harmony);
        }

        public static class Patch_MapServerSide
        {
            private const int MAP_SURFACE = 2048 * 2048;
            private const int CHUNK_COUNT = 64;

            public static void patch(ref Harmony harmony) {
                harmony.Patch(AccessTools.Method(typeof(ZNet), "RPC_PeerInfo"), patchify(typeof(Patch_MapServerSide), "postfix_RPC_PeerInfo"));
            }

            //

            public static void ClientMapExplore(ZRpc client, ZPackage z) {

            }

            public static void postfix_OnNewConnection(ZNetPeer peer, ZNet __instance) {
                if (!__instance.IsServer()) {
                    return;
                }

                peer.m_rpc.Register<ZPackage>("MapExplore", new Action<ZRpc, ZPackage>(Patch_MapServerSide.ClientMapExplore));
            }

            // =============

            public static void postfix_RPC_PeerInfo(ZRpc rpc, ZNet __instance) {
                
                for (int chunk = 0; chunk < 64; ++chunk) {

                }
            }
        }

        public static class Patch_ZNetAwake
        {
            public const int DEFAULT_dataPerSec = 61440;
            public const int OVERRIDE_dataPerSec_ratio = 2;

            public static void patch(ref Harmony harmony) {
                harmony.Patch(
                    AccessTools.Method(typeof(ZNet), "Awake"),
                    null,
                    new HarmonyMethod(typeof(Patch_ZNetAwake).GetMethod("postfix"))
                );
            }

            public static void postfix(ZNet __instance) {
                ServerPatches.m_isServer = __instance.IsServer();

                //if (ServerPatches.m_isServer) {
                //    ZNet.m_serverPassword = "";
                //}

                if (__instance.m_zdoMan.m_dataPerSec == DEFAULT_dataPerSec) {
                    __instance.m_zdoMan.m_dataPerSec *= OVERRIDE_dataPerSec_ratio;
                    ValheimTweaking.LogInfo("m_dataPerSec overriden to " + __instance.m_zdoMan.m_dataPerSec);
                }
            }
        }

        public static class Patch_SpamPeers
        {
            private static DateTime m_lastSpam = DateTime.Now;

            public static void patch(ref Harmony harmony) {
                harmony.Patch(
                    AccessTools.Method(typeof(ZNet), "Update"),
                    null,
                    new HarmonyMethod(typeof(Patch_SpamPeers).GetMethod("postfix"))
                );
            }

            public static void postfix() {
                var now = DateTime.Now;
                if ((now - m_lastSpam).TotalSeconds < 1) {
                    return;
                }

                if (ServerPatches.m_isServer) {
                    foreach (ZNetPeer peer in ZNet.instance.m_peers) {
                        ZNet.instance.RemotePrint(peer.m_rpc, "rawr Xd ;) " + now);
                    }
                }

                m_lastSpam = now;
            }
        }

        public static class Patch_FedjdStartup
        {
            public static void patch(ref Harmony harmony) {
                harmony.Patch(
                    AccessTools.Method(typeof(FejdStartup), "Awake"),
                    null,
                    new HarmonyMethod(typeof(Patch_FedjdStartup).GetMethod("postfix_Awake"))
                );
                harmony.Patch(
                    AccessTools.Method(typeof(FejdStartup), "GetPublicPasswordError"),
                    new HarmonyMethod(typeof(Patch_FedjdStartup).GetMethod("prefix_GetPublicPasswordError")),
                    null
                );
                harmony.Patch(
                    AccessTools.Method(typeof(FejdStartup), "IsPublicPasswordValid"),
                    new HarmonyMethod(typeof(Patch_FedjdStartup).GetMethod("prefix_IsPublicPasswordValid")),
                    null
                );
            }

            public static void postfix_Awake(ref FejdStartup __instance) {
                __instance.m_minimumPasswordLength = 0;
            }

            public static Boolean prefix_GetPublicPasswordError(ref string __result) {
                __result = "";
                return false;
            }

            public static Boolean prefix_IsPublicPasswordValid(ref Boolean __result) {
                __result = true;
                return false;
            }
        }

        public static class Patch_Character
        {
            public static void patch(ref Harmony harmony) {
                harmony.Patch(
                    AccessTools.Method(typeof(Character), "GetHoverName"),
                    null,
                    new HarmonyMethod(typeof(Patch_Character).GetMethod("postfix"))
                    //, null
                );
            }

            public static void prefix(ref GameObject spawnedObject) {
                ValheimTweaking.LogDebug("Spawned " + spawnedObject.GetComponent<Character>().m_name);
            }

            public static void postfix(ref string __result) {
                if (__result == "Greyling") {
                    ValheimTweaking.LogDebug("Replacing name of Greyling");
                    __result = "Little bitch";
                }
            }
        }

        public static class Patch_ZDOMan
        {
            public static void patch(ref Harmony harmony) {
                harmony.Patch(
                   AccessTools.Method(typeof(ZDOMan), "RPC_ZDOData"), 
                   null,
                   new HarmonyMethod(typeof(Patch_ZDOMan).GetMethod("postfix_AddPeer"))
               );
            }

            public static void postfix_AddPeer(ref ZRpc rpc, ref ZPackage pkg) {
                // ValheimTweaking.LogDebug("Added " + netPeer.m_playerName + " " + netPeer.m_characterID);
                // ZDOID zdoid = ZDOMan.instance.
                //ZDO zdo = ZDOMan.instance.GetZDO(zdoid);
                //zdo.Set("pvp", true);
            }

        }
    }
}
