using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    public void HostLobby()
    {
        networkManager.StartHost();
    }

    public void JoinLobby()
    {
        networkManager.networkAddress = "193.123.34.2";
        networkManager.StartClient();
    }
    

}
