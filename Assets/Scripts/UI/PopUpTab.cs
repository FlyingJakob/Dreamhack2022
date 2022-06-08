using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTab : UITab
{
    public Button discardButton;
    public Button URLButton;

    public TextMeshProUGUI title;
    public TextMeshProUGUI text;

    public string url;
    
    public void SetPopupInfo(string title,string text,string url,string linkName)
    {
        this.title.text = title;
        this.text.text = text;
        this.url = url;
        URLButton.gameObject.SetActive(true);
        URLButton.onClick.AddListener(_OpenURL);
        URLButton.GetComponentInChildren<TextMeshProUGUI>().text = linkName;
        
        return;
    }
    public void SetPopupInfo(string title,string text)
    {
        this.title.text = title;
        this.text.text = text;
        URLButton.gameObject.SetActive(false);
        return;
    }
    
    
    public void _OpenURL()
    {
        Application.OpenURL(url);
    }

    
    
    
    
    
}
