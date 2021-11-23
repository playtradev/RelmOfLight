using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientRotation : MonoBehaviour
{
    public bool UseRotation = true;
    public Vector3 AxisOfRotation = new Vector3(0,0,1);
    public float Adjustment = 180;
    private Vector2 previousPos;
    private Vector2 currentPos;

    private void OnEnable()
    {
        if (!UseRotation)
        {
            enabled = false;
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        
            //Check if the object is still for no reasons
            if ((Vector2)transform.position != currentPos)
                //bake previous position to take a new one
                previousPos = currentPos;
            currentPos = transform.position;
            Vector3 dir = currentPos - previousPos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + Adjustment, AxisOfRotation);
            var main = GetComponent<ParticleSystem>().main;
            //int Offset = transform.localScale.x < 0 ? 1 : 0;
            main.startRotationZMultiplier = angle;
       
    }
}
