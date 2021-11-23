using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManagerScript : MonoBehaviour
{

    public static BulletManagerScript Instance;
    private Dictionary<int, GameObject> Bullets = new Dictionary<int, GameObject>();
    public GameObject BulletGameObject;
    public Transform BulletContainer;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetBullet()
    {
        GameObject res = null;
        res = Bullets.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (res == null)
        {
            res = Instantiate(BulletGameObject, BulletContainer);
            Bullets.Add(Bullets.Count, res);
        }
        res.SetActive(true);
        return res;
    }

    public void ResetBullets()
    {
        foreach (GameObject item in Bullets.Values.Where(r=> r.activeInHierarchy).ToList())
        {
            item.GetComponent<BulletScript>().EndBullet(0f, true);
        }
    }

}
