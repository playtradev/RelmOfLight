using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetIndicatorManagerScript : MonoBehaviour
{

    public static TargetIndicatorManagerScript Instance;
    private Dictionary<int, GameObject> TargetsEnemy = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> TargetsPlayer = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> TargetsLockOn = new Dictionary<int, GameObject>();
    public GameObject TargetEnemyPrefab;
    public GameObject TargetPlayerPrefab;
    public GameObject TargetLockOnPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetTargetIndicator(bool LockOn = false)
    {
        Dictionary<int, GameObject> dToCheck = LockOn ? TargetsLockOn : TargetsEnemy;
        GameObject res = null;
        res = dToCheck.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if(res == null)
        {
            GameObject prefab = LockOn ? TargetLockOnPrefab : TargetEnemyPrefab;
            if (prefab == null) return null;
            res = Instantiate(prefab, transform);
            dToCheck.Add(dToCheck.Count, res);
        }
        res.transform.parent = transform;
        res.transform.position = Vector3.zero;
        return res;
    }
}
