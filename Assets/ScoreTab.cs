using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTab : UITab
{
    private void Awake()
    {
        Init();
    }

    public Color selfEntryColor;


    public GameObject entryPrefab;
    public Transform entryGrid;
    
    public override void UpdateTab()
    {
        List<KeyValuePair<string, int>> scores =  SortScores();

        foreach (Transform child in entryGrid)
        {
            Destroy(child.gameObject);
        }
        
        
        
        for (int i = 0; i < scores.Count; i++)
        {

            GameObject entry;
            
            entry = Instantiate(entryPrefab, Vector3.zero, quaternion.identity, entryGrid);

            TextMeshProUGUI[] txt = entry.GetComponentsInChildren<TextMeshProUGUI>();

            if (scores[i].Key.Equals(NetworkClient.localPlayer.GetComponent<PlayerInfo>().name))
            {
                entry.GetComponentInChildren<Image>().color = selfEntryColor;
            }
            
            txt[0].text ="#"+(i+1)+" - "+ scores[i].Key;
            txt[1].text = scores[i].Value.ToString();
        }
        
    }

    public List<KeyValuePair<string, int>> SortScores()
    {
        List<KeyValuePair<string, int>> scores = PlayerManager.singleton.playerHighScores.ToList();

        scores.Sort(
            delegate(KeyValuePair<string, int> pair1,
                KeyValuePair<string, int> pair2) {
                return pair2.Value.CompareTo(pair1.Value);
            }
        );
        return scores;
    }
    
    
    
}
