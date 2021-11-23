using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIBattleFieldManager))]
public class UIBattleFieldManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UIBattleFieldManager origin = (UIBattleFieldManager)target;

        if (origin.Materials.Count > 0)
        {
            foreach (BattleFieldIndicatorMaterialClass item in origin.Materials)
            {
                item.name = item.BattleFieldIndicatorT.ToString();
            }
        }

    }
}
