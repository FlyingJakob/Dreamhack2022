using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
    
    public List<UITab> tabStack = new List<UITab>();
    public List<UITab> tabs;
    public bool isPaused;
    private void Awake()
    {
        InitSingleton();
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
        OpenPopup("Instruktioner","Detta spelet utvecklas med hjälp av dig. Kom till oss och lägg till nya funktioner till spelet tillsammans med oss");
    }
    
    
    
    public void OpenPopup(string title,string text)
    {
        SetTab("popup",true);

        foreach (UITab tab in tabs)
        {
            if (tab.name=="popup")
            {
               TextMeshProUGUI[] textObjs = tab.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
               textObjs[1].text = title;
               textObjs[0].text = text;
               return;
            }
        }
    }

    public GameObject loadingScreen;

    public void SetLoadingScreen(bool state)
    {
        loadingScreen.SetActive(state);
    }
    
    
    private void Update()
    {

        
        SetLoadingScreen(!NetworkClient.isConnected);
        
        
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (noTabsOpen()||tabStack.Last().allowOpenPause)
            {
                SetTab("pause",true);
            }
            else
            {
                ReturnToPreviousTab();
            }
            
            PlayUISound("pause");
            
            //SetPauseTab(!pauseTab.activeSelf&&!settingsTab.activeSelf);
        }
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




}
