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
                ALPlugin.Log.LogInfo("Got LobbyManager"); 
            } else {
                ALPlugin.Log.LogInfo("Couldnt grab LobbyManager");
            }
        }
    }
    public void CreateRoom()
    {  
        autoPlaylister.Gotpm = false;
        autoPlaylister.pm = null;
        ALPlugin.firstLoad = true;
        if(!ALPlugin.Active.Value){return;}
        try{
            lobbyInstance.MaxPlayers = 50; lobbyInstance.isRoomVisible = ALPlugin.Public.Value; lobbyInstance.createRoom_roomInputName.text = ALPlugin.Lobbyname.Value; lobbyInstance.CreateRoom();
        }catch(Exception y){
            ALPlugin.Log.LogError("Couldnt create Lobby\n" + y);
        } 
    }
}