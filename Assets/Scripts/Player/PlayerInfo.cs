using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [SyncVar] public string name;
    [SyncVar] public int score;

    [Command]
    public void CMDSetname(string name)
    {
        this.name = name;
    }
    
    public void AddScore(int amount)
    {
        score += amount;
        PlayerManager.singleton.SetScore(name,score);
    }


    private void Update()
    {
        if (isLocalPlayer)
        {
            UIManager.singleton.SetScoreText(score);
        }
    }
}
