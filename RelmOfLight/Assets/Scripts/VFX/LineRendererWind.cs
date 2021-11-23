using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererWind : MonoBehaviour
{
    public int WindCenter = 0;
    [Tooltip("Number of vertexes affected by the movement, 0 is just the wind center point")]
    public int WindNodesMagnitude = 2;
    [Tooltip("this curve should start from 1 and will show how much the vertex will move")]
    public AnimationCurve NodeInfluenceCurve;
    public float time = 3;
    public AnimationCurve WindBehaviourX;
    public AnimationCurve WindBehaviourY;
    public CordItem[] Items;

    Vector3[] Positions;
    Vector3[] BakedPositions;
    LineRenderer Line;

    // Start is called before the first frame update
    void Start()
    {
        Line = GetComponent<LineRenderer>();
        Positions = new Vector3[Line.positionCount];
        BakedPositions = new Vector3[Line.positionCount];
        Line.GetPositions(Positions);
        Positions.CopyTo(BakedPositions,0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            float DistanceFromCenter =  Mathf.Abs(i - WindCenter);
            if(WindNodesMagnitude>=0)
                DistanceFromCenter =( Mathf.Clamp(DistanceFromCenter, 0, WindNodesMagnitude)/(float)WindNodesMagnitude);
            else
            {
                DistanceFromCenter = i == WindCenter ? 0 : 1;
            }
            float MovementInfluence = NodeInfluenceCurve.Evaluate(DistanceFromCenter);
            float x = BakedPositions[i].x + MovementInfluence*(WindBehaviourX.Evaluate(Mathf.Abs((Time.time / time)-(float)((int)Time.time / (int)time))));
            float y = BakedPositions[i].y + MovementInfluence*(WindBehaviourY.Evaluate(Mathf.Abs((Time.time / time)-(float)((int)Time.time / (int)time))));
            Positions[i] = new Vector3(x, y, 0);
        }
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].Item.position = transform.position + Positions[Items[i].CordIndex];
        }
        GetComponent<LineRenderer>().SetPositions(Positions);
    }
}
[System.Serializable]
public class CordItem
{
    public Transform Item;
    public int CordIndex = 0;
}