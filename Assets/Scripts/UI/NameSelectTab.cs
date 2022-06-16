using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class NameSelectTab : UITab
{
    private void Awake()
    {
        Init();
    }

    public TMP_InputField nameInputField;
    public TMP_InputField emailInputField;
    public TextMeshProUGUI charCounter;
    public TextMeshProUGUI emailWarning;

    private void Update()
    {
        charCounter.text = nameInputField.text.Length + "/15";
        if (nameInputField.text.Length>15||nameInputField.text.Length<2)
        {
            nameInputField.textComponent.color = Color.red;
            charCounter.color = Color.red;
        }
        else
        {
            nameInputField.textComponent.color = Color.white;
            charCounter.color = Color.white;
        }

        if (emailInputField.text.Contains("@")&&(emailInputField.text.Contains(".com")||emailInputField.text.Contains(".se")))
        {
            emailInputField.textComponent.color = Color.white;
            emailWarning.gameObject.SetActive(false);
        }
        else
        {
            emailInputField.textComponent.color = Color.red;
            emailWarning.gameObject.SetActive(true);
        }
    }


    public void Apply()
    {
        /*
        if (nameInputField.text.Length>15||nameInputField.text.Length<2||!(emailInputField.text.Contains("@")&&(emailInputField.text.Contains(".com")||emailInputField.text.Contains(".se"))))
        {
            return;
        }
        */

        if (!PlayerManager.singleton.RegisterPlayer(nameInputField.text,emailInputField.text))
        {
            UIManager.singleton.OpenPopup("namn/email upptaget",
                "Du kan inte använda ett användarnamn eller mailadress som redan används av en annan spelare");
            return;
        }
        
        string name = nameInputField.text;
        NetworkClient.localPlayer.GetComponent<PlayerInfo>().CMDSetname(name);
        NetworkClient.localPlayer.GetComponent<CombatController>().CMDPlayerRespawn();
        PlayerManager.singleton.CMDSetScore(name,0);
        UIManager.singleton.CloseTab("login");
    }
    
    
}
