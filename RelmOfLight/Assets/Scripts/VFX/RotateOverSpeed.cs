using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverSpeed : MonoBehaviour
{

    public Vector3 AxisOfRotation = new Vector3(0, 0, 1);
    public float Range = 45;
    public float Sensibility = .5f;
    public float RotationSpeed = .1f;
    public float RotToCenter = .01f;
    [Range(0,360)]
    public float Adjustment = 180;
    private Vector2 previousPos;
    private Vector2 currentPos;
    float cumulateRot = 0;

    // Update is called once per frame
    void LateUpdate()
    {
        //Check if the object is still for no reasons
        if((Vector2)transform.position!= currentPos)
            //bake previous position to take a new one
            previousPos = currentPos;
        currentPos = transform.position;
        var dir = currentPos - previousPos;
        cumulateRot = Mathf.Clamp(cumulateRot+ RotationSpeed * Mathf.Clamp(Sensibility*(dir.y - dir.x),-1,1),  - Range,  Range);
        if (cumulateRot < 0)
        {
            cumulateRot += RotToCenter;
        }
        else
        {
            cumulateRot -= RotToCenter;
        }
        var angle = Mathf.Lerp(Adjustment - Range, Adjustment + Range, .5f + cumulateRot/Range/2);
        //var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg/360);
        transform.rotation = Quaternion.AngleAxis(angle+Adjustment, AxisOfRotation);
        //var main = GetComponent<ParticleSystem>().main;
        //int Offset = transform.localScale.x < 0 ? 1 : 0;
        //main.startRotationZMultiplier = angle;
    }
}
