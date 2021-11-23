using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTrackerScript : MonoBehaviour
{
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void SetSpeed(float speed)
    {
        anim.speed = speed;
    }

}
