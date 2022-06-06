using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    private static PlayerManager _singleton;
    public static PlayerManager singleton { get { return _singleton; } }
    private void InitSingleton()
    {
        if (_singleton != null && _singleton != this)
        {
            Destroy(this);
        }
        else
        {
            _singleton = this;
        }
    }
    private void Awake()
    {
        InitSingleton();
        
        LoadPlayerData();
    }

    private void SavePlayerData()
    {
        StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath+"/registeredPlayers.txt");
        foreach (string key in registeredPlayers.Keys)
        {
            streamWriter.WriteLine(key+"-"+registeredPlayers[key],true);
        }
        streamWriter.Close();
        print("Saved playerdata to : "+Application.persistentDataPath+"/registeredPlayers.txt");
    }

    private void LoadPlayerData()
    {
        try
        {
            StreamReader streamWriter = new StreamReader(Application.persistentDataPath+"/registeredPlayers.txt");

            int time = 0;
        
            while (!streamWriter.EndOfStream)
            {
                string[] parts = streamWriter.ReadLine().Split('-');
                time++;
                registeredPlayers.Add(parts[0],parts[1]);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        
        
    }
    

    public SyncDictionary<string, string> registeredPlayers = new SyncDictionary<string, string>();

    public bool RegisterPlayer(string name,string email)
    {
        if (registeredPlayers.ContainsKey(name)||registeredPlayers.Values.Contains(email))
        {
            if (registeredPlayers.ContainsKey(name))
            {
                return registeredPlayers[name].Equals(email);
            }
            return false;
        }
        CMDRegisterPlayer(name,email);
        return true;
    }

    [Command(requiresAuthority = false)]
    public void CMDRegisterPlayer(string name,string email)
    {
        registeredPlayers.Add(name,email);
        SavePlayerData();
    }

    public SyncIDictionary <string,int> playerHighScores = new SyncIDictionary<string, int>(new SortedDictionary<string, int>());
    
    [Command(requiresAuthority = false)]
    public void CMDSetScore(string name,int score)
    {
        if (playerHighScores.ContainsKey(name))
        {
            if (playerHighScores[name]<score)
            {
                playerHighScores[name] = score;
            }
        }
        else
        {
            playerHighScores.Add(name,score);
        }
        
    }






}
