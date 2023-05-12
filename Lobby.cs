using UnityEngine;
using System;
using BepInEx;
using System.Collections.Generic;
using AutoLobby;
using ZeepkistClient;

public class Lobby : MonoBehaviour
{
    public static bool start = false;
    private static LobbyManager lobbyInstance = null;
    public static bool importantbs;
    /*private PlaylistMenu pm;
    private Transform PlaylistEditor;*/

    public void Awake()
    {
        ZeepkistNetwork.ConnectedToMasterServer += CreateRoom;
    }
    public void Update()
    {
        if (start) {
            start = false;
            lobbyInstance = UnityEngine.Object.FindObjectOfType<LobbyManager>();

            if (lobbyInstance != null) {
                aLPlugin.Log.LogInfo("Got LobbyManager"); 
            } else {
                aLPlugin.Log.LogInfo("Couldnt grab LobbyManager");
            }
        }
    }
    public void CreateRoom()
    {  
        autoPlaylister.Gotpm = false;
        autoPlaylister.pm = null;
        aLPlugin.firstLoad = true;
        if(!aLPlugin.Active.Value){return;}
        try{
            lobbyInstance.MaxPlayers = 50; lobbyInstance.isRoomVisible = aLPlugin.Public.Value; lobbyInstance.createRoom_roomInputName.text = aLPlugin.Lobbyname.Value; lobbyInstance.CreateRoom();
        }catch(Exception y){
            aLPlugin.Log.LogError("Couldnt create Lobby\n" + y);
        } 
    }
}