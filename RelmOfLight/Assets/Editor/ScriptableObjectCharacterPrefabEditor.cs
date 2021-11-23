using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spine.Unity;

[CustomEditor(typeof(ScriptableObjectCharacterPrefab))]
public class ScriptableObjectCharacterPrefabEditor : Editor
{
    ScriptableObjectCharacterPrefab origin;

    public override void OnInspectorGUI()
    {
        origin = (ScriptableObjectCharacterPrefab)target;

        EditorGUILayout.LabelField("Set up for " + (origin.AbridgedCharInfo != null && origin.AbridgedCharInfo.CharacterID != CharacterNameType.None ? origin.AbridgedCharInfo.Name != "" ? origin.AbridgedCharInfo.Name : "A configured but unnamed character" : "No Specified character").ToUpper());
        if(origin.AbridgedCharInfo != null && origin.AbridgedCharInfo.ConfigurationDate != new System.DateTime())
            EditorGUILayout.LabelField("LAST UPDATED - " + (origin.AbridgedCharInfo == null ? "Never" : origin.AbridgedCharInfo.ConfigurationDate.ToString()));
       /* origin.CharacterPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab to copy stats from", origin.CharacterPrefab, typeof(GameObject), false);
        if (GUILayout.Button("Update Stats from Prefab Character"))
        {
            origin.AbridgedCharInfo = new AbridgedCharacterInfo(origin.CharacterPrefab);
        }
        if (GUILayout.Button("Update Stats from Prefab Character & Remove Prefab Content"))
        {
            origin.AbridgedCharInfo = new AbridgedCharacterInfo(origin.CharacterPrefab);
            origin.CharacterPrefab = null;
        }*/
      

        EditorGUILayout.Space(10);
        origin.AbridgedCharInfo.skeletonDataAsset = (SkeletonDataAsset)EditorGUILayout.ObjectField("skeletonDataAsset", origin.AbridgedCharInfo.skeletonDataAsset, typeof(SkeletonDataAsset), false);

        if (origin.AbridgedCharInfo.skeletonDataAsset != null && !origin.AbridgedCharInfo.skeletonDataAsset.name.Contains("-Short"))
        {
            origin.AbridgedCharInfo.skeletonDataAsset = (SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(origin.AbridgedCharInfo.skeletonDataAsset).Replace(".","-Short."), typeof(SkeletonDataAsset));
        }

        origin.AbridgedCharInfo.UseSkins = EditorGUILayout.Toggle("UseSkins", origin.AbridgedCharInfo.UseSkins);
        if(origin.AbridgedCharInfo.UseSkins)
        {
            List<SkeletonDataAsset> ListSkins = origin.AbridgedCharInfo.Skins;
            int skins = EditorGUILayout.DelayedIntField("NumberOfSkins", ListSkins.Count);
            while (skins < ListSkins.Count)
                ListSkins.RemoveAt(ListSkins.Count - 1);
            while (skins > ListSkins.Count)
                ListSkins.Add(null);

            for (int i = 0; i < ListSkins.Count; i++)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                ListSkins[i] = (SkeletonDataAsset)EditorGUILayout.ObjectField("skeletonDataAsset", origin.AbridgedCharInfo.Skins[i], typeof(SkeletonDataAsset), false);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

        }

        EditorUtility.SetDirty(origin);
        base.OnInspectorGUI();
    }
}
