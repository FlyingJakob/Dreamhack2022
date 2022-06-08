using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnTab : UITab
{
    private float timeCounter;
    public float respawnTime;
    public bool isReady;
    public bool isCounting;
    public TextMeshProUGUI timeText;

    private void Awake()
    {
        Init();
    }
    private void OnEnable()
    {
        isReady = false;
        timeCounter = respawnTime;
        isCounting = true;
        PrintTime();
    }

    private void PrintTime()
    {
        timeText.text = "Återupplivas om : " + (int)timeCounter+" s";
    }

    public void Respawn(InputAction.CallbackContext context)
    {
        if (context.performed&&isReady)
        {
            NetworkClient.localPlayer.GetComponent<CombatController>().CMDPlayerRespawn();
            isReady = false;
            print("isReady = "+isReady);

        }
    }
    
    private void Update()
    {
        if (!isCounting)
        {
            return;
        }
        
        if (timeCounter<=0&&!isReady)
        {
            isReady = true;
            timeText.text = "Tryck [Mellanslag] för att återupplivas";
            isCounting = false;
        }
        if (!isReady)
        {
            float prevTime = timeCounter;
            timeCounter -= Time.deltaTime;
            if ((int)timeCounter!=(int)prevTime)
            {
                PrintTime();
            }
        }
    }
}
