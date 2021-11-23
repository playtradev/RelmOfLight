using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlaytraGamesLtd;
using System.Linq;
using UnityEngine.Video;

/// <summary>
///   public bool IsRandomPos = true;
//[ConditionalField("IsRandomPos", true)] public Vector2Int SpawningPos;
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/atkbase")]
public class ScriptableObjectAttackBase : ScriptableObjectSkillBase
{
    public CharacterNameType AttackOwner = CharacterNameType.None;

    [HideInInspector]
    public float DamageMultiplier
    {
        get
        {
            return Random.Range(_DamageMultiplier.x, _DamageMultiplier.y);
        }
    }

    [Header("General")]

    public CharacterAnimationStateType AnimToFireOnHit = CharacterAnimationStateType.GettingHit;

    public TileAttackPhaseType taPhase = TileAttackPhaseType.None;

    public bool UseAttackerElement = true;
    [ConditionalField("UseAttackerElement", true)] public List<ElementalType> AttackElements = new List<ElementalType>() { ElementalType.Neutral };

    public Vector2 _DamageMultiplier = new Vector2(1, 1);


    public AttackInputType AttackInput;
    public AttackTargetSideType AttackTargetSide;


    public bool useCustomChargeTime_Reset = false;
    [ConditionalField("useCustomChargeTime")]public float customChargeTime = 0f;
    public float ChargingTime
    {
        get
        {
            return useCustomChargeTime_Reset ? customChargeTime : 10f;
        }
    }

    [HideInInspector] public AttackAnimPrefixType PrefixAnim;

    public List<Vector2Int> NewFovSystemList = new List<Vector2Int>();

    [Header("Style")]
    public AttackAnimType AttackAnim;
    public AttackParticlesClass Particles;
    public bool EffectTimeOnImpact = false;

    public SlowDownOnHitClass SlowDownOnHit;

    public bool OverridePsHit = false;
    public HitParticlesType _HitParticlesT;
    [ConditionalField("HitParticlesT", false, HitParticlesType.Resized)] public float _HitResizeMultiplier = 1f;
    public float HitResizeMultiplier
    {
        get
        {
            return (OverridePsHit && _HitParticlesT == HitParticlesType.Resized) ? _HitResizeMultiplier : BattleManagerScript.Instance.HitResizeMultiplier;
        }
    }

    public HitParticlesType HitParticlesT
    {
        get
        {
            return OverridePsHit ? _HitParticlesT :
                (AttackInput == AttackInputType.Weak && BattleManagerScript.Instance.HitParticlesT == HitParticlesType.None) ? HitParticlesType.None :
                (AttackInput == AttackInputType.Weak && BattleManagerScript.Instance.HitParticlesT == HitParticlesType.Resized) ? HitParticlesType.Resized : HitParticlesType.Normal;
        }
    }

    public bool OverridePsInput = false;
    public AttackParticlesInputType _ParticlesInput;
    public AttackParticlesInputType ParticlesInput
    {
        get
        {
            return OverridePsInput ? _ParticlesInput : (AttackParticlesInputType)System.Enum.Parse(typeof(AttackParticlesInputType), AttackInput.ToString()); 
        }
    }

    [System.Serializable]
    public class AttackParticlesClass
    {
        public AttackParticlesSideClass Right;
        [System.Serializable]
        public class AttackParticlesSideClass
        {
            public string CastAddress;
            public string BulletAddress;
            public string HitAddress;
            public GameObject Cast;
            public GameObject Bullet;
            public GameObject Hit;
        }

       
    }

    [Header("Trajectories")]
    public int TrajectoriesNumber;
    public List<BulletType> _BulletT = new List<BulletType>();

    public List<BulletType> BulletT
    {
        get
        {
            return _BulletT.Count == 0 ? new List<BulletType>() { BulletType.Base } : _BulletT;
        }
        set
        {
            _BulletT = value;
        }
    }


    [HideInInspector] public float Grenade_ExplosionDelay = 3f;
    [HideInInspector] public bool Grenade_Defensible = false;
    [HideInInspector] public bool Grenade_CanBeDestroyed = false;
    [HideInInspector] public float Grenade_Health = 10f;
    [HideInInspector] public float Grenade_ExplosionDamageMultiplier = 1f;
    [HideInInspector] public bool Grenade_OverrideBulletEffects = false;
    [HideInInspector] public ParticlesType Grenade_ExplosionPS;
    [HideInInspector] public ParticlesType Grenade_ObjectPS;
    [HideInInspector] public List<Vector2Int> Grenade_ExplosionTiles = new List<Vector2Int>();
    [HideInInspector] public List<DirtyBombEffectsList> Grenade_DirtyBombEffects = new List<DirtyBombEffectsList>();

    [HideInInspector] public bool IsHoming = false;
    [HideInInspector] public float Homing_TimeTillExpiry = 5f;
    [HideInInspector] public float Homing_TurningSpeed = 3f;
    [HideInInspector] public float Homing_SlowDownMultiplier = 0.5f;

    [HideInInspector] public float ChancesOfPooping = 100;
    [HideInInspector] public bool BothSides = false;
    [HideInInspector] public bool InEnemySide = true;
    public bool isPoopingEffectOnTile = false;
    [ConditionalField("IsEffectOnTile", false)] public TileEffectClass PoopingEffectsOnTile;
    public bool isPoopingSpawnSummonOnTile = false;
    [ConditionalField("SpawnSummonOnTile", false)] public TileSummonClass PoopingSummonToSpawn;

 
    [HideInInspector] public TilesAttackTypeClass TilesAtk;


    private void OnEnable()
    {
        switch (AttackAnim)
        {
            case AttackAnimType.Weak_Atk:
                PrefixAnim = AttackAnimPrefixType.Atk1;
                break;
            case AttackAnimType.Strong_Atk:
                PrefixAnim = AttackAnimPrefixType.Atk2;
                break;
            case AttackAnimType.Buff:
                PrefixAnim = AttackAnimPrefixType.S_Buff;
                break;
            case AttackAnimType.Debuff:
                PrefixAnim = AttackAnimPrefixType.S_DeBuff;
                break;
            default:
                break;
        }
    }
}


#region Tiles


public class AttackTypeClass
{
    public int Chances;
    public bool Show;
}

[System.Serializable]
public class TilesAttackTypeClass : AttackTypeClass
{
    public int CurrentChances
    {
        get
        {
            return Chances;
        }
    }


    public bool EffectOnCaster;
    public List<ScriptableObjectAttackEffect> EffectsOnCaster = new List<ScriptableObjectAttackEffect>();


    public BulletLevelType BulletLevel = BulletLevelType.Level_1;
    public BattleFieldAttackType AtkType;
    public bool CanAffectBothSide = false;
    public bool UseBoundaries = false;
    [SerializeField] public Vector2Int RangeLevel = new Vector2Int(0, 10);
    public bool UseOldSystem = true;
    public List<BulletBehaviourInfoClassOnBattleFieldClass> BulletTrajectories = new List<BulletBehaviourInfoClassOnBattleFieldClass>();
}

[System.Serializable]
public class StatsToCheckClass
{
    public StatsCheckType StatToCheck;

    public ValueCheckerType ValueChecker;
    public float PercToCheck;
    public Vector2 InBetween;
    public bool Show;
    public AIType AIToCheck;
    public int AIPerc = 100;
}

[System.Serializable]
public class BulletBehaviourInfoClassOnBattleFieldClass
{
    public float Delay;
    public bool HasABullet = true;
    public bool OverrideBulletLevel = false;
    public BulletLevelType BulletLevel = BulletLevelType.Level_1;
    public bool InfoVisible = false;

    public bool BulletGrenadeVisisble = false;
    public bool BulletPoopingVisisble = false;
    public bool BulletHomingVisisble = false;

  


    public bool OverrideBulletInfo = false;
    //NEW ------------------------
    public List<BulletType> _BulletT = new List<BulletType>();

    public List<BulletType> BulletT
    {
        get
        {
            return _BulletT.Count == 0 ? new List<BulletType>() { BulletType.Base } : _BulletT;
        }
        set
        {
            _BulletT = value;
        }
    }

    [HideInInspector] public float Grenade_ExplosionDelay = 3f;
    [HideInInspector] public bool Grenade_Defensible = false;
    [HideInInspector] public bool Grenade_CanBeDestroyed = false;
    [HideInInspector] public float Grenade_Health = 10f;
    [HideInInspector] public float Grenade_ExplosionDamageMultiplier = 1f;
    [HideInInspector] public bool Grenade_OverrideBulletEffects = false;
    [HideInInspector] public ParticlesType Grenade_ExplosionPS;
    [HideInInspector] public ParticlesType Grenade_ObjectPS;
    [HideInInspector] public List<Vector2Int> Grenade_ExplosionTiles = new List<Vector2Int>();
    [HideInInspector] public List<DirtyBombEffectsList> Grenade_DirtyBombEffects = new List<DirtyBombEffectsList>();

    [HideInInspector] public bool IsHoming = false;
    [HideInInspector] public float Homing_TimeTillExpiry = 5f;
    [HideInInspector] public float Homing_TurningSpeed = 3f;
    [HideInInspector] public float Homing_SlowDownMultiplier = 0.5f;

    [HideInInspector] public float ChancesOfPooping = 100;
    [HideInInspector] public bool InEnemySide = true;
    [HideInInspector] public TileEffectClass PoopingEffectsOnTile;

    //EndNew





    [HideInInspector] public bool toFire = false;
    public bool CopyTo = false;
    public int ExplosionChances = 100;
    public float TimeMultiplier = 1;
    public bool IsIndicatingOntTile = false;
    public AnimationCurve Trajectory_Speed = new AnimationCurve();
    public AnimationCurve Trajectory_Y = new AnimationCurve();
    public AnimationCurve Trajectory_Z = new AnimationCurve();
    public bool Show = true;
    [HideInInspector] public List<BattleFieldAttackTileClass> BulletEffectTiles = new List<BattleFieldAttackTileClass>();

    public BulletBehaviourInfoClassOnBattleFieldClass()
    {

    }


    public BulletBehaviourInfoClassOnBattleFieldClass(BulletBehaviourInfoClassOnBattleFieldClass copyFrom)
    {
        TimeMultiplier = copyFrom.TimeMultiplier;
        Trajectory_Y = new AnimationCurve(copyFrom.Trajectory_Y.keys);
        Trajectory_Z = new AnimationCurve(copyFrom.Trajectory_Z.keys);
        Trajectory_Speed = new AnimationCurve(copyFrom.Trajectory_Speed.keys);

        InfoVisible = copyFrom.InfoVisible;


        Delay = copyFrom.Delay;
        HasABullet = copyFrom.HasABullet;

        BulletGrenadeVisisble = copyFrom.BulletGrenadeVisisble;
        BulletPoopingVisisble = copyFrom.BulletPoopingVisisble;
        BulletHomingVisisble = copyFrom.BulletHomingVisisble;

        OverrideBulletInfo = copyFrom.OverrideBulletInfo;

        BulletType[] btTemp = new BulletType[copyFrom._BulletT.Count];
        copyFrom._BulletT.CopyTo(btTemp);
        _BulletT = btTemp.ToList();

        Grenade_ExplosionDelay = copyFrom.Grenade_ExplosionDelay;
        Grenade_Defensible = copyFrom.Grenade_Defensible;
        Grenade_CanBeDestroyed = copyFrom.Grenade_CanBeDestroyed;
        Grenade_Health = copyFrom.Grenade_Health;
        Grenade_ExplosionDamageMultiplier = copyFrom.Grenade_ExplosionDamageMultiplier;
        Grenade_OverrideBulletEffects = copyFrom.Grenade_OverrideBulletEffects;
        Grenade_ExplosionPS = copyFrom.Grenade_ExplosionPS;
        Grenade_ObjectPS = copyFrom.Grenade_ObjectPS;


        Vector2Int[] v2Temp = new Vector2Int[copyFrom.Grenade_ExplosionTiles.Count];
        copyFrom.Grenade_ExplosionTiles.CopyTo(v2Temp);
        Grenade_ExplosionTiles = v2Temp.ToList();

        DirtyBombEffectsList[] dbTemp = new DirtyBombEffectsList[copyFrom.Grenade_DirtyBombEffects.Count];
        copyFrom.Grenade_DirtyBombEffects.CopyTo(dbTemp);
        Grenade_DirtyBombEffects = dbTemp.ToList();

        IsHoming = copyFrom.IsHoming;
        Homing_TimeTillExpiry = copyFrom.Homing_TimeTillExpiry;
        Homing_TurningSpeed = copyFrom.Homing_TurningSpeed;
        Homing_SlowDownMultiplier = copyFrom.Homing_SlowDownMultiplier;

        ChancesOfPooping = copyFrom.ChancesOfPooping;
        InEnemySide = copyFrom.InEnemySide;
        PoopingEffectsOnTile = copyFrom.PoopingEffectsOnTile;


        ExplosionChances = copyFrom.ExplosionChances;
        TimeMultiplier = copyFrom.TimeMultiplier;

        IsIndicatingOntTile = copyFrom.IsIndicatingOntTile;


        BattleFieldAttackTileClass[] baTemp = new BattleFieldAttackTileClass[copyFrom.BulletEffectTiles.Count];
        copyFrom.BulletEffectTiles.CopyTo(baTemp);
        BulletEffectTiles = baTemp.ToList();
    }
}

[System.Serializable]
public class BattleFieldAttackTileBaseClass
{
    [HideInInspector] public Vector2Int Pos;
    [HideInInspector] public bool ToFire = true;
    public float ExposionChances = 100;
    [Range(0, 1)]
    public float DamagePerc = 1;
    public bool EffectsVisisble = false;
    public bool TileEffectsVisisble = false;
    public bool SummonVisisble = false;
    public bool ChildrenExplosionVisisble = false;

    public bool Foldout = false;
    public bool HasEffect = false;
    public bool showImpact = true;
    [ConditionalField("HasEffect", false)] public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
    [ConditionalField("HasEffect", false)] public float EffectChances = 100;
    public bool IsEffectOnTile = false;
    [ConditionalField("IsEffectOnTile", false)] public TileEffectClass EffectsOnTile;
    public bool SpawnSummonOnTile = false;
    [ConditionalField("SpawnSummonOnTile", false)] public TileSummonClass SummonToSpawn;

    public BattleFieldAttackTileBaseClass()
    {

    }


    BattleFieldAttackTileBaseClass(Vector2Int pos)
    {
        Pos = pos;
    }

    public BattleFieldAttackTileBaseClass(Vector2Int pos, bool hasEffect, List<ScriptableObjectAttackEffect> effects, bool isEffectOnTile, TileEffectClass effectsOnTile, bool spawnSummonOnTile, TileSummonClass summonToSpawn)
    {
        Pos = pos;
        HasEffect = hasEffect;
        Effects = effects;
        IsEffectOnTile = isEffectOnTile;
        EffectsOnTile = effectsOnTile;
        SpawnSummonOnTile = spawnSummonOnTile;
        SummonToSpawn = summonToSpawn;
    }
}



[System.Serializable]
public class BattleFieldAttackTileClass : BattleFieldAttackTileBaseClass
{
   
    public BattleFieldAttackTileClass()
    {

    }
    public BattleFieldAttackTileClass(Vector2Int pos)
    {
        Pos = pos;
    }

    public BattleFieldAttackTileClass(Vector2Int pos, bool hasEffect, List<ScriptableObjectAttackEffect> effects, bool isEffectOnTile, TileEffectClass effectsOnTile, bool spawnSummonOnTile, TileSummonClass summonToSpawn)
    {
        Pos = pos;
        HasEffect = hasEffect;
        Effects = effects;
        IsEffectOnTile = isEffectOnTile;
        EffectsOnTile = effectsOnTile;
        SpawnSummonOnTile = spawnSummonOnTile;
        SummonToSpawn = summonToSpawn;
    }

    public BattleFieldAttackTileClass(BattleFieldAttackTileClass copy, Vector2Int pos)
    {
        Pos = pos;
        HasEffect = copy.HasEffect;
        ExposionChances = copy.ExposionChances;
        ScriptableObjectAttackEffect[] soE = new ScriptableObjectAttackEffect[copy.Effects.Count];
        copy.Effects.CopyTo(soE);
        Effects = soE.ToList();
        IsEffectOnTile = copy.IsEffectOnTile;
        EffectsOnTile = new TileEffectClass(copy.EffectsOnTile);
        SpawnSummonOnTile = copy.SpawnSummonOnTile;
        SummonToSpawn = new TileSummonClass(copy.SummonToSpawn);
        ChildrenExplosionVisisble = copy.ChildrenExplosionVisisble;

        ParticlesChildExplosionClass[] cE = new ParticlesChildExplosionClass[copy.ChildrenExplosion.Count];
        copy.ChildrenExplosion.CopyTo(cE);
        ChildrenExplosion = cE.ToList();
    }

    public List<ParticlesChildExplosionClass> ChildrenExplosion = new List<ParticlesChildExplosionClass>();

}

[System.Serializable]
public class BattleFieldAttackChildTileClass : BattleFieldAttackTileBaseClass
{

    public BattleFieldAttackChildTileClass()
    {

    }

    public BattleFieldAttackChildTileClass(Vector2Int pos)
    {
        Pos = pos;
    }

   
    public BattleFieldAttackChildTileClass(BattleFieldAttackChildTileClass copy, Vector2Int pos)
    {
        Pos = pos;
        HasEffect = copy.HasEffect;
        ScriptableObjectAttackEffect[] soE = new ScriptableObjectAttackEffect[copy.Effects.Count];
        copy.Effects.CopyTo(soE);
        Effects = soE.ToList();
        IsEffectOnTile = copy.IsEffectOnTile;
        EffectsOnTile = new TileEffectClass(copy.EffectsOnTile);
        SpawnSummonOnTile = copy.SpawnSummonOnTile;
        SummonToSpawn = new TileSummonClass(copy.SummonToSpawn);
        ExposionChances = copy.ExposionChances;
    }

}

#endregion

#region Particles

[System.Serializable]
public class ParticlesAttackTypeClass : AttackTypeClass
{
    public int CurrentChances
    {
        get
        {
            return OverrideChances ? OverrideChancesValue : Chances;
        }
    }

    public BulletLevelType BulletLevel = BulletLevelType.Level_1;
    //[SerializeField] public Vector2Int RangeLevel = new Vector2Int(0, 10);
    public bool OverrideChances = false;
    public int OverrideChancesValue;
    public List<BulletBehaviourInfoClass> BulletTrajectories = new List<BulletBehaviourInfoClass>();
    public bool EffectOnCaster;
    public List<ScriptableObjectAttackEffect> EffectsOnCaster = new List<ScriptableObjectAttackEffect>();
}


[System.Serializable]
public class BulletBehaviourInfoClass
{
    public int AttackChances = 100;

    public Vector2Int BulletDistanceInTile;
    public bool OverrideIndicatorPosition;
    public Vector2Int OverrideBulletDistanceInTileIndicator;
    public float TimeMultiplier = 1;
    public AnimationCurve Trajectory_Speed = new AnimationCurve(AnimationCurve.Linear(0,0,1,1).keys);
    public AnimationCurve Trajectory_Y = new AnimationCurve(AnimationCurve.Linear(0,0,1,1).keys);
    public AnimationCurve Trajectory_Z = new AnimationCurve(AnimationCurve.Linear(0, 0, 1, 1).keys);

    public bool InfoVisible = false;
    public bool OverrideBulletLevel = false;
    public BulletLevelType BulletLevel = BulletLevelType.Level_1;
    public bool BulletGrenadeVisisble = false;
    public bool BulletPoopingVisisble = false;
    public bool BulletHomingVisisble = false;

    public bool EffectsVisisble = false;
    public bool TileEffectsVisisble = false;
    public bool SummonVisisble = false;
    public bool ChildrenExplosionVisisble = false;
    public bool OverrideBulletInfo = false;
    //NEW ------------------------
    public List<BulletType> _BulletT = new List<BulletType>();

    public List<BulletType> BulletT
    {
        get
        {
            return _BulletT.Count == 0 ? new List<BulletType>() { BulletType.Base } : _BulletT;
        }
        set
        {
            _BulletT = value;
        }
    }

    [HideInInspector] public float Grenade_ExplosionDelay = 3f;
    [HideInInspector] public bool Grenade_Defensible = false;
    [HideInInspector] public bool Grenade_CanBeDestroyed = false;
    [HideInInspector] public float Grenade_Health = 10f;
    [HideInInspector] public float Grenade_ExplosionDamageMultiplier = 1f;
    [HideInInspector] public bool Grenade_OverrideBulletEffects = false;
    [HideInInspector] public ParticlesType Grenade_ExplosionPS;
    [HideInInspector] public ParticlesType Grenade_ObjectPS;
    [HideInInspector] public List<Vector2Int> Grenade_ExplosionTiles = new List<Vector2Int>();
    [HideInInspector] public List<DirtyBombEffectsList> Grenade_DirtyBombEffects = new List<DirtyBombEffectsList>();

    [HideInInspector] public bool IsHoming = false;
    [HideInInspector] public float Homing_TimeTillExpiry = 5f;
    [HideInInspector] public float Homing_TurningSpeed = 3f;
    [HideInInspector] public float Homing_SlowDownMultiplier = 0.5f;

    [HideInInspector] public float ChancesOfPooping = 100;
    [HideInInspector] public bool InEnemySide = true;
    [HideInInspector] public TileEffectClass PoopingEffectsOnTile;

    public bool HasEffect = false;
    public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
    public float EffectChances = 100;
    [Range(0, 1)]
    public float DamagePerc = 1;
    public bool HasSummon = false;
    public TileSummonClass Summon = new TileSummonClass();
    public float SummonChances = 100;

    public bool IsEffectOnTile = false;
    [ConditionalField("IsEffectOnTile", false)] public TileEffectClass EffectsOnTile;

    public List<ParticlesChildExplosionClass> ChildrenExplosion = new List<ParticlesChildExplosionClass>();

    public BulletBehaviourInfoClass()
    {

    }

    public BulletBehaviourInfoClass(BulletBehaviourInfoClass copyFrom)
    {
        AttackChances = copyFrom.AttackChances;
        BulletDistanceInTile = copyFrom.BulletDistanceInTile;
        OverrideBulletDistanceInTileIndicator = copyFrom.OverrideBulletDistanceInTileIndicator;
        OverrideIndicatorPosition = copyFrom.OverrideIndicatorPosition;
        TimeMultiplier = copyFrom.TimeMultiplier;
        Trajectory_Speed = new AnimationCurve(copyFrom.Trajectory_Speed.keys);
        Trajectory_Y = new AnimationCurve(copyFrom.Trajectory_Y.keys);
        Trajectory_Z = new AnimationCurve(copyFrom.Trajectory_Z.keys);

        InfoVisible = copyFrom.InfoVisible;

        BulletGrenadeVisisble = copyFrom.BulletGrenadeVisisble;
        BulletPoopingVisisble = copyFrom.BulletPoopingVisisble;
        BulletHomingVisisble = copyFrom.BulletHomingVisisble;

        EffectsVisisble = copyFrom.EffectsVisisble;
        TileEffectsVisisble = copyFrom.TileEffectsVisisble;
        SummonVisisble = copyFrom.SummonVisisble;
        ChildrenExplosionVisisble = copyFrom.ChildrenExplosionVisisble;

        OverrideBulletInfo = copyFrom.OverrideBulletInfo;

        BulletType[] btTemp = new BulletType[copyFrom._BulletT.Count];
        copyFrom._BulletT.CopyTo(btTemp);
        _BulletT = btTemp.ToList();

        Grenade_ExplosionDelay = copyFrom.Grenade_ExplosionDelay;
        Grenade_Defensible = copyFrom.Grenade_Defensible;
        Grenade_CanBeDestroyed = copyFrom.Grenade_CanBeDestroyed;
        Grenade_Health = copyFrom.Grenade_Health;
        Grenade_ExplosionDamageMultiplier = copyFrom.Grenade_ExplosionDamageMultiplier;
        Grenade_OverrideBulletEffects = copyFrom.Grenade_OverrideBulletEffects;
        Grenade_ExplosionPS = copyFrom.Grenade_ExplosionPS;
        Grenade_ObjectPS = copyFrom.Grenade_ObjectPS;

        Vector2Int[] v2Temp = new Vector2Int[copyFrom.Grenade_ExplosionTiles.Count];
        copyFrom.Grenade_ExplosionTiles.CopyTo(v2Temp);
        Grenade_ExplosionTiles = v2Temp.ToList();

        DirtyBombEffectsList[] dbTemp = new DirtyBombEffectsList[copyFrom.Grenade_DirtyBombEffects.Count];
        copyFrom.Grenade_DirtyBombEffects.CopyTo(dbTemp);
        Grenade_DirtyBombEffects = dbTemp.ToList();

        IsHoming = copyFrom.IsHoming;
        Homing_TimeTillExpiry = copyFrom.Homing_TimeTillExpiry;
        Homing_TurningSpeed = copyFrom.Homing_TurningSpeed;
        Homing_SlowDownMultiplier = copyFrom.Homing_SlowDownMultiplier;

        ChancesOfPooping = copyFrom.ChancesOfPooping;
        InEnemySide = copyFrom.InEnemySide;
        PoopingEffectsOnTile = copyFrom.PoopingEffectsOnTile;

        HasEffect = copyFrom.HasEffect;

        ScriptableObjectAttackEffect[] EffectTemp = new ScriptableObjectAttackEffect[copyFrom.Effects.Count];
        copyFrom.Effects.CopyTo(EffectTemp);
        Effects = EffectTemp.ToList();

        

        copyFrom.Effects.CopyTo(Effects.ToArray());
        EffectChances = copyFrom.EffectChances;

        HasSummon = copyFrom.HasSummon;
        Summon = copyFrom.Summon;
        SummonChances = copyFrom.SummonChances;

        IsEffectOnTile = copyFrom.IsEffectOnTile;
        EffectsOnTile = copyFrom.EffectsOnTile;


        ParticlesChildExplosionClass[] psCTemp = new ParticlesChildExplosionClass[copyFrom.ChildrenExplosion.Count];
        copyFrom.ChildrenExplosion.CopyTo(psCTemp);
        ChildrenExplosion = psCTemp.ToList();
    }

}

[System.Serializable]
public class ParticlesChildExplosionClass
{
    public float ChildrenBulletDelay = 0.5f;
    public float ChildrenDamageMultiplier = 0.3f;
    public bool Show = true;

    [HideInInspector] public List<BattleFieldAttackChildTileClass> BulletEffectTiles = new List<BattleFieldAttackChildTileClass>();

    public ParticlesChildExplosionClass()
    {

    }

    public ParticlesChildExplosionClass(ParticlesChildExplosionClass copy)
    {
        ChildrenBulletDelay = copy.ChildrenBulletDelay;
        ChildrenDamageMultiplier = copy.ChildrenDamageMultiplier;
        Show = true;

        for (int i = 0; i < copy.BulletEffectTiles.Count; i++)
        {
            BulletEffectTiles.Add(new BattleFieldAttackChildTileClass(copy.BulletEffectTiles[i], copy.BulletEffectTiles[i].Pos));
        }

    }
}

#endregion


#region TileEffect

[System.Serializable]
public class TileEffectClass
{
    public TileActionType TileAction;
    [ConditionalField("TileAction", false, TileActionType.OverTime)] public float HitTime = 1;
    public ParticlesType TileParticlesID;
    public float EffectChances = 100;
    public float DurationOnTile;

    public Vector2 DurationOnTileV;

    public float DurOnTile
    {
        get
        {
            return DurationOnTileV != Vector2.zero ? Random.Range(DurationOnTileV.x, DurationOnTileV.y) : DurationOnTile;
        }
    }



    public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();

    public TileEffectClass()
    {
    }

    public TileEffectClass(TileActionType tileAction, float hitTime, ParticlesType tileParticlesID, float durationOnTile, List<ScriptableObjectAttackEffect> effects)
    {
        TileAction = tileAction;
        HitTime = hitTime;
        TileParticlesID = tileParticlesID;
        DurationOnTile = durationOnTile;
        Effects = effects;
    }

    public TileEffectClass(TileEffectClass tec)
    {
        TileAction = tec.TileAction;
        HitTime = tec.HitTime;
        TileParticlesID = tec.TileParticlesID;
        DurationOnTile = tec.DurationOnTile;
        ScriptableObjectAttackEffect[] abAtkBase = new ScriptableObjectAttackEffect[tec.Effects.Count];
        tec.Effects.CopyTo(abAtkBase);
        Effects = abAtkBase.ToList();
    }

    public TileEffectClass(TileEffectClass tec, List<ScriptableObjectAttackEffect> effects)
    {
        TileAction = tec.TileAction;
        HitTime = tec.HitTime;
        TileParticlesID = tec.TileParticlesID;
        DurationOnTile = tec.DurationOnTile;
        ScriptableObjectAttackEffect[] abAtkBase = new ScriptableObjectAttackEffect[effects.Count];
        effects.CopyTo(abAtkBase);
        Effects = abAtkBase.ToList();
    }
}

[System.Serializable]
public class TileSummonClass
{
    public float SpawnChances = 100;
    public bool UncappedDuration = true;
    [ConditionalField("UncappedDuration", true)] public Vector2 DurationOnField;
    public Vector2Int[] SummonSpawnPositions = new Vector2Int[0];
    public CharacterNameType CharToSummon = CharacterNameType.None;
    public bool hasCharOverrides = false;
    public BaseInfoInjectorClass CharOverrides = new BaseInfoInjectorClass();

    public TileSummonClass()
    {

    }

    public TileSummonClass(float effectChances, bool uncappedDuration, Vector2 durationOnField, Vector2Int[] summonSpawnPositions, CharacterNameType charToSummon)
    {
        SpawnChances = effectChances;
        UncappedDuration = uncappedDuration;
        DurationOnField = durationOnField;
        SummonSpawnPositions = summonSpawnPositions;
        CharToSummon = charToSummon;
    }

    public TileSummonClass(TileSummonClass tileSummonClass)
    {
        SpawnChances = tileSummonClass.SpawnChances;
        UncappedDuration = tileSummonClass.UncappedDuration;
        DurationOnField = tileSummonClass.DurationOnField;
        SummonSpawnPositions = tileSummonClass.SummonSpawnPositions;
        CharToSummon = tileSummonClass.CharToSummon;
    }
}

#endregion


[System.Serializable]
public class DirtyBombEffectsList
{
    public List<ScriptableObjectAttackEffect> list = new List<ScriptableObjectAttackEffect>();
}


[System.Serializable]
public class SlowDownOnHitClass
{
   public float TimeEffect = 0.1f;
   public float DurationOfTimeEffect = 1f;
   public float TimeEffectDelay = 1f;
   public bool TimeEffectChildExplosion = false;
   public bool ApplyImpactSlowToAttacker = false;
}
