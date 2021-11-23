using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStartupScript : MonoBehaviour
{
    public List<GameObject> ObjectsToEnable;
    public List<GameObject> ObjectsToDisable;
    private void Awake()
    {
        for (int i = 0; i < ObjectsToEnable.Count; i++)
        {
            GameObject g = (GameObject)ObjectsToEnable[i];
            if (g == null)
            {
                Debug.LogError("You SILLY! hook the position:" + i + "in ObjectToEnable");
            }
            else
            {
            g.SetActive(true);
            }
        }
        for (int i = 0; i < ObjectsToDisable.Count; i++)
        {
            GameObject g = ObjectsToDisable[i];
            if (g == null)
            {
                Debug.LogError("You SILLY! hook the position:" + i + "in ObjectToDisable");
            }
            else
            {
                g.SetActive(false);
            }
        }
    }
}
