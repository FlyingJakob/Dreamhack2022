using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{

    public override void OnStartClient()
    {
        base.OnStartClient();
        UIManager.singleton.SetTab("menu",false);
        //UIManager.singleton.SetTab("loading",true);
        UIManager.singleton.PlayUISound("load");

    }

    private void OnConnectedToServer()
    {
        UIManager.singleton.SetTab("loading",false);

    }

    public override void OnStartHost()
    {
        UIManager.singleton.SetTab("loading",false);

    }


    public override void OnClientDisconnect()
    {
        UIManager.singleton.SetTab("menu",true);
        UIManager.singleton.PlayUISound("fail");
        UIManager.singleton.OpenPopup("Frånkopplad","Antingen har du ingen uppkoppling eller så uppdateras servern! Kom tillbaka om en stund!");
    }
}
