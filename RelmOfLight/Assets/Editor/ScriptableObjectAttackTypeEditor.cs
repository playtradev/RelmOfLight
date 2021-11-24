using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PlaytraGamesLtd;
using UnityEngine.Video;

[CustomEditor(typeof(ScriptableObjectAttackBase))]
public class ScriptableObjectAttackTypeEditor : Editor
{

    ScriptableObjectAttackBase origin;
    public GameObject ChargingActivationPs;
    public GameObject ChargingLoopPs;
    public GameObject PlaceHolder;
    public ScriptableObjectAttackEffect SlowDown;
    bool showExplosionList = false;
    List<bool> showDirtyBombEffectsList = new List<bool>();
    BattleFieldAttackType bfat;
    BulletBehaviourInfoClassOnBattleFieldClass copyTo = null;

    bool ParticlesVisisble = false;
    bool GeneralVisisble = false;
    bool ElementalVisisble = false;
    bool TimeEffectVisible = false;
    bool SlowDownClassVisisble = false;
    bool DamageVisisble = false;
    bool BehaviourVisisble = false;
    bool BulletVisisble = false;
    bool BulletGrenadeVisisble = false;
    bool BulletPoopingVisisble = false;
    bool BulletHomingVisisble = false;
    bool TrajectoriesVisisble = false;

    public override void OnInspectorGUI()
    {
        origin = (ScriptableObjectAttackBase)target;


        #region General

        GeneralVisisble = EditorGUILayout.Foldout(GeneralVisisble, "General Info ---------------------------------------------");
        if (GeneralVisisble)
        {
            EditorGUI.indentLevel++;
            origin.AttackDisplayName = EditorGUILayout.TextField("Attack Display Name", origin.AttackDisplayName);

            origin.IsMaskAttack = EditorGUILayout.Toggle("IsMaskAttack", origin.IsMaskAttack);

            origin.SkillID = EditorGUILayout.TextField("SKILL GROUP ID", origin.SkillID, new GUIStyle("Used to determine which skill level up group this skill is part of, they should share the same Skill ID"));

            origin.AttackDescription = EditorGUILayout.TextField("Attack Description", origin.AttackDescription);

            origin.AttackVideoClip = (VideoClip)EditorGUILayout.ObjectField("Attack Video Demo", origin.AttackVideoClip, typeof(VideoClip), allowSceneObjects: false);

            origin.AttackIcon = (Sprite)EditorGUILayout.ObjectField("Attack Icon", origin.AttackIcon, typeof(Sprite), allowSceneObjects: false);

            origin.attackDisplayType = (ScriptableObjectAttackBase.AttackDisplayType)EditorGUILayout.EnumPopup("Attack Display Type", origin.attackDisplayType);

            //origin.IsPlayerAttack = EditorGUILayout.Toggle("Is Player Attack", origin.IsPlayerAttack);


            origin.AnimToFireOnHit = (CharacterAnimationStateType)EditorGUILayout.EnumPopup("AnimToFireOnHit", origin.AnimToFireOnHit);

            origin.AttackAnim = (AttackAnimType)EditorGUILayout.EnumPopup("AttackAnim", origin.AttackAnim);

            origin.AttackInput = (AttackInputType)EditorGUILayout.EnumPopup("AttackInput", origin.AttackInput);

            origin.TilesAtk.RangeLevel = EditorGUILayout.Vector2IntField("Range Levels", origin.TilesAtk.RangeLevel);

            origin.TilesAtk.AtkType = (BattleFieldAttackType)EditorGUILayout.EnumPopup("BattleFieldAttackType", origin.TilesAtk.AtkType);
            if (origin.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf || origin.TilesAtk.AtkType == BattleFieldAttackType.OnTarget)
            {
                origin.TilesAtk.CanAffectBothSide = EditorGUILayout.Toggle("CanAffectBothSide", origin.TilesAtk.CanAffectBothSide);
            }

            if ((origin.AttackInput == AttackInputType.Weak || origin.AttackInput == AttackInputType.Strong) && origin.TilesAtk.AtkType == BattleFieldAttackType.OnTarget)
            {
                origin.TilesAtk.UseBoundaries = EditorGUILayout.Toggle("UseBoundaries", origin.TilesAtk.UseBoundaries);
            }
            else
            {
                origin.TilesAtk.UseBoundaries = false;
            }

            EditorGUILayout.Space();
            DamageVisisble = EditorGUILayout.Foldout(DamageVisisble, "Damage Info ------------------------------------------");
            if (DamageVisisble)
            {
                EditorGUI.indentLevel++;
                origin._DamageMultiplier = EditorGUILayout.Vector2Field("Damage Multiplier", origin._DamageMultiplier);
                EditorGUI.indentLevel--;
            }




            EditorGUILayout.Space();
            ElementalVisisble = EditorGUILayout.Foldout(ElementalVisisble, "Elemental Info ------------------------------------");
            if (ElementalVisisble)
            {
                EditorGUI.indentLevel++;
                origin.UseAttackerElement = EditorGUILayout.Toggle("UseAttackerElement", origin.UseAttackerElement);
                if (origin.UseAttackerElement)
                {
                    var list = origin.AttackElements;
                    int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Elements", list.Count));
                    while (newCount < list.Count)
                        list.RemoveAt(list.Count - 1);
                    while (newCount > list.Count)
                        list.Add(ElementalType.Neutral);

                    for (int i = 0; i < list.Count; i++)
                    {
                        origin.AttackElements[i] = (ElementalType)EditorGUILayout.EnumPopup("AttackElements N " + i, origin.AttackElements[i]);
                    }
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            TimeEffectVisible = EditorGUILayout.Foldout(TimeEffectVisible, "Time Effect Info ------------------------------------");
            if (TimeEffectVisible)
            {
                EditorGUI.indentLevel++;
                origin.EffectTimeOnImpact = EditorGUILayout.Toggle("Effect Time On Impact", origin.EffectTimeOnImpact);
                if (origin.EffectTimeOnImpact)
                {
                    SlowDownClassVisisble = EditorGUILayout.Foldout(SlowDownClassVisisble, "SlowDown Info ------------------------------------");
                    if (SlowDownClassVisisble)
                    {
                        origin.SlowDownOnHit.TimeEffect = EditorGUILayout.FloatField("SlowDownSpeed", origin.SlowDownOnHit.TimeEffect);
                        origin.SlowDownOnHit.DurationOfTimeEffect = EditorGUILayout.FloatField("Duration of Time Effect", origin.SlowDownOnHit.DurationOfTimeEffect);
                        origin.SlowDownOnHit.TimeEffectDelay = EditorGUILayout.FloatField("Time Effect Delay", origin.SlowDownOnHit.TimeEffectDelay);
                        origin.SlowDownOnHit.TimeEffectChildExplosion = EditorGUILayout.Toggle("Effect Time On Child Explosions", origin.SlowDownOnHit.TimeEffectChildExplosion);
                        origin.SlowDownOnHit.ApplyImpactSlowToAttacker = EditorGUILayout.Toggle("Apply Impact Slow To Attacker", origin.SlowDownOnHit.ApplyImpactSlowToAttacker);
                    }
                }
            }

            EditorGUILayout.Space();
            BehaviourVisisble = EditorGUILayout.Foldout(BehaviourVisisble, "Behaviour Info ------------------------------------------");
            if (BehaviourVisisble)
            {
                EditorGUI.indentLevel++;
                origin.AttackTargetSide = (AttackTargetSideType)EditorGUILayout.EnumPopup("AttackTargetSide", origin.AttackTargetSide);

                EditorGUILayout.Space();

                origin.useCustomChargeTime_Reset = EditorGUILayout.Toggle("useCustomChargeTime", origin.useCustomChargeTime_Reset);
                if (origin.useCustomChargeTime_Reset)
                {
                    origin.customChargeTime = EditorGUILayout.FloatField("customChargeTime", origin.customChargeTime);
                }


                EditorGUILayout.LabelField("Facing ---->>");
                DrawNewFieldOfView(new Vector2Int(-6, 7), new Vector2Int(-3, 7));



                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }
        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #region Particles
        ParticlesVisisble = EditorGUILayout.Foldout(ParticlesVisisble, "Particles Info -------------------------------------------");
        if (ParticlesVisisble)
        {
            origin.OverridePsHit = EditorGUILayout.Toggle("OverridePsHit", origin.OverridePsHit);
            if (origin.OverridePsHit)
            {
                origin._HitParticlesT = (HitParticlesType)EditorGUILayout.EnumPopup("HitParticlesT", origin._HitParticlesT);
                if (origin._HitParticlesT == HitParticlesType.Resized)
                {
                    origin._HitResizeMultiplier = EditorGUILayout.FloatField("HitResizeMultiplier", origin._HitResizeMultiplier);
                }
            }


            origin.OverridePsInput = EditorGUILayout.Toggle("OverridePsInput", origin.OverridePsInput);
            if (origin.OverridePsInput)
            {
                origin._ParticlesInput = (AttackParticlesInputType)EditorGUILayout.EnumPopup("ParticlesInput", origin._ParticlesInput);
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.Space();


            if (origin.Particles.Right.Cast != null)
            {
                origin.Particles.Right.CastAddress = origin.Particles.Right.Cast.name;
                origin.Particles.Right.Cast = null;
            }

            if (origin.Particles.Right.Cast == null && string.IsNullOrEmpty(origin.Particles.Right.CastAddress))
            {
                origin.Particles.Right.Cast = (GameObject)EditorGUILayout.ObjectField("Right    Cast", origin.Particles.Right.Cast, typeof(GameObject), false);
            }

            if (!string.IsNullOrEmpty(origin.Particles.Right.CastAddress))
            {
                origin.Particles.Right.CastAddress = EditorGUILayout.TextField("Right    Cast", origin.Particles.Right.CastAddress);
            }

            if (origin.Particles.Right.Bullet != null)
            {
                origin.Particles.Right.BulletAddress = origin.Particles.Right.Bullet.name;
                origin.Particles.Right.Bullet = null;
            }

            if (origin.Particles.Right.Bullet == null && string.IsNullOrEmpty(origin.Particles.Right.BulletAddress))
            {
                origin.Particles.Right.Bullet = (GameObject)EditorGUILayout.ObjectField("Right    Bullet", origin.Particles.Right.Bullet, typeof(GameObject), false);
            }

            if (!string.IsNullOrEmpty(origin.Particles.Right.BulletAddress))
            {
                origin.Particles.Right.BulletAddress = EditorGUILayout.TextField("Right    Bullet", origin.Particles.Right.BulletAddress);
            }

            if (origin.Particles.Right.Hit != null)
            {
                origin.Particles.Right.HitAddress = origin.Particles.Right.Hit.name;
                origin.Particles.Right.Hit = null;
            }

            if (origin.Particles.Right.Hit == null && string.IsNullOrEmpty(origin.Particles.Right.HitAddress))
            {
                origin.Particles.Right.Hit = (GameObject)EditorGUILayout.ObjectField("Right    Hit", origin.Particles.Right.Hit, typeof(GameObject), false);
            }

            if (!string.IsNullOrEmpty(origin.Particles.Right.HitAddress))
            {
                origin.Particles.Right.HitAddress = EditorGUILayout.TextField("Right    Hit", origin.Particles.Right.HitAddress);
            }

            EditorGUI.indentLevel--;
        }
        #endregion


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #region Bullet
        BulletVisisble = EditorGUILayout.Foldout(BulletVisisble, "Bullet Info -------------------------------------------");
        if (BulletVisisble)
        {
            EditorGUI.indentLevel++;

            var listBulletT = origin._BulletT;
            int newCountBulletT = Mathf.Max(0, EditorGUILayout.IntField("Bullet Types", origin._BulletT.Count));
            while (newCountBulletT < listBulletT.Count)
                listBulletT.RemoveAt(listBulletT.Count - 1);
            while (newCountBulletT > listBulletT.Count)
                listBulletT.Add(BulletType.Base);

            for (int i = 0; i < listBulletT.Count; i++)
            {
                EditorGUILayout.Space();
                origin._BulletT[i] = (BulletType)EditorGUILayout.EnumPopup("BulletType " + i, origin._BulletT[i]);

                if (origin._BulletT[i] == BulletType.Grenade)
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;

                    BulletGrenadeVisisble = EditorGUILayout.Foldout(BulletGrenadeVisisble, "Grenade Info ------------------------------------------");
                    if (BulletGrenadeVisisble)
                    {
                        EditorGUILayout.Space();


                        origin.Grenade_ExplosionDelay = EditorGUILayout.FloatField("ExplosionDelay", origin.Grenade_ExplosionDelay);
                        origin.Grenade_Defensible = EditorGUILayout.Toggle("CanBeDefended", origin.Grenade_Defensible);
                        origin.Grenade_CanBeDestroyed = EditorGUILayout.Toggle("CanBeDestroyed", origin.Grenade_CanBeDestroyed);
                        if (origin.Grenade_CanBeDestroyed) origin.Grenade_Health = EditorGUILayout.FloatField("Health", origin.Grenade_Health);
                        origin.Grenade_ExplosionDamageMultiplier = EditorGUILayout.FloatField("ExplosionDamageMultiplier", origin.Grenade_ExplosionDamageMultiplier);

                        origin.Grenade_OverrideBulletEffects = EditorGUILayout.Toggle("OverrideBulletPS", origin.Grenade_OverrideBulletEffects);
                        if (origin.Grenade_OverrideBulletEffects)
                        {
                            origin.Grenade_ExplosionPS = (ParticlesType)EditorGUILayout.EnumPopup("Grenade_ExplosionPS", origin.Grenade_ExplosionPS);
                            origin.Grenade_ObjectPS = (ParticlesType)EditorGUILayout.EnumPopup("Grenade_ObjectPS", origin.Grenade_ObjectPS);
                        }

                        var list = origin.Grenade_ExplosionTiles;
                        int newCount = Mathf.Max(1, EditorGUILayout.IntField("Explosion Tiles Count", list.Count));
                        while (newCount < list.Count)
                            list.RemoveAt(list.Count - 1);
                        while (newCount > list.Count)
                            list.Add(Vector2Int.zero);
                        while (newCount < list.Count) list.RemoveAt(list.Count - 1);
                        while (newCount > list.Count) list.Add(Vector2Int.zero);
                        while (list.Count < origin.Grenade_DirtyBombEffects.Count) origin.Grenade_DirtyBombEffects.RemoveAt(origin.Grenade_DirtyBombEffects.Count - 1);
                        while (list.Count > origin.Grenade_DirtyBombEffects.Count) origin.Grenade_DirtyBombEffects.Add(new DirtyBombEffectsList());
                        while (list.Count < showDirtyBombEffectsList.Count) showDirtyBombEffectsList.RemoveAt(showDirtyBombEffectsList.Count - 1);
                        while (list.Count > showDirtyBombEffectsList.Count) showDirtyBombEffectsList.Add(false);
                        showExplosionList = EditorGUILayout.Foldout(showExplosionList, "Explosion Tiles (" + list.Count.ToString() + ")");
                        if (showExplosionList)
                        {
                            for (int a = 0; a < list.Count; a++)
                            {
                                origin.Grenade_ExplosionTiles[a] = EditorGUILayout.Vector2IntField("Explosion On " + origin.Grenade_ExplosionTiles[a].ToString(), origin.Grenade_ExplosionTiles[a]);
                                var effectList = origin.Grenade_DirtyBombEffects[a].list;
                                int effectCount = Mathf.Max(0, EditorGUILayout.IntField("Effects On " + origin.Grenade_ExplosionTiles[a].ToString() + " Count", effectList.Count));
                                while (effectCount < effectList.Count) effectList.RemoveAt(effectList.Count - 1);
                                while (effectCount > effectList.Count) effectList.Add(null);
                                showDirtyBombEffectsList[a] = EditorGUILayout.Foldout(showDirtyBombEffectsList[a], "Effects On " + origin.Grenade_ExplosionTiles[a].ToString() + " (" + effectList.Count.ToString() + ")");
                                if (showDirtyBombEffectsList[a])
                                {
                                    for (int j = 0; j < effectList.Count; j++)
                                    {
                                        origin.Grenade_DirtyBombEffects[a].list[j] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + (j + 1).ToString(), origin.Grenade_DirtyBombEffects[a].list[j], typeof(ScriptableObjectAttackEffect), false);
                                    }
                                }
                                EditorGUILayout.LabelField("====================================");
                            }
                        }
                        EditorGUI.indentLevel--;
                    }

                }



                if (origin.IsHoming)
                {
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel++;
                    BulletHomingVisisble = EditorGUILayout.Foldout(BulletHomingVisisble, "Homing Info ------------------------------------------");
                    if (BulletHomingVisisble)
                    {
                        EditorGUILayout.Space();

                        origin.Homing_TimeTillExpiry = EditorGUILayout.FloatField("TimeTillExpiry", origin.Homing_TimeTillExpiry);
                        origin.Homing_SlowDownMultiplier = EditorGUILayout.FloatField("SlowDownMultiplier", origin.Homing_SlowDownMultiplier);
                        origin.Homing_TurningSpeed = EditorGUILayout.FloatField("TurningSpeed", origin.Homing_TurningSpeed);
                        EditorGUI.indentLevel--;
                    }
                }

                if (origin._BulletT[i] == BulletType.Pooping)
                {
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel++;
                    BulletPoopingVisisble = EditorGUILayout.Foldout(BulletPoopingVisisble, "Pooping Info ------------------------------------------");
                    if (BulletPoopingVisisble)
                    {
                        EditorGUILayout.Space();

                        origin.ChancesOfPooping = EditorGUILayout.FloatField("ChancesOfPooping", origin.ChancesOfPooping);
                        origin.BothSides = EditorGUILayout.Toggle("BothSides", origin.BothSides);
                        if (!origin.BothSides)
                        {
                            origin.InEnemySide = EditorGUILayout.Toggle("InEnemySide", origin.InEnemySide);
                        }
                        origin.isPoopingEffectOnTile = EditorGUILayout.Toggle("isPoopingEffectOnTile", origin.isPoopingEffectOnTile);
                        if (origin.isPoopingEffectOnTile)
                        {
                            origin.PoopingEffectsOnTile.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", origin.PoopingEffectsOnTile.TileAction);
                            if (origin.PoopingEffectsOnTile.TileAction == TileActionType.OverTime)
                            {
                                origin.PoopingEffectsOnTile.HitTime = EditorGUILayout.FloatField("HitTime", origin.PoopingEffectsOnTile.HitTime);
                            }
                            origin.PoopingEffectsOnTile.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("TileParticlesID", origin.PoopingEffectsOnTile.TileParticlesID);
                            origin.PoopingEffectsOnTile.EffectChances = EditorGUILayout.FloatField("EffectChances", origin.PoopingEffectsOnTile.EffectChances);
                            origin.PoopingEffectsOnTile.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", origin.PoopingEffectsOnTile.DurationOnTile);
                            origin.PoopingEffectsOnTile.DurationOnTileV = EditorGUILayout.Vector2Field("DurationOnTileV", origin.PoopingEffectsOnTile.DurationOnTileV);
                            var list = origin.PoopingEffectsOnTile.Effects;
                            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
                            while (newCount < list.Count)
                                list.RemoveAt(list.Count - 1);
                            while (newCount > list.Count)
                                list.Add(null);

                            for (int a = 0; a < list.Count; a++)
                            {
                                origin.PoopingEffectsOnTile.Effects[a] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + a, origin.PoopingEffectsOnTile.Effects[a], typeof(ScriptableObjectAttackEffect), false);
                            }
                        }

                        origin.isPoopingSpawnSummonOnTile = EditorGUILayout.Toggle("isPoopingSpawnSummonOnTile", origin.isPoopingSpawnSummonOnTile);
                        if (origin.isPoopingSpawnSummonOnTile)
                        {
                            DrawSummonInfo(ref origin.PoopingSummonToSpawn);
                        }
                    }

                    EditorGUI.indentLevel--;
                }


            }


            origin.IsHoming = origin._BulletT.Contains(BulletType.Homing);

            EditorGUI.indentLevel--;
        }

        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #region Trajectories

        TrajectoriesVisisble = EditorGUILayout.Foldout(TrajectoriesVisisble, "Trajectories Info -------------------------------------------");
        if (TrajectoriesVisisble)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            if (origin.TilesAtk.AtkType != BattleFieldAttackType.OnItSelf)
            {

                origin.TilesAtk.EffectOnCaster = EditorGUILayout.Toggle("EffectOnCaster", origin.TilesAtk.EffectOnCaster);
                if (origin.TilesAtk.EffectOnCaster)
                {
                    var listEffect = origin.TilesAtk.EffectsOnCaster;
                    int newCountEffect = Mathf.Max(0, EditorGUILayout.DelayedIntField("Effects", origin.TilesAtk.EffectsOnCaster.Count));
                    while (newCountEffect < listEffect.Count)
                        listEffect.RemoveAt(listEffect.Count - 1);
                    while (newCountEffect > listEffect.Count)
                        listEffect.Add(null);
                    EditorGUI.indentLevel++;
                    for (int a = 0; a < listEffect.Count; a++)
                    {

                        origin.TilesAtk.EffectsOnCaster[a] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + a,
                            origin.TilesAtk.EffectsOnCaster[a], typeof(ScriptableObjectAttackEffect), false);
                    }
                }

                copyTo = origin.TilesAtk.BulletTrajectories.Where(r => r.CopyTo).FirstOrDefault();
                origin.TilesAtk.BulletLevel = (BulletLevelType)EditorGUILayout.EnumPopup("Bullet_Level", origin.TilesAtk.BulletLevel);
                origin.TrajectoriesNumber = EditorGUILayout.DelayedIntField("Trajectories", origin.TrajectoriesNumber);

                while (origin.TrajectoriesNumber < origin.TilesAtk.BulletTrajectories.Count && origin.TilesAtk.BulletTrajectories.Count > 0)
                    origin.TilesAtk.BulletTrajectories.RemoveAt(origin.TilesAtk.BulletTrajectories.Count - 1);
                while (origin.TrajectoriesNumber > origin.TilesAtk.BulletTrajectories.Count)
                    origin.TilesAtk.BulletTrajectories.Add(origin.TilesAtk.BulletTrajectories.Count == 0 ? new BulletBehaviourInfoClassOnBattleFieldClass() : new BulletBehaviourInfoClassOnBattleFieldClass(origin.TilesAtk.BulletTrajectories[0]));


                for (int i = 0; i < origin.TrajectoriesNumber; i++)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    origin.TilesAtk.BulletTrajectories[i].InfoVisible = EditorGUILayout.Foldout(origin.TilesAtk.BulletTrajectories[i].InfoVisible, "Trajectory " + i + " Info ------------------------------------------");
                    if (origin.TilesAtk.BulletTrajectories[i].InfoVisible)
                    {
                        EditorGUILayout.Space();

                        origin.TilesAtk.BulletTrajectories[i].Delay = EditorGUILayout.FloatField("Delay", origin.TilesAtk.BulletTrajectories[i].Delay);
                        origin.TilesAtk.BulletTrajectories[i].HasABullet = EditorGUILayout.Toggle("HasABullet", origin.TilesAtk.BulletTrajectories[i].HasABullet);

                        if (origin.TilesAtk.BulletTrajectories[i].HasABullet)
                        {

                            origin.TilesAtk.BulletTrajectories[i].OverrideBulletLevel = EditorGUILayout.Toggle("OverrideBulletLevel", origin.TilesAtk.BulletTrajectories[i].OverrideBulletLevel);
                            if (origin.TilesAtk.BulletTrajectories[i].OverrideBulletLevel)
                            {
                                origin.TilesAtk.BulletTrajectories[i].BulletLevel = (BulletLevelType)EditorGUILayout.EnumPopup("Bullet_Level", origin.TilesAtk.BulletTrajectories[i].BulletLevel);
                            }

                            origin.TilesAtk.BulletTrajectories[i].OverrideBulletInfo = EditorGUILayout.Toggle("Customize Bullet T", origin.TilesAtk.BulletTrajectories[i].OverrideBulletInfo);
                            if (origin.TilesAtk.BulletTrajectories[i].OverrideBulletInfo)
                            {
                                EditorGUI.indentLevel++;

                                var listBulletT = origin.TilesAtk.BulletTrajectories[i]._BulletT;
                                int newCountBulletT = Mathf.Max(0, EditorGUILayout.IntField("Bullet Types", origin.TilesAtk.BulletTrajectories[i]._BulletT.Count));
                                while (newCountBulletT < listBulletT.Count)
                                    listBulletT.RemoveAt(listBulletT.Count - 1);
                                while (newCountBulletT > listBulletT.Count)
                                    listBulletT.Add(BulletType.Base);

                                for (int a = 0; a < listBulletT.Count; a++)
                                {
                                    EditorGUILayout.Space();
                                    origin.TilesAtk.BulletTrajectories[i]._BulletT[a] = (BulletType)EditorGUILayout.EnumPopup("BulletType " + a, origin.TilesAtk.BulletTrajectories[i]._BulletT[a]);


                                    if (origin.TilesAtk.BulletTrajectories[i]._BulletT[a] == BulletType.Grenade)
                                    {
                                        EditorGUILayout.Space();

                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.LabelField("Grenade:");
                                        EditorGUILayout.Space();


                                        origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionDelay = EditorGUILayout.FloatField("ExplosionDelay", origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionDelay);
                                        origin.TilesAtk.BulletTrajectories[i].Grenade_Defensible = EditorGUILayout.Toggle("CanBeDefended", origin.TilesAtk.BulletTrajectories[i].Grenade_Defensible);
                                        origin.TilesAtk.BulletTrajectories[i].Grenade_CanBeDestroyed = EditorGUILayout.Toggle("CanBeDestroyed", origin.TilesAtk.BulletTrajectories[i].Grenade_CanBeDestroyed);
                                        if (origin.TilesAtk.BulletTrajectories[i].Grenade_CanBeDestroyed) origin.TilesAtk.BulletTrajectories[i].Grenade_Health = EditorGUILayout.FloatField("Health", origin.TilesAtk.BulletTrajectories[i].Grenade_Health);
                                        origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionDamageMultiplier = EditorGUILayout.FloatField("ExplosionDamageMultiplier", origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionDamageMultiplier);

                                        origin.TilesAtk.BulletTrajectories[i].Grenade_OverrideBulletEffects = EditorGUILayout.Toggle("OverrideBulletPS", origin.TilesAtk.BulletTrajectories[i].Grenade_OverrideBulletEffects);
                                        if (origin.TilesAtk.BulletTrajectories[i].Grenade_OverrideBulletEffects)
                                        {
                                            origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionPS = (ParticlesType)EditorGUILayout.EnumPopup("Grenade_ExplosionPS", origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionPS);
                                            origin.TilesAtk.BulletTrajectories[i].Grenade_ObjectPS = (ParticlesType)EditorGUILayout.EnumPopup("Grenade_ObjectPS", origin.TilesAtk.BulletTrajectories[i].Grenade_ObjectPS);
                                        }

                                        var listGrenadeExplosion = origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionTiles;
                                        int newCountGrenadeExplosion = Mathf.Max(1, EditorGUILayout.IntField("Explosion Tiles Count", listGrenadeExplosion.Count));
                                        while (newCountGrenadeExplosion < listGrenadeExplosion.Count)
                                            listGrenadeExplosion.RemoveAt(listGrenadeExplosion.Count - 1);
                                        while (newCountGrenadeExplosion > listGrenadeExplosion.Count)
                                            listGrenadeExplosion.Add(Vector2Int.zero);
                                        while (newCountGrenadeExplosion < listGrenadeExplosion.Count) listGrenadeExplosion.RemoveAt(listGrenadeExplosion.Count - 1);
                                        while (newCountGrenadeExplosion > listGrenadeExplosion.Count) listGrenadeExplosion.Add(Vector2Int.zero);
                                        while (listGrenadeExplosion.Count < origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects.Count) origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects.RemoveAt(origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects.Count - 1);
                                        while (listGrenadeExplosion.Count > origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects.Count) origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects.Add(new DirtyBombEffectsList());
                                        while (listGrenadeExplosion.Count < showDirtyBombEffectsList.Count) showDirtyBombEffectsList.RemoveAt(showDirtyBombEffectsList.Count - 1);
                                        while (listGrenadeExplosion.Count > showDirtyBombEffectsList.Count) showDirtyBombEffectsList.Add(false);
                                        showExplosionList = EditorGUILayout.Foldout(showExplosionList, "Explosion Tiles (" + listGrenadeExplosion.Count.ToString() + ")");
                                        if (showExplosionList)
                                        {
                                            for (int u = 0; u < listGrenadeExplosion.Count; u++)
                                            {
                                                origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionTiles[u] = EditorGUILayout.Vector2IntField("Explosion On " + origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionTiles[u].ToString(), origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionTiles[u]);
                                                var effectList = origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects[u].list;
                                                int effectCount = Mathf.Max(0, EditorGUILayout.IntField("Effects On " + origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionTiles[u].ToString() + " Count", effectList.Count));
                                                while (effectCount < effectList.Count) effectList.RemoveAt(effectList.Count - 1);
                                                while (effectCount > effectList.Count) effectList.Add(null);
                                                showDirtyBombEffectsList[u] = EditorGUILayout.Foldout(showDirtyBombEffectsList[u], "Effects On " + origin.TilesAtk.BulletTrajectories[i].Grenade_ExplosionTiles[u].ToString() + " (" + effectList.Count.ToString() + ")");
                                                if (showDirtyBombEffectsList[u])
                                                {
                                                    for (int j = 0; j < effectList.Count; j++)
                                                    {
                                                        origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects[u].list[j] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + (j + 1).ToString(), origin.TilesAtk.BulletTrajectories[i].Grenade_DirtyBombEffects[u].list[j], typeof(ScriptableObjectAttackEffect), false);
                                                    }
                                                }
                                                EditorGUILayout.LabelField("====================================");
                                            }
                                        }
                                        EditorGUI.indentLevel--;
                                    }


                                    origin.TilesAtk.BulletTrajectories[i].IsHoming = origin.TilesAtk.BulletTrajectories[i]._BulletT[a] == BulletType.Homing;
                                    if (origin.TilesAtk.BulletTrajectories[i].IsHoming)
                                    {
                                        EditorGUILayout.Space();

                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.LabelField("Homing:");
                                        EditorGUILayout.Space();

                                        origin.TilesAtk.BulletTrajectories[i].Homing_TimeTillExpiry = EditorGUILayout.FloatField("TimeTillExpiry", origin.TilesAtk.BulletTrajectories[i].Homing_TimeTillExpiry);
                                        origin.TilesAtk.BulletTrajectories[i].Homing_SlowDownMultiplier = EditorGUILayout.FloatField("SlowDownMultiplier", origin.TilesAtk.BulletTrajectories[i].Homing_SlowDownMultiplier);
                                        origin.TilesAtk.BulletTrajectories[i].Homing_TurningSpeed = EditorGUILayout.FloatField("TurningSpeed", origin.TilesAtk.BulletTrajectories[i].Homing_TurningSpeed);
                                        EditorGUI.indentLevel--;
                                    }

                                    if (origin.TilesAtk.BulletTrajectories[i]._BulletT[a] == BulletType.Pooping)
                                    {
                                        EditorGUILayout.Space();

                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.LabelField("Pooping:");
                                        EditorGUILayout.Space();

                                        origin.TilesAtk.BulletTrajectories[i].ChancesOfPooping = EditorGUILayout.FloatField("ChancesOfPooping", origin.TilesAtk.BulletTrajectories[i].ChancesOfPooping);
                                        origin.TilesAtk.BulletTrajectories[i].InEnemySide = EditorGUILayout.Toggle("InEnemySide", origin.TilesAtk.BulletTrajectories[i].InEnemySide);
                                        origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.TileAction);
                                        if (origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.TileAction == TileActionType.OverTime)
                                        {
                                            origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.HitTime = EditorGUILayout.FloatField("HitTime", origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.HitTime);
                                        }
                                        origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("TileParticlesID", origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.TileParticlesID);
                                        origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.EffectChances = EditorGUILayout.FloatField("EffectChances", origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.EffectChances);
                                        origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.DurationOnTile);
                                        origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.DurationOnTileV = EditorGUILayout.Vector2Field("DurationOnTileV", origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.DurationOnTileV);
                                        var listPooping = origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.Effects;
                                        int newCountPooping = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", listPooping.Count));
                                        while (newCountPooping < listPooping.Count)
                                            listPooping.RemoveAt(listPooping.Count - 1);
                                        while (newCountPooping > listPooping.Count)
                                            listPooping.Add(null);

                                        for (int q = 0; q < listPooping.Count; q++)
                                        {
                                            origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.Effects[q] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + q, origin.TilesAtk.BulletTrajectories[i].PoopingEffectsOnTile.Effects[q], typeof(ScriptableObjectAttackEffect), false);
                                        }

                                        EditorGUI.indentLevel--;
                                    }
                                }
                                EditorGUI.indentLevel--;
                            }
                        }


                        origin.TilesAtk.BulletTrajectories[i].CopyTo = EditorGUILayout.Toggle("CopyTo", origin.TilesAtk.BulletTrajectories[i].CopyTo);
                        origin.TilesAtk.BulletTrajectories[i].TimeMultiplier = EditorGUILayout.FloatField("TimeMultiplier", origin.TilesAtk.BulletTrajectories[i].TimeMultiplier);
                        origin.TilesAtk.BulletTrajectories[i].IsIndicatingOntTile = EditorGUILayout.Toggle("IsDamageTileBased", origin.TilesAtk.BulletTrajectories[i].IsIndicatingOntTile);


                        if (copyTo != null)
                        {
                            if ((origin.TilesAtk.BulletTrajectories[i].HasABullet && origin.AttackInput >= AttackInputType.Strong) || origin.AttackInput == AttackInputType.Weak)
                            {
                                origin.TilesAtk.BulletTrajectories[i].Trajectory_Y = EditorGUILayout.CurveField("Trajectory_Y", copyTo.Trajectory_Y);
                                origin.TilesAtk.BulletTrajectories[i].Trajectory_Z = EditorGUILayout.CurveField("Trajectory_Z", copyTo.Trajectory_Z);
                                origin.TilesAtk.BulletTrajectories[i].Trajectory_Speed = EditorGUILayout.CurveField("Trajectory_Speed", copyTo.Trajectory_Speed);
                                if (origin.TilesAtk.BulletTrajectories[i].Trajectory_Speed.length == 0)
                                {
                                    origin.TilesAtk.BulletTrajectories[i].Trajectory_Speed = new AnimationCurve(AnimationCurve.Linear(0, 0, 1, 1).keys);
                                }

                            }
                            origin.TilesAtk.BulletTrajectories[i].ExplosionChances = EditorGUILayout.IntField("ExplosionChances", copyTo.ExplosionChances);
                            origin.TilesAtk.BulletTrajectories[i].BulletEffectTiles = copyTo.BulletEffectTiles.OrderBy(r => Mathf.Sqrt(r.Pos.x) + Mathf.Sqrt(r.Pos.y)).ToList();
                        }
                        else
                        {
                            if ((origin.TilesAtk.BulletTrajectories[i].HasABullet && origin.AttackInput >= AttackInputType.Strong) || origin.AttackInput == AttackInputType.Weak)
                            {
                                origin.TilesAtk.BulletTrajectories[i].Trajectory_Y = EditorGUILayout.CurveField("Trajectory_Y", origin.TilesAtk.BulletTrajectories[i].Trajectory_Y);
                                origin.TilesAtk.BulletTrajectories[i].Trajectory_Z = EditorGUILayout.CurveField("Trajectory_Z", origin.TilesAtk.BulletTrajectories[i].Trajectory_Z);
                                origin.TilesAtk.BulletTrajectories[i].Trajectory_Speed = EditorGUILayout.CurveField("Trajectory_Speed", origin.TilesAtk.BulletTrajectories[i].Trajectory_Speed);
                                if (origin.TilesAtk.BulletTrajectories[i].Trajectory_Speed.length == 0)
                                {
                                    origin.TilesAtk.BulletTrajectories[i].Trajectory_Speed = new AnimationCurve(AnimationCurve.Linear(0, 0, 1, 1).keys);
                                }

                            }
                            origin.TilesAtk.BulletTrajectories[i].ExplosionChances = EditorGUILayout.IntField("ExplosionChances", origin.TilesAtk.BulletTrajectories[i].ExplosionChances);
                            origin.TilesAtk.BulletTrajectories[i].BulletEffectTiles.OrderBy(r => Mathf.Sqrt(r.Pos.x) + Mathf.Sqrt(r.Pos.y));
                        }
                        bfat = origin.TilesAtk.AtkType;

                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        switch (bfat)
                        {
                            case BattleFieldAttackType.OnAreaAttack:
                                DrawBattleTileAtk(origin.TilesAtk.BulletTrajectories[i], new Vector2Int(0, 6), new Vector2Int(0, 12));
                                break;
                            case BattleFieldAttackType.OnTarget:
                                DrawBattleTileAtk(origin.TilesAtk.BulletTrajectories[i], new Vector2Int(-12, 12), new Vector2Int(-6, 6));
                                break;
                            case BattleFieldAttackType.OnRandom:
                                DrawBattleTileAtk(origin.TilesAtk.BulletTrajectories[i], new Vector2Int(0, 1), new Vector2Int(0, 1));
                                break;
                        }
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                    }

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }
            else
            {



                if (origin.TilesAtk.BulletTrajectories.Count == 0)
                {
                    origin.TilesAtk.BulletTrajectories.Add(new BulletBehaviourInfoClassOnBattleFieldClass());
                }
                else if (origin.TilesAtk.BulletTrajectories.Count > 1)
                {
                    origin.TilesAtk.BulletTrajectories.RemoveRange(1, origin.TilesAtk.BulletTrajectories.Count - 1);
                }

                origin.TilesAtk.BulletTrajectories[0].Delay = EditorGUILayout.FloatField("Delay", origin.TilesAtk.BulletTrajectories[0].Delay);
                origin.TilesAtk.BulletTrajectories[0].IsIndicatingOntTile = EditorGUILayout.Toggle("IsDamageTileBased", origin.TilesAtk.BulletTrajectories[0].IsIndicatingOntTile);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;

                DrawBattleTileAtk(origin.TilesAtk.BulletTrajectories[0], new Vector2Int(-12, 12), new Vector2Int(-6, 6));

                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
            }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
       /* List<ScriptableObjectAttackEffect> t;
        foreach (var item in origin.TilesAtk.BulletTrajectories)
        {
            t = item.BulletEffectTiles[0].Effects;
            foreach (var tile in item.BulletEffectTiles)
            {
                tile.HasEffect = true;
                tile.Effects = t;
                tile.ChildrenExplosion = new List<ParticlesChildExplosionClass>();
            }
        }*/
        


        #endregion
        EditorUtility.SetDirty(origin);
    }


    public List<T> RefreshList<T>(int nextVal, List<T> currentList) where T : new()
    {
        while (nextVal < currentList.Count && currentList.Count > 0)
            currentList.RemoveAt(currentList.Count - 1);
        while (nextVal > currentList.Count)
            currentList.Add(new T());

        return currentList;
    }

    public void DrawParticlesChildrenExplosion(ParticlesChildExplosionClass particlesChildExplosion, Vector2Int horizontal, Vector2Int vertical)
    {
        BattleFieldAttackChildTileClass bfatc;
        bool showClose = false;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        for (int x = horizontal.x; x < horizontal.y; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = vertical.x; y < vertical.y; y++)
            {
                bfatc = particlesChildExplosion.BulletEffectTiles.Where(r => r.Pos == new Vector2Int(x, y)).FirstOrDefault();

                showClose = EditorGUILayout.Toggle("", bfatc != null ? true : false, GUILayout.Width(20));
                EditorGUILayout.LabelField(x + "," + y, GUILayout.Width(30));
                //EditorGUILayout.Space(-5f);
                if (showClose)
                {
                    if (bfatc == null)
                    {
                        bfatc = particlesChildExplosion.BulletEffectTiles.Count > 0 ? new BattleFieldAttackChildTileClass(particlesChildExplosion.BulletEffectTiles[0], new Vector2Int(x, y)) : new BattleFieldAttackChildTileClass(new Vector2Int(x, y));
                        particlesChildExplosion.BulletEffectTiles.Add(bfatc);
                    }
                }
                else if (!showClose && bfatc != null)
                {
                    particlesChildExplosion.BulletEffectTiles.Remove(bfatc);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (particlesChildExplosion.BulletEffectTiles.Count > 0)
        {
            WriteChildrenInfo(particlesChildExplosion);
        }

        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
    }

    public void DrawNewFieldOfView(Vector2Int horizontal, Vector2Int vertical)
    {
        bool showClose = true;
        Vector2Int pos;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
      
        for (int x = horizontal.x; x < horizontal.y; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = vertical.x; y < vertical.y; y++)
            {
                pos = new Vector2Int(x, y);

                if (origin.NewFovSystemList.Contains(pos))
                {
                    showClose = EditorGUILayout.Toggle("", true, GUILayout.Width(20));
                    if (!showClose)
                    {
                        origin.NewFovSystemList.Remove(pos);
                    }
                }
                else
                {
                    showClose = EditorGUILayout.Toggle("", false, GUILayout.Width(20));
                    if (showClose)
                    {
                        origin.NewFovSystemList.Add(pos);
                    }
                }
                EditorGUILayout.LabelField(x + "," + y, GUILayout.Width(40));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
       
    }


    public void DrawBattleTileAtk(BulletBehaviourInfoClassOnBattleFieldClass BattleTileTrajectory, Vector2Int horizontal, Vector2Int vertical)
    {
        bool showClose = true;
        EditorGUI.indentLevel++;

        BattleFieldAttackTileClass bfatc;

        for (int x = horizontal.x; x < horizontal.y; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = vertical.x; y < vertical.y; y++)
            {
                bfatc = BattleTileTrajectory.BulletEffectTiles.Where(r => r.Pos == new Vector2Int(x, y)).FirstOrDefault();
                showClose = EditorGUILayout.Toggle("", bfatc != null ? true : false, GUILayout.Width(20));
                EditorGUILayout.LabelField(x + "," + y, GUILayout.Width(30));

                if (showClose)
                {
                    if (bfatc == null)
                    {
                        bfatc = BattleTileTrajectory.BulletEffectTiles.Count > 0 ? new BattleFieldAttackTileClass(BattleTileTrajectory.BulletEffectTiles[0], new Vector2Int(x, y)) : new BattleFieldAttackTileClass(new Vector2Int(x, y));

                        BattleTileTrajectory.BulletEffectTiles.Add(bfatc);
                    }
                }
                else if (!showClose && bfatc != null)
                {
                    BattleTileTrajectory.BulletEffectTiles.Remove(bfatc);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;

        if (BattleTileTrajectory.BulletEffectTiles.Count > 0)
        {
            WriteInfo(BattleTileTrajectory);
        }
    }

    private void WriteInfo(BulletBehaviourInfoClassOnBattleFieldClass origin)
    {
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;

        origin.Show = EditorGUILayout.Foldout(origin.Show, "Tiles");
        if (origin.Show)
        {
            foreach (BattleFieldAttackTileClass item in origin.BulletEffectTiles)
            {
                item.Foldout = EditorGUILayout.Foldout(item.Foldout, item.Pos.ToString());
                if (item.Foldout)
                {
                    ShowTileObject(item);
                }
            }
        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }

    private void WriteChildrenInfo(ParticlesChildExplosionClass origin)
    {
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        origin.Show = EditorGUILayout.Foldout(origin.Show, "Tiles");
        if (origin.Show)
        {
            foreach (BattleFieldAttackChildTileClass item in origin.BulletEffectTiles)
            {
                item.Foldout = EditorGUILayout.Foldout(item.Foldout, item.Pos.ToString());
                if (item.Foldout)
                {
                    ShowChildTileObject(item);
                }
            }
        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }


    private void ShowTileObject(BattleFieldAttackTileClass bfatc)
    {
        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;

        bfatc.DamagePerc = EditorGUILayout.Slider("DamagePercentage", bfatc.DamagePerc, 0, 1);

        bfatc.EffectsVisisble = EditorGUILayout.Foldout(bfatc.EffectsVisisble, "Effect Info ------------------------------------------");
        if (bfatc.EffectsVisisble)
        {
            bfatc.HasEffect = EditorGUILayout.Toggle("HasEffect", bfatc.HasEffect);
            if (bfatc.HasEffect)
            {
                bfatc.EffectChances = EditorGUILayout.FloatField("EffectChances", bfatc.EffectChances);
                var list = bfatc.Effects;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    bfatc.Effects[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + i, bfatc.Effects[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfat.Effects, typeof(ScriptableObjectAttackEffect), false
                }
            }

        }

        bfatc.TileEffectsVisisble = EditorGUILayout.Foldout(bfatc.TileEffectsVisisble, "TileEffect Info ------------------------------------------");
        if (bfatc.TileEffectsVisisble)
        {

            bfatc.IsEffectOnTile = EditorGUILayout.Toggle("HasEffectOnTile", bfatc.IsEffectOnTile);
            if (bfatc.IsEffectOnTile)
            {
                bfatc.EffectsOnTile.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", bfatc.EffectsOnTile.TileAction);
                if (bfatc.EffectsOnTile.TileAction == TileActionType.OverTime)
                {
                    bfatc.EffectsOnTile.HitTime = EditorGUILayout.FloatField("HitTime", bfatc.EffectsOnTile.HitTime);
                }
                bfatc.EffectsOnTile.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", bfatc.EffectsOnTile.TileParticlesID);
                bfatc.EffectsOnTile.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", bfatc.EffectsOnTile.DurationOnTile);
                bfatc.EffectsOnTile.DurationOnTileV = EditorGUILayout.Vector2Field("DurationOnTileV", bfatc.EffectsOnTile.DurationOnTileV);
                bfatc.EffectsOnTile.EffectChances = EditorGUILayout.FloatField("EffectChances", bfatc.EffectsOnTile.EffectChances);
                var list = bfatc.EffectsOnTile.Effects;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    bfatc.EffectsOnTile.Effects[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + i, bfatc.EffectsOnTile.Effects[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfat.Effects, typeof(ScriptableObjectAttackEffect), false
                }
            }
        }
        bfatc.SummonVisisble = EditorGUILayout.Foldout(bfatc.SummonVisisble, "Summon Info ------------------------------------------");
        if (bfatc.SummonVisisble)
        {
            bfatc.SpawnSummonOnTile = EditorGUILayout.Toggle("SpawnSummonOnTile", bfatc.SpawnSummonOnTile);
            if (bfatc.SpawnSummonOnTile)
            {
                DrawSummonInfo(ref bfatc.SummonToSpawn);
            }
        }

        bfatc.ChildrenExplosionVisisble = EditorGUILayout.Foldout(bfatc.ChildrenExplosionVisisble, "ChildrenExplosion Info ------------------------------------------");
        if (bfatc.ChildrenExplosionVisisble)
        {
            var list = bfatc.ChildrenExplosion;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("ChildrenExplosion", (int)list.Count));

            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(list.Count > 0 ? new ParticlesChildExplosionClass(list[0]) : new ParticlesChildExplosionClass());


            for (int i = 0; i < list.Count; i++)
            {
                bfatc.ChildrenExplosion[i].ChildrenBulletDelay = EditorGUILayout.FloatField("ChildrenBulletDelay", bfatc.ChildrenExplosion[i].ChildrenBulletDelay);
                bfatc.ChildrenExplosion[i].ChildrenDamageMultiplier = EditorGUILayout.FloatField("ChildrenDamageMultiplier", bfatc.ChildrenExplosion[i].ChildrenDamageMultiplier);
                DrawParticlesChildrenExplosion(bfatc.ChildrenExplosion[i], new Vector2Int(-6, 7), new Vector2Int(-6, 7));
            }
        }

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }

    private void ShowChildTileObject(BattleFieldAttackTileBaseClass bfatc)
    {
        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        bfatc.ExposionChances = EditorGUILayout.FloatField("ExposionChance", bfatc.ExposionChances);
        bfatc.EffectsVisisble = EditorGUILayout.Foldout(bfatc.EffectsVisisble, "Effect Info ------------------------------------------");
        if (bfatc.EffectsVisisble)
        {
            bfatc.HasEffect = EditorGUILayout.Toggle("HasEffect", bfatc.HasEffect);
            if (bfatc.HasEffect)
            {
                bfatc.EffectChances = EditorGUILayout.FloatField("EffectChances", bfatc.EffectChances);
                var list = bfatc.Effects;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    bfatc.Effects[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + i, bfatc.Effects[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfat.Effects, typeof(ScriptableObjectAttackEffect), false
                }
            }

        }

        bfatc.TileEffectsVisisble = EditorGUILayout.Foldout(bfatc.TileEffectsVisisble, "TileEffect Info ------------------------------------------");
        if (bfatc.TileEffectsVisisble)
        {

            bfatc.IsEffectOnTile = EditorGUILayout.Toggle("HasEffectOnTile", bfatc.IsEffectOnTile);
            if (bfatc.IsEffectOnTile)
            {
                bfatc.EffectsOnTile.TileAction = (TileActionType)EditorGUILayout.EnumPopup("TileAction", bfatc.EffectsOnTile.TileAction);
                if (bfatc.EffectsOnTile.TileAction == TileActionType.OverTime)
                {
                    bfatc.EffectsOnTile.HitTime = EditorGUILayout.FloatField("HitTime", bfatc.EffectsOnTile.HitTime);
                }
                bfatc.EffectsOnTile.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", bfatc.EffectsOnTile.TileParticlesID);
                bfatc.EffectsOnTile.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", bfatc.EffectsOnTile.DurationOnTile);
                bfatc.EffectsOnTile.DurationOnTileV = EditorGUILayout.Vector2Field("DurationOnTileV", bfatc.EffectsOnTile.DurationOnTileV);

                bfatc.EffectsOnTile.EffectChances = EditorGUILayout.FloatField("EffectChances", bfatc.EffectsOnTile.EffectChances);
                var list = bfatc.EffectsOnTile.Effects;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    bfatc.EffectsOnTile.Effects[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + i, bfatc.EffectsOnTile.Effects[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfat.Effects, typeof(ScriptableObjectAttackEffect), false
                }
            }
        }
        bfatc.SummonVisisble = EditorGUILayout.Foldout(bfatc.SummonVisisble, "Summon Info ------------------------------------------");
        if (bfatc.SummonVisisble)
        {
            bfatc.SpawnSummonOnTile = EditorGUILayout.Toggle("SpawnSummonOnTile", bfatc.SpawnSummonOnTile);
            if (bfatc.SpawnSummonOnTile)
            {
                DrawSummonInfo(ref bfatc.SummonToSpawn);
            }
        }


        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }

    void DrawSummonInfo(ref TileSummonClass tsc)
    {
        tsc.CharToSummon = (CharacterNameType)EditorGUILayout.EnumPopup("CharToSpawn", tsc.CharToSummon);
        tsc.SpawnChances = EditorGUILayout.Slider("Spawn Chances", tsc.SpawnChances, 0f, 100f);
        tsc.UncappedDuration = EditorGUILayout.Toggle("Uncapped Duration", tsc.UncappedDuration);
        if (!tsc.UncappedDuration)
        {
            tsc.DurationOnField = EditorGUILayout.Vector2Field("Duration on Field", tsc.DurationOnField);
        }

        List<Vector2Int> spawnOffsets = tsc.SummonSpawnPositions.ToList();
        int offsetCount = Mathf.Max(0, EditorGUILayout.IntField("Summon SpawnPoints", spawnOffsets.Count));
        while (offsetCount < spawnOffsets.Count)
            spawnOffsets.RemoveAt(spawnOffsets.Count - 1);
        while (offsetCount > spawnOffsets.Count)
            spawnOffsets.Add(Vector2Int.zero);

        for (int a = 0; a < spawnOffsets.Count; a++)
        {
            spawnOffsets[a] = EditorGUILayout.Vector2IntField("Offset: " + spawnOffsets[a].ToString(), spawnOffsets[a]);
        }

        tsc.SummonSpawnPositions = spawnOffsets.ToArray();

        tsc.hasCharOverrides = EditorGUILayout.Toggle("HasCharOverrides", tsc.hasCharOverrides);
        if (tsc.hasCharOverrides)
        {
            if (tsc.CharOverrides == null) tsc.CharOverrides = new BaseInfoInjectorClass();
            DrawBaseInfoInjectorClass(ref tsc.CharOverrides);
        }
    }

    bool ShowHealth = false;
    bool ShowEther = false;
    bool ShowSpeed = false;
    bool ShowDamage = false;
    bool ShowShield = false;
    bool ShowLuck = false;
    bool ShowHue = false;

    void DrawBaseInfoInjectorClass(ref BaseInfoInjectorClass biijc)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Overrides: ");

        ShowHealth = EditorGUILayout.Foldout(ShowHealth, "HP_________________________________________");
        if (ShowHealth)
        {
            biijc.OverrideHealth = EditorGUILayout.Toggle("Override Health", biijc.OverrideHealth);
            if (biijc.OverrideHealth)
            {
                biijc.Health = EditorGUILayout.FloatField("Health", biijc.Health);
            }

            biijc.OverrideHealthRegeneration = EditorGUILayout.Toggle("Override Health Regen", biijc.OverrideHealthRegeneration);
            if (biijc.OverrideHealthRegeneration)
            {
                biijc.HealthRegeneration = EditorGUILayout.FloatField("Health Regeneration", biijc.HealthRegeneration);
            }
        }

        ShowEther = EditorGUILayout.Foldout(ShowEther, "Ether_________________________________________");
        if (ShowEther)
        {
            biijc.OverrideEther = EditorGUILayout.Toggle("Override Ether", biijc.OverrideEther);
            if (biijc.OverrideEther)
            {
                biijc.Ether = EditorGUILayout.FloatField("Ether", biijc.Ether);
            }

            biijc.OverrideEtherRegeneration = EditorGUILayout.Toggle("Override Ether Regen", biijc.OverrideEtherRegeneration);
            if (biijc.OverrideEtherRegeneration)
            {
                biijc.EtherRegeneration = EditorGUILayout.FloatField("Ether Regeneration", biijc.EtherRegeneration);
            }
        }

        ShowShield = EditorGUILayout.Foldout(ShowShield, "Shield_________________________________________");
        if (ShowShield)
        {
            biijc.OverrideArmour = EditorGUILayout.Toggle("Override Armour", biijc.OverrideArmour);
            if (biijc.OverrideArmour)
            {
                biijc.Armour = EditorGUILayout.FloatField("Armour", biijc.Armour);
            }

            biijc.OverrideShieldRegeneration = EditorGUILayout.Toggle("Override Shield Regen", biijc.OverrideShieldRegeneration);
            if (biijc.OverrideShieldRegeneration)
            {
                biijc.ShieldRegeneration = EditorGUILayout.FloatField("Shield Regeneration", biijc.ShieldRegeneration);
            }
        }


        ShowSpeed = EditorGUILayout.Foldout(ShowSpeed, "Speed_________________________________________");
        if (ShowSpeed)
        {
            biijc.OverrideSpeed = EditorGUILayout.Toggle("Override Speed", biijc.OverrideSpeed);
            if (biijc.OverrideSpeed)
            {
                biijc.MovementSpeed = EditorGUILayout.FloatField("Speed", biijc.MovementSpeed);
            }

            biijc.OverrideEvasion = EditorGUILayout.Toggle("Override Evasion", biijc.OverrideEvasion);
            if (biijc.OverrideEvasion)
            {
                biijc.Evasion = EditorGUILayout.Vector2Field("Evasion", biijc.Evasion);
            }


        }


        ShowLuck = EditorGUILayout.Foldout(ShowLuck, "Luck_________________________________________");
        if (ShowLuck)
        {

            biijc.OverrideSigilDropBonus = EditorGUILayout.Toggle("Override SigilDropBonus", biijc.OverrideSigilDropBonus);
            if (biijc.OverrideSigilDropBonus)
            {
                biijc.SigilDropBonus = EditorGUILayout.FloatField("SigilDropBonus", biijc.SigilDropBonus);
            }
            biijc.OverrideCriticalWeakBullet = EditorGUILayout.Toggle("Override CriticalWeakBullet", biijc.OverrideCriticalWeakBullet);
            if (biijc.OverrideCriticalWeakBullet)
            {
                biijc.CriticalWeakBullet = EditorGUILayout.Vector2Field("CriticalWeakBullet", biijc.CriticalWeakBullet);
            }

            biijc.OverrideCriticalStrongBullet = EditorGUILayout.Toggle("Override CriticalStrongBullet", biijc.OverrideCriticalStrongBullet);
            if (biijc.OverrideCriticalStrongBullet)
            {
                biijc.CriticalStrongBullet = EditorGUILayout.Vector2Field("CriticalStrongBullet", biijc.CriticalStrongBullet);
            }
        }

        ShowDamage = EditorGUILayout.Foldout(ShowDamage, "Damage_________________________________________");
        if (ShowDamage)
        {
            biijc.OverrideWeakAttackMultiplier = EditorGUILayout.Toggle("Override WeakAttackMultiplier", biijc.OverrideWeakAttackMultiplier);
            if (biijc.OverrideWeakAttackMultiplier)
            {
                biijc.WeakAttackMultiplier = EditorGUILayout.FloatField("Weak Attack Multiplier", biijc.WeakAttackMultiplier);

            }

            biijc.OverrideStrongAttackMultiplier = EditorGUILayout.Toggle("Override StrongAttackMultiplier", biijc.OverrideStrongAttackMultiplier);
            if (biijc.OverrideStrongAttackMultiplier)
            {
                biijc.StrongAttackMultiplier = EditorGUILayout.FloatField("Strong Attack Multiplier", biijc.StrongAttackMultiplier);
            }
        }

        ShowHue = EditorGUILayout.Foldout(ShowHue, "Hue_________________________________________");
        if (ShowHue)
        {
            biijc.OverrideHue = EditorGUILayout.Toggle("Override Hue", biijc.OverrideHue);
            if (biijc.OverrideHue)
            {
                biijc.ColorHueSaturation.color = EditorGUILayout.ColorField("Color", biijc.ColorHueSaturation.color);
                biijc.ColorHueSaturation.hue = EditorGUILayout.FloatField("Hue", biijc.ColorHueSaturation.hue);
                biijc.ColorHueSaturation.sat = EditorGUILayout.FloatField("Saturation", biijc.ColorHueSaturation.sat);
            }
        }

        biijc.overrideDeath = EditorGUILayout.Toggle("Override Death", biijc.overrideDeath);
        if (EditorGUILayout.Foldout(biijc.overrideDeath, "Death Override Data"))
        {
            biijc.deathAnim = (DeathBehaviourType)EditorGUILayout.EnumPopup("Death Anim", biijc.deathAnim);
        }
        EditorGUILayout.Space();
    }
}

public class BattleFieldTileInfo
{
    public BulletBehaviourInfoClassOnBattleFieldClass TileAttackParent;
    public ParticlesChildExplosionClass ParticlesAttackParent;
    public BattleFieldAttackTileBaseClass Tile;
    public bool Show = true;
    public bool Selected = false;

    public BattleFieldTileInfo(BulletBehaviourInfoClassOnBattleFieldClass tileAttackParent, BattleFieldAttackTileClass tile)
    {
        TileAttackParent = tileAttackParent;
        Tile = tile;
    }

    public BattleFieldTileInfo(ParticlesChildExplosionClass particlesAttackParent, BattleFieldAttackTileClass tile)
    {
        ParticlesAttackParent = particlesAttackParent;
        Tile = tile;
    }
    public BattleFieldTileInfo(ParticlesChildExplosionClass particlesAttackParent, BattleFieldAttackChildTileClass tile)
    {
        ParticlesAttackParent = particlesAttackParent;
        Tile = tile;
    }
}
