using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObjectMenuSkin))]
public class ScriptableObjectMenuSkinEditor : Editor
{
    public CharacterNameType LookFor;
    ScriptableObjectMenuSkin origin;
    public override void OnInspectorGUI()
    {

        origin = (ScriptableObjectMenuSkin)target;
        LookFor = (CharacterNameType)EditorGUILayout.EnumPopup("LookFor", LookFor);
        if (GUILayout.Button("Find char"))
        {
            foreach (var item in origin.MenuSkins)
            {
                if(item.CharId == LookFor)
                {

                }
            }
        }

        base.OnInspectorGUI();

        if(origin.test.Count > 0)
        {
            foreach (var item in origin.test)
            {
              /*  if(item.AbridgedCharInfo.skeletonDataAsset != null)
                {
                    origin.MenuSkins.Add(new MenuSkinClass(item.AbridgedCharInfo.CharacterID, item.AbridgedCharInfo.skeletonDataAsset));
                }*/
            }

            origin.test.Clear();
        }
       
    }
}
