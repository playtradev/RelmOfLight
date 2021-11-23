using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

/// <summary>
/// This class GridFight_ContainsStruct the basic info of the character
/// </summary>
/// 

public class CharacterInfoScript : BaseInfoScript
{
    #region Events
    public delegate void BaseSwappableSOChanged();
    public event BaseSwappableSOChanged BaseSwappableSOChangedEvent;
    public delegate void CharEventHasHappened(string CharEventID, CharacterInfoScript charInfo);
    public event CharEventHasHappened CharEventHasHappenedEvent;
    #endregion

    [Header("Requisite Information____________________________________")]
    public ScriptableObjectSwappableBases SwappableBases;

    public ScriptableObjectAttackBase ReflectedAttack;


    [Header("Character Information____________________________________")]
    public CharacterClassType ClassType;
    public BaseCharType BaseChar = BaseCharType.BaseCharacter;
    [Tooltip("Should contain 2 for the HealthUI, one for deselected(0) and one for selected(1)")] public Sprite CharacterIcon;

    [Header("Relations________________________________________________")]
    public ElementalType Elemental;
    public List<RelationshipClass> RelationshipList = new List<RelationshipClass>();

    [Header("Adjustments______________________________________________")]
    [Tooltip("Set by the SCENE LOAD MANAGER")] public LevelType CharaterLevel;

    [SerializeField] public bool OverrideBaseLevelHueScaleInfo = false;
    [SerializeField] public CharLevelHueScaleInfo[] characterLevelHueScaleInfos = new CharLevelHueScaleInfo[10];
	public CharLevelHueScaleInfo[] CharacterLevelHueScaleInfos => characterLevelHueScaleInfos;

    
    public BattleState BattleState;

    public ColorHueSat BaseColorHueSat;


    [Tooltip("Blacklist: Immune to all but not immune to these, WhiteList: Only immune to these")]
    public ExclusionType BuffDebuffImmunitiesListType = ExclusionType.WhiteList;
    public List<BuffDebuffStatsType> BuffDebuffImmunities = new List<BuffDebuffStatsType>();
    public List<BuffDebuffStatsType> BuffDebuffImmunitiesModifier = new List<BuffDebuffStatsType>();
    public List<BuffDebuffStatsType> CurrentBuffDebuffImmunities
    {
        get
        {
            return BuffDebuffImmunities.Union(BuffDebuffImmunitiesModifier).ToList();
        }
    }

    public bool HasImmunityTo(BuffDebuffStatsType immunity)
    {
        if (CurrentBuffDebuffImmunities.Count > 0)
        {
            return BuffDebuffImmunitiesListType == ExclusionType.WhiteList ? CurrentBuffDebuffImmunities.GridFight_ContainsStruct(immunity) : !CurrentBuffDebuffImmunities.GridFight_ContainsStruct(immunity);
        }
        else
        {
            return false;
        }
    }


    [Header("SKILLS SETUP")]
    public ScriptableObjectSkillBase[] SkillLevels = new ScriptableObjectSkillBase[10];
    public int LevelRequiredForSkill(ScriptableObjectSkillBase skill)
    {
        for (int i = 0; i < SkillLevels.Length; i++)
        {
            if (SkillLevels[i] != null && skill.name.Contains(SkillLevels[i].name))
                return i + 1;
        }
        return 0;
    }

    [Header("Passive Skills__________________________________________________")]
    [SerializeField] public ScriptableObjectPassiveSkill[] PassiveSkills = new ScriptableObjectPassiveSkill[0];
    public PassiveSkillClass[] CurrentPassiveSkills;

    [Header("Attacks__________________________________________________")]
    public List<AttackSequence> attackSequences = new List<AttackSequence>();
    public ScriptableObjectAttackBase[] NextAttackSequence
    {
        get
        {
            AttackSequence atkSq = attackSequences.Where(r => r.CheckTrigger(this)).FirstOrDefault();
            return atkSq != null ? atkSq.GetAttackSequence() : null;
        }
    }
    [Tooltip("Attacks must follow this sequence (particles attack)  WEAK/STRONG/Skills (enemy attack) no order")]
    public List<ScriptableObjectAttackBase> BaseAttackTypeInfo = new List<ScriptableObjectAttackBase>();
    public List<ScriptableObjectAttackBase> _CurrentAttackTypeInfo = new List<ScriptableObjectAttackBase>();
    public List<ScriptableObjectAttackBase> CurrentAttackTypeInfo
    {
        get
        {
           
            List<ScriptableObjectAttackBase> t = new List<ScriptableObjectAttackBase>();
            t.AddRange(_CurrentAttackTypeInfo);
            t = t.Distinct().ToList();
            return t;
        }
        set
        {
            _CurrentAttackTypeInfo = value;
        }
    }
 
    [Header("Death Handling___________________________________________")]
    [Tooltip("The reward this unit's attacker receives upon the death of it")] public List<DeathDropInfoScript> DeathDrops = new List<DeathDropInfoScript>();

    [Tooltip("Length of time, in seconds, that the character will take to respawn once killed")]
    public float CharacterRespawnLengthNew = 20f;
    public float ModifierCharacterRespawnLength = 0f;

    public float CurrentCharacterRespawnLength
    {
        get
        {
            return CharacterRespawnLengthNew;
        }
    }

    public int _Coins = 1;

    public bool OverrideSigilsTypes = false;
    public float _CoinsMultiplier = 1;
    public float CoinsMultiplierModifier = 0;
    public float CoinsMultiplier
    {
        get
        {
            return _CoinsMultiplier + CoinsMultiplierModifier;
        }
        set
        {
            _CoinsMultiplier = value;
        }
    }

    [Header("Arrival Flair____________________________________________")]
    public ParticlesType ArrivingParticles = ParticlesType.CharArrivingSmoke;

    [Header("Aesthetic Setup__________________________________________")]
    public Transform Head;

    [Header("Control Setup____________________________________________")]
    [Tooltip("Set externally at runtime")] public List<ControllerType> PlayerController = new List<ControllerType>();
    [Tooltip("Set externally at runtime")] public bool IsBornOfWave = false;
    [Tooltip("Set externally at runtime")] public bool IsSummon = false;
    [HideInInspector] public float RecruitedTime = 0f;
    public bool isPlayerControlledCharacter => !IsSummon && !IsBornOfWave && !PlayerController.GridFight_ContainsStruct(ControllerType.None) && !PlayerController.GridFight_ContainsStruct(ControllerType.Enemy);


    public List<BulletTypeModifierClass> BulletTypeModifier = new List<BulletTypeModifierClass>();
    public CharActionBeheaviourClass Behaviour;
    [System.Serializable]
    public class CharActionBeheaviourClass
    {
        public InputBehaviourType InputBehaviour;
        public MovementActionType MovementActionN = MovementActionType.LeftRight;
        public DeathBehaviourType DeathBehaviour;
    }

    //DEATH EXPLOSION STUFF
    public ParticlesType DeathExplosion_OverrideParticles = ParticlesType.None;
    public float DeathExplosion_Delay = 0f;
    public float DeathDisableing_Delay = 0.5f;
    public bool Death_UseSound = true;
    //

    public List<global::CharacterEvent> CharacterEvents = new List<global::CharacterEvent>();

    //===========================================================


    //FORMES____FORMES____FORMES____FORMES____FORMES____FORMES____FORMES____FORMES____FORMES____FORMES____FORMES
    protected bool noCharFormeInfoScript = false;
    protected CharacterFormesScript charFormeInfo = null;
    public CharacterFormesScript CharFormeInfo
    {
        get
        {
            if (!noCharFormeInfoScript && charFormeInfo == null)
            {
                charFormeInfo = GetComponentInChildren<CharacterFormesScript>();
                if (charFormeInfo == null) noCharFormeInfoScript = true;
            }
            return charFormeInfo;
        }
    }
    public string CurFormeAnimPrefix => CharFormeInfo != null && CharFormeInfo.CurrentForme != null ? CharFormeInfo.CurrentForme.AnimationPrefix : "";
    public string CurFormeAnimSuffix => CharFormeInfo != null && CharFormeInfo.CurrentForme != null ? CharFormeInfo.CurrentForme.AnimationSuffix : "";
    //===========================================================






    private void Awake()
    {
        SwappableBases = Instantiate(SwappableBases);
        foreach (ScriptableObjectSwappableBase item in SwappableBases.Bases)
        {
            SwappableBases.RuntimeBases.Add(new RuntimeBasesClass(item.SwappableType.ToString(), Instantiate(item)));
        }
        List<ScriptableObjectAttackBase> t = new List<ScriptableObjectAttackBase>();
        foreach (ScriptableObjectAttackBase item in CurrentAttackTypeInfo)
        {
            if (item != null)
            {
                t.Add(Instantiate(item));
                t.Last().AttackOwner = CharacterID;
                BaseAttackTypeInfo.Add(t.Last());
            }
        }
        BattleState = BattleManagerScript.Instance.CurrentBattleState;
        BattleManagerScript.Instance.CurrentBattleStateChangedEvent += Instance_CurrentBattleStateChangedEvent;

        CurrentAttackTypeInfo = t;
    }

    private void OnEnable()
    {
        foreach (CharacterEvent CharacterEvent in CharacterEvents) CharacterEvent.Initialize(this);
    }

    private void OnDisable()
    {
        foreach (CharacterEvent CharacterEvent in CharacterEvents) CharacterEvent.Reset(this);
    }

    public void OnValidate()
    {
        if (Application.isPlaying)
        {
            BaseSwappableSOChangedEvent?.Invoke();
        }
        foreach (AttackSequence atkSeq in attackSequences)
        {
            atkSeq.GenerateName();
        }
        foreach (CharacterEvent CharacterEvent in CharacterEvents) if (!Application.isPlaying) CharacterEvent.OnValidate();
    }


    private void Instance_CurrentBattleStateChangedEvent(BattleState currentBattleState)
    {
        BattleState = currentBattleState;
    }
    int temp_int;


    List<PassiveSkillClass> temp_PassiveSkill = new List<PassiveSkillClass>();
    int temp_Int = 0;
   
    CharacterInfoScript _BaseValues = null;
    CharacterInfoScript BaseValues
    {
        get
        {
            if (_BaseValues == null)
                _BaseValues = BattleManagerScript.Instance.GetCharacterPrefab(CharacterID).GetComponentInChildren<CharacterInfoScript>();
            return _BaseValues;
        }
    }

    public override void SetupChar(BaseInfoInjectorClass info = null, float strengthScaler = 1, bool updateWithLevel = true, bool useSigils = true)
    {
        SetBase();
    }

   

    private ScriptableObjectAttackBase GetAttackByName(string name)
    {
        for (int i = 0; i < CurrentAttackTypeInfo.Count; i++)
        {
            if(CurrentAttackTypeInfo[i].name.Contains(name))
            {
                return CurrentAttackTypeInfo[i];
            }
        }
        Debug.LogError("This attack is missing in the list of attacks   " + name);
        return null;
    }

    public void SetBase()
    {

        HealthStats.B_Health = HealthStats.Health;

        //HealthStats
        HealthStats.B_Armour = HealthStats.Armour;

        if (CharacterID == CharacterNameType.CleasTemple_Character_Valley_Donna)
        {
        }
        //SpeedStats
        SpeedStats.B_BaseSpeed = SpeedStats.BaseSpeed;
        SpeedStats.B_MovementSpeed = SpeedStats.MovementSpeed;
        SpeedStats.B_ActionTime = SpeedStats.ActionTime;
        SpeedStats.B_LeaveAnimSpeed = SpeedStats.LeaveAnimSpeed;
        SpeedStats.B_ArriveAnimSpeed = SpeedStats.ArriveAnimSpeed;
        SpeedStats.B_WeakBulletSpeed = SpeedStats.WeakBulletSpeed;
        SpeedStats.B_WeakBulletSpeedV = SpeedStats.WeakBulletSpeedV;
        SpeedStats.B_StrongBulletSpeed = SpeedStats.StrongBulletSpeed;
        SpeedStats.B_StrongBulletSpeedV = SpeedStats.StrongBulletSpeedV;
        SpeedStats.B_AttackLoopDuration = SpeedStats.AttackLoopDuration;
        //DamageStats
        DamageStats.B_BaseDamage = DamageStats.BaseDamage;

    }


    public void ResetToBase()
    {
        HealthStats.Health = HealthStats.Current_B_Health;
    }

    public void SetupTobaseValue()
    {
        //HealthStats
        HealthStats.Health = HealthStats.Current_B_Health;
        HealthStats.Armour = HealthStats.Current_B_Armour;
        HealthStats.ArmourT = HealthStats.B_ArmourT;

        //SpeedStats
        SpeedStats.BaseSpeed = SpeedStats.Current_B_BaseSpeed;
        SpeedStats.MovementSpeed = SpeedStats.Current_B_MovementSpeed;
        SpeedStats.LeaveAnimSpeed = SpeedStats.B_LeaveAnimSpeed;
        SpeedStats.ArriveAnimSpeed = SpeedStats.B_ArriveAnimSpeed;
        SpeedStats.WeakBulletSpeed = SpeedStats.B_WeakBulletSpeed;
        SpeedStats.WeakBulletSpeedV = SpeedStats.B_WeakBulletSpeedV;
        SpeedStats.StrongBulletSpeed = SpeedStats.B_StrongBulletSpeed;
        SpeedStats.StrongBulletSpeedV = SpeedStats.B_StrongBulletSpeedV;
        SpeedStats.AttackLoopDuration = SpeedStats.B_AttackLoopDuration;
        //DamageStats
        DamageStats.BaseDamage = DamageStats.B_BaseDamage;

    }


    CharacterEvolutionClass charEvo;
    
    int resI;
    int sumc;

    public BaseInfoInjectorClass GetBaseInfoInjectorClass()
    {
        BaseInfoInjectorClass res = new BaseInfoInjectorClass();
        res.OverrideHealth = true;
        res.OverrideEther = true;
        res.OverrideArmour = true;
        res.OverrideHealthRegeneration = true;
        res.OverrideSpeed = true;
        res.overrideDeath = true;
        res.OverrideEtherRegeneration = true;
        res.OverrideShieldRegeneration = true;
        res.OverrideEvasion = true;
        res.OverrideSigilDropBonus = true;
        res.OverrideCriticalWeakBullet = true;
        res.OverrideCriticalStrongBullet = true;
        res.OverrideStrongAttackMultiplier = true;
        res.OverrideWeakAttackMultiplier = true;
        //res.overrideAttacks = true;
        res.OverrideHue = true;

        res.Health = HealthStats.Health;

        res.Armour = HealthStats.Armour;


        res.MovementSpeed = SpeedStats.MovementSpeed;

        res.deathAnim = Behaviour.DeathBehaviour;

        res.ColorHueSaturation = BaseColorHueSat;
      
        res.AddToBasicAttacks = false;

        return res;
    }

    public void AddAttacks(List<ScriptableObjectAttackBase> attacks, bool justAdd, bool clearDefaults = false)
    {
        List<ScriptableObjectAttackBase> tAtk = new List<ScriptableObjectAttackBase>();
        foreach (ScriptableObjectAttackBase item in attacks)
        {
            if (item != null)
            {
                tAtk.Add(Instantiate(item));
                tAtk.Last().AttackOwner = CharacterID;
            }
        }

        if (clearDefaults)
        {
            _CurrentAttackTypeInfo.Clear();
        }
      

    }

    public System.Collections.IEnumerator CharEventHappened(string ID)
    {
        yield return null;
        CharEventHasHappenedEvent?.Invoke(ID, this);
        //Debug.LogError("CHARACTER EVENT " + ID.ToUpper() + " HAS HAPPENED FOR " + CharacterID.ToString().ToUpper());
    }
}


public class LevelsInfoClass
{
    public LevelType Level;
    public float ExpNeeded;

    public LevelsInfoClass()
    {

    }

    public LevelsInfoClass(LevelType level, float expNeeded)
    {
        Level = level;
        ExpNeeded = expNeeded;
    }
}

[System.Serializable]
public class DeathDropInfoScript
{
    public ScriptableObjectItemPowerUps powerUp;
    public DeathDropTypes deathDropType = DeathDropTypes.Throw;
    [ConditionalField("deathDropType", false, DeathDropTypes.Throw)] public float throwDuration = 1f;
    [ConditionalField("deathDropType", false, DeathDropTypes.Throw)] public float throwDelay = 1f;
    [ConditionalField("deathDropType", false, DeathDropTypes.Throw)] public bool overrideDeathParticles = false;
    [ConditionalField("deathDropType", false, DeathDropTypes.Throw)]
    [Tooltip("needs to be a particle prefab that GridFight_ContainsStruct at least 1 VFXOffsetToTargetVOL component in the children")]
    public GameObject throwParticles = null;
}











[System.Serializable]
public class CharacterEvent
{
    public string Name = "";

    protected bool initialized = false;
    protected bool hasHappened = false;

    public bool HappenMultipleTimes = false;
   // public List<CharacterEventTrigger> Triggers = new List<CharacterEventTrigger>();
   // public List<CharacterEventAction> Actions = new List<CharacterEventAction>();

    public void Initialize(CharacterInfoScript charInfo)
    {
        if (initialized) return;

      /*  foreach (CharacterEventTrigger trigger in Triggers)
        {
            trigger.Initialize(charInfo, this);
        }
        foreach (CharacterEventAction Action in Actions)
        {
            Action.Initialize(charInfo, this);
        }*/
        initialized = true;
    }

    public void Reset(CharacterInfoScript charInfo)
    {
        if (!initialized) return;

      /*  foreach (CharacterEventTrigger trigger in Triggers)
        {
            trigger.Reset(charInfo);
        }
        foreach (CharacterEventAction Action in Actions)
        {
            Action.Reset();
        }*/
        initialized = false;
    }

    public void TriggerCheck(CharacterInfoScript charInfo)
    {
        if (hasHappened && !HappenMultipleTimes) return;

        bool triggersNotMet = false;
     /*   foreach (CharacterEventTrigger trigger in Triggers)
        {
            if (!trigger.TriggerCheck) triggersNotMet = true;
        }
        if (triggersNotMet) return;

        hasHappened = true;

        //Debug.LogError("TRIGGERED CHARACTER EVENT");
        foreach (CharacterEventAction action in Actions)
        {
            action.DoAction();
        }*/

        charInfo.StartCoroutine(charInfo.CharEventHappened(Name));
    }


    public void OnValidate()
    {
      //  foreach (CharacterEventTrigger trigger in Triggers) trigger.OnValidate();
      //  foreach (CharacterEventAction action in Actions) action.OnValidate();
    }
}

[System.Serializable]
public class CharacterEventTrigger
{
    [HideInInspector] public string Name = "";
    protected string GeneratedName
    {
        get
        {
            string res = triggerType.ToString().ToUpper() + " CHECK";
            switch (triggerType)
            {
                case CharacterEventTriggerType.None:
                    break;
                case CharacterEventTriggerType.Health:
                    res += " || IS HEALTH " + healhCompareType.ToString().ToUpper() + " " + healthComparisonValue.ToString() + "%";
                    break;
                case CharacterEventTriggerType.Forme:
                    res += " || IS FORME: " + formeIDToMatch.ToUpper();
                    break;
                case CharacterEventTriggerType.CharEvent_HasHappened:
                    res += " || " + charEventThatHappened_ID.ToUpper();
                    break;
                default:
                    break;
            }
            return res;
        }
    }

    [HideInInspector] public CharacterEvent charEvent = null;

    protected bool triggeredBefore = false;
    protected bool triggering
    {
        get
        {
            bool res = false;

            switch (triggerType)
            {
                case CharacterEventTriggerType.None:
                    res = false;
                    break;
                case CharacterEventTriggerType.Health:
                    res = CheckTriggers_Health();
                    break;
                case CharacterEventTriggerType.Forme:
                    res = CheckTriggers_Forme();
                    break;
                case CharacterEventTriggerType.Death:
                    res = CheckTriggers_Death();
                    break;
                case CharacterEventTriggerType.CharEvent_HasHappened:
                    res = CheckTriggers_CharEventHasHappened();
                    break;
                default:
                    res = false;
                    break;
            }
            if (!triggeredBefore) triggeredBefore = res;
            return res;
        }
    }
    public bool TriggerCheck
    {
        get
        {
            switch (triggerCheckType)
            {
                case TriggerCheckType.AtAnyPoint:
                    return triggering || triggeredBefore;
                case TriggerCheckType.Currently:
                    return triggering;
                default:
                    return false;
            }
        }
    }

    protected bool firstPassComplete = false;

    public TriggerCheckType triggerCheckType = TriggerCheckType.AtAnyPoint;
    public CharacterEventTriggerType triggerType = CharacterEventTriggerType.None;

    public void Initialize(CharacterInfoScript charInfo, CharacterEvent cEvent)
    {
        charEvent = cEvent;

        switch (triggerType)
        {
            case CharacterEventTriggerType.None:
                break;
            case CharacterEventTriggerType.Health:
                charInfo.HealthChangedEvent += TrackHealthChange;
                break;
            case CharacterEventTriggerType.Forme:
                if (charInfo.CharFormeInfo == null)
                {
                    Debug.LogError("CANNOT TRACK FORME CHANGES IN CHARACTER EVENTS FOR " + charInfo.CharacterID.ToString().ToUpper() + "... EITHER THERE ARE NO FORMES OR THE INITIALIZATION ORDER IS WRONG... ABORTING");
                    break;
                }
                charInfo.CharFormeInfo.FormeChangedEvent += TrackFormeChange;
                break;
            case CharacterEventTriggerType.Death:
                charInfo.DeathEvent += TrackCharacterDeath;
                break;
            case CharacterEventTriggerType.CharEvent_HasHappened:
                charInfo.CharEventHasHappenedEvent += TrackCharEventHasHappened;
                break;
            default:
                break;
        }
    }

    public void Reset(CharacterInfoScript charInfo)
    {
        triggeredBefore = false;
        //firstPassComplete = false;

        charInfo.HealthChangedEvent -= TrackHealthChange;
        charInfo.CharFormeInfo.FormeChangedEvent -= TrackFormeChange;
        charInfo.DeathEvent -= TrackCharacterDeath;
        charInfo.CharEventHasHappenedEvent -= TrackCharEventHasHappened;
    }


    //TRACK => Incoming change, triggers a check in the event || NEEDS A charEvent.TriggerCheck(); CHECK AT THE END && firstPassComplete = true; AT THE START
    //CheckTriggers => called by a trigger check in the event || NEEDS A !firstPassComplete CHECK AT THE START

    [ConditionalField("triggerType", false, CharacterEventTriggerType.Forme)]
    public string formeIDToMatch = "";

    public void TrackFormeChange(string newForme, CharacterInfoScript charInfo)
    {
        firstPassComplete = true;
        curForme = newForme;
        charEvent.TriggerCheck(charInfo);
    }
    protected string curForme = "";
    public bool CheckTriggers_Forme()
    {
        if (!firstPassComplete) return false;
        return formeIDToMatch.ToUpper() == curForme.ToUpper();
    }

    [ConditionalField("triggerType", false, CharacterEventTriggerType.Health)]
    public CompareType healhCompareType = CompareType.IsEqualTo;

    [ConditionalField("triggerType", false, CharacterEventTriggerType.Health)]
    public float healthComparisonValue = 0f;

    public void TrackHealthChange(BaseInfoScript charInfo, float a, float b, bool instantChange = false)
    {
        firstPassComplete = true;
        curHealthPerc = charInfo.HealthPerc;
        charEvent.TriggerCheck((CharacterInfoScript)charInfo);
    }
    protected float curHealthPerc = 1000000f;
    public bool CheckTriggers_Health()
    {
        if (!firstPassComplete) return false;
        switch (healhCompareType)
        {
            case CompareType.MoreThan:
                return curHealthPerc > healthComparisonValue;
            case CompareType.LessThan:
                return curHealthPerc < healthComparisonValue;
            case CompareType.IsEqualTo:
                return curHealthPerc == healthComparisonValue;
            case CompareType.None:
                return false;
            default:
                return false;
        }
    }

    public void TrackCharacterDeath(BaseInfoScript charInfo)
    {
        firstPassComplete = true;
        isDead = true;
        charEvent.TriggerCheck((CharacterInfoScript)charInfo);
    }
    protected bool isDead = false;
    public bool CheckTriggers_Death()
    {
        if (!firstPassComplete) return false;
        return isDead;
    }

    [ConditionalField("triggerType", false, CharacterEventTriggerType.CharEvent_HasHappened)]
    public string charEventThatHappened_ID = "";

    public void TrackCharEventHasHappened(string ID, CharacterInfoScript charInfo)
    {
        firstPassComplete = true;
        hasTrackedCharEventHappened = ID.ToUpper() == charEventThatHappened_ID.ToUpper();
        charEvent.TriggerCheck(charInfo);
    }
    protected bool hasTrackedCharEventHappened = false;
    public bool CheckTriggers_CharEventHasHappened()
    {
        if (!firstPassComplete) return false;
        return hasTrackedCharEventHappened;
    }



    public void OnValidate()
    {
        Name = GeneratedName;
    }
}

[System.Serializable]
public class CharacterEventAction
{
    [HideInInspector] public string Name = "";
    protected string GeneratedName
    {
        get
        {
            string res = actionType.ToString().ToUpper();
            switch (actionType)
            {
                case CharacterEventActionType.None:
                    break;
                case CharacterEventActionType.Forme_Change:
                    res += " || CHANGE TO: " + ChangeTo_FormeID.ToUpper();
                    break;
                case CharacterEventActionType.Health_Set:
                    res += " || SET TO: " + HealthPercToSetTo.ToString() + "%";
                    break;
                default:
                    break;
            }
            return res;
        }
    }

    [HideInInspector] public CharacterInfoScript trackedCharInfo = null;
    [HideInInspector] public CharacterEvent charEvent = null;

    public CharacterEventActionType actionType = CharacterEventActionType.None;

    public void Initialize(CharacterInfoScript charInfo, CharacterEvent cEvent)
    {
        trackedCharInfo = charInfo;
        charEvent = cEvent;
    }

    public void Reset()
    {

    }

    public void DoAction()
    {
        switch (actionType)
        {
            case CharacterEventActionType.None:
                break;
            case CharacterEventActionType.Forme_Change:
                DoAction_FormeChange();
                break;
            case CharacterEventActionType.Health_Set:
                DoAction_HealthChange();
                break;
            default:
                break;
        }
    }


    [ConditionalField("actionType", false, CharacterEventActionType.Forme_Change)]
    public string ChangeTo_FormeID = "";
    [ConditionalField("actionType", false, CharacterEventActionType.Forme_Change)]
    public float ChangedForme_StartingHPPerc = 100f;
    void DoAction_FormeChange()
    {
        trackedCharInfo.CharFormeInfo.ChangeForme(ChangeTo_FormeID, ChangedForme_StartingHPPerc);
    }

    [ConditionalField("actionType", false, CharacterEventActionType.Health_Set)]
    public float HealthPercToSetTo = 100f;
    void DoAction_HealthChange()
    {
        trackedCharInfo.Health = trackedCharInfo.HealthStats.Current_B_Health * (HealthPercToSetTo / 100f);
    }





    public void OnValidate()
    {
        Name = GeneratedName;
    }
}

[System.Serializable]
public class MaskInfoClass
{
    public delegate void MaskValueUpdate(float perc, float val);
    public MaskValueUpdate MaskValueUpdateEvent;
    public delegate void MaskCharged();
    public MaskCharged MaskChargedEvent;

    public ScriptableObjectSkillMask Mask;
    public int _currentMaskPoints = 0;
    public int CurrentMaskPoints
    {
        get
        {
            return _currentMaskPoints;
        }
        set
        {
            if (_currentMaskPoints != Mathf.Clamp(value, 0, Mask != null ? Mask.MaskPoints : 1000000))
            {
                _currentMaskPoints = Mathf.Clamp(value, 0, Mask != null ? Mask.MaskPoints : 1000000);
                MaskValueUpdateEvent?.Invoke(ProgressToUltimate * 100f, CurrentMaskPoints);
                if (ProgressToUltimate == 1f)
                {
                    MaskChargedEvent?.Invoke();
                }
            }
        }
    }
    public float ProgressToUltimate => Mask != null && Mask.MaskPoints == 0 ? 1f : (float)CurrentMaskPoints / (Mask == null ? 1f : (float)Mask.MaskPoints);
    public float RequiredMaskPoints => Mask != null ? Mask.MaskPoints : 100f;
    public MaskInfoClass()
    {

    }
}

[System.Serializable]
public struct CharLevelHueScaleInfo
{
    public Color Color;
    [Tooltip("Number between -0.5, 0.5, with a default of 0")] [Range(-0.5f, 0.5f)] public float Hue;
    [Tooltip("Number between 0, 2, with a default of 1")] [Range(0f, 2f)] public float Saturation;
    public ColorHueSat ColorHueSat => new ColorHueSat(Color, Hue, Saturation);
    public Vector3 Scale;

    public CharLevelHueScaleInfo(Color color, Vector3 scale, float hue, float saturation)
    {
        Color = color;
        Hue = hue;
        Saturation = saturation;
        Scale = scale;
    }

    public static CharLevelHueScaleInfo normal => new CharLevelHueScaleInfo(Color.white, Vector3.one, 0f, 1f);
}


[System.Serializable]
public class BulletTypeModifierClass
{
    public BulletType BulletTypeModifier;
    public AttackInputType InputType;
    public BulletTypeModifierClass()
    {

    }
}

