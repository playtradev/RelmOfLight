using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScaleScript : MonoBehaviour
{
    public Vector2 ScaleMinMax = new Vector2(1f, 1f);
    public Vector3 RotationMin = new Vector3(0f, 0,0);
    public Vector3 RotationMax = new Vector3(360, 360, 360);
    //Get the attached Animator
    
    void OnEnable()
    {

        //Set a random scale
        float scale = Random.Range((float)ScaleMinMax[0], (float)ScaleMinMax[1]);
        Vector3 rotation =new Vector3(Random.Range((float)RotationMin[0], (float)RotationMax[0]), Random.Range((float)RotationMin[1], (float)RotationMax[1]), Random.Range((float)RotationMin[2], (float)RotationMax[2]));
        transform.rotation = Quaternion.Euler(rotation);
        transform.localScale = new Vector3(scale, scale, scale); 
    }
}
