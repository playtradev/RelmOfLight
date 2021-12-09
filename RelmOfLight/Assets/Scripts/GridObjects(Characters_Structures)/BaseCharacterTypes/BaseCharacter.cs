//using nn.ec;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
//||
public class BaseCharacter : MonoBehaviour, System.IDisposable, IDamageReceiver, IDamageMaker, ISpineCharacter
{
    //INTERFACE VARIABLES
    public CharacterNameType CharName => CharInfo != null ? CharInfo.CharacterID : CharacterNameType.None;
    public GameObject ReceiverGO => gameObject;
    public Vector2Int GridPosition => CharInfo.CurrentTilePos;
    public List<Vector2Int> GridPositions => CharInfo.Pos;
    public BaseInfoScript InfoScript => CharInfo;
    float IDamageReceiver.Health { get => CharInfo.Health; set => CharInfo.Health = value; }
    public BaseCharacter ReferenceCharacter => this;
    //
    public bool isMaskSkillBoss = false;

    public ElementalType[] AllElements
    {
        get
        {
            ElementalType[] elements = CharInfo == null ? new ElementalType[0] : new ElementalType[] { CharInfo.Elemental };
            return elements;
            // NOT USING ANY MORE:
            //return CharInfo.MaskInfo.Mask != null ? elements.Concat(CharInfo.MaskInfo.Mask.ElementalTypes).ToArray() : elements;
        }
    }

#if UNITY_EDITOR
    [HideInInspector] public ScriptableObjectAttackEffect testAtkEffect = null;
#endif

    public ScriptableObjectBaseCharacterInput currentInputProfile;
    public ScriptableObjectBaseCharacterBaseAttack currentAttackProfile;
    public ScriptableObjectBaseCharacterBaseMove currentMoveProfile;
    public ScriptableObjectBaseCharacterDeath currentDeathProfile;


    [HideInInspector] public float SecondsLeftToRespawn = 0f;
    public float ReviveProgress = 0f;
    public IEnumerator RespawnSequencerCo = null;
    public IEnumerator ReviveSequencer(float duration = -1f)
    {
        float timeElapsed = 0f;
        duration = BattleManagerScript.Instance.ReturnCharsTurn * BattleManagerScript.Instance.BaseActionTime;
        while (timeElapsed != duration && died)
        {
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            timeElapsed = Mathf.Clamp(timeElapsed + BattleManagerScript.Instance.DeltaTime, 0f, duration);
            SecondsLeftToRespawn = Mathf.Clamp(duration - timeElapsed, 0f, duration);
            ReviveProgress = timeElapsed / duration;
        }
        SecondsLeftToRespawn = 0f;
        CharBackFromDeath();
    }

    public void OnEnable()
    {
        StartCoroutine(VitalityUpdater());
    }

    IEnumerator VitalityUpdater()
    {
        int counter = 0;
        while (true)
        {
            if (counter == 10)
            {
                
                UpdateVitalities();

                counter = 0;
            }
            else
            {
                counter++;
            }
            yield return null;

        }
    }


    public void CharBackFromDeath()
    {
        died = false;
        gameObject.SetActive(true);
        CharInfo.HealthStats.Health = CharInfo.HealthStats.Current_B_Health;
        UpdateVitalities();
    }


    //------------------------------------

    #region Events

    public delegate void CharacterHealthChanged(BaseCharacter cb);
    public event CharacterHealthChanged CharacterHealthChangedEvent;

    public delegate void CharacterEtherChanged(BaseCharacter cb);
    public event CharacterEtherChanged CharacterEtherChangedEvent;

    public delegate void CurrentCharIsDead(BaseCharacter cb);
    public event CurrentCharIsDead CurrentCharIsDeadEvent;

    public delegate void CurrentCharIsSpawned(BaseCharacter cb);
    public event CurrentCharIsSpawned CurrentCharIsSpawnedEvent;

    public delegate void CurrentCharEtherDepleted(BaseCharacter cb);
    public event CurrentCharEtherDepleted CurrentCharEtherDepletedEvent;

    public delegate void CurrentCharShieldDepleted(BaseCharacter cb);
    public event CurrentCharShieldDepleted CurrentCharShieldDepletedEvent;

    public delegate void TileMovementComplete(BaseCharacter movingChar);
    public event TileMovementComplete TileMovementCompleteEvent;

    public delegate void StatsChanged(float value, BattleFieldIndicatorType changeType, BaseCharacter charOwner);
    public event StatsChanged StatsChangedEvent;

    public delegate void EffectChanged(string value, BattleFieldIndicatorType changeType, BaseCharacter charOwner);
    public event EffectChanged EffectChangedEvent;

    public delegate void CurrentCharIsRebirth(CharacterNameType cName, List<ControllerType> playerController, TeamSideType side);
    public event CurrentCharIsRebirth CurrentCharIsRebirthEvent;

    public delegate void CurrentCharStartingAction(ControllerType playerController, CharacterNameType characterID, CharacterActionType action, string identifier);
    public event CurrentCharStartingAction CurrentCharStartingActionEvent;

    public delegate void StrongAttackRequest();
    public event StrongAttackRequest StrongAttackRequestEvent;


    public void Unsub_CurrentCharStartingActionEvent()
    {
        if (CurrentCharStartingActionEvent != null)
        {
            CurrentCharStartingActionEvent = null;
        }
    }


    protected virtual void Call_CurrentCharIsDeadEvent()
    {
        CurrentCharIsDeadEvent?.Invoke(this);
    }


    protected virtual void Call_CurrentCharIsRebirthEvent()
    {
        CurrentCharIsRebirthEvent(CharInfo.CharacterID, CharInfo.PlayerController, CharInfo.Side);
    }

    public void Invoke_TileMovementCompleteEvent()
    {
        TileMovementCompleteEvent?.Invoke(this);
    }
    #endregion

    #region BaseChar Variables
    public CharacterInfoScript _CharInfo;

    public virtual CharacterInfoScript CharInfo
    {
        get
        {
            if (_CharInfo == null)
            {
                _CharInfo = GetComponentInChildren<CharacterInfoScript>(true);
                _CharInfo.BaseSpeedChangedEvent += _CharInfo_BaseSpeedChangedEvent;
                _CharInfo.DeathEvent += _CharInfo_DeathEvent;
                _CharInfo.BaseSwappableSOChangedEvent += _CharInfo_BaseSwappableSOChangedEvent;
                _CharInfo.GetComponent<Rigidbody>().isKinematic = false;

                _CharInfo.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
            return _CharInfo;
        }
    }

    public void ForceMovementEvent(InputDirectionType dir)
    {
        currentInputProfile.UseDir = true;
        currentInputProfile.UseStrong = false;
        currentInputProfile.NewDir = dir;
    }

    public void Strongattack()
    {
        if (IsOnField && BattleManagerScript.Instance.ManaCostSkill <= (CharInfo.Side == TeamSideType.LeftSideTeam ?
            BattleManagerScript.Instance.LeftMana.CurrentMana : BattleManagerScript.Instance.RightMana.CurrentMana))
        {
            if (CharInfo.Side == TeamSideType.LeftSideTeam)
            {
                BattleManagerScript.Instance.LeftMana.CurrentMana -= BattleManagerScript.Instance.ManaCostSkill;
            }
            else
            {
                BattleManagerScript.Instance.RightMana.CurrentMana -= BattleManagerScript.Instance.ManaCostSkill;
            }
            currentInputProfile.UseStrong = true;
            currentInputProfile.UseDir = false;
            ParticleManagerScript.Instance.FireParticlesInTransform(BattleManagerScript.Instance.StrongAttackInFeedback, spineT);
            StrongAttackRequestEvent?.Invoke();

        }
    }

    private void _CharInfo_BaseSwappableSOChangedEvent()
    {
        UpdateSwappableSO(CharInfo.Behaviour.MovementActionN);
    }



    public bool _died = false;
    public bool died
    {
        get
        {
            return _died;
        }
        set
        {
            _died = value;
        }
    }

    public bool IsOnField
    {
        get
        {
            return _IsOnField;
        }
        set
        {
            CharInfo.isOnField = value;
            _IsOnField = value;
        }
    }
    public bool _IsOnField = false;
    public bool _isMoving = false;
    public bool isMoving
    {
        get
        {
            return _isMoving;
        }
        set
        {
            //Debug.LogError("_isMoving   " + value);
            _isMoving = value;
        }
    }
    public bool isTeleporting = false;


    //Spine Char Interface setup
    public SpineAnimationManager SpineAnim { get { return _spineAnim; } set { _spineAnim = value; } }
    protected SpineAnimationManager _spineAnim;
    public Vector3 LocalSpinePosoffset
    {
        get
        {
            return _LocalSpinePosoffset;
        }
        set
        {
            if (_LocalSpinePosoffset == new Vector3(100, 100, 100))
            {
                _LocalSpinePosoffset = value;
            }
        }
    }
    public Vector3 _LocalSpinePosoffset = new Vector3(100, 100, 100);
    public float AnimSpeed { get { return _animSpeed; } set { _animSpeed = value; } }
    protected float _animSpeed = 1f;
    public Transform spineT { get { return _spineT; } set { _spineT = value; } }
    protected Transform _spineT;
    //=========================

    protected bool pauseOnLastFrame = false;

    public List<CharacterActionType> CharActionlist = new List<CharacterActionType>();
    public List<CharacterActionType> B_CharActionlist = new List<CharacterActionType>();
    public delegate void CharacterActionListChanged(List<CharacterActionType> newActionList);
    public event CharacterActionListChanged CharacterActionListChangedEvent;
    public void Fire_CharacterActionListChangedEvent() => CharacterActionListChangedEvent?.Invoke(CharActionlist);
    public UnitManagementScript UMS;
    public BoxCollider CharBoxCollider;

    public delegate void CharacterSelected(bool newState, ControllerType controller);
    public event CharacterSelected CharacterSelectedEvent;
    public void Fire_CharacterSelectedEvent(bool state, ControllerType controller) => CharacterSelectedEvent?.Invoke(state, controller);


    public ControllerType _CurrentPlayerController;
    public ControllerType CurrentPlayerController
    {
        get => _CurrentPlayerController;
        set
        {
            bool changed = _CurrentPlayerController != value;
            _CurrentPlayerController = value;

            if (!changed) return;

            CharacterSelectedEvent?.Invoke(value.ToString().ToUpper().Contains("PLAYER"), _CurrentPlayerController);
        }
    }
    public bool isSelectable => CurrentPlayerController == ControllerType.None && !died && CharInfo.isPlayerControlledCharacter;

    [HideInInspector] public int CharOredrInLayer = 0;
    protected List<BattleTileScript> currentBattleTilesToCheck = new List<BattleTileScript>();

    [HideInInspector] public GameObject CastStartLoopPS = null;
    [HideInInspector] public GameObject CastLoopPS = null;
    [HideInInspector] public GameObject CastActivationPS = null;

    public List<ParticleHelperScript> LayersPs = new List<ParticleHelperScript>();

    public int CharacterRespawnLength
    {
        get
        {
            return (int)CharInfo.CurrentCharacterRespawnLength;
        }
    }

    public bool CanBeAffectedByTilesEffect
    {
        get
        {
            return (HasBuffDebuff(BuffDebuffStatsType.Disable_CollisionWithTileEffect) ? GetBuffDebuff(BuffDebuffStatsType.CharacterSpawnWait).CurrentBuffDebuff.BoolValue : true) && isActiveAndEnabled;
        }
    }


    [HideInInspector] public List<HitInfoClass> HittedByList = new List<HitInfoClass>();
    [HideInInspector]
    public HitInfoClass LastHitter
    {
        get
        {
            HitInfoClass lastHitter = null;
            foreach (HitInfoClass hitter in HittedByList)
            {
                if (lastHitter == null || lastHitter.TimeLastHit < hitter.TimeLastHit) lastHitter = hitter;
            }
            return lastHitter;
        }
    }

    #endregion

    #region Variables that we have to decide if still useful

    public bool CanAttack = false;

    #endregion

    #region Buff_Debuff Variables

    public List<BuffDebuffClass> BuffsDebuffsList = new List<BuffDebuffClass>();
    public List<BuffDebuffIconClass> BuffDebuffIcons = new List<BuffDebuffIconClass>();
    public IEnumerator ZombificationCo = null;
    public GameObject zombiePs = null;
    public BuffDebuffIconClass temp_BdIcon;
    public BuffDebuffIconClass temp_CBdIcon;
    public List<BuffDebuffStatsType> temp_StatsToAffect = new List<BuffDebuffStatsType>();

    #endregion
    #region SupportVariables
    bool FireArrivingPs_Audio = true;
    protected GameObject tempGameObject = null;
    protected float tempFloat_1;
    protected int tempInt_1, tempInt_2, tempInt_3;
    protected Vector2Int tempVector2Int;
    protected Vector3 tempVector3;
    protected string tempString;
    List<Vector2Int> tempList_Vector2int = new List<Vector2Int>();
    [HideInInspector] public BattleFieldIndicatorType healthCT = BattleFieldIndicatorType.Damage;
    string queuedAnim_Name = "";
    bool queuedAnim_Loop = false;
    float queuedAnim_Transition = 0f;
    float queuedAnim_Speed = 1f;
    protected bool interruptPuppetAnim = false;
    protected bool _isPuppeting = false;

    public bool isPuppeting
    {
        get
        {
            return _isPuppeting;
        }
        set
        {
            _isPuppeting = value;
        }
    }
    public float defenceAnimSpeedMultiplier = 5f;
    protected int puppetAnimCompleteTick = 0;
    #endregion


    public string _CharID;
    public string CharID
    {
        get
        {
            return _CharID;
        }
        set
        {
            if (value == "EluAlly")
            {

            }
            _CharID = value;
        }
    }

    public float GetMovementAnimSpeed(string animState)
    {
        return (SpineAnim.GetAnimLenght(animState) * (CharInfo.SpeedStats.IntroPerc + CharInfo.SpeedStats.LoopPerc) /
                    CharInfo.SpeedStats.TileMovementTime) * CharInfo.SpeedStats.MovementSpeed * CharInfo.BaseSpeed;
    }

    public float GetMovementAnimSpeed(CharacterAnimationStateType animState)
    {
        return (SpineAnim.GetAnimLenght(animState.ToString()) * (CharInfo.SpeedStats.IntroPerc + CharInfo.SpeedStats.LoopPerc) /
                    CharInfo.SpeedStats.TileMovementTime) * CharInfo.SpeedStats.MovementSpeed * CharInfo.BaseSpeed;
    }



    public void UpdateVitalities()
    {
        currentInputProfile.UpdateVitalities();
        currentAttackProfile.UpdateVitalities();
        currentMoveProfile.UpdateVitalities();
        currentDeathProfile.UpdateVitalities();

        UMS.SetHP();
    }

    #region Setup Character

    public virtual void SetCharacterStats(BaseInfoInjectorClass info, float startingHp = -1f, bool useSigils = true)
    {

        if (info != null)
        {
            CharInfo?.SetupChar(info, useSigils: useSigils);
        }
        else
        {
            CharInfo?.ResetToBase();
        }

        CharInfo.BaseColorHueSat = SpineAnim.GetColorHueSat();
        if (CharInfo != null && startingHp != -1f) CharInfo.Health = CharInfo.HealthStats.Current_B_Health * (startingHp / 100f);
        RefreshSwappableSOs();
        CharInfo.FireHealthChangedEvent();

    }

    public virtual void SetupCharacterSide(TeamSideType teamSide, bool restoreChar = true, bool showHP = true, bool showEther = true)
    {
        currentAttackProfile.CharOwner = this;
        currentMoveProfile.CharOwner = this;
        currentInputProfile.CharOwner = this;
        currentDeathProfile.CharOwner = this;

        currentInputProfile.SetupCharacterSide(restoreChar);

        SpineAnimatorsetup(showHP, showEther, CharInfo.Behaviour.InputBehaviour != InputBehaviourType.AIDumb);
        LocalSpinePosoffset = SpineAnim.transform.localPosition;
        int layer = CharInfo.Side == TeamSideType.LeftSideTeam ? 9 : 10;
        if (CharInfo.UseLayeringSystem)
        {
            SpineAnim.gameObject.layer = layer;
        }
        SpineAnim.gameObject.tag = CharInfo.Side.ToString();
        gameObject.tag = CharInfo.Side.ToString();
    }

    public void _CharInfo_BaseSpeedChangedEvent(float baseSpeed)
    {
        SpineAnim.SetAnimationSpeed(baseSpeed);
    }


    public void _CharInfo_DeathEvent(BaseInfoScript charInfo)
    {
        if (HasBuffDebuff(BuffDebuffStatsType.Rebirth))
        {
            RebirthEffect();
            return;
        }
        //if (CharInfo.BlockDeath)
        //{
        //    CharInfo.BlockDeath = false;
        //    return;
        //}

        if (CharInfo.Health > 0f || !isActiveAndEnabled) return;
        StartCoroutine(SetCharDead());
    }

    public virtual void RebirthEffect()
    {
        tempFloat_1 = CharInfo.HealthStats.Current_B_Health;
        CharInfo.Health += tempFloat_1;
        EffectChangedEvent?.Invoke("", BattleFieldIndicatorType.Rebirth, this/*SpineAnim.transform*/);
        GetBuffDebuff(BuffDebuffStatsType.Rebirth).Stop_Co = true;
    }


    public virtual void SetAttackReady(bool value)
    {
        if (CharBoxCollider != null && !HasBuffDebuff(BuffDebuffStatsType.ShadowForm))
        {
            CharBoxCollider.enabled = value;
        }
        CanAttack = value;
        IsOnField = value;

        HittedByList.Clear();

        currentInputProfile.SetAttackReady(value);
        currentAttackProfile.SetAttackReady(value);
        currentMoveProfile.SetAttackReady(value);
        currentDeathProfile.SetAttackReady(value);

        SetLayer();
    }


    public virtual IEnumerator SetCharDead()
    {
        if (died || !isActiveAndEnabled) yield break;

        while (HasBuffDebuff(BuffDebuffStatsType.MeleeAttack))
        {
            yield return null;
        }

        died = true;
        currentInputProfile.SetCharDead();
        currentAttackProfile.SetCharDead();
        currentMoveProfile.SetCharDead();
        Call_CurrentCharIsDeadEvent();

        yield return BattleManagerScript.Instance.WaitFor(0.2f, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

        CharInfo.SpeedStats.IsForcedValue = false;


        //THis override only applies if the whole explosion system needs to be overriden and not just the particles themselves!!!!
        currentDeathProfile.overrideDeathParticles = ReleaseDeathDrops();

        currentDeathProfile.SetCharDead();
      
        StopChargingPs(true);
        isMoving = false;
        isTeleporting = false;

        LocalSpinePosoffset = SpineAnim.transform.localPosition;
        RespawnSequencerCo = ReviveSequencer();
        StartCoroutine(RespawnSequencerCo);

    }
    public IEnumerator DisableCharCo = null;
    public void DisableChar(float deathDisableing_Delay)
    {
        DisableCharCo = DisableChar_Co(deathDisableing_Delay);
        StartCoroutine(DisableCharCo);
    }

    public IEnumerator DisableChar_Co(float deathDisableing_Delay)
    {
        yield return BattleManagerScript.Instance.WaitFor(deathDisableing_Delay);
        if (DisableCharCo != null)
        {
            SetAttackReady(false);
            ResetBaseChar();
            while (BuffsDebuffsList.Count > 0)
            {
                yield return null;
            }
            gameObject.SetActive(false);
            DisableCharCo = null;
        }
    }

    //Returns whether or not it overrides the death particles
    bool ReleaseDeathDrops()
    {
        if (LastHitter == null || (BaseCharacter)LastHitter.hitter == null || CharInfo.DeathDrops == null) return false;

        //Calculate chances here if there is a randomisation system in place in the future
        bool overrideDeathParticles = false;

        foreach (DeathDropInfoScript deathDrop in CharInfo.DeathDrops)
        {
            if (deathDrop.deathDropType == DeathDropTypes.Embue)
            {
                ItemSpawnerManagerScript.Instance.CollectPowerUp(deathDrop.powerUp, (BaseCharacter)LastHitter.hitter, SpineAnim.transform.position);
            }
            else
            {

                if (deathDrop.powerUp == null)
                {
                    continue;
                }

                //Get a battletile that doesn't have a powerup already on it
                BattleTileScript bts = null;
                while (bts == null || bts.BattleTileState != BattleTileStateType.Empty || ItemSpawnerManagerScript.Instance.SpawnedItems.Where(r => r.isActiveAndEnabled && r.Pos == bts.Pos).ToList().FirstOrDefault() != null)
                {
                    bts = GridManagerScript.Instance.GetFreeBattleTile(CharInfo.WalkingSide == WalkingSideType.LeftSide ? WalkingSideType.RightSide : WalkingSideType.LeftSide);
                }
                //Throw the potion to that tile

                ItemSpawnerManagerScript.Instance.SpawnPotionFromTo(spineT.position, bts.Pos, deathDrop.powerUp, deathDrop.throwDuration, deathDrop.throwParticles, deathDrop.throwDelay, CharInfo.GetComponent<MeshRenderer>());
                overrideDeathParticles = deathDrop.overrideDeathParticles ? true : overrideDeathParticles;
            }
        }
        return overrideDeathParticles;
    }

    public virtual void SetUpEnteringOnBattle(CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving, bool loop = false, bool fireArrivingPs_Audio = true)
    {
        FireArrivingPs_Audio = fireArrivingPs_Audio;
        currentInputProfile.SetUpEnteringOnBattle(anim, loop);
        currentAttackProfile.SetUpEnteringOnBattle(anim, loop);
        currentMoveProfile.SetUpEnteringOnBattle(anim, loop);
        currentDeathProfile.SetUpEnteringOnBattle(anim, loop);

        if (anim != CharacterAnimationStateType.Arriving)
        {
            CharArrivedOnBattleField(anim != CharacterAnimationStateType.JumpTransition_IN, anim);
            FireArrivingPs_Audio = true;
        }
        else
        {
            AnimSpeed = BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets || BattleManagerScript.Instance.BattleSpeed == 0.015f ?
                3 : BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle ?
                CharInfo.Behaviour.InputBehaviour == InputBehaviourType.PlayerInput ? 10 : 3 : 3;

            if (FireArrivingPs_Audio && AnimSpeed == 10)
            {
                ArrivingEvent(GridManagerScript.Instance.GetBattleTile(CharInfo.CurrentTilePos).transform.position);
            }
        }
    }

    public virtual void SetUpLeavingBattle(bool withAudio = true)
    {
        CharInfo.SpeedStats.IsForcedValue = false;

        if (HasBuffDebuff(BuffDebuffStatsType.FireParticlesToChar))
        {
            ResetTargetBuffDebuff(BuffDebuffStatsType.FireParticlesToChar);
        }


        if (HasBuffDebuff(BuffDebuffStatsType.WalkingSide))
        {
            ResetTargetBuffDebuff(BuffDebuffStatsType.WalkingSide);
        }

        currentInputProfile.SetUpLeavingBattle(withAudio);
        currentAttackProfile.SetUpLeavingBattle(withAudio);
        currentMoveProfile.SetUpLeavingBattle(withAudio);
        currentDeathProfile.SetUpLeavingBattle(withAudio);
    }

    public virtual void CharArrivedOnBattleField(bool overrideAnimAndPos = true, CharacterAnimationStateType anim = CharacterAnimationStateType.Idle)
    {
        SetAttackReady(true);
        CurrentCharIsSpawnedEvent?.Invoke(this);
        currentInputProfile.CharArrivedOnBattleField(overrideAnimAndPos, anim);
        currentAttackProfile.CharArrivedOnBattleField(overrideAnimAndPos, anim);
        currentMoveProfile.CharArrivedOnBattleField(overrideAnimAndPos, anim);
        currentDeathProfile.CharArrivedOnBattleField(overrideAnimAndPos, anim);
        BattleManagerScript.Instance?.Trigger_CharacterArrivedOnFieldEvent(this);
    }

    public virtual void OnDestroy()
    {
        currentAttackProfile.Destroy();
        currentInputProfile.Destroy();
        currentMoveProfile.Destroy();
        currentDeathProfile.Destroy();

        CurrentCharIsDeadEvent = null;
        CurrentCharIsRebirthEvent = null;
        CurrentCharStartingActionEvent = null;
        TileMovementCompleteEvent = null;
        StatsChangedEvent = null;


    }


    public void ResetBuffDebuff(bool completeBuffDebuffProcesses = false)
    {
        BuffsDebuffsList.ToList().ForEach(r =>
        {
            r.Duration = 0;
            if (completeBuffDebuffProcesses)
                CompleteBuffDebuff(r);
            r.Stop_Co = true;
        }
        );
    }

    public void ResetTargetBuffDebuff(BuffDebuffStatsType targetBuffDebuff)
    {
        BuffsDebuffsList.ForEach(r =>
        {
            if (r.CurrentBuffDebuff.StatsToAffect == targetBuffDebuff)
            {
                r.Duration = 0;
                r.Stop_Co = true;
            }
        }
        );
    }

    public void ResetBaseChar(bool resetBuffDebuff = true, bool resetDied = true, bool resetAnim = true)
    {
        isMoving = false;
        isTeleporting = false;
        if (resetDied)
        {
            died = false;
        }
        currentInputProfile.Reset();
        currentMoveProfile.Reset();
        currentAttackProfile.Reset();
        currentDeathProfile.Reset();
        if (resetBuffDebuff)
        {
            ResetBuffDebuff();
        }

        if (CastLoopPS != null)
        {
            CastLoopPS.SetActive(false);
        }
        if (CastStartLoopPS != null)
        {
            CastStartLoopPS.SetActive(false);
        }
        if (CastActivationPS != null)
        {
            CastActivationPS.SetActive(false);
        }

        CharInfo.SpeedStats.IsForcedValue = false;
        SpineAnim.transform.localPosition = LocalSpinePosoffset;
        if (!SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Defeat.ToString()) && resetAnim)
        {
            SpineAnim.CurrentAnim = "";
            SetAnimation(CharacterAnimationStateType.Idle, true, 0f);
        }
    }

    public void RemoveFromBoard()
    {
        SetAttackReady(false);
        for (int i = 0; i < CharInfo.Pos.Count; i++)
        {
            if (GridManagerScript.Instance.GetBattleTile(CharInfo.Pos[i]).BattleTileState == BattleTileStateType.Occupied) GridManagerScript.Instance.SetBattleTileState(CharInfo.Pos[i], BattleTileStateType.Empty);
        }
        transform.position = new Vector3(100, 100, 100);
        gameObject.SetActive(false);
    }

    public void RefreshSwappableSOs()
    {
        UpdateSwappableSO(CharInfo.Behaviour.MovementActionN);
    }


    public void UpdateSwappableSO(MovementActionType move)
    {
        if (currentAttackProfile == null)
        {
            currentAttackProfile?.Reset();
            currentAttackProfile = (ScriptableObjectBaseCharacterBaseAttack)CharInfo.SwappableBases.RuntimeBases.Where(r => r.BaseName == SwappableActionType.Tiles.ToString()).First().Swappable;
            currentAttackProfile.CharOwner = this;
        }
        if (currentInputProfile == null)
        {
            currentInputProfile?.EndInput();
            currentInputProfile = (ScriptableObjectBaseCharacterInput)CharInfo.SwappableBases.RuntimeBases.Where(r => r.BaseName == InputBehaviourType.AIInput.ToString()).First().Swappable;
            currentInputProfile.CharOwner = this;
        }
        if (currentMoveProfile == null || (move != MovementActionType.None && currentMoveProfile.SwappableType.ToString() != move.ToString()))
        {
            currentMoveProfile?.Reset();
            CharInfo.Behaviour.MovementActionN = move;
            currentMoveProfile = (ScriptableObjectBaseCharacterBaseMove)CharInfo.SwappableBases.RuntimeBases.Where(r => r.BaseName == move.ToString()).First().Swappable;
            currentMoveProfile.CharOwner = this;
        }
        if (!CharInfo.IsSummon && currentDeathProfile == null)
        {
            currentDeathProfile?.Reset();
            currentDeathProfile = (ScriptableObjectBaseCharacterDeath)CharInfo.SwappableBases.RuntimeBases.Where(r => r.BaseName == DeathBehaviourType.Reverse_Arrives.ToString()).First().Swappable;
            currentDeathProfile.CharOwner = this;
        }
    }

    #endregion

    #region Attack

    public virtual void BackfireEffect(CurrentAttackInfoClass cAtk)
    {
        //BACKFIRE APPLY DAMAGE BASED ON HOW MUCH DAMAGE WAS DEALT
        SetDamage(new DamageInfoClass(this,
            null,
            cAtk.CurrentAttack,
            new List<ScriptableObjectAttackEffect>(),
            ElementalType.Neutral,
            false,
            false,
            false,
            false,
            SpineAnim.transform.position),
            GetBuffDebuff(BuffDebuffStatsType.Backfire).CurrentBuffDebuff.StatsChecker == StatsCheckerType.Multiplier ?
                GetBuffDebuff(BuffDebuffStatsType.Backfire).CurrentBuffDebuff.Value * cAtk.GetCurrentAttackDamage : GetBuffDebuff(BuffDebuffStatsType.Backfire).CurrentBuffDebuff.Value);

        ParticleManagerScript.Instance.FireAttackParticlesInPosition(cAtk.CurrentAttack.Particles.Right.HitAddress,
            cAtk.CurrentAttack.AttackOwner, AttackParticlePhaseTypes.Hit, transform.position, CharInfo.Facing, cAtk.CurrentAttack.ParticlesInput, HitParticlesType.Normal, 1);

        currentAttackProfile.InteruptAttack();
        SetAnimation("Idle", true);
    }

    public virtual string GetAttackAnimName(CurrentAttackInfoClass cAtk)
    {
        return CharInfo.CurFormeAnimPrefix + cAtk.CurrentAttack.PrefixAnim + (cAtk.CurrentAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? "_Loop" : "_AtkToIdle") + CharInfo.CurFormeAnimSuffix;
    }

    #endregion

    #region Buff/Debuff

    public bool HasBuffDebuff(BuffDebuffStatsType type)
        => BuffsDebuffsList.Where(r => (int)r.CurrentBuffDebuff.StatsToAffect == (int)type).ToArray().Length > 0;

    public bool HasBuffDebuffs(BuffDebuffStatsType[] types, bool hasAll = false)
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (HasBuffDebuff(types[i]))
                if (!hasAll)
                    return true;
                else
                if (hasAll)
                    return false;
        }
        return hasAll; 
    }

    public BuffDebuffClass GetBuffDebuff(BuffDebuffStatsType type)
    {
        return BuffsDebuffsList.Where(r => r.CurrentBuffDebuff.StatsToAffect == type).FirstOrDefault();
    }

    public void Buff_DebuffCo(BaseCharacter effectMaker, ScriptableObjectAttackEffect effect, ScriptableObjectAttackBase atk, bool pass = false, bool displayTextPopup = true)
    {
        if (!pass && (!IsOnField || died || CharInfo.Health <= 0 || BattleManagerScript.Instance.CurrentBattleState == BattleState.WaveEnd))
        {
            return;
        }
        temp_BdIcon = null;
        temp_CBdIcon = null;
        if (effect.OldSystem)
        {
            if (CharInfo.HasImmunityTo(effect.StatsToAffect))
            {
                return;
            }
            if (effect.icon != null)
            {
                temp_BdIcon = new BuffDebuffIconClass(effect.icon, effect.classification, effect.StatsToAffect, effect.recolorCharUI, effect.statusIconColor);
            }
            if (effect.SetIconOnCaster && effect.OnCasterIcon != null)
            {
                temp_CBdIcon = new BuffDebuffIconClass(effect.OnCasterIcon, effect.OnCasterClassification, effect.StatsToAffect, effect.OnCasterRecolorCharUI, effect.OnCasterStatusIconColor);
            }

            StartBuffDebuff(new BuffDebuffClass(effect.StatsToAffect, effect.level, new StatsToAffectClass(effect, atk), effect.Duration, temp_BdIcon, effectMaker, temp_CBdIcon, effect), displayTextPopup ? effect.NameShowedOnIndicator : "");
        }
        else
        {
            bool icon = true;
            for (int i = 0; i < effect.StatsToAffectList.Count; i++)
            {
                temp_BdIcon = null;
                if (CharInfo.HasImmunityTo(effect.StatsToAffectList[i].StatsToAffect))
                {
                    continue;
                }
                if (icon)
                {
                    icon = false;

                    if (effect.icon != null)
                    {
                        temp_BdIcon = new BuffDebuffIconClass(effect.icon, effect.classification, effect.StatsToAffect, effect.recolorCharUI, effect.statusIconColor);
                    }
                    if (effect.SetIconOnCaster && effect.OnCasterIcon != null)
                    {
                        for (int a = 0; a < effect.StatsToAffectList.Count; a++)
                        {
                            temp_StatsToAffect.Add(effect.StatsToAffectList[a].StatsToAffect);
                        }
                        temp_CBdIcon = new BuffDebuffIconClass(effect.OnCasterIcon, effect.OnCasterClassification, temp_StatsToAffect, effect.OnCasterRecolorCharUI, effect.OnCasterStatusIconColor);
                    }
                }
                StartBuffDebuff(new BuffDebuffClass(effect.StatsToAffectList[i].StatsToAffect, effect.level, new StatsToAffectClass(effect.StatsToAffectList[i], atk), effect.Duration, temp_BdIcon, effectMaker, temp_CBdIcon, effect), displayTextPopup && i == 0 ? effect.NameShowedOnIndicator : "", i == 0);
            }
        }

        UMS.buffIconHandler.RefreshIcons(BuffDebuffIcons);
    }
    List<BuffDebuffClass> buffDebuffItems = new List<BuffDebuffClass>();
    public void StartBuffDebuff(BuffDebuffClass bdClass, string displayTextPopup = "", bool displayParticles = true)
    {
        BuffDebuffClass temp_BuffDebuff;

        buffDebuffItems = BuffsDebuffsList.Where(r => r.Stat == bdClass.CurrentBuffDebuff.StatsToAffect).ToList();
        if (buffDebuffItems.Count == 0)
        {
            if (!isActiveAndEnabled)
            {

            }
            //Debug.Log(bdClass.Name + "   " + newBuffDebuff.Last());
            temp_BuffDebuff = bdClass;
            temp_BuffDebuff.BuffDebuffCo = Buff_DebuffCoroutine(temp_BuffDebuff, displayTextPopup, displayParticles);
            BuffsDebuffsList.Add(temp_BuffDebuff);
            StartCoroutine(temp_BuffDebuff.BuffDebuffCo);
        }
        else //Refresh current BuffDebuff duration
        {
            if (buffDebuffItems[0].Level == bdClass.Level)
            {
                if (bdClass.Effect.StackType == BuffDebuffStackType.Stackable && buffDebuffItems[0].CurrentStack < bdClass.Effect.maxStack)
                {
                    temp_BuffDebuff = bdClass;
                    temp_BuffDebuff.BuffDebuffCo = Buff_DebuffCoroutine(temp_BuffDebuff, displayTextPopup, displayParticles);
                    BuffsDebuffsList.Add(temp_BuffDebuff);
                    temp_BuffDebuff.CurrentStack = buffDebuffItems[0].CurrentStack + 1;
                    StartCoroutine(temp_BuffDebuff.BuffDebuffCo);
                    foreach (var item in buffDebuffItems)
                    {
                        item.CurrentStack++;
                    }
                }
                else if (bdClass.Effect.StackType == BuffDebuffStackType.Refreshable)
                {
                    buffDebuffItems[0].Duration = bdClass.Effect.Duration;
                    buffDebuffItems[0].Timer = 0;
                    buffDebuffItems[0].Offset = 0;
                    if (bdClass.Effect.Particles != ParticlesType.None && buffDebuffItems[0].PsHelper != null)
                    {
                        buffDebuffItems[0].PsHelper.AddPSTime(bdClass.Effect.Duration);
                    }
                }
            }
            else if (buffDebuffItems[0].Level > bdClass.Effect.level)
            {

            }
            else if (buffDebuffItems[0].Level < bdClass.Level)
            {
                temp_BuffDebuff = bdClass;
                temp_BuffDebuff.BuffDebuffCo = Buff_DebuffCoroutine(temp_BuffDebuff, displayTextPopup, displayParticles);
                BuffsDebuffsList.Add(temp_BuffDebuff);
                StartCoroutine(temp_BuffDebuff.BuffDebuffCo);
                foreach (var item in buffDebuffItems)
                {
                    item.CurrentStack++;
                }
                if (bdClass.Effect.StackType == BuffDebuffStackType.Refreshable)
                {
                    buffDebuffItems[0].Stop_Co = true;
                }
            }
        }
    }

    public void StealAttack(List<ScriptableObjectAttackBase> stolenAtk, float duration, CharacterNameType stolenAttackOwner)
    {
        StartCoroutine(StolenAtkCo(stolenAtk, duration, stolenAttackOwner));
    }
    public bool IsStealingOn = false;
    public CharacterNameType StolenAttackOwner;
    public IEnumerator StolenAtkCo(List<ScriptableObjectAttackBase> stolenAtk, float duration, CharacterNameType stolenAttackOwner)
    {
        StolenAttackOwner = stolenAttackOwner;
        IsStealingOn = true;

        for (int i = 0; i < stolenAtk.Count; i++)
        {
            CharInfo._CurrentAttackTypeInfo.Remove(CharInfo.BaseAttackTypeInfo.Where(r => r.AttackInput == stolenAtk[i].AttackInput).First());

        }
        //temp_SOAtkBase.StaminaCost = CharInfo.BaseAttackTypeInfo.Where(r => r.AttackInput == stolenAtk.AttackInput).First().StaminaCost;
        CharInfo._CurrentAttackTypeInfo.AddRange(stolenAtk);
        yield return BattleManagerScript.Instance.WaitFor(duration, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause, () => CharInfo.HealthPerc <= 0 || !IsStealingOn);
        for (int i = 0; i < stolenAtk.Count; i++)
        {
            CharInfo._CurrentAttackTypeInfo.Remove(stolenAtk[i]);
        }

        if (IsStealingOn)
        {
            IsStealingOn = false;

            for (int i = 0; i < stolenAtk.Count; i++)
            {
                CharInfo._CurrentAttackTypeInfo.Add(CharInfo.BaseAttackTypeInfo.Where(r => r.AttackInput == stolenAtk[i].AttackInput).First());
            }
        }

    }

    CharacterActionType[] _StunDisabledActions = null;
    /// <summary>
    /// List of actions blocked when a character is stunned, kept seperate so they can be changed in a single place
    /// </summary>
    CharacterActionType[] StunDisabledActions
    {
        get
        {
            if(_StunDisabledActions == null)
                _StunDisabledActions = new CharacterActionType[] { CharacterActionType.Skill1, CharacterActionType.Skill2, CharacterActionType.Skill3, CharacterActionType.Move, CharacterActionType.Defence, CharacterActionType.Strong, CharacterActionType.Weak, CharacterActionType.SwitchCharacter };
            return _StunDisabledActions;
        }
    }
        
    public IEnumerator Teleport(BuffDebuffClass bdClass, Vector2Int nextPos, ParticlesType psIn, ParticlesType psOut, string anim)
    {
        isTeleporting = true;
        MovementActionType prevMoveType = CharInfo.Behaviour.MovementActionN;
        UpdateSwappableSO(MovementActionType.Teleport);
        ((ScriptableObjectBaseCharacterTeleport)currentMoveProfile).overrideTeleportParticleIn = psIn;
        ((ScriptableObjectBaseCharacterTeleport)currentMoveProfile).overrideTeleportParticleOut = psOut;
        currentMoveProfile.animState = anim;
        if (CharInfo.Behaviour.InputBehaviour == InputBehaviourType.AIInput)
        {
            ((ScriptableObjectAIInput)currentInputProfile).possiblePositions.Clear();
            ((ScriptableObjectAIInput)currentInputProfile).possiblePos = null;
        }
        CharInfo.WalkingSide = GridManagerScript.Instance.GetBattleTile(nextPos).WalkingSide; //Set the walking side to be viable
        yield return currentMoveProfile.StartMovement(nextPos);
        UpdateSwappableSO(prevMoveType);
        isTeleporting = false;
    }

    public float StatsMultipler(float b_Value, float multiplier)
    {
        return b_Value * multiplier;
    }

    public float GetBuffDebuffValue(BuffDebuffClass bdClass, float baseValue)
    {
        return bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(baseValue, bdClass.CurrentBuffValue) :
            bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.OnCasterAttack ? bdClass.CurrentBuffDebuff.Atk != null ? bdClass.EffectMaker.GetBaseDamage(bdClass.CurrentBuffDebuff.Atk.AttackInput) * bdClass.CurrentBuffValue : 0 :
            bdClass.CurrentBuffValue;
    }

    public IEnumerator Buff_DebuffCoroutine(BuffDebuffClass bdClass, string displayTextPopup = "", bool displayParticles = true)
    {
        GameObject ps = null;
        GameObject onCasterPS = null;
        float dur = 0;
        float psTime = 0;

        if (!string.IsNullOrEmpty(displayTextPopup))
        {
            EffectChangedEvent?.Invoke(displayTextPopup, bdClass.Effect.classification == StatusEffectType.Buff ? BattleFieldIndicatorType.Buff : BattleFieldIndicatorType.Debuff, this);
        }

        if (bdClass.BdIcon != null)
        {
            BuffDebuffIcons.Add(bdClass.BdIcon);
            RefreshBuffDebuffIcons();
        }
        if (bdClass.CasterBdIcon != null)
        {
            bdClass.EffectMaker.BuffDebuffIcons.Add(bdClass.CasterBdIcon);
            bdClass.EffectMaker.RefreshBuffDebuffIcons();
        }
        if (bdClass.Effect.AnimToFired != CharacterAnimationStateType.none)
        {
            SetAnimation(bdClass.Effect.AnimToFired);
        }

        if (displayParticles && bdClass.Effect.Particles != ParticlesType.None)
        {
            ParticleManagerScript.Instance.SetEmissivePsInTransform(bdClass.Effect.Particles, CharInfo.Facing, bdClass.Effect.AttachPsToHead ? CharInfo.Head : SpineAnim.transform,
                    CharInfo.GetComponent<MeshRenderer>(), ref ps);
            if (bdClass.Duration > 0)
            {
                bdClass.PsHelper = ps.GetComponent<ParticleHelperScript>();
                bdClass.PsHelper.UpdatePSTime(bdClass.Duration);
            }

            dur = bdClass.Duration;
        }

        if (bdClass.Effect.SetParticlesOnCaster && bdClass.Effect.ParticlesOnCaster != ParticlesType.None)
        {
            ParticleManagerScript.Instance.SetEmissivePsInTransform(bdClass.Effect.ParticlesOnCaster, bdClass.EffectMaker.CharInfo.Facing, bdClass.EffectMaker.SpineAnim.transform,
                      bdClass.EffectMaker.CharInfo.GetComponent<MeshRenderer>(), ref onCasterPS);
            if (bdClass.Duration > 0)
            {
                bdClass.OnCasterPsHelper = onCasterPS.GetComponent<ParticleHelperScript>();
                bdClass.OnCasterPsHelper.UpdatePSTime(bdClass.Duration);
            }
        }

        BaseCharacter target = bdClass.CurrentBuffDebuff.OnCaster ? bdClass.EffectMaker : this;
        yield return BattleManagerScript.Instance.WaitFor(bdClass.Effect.Delay, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause, () => (bdClass.Stop_Co || CharInfo.HealthPerc == 0 || target.died || !bdClass.CurrentBuffDebuff.useDelay));

        if (!bdClass.Stop_Co && (CharInfo.HealthPerc > 0) && !target.died)
        {
            float val = 0;
            float totalchangesVal = 0;
            Vector2 totalchangesValV = Vector2.zero;
            Vector2 valV = Vector2.zero;
            ElementalType prevElemental = ElementalType.Neutral;
            WalkingSideType prevWalking = WalkingSideType.LeftSide;
            Vector3 boxColliderSize = Vector3.zero;

            switch (bdClass.Stat)
            {
                case BuffDebuffStatsType.Regen:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                    target.CharInfo.Health += val;
                    StatsChangedEvent?.Invoke(val, BattleFieldIndicatorType.Heal, target);
                    break;
                case BuffDebuffStatsType.Drain:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? CharInfo.HealthStats.B_Health : CharInfo.HealthStats.Health);
                    if (bdClass.Duration == 0)
                    {
                        ps.transform.eulerAngles = new Vector3(0, 0, 0);
                        foreach (VFXOffsetToTargetVOL item in ps.GetComponentsInChildren<VFXOffsetToTargetVOL>(true))
                        {
                            item.cb = bdClass.EffectMaker;
                        }
                        StatsChangedEvent?.Invoke(val, BattleFieldIndicatorType.Heal, bdClass.EffectMaker);
                        StatsChangedEvent?.Invoke(val, BattleFieldIndicatorType.Damage, this);
                        bdClass.EffectMaker.CharInfo.Health += val;
                        CharInfo.Health -= val;
                    }
                    break;
                case BuffDebuffStatsType.Zombie:
                    if (CharInfo.Health > 0 && IsOnField)
                    {
                        if (CharInfo.BaseSpeed == 0)
                        {
                            bdClass.Duration = 0;
                        }
                        else
                        {
                            BattleManagerScript.Instance.Zombification(this, bdClass.Duration);
                        }
                    }
                    break;
                case BuffDebuffStatsType.Bleed:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                    target.CharInfo.Health += val;
                    StatsChangedEvent?.Invoke(Mathf.Abs(val), BattleFieldIndicatorType.Damage, target);
                    //Apply Bleed
                    ParticleManagerScript.Instance.FireParticlesInTransform(ParticlesType.Status_Debuff_Bleed, target.SpineAnim.transform);
                    break;
                case BuffDebuffStatsType.Cursed:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                    target.CharInfo.Health += val;
                    StatsChangedEvent?.Invoke(Mathf.Abs(val), BattleFieldIndicatorType.Damage, target);
                    SpineAnim.Start_ColorCurveLooper(bdClass.CurrentBuffDebuff.ColorSizeCurve, bdClass.CurrentBuffDebuff.ColorSize.ColorHueSat.color, CharInfo.BaseColorHueSat.color, bdClass.Effect.OvertimeRatio);
                    break;
                case BuffDebuffStatsType.AttackChange:
                    CharInfo.CurrentAttackTypeInfo.Add(bdClass.CurrentBuffDebuff.Atk);

                    break;
                case BuffDebuffStatsType.Legion:
                    AddressableCharacterInstancer CharCreator = null;
                    if (bdClass.CurrentBuffDebuff.ClonePrefab != CharacterNameType.None)
                    {
                        CharCreator = new AddressableCharacterInstancer(this, new TeamSideInformationClass(new List<ControllerType> { ControllerType.None }, bdClass.CurrentBuffDebuff.ClonePrefab, CharInfo.Side, LevelType.Novice),
                            transform.parent, CharInfo.Side, showHP: true, showEther: false, displayWarning: false);
                        while (CharCreator.IsWorking)
                            yield return null;
                    }


                    BattleManagerScript.Instance.CloneUnit(
                   bdClass.CurrentBuffDebuff.ClonePrefab == CharacterNameType.None ? this : CharCreator.Result, bdClass.Effect.Particles, bdClass.Effect.CloneAsManyAsCurrentEnemies ? BattleManagerScript.Instance.TeamInfo[CharInfo.Side].enemiesCharactersOnField.Length : bdClass.Effect.CloneAmount,
                   bdClass.CurrentBuffDebuff.ClonePowerScale, bdClass.CurrentBuffDebuff.CloneStartingEffect, bdClass.CurrentBuffDebuff.SpawnInClosePosition);
                    break;
                case BuffDebuffStatsType.Element:
                    prevElemental = target.CharInfo.Elemental;
                    target.CharInfo.Elemental = bdClass.CurrentBuffDebuff.Elemental;
                    break;
                case BuffDebuffStatsType.HP:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                    target.CharInfo.Health += val;
                    StatsChangedEvent?.Invoke(Mathf.Abs(val), val > 0 ? BattleFieldIndicatorType.Heal : BattleFieldIndicatorType.Damage, target);
                    break;
                case BuffDebuffStatsType.Armour:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Armour : target.CharInfo.HealthStats.Armour);
                    totalchangesVal += val;
                    target.CharInfo.HealthStats.Armour += val;
                    break;
                case BuffDebuffStatsType.ActionTime:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_MovementTime : target.CharInfo.SpeedStats.MovementTime);
                    totalchangesVal += val;
                    target.CharInfo.SpeedStats.MovementTime += val;
                    break;
                case BuffDebuffStatsType.ArmourType:
                    if (!target.CharInfo.HealthStats.ArmourT.GridFight_ContainsStruct(bdClass.CurrentBuffDebuff.ArmourT))
                    {
                        target.CharInfo.HealthStats.ArmourT.Add(bdClass.CurrentBuffDebuff.ArmourT);
                    }
                    break;
                case BuffDebuffStatsType.Speed_Base:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_BaseSpeed : target.CharInfo.SpeedStats.BaseSpeed);
                    totalchangesVal += val;
                    target.CharInfo.SpeedStats.BaseSpeed += val;

                    target.SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
                    break;
                case BuffDebuffStatsType.Speed_Movement:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_MovementSpeed : target.CharInfo.SpeedStats.MovementSpeed);
                    totalchangesVal += val;
                    target.CharInfo.SpeedStats.MovementSpeed += val;
                    break;
                /*  case BuffDebuffStatsType.Speed_Attack_Loop:
                      val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_AttackLoopDuration : target.CharInfo.SpeedStats.AttackLoopDuration);
                      totalchangesVal += val;
                      target.CharInfo.SpeedStats.AttackLoopDuration = val;

                      break;*/
                case BuffDebuffStatsType.Speed_Weak_Bullet:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_WeakBulletSpeed : target.CharInfo.SpeedStats.WeakBulletSpeed);
                    totalchangesVal += val;
                    target.CharInfo.SpeedStats.WeakBulletSpeed += val;

                    valV = new Vector2(GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_WeakBulletSpeedV.x : target.CharInfo.SpeedStats.WeakBulletSpeedV.x),
                        GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_WeakBulletSpeedV.y : target.CharInfo.SpeedStats.WeakBulletSpeedV.y));
                    totalchangesValV += valV;
                    target.CharInfo.SpeedStats.WeakBulletSpeedV += valV;
                    break;
                case BuffDebuffStatsType.Speed_Strong_Bullet:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_StrongBulletSpeed : target.CharInfo.SpeedStats.StrongBulletSpeed);
                    totalchangesVal += val;
                    target.CharInfo.SpeedStats.StrongBulletSpeed += val;

                    valV = new Vector2(GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_StrongBulletSpeedV.x : target.CharInfo.SpeedStats.StrongBulletSpeedV.x),
                        GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_StrongBulletSpeedV.y : target.CharInfo.SpeedStats.StrongBulletSpeedV.y));
                    totalchangesValV += valV;
                    target.CharInfo.SpeedStats.StrongBulletSpeedV += valV;
                    break;
                case BuffDebuffStatsType.Damage_Base:
                    val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.DamageStats.B_BaseDamage : target.CharInfo.DamageStats.BaseDamage);
                    totalchangesVal += val;
                    target.CharInfo.DamageStats.BaseDamage += val;
                    break;
                case BuffDebuffStatsType.Teleport:
                    //Debug.LogError("Teleport");
                    yield return Teleport(bdClass, bdClass.Effect.TeleportInRandom ? GridManagerScript.Instance.GetFreeBattleTile(CharInfo.WalkingSide).Pos : bdClass.Effect.FixedPos, bdClass.Effect.TeleportParticlesIn, bdClass.Effect.TeleportParticlesOut, bdClass.Effect.TeleportArrivingAnim);
                    break;
                case BuffDebuffStatsType.ScaleCharacterSize:
                    val = target.transform.localScale.x;
                    target.ReScaleSpineT(new Vector3(bdClass.CurrentBuffValue, bdClass.CurrentBuffValue, bdClass.CurrentBuffValue));
                    break;
                case BuffDebuffStatsType.WalkingSide:
                    prevWalking = target.CharInfo.WalkingSide;
                    target.CharInfo.WalkingSide = bdClass.CurrentBuffDebuff.WalkingSide;
                    break;
                case BuffDebuffStatsType.ShadowForm:
                    target.CharBoxCollider.enabled = false;
                    break;
                case BuffDebuffStatsType.BoxColliderSize:
                    boxColliderSize = target.CharBoxCollider.size;
                    if (bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Multiplier)
                    {
                        target.CharBoxCollider.size *= bdClass.CurrentBuffValue;
                    }
                    else if (bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Value)
                    {
                        target.CharBoxCollider.size = new Vector3(bdClass.CurrentBuffValue, bdClass.CurrentBuffValue, bdClass.CurrentBuffValue);
                    }
                    break;
                case BuffDebuffStatsType.ActionDisable_WeakAttack:
                    if (target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Weak))
                    {
                        target.CharActionlist.Remove(CharacterActionType.Weak);
                    }
                    break;
                case BuffDebuffStatsType.ActionDisable_StrongAttack:
                    if (target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Strong))
                    {
                        target.CharActionlist.Remove(CharacterActionType.Strong);
                    }
                    break;
                case BuffDebuffStatsType.ActionDisable_Skill1:
                    if (target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill1))
                    {
                        target.CharActionlist.Remove(CharacterActionType.Skill1);
                    }
                    break;
                case BuffDebuffStatsType.ActionDisable_Skill2:
                    if (target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill2))
                    {
                        target.CharActionlist.Remove(CharacterActionType.Skill2);
                    }
                    break;
                case BuffDebuffStatsType.ActionDisable_Mask:
                    if (target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill3))
                    {
                        target.CharActionlist.Remove(CharacterActionType.Skill3);
                    }
                    break;
                case BuffDebuffStatsType.ActionDisable_Move:
                    if (target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Move))
                    {
                        target.CharActionlist.Remove(CharacterActionType.Move);
                    }
                    break;
                case BuffDebuffStatsType.ActionDisable_Swap:
                    if (target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.SwitchCharacter))
                    {
                        target.CharActionlist.Remove(CharacterActionType.SwitchCharacter);
                    }
                    break;
                case BuffDebuffStatsType.Tile_Free:
                    if (target.CharInfo.IsSummon)
                    {
                        target.GetComponent<SummonScript>().countingDown = false;
                    }
                    break;
                case BuffDebuffStatsType.ChancgeColor:
                    SpineAnim.SetDefaultOverlayColor(bdClass.CurrentBuffDebuff.ColorSize.ColorHueSat);
                    break;
                case BuffDebuffStatsType.RemoveBuffs:
                    foreach (BuffDebuffClass item in BuffsDebuffsList.Where(r => r.Effect.classification == StatusEffectType.Buff && r.Level <= bdClass.Level))
                    {
                        item.Stop_Co = true;
                        item.Duration = 0;
                    }
                    break;
                case BuffDebuffStatsType.RemoveDebuffs:
                    foreach (BuffDebuffClass item in BuffsDebuffsList.Where(r => r.Effect.classification == StatusEffectType.Debuff && r.Level <= bdClass.Level))
                    {
                        item.Stop_Co = true;
                        item.Duration = 0;
                    }
                    break;
                case BuffDebuffStatsType.FireParticlesToChar:
                    LayersPs.Add(bdClass.PsHelper);
                    break;
                case BuffDebuffStatsType.Stun:
                    target.currentAttackProfile.InteruptAttack();
                    Vector3 pos = ps.transform.GetChild(0).localPosition;
                    pos.y = target.SpineAnim.skeleton.Data.Height * target.SpineAnim.skeleton.ScaleY * 0.001f;
                    ps.transform.GetChild(0).localPosition = pos;
                    for (int i = 0; i < StunDisabledActions.Length; i++)
                    {
                        if (target.CharActionlist.GridFight_ContainsStruct(StunDisabledActions[i]))
                            target.CharActionlist.Remove(StunDisabledActions[i]);
                    }
                    target.SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
                    CameraManagerScript.Shaker?.PlayShake("Getting_Hit");
                    break;
                case BuffDebuffStatsType.ChancgeColorWithCurve:
                    SpineAnim.Start_ColorCurveLooper(bdClass.CurrentBuffDebuff.ColorSizeCurve, bdClass.CurrentBuffDebuff.ColorSize.ColorHueSat.color, CharInfo.BaseColorHueSat.color, bdClass.CurrentBuffDebuff.OnDuration ? bdClass.Duration : bdClass.Effect.OvertimeRatio);
                    break;
                case BuffDebuffStatsType.DeathSentence:
                    if (bdClass.CurrentBuffDebuff.BoolValue)
                    {
                        target.SpineAnim.Start_ColorCurveLooper(bdClass.CurrentBuffDebuff.ColorSizeCurve, bdClass.CurrentBuffDebuff.ColorSize.ColorHueSat.color, CharInfo.BaseColorHueSat.color, bdClass.CurrentBuffDebuff.OnDuration ? bdClass.Duration : bdClass.Effect.OvertimeRatio);
                    }
                    break;
            }

            if (bdClass.Duration > 0)
            {
                dur = bdClass.Duration;

                while (bdClass.Timer <= (bdClass.Duration * BattleManagerScript.Instance.BaseActionTime) && !bdClass.Stop_Co && ((bdClass.Stat != BuffDebuffStatsType.Cursed && target.CharInfo.HealthPerc > 0 && !target.died) || bdClass.Stat == BuffDebuffStatsType.Cursed))
                {
                    yield return BattleManagerScript.Instance.WaitUpdate(() => (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle &&
                    (bdClass.Stat != BuffDebuffStatsType.MeleeAttack && !bdClass.Stop_Co)) || BattleManagerScript.Instance.isSkillHappening.Value, () => bdClass.Stop_Co);
                    bdClass.Timer += BattleManagerScript.Instance.DeltaTime;
                    psTime += BattleManagerScript.Instance.DeltaTime;
                    if (ps != null && dur != 0 && dur != (bdClass.Duration * BattleManagerScript.Instance.BaseActionTime))
                    {
                        ps.GetComponent<ParticleHelperScript>().UpdatePSTime(psTime + (bdClass.Duration * BattleManagerScript.Instance.BaseActionTime));
                    }

                    if (bdClass.Effect.OvertimeRatio != 0 && bdClass.Timer >= bdClass.Offset + (bdClass.Effect.OvertimeRatio * BattleManagerScript.Instance.BaseActionTime))
                    {
                        bdClass.Offset = bdClass.Timer;
                        switch (bdClass.Stat)
                        {
                            case BuffDebuffStatsType.Regen:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                                target.CharInfo.Health += val;
                                StatsChangedEvent?.Invoke(val, BattleFieldIndicatorType.Heal, target);
                                ParticleManagerScript.Instance.SetEmissivePsInTransform(ParticlesType.Status_Buff_Regen_Hit, target.CharInfo.Facing, target.SpineAnim.transform,
                                target.CharInfo.GetComponent<MeshRenderer>(), ref tempGameObject);
                                break;
                            case BuffDebuffStatsType.Drain:
                                if (!bdClass.Effect.BoolValue)
                                {
                                    StatsChangedEvent?.Invoke(val, BattleFieldIndicatorType.Heal, bdClass.EffectMaker);
                                    bdClass.EffectMaker.CharInfo.Health += val;
                                }
                                totalchangesVal += val;
                                StatsChangedEvent?.Invoke(val, BattleFieldIndicatorType.Damage, this);
                                CharInfo.Health -= val;
                                break;
                            case BuffDebuffStatsType.Bleed:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                                CharInfo.Health += val;
                                StatsChangedEvent?.Invoke(Mathf.Abs(val), BattleFieldIndicatorType.Damage, target);
                                ParticleManagerScript.Instance.FireParticlesInTransform(ParticlesType.Status_Debuff_Bleed, target.SpineAnim.transform);
                                break;
                            case BuffDebuffStatsType.Cursed:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                                CharInfo.Health += val;
                                StatsChangedEvent?.Invoke(Mathf.Abs(val), BattleFieldIndicatorType.Damage, target);
                                SpineAnim.Start_ColorCurveLooper(bdClass.CurrentBuffDebuff.ColorSizeCurve, bdClass.CurrentBuffDebuff.ColorSize.ColorHueSat.color, CharInfo.BaseColorHueSat.color, bdClass.Effect.OvertimeRatio);
                                break;
                            case BuffDebuffStatsType.MeleeAttack:
                                while (SpineAnim.CurrentAnim == "MeleeAtk")
                                {
                                    yield return null;
                                }

                                break;
                            case BuffDebuffStatsType.HP:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
                                target.CharInfo.Health += val;
                                StatsChangedEvent?.Invoke(Mathf.Abs(val), val > 0 ? BattleFieldIndicatorType.Heal : BattleFieldIndicatorType.Damage, target);
                                break;
                            case BuffDebuffStatsType.Armour:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Armour : target.CharInfo.HealthStats.Armour);
                                totalchangesVal += val;
                                target.CharInfo.HealthStats.Armour += val;
                                break;
                            case BuffDebuffStatsType.ActionTime:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_MovementTime : target.CharInfo.SpeedStats.MovementTime);
                                totalchangesVal += val;
                                target.CharInfo.SpeedStats.MovementTime += val;
                                break;
                            case BuffDebuffStatsType.Speed_Base:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_BaseSpeed : target.CharInfo.SpeedStats.BaseSpeed);
                                totalchangesVal += val;
                                target.CharInfo.SpeedStats.BaseSpeed += val;

                                target.SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);

                                break;
                            case BuffDebuffStatsType.Speed_Movement:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_MovementSpeed : target.CharInfo.SpeedStats.MovementSpeed);
                                totalchangesVal += val;
                                target.CharInfo.SpeedStats.MovementSpeed += val;
                                break;
                            /*   case BuffDebuffStatsType.Speed_Attack_Loop:
                                   val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_AttackLoopDuration : target.CharInfo.SpeedStats.AttackLoopDuration);
                                   totalchangesVal += val;
                                   target.CharInfo.SpeedStats.AttackLoopDuration = val;
                                   break;*/
                            case BuffDebuffStatsType.Speed_Weak_Bullet:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_WeakBulletSpeed : target.CharInfo.SpeedStats.WeakBulletSpeed);
                                totalchangesVal += val;
                                target.CharInfo.SpeedStats.WeakBulletSpeed += val;

                                valV = new Vector2(GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_WeakBulletSpeedV.x : target.CharInfo.SpeedStats.WeakBulletSpeedV.x),
                                    GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_WeakBulletSpeedV.y : target.CharInfo.SpeedStats.WeakBulletSpeedV.y));
                                totalchangesValV += valV;
                                target.CharInfo.SpeedStats.WeakBulletSpeedV += valV;
                                break;
                            case BuffDebuffStatsType.Speed_Strong_Bullet:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_StrongBulletSpeed : target.CharInfo.SpeedStats.StrongBulletSpeed);
                                totalchangesVal += val;
                                target.CharInfo.SpeedStats.StrongBulletSpeed += val;

                                valV = new Vector2(GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_StrongBulletSpeedV.x : target.CharInfo.SpeedStats.StrongBulletSpeedV.x),
                                    GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.SpeedStats.B_StrongBulletSpeedV.y : target.CharInfo.SpeedStats.StrongBulletSpeedV.y));
                                totalchangesValV += valV;
                                target.CharInfo.SpeedStats.StrongBulletSpeedV += valV;
                                break;
                            case BuffDebuffStatsType.Damage_Base:
                                val = GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.DamageStats.B_BaseDamage : target.CharInfo.DamageStats.BaseDamage);
                                totalchangesVal += val;
                                target.CharInfo.DamageStats.BaseDamage += val;
                                break;
                            case BuffDebuffStatsType.ChancgeColorWithCurve:
                                if(!bdClass.CurrentBuffDebuff.OnDuration)
                                {
                                    SpineAnim.Start_ColorCurveLooper(bdClass.CurrentBuffDebuff.ColorSizeCurve, bdClass.CurrentBuffDebuff.ColorSize.ColorHueSat.color, CharInfo.BaseColorHueSat.color, bdClass.Effect.OvertimeRatio);
                                }
                                break;
                            case BuffDebuffStatsType.DeathSentence:
                                if (!bdClass.CurrentBuffDebuff.OnDuration && bdClass.CurrentBuffDebuff.BoolValue)
                                {
                                    target.SpineAnim.Start_ColorCurveLooper(bdClass.CurrentBuffDebuff.ColorSizeCurve, bdClass.CurrentBuffDebuff.ColorSize.ColorHueSat.color, CharInfo.BaseColorHueSat.color, bdClass.Effect.OvertimeRatio);
                                }
                                break;
                        }
                    }
                }

                switch (bdClass.Stat)
                {
                    case BuffDebuffStatsType.ChancgeColorWithCurve:
                        //SpineAnim.SetDefaultOverlayColor(CharInfo.BaseColorHueSat);
                        SpineAnim.Stop_ColorCurveLooper();
                        break;
                    case BuffDebuffStatsType.Drain:
                        if (bdClass.Effect.BoolValue)
                        {
                            StatsChangedEvent?.Invoke(totalchangesVal, BattleFieldIndicatorType.Heal, bdClass.EffectMaker/*SpineAnim.transform*/);
                            bdClass.EffectMaker.CharInfo.Health += totalchangesVal;
                        }
                        break;
                    case BuffDebuffStatsType.AttackChange:
                        target.CharInfo.CurrentAttackTypeInfo.Remove(bdClass.CurrentBuffDebuff.Atk);
                        break;
                    case BuffDebuffStatsType.Element:
                        target.CharInfo.Elemental = prevElemental;
                        break;
                    case BuffDebuffStatsType.Armour:
                        target.CharInfo.HealthStats.Armour -= totalchangesVal;
                        break;
                    case BuffDebuffStatsType.ActionTime:
                        target.CharInfo.SpeedStats.MovementTime -= totalchangesVal;
                        break;
                    case BuffDebuffStatsType.ArmourType:
                        if (target.CharInfo.HealthStats.B_ArmourT.GridFight_ContainsStruct(bdClass.CurrentBuffDebuff.ArmourT) && !target.CharInfo.HealthStats.ArmourT.GridFight_ContainsStruct(bdClass.CurrentBuffDebuff.ArmourT))
                        {
                            target.CharInfo.HealthStats.ArmourT.Add(bdClass.CurrentBuffDebuff.ArmourT);
                        }
                        break;
                    case BuffDebuffStatsType.StopChar:
                        target.CharInfo.BaseSpeed = CharInfo.SpeedStats.Current_B_BaseSpeed;
                        target.SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);

                        if (target.CharInfo.Behaviour.InputBehaviour == InputBehaviourType.PlayerInput)
                        {
                            if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.SwitchCharacter))
                            {
                                target.CharActionlist.Add(CharacterActionType.SwitchCharacter);
                            }
                            if (target.CharInfo.HealthPerc > 0)
                            {
                                yield return BattleManagerScript.Instance.RemoveCharacterFromBaord(target, true);
                            }

                        }
                        break;
                    case BuffDebuffStatsType.Speed_Base:

                        target.CharInfo.SpeedStats.BaseSpeed -= totalchangesVal;

                        target.SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
                        break;
                    case BuffDebuffStatsType.Speed_Movement:
                        target.CharInfo.SpeedStats.MovementSpeed -= totalchangesVal;
                        if (isMoving)
                        {
                            SpineAnim.SetAnimationSpeed(GetMovementAnimSpeed(SpineAnim.CurrentAnim));
                        }
                        break;
                    /*   case BuffDebuffStatsType.Speed_Attack_Loop:
                         //  target.CharInfo.SpeedStats.AttackLoopDuration -= totalchangesVal;
                           break;*/
                    case BuffDebuffStatsType.Speed_Weak_Bullet:
                        target.CharInfo.SpeedStats.WeakBulletSpeed -= totalchangesVal;

                        target.CharInfo.SpeedStats.WeakBulletSpeedV -= totalchangesValV;
                        break;
                    case BuffDebuffStatsType.Speed_Strong_Bullet:
                        target.CharInfo.SpeedStats.StrongBulletSpeed -= totalchangesVal;

                        target.CharInfo.SpeedStats.StrongBulletSpeedV -= totalchangesValV;
                        break;
                    case BuffDebuffStatsType.Damage_Base:
                        target.CharInfo.DamageStats.BaseDamage -= totalchangesVal;
                        break;
                    case BuffDebuffStatsType.ScaleCharacterSize:
                        target.ReScaleSpineT(new Vector3(val, val, val));
                        break;
                    case BuffDebuffStatsType.WalkingSide:
                        CharInfo.WalkingSide = BattleManagerScript.Instance.TeamInfo[CharInfo.Side].WalkingSide;
                        if (GridManagerScript.Instance.GetBattleTile(CharInfo.CurrentTilePos).WalkingSide != CharInfo.WalkingSide && CharInfo.HealthPerc > 0 && !died)
                        {
                            yield return Teleport(bdClass, GridManagerScript.Instance.GetFreeBattleTile(CharInfo.WalkingSide).Pos, bdClass.Effect.TeleportParticlesIn, bdClass.Effect.TeleportParticlesOut, "Idle");
                        }
                        break;
                    case BuffDebuffStatsType.BoxColliderSize:

                        if (bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Multiplier)
                        {
                            target.CharBoxCollider.size /= bdClass.CurrentBuffValue;
                        }
                        else if (bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Value)
                        {
                            target.CharBoxCollider.size = boxColliderSize;
                        }
                        break;
                    case BuffDebuffStatsType.ShadowForm:
                        target.CharBoxCollider.enabled = true;
                        break;
                    case BuffDebuffStatsType.ActionDisable_WeakAttack:
                        if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Weak) && target.B_CharActionlist.GridFight_ContainsStruct(CharacterActionType.Weak))
                        {
                            target.CharActionlist.Add(CharacterActionType.Weak);
                        }
                        break;
                    case BuffDebuffStatsType.ActionDisable_StrongAttack:
                        if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Strong) && target.B_CharActionlist.GridFight_ContainsStruct(CharacterActionType.Strong))
                        {
                            target.CharActionlist.Add(CharacterActionType.Strong);
                        }
                        break;
                    case BuffDebuffStatsType.ActionDisable_Skill1:
                        if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill1) && target.B_CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill1))
                        {
                            target.CharActionlist.Add(CharacterActionType.Skill1);
                        }
                        break;
                    case BuffDebuffStatsType.ActionDisable_Skill2:
                        if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill2) && target.B_CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill2))
                        {
                            target.CharActionlist.Add(CharacterActionType.Skill2);
                        }
                        break;
                    case BuffDebuffStatsType.ActionDisable_Mask:
                        if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill3) && target.B_CharActionlist.GridFight_ContainsStruct(CharacterActionType.Skill3))
                        {
                            target.CharActionlist.Add(CharacterActionType.Skill3);
                        }
                        break;
                    case BuffDebuffStatsType.ActionDisable_Move:
                        if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.Move) && target.B_CharActionlist.GridFight_ContainsStruct(CharacterActionType.Move))
                        {
                            target.CharActionlist.Add(CharacterActionType.Move);
                        }
                        break;
                    case BuffDebuffStatsType.ActionDisable_Swap:
                        if (!target.CharActionlist.GridFight_ContainsStruct(CharacterActionType.SwitchCharacter) && target.B_CharActionlist.GridFight_ContainsStruct(CharacterActionType.SwitchCharacter))
                        {
                            target.CharActionlist.Add(CharacterActionType.SwitchCharacter);
                        }
                        break;
                    case BuffDebuffStatsType.ChancgeColor:
                        SpineAnim.SetDefaultOverlayColor(CharInfo.BaseColorHueSat);
                        break;
                    case BuffDebuffStatsType.MeleeAttack:
                        CharInfo.WalkingSide = prevWalking;
                        if (!CharInfo.Pos.Contains(new Vector2Int((int)valV.x, (int)valV.y)))
                        {
                            BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int((int)valV.x, (int)valV.y));
                            if (bts.BattleTileState != BattleTileStateType.Empty)
                            {
                                bts = GridManagerScript.Instance.GetFreeInRowTileAdjacentTo(new Vector2Int((int)valV.x, (int)valV.y), CharInfo.WalkingSide);
                                if (bts == null)
                                {
                                    bts = GridManagerScript.Instance.GetFreeBattleTile(CharInfo.WalkingSide);
                                }
                            }
                            if (!bdClass.Stop_Co)
                            {
                                yield return Teleport(bdClass, bts.Pos, bdClass.Effect.MeleeBackTeleportParticlesIn, bdClass.Effect.MeleeBackTeleportParticlesOut, "Idle");
                            }
                        }
                        break;
                    case BuffDebuffStatsType.FireParticlesToChar:
                        LayersPs.Remove(bdClass.PsHelper);
                        break;
                    case BuffDebuffStatsType.Stun:
                        for (int i = 0; i < StunDisabledActions.Length; i++)
                        {
                            if (!target.CharActionlist.GridFight_ContainsStruct(StunDisabledActions[i]) && target.B_CharActionlist.GridFight_ContainsStruct(StunDisabledActions[i]))
                                target.CharActionlist.Add(StunDisabledActions[i]);
                        }
                        target.SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
                        //target.CharInfo.Shield = target.CharInfo.ShieldStats.B_Base;
                        break;
                    case BuffDebuffStatsType.DeathSentence:
                        target.CharInfo.Health = -5;
                        StatsChangedEvent?.Invoke(Mathf.Abs(-5), BattleFieldIndicatorType.Damage, target);
                        break;
                }
            }
        }

        CompleteBuffDebuff(bdClass);
    }

    void CompleteBuffDebuff(BuffDebuffClass bdClass)
    {
       
        if (bdClass.Effect.StackType == BuffDebuffStackType.Stackable || bdClass.CurrentStack > 0)
        {
            foreach (BuffDebuffClass item in BuffsDebuffsList.Where(r => r.CurrentBuffDebuff.StatsToAffect == bdClass.CurrentBuffDebuff.StatsToAffect).ToList())
            {
                item.CurrentStack--;
            }
        }

        BuffsDebuffsList.Remove(bdClass);
        if (bdClass.BdIcon != null)
        {
            BuffDebuffIcons.Remove(bdClass.BdIcon);
            RefreshBuffDebuffIcons();
        }

        if (bdClass.CasterBdIcon != null)
        {
            bdClass.EffectMaker.BuffDebuffIcons.Remove(bdClass.CasterBdIcon);
            bdClass.EffectMaker.RefreshBuffDebuffIcons();
        }

        if (bdClass.PsHelper != null && bdClass.PsHelper.gameObject.activeInHierarchy)
        {
            bdClass.PsHelper.transform.SetParent(null);
            if (bdClass.PsHelper != null)
            {
                bdClass.PsHelper.StopWithDelay(0.5f);
            }
        }

        if (bdClass.OnCasterPsHelper != null && bdClass.OnCasterPsHelper.gameObject.activeInHierarchy)
        {
            bdClass.OnCasterPsHelper.transform.SetParent(null);
            if (bdClass.OnCasterPsHelper != null)
            {
                bdClass.OnCasterPsHelper.StopWithDelay(0.5f);
            }
        }
    }


    public void RefreshBuffDebuffIcons()
    {
        UMS.buffIconHandler.RefreshIcons(BuffDebuffIcons);
    }
    #endregion

    #region Animation

    public void ArrivingEvent(Vector3 psPos)
    {
        //CameraManagerScript.Shaker.PlayShake("Arriving_Impact");

        tempGameObject = ParticleManagerScript.Instance.GetParticle(CharInfo.ArrivingParticles);
        tempGameObject.transform.position = psPos;
        tempGameObject.SetActive(true);
    }

    public void QueueAnimation(string animState, bool loop = false, float transition = 0, float speed = 1f)
    {
        queuedAnim_Name = animState;
        queuedAnim_Loop = loop;
        queuedAnim_Transition = transition;
        queuedAnim_Speed = speed;
    }

    public bool PlayQueuedAnim()
    {
        if (queuedAnim_Name != "")
        {
            SetAnimation(queuedAnim_Name, queuedAnim_Loop, queuedAnim_Transition);
            SpineAnim.SetAnimationSpeed(queuedAnim_Speed);
            QueueAnimation("", false, 0f, 1f);
            return true;
        }
        return false;
    }

    public IEnumerator StartPuppetAnimation(string animState, int loops, bool _pauseOnEndFrame = false, float animSpeed = 1f, bool loop = false, bool infinite = false)
    {
        if (isPuppeting)
        {
            interruptPuppetAnim = true;
            while (isPuppeting)
            {
                yield return null;
            }
        }
        yield return PuppetAnimation(animState, loops, _pauseOnEndFrame, animSpeed, loop, infinite);
    }

    public IEnumerator PuppetAnimation(string animState, int loops, bool _pauseOnEndFrame = false, float animSpeed = 1f, bool loop = false, bool infinite = false)
    {
        isPuppeting = true;
        interruptPuppetAnim = false;
        puppetAnimCompleteTick = 0;
        int currentAnimPlay = 0;
        while (currentAnimPlay < loops || infinite)
        {
            SetAnimation(animState, loop, _pauseOnLastFrame: (currentAnimPlay + 1 == loops && _pauseOnEndFrame && !infinite));
            SpineAnim.SetAnimationSpeed(animSpeed);
            while (currentAnimPlay == puppetAnimCompleteTick && !interruptPuppetAnim)
            {
                yield return null;
            }
            if (interruptPuppetAnim) break;
            currentAnimPlay++;
        }
        isPuppeting = false;
    }

    public virtual void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0, bool useFormeNaming = true, bool overrideRules = false)
    {
        SetAnimation(animState.ToString(), loop, transition, useFormeNaming: useFormeNaming, overrideRules: overrideRules);
    }

    public IEnumerator SlowDownAnimation(float perc, System.Func<bool> condition)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() =>
        {
            SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed * perc);
        }, condition);
        SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
    }

    public string GetAnimName(string AnimState) => CharInfo.CurFormeAnimPrefix + AnimState + CharInfo.CurFormeAnimSuffix;

    public virtual void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false, bool useFormeNaming = true, bool overrideRules = false)
    {
        //Debug.Log(CharInfo.CharacterID + "   " + animState + "   " + Time.time);
        if (overrideRules)
        {
            goto rulesOverriden;
        }

        //RULES______RULES______RULES______RULES______RULES______RULES______RULES______RULES______RULES______RULES______RULES______RULES______RULES
        if (animState.Contains("Defeat"))
        {
            currentAttackProfile.InteruptAttack();
        }

        if ((currentAttackProfile.CurrentAttackPhase(AttackInputType.Strong) == AttackPhasesType.Reset && !animState.Contains("Reverse")) ||
            //(CharInfo.BaseSpeed == 0 && !BattleManagerScript.Instance.isSkillHappening.Value) ||
            SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Defeat_ReverseArrive.ToString()) ||
            (SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Arriving.ToString()) && !animState.Contains("S_")) ||
            SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Reverse_Arriving.ToString()) ||
            (HasBuffDebuff(BuffDebuffStatsType.MeleeAttack) && (animState != "MeleeAtk" && animState != "Idle")))
        {
            //Debug.Log(CharInfo.CharacterID + "   " + animState + "   exit1");
            return;
        }

        if (animState.Contains(CharacterAnimationStateType.Defeat_ReverseArrive.ToString()) ||
            animState.Contains(CharacterAnimationStateType.Arriving.ToString()))
        {
            currentAttackProfile.InteruptAttack();
        }

        if (currentAttackProfile.SetAnimation(animState, loop, transition, _pauseOnLastFrame) ||
          currentMoveProfile.SetAnimation(animState, loop, transition, _pauseOnLastFrame) ||
          currentInputProfile.SetAnimation(animState, loop, transition, _pauseOnLastFrame) ||
          currentDeathProfile.SetAnimation(animState, loop, transition, _pauseOnLastFrame))
        {
            //Debug.Log(CharInfo.CharacterID + "   " + animState + "   exit2");
            return;
        }



        if (CharInfo.Behaviour.InputBehaviour == InputBehaviourType.PlayerInput && animState.Contains("IdleToAtk"))
        {
            AnimSpeed = (SpineAnim.GetAnimLenght(animState) / CharInfo.SpeedStats.IdleToAttackDuration) * CharInfo.BaseSpeed;
        }
        else if (CharInfo.Behaviour.InputBehaviour == InputBehaviourType.PlayerInput && !animState.Contains("Atk1") && animState.Contains("AtkToIdle"))
        {
            if (animState.Contains("Atk2") && CharInfo.SpeedStats.OverrideAtkToIdleDuration)
            {
                AnimSpeed = (SpineAnim.GetAnimLenght(animState) / CharInfo.SpeedStats.AttackToIdleDuration) * CharInfo.BaseSpeed;
            }
            else if (animState.Contains("S_Buff") && CharInfo.SpeedStats.Override_Buff_AtkToIdleDuration)
            {
                AnimSpeed = (SpineAnim.GetAnimLenght(animState) / CharInfo.SpeedStats.Buff_AttackToIdleDuration) * CharInfo.BaseSpeed;
            }
            else if (animState.Contains("S_DeBuff") && CharInfo.SpeedStats.Override_Debuff_AtkToIdleDuration)
            {
                AnimSpeed = (SpineAnim.GetAnimLenght(animState) / CharInfo.SpeedStats.Debuff_AttackToIdleDuration) * CharInfo.BaseSpeed;
            }
        }
        else if (animState.Contains("Arriv") || animState.Contains("Jump"))
        {
            AnimSpeed = BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets || BattleManagerScript.Instance.BattleSpeed == 0.015f ?
                3 : BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle ?
                CharInfo.Behaviour.InputBehaviour == InputBehaviourType.PlayerInput ? 10 : 3 : 3;        // animState.GridFight_ContainsStruct("Rev") ? CharInfo.SpeedStats.LeaveAnimSpeed : CharInfo.SpeedStats.ArriveAnimSpeed;
        }
        else if (animState.Contains("Defending"))
        {
            AnimSpeed = defenceAnimSpeedMultiplier;
        }
        else
        {
            AnimSpeed = CharInfo.BaseSpeed;
        }
        //=====================================================================================================================

        rulesOverriden:
        animState = useFormeNaming ? GetAnimName(animState) : animState;
        // Debug.Log(animState + "    " + SpineAnim.CurrentAnim);

        if (!SpineAnim.HasAnimation(animState))
        {
            // Debug.Log(CharInfo.CharacterID + "   " + animState + "   exit3");
            return;
        }

        //Debug.Log(CharInfo.CharacterID + "   " + animState + " no  exit");
        pauseOnLastFrame = _pauseOnLastFrame;
        SpineAnim.SetAnim(animState, loop, transition);
        SpineAnim.SetAnimationSpeed(AnimSpeed);
    }


    public void SpineAnimatorsetup(bool showHP = true, bool showEther = true, bool setSpine = true)
    {
        if(SpineAnim == null)
        {
            SpineAnim = GetComponentInChildren<SpineAnimationManager>(true);

            if (setSpine)
            {
                SpineAnim.SetupSpineAnim();
                spineT = SpineAnim.transform;
                SpineAnim.gameObject.tag = CharInfo.Side.ToString();
                if (SpineAnim.SpineAnimationState != null)
                {
                    SpineAnim.CharOwner = this;
                    SpineAnim.SpineAnimationState.Complete += SpineAnimationState_Complete;
                    SpineAnim.SpineAnimationState.Event += SpineAnimationState_Event;


                }
            }
        }
       
        UMS.VitalityContainer.parent = SpineAnim.transform;
        UMS.buffIconHandler.transform.parent = SpineAnim.transform;
        UMS.EnableBattleBars(showHP);
    }

    public virtual void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name.Equals("StopDefending") && CharInfo.Behaviour.InputBehaviour == InputBehaviourType.PlayerInput)
        {
            SpineAnim.SetAnimationSpeed(0);
        }
        else if (e.Data.Name.Equals("FireArrivingParticle"))
        {
            if (AnimSpeed == 3 && FireArrivingPs_Audio)
            {
                ArrivingEvent(transform.position);
            }
            else if(!FireArrivingPs_Audio)
            {
                FireArrivingPs_Audio = true;
            }
        }
        else if (e.Data.Name.Equals("AudioEvent"))
        {
        }
        else if (e.Data.Name.Equals("CameraShake"))
        {
            CameraManagerScript.Shaker.PlayShake(e.String);
        }
        else if (e.Data.Name.Equals("FireCastParticle"))
        {
            CurrentAttackInfoClass caic = currentAttackProfile.CurrentAttack(trackEntry.Animation.Name);
            if (caic != null)
            {
                currentAttackProfile.CastAttackParticles(currentAttackProfile.CurrentAttack(trackEntry.Animation.Name));
                if (caic.CurrentAttack.AttackInput == AttackInputType.Strong)
                {
                    // ParticleManagerScript.Instance.SetEmissivePsInTransform(CharInfo.CastActivationPS, CharInfo.Facing, SpineAnim.transform,
                    // CharInfo.GetComponent<MeshRenderer>(), ref CastActivationPS);
                    StopChargingPs(false);
                }


                if (currentAttackProfile.SwappableType == SwappableActionType.Particles)
                {
                    currentAttackProfile.CreateAttack(currentInputProfile.nextAttackPos, currentAttackProfile.CurrentAttack(trackEntry.Animation.Name));
                }
                else if (currentAttackProfile.SwappableType == SwappableActionType.Tiles)
                {
                    ((ScriptableObjectBaseCharacterTilesAttack)currentAttackProfile).CreateTileAttackBullets(currentInputProfile.nextAttackPos, currentAttackProfile.CurrentAttack(trackEntry.Animation.Name));
                }

                caic.currentAttackPhase = AttackPhasesType.Shoot;
            }
        }
        else if (e.Data.Name.Equals("FireBulletParticle"))
        {
            /*CharInfo.Ether -= CharInfo.IsTired ? 0 : currentAttackProfile.CurrentAttack(trackEntry.Animation.Name).CurrentAttack.StaminaCost;
            if (currentAttackProfile.SwappableType == SwappableActionType.Particles)
            {
                currentAttackProfile.CreateAttack(currentInputProfile.nextAttackPos, currentAttackProfile.CurrentAttack(trackEntry.Animation.Name));
            }
            else if (currentAttackProfile.SwappableType == SwappableActionType.Tiles)
            {
                currentAttackProfile.CreateBullet(currentInputProfile.nextAttackPos, currentAttackProfile.CurrentAttack(trackEntry.Animation.Name));
            }*/
        }
        else if (e.Data.Name.Equals("FireTileAttack") && currentAttackProfile.SwappableType == SwappableActionType.Tiles)
        {
            currentAttackProfile.CreateAttack(currentInputProfile.nextAttackPos, currentAttackProfile.CurrentAttack(trackEntry.Animation.Name));
        }
        else
        {
            for (int i = 0; i < SpineAnim.CustomEvents.Count; i++)
            {
                SpineAnim.CustomEvents[i].TryDoEvent(e.Data.Name);
            }
        }
    }

    public virtual void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        //Debug.Log(CharInfo.CharacterID +  "    Completed   " + trackEntry.Animation.Name + "   " + Time.time);
        if (isPuppeting) puppetAnimCompleteTick++;
        if (pauseOnLastFrame) return;
        if (PlayQueuedAnim()) return;

        if (trackEntry.Animation.Name == "<empty>" || trackEntry.Animation.Name != SpineAnim.CurrentAnim || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString() || (SpineAnim.CurrentAnim == CharacterAnimationStateType.Defeat.ToString() && CharInfo.Behaviour.DeathBehaviour == DeathBehaviourType.Defeat)
          || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString() || (isMoving && (!trackEntry.Animation.Name.Contains("Dash") && !trackEntry.Animation.Name.Contains("_"))))
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;

        if ((completedAnim.Contains(CharacterAnimationStateType.Arriving.ToString()) && !completedAnim.Contains("Reverse")) || completedAnim == CharacterAnimationStateType.JumpTransition_IN.ToString() || completedAnim.Contains("Growing"))
        {
            CharArrivedOnBattleField();
        }



        if (completedAnim == CharacterAnimationStateType.Defeat_IdleToLoop.ToString())
        {
            SetAnimation(CharacterAnimationStateType.Defeat_Loop, true);
            return;
        }

        if (completedAnim == CharacterAnimationStateType.Defeat.ToString() && CharInfo.Behaviour.DeathBehaviour == DeathBehaviourType.Defeat_And_Explode)
        {
            SpineAnim.CurrentAnim = "";
            return;
        }

        if (completedAnim == "MeleeAtk")
        {
            ResetTargetBuffDebuff(BuffDebuffStatsType.MeleeAttack);
        }

        if (completedAnim.Contains("Speaking_IdleToLoop"))
        {
            SetAnimation(completedAnim.Contains("Flip") ? "Speaking_Loop_Flip" : "Speaking_Loop", true);
            return;
        }

        if (currentAttackProfile.SpineAnimationState_Complete(completedAnim) ||
            currentMoveProfile.SpineAnimationState_Complete(completedAnim) ||
            currentInputProfile.SpineAnimationState_Complete(completedAnim) ||
            currentDeathProfile.SpineAnimationState_Complete(completedAnim))
        {
            return;
        }

        SpineAnimationState_Complete(completedAnim);

        if (completedAnim != CharacterAnimationStateType.Idle.ToString() && !SpineAnim.Loop)
        {
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
            SetAnimation(CharacterAnimationStateType.Idle, true);
        }
    }

    public virtual void SpineAnimationState_Complete(string completedAnim)
    {
    }

    #endregion

    public IEnumerator StartMovement(InputDirectionType inDir, float value)
    {
        if (!BattleManagerScript.Instance.isSkillHappening.Value && !HasBuffDebuff(BuffDebuffStatsType.MeleeAttack) && !HasBuffDebuff(BuffDebuffStatsType.StopChar) && !isTeleporting && !isMoving && (currentAttackProfile.CurrentAttackPhase(AttackInputType.Strong) <= AttackPhasesType.Charging || currentAttackProfile.CurrentAttackPhase(AttackInputType.Strong) == AttackPhasesType.End))
        {
            if (currentAttackProfile.CurrentAttackPhase(AttackInputType.Strong) != AttackPhasesType.Firing && CharInfo.BaseSpeed > 0)
            {
                FireActionEvent(CharacterActionType.Move);
                //Debug.Log(CharInfo.CharacterID + "    " + inDir + "    " + Time.time + "    " + isMoving);
                yield return currentMoveProfile.StartMovement(inDir);
            }
        }
    }


    DefendingActionType atkRes;
    GameObject psGo = null;
    public virtual DefendingActionType SetDamage(DamageInfoClass damageInfo, float damage)
    {
        atkRes = DefendingActionType.None;

        if (!IsOnField ||
            (died && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle) ||
            BattleManagerScript.Instance.CurrentBattleState == BattleState.WaveEnd)
        {
            return DefendingActionType.Normal;
        }

       

        if (damageInfo.Attacker.ReferenceCharacter.CharInfo.Side != CharInfo.Side)
        {
            atkRes = currentInputProfile.CheckAction(damageInfo, ref damage);
        }

        if(damageInfo.Attacker.ReferenceCharacter.CharInfo.Behaviour.InputBehaviour == InputBehaviourType.AIInput)
        {
            damageInfo.Attacker.ReferenceCharacter.currentInputProfile.WaitingForHit = false;
        }
        //  Debug.Log(CharInfo.CharacterID + "    " + BattleManagerScript.Instance.CurrentSelectedCharacters[0].SwapState);
    
        SpineAnim.PlayHitWhiteOut();

     

        if (CharInfo.HealthStats.ArmourT.GridFight_ContainsStruct(ArmourType.Blocking) && damageInfo.BulletOwner != null)
        {
            damageInfo.BulletOwner.isMoving = false;
            if (damageInfo.BulletOwner.BulletT.GridFight_ContainsStruct(BulletType.Piercing))
            {
                damageInfo.BulletOwner.BulletT.Remove(BulletType.Piercing);
                damageInfo.BulletOwner.BulletT.Add(BulletType.Base);
            }
        }

        if (damageInfo.Attacker == this && HasBuffDebuff(BuffDebuffStatsType.Backfire) && damage > 0f)
        {
            healthCT = BattleFieldIndicatorType.Backfire;
        }
        else if (HasBuffDebuff(BuffDebuffStatsType.Invulnerable))
        {
            damage = 0;
            healthCT = BattleFieldIndicatorType.Invulnerable;
            psGo = ParticleManagerScript.Instance.FireParticlesInTransform(CharInfo.Pos.Count == 1 ? ParticlesType.ShieldTotalDefence : ParticlesType.ShieldBigTotalDefence, transform, true, CharInfo.Facing, false);
        }


        if (damageInfo.isReflected)
        {
            ParticleManagerScript.Instance.SetEmissivePsInTransform(ParticlesType.ReflectedBulletImpact, CharInfo.Facing, SpineAnim.transform,
            CharInfo.GetComponent<MeshRenderer>(), ref psGo);

            //Slow down the effect is time is slowed on impact
            ParticleHelperScript particleHelperScript = psGo.GetComponent<ParticleHelperScript>();

            if (particleHelperScript != null && damageInfo.AtkSO.EffectTimeOnImpact)
            {
                particleHelperScript.SetSimulationSpeedOverTime(damageInfo.AtkSO.SlowDownOnHit);
            }
        }

        healthCT = damage < 0 ? BattleFieldIndicatorType.Heal : damage == 0 ? BattleFieldIndicatorType.NoDamage : damageInfo.IsCritical ? BattleFieldIndicatorType.CriticalHit : BattleFieldIndicatorType.Damage;


        float elementDamageMultiplier = BattleManagerScript.Instance.elementData.AttackDamageMultiplier(
        damageInfo.AtkSO != null ? damageInfo.AtkSO.UseAttackerElement ? new ElementalType[] { damageInfo.Attacker.ReferenceCharacter.CharInfo.Elemental } : damageInfo.AtkSO.AttackElements.ToArray() : new ElementalType[] { ElementalType.Neutral },
        AllElements);



        if (damageInfo.BulletOwner == null || !damageInfo.BulletOwner.BulletT.GridFight_ContainsStruct(BulletType.Unstoppable))
        {
            if (healthCT != BattleFieldIndicatorType.Heal && healthCT != BattleFieldIndicatorType.NoDamage)
            {
                damage *= elementDamageMultiplier;
                if (damage <= CharInfo.HealthStats.Armour)
                {
                    healthCT = BattleFieldIndicatorType.Armored;
                    StatsChangedEvent?.Invoke(69, BattleFieldIndicatorType.Armored, this);
                    damage = 0;
                }
                else
                {
                    healthCT = elementDamageMultiplier == 1f ? healthCT : elementDamageMultiplier > 1f ? BattleFieldIndicatorType.Effective : BattleFieldIndicatorType.UnEffective;
                    if (elementDamageMultiplier != 1f)
                        StatsChangedEvent?.Invoke(69, elementDamageMultiplier > 1f ? BattleFieldIndicatorType.EffectiveText : BattleFieldIndicatorType.InEffectiveText, this);
                    damage = damage - CharInfo.HealthStats.Armour;
                }
            }
        }


        if (damageInfo.Attacker.ReferenceCharacter.CharInfo.Side != CharInfo.Side)
        {
            if (atkRes == DefendingActionType.None || atkRes == DefendingActionType.Undefendable)
            {
                if (HasBuffDebuff(BuffDebuffStatsType.Undead) && damage != 0)
                {
                    damage *= -1;
                }

                if (HasBuffDebuff(BuffDebuffStatsType.Stun) && damage != 0)
                {
                    damage *= GetBuffDebuff(BuffDebuffStatsType.Stun).CurrentBuffValue;
                    healthCT = BattleFieldIndicatorType.StugDamage;
                }

                if (damage > 0)
                {
                    if (SpineAnim.CurrentAnim.Contains(damageInfo.AtkSO.AnimToFireOnHit.ToString()))
                    {
                        SpineAnim.skeletonAnimation.state.GetCurrent(0).TrackTime = 0;
                    }
                    else
                    {
                        SetAnimation(damageInfo.AtkSO.AnimToFireOnHit);
                    }
                }
            }
            else
            {
                if(healthCT != BattleFieldIndicatorType.NoDamage)
                {
                    healthCT = BattleFieldIndicatorType.Shielded;
                }
            }
        }

       

        FireActionEvent(CharacterActionType.GettingHit);

        SetFinalDamage((BaseCharacter)damageInfo.Attacker, damage);// / GridManagerScript.Instance.GetBattleTile(CharInfo.CurrentTilePos).TileADStats.y);

        if (damage != 0 && healthCT != BattleFieldIndicatorType.NoDamage)
        {
            //StatsChangedEvent?.Invoke(Mathf.Abs(damage), healthCT, this);
        }


        if (!died && CharInfo.HealthPerc > 0 && atkRes != DefendingActionType.Reflected)
        {
            foreach (ScriptableObjectAttackEffect eff in damageInfo.Effects)
            {
                if (CanBeAffectedByTilesEffect && ((eff.classification == StatusEffectType.Buff && CharInfo.Side == damageInfo.Attacker.ReferenceCharacter.CharInfo.Side)
                    || (eff.classification == StatusEffectType.Debuff && CharInfo.Side != damageInfo.Attacker.ReferenceCharacter.CharInfo.Side)))
                {
                    Buff_DebuffCo(damageInfo.Attacker.ReferenceCharacter, eff, damageInfo.AtkSO);
                }
            }
        }


        if ((CharInfo.CharacterID == CharacterNameType.CrystalLeft && damageInfo.Attacker.ReferenceCharacter.CharInfo.Side == TeamSideType.RightSideTeam) ||
            (CharInfo.CharacterID == CharacterNameType.CrystalRight && damageInfo.Attacker.ReferenceCharacter.CharInfo.Side == TeamSideType.LeftSideTeam))
        {
            StartCoroutine(BattleManagerScript.Instance.RemoveCharacterFromBaord(damageInfo.Attacker.ReferenceCharacter, true));
        }

        return atkRes;
    }



    public float GetBaseDamage(AttackInputType atkType)
    {
        return CharInfo.DamageStats.BaseDamage;
    }

    public void SetLayer()
    {
        CharOredrInLayer = 101 + (CharInfo.CurrentTilePos.x * 13) + (CharInfo.Facing == FacingType.Right ? CharInfo.CurrentTilePos.y - 12 : CharInfo.CurrentTilePos.y);
        if (CharInfo.UseLayeringSystem)
        {
            foreach (ParticleHelperScript item in LayersPs)
            {
                item.UpdatePsSortingOrder(CharOredrInLayer);
            }
            SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
        }
    }

    public void CallStatsChangedEvent(float value, BattleFieldIndicatorType changeType, BaseCharacter charOwner)
    {
        StatsChangedEvent?.Invoke(value, changeType, charOwner);
    }

    public virtual void SetFinalDamage(IDamageMaker attacker, float damage, HitInfoClass hic = null)
    {
        hic = HittedByList.Where(r => r.CharacterId == attacker.CharName).FirstOrDefault();
        if (hic == null)
        {
            HittedByList.Add(new HitInfoClass(attacker, damage));
        }
        else
        {
            hic.Damage += damage;
            hic.UpdateLastHitTime();
        }
        if ((BaseCharacter)attacker != null) currentInputProfile.SetFinalDamage((BaseCharacter)attacker, ref damage, hic);

        if (CharInfo.HealthPerc > 0)
        {
            
            attacker?.MadeDamage(this, damage);
            CharInfo.Health -= damage;
            CharacterHealthChangedEvent?.Invoke(this);
        }
    }

    public void CallEtherEvent(BaseInfoScript charInfo, float perc, float val, bool instantChange = false)
    {
        CharacterEtherChangedEvent?.Invoke(this);
    }

    public void StopChargingPs(bool stopImmediately)
    {
        if (CastLoopPS != null && CastStartLoopPS != null)
        {
            if (!stopImmediately)
            {
                CastLoopPS.GetComponent<ParticleHelperScript>().UpdatePSMeshTime(0.1f);
            }
            else
            {
                CastLoopPS?.SetActive(false);
                CastStartLoopPS?.SetActive(false);
            }
            CastLoopPS.transform.parent = null;
            CastStartLoopPS.transform.parent = null;
            CastLoopPS = null;
            CastStartLoopPS = null;
        }
    }

    public virtual void MadeDamage(IDamageReceiver target, float damage)
    {

    }

    public ElementalWeaknessType GetElementalMultiplier(List<ElementalResistenceClass> armorElelmntals, ElementalType elementalToCheck)
    {
        int resVal = 0;

        foreach (ElementalResistenceClass elemental in armorElelmntals)
        {

            if (elemental.Elemental != elementalToCheck)
            {
                int res = (int)elemental.Elemental + (int)elementalToCheck;
                if (res > 0)
                {
                    res -= 8;
                }

                resVal += (int)(ElementalWeaknessType)System.Enum.Parse(typeof(ElementalWeaknessType), ((RelationshipBetweenElements)res).ToString().Split('_').First());
            }
            else
            {
                resVal = (int)ElementalWeaknessType.Neutral;
            }
        }

        return (ElementalWeaknessType)(resVal);
    }

    public void SetValueFromVariableName(string vName, object value)
    {
        GetType().GetField(vName).SetValue(this, value);
    }

    public void Dispose()
    {
    }

    public void FireActionEvent(CharacterActionType action)
    {
        CurrentCharStartingActionEvent?.Invoke(CurrentPlayerController, CharInfo.CharacterID, action, CharID);
    }

    public List<InputDirectionType> GetFreeDir()
    {
        List<InputDirectionType> res = new List<InputDirectionType>();

        if (isDirFree(InputDirectionType.Down))
        {
            res.Add(InputDirectionType.Down);
        }
        if (isDirFree(InputDirectionType.Up))
        {
            res.Add(InputDirectionType.Up);
        }
        if (isDirFree(InputDirectionType.Left))
        {
            res.Add(InputDirectionType.Left);
        }
        if (isDirFree(InputDirectionType.Right))
        {
            res.Add(InputDirectionType.Right);
        }

        return res;
    }

    BattleTileScript temp_bts = null;
    private bool isDirFree(InputDirectionType dir)
    {
        bool found = false;
        Vector2Int startingPos = CharInfo.CurrentTilePos;
        while (!found)
        {

            startingPos += dir == InputDirectionType.Up ? new Vector2Int(-1, 0) :
                dir == InputDirectionType.Down ? new Vector2Int(1, 0) :
                dir == InputDirectionType.Left ? new Vector2Int(0, -1) :
                new Vector2Int(0, 1);
            temp_bts = GridManagerScript.Instance.GetBattleTile(startingPos);
            if (temp_bts != null && temp_bts.BattleTileState == BattleTileStateType.Empty && temp_bts.WalkingSide == CharInfo.WalkingSide)
            {
                return true;
            }
            else
            {
                if (!CharInfo.Pos.GridFight_ContainsStruct(startingPos))
                {
                    found = true;
                }
            }
        }

        return false;
    }

    public void ResetSpineT()
    {
        foreach (Transform battleTransform in UMS.BattleTransforms)
        {
            battleTransform.localScale = Vector3.one;
        }
        spineT.localScale = Vector3.one;
    }


    public void ReScaleSpineT(Vector3 newScale)
    {
        Vector3 ScaleingDifferential = new Vector3(newScale.x / spineT.localScale.x, newScale.y / spineT.localScale.y, newScale.z / spineT.localScale.z);
        foreach (Transform battleTransform in UMS.BattleTransforms)
        {
            battleTransform.localScale = new Vector3(
                battleTransform.localScale.x / ScaleingDifferential.x,
                battleTransform.localScale.y / ScaleingDifferential.y,
                battleTransform.localScale.z / ScaleingDifferential.z
                );
        }
        spineT.localScale = newScale;
    }
}


[System.Serializable]
public struct DamageInfoClass
{
    public IDamageMaker Attacker;
    public ElementalType Elemental;
    public bool IsCritical;
    public bool IsAttackBlocking;
    public ScriptableObjectAttackBase AtkSO;
    public bool CanBeReflected;
    public bool isReflected;
    public Vector3 StartingPos;
    public bool Indefensible;
    public BulletScript BulletOwner;
    public List<ScriptableObjectAttackEffect> Effects;
    public bool DisplayTextPopup;

    public DamageInfoClass(BaseCharacter attacker, BulletScript bulletOwner, ScriptableObjectAttackBase atkSO, List<ScriptableObjectAttackEffect> effects,
        ElementalType elemental, bool isCritical, bool isAttackBlocking, bool canBeReflected, bool isreflected, Vector3 startingPos, bool indefensible = false, bool displayTextPopup = true)
    {
        IsCritical = isCritical;
        IsAttackBlocking = isAttackBlocking;
        Effects = effects;
        Attacker = attacker;
        Elemental = elemental;
        AtkSO = atkSO;
        CanBeReflected = canBeReflected;
        isReflected = isreflected;
        StartingPos = startingPos;
        Indefensible = indefensible;
        BulletOwner = bulletOwner;
        DisplayTextPopup = displayTextPopup;
    }
}

[System.Serializable]
public class CurrentBuffsDebuffsClass
{
    public ElementalResistenceClass ElementalResistence;
    public float Duration;
    public IEnumerator BuffDebuffCo;

    public CurrentBuffsDebuffsClass()
    {
    }

    public CurrentBuffsDebuffsClass(ElementalResistenceClass elementalResistence, IEnumerator buffDebuffCo, float duration)
    {
        ElementalResistence = elementalResistence;
        BuffDebuffCo = buffDebuffCo;
        Duration = duration;
    }
}


[System.Serializable]
public class BuffDebuffIconClass
{
    public Sprite Icon;
    public StatusEffectType Classification = StatusEffectType.Buff;
    public List<BuffDebuffStatsType> StatsToAffect = new List<BuffDebuffStatsType>();
    public bool RecolorCharUI = false;
    public Color StatusIconColor = Color.magenta;

    public BuffDebuffIconClass()
    {
    }

    public BuffDebuffIconClass(Sprite icon, StatusEffectType classification, BuffDebuffStatsType statsToAffect, bool recolorCharUI, Color statusIconColor)
    {
        Icon = icon;
        Classification = classification;
        StatsToAffect = new List<BuffDebuffStatsType> { statsToAffect };
        StatusIconColor = statusIconColor;
        RecolorCharUI = recolorCharUI;
    }

    public BuffDebuffIconClass(Sprite icon, StatusEffectType classification, List<BuffDebuffStatsType> statsToAffect, bool recolorCharUI, Color statusIconColor)
    {
        Icon = icon;
        Classification = classification;
        StatsToAffect = statsToAffect;
        StatusIconColor = statusIconColor;
        RecolorCharUI = recolorCharUI;
    }
}

[System.Serializable]
public class BuffDebuffClass
{
    public bool Stop_Co = false;
    public StatsToAffectClass CurrentBuffDebuff;
    public IEnumerator BuffDebuffCo;
    public float Timer;
    public float Offset = 0;
    public float Duration;
    public BuffDebuffStatsType Stat;
    public int Level;
    public ParticleHelperScript PsHelper;
    public ParticleHelperScript OnCasterPsHelper;
    public float CurrentBuffValue
    {
        get
        {
            return CurrentBuffDebuff.Value;
        }
    }
    public int CurrentStack = 1;
    public BaseCharacter EffectMaker;
    public ScriptableObjectAttackEffect Effect;
    public GameObject ps;
    public BuffDebuffIconClass BdIcon;
    public BuffDebuffIconClass CasterBdIcon;

    public BuffDebuffClass()
    {

    }

    public BuffDebuffClass(BuffDebuffStatsType stat, int level, StatsToAffectClass currentCuffDebuff, float duration, BuffDebuffIconClass bdIcon, BaseCharacter effectMaker, BuffDebuffIconClass casterBdIcon, ScriptableObjectAttackEffect effect)
    {
        Stat = stat;
        Level = level;
        CurrentBuffDebuff = currentCuffDebuff;
        Duration = duration;
        CurrentStack = 1;
        EffectMaker = effectMaker;
        Effect = effect;
        BdIcon = bdIcon;
        CasterBdIcon = casterBdIcon;
    }

    public BuffDebuffClass(BuffDebuffStatsType stat, int level, StatsToAffectClass currentCuffDebuff, IEnumerator buffDebuffCo, float duration, BuffDebuffIconClass bdIcon, BaseCharacter effectMaker, BuffDebuffIconClass casterBdIcon, ScriptableObjectAttackEffect effect)
    {
        Stat = stat;
        Level = level;
        CurrentBuffDebuff = currentCuffDebuff;
        BuffDebuffCo = buffDebuffCo;
        Duration = duration;
        CurrentStack = 1;
        EffectMaker = effectMaker;
        Effect = effect;
        BdIcon = bdIcon;
        CasterBdIcon = casterBdIcon;
    }
}

[System.Serializable]
public class RelationshipClass
{
    public string name;
    [HideInInspector] public CharacterNameType CharOwnerId = CharacterNameType.None;
    public CharacterNameType CharacterId = CharacterNameType.None;

    [SerializeField] private int BasicValue;
    public int _CurrentValue;


    public int CurrentValue
    {
        get
        {
            return BasicValue + _CurrentValue;
        }
        set
        {
            _CurrentValue = value - BasicValue;
        }
    }

    public RelationshipClass()
    {

    }

    public RelationshipClass(CharacterNameType charOwnerId, CharacterNameType characterId, int basicValue)
    {
        CharOwnerId = charOwnerId;
        CharacterId = characterId;
        BasicValue = basicValue;
    }
}

[System.Serializable]
public class CurrentAttackInfoClass
{
    public ScriptableObjectAttackBase CurrentAttack;
    public AttackPhasesType _currentAttackPhase = AttackPhasesType.End;
    public AttackPhasesType currentAttackPhase
    {
        get
        {
            return _currentAttackPhase;
        }
        set
        {

            if (value == AttackPhasesType.Firing)
            {

            }
            //Debug.Log("Phase    " + value);
            _currentAttackPhase = value;
        }
    }

    public ScriptableObjectBaseCharacterBaseAttack AttackSO;

    public bool interruptToIdle = true;
    public bool _isAttackInterupt = false;
    public bool isAttackInterupt
    {
        get
        {
            return AttackSO.CharOwner.isMoving ? true : _isAttackInterupt;
        }
        set
        {
            _isAttackInterupt = value;
        }
    }

    public int shotsLeftInAttack
    {
        get
        {
            return _shotsLeftInAttack;
        }
        set
        {
            _shotsLeftInAttack = value;
        }
    }
    public int _shotsLeftInAttack = 0;

    public CurrentAttackInfoClass()
    {

    }

    public CurrentAttackInfoClass(ScriptableObjectAttackBase currentAttack, ScriptableObjectBaseCharacterBaseAttack attackSO)
    {
        CurrentAttack = currentAttack;
        AttackSO = attackSO;
    }

    public float GetCurrentAttackDamage
    {
        get
        {
            return AttackSO.CharOwner.CharInfo.DamageStats.BaseDamage * CurrentAttack.DamageMultiplier;
               // * (BattleManagerScript.Instance.isSkillHappening.Value ? 1 : GridManagerScript.Instance.GetBattleTile(AttackSO.CharOwner.CharInfo.Pos[0]).TileADStats.x);
        }
    }

    public void InteruptAttack(bool resetToIdle = true)
    {
        interruptToIdle = resetToIdle;
        isAttackInterupt = true;
        shotsLeftInAttack = 0;
        currentAttackPhase = AttackPhasesType.End;
        AttackSO.CharOwner.StopChargingPs(true);
    }

    public IEnumerator AttackCo(bool isFungus = false)
    {
        while (AttackSO.CharOwner.isMoving)
        {
            yield return null;
        }

        CurrentAttack.taPhase = TileAttackPhaseType.None;

             //Debug.Log("Start   " + CurrentAttack.AttackInput);

        yield return IdleToAttack(isFungus);

             //Debug.Log("End   " + CurrentAttack.AttackInput);


        if (CurrentAttack.AttackInput == AttackInputType.Weak || (!isFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets)))
        {
            yield break;
        }

        AttackSO.CharOwner.StopChargingPs(AttackSO.CharOwner.CastActivationPS == null);
        if (AttackSO.CharOwner.CastActivationPS != null)
        {
            AttackSO.CharOwner.CastActivationPS.transform.parent = null;
            AttackSO.CharOwner.CastActivationPS = null;
        }
        AttackSO.isStrongStop = false;
        AttackSO.isStrongLoading = false;
        AttackSO.isStrongChargingParticlesOn = false;
    }

    private IEnumerator IdleToAttack(bool isFungus = false)
    {
        yield return AttackSO.StartIdleToAtk(this);
        if (isAttackInterupt) yield break;

       // Debug.Log(currentAttackPhase + "     " + 0 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
        if (CurrentAttack.AttackInput >= AttackInputType.Strong && AttackSO.CharOwner.currentAttackProfile.CurrentAttackPhase(AttackInputType.Weak) != AttackPhasesType.End)
        {
            currentAttackPhase = AttackPhasesType.Charging;
        }
        else
        {
            AttackSO.CharOwner.SetAnimation(CurrentAttack.PrefixAnim + "_IdleToAtk");
            currentAttackPhase = currentAttackPhase == AttackPhasesType.End ? AttackPhasesType.Start : currentAttackPhase;
        }
       // Debug.Log(currentAttackPhase + "     " + 1 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
        while (currentAttackPhase < AttackPhasesType.Charging && shotsLeftInAttack > 0 &&
            (!isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)))
        {
            if (isAttackInterupt) yield break;
            yield return AttackSO.IdleToAtk(this);
        }

        yield return Charging(isFungus);
    }

    private IEnumerator Charging(bool isFungus = false, bool multipleAttack = false)
    {
        if (isAttackInterupt) yield break;
        //Debug.Log(currentAttackPhase + "     " + 2 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
        if (CurrentAttack.AttackInput >= AttackInputType.Strong && AttackSO.CharOwner.currentAttackProfile.CurrentAttackPhase(AttackInputType.Weak) != AttackPhasesType.End)
        {
        }
        else
        {
            AttackSO.CharOwner.SetAnimation(CurrentAttack.PrefixAnim + "_Charging", true);
        }
        yield return AttackSO.StartCharging(this);
        if (AttackSO.CharOwner.CastStartLoopPS == null && AttackSO.CharOwner.CastLoopPS == null && CurrentAttack.AttackInput == AttackInputType.Strong)
        {
          
        }
        while (currentAttackPhase <= AttackPhasesType.Charging && shotsLeftInAttack > 0 && CurrentAttack.ChargingTime > 0 &&
            (!isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)))
        {
            if (isAttackInterupt) yield break;
            yield return AttackSO.Charging(this);
        }
        if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
        {
            InteruptAttack();
        }

        yield return Loop(isFungus, multipleAttack);
    }

    public IEnumerator Loop(bool isFungus = false, bool multipleAttack = false)
    {
        //Debug.Log(currentAttackPhase + "     " + 3 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
        yield return AttackSO.StartLoop(this);
        //Debug.Log(currentAttackPhase + "     " + 4 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
        if (shotsLeftInAttack > 0 && (!isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)))
        {
            while ((!isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets))
                && shotsLeftInAttack > 0 && (CurrentAttack.AttackInput == AttackInputType.Weak))
            {
                currentAttackPhase = AttackPhasesType.Firing;

                if (AttackSO.CharOwner.SpineAnim.CurrentAnim.Contains("_Loop"))
                {
                    AttackSO.CharOwner.SpineAnim.skeletonAnimation.state.GetCurrent(0).TrackTime = 0;
                }
                else
                {
                    AttackSO.CharOwner.SetAnimation(CurrentAttack.PrefixAnim + "_Loop");
                }
                AttackSO.CharOwner.FireActionEvent(CharacterActionType.Weak);
                shotsLeftInAttack = shotsLeftInAttack == 2 ? 1 : 0;
                yield return null;
                while ((currentAttackPhase == AttackPhasesType.Firing || currentAttackPhase == AttackPhasesType.Shoot) &&
                    (CurrentAttack.AttackInput == AttackInputType.Weak) &&
                    (!isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)))
                {
                    //Debug.Log(currentAttackPhase + "     " + 7 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
                    if (isAttackInterupt) yield break;
                    yield return AttackSO.Loop(this);
                    if (isAttackInterupt) yield break;
                }

                if (CurrentAttack.AttackInput == AttackInputType.Weak && shotsLeftInAttack == 1)
                {
                    currentAttackPhase = AttackPhasesType.Charging;
                    yield return Charging(isFungus, true);
                    if (isAttackInterupt) yield break;
                }
            }
            if (isAttackInterupt) yield break;
            if (!multipleAttack)
            {
                if (isAttackInterupt) yield break;
                yield return AtkToIdle(isFungus);
            }

        }
        else if ((shotsLeftInAttack == 0) || (!isFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets)))
        {
            if (CurrentAttack.AttackInput >= AttackInputType.Strong && AttackSO.CharOwner.currentAttackProfile.CurrentAttackPhase(AttackInputType.Weak) != AttackPhasesType.End)
            {
            }
            else
            {
                InteruptAttack();
                if (interruptToIdle && AttackSO.CharOwner.currentAttackProfile.CurrentAttackPhase(AttackInputType.Strong) == AttackPhasesType.End &&
                    AttackSO.CharOwner.currentAttackProfile.CurrentAttackPhase(AttackInputType.Skill1) == AttackPhasesType.End && AttackSO.CharOwner.currentAttackProfile.CurrentAttackPhase(AttackInputType.Skill2) == AttackPhasesType.End)
                {
                    AttackSO.CharOwner.SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
                }
            }
        }
    }

    public IEnumerator AtkToIdle(bool isFungus = false)
    {
        if (isAttackInterupt) yield break;
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

        if ((!isFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || (isFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets)))
        {
            yield break;
        }

        while (CurrentAttack.AttackInput >= AttackInputType.Strong && AttackSO.CharOwner.isMoving)
        {
            //Debug.Log(currentAttackPhase + "     " + 5 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
            if (isAttackInterupt) yield break;
            yield return null;
        }
        //Debug.Log(currentAttackPhase + "     " + 6 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
        AttackSO.CharOwner.SetAnimation(CurrentAttack.PrefixAnim + "_AtkToIdle");
        while (currentAttackPhase != AttackPhasesType.End)
        {
           // Debug.Log(currentAttackPhase + "     " + 10 + "    " + shotsLeftInAttack + "    " + CurrentAttack.AttackInput);
            yield return AttackSO.AtkToIdle(this);
            if (isAttackInterupt)
            {
                yield break;
            }
            if (shotsLeftInAttack > 0 && CurrentAttack.AttackInput == AttackInputType.Weak)
            {
                yield return Loop(isFungus);
            }
        }
    }

}