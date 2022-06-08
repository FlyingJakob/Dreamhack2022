using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterSeconds : MonoBehaviour
{
    public float time;
    private void Start()
    {
        StartCoroutine(destroyAfterSeconds());
    }
    private IEnumerator destroyAfterSeconds()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
