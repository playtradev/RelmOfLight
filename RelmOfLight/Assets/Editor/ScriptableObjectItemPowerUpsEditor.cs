using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObjectItemPowerUps))]
public class ScriptableObjectItemPowerUpsEditor : Editor
{



    ScriptableObjectItemPowerUps origin;

    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();

        origin = (ScriptableObjectItemPowerUps)target;

        EditorGUILayout.LabelField("PowerUp GENERAL");
        origin.powerUpText = EditorGUILayout.TextField("powerUpText", origin.powerUpText);
        origin.color = (PowerUpColorTypes)EditorGUILayout.EnumPopup("color", origin.color); 
        origin.activeParticles = (GameObject)EditorGUILayout.ObjectField("activeParticles ", origin.activeParticles, typeof(GameObject), false);
        origin.terminationParticles = (GameObject)EditorGUILayout.ObjectField("terminationParticles ", origin.terminationParticles, typeof(GameObject), false);
        origin.InfiniteOnFieldDuration = EditorGUILayout.Toggle("InfiniteOnFieldDuration", origin.InfiniteOnFieldDuration);
        if (origin.InfiniteOnFieldDuration)
        {
            origin.DurationOnField = EditorGUILayout.FloatField("DurationOnField", origin.DurationOnField);
        }

        EditorGUILayout.LabelField("GENERAL");
        EditorGUILayout.Space();
        origin.level = EditorGUILayout.IntSlider("level", origin.level, 0, 4);
        origin.StackType = (BuffDebuffStackType)EditorGUILayout.EnumPopup("StackType", origin.StackType);
        if(origin.StackType == BuffDebuffStackType.Stackable)
        {
            origin.maxStack = EditorGUILayout.IntSlider("maxStack", origin.maxStack, 1, 4);
        }
        origin._Duration = EditorGUILayout.Vector2Field("Duration", origin._Duration);
        if (origin._Duration != Vector2.zero)
        {
            origin._OvertimeRatio = EditorGUILayout.Vector2Field("OvertimeRatio", origin._OvertimeRatio);
        }
        origin.NameShowedOnIndicator = EditorGUILayout.TextField("NameShowedOnIndicator", origin.NameShowedOnIndicator);
        origin.OldSystem = EditorGUILayout.Toggle("OldSystem", origin.OldSystem);
        if (origin.OldSystem)
        {
            origin.OnCaster = EditorGUILayout.Toggle("OnCaster", origin.OnCaster);

            origin.StatsToAffect = (BuffDebuffStatsType)EditorGUILayout.EnumPopup("StatsToAffect", origin.StatsToAffect);
            if (origin.StatsToAffect == BuffDebuffStatsType.AttackChange)
            {
                origin.Atk = (ScriptableObjectAttackBase)EditorGUILayout.ObjectField("Atk", origin.Atk, typeof(ScriptableObjectAttackBase), false);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.Legion)
            {
                origin.ClonePrefab = (CharacterNameType)EditorGUILayout.EnumPopup("Clone Replacement", origin.ClonePrefab);
                origin.ClonePowerScale = EditorGUILayout.FloatField("Clone Power Multiplier", origin.ClonePowerScale);
                origin.CloneAsManyAsCurrentEnemies = EditorGUILayout.ToggleLeft("Clone count matches enemies", origin.CloneAsManyAsCurrentEnemies);
                if (!origin.CloneAsManyAsCurrentEnemies) origin.CloneAmount = EditorGUILayout.IntField("Number of clones", origin.CloneAmount);

                origin.SpawnInClosePosition = EditorGUILayout.ToggleLeft("Spawn clone in a close Position", origin.SpawnInClosePosition);
                origin.CloneStartingEffect = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Clone Starting Effect", origin.CloneStartingEffect, typeof(ScriptableObjectAttackEffect), false);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.WalkingSide)
            {
                origin.WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSide", origin.WalkingSide);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.Tile_ChangeSide)
            {
                origin.WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSide", origin.WalkingSide);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.Element)
            {
                origin.Elemental = (ElementalType)EditorGUILayout.EnumPopup("Elemental", origin.Elemental);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.ArmourType)
            {
                origin.ArmourT = (ArmourType)EditorGUILayout.EnumPopup("ArmourType", origin.ArmourT);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.ForceAI)
            {
                origin.ForcedAI = (AIType)EditorGUILayout.EnumPopup("ForcedAI", origin.ForcedAI);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.HP_Regen_OnGrid_OnOff || origin.StatsToAffect == BuffDebuffStatsType.Ether_Regen_OnGrid_OnOff)
            {
                origin.BoolValue = EditorGUILayout.Toggle("Value", origin.BoolValue);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.Cursed)
            {
                EditorGUI.indentLevel++;
                origin.StatsChecker = (StatsCheckerType)EditorGUILayout.EnumPopup("StatsChecker", origin.StatsChecker);
                if (origin.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    origin.Value = EditorGUILayout.Vector2Field("OnCasterAttackMultipier", origin.Value);
                }
                else
                {
                    origin.BaseCurrentValue = EditorGUILayout.Toggle("BaseCurrentValue", origin.BaseCurrentValue);
                    origin.Value = EditorGUILayout.Vector2Field("Value", origin.Value);
                }
                origin.ColorSize.Color = EditorGUILayout.ColorField("Overlay Color", origin.ColorSize.Color);
                origin.ColorSize.Hue = EditorGUILayout.Slider("Hue", origin.ColorSize.Hue, -0.5f, 0.5f);
                origin.ColorSize.Saturation = EditorGUILayout.Slider("Saturation", origin.ColorSize.Saturation, 0f, 2f);
                origin.ColorSizeCurve = EditorGUILayout.CurveField("ColorSizeCurve", origin.ColorSizeCurve);
                EditorGUI.indentLevel--;
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.DeathSentence)
            {
                EditorGUI.indentLevel++;
                origin.BoolValue = EditorGUILayout.Toggle("UseColorWithCurve", origin.BoolValue);
                if (origin.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    origin.ColorSize.Color = EditorGUILayout.ColorField("Overlay Color", origin.ColorSize.Color);
                    origin.ColorSize.Hue = EditorGUILayout.Slider("Hue", origin.ColorSize.Hue, -0.5f, 0.5f);
                    origin.ColorSize.Saturation = EditorGUILayout.Slider("Saturation", origin.ColorSize.Saturation, 0f, 2f);
                    origin.ColorSizeCurve = EditorGUILayout.CurveField("ColorSizeCurve", origin.ColorSizeCurve);
                }
                EditorGUI.indentLevel--;
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.ChancgeColor || origin.StatsToAffect == BuffDebuffStatsType.ChancgeColorWithCurve)
            {
                EditorGUI.indentLevel++;
                origin.ColorSize.Color = EditorGUILayout.ColorField("Overlay Color", origin.ColorSize.Color);
                origin.ColorSize.Hue = EditorGUILayout.Slider("Hue", origin.ColorSize.Hue, -0.5f, 0.5f);
                origin.ColorSize.Saturation = EditorGUILayout.Slider("Saturation", origin.ColorSize.Saturation, 0f, 2f);
                if (origin.StatsToAffect == BuffDebuffStatsType.ChancgeColorWithCurve)
                {
                    origin.ColorSizeCurve = EditorGUILayout.CurveField("ColorSizeCurve", origin.ColorSizeCurve);
                }
                EditorGUI.indentLevel--;
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.MeleeAttack)
            {
                origin.StatsChecker = StatsCheckerType.OnCasterAttack;
                origin._Duration = Vector2Int.one;
                origin.Value = EditorGUILayout.Vector2Field("OnCasterAttackMultipier", origin.Value);
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.Drain)
            {
                origin.BoolValue = EditorGUILayout.Toggle("DrainAtTheEnd", origin.BoolValue);

                origin.StatsChecker = (StatsCheckerType)EditorGUILayout.EnumPopup("StatsChecker", origin.StatsChecker);
                if (origin.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    origin.Value = EditorGUILayout.Vector2Field("OnCasterAttackMultipier", origin.Value);

                }
                else
                {
                    origin.BaseCurrentValue = EditorGUILayout.Toggle("BaseCurrentValue", origin.BaseCurrentValue);
                    origin.Value = EditorGUILayout.Vector2Field("Value", origin.Value);

                }
            }
            else if (origin.StatsToAffect == BuffDebuffStatsType.Teleport ||
                origin.StatsToAffect == BuffDebuffStatsType.Rebirth ||
                origin.StatsToAffect == BuffDebuffStatsType.Disable_CollisionWithTileEffect ||
                origin.StatsToAffect == BuffDebuffStatsType.Backfire ||
                origin.StatsToAffect == BuffDebuffStatsType.Tile_Blocked ||
                origin.StatsToAffect == BuffDebuffStatsType.Tile_ChangeSide ||
                origin.StatsToAffect == BuffDebuffStatsType.Tile_Free ||
                origin.StatsToAffect == BuffDebuffStatsType.ActionDisable_WeakAttack ||
                origin.StatsToAffect == BuffDebuffStatsType.ActionDisable_StrongAttack ||
                origin.StatsToAffect == BuffDebuffStatsType.ActionDisable_Skill1 ||
                origin.StatsToAffect == BuffDebuffStatsType.ActionDisable_Skill2 ||
                origin.StatsToAffect == BuffDebuffStatsType.ActionDisable_Mask ||
                origin.StatsToAffect == BuffDebuffStatsType.ActionDisable_Move ||
                origin.StatsToAffect == BuffDebuffStatsType.ActionDisable_Swap ||
                origin.StatsToAffect == BuffDebuffStatsType.Confusion ||
                origin.StatsToAffect == BuffDebuffStatsType.Undead ||
                origin.StatsToAffect == BuffDebuffStatsType.RemoveBuffs ||
                origin.StatsToAffect == BuffDebuffStatsType.RemoveDebuffs ||
                origin.StatsToAffect == BuffDebuffStatsType.FireParticlesToChar ||
                origin.StatsToAffect == BuffDebuffStatsType.StopChar ||
                origin.StatsToAffect == BuffDebuffStatsType.ShadowForm ||
                origin.StatsToAffect == BuffDebuffStatsType.StealAttack ||
                origin.StatsToAffect == BuffDebuffStatsType.KillPoolChar ||
                origin.StatsToAffect == BuffDebuffStatsType.Invulnerable)
            {
            }
            else
            {
                origin.StatsChecker = (StatsCheckerType)EditorGUILayout.EnumPopup("StatsChecker", origin.StatsChecker);
                if (origin.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    origin.Value = EditorGUILayout.Vector2Field("OnCasterAttackMultipier", origin.Value);
                }
                else
                {
                    origin.BaseCurrentValue = EditorGUILayout.Toggle("BaseCurrentValue", origin.BaseCurrentValue);
                    origin.Value = EditorGUILayout.Vector2Field("Value", origin.Value);
                }
            }
        }
        else
        {
            var list = origin.StatsToAffectList;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of NewStats", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(new StatsToAffectClass());

            for (int i = 0; i < list.Count; i++)
            {
                ShowNewStats(list[i]);
            }
        }

        origin.AnimToFired = (CharacterAnimationStateType)EditorGUILayout.EnumPopup("AnimToFired", origin.AnimToFired);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PARTICLES");
        EditorGUILayout.Space();
        origin.Particles = (ParticlesType)EditorGUILayout.EnumPopup("Particles", origin.Particles);


        origin.AttachPsToHead = EditorGUILayout.Toggle("AttachPsToHead", origin.AttachPsToHead);

        if (origin.StatsToAffect == BuffDebuffStatsType.Teleport || origin.StatsToAffect == BuffDebuffStatsType.WalkingSide)
        {
            origin.TeleportParticlesIn = (ParticlesType)EditorGUILayout.EnumPopup("TeleportParticlesIn", origin.TeleportParticlesIn);
            origin.TeleportParticlesOut = (ParticlesType)EditorGUILayout.EnumPopup("TeleportParticlesOut", origin.TeleportParticlesOut);
            if (origin.StatsToAffect == BuffDebuffStatsType.Teleport)
            {
                origin.TeleportInRandom = EditorGUILayout.Toggle("TeleportInRandom", origin.TeleportInRandom);
                if (!origin.TeleportInRandom)
                {
                    origin.FixedPos = EditorGUILayout.Vector2IntField("FixedPos", origin.FixedPos);
                }
            }
            origin.UseAnimEnumOrStringAnim = EditorGUILayout.Toggle("UseAnimEnumOrStringAnim", origin.UseAnimEnumOrStringAnim);
            if (origin.UseAnimEnumOrStringAnim)
            {
                origin.AnimEnumTeleportArrivingAnim = (CharacterAnimationStateType)EditorGUILayout.EnumPopup("AnimEnumTeleportArrivingAnim", origin.AnimEnumTeleportArrivingAnim);
            }
            else
            {
                origin.StringTeleportArrivingAnim = EditorGUILayout.TextField("StringTeleportArrivingAnim", origin.StringTeleportArrivingAnim);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI DISPLAY");
        EditorGUILayout.Space();

        origin.classification = (StatusEffectType)EditorGUILayout.EnumPopup("classification", origin.classification);
        origin.icon = (Sprite)EditorGUILayout.ObjectField("icon", origin.icon, typeof(Sprite), false);
        origin.recolorCharUI = EditorGUILayout.Toggle("recolorCharUI", origin.recolorCharUI);
        if (origin.recolorCharUI)
        {
            origin.statusIconColor = EditorGUILayout.ColorField("statusIconColor", origin.statusIconColor);
        }

        origin.SetParticlesOnCaster = EditorGUILayout.Toggle("SetParticlesOnCaster", origin.SetParticlesOnCaster);
        if (origin.SetParticlesOnCaster)
        {
            origin.ParticlesOnCaster = (ParticlesType)EditorGUILayout.EnumPopup("ParticlesOnCaster", origin.ParticlesOnCaster);

        }

        origin.SetIconOnCaster = EditorGUILayout.Toggle("SetIconOnCaster", origin.SetIconOnCaster);
        if (origin.SetIconOnCaster)
        {
            origin.OnCasterClassification = (StatusEffectType)EditorGUILayout.EnumPopup("OnCasterClassification", origin.OnCasterClassification);
            origin.OnCasterIcon = (Sprite)EditorGUILayout.ObjectField("OnCasterIcon", origin.OnCasterIcon, typeof(Sprite), false);
            origin.OnCasterRecolorCharUI = EditorGUILayout.Toggle("OnCasterRecolorCharUI", origin.OnCasterRecolorCharUI);
            if (origin.OnCasterRecolorCharUI)
            {
                origin.OnCasterStatusIconColor = EditorGUILayout.ColorField("OnCasterStatusIconColor", origin.OnCasterStatusIconColor);
            }
        }

      

        if (GUILayout.Button("Apply name"))
        {

            string assetPath = AssetDatabase.GetAssetPath(origin.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, origin.classification + "_" + origin.StatsToAffect + "_" + origin.StackType + "_" + origin.level);
            AssetDatabase.SaveAssets();
        }

        EditorUtility.SetDirty(origin);
    }

    public void ShowNewStats(StatsToAffectClass newStats)
    {
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;

        newStats.Show = EditorGUILayout.Foldout(newStats.Show, "New Stats Info -------------------------------------------");
        if (newStats.Show)
        {
            newStats.OnCaster = EditorGUILayout.Toggle("OnCaster", newStats.OnCaster);
            newStats.useDelay = EditorGUILayout.Toggle("useDelay", newStats.useDelay);
            newStats.StatsToAffect = (BuffDebuffStatsType)EditorGUILayout.EnumPopup("StatsToAffect", newStats.StatsToAffect);
            if (newStats.StatsToAffect == BuffDebuffStatsType.AttackChange)
            {
                newStats.Atk = (ScriptableObjectAttackBase)EditorGUILayout.ObjectField("Atk", newStats.Atk, typeof(ScriptableObjectAttackBase), false);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.Legion)
            {
                newStats.ClonePrefab = (CharacterNameType)EditorGUILayout.EnumPopup("Clone Replacement", newStats.ClonePrefab);
               
                newStats.ClonePowerScale = EditorGUILayout.FloatField("Clone Power Multiplier", newStats.ClonePowerScale);
                newStats.CloneAsManyAsCurrentEnemies = EditorGUILayout.ToggleLeft("Clone count matches enemies", newStats.CloneAsManyAsCurrentEnemies);
                if (!newStats.CloneAsManyAsCurrentEnemies) newStats.CloneAmount = EditorGUILayout.IntField("Number of clones", newStats.CloneAmount);

                newStats.SpawnInClosePosition = EditorGUILayout.ToggleLeft("Spawn clone in a close Position", newStats.SpawnInClosePosition);
                newStats.CloneStartingEffect = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Clone Starting Effect", newStats.CloneStartingEffect, typeof(ScriptableObjectAttackEffect), false);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.WalkingSide)
            {
                newStats.WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSide", newStats.WalkingSide);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.Tile_ChangeSide)
            {
                newStats.WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSide", newStats.WalkingSide);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.Element)
            {
                newStats.Elemental = (ElementalType)EditorGUILayout.EnumPopup("Elemental", newStats.Elemental);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.ArmourType)
            {
                newStats.ArmourT = (ArmourType)EditorGUILayout.EnumPopup("ArmourType", newStats.ArmourT);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.ForceAI)
            {
                newStats.ForcedAI = (AIType)EditorGUILayout.EnumPopup("ForcedAI", newStats.ForcedAI);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.HP_Regen_OnGrid_OnOff || newStats.StatsToAffect == BuffDebuffStatsType.Ether_Regen_OnGrid_OnOff)
            {
                newStats.BoolValue = EditorGUILayout.Toggle("Value", newStats.BoolValue);
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.Cursed)
            {
                EditorGUI.indentLevel++;
                newStats.StatsChecker = (StatsCheckerType)EditorGUILayout.EnumPopup("StatsChecker", newStats.StatsChecker);
                if (newStats.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    newStats._Value = EditorGUILayout.Vector2Field("OnCasterAttackMultipier", newStats._Value);
                }
                else
                {
                    newStats.BaseCurrentValue = EditorGUILayout.Toggle("BaseCurrentValue", newStats.BaseCurrentValue);
                    newStats._Value = EditorGUILayout.Vector2Field("Value", newStats._Value);
                }
                newStats.ColorSize.Color = EditorGUILayout.ColorField("Overlay Color", newStats.ColorSize.Color);
                newStats.ColorSize.Hue = EditorGUILayout.Slider("Hue", newStats.ColorSize.Hue, -0.5f, 0.5f);
                newStats.ColorSize.Saturation = EditorGUILayout.Slider("Saturation", newStats.ColorSize.Saturation, 0f, 2f);
                newStats.ColorSizeCurve = EditorGUILayout.CurveField("ColorSizeCurve", newStats.ColorSizeCurve);
                EditorGUI.indentLevel--;
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.DeathSentence)
            {
                EditorGUI.indentLevel++;
                newStats.BoolValue = EditorGUILayout.Toggle("UseColorWithCurve", newStats.BoolValue);
                if (newStats.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    newStats.ColorSize.Color = EditorGUILayout.ColorField("Overlay Color", newStats.ColorSize.Color);
                    newStats.ColorSize.Hue = EditorGUILayout.Slider("Hue", newStats.ColorSize.Hue, -0.5f, 0.5f);
                    newStats.ColorSize.Saturation = EditorGUILayout.Slider("Saturation", newStats.ColorSize.Saturation, 0f, 2f);
                    newStats.ColorSizeCurve = EditorGUILayout.CurveField("ColorSizeCurve", newStats.ColorSizeCurve);
                }
                EditorGUI.indentLevel--;
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.ChancgeColor || newStats.StatsToAffect == BuffDebuffStatsType.ChancgeColorWithCurve)
            {
                EditorGUI.indentLevel++;
                newStats.ColorSize.Color = EditorGUILayout.ColorField("Overlay Color", newStats.ColorSize.Color);
                newStats.ColorSize.Hue = EditorGUILayout.Slider("Hue", newStats.ColorSize.Hue, -0.5f, 0.5f);
                newStats.ColorSize.Saturation = EditorGUILayout.Slider("Saturation", newStats.ColorSize.Saturation, 0f, 2f);
                if(newStats.StatsToAffect == BuffDebuffStatsType.ChancgeColorWithCurve)
                {
                    newStats.OnDuration = EditorGUILayout.Toggle("OnDuration", newStats.OnDuration);
                    newStats.ColorSizeCurve = EditorGUILayout.CurveField("ColorSizeCurve", newStats.ColorSizeCurve);
                }
                EditorGUI.indentLevel--;
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.Drain)
            {
                newStats.BoolValue = EditorGUILayout.Toggle("DrainAtTheEnd", newStats.BoolValue);
                newStats.StatsChecker = (StatsCheckerType)EditorGUILayout.EnumPopup("StatsChecker", newStats.StatsChecker);
                if (newStats.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    newStats._Value = EditorGUILayout.Vector2Field("OnCasterAttackMultipier", newStats._Value);

                }
                else
                {
                    newStats.BaseCurrentValue = EditorGUILayout.Toggle("BaseValue", newStats.BaseCurrentValue);
                    newStats._Value = EditorGUILayout.Vector2Field("Value", newStats._Value);

                }
            }
            else if (newStats.StatsToAffect == BuffDebuffStatsType.Teleport ||
                newStats.StatsToAffect == BuffDebuffStatsType.Rebirth ||
                newStats.StatsToAffect == BuffDebuffStatsType.Disable_CollisionWithTileEffect ||
                newStats.StatsToAffect == BuffDebuffStatsType.Backfire ||
                newStats.StatsToAffect == BuffDebuffStatsType.Tile_Blocked ||
                newStats.StatsToAffect == BuffDebuffStatsType.Tile_ChangeSide ||
                newStats.StatsToAffect == BuffDebuffStatsType.Tile_Free ||
                newStats.StatsToAffect == BuffDebuffStatsType.ActionDisable_WeakAttack ||
                newStats.StatsToAffect == BuffDebuffStatsType.ActionDisable_StrongAttack ||
                newStats.StatsToAffect == BuffDebuffStatsType.ActionDisable_Skill1 ||
                newStats.StatsToAffect == BuffDebuffStatsType.ActionDisable_Skill2 ||
                newStats.StatsToAffect == BuffDebuffStatsType.ActionDisable_Mask ||
                newStats.StatsToAffect == BuffDebuffStatsType.ActionDisable_Move ||
                newStats.StatsToAffect == BuffDebuffStatsType.ActionDisable_Swap ||
                newStats.StatsToAffect == BuffDebuffStatsType.Confusion ||
                newStats.StatsToAffect == BuffDebuffStatsType.Undead ||
                newStats.StatsToAffect == BuffDebuffStatsType.RemoveBuffs ||
                newStats.StatsToAffect == BuffDebuffStatsType.RemoveDebuffs ||
                newStats.StatsToAffect == BuffDebuffStatsType.FireParticlesToChar ||
                newStats.StatsToAffect == BuffDebuffStatsType.StopChar ||
                newStats.StatsToAffect == BuffDebuffStatsType.ShadowForm ||
                newStats.StatsToAffect == BuffDebuffStatsType.StealAttack ||
                newStats.StatsToAffect == BuffDebuffStatsType.KillPoolChar ||
                newStats.StatsToAffect == BuffDebuffStatsType.Invulnerable)
            {
            }
            else
            {
                newStats.StatsChecker = (StatsCheckerType)EditorGUILayout.EnumPopup("StatsChecker", newStats.StatsChecker);
                if (newStats.StatsChecker == StatsCheckerType.OnCasterAttack)
                {
                    newStats._Value = EditorGUILayout.Vector2Field("OnCasterAttackMultipier", newStats._Value);

                }
                else
                {
                    newStats.BaseCurrentValue = EditorGUILayout.Toggle("BaseValue", newStats.BaseCurrentValue);
                    newStats._Value = EditorGUILayout.Vector2Field("Value", newStats._Value);

                }
            }
        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }
}
