using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(BattleInfoManagerScript))]
public class BattleInfoManagerScriptEditor : Editor
{

    public override void OnInspectorGUI()
    {
       

        BattleInfoManagerScript origin = (BattleInfoManagerScript)target;
        base.OnInspectorGUI();

    }
   
}