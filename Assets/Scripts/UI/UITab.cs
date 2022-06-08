using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UITab : MonoBehaviour
{
    private Vector3 size;
    public Transform tab;
    public string name;
    private Coroutine TransitionCoroutine;
    public bool isActive;
    public bool isFixedTab;
    public bool allowOpenPause;
    public bool placeOnBottom;
    public bool isOverlay;



    public void AddPopup(String text)
    {
        
    }
    
    public void Init()
    {
        print("init");
        size = tab.localScale;
    }

    public Vector3 GetSize()
    {
        return size;
    }

    public virtual void ResetTab()
    {
        
    }

    
    public virtual void UpdateTab()
    {
        ButtonHighlight[] buttons = GetComponentsInChildren<ButtonHighlight>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].OnHoverExit();
        }
    }
    public void SetTab(bool state)
    {
        if (state==isActive)
        {
            return;
        }
        
        
        isActive = state;

        if (TransitionCoroutine!=null)
        {
            StopCoroutine(TransitionCoroutine);
        }
        
        tab.gameObject.SetActive(true);
        
        if (state)
        {
            TransitionCoroutine = StartCoroutine(CoroutineOpenTab(tab.transform));
        }
        else
        {
            TransitionCoroutine = StartCoroutine(CoroutineCloseTab(tab.transform));
        }
        
        
    }
    
    private IEnumerator CoroutineOpenTab(Transform _transform)
    {
        //If the tab is opened from another awake it will not yet be init. Wack solution
        if (size==Vector3.zero)
        {
            size = _transform.localScale;
        }

        _transform.gameObject.SetActive(true);
        Vector3 target = size;
        _transform.localScale = new Vector3(0, _transform.localScale.y, 0);
        while (_transform.localScale!=target)
        {
            _transform.localScale = Vector3.Lerp(_transform.localScale, target, Time.deltaTime * 30f);
            yield return null;
        }
        _transform.localScale = target;
    }
    private IEnumerator CoroutineCloseTab(Transform _transform)
    {
        Vector3 target = new Vector3(0, _transform.localScale.y, 0);
        
        
        while (_transform.localScale.x>0.1f)
        {
            _transform.localScale = Vector3.Lerp(_transform.localScale, target, Time.deltaTime * 30f);
            yield return null;
        }
        _transform.gameObject.SetActive(false);
        
        gameObject.SetActive(false);
    }
    
}
