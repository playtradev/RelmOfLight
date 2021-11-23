using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderRandomFeeder : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        //GetComponent<SpriteRenderer>().material.SetFloat("_TimeBeginning", Time.time);
        GetComponent<SpriteRenderer>().material.SetFloat("_Random", Random.Range(0f,20000.1f));
    }


}
