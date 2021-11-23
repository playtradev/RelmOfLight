using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowDownGame : MonoBehaviour
{
    public float Speed = 0.1f;
    public KeyCode keySlow = KeyCode.Z;
    public KeyCode keyNormal = KeyCode.X;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keySlow))
        {
            Time.timeScale = Speed;
            //Time.fixedDeltaTime = Speed;
        }
        if (Input.GetKeyDown(keyNormal))
        {
            Time.timeScale = 1;
            //Time.fixedDeltaTime = 1;

        }
    }
}
