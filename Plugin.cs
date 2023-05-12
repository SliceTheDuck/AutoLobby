using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;

namespace AutoLobby
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class aLPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<string> Lobbyname;
        public static ConfigEntry<string> Levels;
        public static ConfigEntry<bool> Active;
        public static ConfigEntry<bool> Public;

        public static ManualLogSource Log;

        public static Lobby Lobby;
        public static autoPlaylister Playlister;

        public static bool firstLoad = false;
        public string configentry;
        public static List<int> levelIds;
        private bool validateCoroutineRunning = false;

        Harmony harmony;

        private void Awake()
        {
            Lobbyname = Config.Bind("General", "Lobbyname", "Lobby", new ConfigDescription("Name to create lobby with"));
            Active = Config.Bind("General", "Active", false, new ConfigDescription("Should lobby be auto created?"));
            Public = Config.Bind("General", "Public", false, new ConfigDescription("Should lobby be public?"));
            Levels = Config.Bind("General", "Levelid", "5550", new ConfigDescription("gtr levelid's of levels to auto add on start, separated by \",\" (Doesnt work yet ^^)"));

            Levels.SettingChanged += (sender, args) => LevelsChange();

            Lobby = new GameObject("Lobby").AddComponent<Lobby>();
            DontDestroyOnLoad(Lobby);
            Playlister = new GameObject("autoPlaylister").AddComponent<autoPlaylister>();
            DontDestroyOnLoad(Playlister);
            
            string[] levelIdsStr = Levels.Value.Split(',');
            levelIds = new List<int>();

            foreach (string i in levelIdsStr) {
                if (int.TryParse(i.Trim(), out int levelId)) {
                    levelIds.Add(levelId);
                }
                else {
                    Log.LogError($"Invalid level ID: {i}");
                }
            }
            // Plugin startup logic
            harmony = new Harmony("autoLobby");

            Log = base.Logger;

            harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        private void LevelsChange()
        {
            if (validateCoroutineRunning) return;

            StartCoroutine(LevelsCoroutine());
            Log.LogInfo("Got change");
        }
        private IEnumerator LevelsCoroutine()
        {
            validateCoroutineRunning = true;

            yield return new WaitForSeconds(10f);

            UpdateLevelList();

            validateCoroutineRunning = false;
        }
        private void UpdateLevelList()
        {
            Log.LogDebug("Updating Levellist");
            string[] levelIdsStr = Levels.Value.Split(',');
            levelIds = new List<int>();

            foreach (string i in levelIdsStr) {
                if (int.TryParse(i.Trim(), out int levelId)) {
                    levelIds.Add(levelId);
                }
                else {
                    Log.LogError($"Invalid level ID: {i}");
                }
            }
        }
    }
}
