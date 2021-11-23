using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDelay : MonoBehaviour
{
    [Range(0f,2f)]
    public float Delay = 0;

    private void OnEnable()
    {
        GetComponent<AudioSource>().PlayDelayed(Delay);
    }
}
