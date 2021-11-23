using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(BaseCharacter)), CanEditMultipleObjects]
public class BaseCharacterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BaseCharacter origin = (BaseCharacter)target;
        origin.testAtkEffect = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Debug BuffDebuff ", origin.testAtkEffect, typeof(ScriptableObjectAttackEffect), false);
        if (GUILayout.Button("Apply Debug BuffDebuff"))
        {
            origin.Buff_DebuffCo(origin, origin.testAtkEffect, null);
        }
    }
   
}