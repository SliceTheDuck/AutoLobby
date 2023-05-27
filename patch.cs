using HarmonyLib;
using System;
using ZeepkistClient;
using UnityEngine;
using AutoLobby;

namespace lobby.Patches
{
    [HarmonyPatch(typeof(LobbyManager), "Awake")]
    class LMAwake
    {
        [HarmonyPostfix]
        public static void Patch()
        {
            Lobby.start = true;
        }
    }
    [HarmonyPatch(typeof(GameMaster), "ReleaseTheZeepkists")]
    class GMReleaseTheZeepkists
    {
        [HarmonyPostfix]
        public static void Patch()
        {
            if(ALPlugin.firstLoad){
                autoPlaylister.PushLevels(); ALPlugin.firstLoad = false;
            }
        }
    }
}