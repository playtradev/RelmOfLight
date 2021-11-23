using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyRotation : MonoBehaviour
{

    public Vector3 Rotation;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Rotation) ;
    }
}
