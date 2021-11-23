using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticles : MonoBehaviour
{
    public GameObject MeshParticles;
    public GameObject BillboardParticles;

    public List<GameObject> totalObj = new List<GameObject>();


    private void Instance_ButtonXUpEvent(int player, float value)
    {
        foreach (GameObject item in totalObj)
        {
            Destroy(item);
        }
    }

    private void Instance_ButtonBUpEvent(int player, float value)
    {
        GameObject go = Instantiate(BillboardParticles, new Vector3(Random.Range(-10, 10), Random.Range(-3, 3), 0), Quaternion.identity);
        totalObj.Add(go);
    }

    private void Instance_ButtonAUpEvent(int player, float value)
    {
        GameObject go = Instantiate(MeshParticles, new Vector3(Random.Range(-10, 10), Random.Range(-3, 3), 0), Quaternion.identity);
        totalObj.Add(go);
    }
}
