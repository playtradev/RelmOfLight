using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpineAnimationManager))]
public class SpineAnimatorManagerEditor : Editor
{

    SpineAnimationManager origin;

    public GameObject Space_Time_Curves_UpMovement;
    public GameObject Space_Time_Curves_DownMovement;
    public GameObject Space_Time_Curves_BackwardMovement;
    public GameObject Space_Time_Curves_ForwardMovement;

    public GameObject Speed_Time_Curves_UpMovement;
    public GameObject Speed_Time_Curves_DownMovement;
    public GameObject Speed_Time_Curves_BackwardMovement;
    public GameObject Speed_Time_Curves_ForwardMovement;


    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        base.OnInspectorGUI();
        //test = false;
        origin = (SpineAnimationManager)target;
       
        if (origin.Space_Time_Curves.ForwardMovement == null)
        {
            origin.Space_Time_Curves.ForwardMovement = Space_Time_Curves_ForwardMovement.GetComponent<Base_MovementSpeedScript>().Curve;
            origin.Space_Time_Curves.BackwardMovement = Space_Time_Curves_BackwardMovement.GetComponent<Base_MovementSpeedScript>().Curve;
            origin.Space_Time_Curves.UpMovement = Space_Time_Curves_UpMovement.GetComponent<Base_MovementSpeedScript>().Curve;
            origin.Space_Time_Curves.DownMovement = Space_Time_Curves_DownMovement.GetComponent<Base_MovementSpeedScript>().Curve;
        }

        origin.Space_Time_Curves.UpMovement = EditorGUILayout.CurveField("UpMovementSpeed", origin.Space_Time_Curves.UpMovement);
        origin.Space_Time_Curves.DownMovement = EditorGUILayout.CurveField("DownMovementSpeed", origin.Space_Time_Curves.DownMovement);
        origin.Space_Time_Curves.BackwardMovement = EditorGUILayout.CurveField("ForwardMovementSpeed", origin.Space_Time_Curves.BackwardMovement);
        origin.Space_Time_Curves.ForwardMovement = EditorGUILayout.CurveField("BackwardMovementSpeed", origin.Space_Time_Curves.ForwardMovement);

        EditorUtility.SetDirty(origin);
    }
}
