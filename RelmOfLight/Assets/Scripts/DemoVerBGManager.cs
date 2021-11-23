using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoVerBGManager : MonoBehaviour
{
    public static DemoVerBGManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void FlyInOut(bool state)
    {
        GetComponent<Animation>().Stop();
        GetComponent<Animation>().clip = state ? GetComponent<Animation>().GetClip("DemoVerBG_In") : GetComponent<Animation>().GetClip("DemoVerBG_Out");
        GetComponent<Animation>().Play();
    }
}
