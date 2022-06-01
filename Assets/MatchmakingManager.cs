using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using Ping = UnityEngine.Ping;

public class MatchmakingManager : MonoBehaviour
{
    public string ip;
    public int port;
    public bool startMatchmaking;
    
    private void Update()
    {
        if (startMatchmaking)
        {
            StartMatchmaking();
            startMatchmaking = false;
        }
    }


    public void StartMatchmaking()
    {
        StartCoroutine(pingServer(ip,port));

    }

    private IEnumerator pingServer(string ip,int port)
    {

        Ping p = new Ping(ip+":"+port);
        
        print("ping "+ip+":"+port);
        
        while (!p.isDone)
        {
            yield return null;
        }

        print(p.time+"ms");

    }

    private void OnConnectedToServer()
    {
        print("Bajshora");
    }
}
