using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDeactivated : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
