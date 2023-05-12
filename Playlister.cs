using UnityEngine;
using ZeepkistClient;
using ZeepkistNetworking;
using System;
using System.Collections.Generic;
using AutoLobby;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

public class autoPlaylister : MonoBehaviour
{
    public static PlaylistMenu pm;
    public static bool Gotpm = false;
    private static Transform PlaylistEditor;
    public static async void PushLevels()
    {
        if(!aLPlugin.Active.Value){return;}
        while(!Gotpm){
            try{
                GameObject gameplayCanvas = GameObject.Find("Gameplay Canvas");
                Transform OnlineUI = gameplayCanvas.transform.GetChild(1);
                PlaylistEditor = OnlineUI.transform.GetChild(4);
                pm = PlaylistEditor.GetComponent<PlaylistMenu>();
                aLPlugin.Log.LogInfo("Got pm");
                Gotpm = true;
            }catch(Exception){}
        }
        foreach(int i in aLPlugin.levelIds)
        {
            string returnValue = await GetGTRData(i);
            aLPlugin.Log.LogInfo(returnValue);
            var data = JsonConvert.DeserializeObject<LevelJson>(returnValue);
            AddToPlaylist(pm, data.author.ToString(), data.name.ToString(), data.uniqueId.ToString(), (ulong)data.workshopId);
        }
    }

    public static void AddToPlaylist(PlaylistMenu pm, string author, string name, string uid, ulong workshopID)
    {
        aLPlugin.Log.LogInfo("Started Adding Level");
        pm.thePlaylist = ZeepkistNetwork.CurrentLobby.Playlist;
        pm.random = ZeepkistNetwork.CurrentLobby.PlaylistRandom;
        pm.roundLength = ZeepkistNetwork.CurrentLobby.PlaylistTime;
        pm.currentPlayListIndex = ZeepkistNetwork.CurrentLobby.CurrentPlaylistIndex;
        pm.nextPlayListIndex = ZeepkistNetwork.CurrentLobby.NextPlaylistIndex;
        pm.RebuildPlaylist(PlaylistMenu.RebuildPlaylistOptions.refresh, 0);

        ZeepkistNetworking.OnlineZeeplevel ozl = new ZeepkistNetworking.OnlineZeeplevel()
        {
            Author = author,
            Name = name,
            UID = uid,
            WorkshopID = workshopID
        };

        pm.thePlaylist.Add(ozl);
        pm.RebuildPlaylist(PlaylistMenu.RebuildPlaylistOptions.refresh, 0);

        ZeepkistNetwork.NetworkClient?.SendPacket<ChangeLobbyPlaylistPacket>(new ChangeLobbyPlaylistPacket()
        {
            NewTime = pm.roundLength,
            IsRandom = pm.random,
            Playlist = pm.thePlaylist
        });
        ZeepkistNetwork.CurrentLobby.Playlist = pm.thePlaylist;
        ZeepkistNetwork.CurrentLobby.PlaylistRandom = pm.random;
        ZeepkistNetwork.CurrentLobby.PlaylistTime = pm.roundLength;
        pm.acceptPlayListEvent.Invoke();
    }
    private async void GetLevel(int j)
    {
        string returnValue = await GetGTRData(j);
        var data = JsonConvert.DeserializeObject<LevelJson>(returnValue);
    }
    private static async Task<string> GetGTRData(int j)
    {

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://api.zeepkist-gtr.com/levels/{j}");
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            } else {
                return null;
            }
        }
    }
    public class LevelJson
    {
        public int id { get; set; }
        public string uniqueId { get; set; }
        public ulong workshopId { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string timeAuthor { get; set; }
        public string timeGold { get; set; }
        public string timeSilver { get; set; }
        public string timeBronze { get; set; }
        public string thumbnailUrl { get; set; }
    }
}