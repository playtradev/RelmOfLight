using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXmanager : MonoBehaviour
{
    public static SFXmanager Instance;
    [SerializeField]
    private AudioSource audioS;
    public AudioClip ArrivingImpact;
    public AudioClip ArrivingSpawn;
    public AudioClip DefenceNormal;
    public AudioClip DefenceSpecial;


    public float vol;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayOnce(AudioClip clip)
    {
        audioS.PlayOneShot(clip, vol);
    }
}
