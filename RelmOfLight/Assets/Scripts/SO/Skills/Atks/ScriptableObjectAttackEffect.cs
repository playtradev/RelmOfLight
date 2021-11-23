using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect/AttackEffect")]
public class ScriptableObjectAttackEffect : ScriptableObject
{

    public float Duration
    {
        get
        {
            return Random.Range(_Duration.x, _Duration.y);
        }
    }

    public float Delay
    {
        get
        {
            return Random.Range(_Delay.x, _Delay.y);
        }
    }

    public float OvertimeRatio
    {
        get
        {
            return Random.Range(_OvertimeRatio.x, _OvertimeRatio.y);
        }
    }



    [Header("General")]
    [Range(1, 4)] public int level = 1;
    public BuffDebuffStackType StackType = BuffDebuffStackType.Refreshable;
    [ConditionalField("StackType", false, BuffDebuffStackType.Stackable)] public int maxStack = 3;

    public bool OldSystem = true;

    public BuffDebuffStatsType StatsToAffect;

    public StatsCheckerType StatsChecker;
    public string NameShowedOnIndicator = "";
    public Vector2 Value = Vector2.one;

    public Vector2 _Duration;
    public Vector2 _Delay = Vector2.zero;

    public Vector2 _OvertimeRatio = Vector2.one;

    public CharacterAnimationStateType AnimToFired = CharacterAnimationStateType.none;
    
    public List<StatsToAffectClass> StatsToAffectList = new List<StatsToAffectClass>();


    [Header("Particles")]
    public ParticlesType Particles;

    public bool AttachPsToHead = false;

    public ParticlesType TeleportParticlesIn = ParticlesType.CharArrivingSmoke;
    public ParticlesType TeleportParticlesOut = ParticlesType.CharArrivingSmoke;

    public CharacterAnimationStateType AnimEnumTeleportArrivingAnim = CharacterAnimationStateType.Idle;
    public bool UseAnimEnumOrStringAnim =  true;
    public string StringTeleportArrivingAnim = "Idle";
    public string TeleportArrivingAnim
    {
        get
        {
            return !UseAnimEnumOrStringAnim ? StringTeleportArrivingAnim : AnimEnumTeleportArrivingAnim.ToString();
        }
    }

    public bool TeleportInRandom = true;
    public Vector2Int FixedPos;


    public ParticlesType MeleeGoTeleportParticlesIn = ParticlesType.CharArrivingSmoke;
    public ParticlesType MeleeGoTeleportParticlesOut = ParticlesType.CharArrivingSmoke;
    public ParticlesType MeleeBackTeleportParticlesIn = ParticlesType.CharArrivingSmoke;
    public ParticlesType MeleeBackTeleportParticlesOut = ParticlesType.CharArrivingSmoke;

    public ParticlesType MeleeAttackParticles = ParticlesType.DonnaMeleeSlash;

    [Header("UI Display")]
    public StatusEffectType classification = StatusEffectType.Buff;
    public bool ShowIcon = false;
    public Sprite icon;
   

    public bool recolorCharUI = false;
    [ConditionalField("recolorCharUI")] public Color statusIconColor = Color.magenta;

    public bool SetParticlesOnCaster = false;
    public ParticlesType ParticlesOnCaster;

    public bool SetIconOnCaster = false;
    public StatusEffectType OnCasterClassification = StatusEffectType.Buff;
    public Sprite OnCasterIcon = null;
    public bool OnCasterRecolorCharUI = false;
    [ConditionalField("recolorCharUI")] public Color OnCasterStatusIconColor = Color.magenta;


    [HideInInspector] public ScriptableObjectAttackBase Atk;
    public AIType ForcedAI;
    public CharLevelHueScaleInfo ColorSize;
    public bool OnDuration = true;
    public AnimationCurve ColorSizeCurve;
    public bool BoolValue = false;
    public WalkingSideType WalkingSide;
    public ArmourType ArmourT;
    public bool BaseCurrentValue = true;
    public ElementalType Elemental;
    public AttackInputType[] AttackToSteal;
    public bool OnCaster = false;
    public CharacterNameType ClonePrefab;  //If left to null, the basic character will be used in a nerfed state
    public bool OverrideAI = false;
    public float ClonePowerScale = 0.5f; //How much of a nerf the clone receives upon creation
    public bool CloneAsManyAsCurrentEnemies = false;
    public int CloneAmount = 1;
    public bool SpawnInClosePosition = true;
    public ScriptableObjectAttackEffect CloneStartingEffect;
}


[System.Serializable]
public class StatsToAffectClass
{
    public BuffDebuffStatsType StatsToAffect;
    public StatsCheckerType StatsChecker;
    public Vector2 _Value = Vector2.one;

    public float Value
    {
        get
        {
            return Random.Range(_Value.x, _Value.y);
        }
    }
    public bool useDelay = true;
    public bool Show = false;
    public bool BoolValue = false;
    public CharLevelHueScaleInfo ColorSize;
    public bool OnDuration = true;
    public AnimationCurve ColorSizeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public ScriptableObjectAttackBase Atk;
    public WalkingSideType WalkingSide;
    public AIType ForcedAI;
    public ArmourType ArmourT;
    public bool BaseCurrentValue = true;
    public ElementalType Elemental;
    public CharacterNameType ClonePrefab;  //If left to null, the basic character will be used in a nerfed state
    public bool OverrideAI = false;
    public float ClonePowerScale = 0.5f; //How much of a nerf the clone receives upon creation
    public bool CloneAsManyAsCurrentEnemies = false;
    public int CloneAmount = 1;
    public bool SpawnInClosePosition = true;
    public ScriptableObjectAttackEffect CloneStartingEffect;
    public bool OnCaster = false;
    public AttackInputType[] AttackToSteal;

    public StatsToAffectClass(StatsToAffectClass copy, ScriptableObjectAttackBase atk)
    {
        StatsToAffect = copy.StatsToAffect;
        StatsChecker = copy.StatsChecker;
        _Value = copy._Value;
        BoolValue = copy.BoolValue;
        Atk = atk;
        BaseCurrentValue = copy.BaseCurrentValue;
        WalkingSide = copy.WalkingSide;
        ArmourT = copy.ArmourT;
        Elemental = copy.Elemental;
        ClonePrefab = copy.ClonePrefab;  //If left to null, the basic c
        ClonePowerScale = copy.ClonePowerScale; //How much of a nerf the clone r
        CloneAsManyAsCurrentEnemies = copy.CloneAsManyAsCurrentEnemies;
        CloneAmount = copy.CloneAmount;
        SpawnInClosePosition = copy.SpawnInClosePosition;
        CloneStartingEffect = copy.CloneStartingEffect;
        OnCaster = copy.OnCaster;
        useDelay = copy.useDelay;
        ColorSize = copy.ColorSize;
        OnDuration = copy.OnDuration;
        ColorSizeCurve = copy.ColorSizeCurve;
        ForcedAI = copy.ForcedAI;
        AttackToSteal = copy.AttackToSteal;
    }


    public StatsToAffectClass(ScriptableObjectAttackEffect copy, ScriptableObjectAttackBase atk)
    {
        StatsToAffect = copy.StatsToAffect;
        StatsChecker = copy.StatsChecker;
        _Value = copy.Value;
        BoolValue = copy.BoolValue;
        Atk = atk;
        BaseCurrentValue = copy.BaseCurrentValue;
        WalkingSide = copy.WalkingSide;
        ArmourT = copy.ArmourT;
        Elemental = copy.Elemental;
        ClonePrefab = copy.ClonePrefab;  //If left to null, the basic c
        ClonePowerScale = copy.ClonePowerScale; //How much of a nerf the clone r
        CloneAsManyAsCurrentEnemies = copy.CloneAsManyAsCurrentEnemies;
        CloneAmount = copy.CloneAmount;
        SpawnInClosePosition = copy.SpawnInClosePosition;
        CloneStartingEffect = copy.CloneStartingEffect;
        OnCaster = copy.OnCaster;
        useDelay = true;
        OnDuration = copy.OnDuration;
        ColorSize = copy.ColorSize;
        ColorSizeCurve = copy.ColorSizeCurve;
        ForcedAI = copy.ForcedAI;
        AttackToSteal = copy.AttackToSteal;
    }


    public StatsToAffectClass()
    {

    }
}