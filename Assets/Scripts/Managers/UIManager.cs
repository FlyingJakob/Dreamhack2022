using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;
    public static UIManager singleton { get { return _singleton; } }
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

    public GameObject HUD;
    
    public List<UITab> tabStack = new List<UITab>();
    public List<UITab> tabs;
    public bool isPaused;
    private void Awake()
    {
        InitSingleton();
        crossOrgSize = crosshair.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        UILayer = LayerMask.NameToLayer("UI");
        SetTab("menu",true);
        audioSource = GetComponent<AudioSource>();
    }
    
    private bool noTabsOpen() => tabStack.Count == 0;
    
    public void OpenTab(string name)
    {
        SetTab(name,true);
    }
    public void CloseTab(string name)
    {
        SetTab(name,false);
    }
    public void CloseAllTabs()
    {
        while (tabStack.Count>0)
        {
            SetTab(tabStack[0].name,false);
        }
    }
    public void ResetTab(string name)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].name == name)
            {
                tabs[i].ResetTab();
            }
        }
    }
    
    public void SetTab(string name,bool state)
    {
        print("Set Tab "+name+" -> "+state);
        
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].name==name)
            {
                if (state)
                {
                    if (tabStack.Contains(tabs[i]))
                    {
                        if (tabs[i].isFixedTab)
                        {
                            UITab tabToMove = tabs[i];
                            tabs.Remove(tabToMove);
                            tabs.Add(tabToMove);
                        }
                        return;                        
                    }
                    
                    if (tabs[i].placeOnBottom)
                    {
                        tabs[i].gameObject.SetActive(true);
                        tabStack.Insert(0,tabs[i]);
                        tabs[i].UpdateTab();
                    }
                    else
                    {
                        if (tabs[i].isOverlay)
                        {
                            
                        }
                        else
                        {
                            if (tabStack.Count>0)
                            {
                                tabStack.Last().gameObject.SetActive(true);
                                tabStack.Last().SetTab(false);
                            }
                        }
                        
                        
                        tabStack.Add(tabs[i]);
                    }
                    
                    UnlockCursor();
                }
                else
                {

                    if (!tabs[i].isActive)
                    {
                        tabs[i].gameObject.SetActive(false);
                        tabStack.Remove(tabs[i]);
                    }
                    

                    if (!tabStack.Contains(tabs[i]))
                    {
                        print("Didnt do anyting to tab");
                        return;
                    }
                    
                    
                    tabStack.Remove(tabs[i]);

                    if (noTabsOpen())
                    {
                        lockCursor();
                    }
                    else
                    {
                        if (tabStack.Last()!=null)
                        {
                            tabStack.Last().gameObject.SetActive(true);
                            tabStack.Last().SetTab(true);
                        }
                        else
                        {
                            print("Null wtf");
                        }
                        
                    }
                }

                if (tabs[i].placeOnBottom&&tabStack.Count>1)
                {
                    tabs[i].gameObject.SetActive(false);
                }
                else
                {
                    
                    tabs[i].gameObject.SetActive(true);
                    tabs[i].SetTab(state);
                    if (state)
                    {
                        tabs[i].UpdateTab();
                    }
                }
                
                
                return;
            }            
        }

        if (tabStack.Count==0)
        {
            
        }
        
        Debug.LogError("Tab name incorrect");
    }

    
    public void ReturnToPreviousTab()
    {
        if (tabStack.Last().isFixedTab)
        {
            return;
        }
        
        tabStack.Last().SetTab(false);
        tabStack.RemoveAt(tabStack.Count - 1);
        if (tabStack.Count>0)
        {
            tabStack.Last().gameObject.SetActive(true);
            tabStack.Last().SetTab(true);
            tabStack.Last().UpdateTab();
        }

        if (noTabsOpen())
        {
            lockCursor();
        }
    }
    
    
    
    private void UnlockCursor()
    {
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void lockCursor()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public RectTransform crosshair;
    
    public void SetCrosshairPos(Vector3 pos)
    {
        crosshair.position = Camera.main.WorldToScreenPoint(pos);
    }

    private Coroutine hitmarkCoroutine;
    public void MarkHit()
    {
        if (hitmarkCoroutine!=null)
        {
            StopCoroutine(hitmarkCoroutine);
        }
        hitmarkCoroutine = StartCoroutine(CoroutineMarkHit());
    }

    public AnimationCurve hitMarkSize;
    private Vector2 crossOrgSize;
    private IEnumerator CoroutineMarkHit()
    {
        RectTransform cross = crosshair.transform.GetChild(0).GetComponent<RectTransform>();
        
        float tc = 0;
        
        while (hitMarkSize.Evaluate(tc)>0)
        {
            cross.sizeDelta =crossOrgSize + Vector2.one * hitMarkSize.Evaluate(tc);
            tc += Time.deltaTime;
            yield return null;
        }

        cross.sizeDelta = crossOrgSize;
    }

    [Header("UISounds")] 
    public AudioClip hoverClip;
    public AudioClip clickClip;
    public AudioClip failClip;
    public AudioClip loadClip;
    public AudioClip pauseClip;
    private AudioSource audioSource;
    
    public void PlayUISound(string name)
    {
        switch (name)
        {
            case "hover" : audioSource.PlayOneShot(hoverClip);
                break;
            case "click" : audioSource.PlayOneShot(clickClip);
                break;
            case "fail" : audioSource.PlayOneShot(failClip);
                break;
            case "load" : audioSource.PlayOneShot(loadClip);
                break;
            case "pause" : audioSource.PlayOneShot(pauseClip);
                break;
        }
    }

    public void OpenInstructions()
    {
        OpenPopup("Instruktioner","Detta spelet utvecklas med hj??lp av dig. Kom till oss och l??gg till nya funktioner till spelet tillsammans med oss");
    }
    public void OpenCode()
    {
        OpenURLPopup("Kolla p?? koden","Om du ??r intresserad av hur detta spelet ??r gjort finns all kod tillg??nglig p?? github.","https://github.com/FlyingJakob/Dreamhack2022","GitHub");
    }
    
    
    public void OpenPopup(string title,string text)
    {
        SetTab("popup",true);

        foreach (UITab tab in tabs)
        {
            if (tab.name=="popup")
            { 
                ((PopUpTab)tab).SetPopupInfo(title,text);
                return;
            }
        }
    }

    public void OpenURLPopup(string title,string text,string url,string linkName)
    {
        SetTab("popup",true);

        foreach (UITab tab in tabs)
        {
            if (tab.name=="popup")
            {
                ((PopUpTab)tab).SetPopupInfo(title,text,url,linkName);
            }
        }
    }
    

    public GameObject loadingScreen;

    public void SetLoadingScreen(bool state)
    {
        loadingScreen.SetActive(state);
    }

    public void SetScoreTab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OpenTab("score");
        }

        if (context.canceled)
        {
            CloseTab("score");

        }
    }
    
    
    
    public void Back(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        if (noTabsOpen()||tabStack.Last().allowOpenPause)
        {
            SetTab("pause",true);
        }
        else
        {
            ReturnToPreviousTab();
        }
            
        PlayUISound("pause");

    }

    private void Update()
    {
        SetLoadingScreen(!NetworkClient.isConnected);
        HUD.SetActive(noTabsOpen());
    }
    
    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    private LayerMask UILayer;
    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public TextMeshProUGUI scoreText;
    public void SetScoreText(int score)
    {
        if (HUD.activeSelf)
        {
            scoreText.text = "score : " + score;
        }
    }


}
