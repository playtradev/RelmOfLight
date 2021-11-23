using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileShaderPositionFeeder : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_TimeBeginning", Time.time);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_TilePos", transform.position.x);
    }
}
