using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBulletSpeedController : MonoBehaviour
{
    [Header("Dev parameter")]
    public float BulletTargetTime = 1;
    [Header("VFX Artist parameter")]
    public float BulletOriginalTime = 1;

    //PRIVATE
    VFXBulletSpeedCalibration[] SpeedCalibrators;

    private void Awake()
    {
        SpeedCalibrators = GetComponentsInChildren<VFXBulletSpeedCalibration>();
        ApplyTargetTime();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ApplyTargetTime();
    }

    public void ApplyTargetTime()
    {
        foreach (VFXBulletSpeedCalibration vfx in SpeedCalibrators)
        {
            vfx.BulletDuration = BulletTargetTime;
            vfx.BulletOriginalDuration = BulletOriginalTime;
            vfx.MultiplyTime();
        }
    }
}
