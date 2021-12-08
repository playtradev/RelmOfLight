using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using PlaytraGamesLtd;

[CanEditMultipleObjects]
[CustomEditor(typeof(CharacterInfoScript))]
public class CharacterInfoScriptEditor : Editor
{
    public ScriptableObjectSwappableBases SwappableBases;
    //wave.Show = EditorGUILayout.Foldout(wave.Show, (string.IsNullOrEmpty(wave.name) ? "Wave Name" : wave.name) + "  ---------------------------");
    //typeOfCharacter.CharacterName = (CharacterNameType)EditorGUILayout.EnumPopup("CharacterName", typeOfCharacter.CharacterName);
    bool ShowIdentity = false, ShowGridPos = false, ShowLayering = false, ShowVitality = false, ShowHealth = false,
         ShowEther = false, ShowSpeed = false, ShowDamage = false, ShowShield = false, ShowSpeedMovement = false,
         ShowSpeedAtk = false, ShowLuck = false, ShowWeakAtk = false, ShowStronAtk = false, ShowAttackSo = false,
         ShowMisc = false, ShowControl = false, ShowAI = false, ShowStagger = false, ShowBehaviour = false, ShowDeathExplosionInfo = false,
         ShowLevels = false, ShowSkills = false, ShowSkills_General = false, ShowSkills_Levelled = false;

    //SerializedProperty testProp;
    CharacterInfoScript origin;

    private void OnEnable()
    {
        //testProp = serializedObject.FindProperty("RelationshipList");
    }

    public override void OnInspectorGUI()
    {
        origin = (CharacterInfoScript)target;
        //serializedObject.Update();
        //EditorGUILayout.PropertyField(testProp);
        //serializedObject.ApplyModifiedProperties();

        // base.OnInspectorGUI();
        EditorGUILayout.Space();
        ShowIdentity = EditorGUILayout.Foldout(ShowIdentity, "Identity_________________________________________________");
        if (ShowIdentity)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            origin.Name = EditorGUILayout.TextField("Name", origin.Name);
            origin.CharacterID = (CharacterNameType)EditorGUILayout.EnumPopup("CharacterID", origin.CharacterID);

            origin.ClassType = (CharacterClassType)EditorGUILayout.EnumPopup("ClassType", origin.ClassType);
            origin.BaseChar = (BaseCharType)EditorGUILayout.EnumPopup("BaseChar", origin.BaseChar);

            origin.CharacterIcon = (Sprite)EditorGUILayout.ObjectField("Icon", origin.CharacterIcon, typeof(Sprite), false);
       
            origin.CharaterLevel = (LevelType)EditorGUILayout.EnumPopup("CharaterLevel", origin.CharaterLevel);

            origin.BuffDebuffImmunitiesListType = (ExclusionType)EditorGUILayout.EnumPopup("BuffDebuffImmunitiesListType", origin.BuffDebuffImmunitiesListType);
            var list = origin.BuffDebuffImmunities;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of BuffDebuffImmunitiesListType", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(BuffDebuffStatsType.Armour);
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                origin.BuffDebuffImmunities[i] = (BuffDebuffStatsType)EditorGUILayout.EnumPopup("BuffDebuffImmunities " + i, origin.BuffDebuffImmunities[i]);
            }
            EditorGUI.indentLevel--;
            origin.Elemental = (ElementalType)EditorGUILayout.EnumPopup("Elemental", origin.Elemental);

            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;


        }


        EditorGUILayout.Space();
        ShowVitality = EditorGUILayout.Foldout(ShowVitality, "Attributes_________________________________________");
        if (ShowVitality)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            EditorGUILayout.Space();
            ShowSkills = EditorGUILayout.Foldout(ShowSkills, "Skills_________________________________________");
            if (ShowSkills)
            {
                EditorGUI.indentLevel++;
                DrawAndValidate_UpgradeList(origin);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            ShowHealth = EditorGUILayout.Foldout(ShowHealth, "Health_________________________________________");
            if (ShowHealth)
            {
                origin.HealthStats.Health = EditorGUILayout.FloatField("Health", origin.HealthStats.Health);
                origin.HealthStats.Armour = EditorGUILayout.FloatField("Armour", origin.HealthStats.Armour);

                var list = origin.HealthStats.ArmourT;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of armour types", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(ArmourType.Base);
                EditorGUI.indentLevel++;
                for (int i = 0; i < list.Count; i++)
                {
                    origin.HealthStats.ArmourT[i] = (ArmourType)EditorGUILayout.EnumPopup("ArmourType " + i, origin.HealthStats.ArmourT[i]);
                }
                EditorGUI.indentLevel--;


            }
            EditorGUILayout.Space();


            ShowSpeed = EditorGUILayout.Foldout(ShowSpeed, "Speed_________________________________________");
            if (ShowSpeed)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                origin.SpeedStats.BaseSpeed = EditorGUILayout.FloatField("BaseSpeed", origin.SpeedStats.BaseSpeed);
                EditorGUILayout.LabelField("MovementTime:");
                origin.SpeedStats.MovementTime = EditorGUILayout.Slider(origin.SpeedStats.MovementTime, 0, 100);
                EditorGUILayout.LabelField("AttackTime:");
                origin.SpeedStats.AttackTime = EditorGUILayout.Slider(origin.SpeedStats.AttackTime, 0, 100);
                origin.SpeedStats.ArriveAnimSpeed = EditorGUILayout.FloatField("ArriveAnimSpeed", origin.SpeedStats.ArriveAnimSpeed);
                origin.SpeedStats.LeaveAnimSpeed = EditorGUILayout.FloatField("LeaveAnimSpeed", origin.SpeedStats.LeaveAnimSpeed);
                EditorGUILayout.Space();
                ShowSpeedMovement = EditorGUILayout.Foldout(ShowSpeedMovement, "Movement_________________________________________");
                if (ShowSpeedMovement)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("TileMovementTime:");
                    origin.SpeedStats.TileMovementTime = EditorGUILayout.Slider(origin.SpeedStats.TileMovementTime, 0, 2);
                    EditorGUILayout.LabelField("MovementCost:");
                    origin.SpeedStats.MovementCost = EditorGUILayout.Slider(origin.SpeedStats.MovementCost, 0, 10);

                    EditorGUILayout.LabelField("MovementSpeed:");
                    origin.SpeedStats.MovementSpeed = EditorGUILayout.Slider(origin.SpeedStats.MovementSpeed, 0, 10);
                    origin.SpeedStats.CuttingPerc = EditorGUILayout.FloatField("CuttingPerc", origin.SpeedStats.CuttingPerc);
                    origin.SpeedStats.IntroPerc = EditorGUILayout.FloatField("IntroPerc", origin.SpeedStats.IntroPerc);
                    origin.SpeedStats.LoopPerc = EditorGUILayout.FloatField("LoopPerc", origin.SpeedStats.LoopPerc);
                    origin.SpeedStats.EndPerc = EditorGUILayout.FloatField("EndPerc", origin.SpeedStats.EndPerc);


                    origin.SpeedStats.ReactionTime = EditorGUILayout.Vector2Field("ReactionTime", origin.SpeedStats.ReactionTime);
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }

                ShowSpeedAtk = EditorGUILayout.Foldout(ShowSpeedAtk, "Atk_________________________________________");
                if (ShowSpeedAtk)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("AttackLoopDuration:");
                    origin.SpeedStats.AttackLoopDuration = EditorGUILayout.Slider(origin.SpeedStats.AttackLoopDuration, 0, 2);

                    origin.SpeedStats.UpdateLoopDurationForTileAttack = EditorGUILayout.Toggle("UpdateLoopDurationForTileAttack", origin.SpeedStats.UpdateLoopDurationForTileAttack);
                    if (origin.SpeedStats.UpdateLoopDurationForTileAttack)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField("AttackLoopDurationTileAttack:");
                        origin.SpeedStats.AttackLoopDurationTileAttack = EditorGUILayout.Slider(origin.SpeedStats.AttackLoopDurationTileAttack, 0, 2);
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.LabelField("SlowDownPercentageOnHolding:");
                    origin.SpeedStats.SlowDownPercentageOnHolding = EditorGUILayout.Slider(origin.SpeedStats.SlowDownPercentageOnHolding, 0, 1);
                    EditorGUILayout.LabelField("IdleToAttackDuration:");
                    origin.SpeedStats.IdleToAttackDuration = EditorGUILayout.Slider(origin.SpeedStats.IdleToAttackDuration, 0, 2);

                    origin.SpeedStats.OverrideAtkToIdleDuration = EditorGUILayout.Toggle("OverrideAtkToIdleDuration", origin.SpeedStats.OverrideAtkToIdleDuration);

                    if (origin.SpeedStats.OverrideAtkToIdleDuration)
                    {
                        EditorGUILayout.LabelField("AtkToIdleDuration:");
                        origin.SpeedStats.AttackToIdleDuration = EditorGUILayout.Slider(origin.SpeedStats.AttackToIdleDuration, 0, 2);
                    }

                    origin.SpeedStats.Override_Buff_AtkToIdleDuration = EditorGUILayout.Toggle("Override_Buff_AtkToIdleDuration", origin.SpeedStats.Override_Buff_AtkToIdleDuration);

                    if (origin.SpeedStats.Override_Buff_AtkToIdleDuration)
                    {
                        EditorGUILayout.LabelField("Buff_AttackToIdleDuration:");
                        origin.SpeedStats.Buff_AttackToIdleDuration = EditorGUILayout.Slider(origin.SpeedStats.Buff_AttackToIdleDuration, 0, 2);
                    }

                    origin.SpeedStats.Override_Debuff_AtkToIdleDuration = EditorGUILayout.Toggle("Override_Debuff_AtkToIdleDuration", origin.SpeedStats.Override_Debuff_AtkToIdleDuration);

                    if (origin.SpeedStats.Override_Debuff_AtkToIdleDuration)
                    {
                        EditorGUILayout.LabelField("Debuff_AttackToIdleDuration:");
                        origin.SpeedStats.Debuff_AttackToIdleDuration = EditorGUILayout.Slider(origin.SpeedStats.Debuff_AttackToIdleDuration, 0, 2);
                    }


                    origin.SpeedStats.WeakBulletSpeed = EditorGUILayout.FloatField("WeakBulletSpeed", origin.SpeedStats.WeakBulletSpeed);
                    origin.SpeedStats.WeakBulletSpeedV = EditorGUILayout.Vector2Field("WeakBulletSpeedV", origin.SpeedStats.WeakBulletSpeedV);

                    origin.SpeedStats.StrongBulletSpeed = EditorGUILayout.FloatField("StrongBulletSpeed", origin.SpeedStats.StrongBulletSpeed);
                    origin.SpeedStats.StrongBulletSpeedV = EditorGUILayout.Vector2Field("StrongBulletSpeedV", origin.SpeedStats.StrongBulletSpeedV);

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            ShowDamage = EditorGUILayout.Foldout(ShowDamage, "Damage_________________________________________");
            if (ShowDamage)
            {
                origin.DamageStats.BaseDamage = EditorGUILayout.FloatField("BaseDamage", origin.DamageStats.BaseDamage);
                origin.ReflectedAttack = (ScriptableObjectAttackBase)EditorGUILayout.ObjectField("ReflectedAttack", origin.ReflectedAttack, typeof(ScriptableObjectAttackBase), false);

                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();
                ShowAttackSo = EditorGUILayout.Foldout(ShowAttackSo, "Attacks_________________________________________");
                if (ShowAttackSo)
                {
                    var list = origin.attackSequences;
                    int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Number of attackSequences", list.Count));
                    while (newCount < list.Count)
                        list.RemoveAt(list.Count - 1);
                    while (newCount > list.Count)
                        list.Add(null);
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < list.Count; i++)
                    {
                        DrawAttackSequence(origin.attackSequences[i]);
                    }
                    EditorGUI.indentLevel--;


                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    var listCatk = origin._CurrentAttackTypeInfo;
                    newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Number of Currents Atk", listCatk.Count));
                    while (newCount < listCatk.Count)
                        listCatk.RemoveAt(listCatk.Count - 1);
                    while (newCount > listCatk.Count)
                        listCatk.Add(null);
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < listCatk.Count; i++)
                    {
                        origin._CurrentAttackTypeInfo[i] = (ScriptableObjectAttackBase)EditorGUILayout.ObjectField("Atk " + i, origin._CurrentAttackTypeInfo[i], typeof(ScriptableObjectAttackBase), false);
                    }
                    EditorGUI.indentLevel--;
                }


                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }


        EditorGUILayout.Space();
        ShowMisc = EditorGUILayout.Foldout(ShowMisc, "Misc_________________________________________");
        if (ShowMisc)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            var list = origin.DeathDrops;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of death drops", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(new DeathDropInfoScript());
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                DrawDeathDrop(origin.DeathDrops[i]);
            }
            EditorGUI.indentLevel--;
            origin.CharacterRespawnLengthNew = EditorGUILayout.FloatField("CharacterRespawnLength", origin.CharacterRespawnLengthNew);

            origin.ArrivingParticles = (ParticlesType)EditorGUILayout.EnumPopup("ArrivingParticles", origin.ArrivingParticles);


            origin.OverrideBaseLevelHueScaleInfo = EditorGUILayout.Toggle("Override Base Scaling and Hueing", origin.OverrideBaseLevelHueScaleInfo);
            if (origin.OverrideBaseLevelHueScaleInfo)
            {
                if (origin.characterLevelHueScaleInfos == null || origin.characterLevelHueScaleInfos.Length != 10)
                {
                    origin.characterLevelHueScaleInfos = new CharLevelHueScaleInfo[] { CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal, CharLevelHueScaleInfo.normal };
                }
                for (int i = 1; i < origin.characterLevelHueScaleInfos.Length + 1; i++)
                {
                    DrawCharLevelHueScaleInfo("Level " + i.ToString() + " Settings", ref origin.characterLevelHueScaleInfos[i - 1]);
                }
            }

            var listPassiveSkill = origin.PassiveSkills.ToList();
            newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of PassiveSkills", listPassiveSkill.Count));
            while (newCount < listPassiveSkill.Count)
                listPassiveSkill.RemoveAt(listPassiveSkill.Count - 1);
            while (newCount > listPassiveSkill.Count)
                listPassiveSkill.Add(null);
            EditorGUI.indentLevel++;
            for (int i = 0; i < listPassiveSkill.Count; i++)
            {
                listPassiveSkill[i] = (ScriptableObjectPassiveSkill)EditorGUILayout.ObjectField("PassiveSkill " + i, listPassiveSkill[i], typeof(ScriptableObjectPassiveSkill), false);
            }
            EditorGUI.indentLevel--;
            origin.PassiveSkills = new ScriptableObjectPassiveSkill[listPassiveSkill.Count];
            listPassiveSkill.CopyTo(origin.PassiveSkills);

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        ShowControl = EditorGUILayout.Foldout(ShowControl, "Control_________________________________________");
        if (ShowControl)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;


            var list = origin.PlayerController;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Player controllers", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(ControllerType.None);
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                origin.PlayerController[i] = (ControllerType)EditorGUILayout.EnumPopup("PlayerController" + i, origin.PlayerController[i]);
            }
            EditorGUI.indentLevel--;

            origin.IsBornOfWave = EditorGUILayout.Toggle("IsBornOfWave", origin.IsBornOfWave);
            origin.IsSummon = EditorGUILayout.Toggle("IsSummon", origin.IsSummon);

            ShowBehaviour = EditorGUILayout.Foldout(ShowBehaviour, "Behaviour_________________________________________");
            if (ShowBehaviour)
            {
                origin.Behaviour.InputBehaviour = (InputBehaviourType)EditorGUILayout.EnumPopup("InputBehaviour", origin.Behaviour.InputBehaviour);
                origin.Behaviour.MovementActionN = (MovementActionType)EditorGUILayout.EnumPopup("MovementAction", origin.Behaviour.MovementActionN);
                origin.Behaviour.DeathBehaviour = (DeathBehaviourType)EditorGUILayout.EnumPopup("DeathBehaviour", origin.Behaviour.DeathBehaviour);

                origin.Death_UseSound = EditorGUILayout.Toggle("Use Death Sound", origin.Death_UseSound);
                ShowDeathExplosionInfo = EditorGUILayout.Foldout(ShowDeathExplosionInfo, "Death Explosion Setup_____________________");
                if (ShowDeathExplosionInfo)
                {
                    EditorGUI.indentLevel++;
                    origin.DeathExplosion_OverrideParticles = (ParticlesType)EditorGUILayout.EnumPopup("Override Particles", origin.DeathExplosion_OverrideParticles);
                    origin.DeathExplosion_Delay = EditorGUILayout.FloatField("Delay Before Explosion", origin.DeathExplosion_Delay);
                    origin.DeathDisableing_Delay = EditorGUILayout.FloatField("Delay DeathDisableing_Delay", origin.DeathDisableing_Delay);
                    EditorGUI.indentLevel--;
                }
            }

            var listEvent = origin.CharacterEvents;
            newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Event", listEvent.Count));
            while (newCount < listEvent.Count)
                listEvent.RemoveAt(listEvent.Count - 1);
            while (newCount > listEvent.Count)
                listEvent.Add(new CharacterEvent());
            EditorGUI.indentLevel++;
            for (int i = 0; i < listEvent.Count; i++)
            {
                DrawEvent(origin.CharacterEvents[i]);
            }
            EditorGUI.indentLevel--;



            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        ShowGridPos = EditorGUILayout.Foldout(ShowGridPos, "Grid Positioning_________________________________________");
        if (ShowGridPos)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            origin.CurrentTilePos = EditorGUILayout.Vector2IntField("CurrentTilePos", origin.CurrentTilePos);
            origin.Side = (TeamSideType)EditorGUILayout.EnumPopup("Side", origin.Side);
            origin.WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSide", origin.WalkingSide);
            origin.Facing = (FacingType)EditorGUILayout.EnumPopup("Facing", origin.Facing);

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();
        ShowLayering = EditorGUILayout.Foldout(ShowLayering, "Layering_________________________________________");
        if (ShowLayering)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            origin.UseLayeringSystem = EditorGUILayout.Toggle("UseLayeringSystem", origin.UseLayeringSystem);

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

        }
        if (!Application.isPlaying)
        {
            origin.SwappableBases = SwappableBases;
        }

        if (origin.RelationshipList.Count > 0)
        {
            foreach (RelationshipClass item in origin.RelationshipList)
            {
                if (item.name != item.CharacterId.ToString())
                {
                    item.name = item.CharacterId.ToString();
                }
                if (item.CharOwnerId != origin.CharacterID)
                {
                    item.CharOwnerId = origin.CharacterID;
                }
            }
        }

        if (origin.transform.GetComponentsInChildren<Transform>().Where(r => r.name == "Head").ToList().Count == 0)
        {
            GameObject head = Instantiate(new GameObject(), origin.transform);
            head.name = "Head";
            origin.Head = head.transform;
        }


        if (GUILayout.Button("Set Character Default Shaders"))
        {
            MeshRenderer rend = origin.gameObject.GetComponent<MeshRenderer>();
            if (rend == null)
            {
                Debug.LogError("No Mesh Renderer for Char, Cannot Set Default Char Shader");
            }
            else
            {
                Shader thaShader = Shader.Find("Universal Render Pipeline/2D/Spine/Sprite");
                if (thaShader == null)
                {
                    Debug.LogError("Universal Render Pipeline/2D/Spine/Sprite not found! Make sure you have the right packages installed and are on the right branch!!!!");
                }
                else
                {
                    rend.sharedMaterial.shader = thaShader;
                    rend.sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
                    rend.sharedMaterial.DisableKeyword("_ADDITIVEBLEND");
                    rend.sharedMaterial.DisableKeyword("_ADDITIVEBLEND_SOFT");
                    rend.sharedMaterial.DisableKeyword("_MULTIPLYBLEND");
                    rend.sharedMaterial.DisableKeyword("_MULTIPLYBLEND_X2");
                    rend.sharedMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    rend.sharedMaterial.EnableKeyword("_COLOR_ADJUST");
                    rend.sharedMaterial.SetColor("_OverlayColor", new Color(1f, 1f, 1f, 0f));
                }
            }
        }
        AddB_Base(origin);
        EditorUtility.SetDirty(origin);
    }


    private void AddB_Base(CharacterInfoScript origin)
    {
        if (Application.isPlaying)
        {
            return;
        }
        //HealthStats
        origin.HealthStats.B_Health = origin.HealthStats.Health;
        origin.HealthStats.B_Armour = origin.HealthStats.Armour;

        //SpeedStats
        origin.SpeedStats.B_BaseSpeed = origin.SpeedStats.BaseSpeed;
        origin.SpeedStats.B_MovementSpeed = origin.SpeedStats.MovementSpeed;
        origin.SpeedStats.B_MovementTime = origin.SpeedStats.MovementTime;
        origin.SpeedStats.B_LeaveAnimSpeed = origin.SpeedStats.LeaveAnimSpeed;
        origin.SpeedStats.B_ArriveAnimSpeed = origin.SpeedStats.ArriveAnimSpeed;
        origin.SpeedStats.B_WeakBulletSpeed = origin.SpeedStats.WeakBulletSpeed;
        origin.SpeedStats.B_WeakBulletSpeedV = origin.SpeedStats.WeakBulletSpeedV;
        origin.SpeedStats.B_StrongBulletSpeed = origin.SpeedStats.StrongBulletSpeed;
        origin.SpeedStats.B_StrongBulletSpeedV = origin.SpeedStats.StrongBulletSpeedV;
        origin.SpeedStats.B_AttackLoopDuration = origin.SpeedStats.AttackLoopDuration;
        //DamageStats
        origin.DamageStats.B_BaseDamage = origin.DamageStats.BaseDamage;

    }


    void DrawAttackSequence(AttackSequence atkS)
    {
        atkS.rename = EditorGUILayout.Toggle("rename", atkS.rename);
        if (atkS.rename)
        {
            atkS.newName = EditorGUILayout.TextField("newName", atkS.newName);
        }

        var listgroupedAttacks = atkS.groupedAttacks;
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of groupedAttacks", listgroupedAttacks.Count));
        while (newCount < listgroupedAttacks.Count)
            listgroupedAttacks.RemoveAt(listgroupedAttacks.Count - 1);
        while (newCount > listgroupedAttacks.Count)
            listgroupedAttacks.Add(null);
        EditorGUI.indentLevel++;
        for (int i = 0; i < listgroupedAttacks.Count; i++)
        {
            atkS.groupedAttacks[i] = (ScriptableObjectAttackBase)EditorGUILayout.ObjectField("atk " + i, atkS.groupedAttacks[i], typeof(ScriptableObjectAttackBase), false);
        }
        EditorGUI.indentLevel--;

        atkS.triggerType = (TriggerType)EditorGUILayout.EnumPopup("triggerType", atkS.triggerType);

        atkS.StatToCheck = (StatsCheckType)EditorGUILayout.EnumPopup("StatsCheckType", atkS.StatToCheck);
        if (atkS.StatToCheck != StatsCheckType.None)
        {
            atkS.ValueChecker = (ValueCheckerType)EditorGUILayout.EnumPopup("ValueChecker", atkS.ValueChecker);
            if (atkS.ValueChecker == ValueCheckerType.Between)
            {
                atkS.InBetween = EditorGUILayout.Vector2Field("InBetween", atkS.InBetween);
            }
            else
            {
                atkS.PercToCheck = EditorGUILayout.FloatField("PercToCheck", atkS.PercToCheck);
            }
        }
    }


    void DrawDeathDrop(DeathDropInfoScript drop)
    {
        drop.powerUp = (ScriptableObjectItemPowerUps)EditorGUILayout.ObjectField("powerup ", drop.powerUp, typeof(ScriptableObjectItemPowerUps), false);
        drop.deathDropType = (DeathDropTypes)EditorGUILayout.EnumPopup("deathDropType", drop.deathDropType);
        if (drop.deathDropType == DeathDropTypes.Throw)
        {
            drop.throwDuration = EditorGUILayout.FloatField("throwDuration", drop.throwDuration);
            drop.throwDelay = EditorGUILayout.FloatField("throwDelay", drop.throwDelay);
            drop.overrideDeathParticles = EditorGUILayout.Toggle("overrideDeathParticles", drop.overrideDeathParticles);
            if (drop.overrideDeathParticles)
            {
                drop.throwParticles = (GameObject)EditorGUILayout.ObjectField("throwParticles ", drop.throwParticles, typeof(GameObject), false);
            }
        }
    }

    void DrawEvent(CharacterEvent cEvent)
    {
        cEvent.Name = EditorGUILayout.TextField("Name", cEvent.Name);
        cEvent.HappenMultipleTimes = EditorGUILayout.Toggle("HappenMultipleTimes", cEvent.HappenMultipleTimes);

        /*   var listTriggers = cEvent.Triggers;
           int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of triggers", listTriggers.Count));
           while (newCount < listTriggers.Count)
               listTriggers.RemoveAt(listTriggers.Count - 1);
           while (newCount > listTriggers.Count)
               listTriggers.Add(new CharacterEventTrigger());
           EditorGUI.indentLevel++;
           for (int i = 0; i < listTriggers.Count; i++)
           {
               DrawTriggers(cEvent.Triggers[i]);
           }
           EditorGUI.indentLevel--;


           var listAction = cEvent.Actions;
           newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of triggers", listAction.Count));
           while (newCount < listAction.Count)
               listAction.RemoveAt(listAction.Count - 1);
           while (newCount > listAction.Count)
               listAction.Add(new CharacterEventAction());
           EditorGUI.indentLevel++;
           for (int i = 0; i < listAction.Count; i++)
           {
               DrawAction(cEvent.Actions[i]);
           }*/
        EditorGUI.indentLevel--;
    }





    void DrawTriggers(CharacterEventTrigger trigger)
    {
        trigger.triggerCheckType = (TriggerCheckType)EditorGUILayout.EnumPopup("triggerCheckType", trigger.triggerCheckType);
        trigger.triggerType = (CharacterEventTriggerType)EditorGUILayout.EnumPopup("triggerType", trigger.triggerType);

        if(trigger.triggerType == CharacterEventTriggerType.Forme)
        {
            trigger.formeIDToMatch = EditorGUILayout.TextField("formeIDToMatch", trigger.formeIDToMatch);
        }
        else if (trigger.triggerType == CharacterEventTriggerType.Health)
        {
            trigger.healhCompareType = (CompareType)EditorGUILayout.EnumPopup("healhCompareType", trigger.healhCompareType);

            trigger.healthComparisonValue = EditorGUILayout.FloatField("healthComparisonValue", trigger.healthComparisonValue);
        }
        else if (trigger.triggerType == CharacterEventTriggerType.Health)
        {
            trigger.charEventThatHappened_ID = EditorGUILayout.TextField("charEventThatHappened_ID", trigger.charEventThatHappened_ID);
        }
    }

    void DrawAction(CharacterEventAction cAction)
    {
        cAction.actionType = (CharacterEventActionType)EditorGUILayout.EnumPopup("actionType", cAction.actionType);
        if (cAction.actionType == CharacterEventActionType.Forme_Change)
        {
            cAction.ChangeTo_FormeID = EditorGUILayout.TextField("ChangeTo_FormeID", cAction.ChangeTo_FormeID);
            cAction.ChangedForme_StartingHPPerc = EditorGUILayout.FloatField("ChangedForme_StartingHPPerc", cAction.ChangedForme_StartingHPPerc);

        }
        else if (cAction.actionType == CharacterEventActionType.Health_Set)
        {
            cAction.HealthPercToSetTo = EditorGUILayout.FloatField("HealthPercToSetTo", cAction.HealthPercToSetTo);
        }

    }

    void DrawCharLevelHueScaleInfo(string Title, ref CharLevelHueScaleInfo info)
    {
        EditorGUILayout.LabelField(Title);
        EditorGUI.indentLevel++;
        info.Color = EditorGUILayout.ColorField("Overlay Color", info.Color);
        info.Hue = EditorGUILayout.Slider("Hue", info.Hue, -0.5f, 0.5f);
        info.Saturation = EditorGUILayout.Slider("Hue", info.Saturation, 0f, 2f);
        info.Scale = EditorGUILayout.Vector3Field("Scale", info.Scale);
        EditorGUI.indentLevel--;
    }

    T[] DrawArray<T>(T[] thaArray, string listTitle, string itemTitle = "", int indexAdjustment = 0, Vector2Int sizeMinMax = new Vector2Int()) where T : Object
        => DrawList(thaArray.ToList(), listTitle, itemTitle, indexAdjustment, sizeMinMax).ToArray();
    List<T> DrawList<T>(List<T> thaList, string listTitle, string itemTitle = "", int indexAdjustment = 0, Vector2Int sizeMinMax = new Vector2Int()) where T : Object
    {
        List<T> tempList = thaList;

        int count = 0;
        if (sizeMinMax.x == sizeMinMax.y && sizeMinMax != new Vector2Int())
            EditorGUILayout.LabelField(listTitle);
        else
            count = Mathf.Max(0, EditorGUILayout.DelayedIntField(listTitle, tempList.Count));

        if (sizeMinMax != new Vector2Int())
            count = Mathf.Clamp(count, sizeMinMax.x, sizeMinMax.y);

        while(tempList.Count != count)
        {
            if(count < tempList.Count)
                tempList.RemoveAt(tempList.Count - 1);
            else
                tempList.Add(null);
        }

        EditorGUI.indentLevel++;
        for (int i = 0; i < tempList.Count; i++)
        {
            thaList[i] = (T)EditorGUILayout.ObjectField((itemTitle != "" ? (itemTitle + " ") : "") + (i + indexAdjustment).ToString(), tempList[i], typeof(T), false);
        }

        EditorGUI.indentLevel--;
        return thaList;
    }



    CharSkillsListEditorInfo CharSkillsListInfo;
    ScriptableObjectSkillBase[] GeneralSkillList = new ScriptableObjectSkillBase[0];
    ScriptableObjectSkillBase[] LevelledSkillList = new ScriptableObjectSkillBase[10];
    void DrawAndValidate_UpgradeList(CharacterInfoScript origin)
    {
        if(!CharSkillsListInfo.SetUp)
        CharSkillsListInfo = new CharSkillsListEditorInfo(origin);

        ShowSkills_General = EditorGUILayout.Foldout(ShowSkills_General, "General Skills (Passives & Attacks)");
        if (ShowSkills_General)
        {
            if (GeneralSkillList == null || GeneralSkillList.Length < CharSkillsListInfo.NonLevelled_Skills.Length)
                GeneralSkillList = new ScriptableObjectSkillBase[CharSkillsListInfo.NonLevelled_Skills.Length];

            for (int i = 0; i < CharSkillsListInfo.NonLevelled_Skills.Length; i++)
            {
                GeneralSkillList[i] = CharSkillsListInfo.NonLevelled_Skills[i];
            }
            for (int i = 0; i < GeneralSkillList.Length; i++)
            {
                GeneralSkillList[i] = GeneralSkillList[i] == null || GeneralSkillList.ToList().IndexOf(GeneralSkillList[i]) == i ? GeneralSkillList[i] : null;
            }

            GeneralSkillList = DrawArray(GeneralSkillList, "General Skills Num", "General Skill");

            CharSkillsListInfo.NonLevelled_Skills = GeneralSkillList;
        }

        ShowSkills_Levelled = EditorGUILayout.Foldout(ShowSkills_Levelled, "Levelled Skills (Passives & Attacks)");
        if (ShowSkills_Levelled)
        {
            //Sort into the correct level
            //LevelledSkillList = new ScriptableObjectSkillBase[10];
            //for (int i = 0; i < LevelledSkillList.Length; i++)
            //{
            //    LevelledSkillList[i] = CharSkillsListInfo.SkillsLevelsList[i];//CharSkillsListInfo.Levelled_Skills.Where(r => r != null && origin.LevelRequiredForSkill(r) == (i + 1)).FirstOrDefault();
            //}
            LevelledSkillList = CharSkillsListInfo.SkillsLevelsList;
            LevelledSkillList = DrawArray(LevelledSkillList, "Levelled Skills ", "Level", +1, new Vector2Int(10,10));

            CharSkillsListInfo.Levelled_Skills = LevelledSkillList;
            CharSkillsListInfo.SkillsLevelsList = LevelledSkillList;
        }

        if (GUILayout.Button("Apply Changes"))
        {
            //for (int i = 0; i < origin.SkillLevels.Length; i++)
            //{
            //    origin.SkillLevels[i] = CharSkillsListInfo.Levelled_Skills.Where(r => origin.LevelRequiredForSkill(r) == i).FirstOrDefault();
            //}
            origin.SkillLevels = CharSkillsListInfo.SkillsLevelsList;
            origin._CurrentAttackTypeInfo = CharSkillsListInfo.Attack_Skills.ToList();
            origin.PassiveSkills = CharSkillsListInfo.Passive_Skills;
            CharSkillsListInfo.Clear();
        }
    }
}

public struct CharSkillsListEditorInfo
{
    List<ScriptableObjectSkillBase> _tempL;

    public ScriptableObjectSkillBase[] Skills;

    public ScriptableObjectSkillBase[] SkillsLevelsList;
    public int LevelRequiredForSkill(ScriptableObjectSkillBase skill)
    {
        for (int i = 0; i < SkillsLevelsList.Length; i++)
        {
            if (skill == SkillsLevelsList[i])
                return i + 1;
        }
        return 0;
    }

    public ScriptableObjectSkillBase[] NonLevelled_Skills
    {

        get //=> Skills.Where(r => r == null || r.MinLevel == 0).ToArray();
        {
            List<ScriptableObjectSkillBase> res = new List<ScriptableObjectSkillBase>();
            for (int i = 0; i < Skills.Length; i++)
            {
                if (Skills[i] == null || LevelRequiredForSkill(Skills[i]) == 0)
                    res.Add(Skills[i]);
            }
            return res.ToArray();
        }
        set
        {
            _tempL = new List<ScriptableObjectSkillBase>();
            for (int i = 0; i < value.Length; i++)
            {
                //if (Levelled_Skills.Contains(value[i]))
                //    Debug.LogError("Tried to add an item to the generic skills that currently exists in the levelled skills");
                _tempL.Add(value[i]);
                if (_tempL.Last() != null)
                {
                    //_tempL.Last().MinLevel = 0;
                    //EditorUtility.SetDirty(_tempL.Last());
                }
            }

            Skills = _tempL.ToArray().Concat(Levelled_Skills).ToArray();
        }
    }
    public ScriptableObjectSkillBase[] Levelled_Skills
    {
        get //=> Skills.Where(r => r != null && r.MinLevel != 0).ToArray();
        {
            List<ScriptableObjectSkillBase> res = new List<ScriptableObjectSkillBase>();
            for (int i = 0; i < Skills.Length; i++)
            {
                if (Skills[i] != null && LevelRequiredForSkill(Skills[i]) != 0)
                    res.Add(Skills[i]);
            }
            return res.ToArray();
        }
        set
        {
            _tempL = new List<ScriptableObjectSkillBase>();
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == null)
                    continue;
                //if (Levelled_Skills.Contains(value[i]))
                //    Debug.LogError("Tried to add an item to the levelled skills that currently exists in the generic skills");
                if (_tempL.Contains(value[i]))
                    continue;
                
                _tempL.Add(value[i]);
                //_tempL.Last().MinLevel = (i + 1);
                //EditorUtility.SetDirty(_tempL.Last());
            }

            Skills = _tempL.ToArray().Concat(NonLevelled_Skills).ToArray();
        }
    }
        
        

    public ScriptableObjectAttackBase[] Attack_Skills
    {
        get
        {
            ScriptableObjectSkillBase[] source = Skills.Where(r => r != null && r.GetType() == typeof(ScriptableObjectAttackBase)).ToArray();
            ScriptableObjectAttackBase[] converted = new ScriptableObjectAttackBase[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                converted[i] = (ScriptableObjectAttackBase)source[i];
            }
            return converted;
        }
    }
    public ScriptableObjectPassiveSkill[] Passive_Skills
    {
        get
        {
            ScriptableObjectSkillBase[] source = Skills.Where(r => r != null && r.GetType() == typeof(ScriptableObjectPassiveSkill)).ToArray();
            ScriptableObjectPassiveSkill[] converted = new ScriptableObjectPassiveSkill[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                converted[i] = (ScriptableObjectPassiveSkill)source[i];
            }
            return converted;
        }
    }

    public CharSkillsListEditorInfo(CharacterInfoScript charInfo)
    {
        //List<int> hashList = new List<int>();
        List<ScriptableObjectSkillBase> skillsList = new List<ScriptableObjectSkillBase>();
        for (int i = 0; i < charInfo._CurrentAttackTypeInfo.Count; i++)
        {
            skillsList.Add(charInfo._CurrentAttackTypeInfo[i]);
        //    hashList.Add(skillsList.Last() == null ? 0 : skillsList.Last().GetHashCode());
        }
        for (int i = 0; i < charInfo.PassiveSkills.Length; i++)
        {
            skillsList.Add(charInfo.PassiveSkills[i]);
        //    hashList.Add(skillsList.Last() == null ? 0 : skillsList.Last().GetHashCode());
        }
        Skills = skillsList.ToArray();


        if (charInfo.SkillLevels.Length != 10)
            charInfo.SkillLevels = new ScriptableObjectSkillBase[10];
        skillsList = new List<ScriptableObjectSkillBase>();
        for (int i = 0; i < charInfo.SkillLevels.Length; i++)
        {
            skillsList.Add(charInfo.SkillLevels[i]);
        }
        SkillsLevelsList = skillsList.ToArray();

        _tempL = new List<ScriptableObjectSkillBase>();
        SetUp = true;
        //HashCodes = hashList.ToArray();
    }

    public bool SetUp;
    public void Clear() => SetUp = false;


    //public int[] HashCodes;
    //public bool IsEqual(CharSkillsListEditorInfo compareItem)
    //{
    //    for (int i = 0; i < HashCodes.Length; i++)
    //    {
    //        if (!compareItem.HashCodes.Contains(HashCodes[i]))
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}
}