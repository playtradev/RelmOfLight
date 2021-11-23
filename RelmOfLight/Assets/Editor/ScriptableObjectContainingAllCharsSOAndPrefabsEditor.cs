using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Spine.Unity;

[CustomEditor(typeof(ScriptableObjectContainingAllCharsSOAndPrefabs))]
public class ScriptableObjectContainingAllCharsSOAndPrefabsEditor : Editor
{
    ScriptableObjectContainingAllCharsSOAndPrefabs origin;

    public override void OnInspectorGUI()
    {
        origin = (ScriptableObjectContainingAllCharsSOAndPrefabs)target;

        origin.SkeletonC = (SkeletonAssetContainer)EditorGUILayout.ObjectField("SkeletonC", origin.SkeletonC, typeof(SkeletonAssetContainer), false);
        origin.CharactersData = (ScriptableObjectContainingAllCharsSO)EditorGUILayout.ObjectField("CharactersSO Data", origin.CharactersData, typeof(ScriptableObjectContainingAllCharsSO), false);

        if (origin.CharactersData == null)
            return;

        EditorUtility.SetDirty(origin);
        foreach (ScriptableObjectCharacterPrefab item in origin.CharactersData.CharacterSo)
        {
            EditorUtility.SetDirty(item);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("MASS UPDATE STATS");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("From Scriptable Object Data"))
        {
            origin.Update_ScriptableObjectStats_FromScriptableObjects();
        }
        if (GUILayout.Button("From Here"))
        {
            origin.Update_ScriptableObjectStats_FromSOPrefabGroup();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("MASS MANAGE PREFAB DATA");
        if (GUILayout.Button("Remove Prefab Data From Scriptable Objects"))
        {
            origin.RemoveAllPrefabsFromWithinScriptableObjects();
        }
        if (GUILayout.Button("Add Prefab Data From Here To Scriptable Objects"))
        {
            origin.AddPrefabsToScriptableObjectsFromList();
        }


        EditorUtility.SetDirty(origin);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("GROUPS (" + origin.SOPrefabGroups.Count.ToString() + ")");

        SkeletonLoaderClass slc;
       // origin.SkeletonC.Jsons.Clear();
       // origin.SkeletonC.SkeletonDatas.Clear();
        foreach (CharacterSOPrefabGroup characterSOPrefabGroup in origin.SOPrefabGroups)
        {
          /*  if (characterSOPrefabGroup.Prefab.name.Contains("_Riki_Checked"))
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(characterSOPrefabGroup.Prefab), characterSOPrefabGroup.Prefab.name.Replace("_Riki_Checked", ""));
            }*/

          /*  if (origin.SkeletonC != null && characterSOPrefabGroup.Prefab != null)
            {
                if(!origin.SkeletonC.DoesSkeletonExist(characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.CharacterID.ToString()))
                {
                    if(characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.skeletonDataAsset != null && characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.skeletonDataAsset != null)
                    {
                        slc = new SkeletonLoaderClass(characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.CharacterID.ToString(), characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.skeletonDataAsset.skeletonJSON,
                            characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.skeletonDataAsset.atlasAssets, characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.skeletonDataAsset.scale);
                        origin.SkeletonC.Jsons.Add(slc);
                    }
                }
            }*/


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                characterSOPrefabGroup.ScriptableObject == null ? "" :
                (characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo != null && characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.CharacterID != CharacterNameType.None ? characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.Name != "" ? characterSOPrefabGroup.ScriptableObject.AbridgedCharInfo.Name :
                "A configured but unnamed character" : "No Specified character").ToUpper());
            characterSOPrefabGroup.ScriptableObject = (ScriptableObjectCharacterPrefab)EditorGUILayout.ObjectField(characterSOPrefabGroup.ScriptableObject, typeof(ScriptableObjectCharacterPrefab), false);
            characterSOPrefabGroup.Prefab = (GameObject)EditorGUILayout.ObjectField(characterSOPrefabGroup.Prefab, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();
        }
        Refresh_SOPrefabGroups();


       
    }


    public void Refresh_SOPrefabGroups()
    {
        if (origin.CharactersData == null || origin.CharactersData.CharacterSo == null)
            return;

        EditorUtility.SetDirty(origin);
        if (origin.SOPrefabGroups == null)
            origin.SOPrefabGroups = new List<CharacterSOPrefabGroup>();

        foreach (ScriptableObjectCharacterPrefab scriptableObj in origin.CharactersData.CharacterSo)
        {
            if (origin.SOPrefabGroups.Where(r => r.ScriptableObject == scriptableObj).Count() != 0)
                continue;
            origin.SOPrefabGroups.Add(new CharacterSOPrefabGroup(scriptableObj));
        }

        foreach (CharacterSOPrefabGroup SOPrefabGroup in origin.SOPrefabGroups.ToArray())
        {
            if (SOPrefabGroup.ScriptableObject != null && origin.CharactersData.CharacterSo.ToList().Contains(SOPrefabGroup.ScriptableObject))
                continue;
            origin.SOPrefabGroups.Remove(SOPrefabGroup);
        }

        EditorUtility.SetDirty(origin);
    }
}

