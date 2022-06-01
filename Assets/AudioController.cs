using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;


    public AudioClip laserClip;
    public AudioClip explosionClip;
    
    public void PlayClip(string name)
    {
        switch (name)
        {
            case "laser": audioSource.PlayOneShot(laserClip);
                print("Skjut");
                break;
            case "explosion": audioSource.PlayOneShot(explosionClip);
                break;
        }
    }
    
    
}
