using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(BattleTileScript))]
public class BattleTileScriptEditor : Editor
{

    public TileEffectClass TileEffectTOTest;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BattleTileScript origin = (BattleTileScript)target;
        if(TileEffectTOTest == null)
        {
            TileEffectTOTest = new TileEffectClass();
        }
        TileEffectTOTest.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", TileEffectTOTest.TileAction);
        if (TileEffectTOTest.TileAction == TileActionType.OverTime)
        {
            TileEffectTOTest.HitTime = EditorGUILayout.FloatField("HitTime", TileEffectTOTest.HitTime);
        }
        TileEffectTOTest.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", TileEffectTOTest.TileParticlesID);
        TileEffectTOTest.EffectChances = EditorGUILayout.FloatField("EffectChances", TileEffectTOTest.EffectChances);
        TileEffectTOTest.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", TileEffectTOTest.DurationOnTile);
        TileEffectTOTest.DurationOnTileV = EditorGUILayout.Vector2Field("DurationOnTileV", TileEffectTOTest.DurationOnTileV);
        var list = TileEffectTOTest.Effects;
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
        while (newCount < list.Count)
            list.RemoveAt(list.Count - 1);
        while (newCount > list.Count)
            list.Add(null);

        for (int i = 0; i < list.Count; i++)
        {
            TileEffectTOTest.Effects[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + i, TileEffectTOTest.Effects[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
        }


        if (GUILayout.Button("Apply Debug BuffDebuff"))
        {
            origin.SetupEffect(TileEffectTOTest, null, null);
        }
    }
   
}