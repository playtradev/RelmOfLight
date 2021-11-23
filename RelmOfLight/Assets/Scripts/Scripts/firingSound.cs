using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firingSound : MonoBehaviour
{
    public AudioClip sfx = null;
    public float timeLoop = 1;
    AudioSource audioS;
    bool playing = false;
    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playing == false)
        {
            if (sfx != null)
            {
                audioS.PlayOneShot(sfx);
            }
            playing = true;
            Invoke("PlayingOff", timeLoop);
        }
    }
    void PlayingOff()
    {
        playing = false;
    }
}
