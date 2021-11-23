using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PlaytraGamesLtd;
using System.Linq;
using UnityEngine.Video;

[CustomEditor(typeof(ScriptableObjectPassiveSkill))]
public class ScriptableObjectPassiveSkillEditor : Editor
{
    ScriptableObjectPassiveSkill origin;
    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();

        origin = (ScriptableObjectPassiveSkill)target;

        EditorGUILayout.LabelField("GENERAL");
        EditorGUILayout.Space();
        //origin.MinLevel = EditorGUILayout.IntField("level", origin.MinLevel);
        //EditorGUILayout.LabelField("Level of Skill: " + origin.MinLevel + " (Set in character)");
        origin.AttackDisplayName = EditorGUILayout.TextField("Skill Display Name", origin.AttackDisplayName);
        origin.SkillID = EditorGUILayout.TextField("SKILL GROUP ID", origin.SkillID, new GUIStyle("Used to determine which skill level up group this skill is part of, they should share the same Skill ID"));
        origin.AttackDescription = EditorGUILayout.TextField("Skill Description", origin.AttackDescription);
        origin.AttackVideoClip = (VideoClip)EditorGUILayout.ObjectField("Skill Video Demo", origin.AttackVideoClip, typeof(VideoClip), allowSceneObjects: false);
        origin.AttackIcon = (Sprite)EditorGUILayout.ObjectField("Skill Icon", origin.AttackIcon, typeof(Sprite), allowSceneObjects: false);
        origin.attackDisplayType = (ScriptableObjectAttackBase.AttackDisplayType)EditorGUILayout.EnumPopup("Skill Display Type", origin.attackDisplayType);
        origin.IsMaskAttack = EditorGUILayout.Toggle("IsMaskAttack", origin.IsMaskAttack);

        var list = origin.PassiveSkills.ToList();
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Passive skills", list.Count));
        while (newCount < list.Count)
            list.RemoveAt(list.Count - 1);
        while (newCount > list.Count)
            list.Add(new PassiveSkillClass());


        origin.PassiveSkills = list.ToArray();

        for (int i = 0; i < list.Count; i++)
        {
            ShowNewStats(list[i]);
        }

        EditorUtility.SetDirty(origin);
    }

    public void ShowNewStats(PassiveSkillClass newStats)
    {
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;

        newStats.Show = EditorGUILayout.Foldout(newStats.Show, "New Stats Info -------------------------------------------");
        if (newStats.Show)
        {
            newStats.PassiveSkillTarget = (PassiveSkillTargetType)EditorGUILayout.EnumPopup("StatsToAffect", newStats.PassiveSkillTarget);
            newStats.PassiveSkill = (PassiveSkillType)EditorGUILayout.EnumPopup("StatsToAffect", newStats.PassiveSkill);
            if (newStats.PassiveSkill == PassiveSkillType.AddBulletType)
            {
                newStats.BulletModifier.BulletTypeModifier = (BulletType)EditorGUILayout.EnumPopup("BulletTypeModifier", newStats.BulletModifier.BulletTypeModifier);
                newStats.BulletModifier.InputType = (AttackInputType)EditorGUILayout.EnumPopup("InputType", newStats.BulletModifier.InputType);
            }
            else if (newStats.PassiveSkill == PassiveSkillType.BlackList || newStats.PassiveSkill == PassiveSkillType.WhiteList)
            {
                var list = newStats.BuffDebuffImmunities;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of immunities", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(BuffDebuffStatsType.ActionDisable_Mask);

                for (int i = 0; i < list.Count; i++)
                {
                    newStats.BuffDebuffImmunities[i] = (BuffDebuffStatsType)EditorGUILayout.EnumPopup("BuffDebuffImmunities " + i, newStats.BuffDebuffImmunities[i]);
                }
            }
            else if (newStats.PassiveSkill == PassiveSkillType.Ether_Regen_OnGrid_OnOff || newStats.PassiveSkill == PassiveSkillType.HP_Regen_OnGrid_OnOff)
            {
               
            }
            else
            {
                newStats.Value = EditorGUILayout.FloatField("Multiplier", newStats.Value);
            }

        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }
}
