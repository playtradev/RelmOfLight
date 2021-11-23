using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEventReceiverScript : MonoBehaviour
{
    public void SetFalse()
    {
        CameraManagerScript.Shaker.HaltShaking();
    }

}
