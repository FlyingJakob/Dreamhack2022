using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{

    public float followSpeed;
    public float rotationSpeed;
    public Transform followTransform;
    public AnimationCurve fovShootCurve;
    public Camera camera;
    
    private static CameraController _singleton;

    public static CameraController singleton
    {
        get { return _singleton; }
    }

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

    public void Shoot()
    {
        if (fovCoroutine!=null)
        {
            StopCoroutine(fovCoroutine);
        }
        
        fovCoroutine = StartCoroutine(FOVShootCoroutine());
    }


    private Coroutine fovCoroutine;
    private float originalFOV;
    private IEnumerator FOVShootCoroutine()
    {
        float tc = 0;
        
        while (fovShootCurve.Evaluate(tc)>=0)
        {
            camera.fieldOfView = originalFOV + fovShootCurve.Evaluate(tc);
            tc += Time.deltaTime;
            yield return null;
        }

        camera.fieldOfView = originalFOV;
    }
    
    
    private void Awake()
    {
        InitSingleton();
        camera = GetComponent<Camera>();
        originalFOV  = camera.fieldOfView;
    }

    void FixedUpdate()
    {
        if (followTransform == null)
        {
            if (NetworkClient.localPlayer != null)
            {
                followTransform = NetworkClient.localPlayer.GetComponentInChildren<CamPos>().transform;
            }

            return;
        }

        transform.position = Vector3.Lerp(transform.position, followTransform.position, Time.deltaTime * followSpeed);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, followTransform.rotation, Time.deltaTime * rotationSpeed);
    }
}
    
    
