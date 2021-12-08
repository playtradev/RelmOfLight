using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseInfoScript : MonoBehaviour
{
    #region Events
    public delegate void BaseSpeedChanged(float baseSpeed);
    public event BaseSpeedChanged BaseSpeedChangedEvent;
    public delegate void CharacterEventStructure(BaseInfoScript charInfo);
    public event CharacterEventStructure DeathEvent;
    public event CharacterEventStructure ShieldDepletedEvent;
    public event CharacterEventStructure MaskReadyEvent;

    public delegate void ValueUpdate(BaseInfoScript charInfo, float perc = -999f, float val = -999, bool instantChange = false);
    public event ValueUpdate HealthChangedEvent;
    public event ValueUpdate ShieldChangedEvent;
    public event ValueUpdate MaskChargeChangedEvent;
    public event ValueUpdate StaggeredProgressChangedEvent;

  

    //INTERFACES FOR SOME BULLSHIT
    protected void MaskValueChanged(float perc, float val) => MaskChargeChangedEvent?.Invoke(this, perc, val);
    protected void MaskReady() => MaskReadyEvent?.Invoke(this);
    #endregion

    [HideInInspector] public bool isOnField;

    [HideInInspector] public WaveNPCTypes NPCType = WaveNPCTypes.None;

    [Header("Identity_________________________________________________")]
    public string Name;
    public CharacterNameType CharacterID;
    [HideInInspector] public int CharInstanceID = -1;

    [Header("Grid Positioning_________________________________________")]
    public List<Vector2Int> _Pos = new List<Vector2Int>();
    public List<Vector2Int> Pos
    {
        get
        {
            _Pos.Clear();
            foreach (Vector2Int item in OccupiedTiles)
            {
                _Pos.Add(item + CurrentTilePos);
            }
            return _Pos;
        }
    }

    public List<Vector2Int> OccupiedTiles
    {
        get
        {
            return _OccupiedTiles;
        }
        set
        {
            foreach (Vector2Int item in value)
            {
                if (item == Vector2Int.zero)
                {

                }
            }
            _OccupiedTiles = value;
        }
    }
    public List<Vector2Int> _OccupiedTiles = new List<Vector2Int>();

    public Vector2Int CurrentTilePos
    {
        get
        {
            return _CurrentTilePos;
        }
        set
        {
            _CurrentTilePos = value;
        }
    }
    public Vector2Int _CurrentTilePos;
    
    [Tooltip("Set externally at runtime")] public TeamSideType Side;
    public TeamSideType RivalSide
    {
        get
        {
            return Side == TeamSideType.LeftSideTeam ? TeamSideType.RightSideTeam : TeamSideType.LeftSideTeam;
        }
    }
    [Tooltip("Set externally at runtime")] public WalkingSideType WalkingSide;
    [Tooltip("Set externally at runtime")] public FacingType Facing;

    [Header("Layering_________________________________________________")]
    public bool UseLayeringSystem = true;

    [Header("Vitality_________________________________________________")]
    public HealthStastsClass HealthStats;
    [System.Serializable]
    public class HealthStastsClass
    {
        public float Health;
        public float B_Health;
        public float BaseHealthModifier;

        public float Current_B_Health
        {
            get
            {
                return B_Health + (B_Health * BaseHealthModifier);
            }
        }


        public List<ArmourType> ArmourT = new List<ArmourType>();

        public float Armour = 1;
        public float B_Armour = 1;
        public float BaseArmourModifier;
        public float Current_B_Armour
        {
            get
            {
                return B_Armour + (B_Armour * BaseArmourModifier);
            }
        }



        public List<ArmourType> B_ArmourT = new List<ArmourType>();

    }
    public float HealthPerc
    {
        get
        {
            return (Health * 100) / (HealthStats.Current_B_Health == 0f ? 1f : HealthStats.Current_B_Health);
        }
    }
    public float Health
    {
        get
        {
            return HealthStats.Health;
        }
        set
        {
            if (value > HealthStats.Current_B_Health)
            {
                value = HealthStats.Current_B_Health;
            }
            if (value <= 0)
            {
                value = value <= 0 ? 0 : value;
            }

            if (HealthStats.Health != value)
            {
               // VitalityBox?.CurrentChar_HealthChange(this, HealthPerc, value, !(Mathf.Abs(HealthStats.Health - value) > (HealthStats.Regeneration / 5)));
                HealthStats.Health = value;
                HealthChangedEvent?.Invoke(this, HealthPerc, value, Mathf.Abs(HealthStats.Health - value) < 1);

            }

            if (HealthStats.Health <= 0)
            {
                if (HealthStats.Health > 0)
                    return;
                DeathEvent?.Invoke(this);
            }
        }
    }

  

    public void FireHealthChangedEvent()
    {
        HealthChangedEvent?.Invoke(this, HealthPerc, Health);
    }

    public int ManaCostN = 2;


    public SpeedStastsClass SpeedStats;
    [System.Serializable]
    public class SpeedStastsClass
    {
        public float BaseSpeed = 1;
        public float BaseBaseSpeedModifier;
        [HideInInspector] public float B_BaseSpeed = 1;

        public float Current_B_BaseSpeed
        {
            get
            {
                return B_BaseSpeed + (B_BaseSpeed * BaseBaseSpeedModifier);
            }
        }
        public bool IsForcedValue = false;
        public float ForcedValue = 0;

        public float CurrentMovementTime
        {
            get
            {
                return BattleManagerScript.Instance.BaseActionTime / MovementTime;
            }
        }

        public float MovementTime = 1;
        [HideInInspector] public float B_MovementTime = 1;

        public float CurrentAttackTime
        {
            get
            {
                return BattleManagerScript.Instance.BaseActionTime / AttackTime;
            }
        }

        public float AttackTime = 1;
        [HideInInspector] public float B_AttackTime = 1;
        public float MovementSpeed = 1;
        public float BaseMovementSpeedModifier;
        [HideInInspector] public float B_MovementSpeed = 1;
        public float Current_B_MovementSpeed
        {
            get
            {
                return B_MovementSpeed + (B_MovementSpeed * BaseMovementSpeedModifier);
            }
        }

        [Range(0, 2)]
        public float TileMovementTime = 1;
        [Range(0, 10)]
        public float MovementCost = 2;

        public float CalculatedSpeed
        {
            get
            {
                return MovementSpeed / TileMovementTime;
            }
        }

        public float CuttingPerc = 0.85f;

        public float IntroPerc = 0.15f;
        public float LoopPerc = 0.70f;
        public float EndPerc = 0.15f;


        [Range(0, 1)]
        public float AttackLoopDuration = 0.5f;
        public bool UpdateLoopDurationForTileAttack = false;
        [ConditionalField("UpdateLoopDurationForTileAttack", false)]
        [Range(0, 1)]
        public float AttackLoopDurationTileAttack = 0.5f;
        [Tooltip("Value used to slow down the loop animation when holding the attack")]
        [Range(0, 1)]
        public float SlowDownPercentageOnHolding = 0.3f;
        [Range(0, 1)]
        public float IdleToAttackDuration = 0.01f;


        public bool OverrideAtkToIdleDuration = false;
        public float AttackToIdleDuration = 0.01f;

        public bool Override_Buff_AtkToIdleDuration = false;
        public float Buff_AttackToIdleDuration = 0.01f;

        public bool Override_Debuff_AtkToIdleDuration = false;
        public float Debuff_AttackToIdleDuration = 0.01f;

        public float _LeaveAnimSpeed = 10;
        public float _ArriveAnimSpeed = 10;

        public float LeaveAnimSpeed
        {
            get
            {
                return _LeaveAnimSpeed;
            }
            set
            {
                _LeaveAnimSpeed = value;
            }
        }
        public float ArriveAnimSpeed
        {
            get
            {
                return _ArriveAnimSpeed;
            }
            set
            {
                _ArriveAnimSpeed = value;
            }
        }

        public Vector2 ReactionTime = new Vector2(0, 0.3f);

        [HideInInspector] public float ReactionTimeValue
        {
            get
            {
                return Random.Range(ReactionTime.x, ReactionTime.y);
            }
        }

        public float WeakBulletSpeed = 5;
        public Vector2 WeakBulletSpeedV;

        public float WeakBulletS
        {
            get
            {
                return WeakBulletSpeedV != Vector2.zero ? Random.Range(WeakBulletSpeedV.x, WeakBulletSpeedV.y) : WeakBulletSpeed;
            }
        }

        public float StrongBulletSpeed = 5;
        public Vector2 StrongBulletSpeedV;

        public float StrongBulletS
        {
            get
            {
                return StrongBulletSpeedV != Vector2.zero ? Random.Range(StrongBulletSpeedV.x, StrongBulletSpeedV.y) : StrongBulletSpeed;
            }
        }


       
        [HideInInspector] public float B_AttackSpeed = 1;
        [HideInInspector] public float B_LeaveAnimSpeed = 3;
        [HideInInspector] public float B_ArriveAnimSpeed = 3;
        [HideInInspector] public float B_WeakBulletSpeed = 5;
        [HideInInspector] public float B_StrongBulletSpeed = 5;
        [HideInInspector] public Vector2 B_WeakBulletSpeedV = Vector2.zero;
        [HideInInspector] public Vector2 B_StrongBulletSpeedV = Vector2.zero;
        [HideInInspector] public float B_AttackLoopDuration = 0.5f;

    }
    public float BaseSpeed
    {
        get
        {
            //return SpeedStats.BaseSpeed <= 0 ? 0 : IsTired ? SpeedStats.BaseSpeed * UniversalGameBalancer.Instance.BaseSpeedSlowDownOnLowEther_Multiplier : SpeedStats.BaseSpeed;
            return SpeedStats.IsForcedValue ? SpeedStats.ForcedValue : SpeedStats.BaseSpeed;
        }
        set
        {
            BaseSpeedChangedEvent?.Invoke(value);
            SpeedStats.BaseSpeed = value;
        }
    }

    public DamageStastsClass DamageStats;
    [System.Serializable]
    public class DamageStastsClass
    {
        public float BaseDamage = 10;
        [HideInInspector] public float B_BaseDamage = 10f;
        [HideInInspector] public List<ElementalResistenceClass> ElementalsResistence = new List<ElementalResistenceClass>();
        [HideInInspector] public ElementalType CurrentElemental;
    }

    public virtual void SetupChar(BaseInfoInjectorClass info = null, float strengthScaler = 1f, bool updateWithLevel = true, bool useSigils = true)
    {
     
    }


    public float StatsMultipler(float b_Value, float multiplier)
    {
        return b_Value * multiplier;
    }

    public float GetBuffDebuffValue(PassiveSkillsValueType valueType, float value, float baseValue)
    {
        return valueType == PassiveSkillsValueType.Multiplier ? StatsMultipler(baseValue, value) :
            value;
    }
}

[System.Serializable]
public class BaseInfoInjectorClass //THIS HAS AN EDITOR COMPONENT IN THE ATTACK TYPE EDITOR THAT SHOULD BE UPDATED IF THIS IS CHANGED
{
    public bool OverrideHealth = false;
    public float Health;
    public bool OverrideHealthRegeneration = false;
    public float HealthRegeneration;

    public bool OverrideArmour = false;
    public float Armour;
    public bool OverrideShieldRegeneration = false;
    public float ShieldRegeneration;


    public bool OverrideEther = false;
    public float Ether;
    public bool OverrideEtherRegeneration = false;
    public float EtherRegeneration;

    public bool OverrideSpeed = false;
    public float MovementSpeed;
    public bool OverrideEvasion = false;
    public Vector2 Evasion;


    public bool OverrideSigilDropBonus = false;
    public float SigilDropBonus;

    public bool OverrideCriticalWeakBullet = false;
    public Vector2 CriticalWeakBullet;
    public bool OverrideCriticalStrongBullet = false;
    public Vector2 CriticalStrongBullet;

    public bool OverrideWeakAttackMultiplier = false;
    public float WeakAttackMultiplier;
    public bool OverrideStrongAttackMultiplier = false;
    public float StrongAttackMultiplier;

    public bool OverrideHue = false;
    public ColorHueSat ColorHueSaturation;

    public bool OverrideAggroMultiplier = false;
    public int AggroMultiplier = 1;


    public bool overrideDeath = false;
    [ConditionalField("overrideDeath")] public DeathBehaviourType deathAnim = DeathBehaviourType.Explosion;

    public bool overrideAttacks = false; //NOT DISPLAYED IN THE EDITOR VERSION YET
    [ConditionalField("overrideAttacks")] public List<ScriptableObjectAttackBase> AttacksToAdd = new List<ScriptableObjectAttackBase>();
    [ConditionalField("overrideAttacks")] public bool AddToBasicAttacks = false;

    public bool isEqualTo(BaseInfoInjectorClass info)
    {
        if (OverrideHealth != info.OverrideHealth) return false;
        if (OverrideEther != info.OverrideEther) return false;
        if (OverrideHealthRegeneration != info.OverrideHealthRegeneration) return false;
        if (OverrideSpeed != info.OverrideSpeed) return false;
        if (OverrideEtherRegeneration != info.OverrideEtherRegeneration) return false;
        if (overrideDeath != info.overrideDeath) return false;
        if (overrideAttacks != info.overrideAttacks) return false;
        if (OverrideStrongAttackMultiplier != info.OverrideStrongAttackMultiplier) return false;
        if (OverrideWeakAttackMultiplier != info.OverrideWeakAttackMultiplier) return false;
        if (OverrideArmour != info.OverrideArmour) return false;
        if (OverrideShieldRegeneration != info.OverrideShieldRegeneration) return false;
        if (OverrideEvasion != info.OverrideEvasion) return false;
        if (OverrideCriticalStrongBullet != info.OverrideCriticalStrongBullet) return false;
        if (OverrideCriticalWeakBullet != info.OverrideCriticalWeakBullet) return false;
        if (OverrideHue != info.OverrideHue) return false;
        if (OverrideAggroMultiplier != info.OverrideAggroMultiplier) return false;


        if (Health != info.Health) return false;
        if (HealthRegeneration != info.HealthRegeneration) return false;
        if (Armour != info.Armour) return false;
        if (ShieldRegeneration != info.ShieldRegeneration) return false;
        if (Ether != info.Ether) return false;
        if (EtherRegeneration != info.EtherRegeneration) return false;
        if (WeakAttackMultiplier != info.WeakAttackMultiplier) return false;
        if (StrongAttackMultiplier != info.StrongAttackMultiplier) return false;
        if (Evasion != info.Evasion) return false;
        if (CriticalWeakBullet != info.CriticalWeakBullet) return false;
        if (CriticalStrongBullet != info.CriticalStrongBullet) return false;
        if (MovementSpeed != info.MovementSpeed) return false;
        if (deathAnim != info.deathAnim) return false;
        if (AttacksToAdd != info.AttacksToAdd) return false;
        if (AddToBasicAttacks != info.AddToBasicAttacks) return false;
        if (ColorHueSaturation.color != info.ColorHueSaturation.color) return false;
        if (ColorHueSaturation.hue != info.ColorHueSaturation.hue) return false;
        if (ColorHueSaturation.sat != info.ColorHueSaturation.sat) return false;
        if (AggroMultiplier != info.AggroMultiplier) return false;
        return true;
    }
}