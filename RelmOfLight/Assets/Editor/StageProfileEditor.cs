using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(StageProfile)), CanEditMultipleObjects]
public class StageProfileEditor : Editor
{
    StageProfile origin;

    Dictionary<string, bool> SectionDisplayToggles = new Dictionary<string, bool>();

    BattleInfoManagerScript temp_BattleInfoM;

    public override void OnInspectorGUI()
    {
        origin = (StageProfile)target;
        origin.Addressables = true;
        DrawGroup("Stage Identity", DrawGroup_StageIdentity);
        DrawGroup("Scene Objects", DrawGroup_SceneObjects);
        base.OnInspectorGUI();

        EditorUtility.SetDirty(origin);
    }




    string titleFlair = "________________________________________________________________________";
    delegate void DrawGroupMethod();
    void DrawGroup(string title, DrawGroupMethod group, bool activeByDefault = false, bool useFlair = true)
    {
        if (!SectionDisplayToggles.ContainsKey(title))
            SectionDisplayToggles.Add(title, activeByDefault);
        SectionDisplayToggles[title] = EditorGUILayout.Foldout(SectionDisplayToggles[title], title.ToUpper() + (useFlair ? titleFlair.Remove(Mathf.Clamp(titleFlair.Length - 1 - title.Length, 0, 1000), title.Length) : ""));
        EditorGUILayout.Space();
        if (SectionDisplayToggles[title])
        {
            EditorGUI.indentLevel++;
            group.Invoke();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
    }


    Vector2 scroll;
    void DrawGroup_StageIdentity()
    {
        origin.Name = EditorGUILayout.TextField("Name", origin.Name);
        origin.ID = EditorGUILayout.TextField("ID", origin.ID);
        origin.sceneName = EditorGUILayout.TextField("Scene Name", origin.sceneName);
       // origin.Thumbnail = (Sprite)EditorGUILayout.ObjectField("Thumbnail", origin.Thumbnail, typeof(Sprite), false);
        EditorGUILayout.LabelField("Environment Description");

        EditorStyles.textField.wordWrap = true;
        origin.Description = EditorGUILayout.TextField(origin.Description, GUILayout.Height(60));
        EditorStyles.textField.wordWrap = false;

        origin.stageNameType = (StageNameType)EditorGUILayout.EnumPopup("Environment Type", origin.stageNameType);
        origin.type = (StageType)EditorGUILayout.EnumPopup("Combat Type", origin.type);

        if (!SectionDisplayToggles.ContainsKey(nameof(origin.RequiredCompletedStages)))
            SectionDisplayToggles.Add(nameof(origin.RequiredCompletedStages), false);
        SectionDisplayToggles[nameof(origin.RequiredCompletedStages)] = EditorGUILayout.Foldout(SectionDisplayToggles[nameof(origin.RequiredCompletedStages)], "Required Completed Stages");
        if (SectionDisplayToggles[nameof(origin.RequiredCompletedStages)])
        {
            EditorGUI.indentLevel++;
            List<string> reqsList = origin.RequiredCompletedStages.ToList();
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", reqsList.Count));
            while (newCount < reqsList.Count)
                reqsList.RemoveAt(reqsList.Count - 1);
            while (newCount > reqsList.Count)
                reqsList.Add("");
            EditorGUI.indentLevel++;
            for (int i = 0; i < reqsList.Count; i++)
            {
                reqsList[i] = EditorGUILayout.TextField("Required Stage " + (i + 1).ToString(), reqsList[i]);
            }
            EditorGUI.indentLevel--;
            origin.RequiredCompletedStages = reqsList.ToArray();
            EditorGUI.indentLevel--;
        }
    }


    void DrawGroup_SceneObjects()
    {
      //  origin.Rewired = (GameObject)EditorGUILayout.ObjectField("Rewired", origin.Rewired, typeof(GameObject), false);

        EditorGUILayout.Space();

        Draw_AddressableStageObjects();

        /*  temp_WaveM = origin.Wave != null ? origin.Wave.GetComponentInChildren<WaveManagerScript>() : null;
          temp_BattleInfoM = origin.BattleInfoManager != null ? origin.BattleInfoManager.GetComponentInChildren<BattleInfoManagerScript>() : null;*/

        EditorGUILayout.Space();

    }

    private void Draw_AddressableStageObjects()
    {
        origin.AddressableBaseName = EditorGUILayout.TextField("Addressable Base Name", origin.AddressableBaseName);

        origin.BattleManagerName = EditorGUILayout.TextField("BattleManager", origin.BattleManagerName);

        origin.BattleInfoManagerName = EditorGUILayout.TextField("BattleInfoManager", origin.BattleInfoManagerName);

        origin.BaseEnvironmentName = EditorGUILayout.TextField("BaseEnvironment", origin.BaseEnvironmentName);

        origin.WaveManagerName = EditorGUILayout.TextField("WaveManager", origin.WaveManagerName);

        origin.EventManagerName = EditorGUILayout.TextField("EventManager", origin.EventManagerName);

        origin.UI_BattleName = EditorGUILayout.TextField("UI_Battle", origin.UI_BattleName);

        origin.IndicatorCanvasName = EditorGUILayout.TextField("IndicatorCanvas", origin.IndicatorCanvasName);

        origin.AudioManagerName = EditorGUILayout.TextField("AudioManager", origin.AudioManagerName);
    }
}
