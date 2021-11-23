using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneLoadManager))]
public class SceneLoadManagerEditor : Editor
{
    SceneLoadManager _Origin;
    SceneLoadManager Origin
    {
        set => _Origin = value;
        get
        {
            if(_Origin == null)
                _Origin = (SceneLoadManager)target;
            return _Origin;
        }
    }

    GridForceBuildSettings BuildSettings = new GridForceBuildSettings(false, false, false, false);

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("BUILD STUFF");

        BuildSettings.DrawGUI();

        EditorUtility.SetDirty(Origin);
        GUILayout.Space(20);

        base.OnInspectorGUI();
    }

}

public static class SceneLoadManagerEditor_ExtentionMethods
{
    static bool Expanded = false;
    public static void DrawGUI(this GridForceBuildSettings gridForceBuildSettings)
    {
        GUILayout.Space(5);
        Expanded = EditorGUILayout.Foldout(Expanded, "Build Settings");
        if (Expanded)
        {
            EditorGUI.indentLevel++; 
            EditorGUILayout.LabelField("Platforms");
            EditorGUI.indentLevel++;
            gridForceBuildSettings.CoreSightEnabled = EditorGUILayout.Toggle("CoreSight(G.round) Enabled", gridForceBuildSettings.CoreSightEnabled);
            gridForceBuildSettings.SteamEnabled = EditorGUILayout.Toggle("Steam Enabled", gridForceBuildSettings.SteamEnabled);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Custom Settings");
            EditorGUI.indentLevel++;
            gridForceBuildSettings.CheatsEnabled = EditorGUILayout.Toggle("Cheats Enabled", gridForceBuildSettings.CheatsEnabled);
            gridForceBuildSettings.EnableMouseSupport = EditorGUILayout.Toggle("Mouse Support Enabled", gridForceBuildSettings.EnableMouseSupport);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        GUILayout.Space(5);
    }
}