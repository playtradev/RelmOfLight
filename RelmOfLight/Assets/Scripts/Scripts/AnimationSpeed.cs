using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires an animator to work, Change the speed of animations OnEnable and on command
[RequireComponent(typeof(Animator))]

public class AnimationSpeed : MonoBehaviour
{
    public float Speed = 1;
    //Get the attached Animator
    Animator anim;
    // Start is called before the first frame update
    void OnEnable()
    {
        anim = GetComponent<Animator>();
        //Play current animation at the speed selected
        anim.speed = Speed;
        anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0);
    }
    public void ChangeSpeed(float speed)
    {
        Speed = speed;
        anim.speed = Speed;
        anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0);

    }
}
