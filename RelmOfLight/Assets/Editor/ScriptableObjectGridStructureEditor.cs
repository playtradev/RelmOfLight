using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(ScriptableObjectGridStructure))]
public class ScriptableObjectGridStructureEditor : Editor
{
    bool ShowTiles = false;
    BattleTileInfo bti, lastSelectedBti = null;
    WalkingSideType newWalking = WalkingSideType.LeftSide;

    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        ScriptableObjectGridStructure origin = (ScriptableObjectGridStructure)target;


        newWalking = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSideTypeOveeride", newWalking);

        if (GUILayout.Button("Apply WalkingSide"))
        {
            
            foreach (var item in origin.GridInfo)
            {
                item.BattleTileState = BattleTileStateType.Empty;
                item.WalkingSide = newWalking;
            }
        }


        origin.HasBaseTileEffect = EditorGUILayout.ToggleLeft("HasEffect", origin.HasBaseTileEffect);
        if (origin.HasBaseTileEffect)
        {
            origin.BaseEffectsOnTile.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", origin.BaseEffectsOnTile.TileAction);
            if (origin.BaseEffectsOnTile.TileAction == TileActionType.OverTime)
            {
                origin.BaseEffectsOnTile.HitTime = EditorGUILayout.FloatField("HitTime", origin.BaseEffectsOnTile.HitTime);
            }
            origin.BaseEffectsOnTile.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", origin.BaseEffectsOnTile.TileParticlesID);
            origin.BaseEffectsOnTile.EffectChances = EditorGUILayout.FloatField("EffectChances", origin.BaseEffectsOnTile.EffectChances);
            var list = origin.BaseEffectsOnTile.Effects;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            for (int a = 0; a < list.Count; a++)
            {
                origin.BaseEffectsOnTile.Effects[a] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + a, origin.BaseEffectsOnTile.Effects[a], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
            }
        }


        bool lastSelected = false;
        bool selected = false;
        if(origin.GridInfo.Count > 0)
        {
            EditorGUILayout.Space();
            for (int x = 0; x < 6; x++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int y = 0; y < 12; y++)
                {
                    //Debug.Log(x + "   " + y);
                    bti = origin.GridInfo.Where(r => r.Pos == new Vector2Int(x, y)).First();
                    bti.name = x + "," + y;
                    GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.label);

                    switch (bti.BattleTileState)
                    {
                        case BattleTileStateType.NonUsable:
                            TextFieldStyles.normal.textColor = Color.red;
                            break;
                        case BattleTileStateType.Empty:
                            TextFieldStyles.normal.textColor = bti.WalkingSide == WalkingSideType.LeftSide ? Color.green : Color.magenta;
                            break;
                        case BattleTileStateType.Occupied:
                            TextFieldStyles.normal.textColor = Color.green;
                            break;
                        case BattleTileStateType.Blocked:
                            TextFieldStyles.normal.textColor = Color.blue;
                            break;
                        case BattleTileStateType.Bound:
                            TextFieldStyles.normal.textColor = Color.yellow;
                            break;
                    }

                    lastSelected = bti.BattleTileState == BattleTileStateType.NonUsable ? false : true;
                    selected = EditorGUILayout.ToggleLeft(x + "," + y, lastSelected, TextFieldStyles, GUILayout.Width(40));
                    if (lastSelected != selected && selected)
                    {
                        lastSelectedBti = bti;
                    }

                    bti.BattleTileState = selected ?
                        bti.BattleTileState != BattleTileStateType.NonUsable ? bti.BattleTileState : BattleTileStateType.Empty : BattleTileStateType.NonUsable;
                }
                EditorGUILayout.EndHorizontal();
            }

            if(lastSelectedBti != null)
            {
                lastSelectedBti.WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSideType", lastSelectedBti.WalkingSide);
                lastSelectedBti.BattleTileState = (BattleTileStateType)EditorGUILayout.EnumPopup("BattleTileState", lastSelectedBti.BattleTileState);
                lastSelectedBti.HasEffect = EditorGUILayout.ToggleLeft("HasEffect", lastSelectedBti.HasEffect);
                if (lastSelectedBti.HasEffect)
                {
                    lastSelectedBti.BaseEffectsOnTile.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", lastSelectedBti.BaseEffectsOnTile.TileAction);
                    if (lastSelectedBti.BaseEffectsOnTile.TileAction == TileActionType.OverTime)
                    {
                        lastSelectedBti.BaseEffectsOnTile.HitTime = EditorGUILayout.FloatField("HitTime", lastSelectedBti.BaseEffectsOnTile.HitTime);
                    }
                    lastSelectedBti.BaseEffectsOnTile.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", lastSelectedBti.BaseEffectsOnTile.TileParticlesID);
                    lastSelectedBti.BaseEffectsOnTile.EffectChances = EditorGUILayout.FloatField("EffectChances", lastSelectedBti.BaseEffectsOnTile.EffectChances);
                    lastSelectedBti.BaseEffectsOnTile.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", lastSelectedBti.BaseEffectsOnTile.DurationOnTile);
                    lastSelectedBti.BaseEffectsOnTile.DurationOnTileV = EditorGUILayout.Vector2Field("DurationOnTileV", lastSelectedBti.BaseEffectsOnTile.DurationOnTileV);


                    var list = lastSelectedBti.BaseEffectsOnTile.Effects;
                    int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
                    while (newCount < list.Count)
                        list.RemoveAt(list.Count - 1);
                    while (newCount > list.Count)
                        list.Add(null);

                    for (int a = 0; a < list.Count; a++)
                    {
                        lastSelectedBti.BaseEffectsOnTile.Effects[a] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + a, lastSelectedBti.BaseEffectsOnTile.Effects[a], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
                    }
                }
                lastSelectedBti.OverrideTileADStats = EditorGUILayout.ToggleLeft("OverrideTileADStats", lastSelectedBti.OverrideTileADStats);
                if (lastSelectedBti.OverrideTileADStats)
                {
                    lastSelectedBti.TileAD = EditorGUILayout.Vector2Field("TileAD", lastSelectedBti.TileAD);
                }
               // lastSelectedBti.TileSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", lastSelectedBti.TileSprite, typeof(Sprite), true, GUILayout.Width(512), GUILayout.Height(512));
            }



            ShowTiles = EditorGUILayout.Foldout(ShowTiles, "ShowTiles Info ---------------------------------------------");
            if (ShowTiles)
            {
                for (int i = 0; i < origin.GridInfo.Count; i++)
                {
                    if(origin.GridInfo[i].BattleTileState != BattleTileStateType.NonUsable)
                    {
                        origin.GridInfo[i].ShowClose = EditorGUILayout.Foldout(origin.GridInfo[i].ShowClose, origin.GridInfo[i].Pos + "   Info ---------------------------------------------");
                        if (origin.GridInfo[i].ShowClose)
                        {
                            origin.GridInfo[i].WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSideType", origin.GridInfo[i].WalkingSide);
                            origin.GridInfo[i].BattleTileState = (BattleTileStateType)EditorGUILayout.EnumPopup("BattleTileState", origin.GridInfo[i].BattleTileState);
                            origin.GridInfo[i].HasEffect = EditorGUILayout.ToggleLeft("HasEffect", origin.GridInfo[i].HasEffect);
                            if (origin.GridInfo[i].HasEffect)
                            {
                                origin.GridInfo[i].BaseEffectsOnTile.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", origin.GridInfo[i].BaseEffectsOnTile.TileAction);
                                if (origin.GridInfo[i].BaseEffectsOnTile.TileAction == TileActionType.OverTime)
                                {
                                    origin.GridInfo[i].BaseEffectsOnTile.HitTime = EditorGUILayout.FloatField("HitTime", origin.GridInfo[i].BaseEffectsOnTile.HitTime);
                                }
                                origin.GridInfo[i].BaseEffectsOnTile.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", origin.GridInfo[i].BaseEffectsOnTile.TileParticlesID);
                                origin.GridInfo[i].BaseEffectsOnTile.EffectChances = EditorGUILayout.FloatField("EffectChances", origin.GridInfo[i].BaseEffectsOnTile.EffectChances);
                                origin.GridInfo[i].BaseEffectsOnTile.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", origin.GridInfo[i].BaseEffectsOnTile.DurationOnTile);
                                origin.GridInfo[i].BaseEffectsOnTile.DurationOnTileV = EditorGUILayout.Vector2Field("DurationOnTileV", origin.GridInfo[i].BaseEffectsOnTile.DurationOnTileV);
                                var list = origin.GridInfo[i].BaseEffectsOnTile.Effects;
                                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
                                while (newCount < list.Count)
                                    list.RemoveAt(list.Count - 1);
                                while (newCount > list.Count)
                                    list.Add(null);

                                for (int a = 0; a < list.Count; a++)
                                {
                                    origin.GridInfo[i].BaseEffectsOnTile.Effects[a] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + a, origin.GridInfo[i].BaseEffectsOnTile.Effects[a], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
                                }
                            }
                            origin.GridInfo[i].OverrideTileADStats = EditorGUILayout.ToggleLeft("OverrideTileADStats", origin.GridInfo[i].OverrideTileADStats);
                            if (origin.GridInfo[i].OverrideTileADStats)
                            {
                                origin.GridInfo[i].TileAD = EditorGUILayout.Vector2Field("TileAD", origin.GridInfo[i].TileAD);
                            }
                            // origin.GridInfo[i].TileSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", origin.GridInfo[i].TileSprite, typeof(Sprite), true, GUILayout.Width(512), GUILayout.Height(512));
                        }
                    }
                }
            }
        }

        EditorUtility.SetDirty(origin);
    }
}
