using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{

    
    public override void Awake()
    {
        base.Awake();
        /*
        var args = System.Environment.GetCommandLineArgs();
        print("Port = "+args[1]);
        GetComponent<SimpleWebTransport>().port = ushort.Parse(args[1]);
        */
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        UIManager.singleton.SetTab("menu",false);
        UIManager.singleton.SetTab("login",true);
        //UIManager.singleton.SetTab("loading",true);
        UIManager.singleton.PlayUISound("load");

    }

    private void OnConnectedToServer()
    {

    }

    public override void OnStartHost()
    {

    }
    

    public override void OnClientDisconnect()
    {
        UIManager.singleton.SetTab("menu",true);
        UIManager.singleton.PlayUISound("fail");
        UIManager.singleton.OpenPopup("Frånkopplad","Antingen har du ingen uppkoppling eller så uppdateras servern! Kom tillbaka om en stund!");
    }
}
