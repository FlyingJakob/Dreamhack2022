using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonHighlight : MonoBehaviour
{
    private Vector3 startScale;

    public float hoverSize;



    public Color HoverColor;
    public float colorTime;

    private Image image;

    private Color originalColor;

    public bool freeze;
    
    private void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        startScale = transform.localScale;
    }

    private Coroutine activeCoroutine;
    
    public void OnHoverEnter()
    {
        if (freeze)
        {
            return;
        }
     
        UIManager.singleton.PlayUISound("hover");
        
        if (activeCoroutine!=null)
        {
            StopCoroutine(activeCoroutine);
        }
        
        activeCoroutine = StartCoroutine(CoroutineResize(startScale*hoverSize));
    }

    public void OnClick()
    {
        if (freeze)
        {
            return;
        }
        UIManager.singleton.PlayUISound("click");

     
    }
    
    
    public void OnHoverExit()
    {
        if (startScale==Vector3.zero)
        {
            return;
        }
        
        
        
        if (freeze)
        {
            return;
        }
        if (activeCoroutine!=null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(CoroutineResize(startScale));
        
    }

    private IEnumerator CoroutineResize(Vector3 size)
    {

        
        
        float counter = 0;
        while (transform.localScale!=size)
        {
            counter += Time.deltaTime;

            if (counter<colorTime)
            {
                //image.color = Color.HSVToRGB(Random.Range(0f, 1f), 0.7f, 0.8f);
            }
            else
            {
                //image.color = originalColor;
            }
            
            transform.localScale = Vector3.Lerp(transform.localScale, size,10*Time.deltaTime);
            yield return null;
        }
        
        
        
    }
    
    
    

}
