using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class BattleManagerScript : MonoBehaviour
{
    public static BattleManagerScript Instance;

    [HideInInspector]public ManaInfoClass LeftMana = new ManaInfoClass();
    [HideInInspector]public ManaInfoClass RightMana = new ManaInfoClass();
    public int MaxMana;
    public float ManaTime;
    public int ManaCostMovement = 1;
    public int ManaCostSkill = 1;
    public UICrystalInfo LeftCInfo;
    public UICrystalInfo RightCInfo;


    public float BaseActionTime = 1;
    public float ReturnCharsTurn = 20;

    #region EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS___EVENTS
    //TIME EVENTS____TIME EVENTS____TIME EVENTS____TIME EVENTS____TIME EVENTS
    public delegate void CurrentBattleSpeedChanged(float currentBattleSpeed);
    public event CurrentBattleSpeedChanged CurrentBattleSpeedChangedEvent;

    public delegate void CurrentSwapTimerChanged(BaseCharacter cb, float timeLeft, ControllerType playerController);
    public event CurrentSwapTimerChanged CurrentSwapTimerChangedEvent;


    //BATTLE STATES EVENTS____BATTLE STATES EVENTS____BATTLE STATES EVENTS
    public delegate void CurrentBattleStateChanged(BattleState currentBattleState);
    public event CurrentBattleStateChanged CurrentBattleStateChangedEvent;

    public delegate void CurrentFungusStateChanged(FungusDialogType currentFungusState);
    public event CurrentFungusStateChanged CurrentFungusStateChangedEvent;

    public delegate void CurrentSwapTimerIsUpChanged(BaseCharacter cb);
    public event CurrentSwapTimerIsUpChanged CurrentSwapTimerIsUpChangedEvent;


    //GAME SEQUENCING EVENTS____GAME SEQUENCING EVENTS____GAME SEQUENCING EVENTS
    public delegate void MatchLost();
    public event MatchLost MatchLostEvent;
    public void Trigger_MatchLost() => MatchLostEvent?.Invoke();

    public delegate void MatchWon();
    public event MatchWon MatchWonEvent;
    public void Trigger_WonMatch() => MatchWonEvent?.Invoke();

    public delegate void CalledCheckPoint();
    public event CalledCheckPoint CalledCheckPointEvent;
    public void Trigger_CallCheckPointEvent() => CalledCheckPointEvent?.Invoke();


    //CHARACTER BASED EVENTS____CHARACTER BASED EVENTS____CHARACTER BASED EVENTS
    public delegate void CharacterDied(BaseCharacter cb);
    public event CharacterDied CharacterDiedEvent;

    public delegate void CharacterCreated(BaseCharacter cb);
    public event CharacterCreated CharacterCreatedEvent;

    public delegate void CharacterArrivedOnField(BaseCharacter cb);
    public event CharacterArrivedOnField CharacterArrivedOnFieldEvent;
    public void Trigger_CharacterArrivedOnFieldEvent(BaseCharacter cb) => CharacterArrivedOnFieldEvent?.Invoke(cb);

    public delegate void CharacterRecruited(BaseCharacter cb);
    public event CharacterRecruited CharacterRecruitedEvent;


    //LOADOUT BASED EVENTS____LOADOUT BASED EVENTS____LOADOUT BASED EVENTS
    public delegate void MasksChangedDuringBattle();
    public event MasksChangedDuringBattle MasksChangedDuringBattleEvent;
    public void Trigger_MasksChanged() => MasksChangedDuringBattleEvent?.Invoke();
	#endregion

	public Dictionary<ControllerType, BaseCharacter> CurrentSelectedCharacters = new Dictionary<ControllerType, BaseCharacter>();

    //????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
    public bool TesterOn = false;
    [HideInInspector] public ActiveSkillClass isSkillHappening = new ActiveSkillClass(ControllerType.Player1, false);
    //????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????


    [Header("BASE EFFECTS"), Space(10)]
    public ScriptableObjectAttackEffect BaseEffect_Teleport;
    public ScriptableObjectAttackEffect BaseEffect_Stun;
    public ScriptableObjectAttackEffect BaseEffect_InvulnerabilityOnDeath;


    [Header(" - BULLET MUTATORS - ")]
    [Tooltip("Set the weak attack hit particle mode")] public HitParticlesType HitParticlesT;
    [ConditionalField("HitParticlesT", false, HitParticlesType.Resized)] public float HitResizeMultiplier = 1f;


    [Header("CHARACTER MANAGEMENT CONFIG"), Space(10)]
    public GameObject CharacterBasePrefab;
    [SerializeField] public Transform CharactersContainer;


    [Header("DATA PROFILES"), Space(10)]
    public ScriptableObjectAllElementData elementData = null;
    public ScriptableObjectAllClassData classData = null;
    public ScriptableObjectContainingAllCharsSO CharactersData = null;
    public ScriptableObjectContainingAllCharsSOAndPrefabs NonAddressableCharactersData = null;
    public List<ScriptableObjectCharacterPrefab> ListOfScriptableObjectCharacterPrefab => CharactersData == null ? new List<ScriptableObjectCharacterPrefab>() : CharactersData.CharacterSo.ToList();


    //GAME STATE______GAME STATE______GAME STATE______GAME STATE______GAME STATE______GAME STATE______GAME STATE______GAME STATE______GAME STATE______GAME STATE
    public bool ShowUIBattle = false;

	public BattleState CurrentBattleState = BattleState.Battle;
	public float BattleSpeed = 1;


    //GAME SPEED STATE______GAME SPEED STATE______GAME SPEED STATE______GAME SPEED STATE______GAME SPEED STATE______GAME SPEED STATE______GAME SPEED STATE

    public float FixedDeltaTime => Time.fixedDeltaTime;
    public float DeltaTime => Time.deltaTime;
    //____________________________________________________________________________________________________________________________________________________________________________


    //CHARACTER HANDLING_____CHARACTER HANDLING_____CHARACTER HANDLING_____CHARACTER HANDLING_____CHARACTER HANDLING_____CHARACTER HANDLING_____CHARACTER HANDLING_____CHARACTER HANDLING
    [Header(" - CHARACTER HANDLING - ")]
    public List<BaseCharacter> AllCharacters = new List<BaseCharacter>();
  
    public IDamageReceiver[] AllDamageReceivers
    {
        get
        {
            return AllCharacters.ToArray();
        }
    }

    /// <summary>
    /// Character Prefabs that have been preloaded through the addressables
    /// </summary>
    protected Dictionary<CharacterNameType, GameObject> CharacterPrefabs = new Dictionary<CharacterNameType, GameObject>();
    public GameObject GetCharacterPrefab(CharacterNameType characterName)
    {
        if (CharacterPrefabs.Count == 0 && NonAddressableCharactersData != null)
        {
            foreach (CharacterSOPrefabGroup group in NonAddressableCharactersData.SOPrefabGroups)
            {
                CharacterPrefabs.Add(group.ScriptableObject.AbridgedCharInfo.CharacterID, group.Prefab);
            }
        }
        //return CharactersData.CharacterSo.Where(r => r.AbridgedCharInfo != null && r.AbridgedCharInfo.CharacterID == characterName).FirstOrDefault().CharacterPrefab.GetComponentInChildren<CharacterInfoScript>();
        if (!CharacterPrefabs.ContainsKey(characterName))
        {
            //GET THE PREFAB HERE USING ASYNC or more likely RETURN AN ERROR
            //CharacterPrefabs.Add(characterName, null);
            //Debug.LogError("ERROR GETTING CHARACTER FROM LOADED PREFABS: " + characterName.ToString() + "\nTHEY NEED TO BE ADDED TO THE ADDRESSABLE CHARACTERS FOR THIS STAGE");
            return null;
        }
        return CharacterPrefabs[characterName] == null ? null : CharacterPrefabs[characterName];
    }
    public void AddCharacterPrefab(CharacterNameType characterName, GameObject prefab)
    {
        if (CharacterPrefabs.ContainsKey(characterName))
            return;
        CharacterPrefabs.Add(characterName, prefab);
    }
    //____________________________________________________________________________________________________________________________________________________________________________


    //TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING____TEAM HANDLING
    [Header(" - TEAM HANDLING - ")]
    [Tooltip("This is overwritten by the Scene Load Manager setting of the same name")] public List<Color> _PlayersColor = new List<Color>(4);
    public List<Color> PlayersColor => SceneLoadManager.Instance != null ? SceneLoadManager.Instance.playersColor : _PlayersColor;
    public List<Sprite> playersNumberBig = new List<Sprite>();
    public List<Sprite> playersNumberSmall = new List<Sprite>();
    public Dictionary<TeamSideType, TeamCharacterInfoClass> TeamInfo = new Dictionary<TeamSideType, TeamCharacterInfoClass>()
    {
        { TeamSideType.LeftSideTeam, new TeamCharacterInfoClass(TeamSideType.LeftSideTeam) },
        { TeamSideType.RightSideTeam, new TeamCharacterInfoClass(TeamSideType.RightSideTeam) },
    };
    private List<TeamSideInformationClass> PlayerBattleInfo = new List<TeamSideInformationClass>();
    private SwapInfoClass temp_SIC;
    //____________________________________________________________________________________________________________________________________________________________________________


    //OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER____OTHER
    private BaseCharacter tempCB;
    private bool temp_Bool = false;
    private Vector2Int temp_V2;
    private List<ScriptableObjectPassiveSkill> temp_PassiveSkills;
    //____________________________________________________________________________________________________________________________________________________________________________

  
    //OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION____OPTIMISATION
    public List<SpriteAtlasInfoClass> SpriteAtlasses = new List<SpriteAtlasInfoClass>();
    //____________________________________________________________________________________________________________________________________________________________________________

    #region Unity Life Cycle
    private void Awake()
    {
        Instance = this;
    }

    public Vector2Int NextPosTest = new Vector2Int();
    public CharacterNameType cnameee;
    public ControllerType pcontroller;
	private void Update()
    {
        if(Input.GetKeyUp( KeyCode.A))
        {
            SetCharOnBoardOnFixedPos(pcontroller, AllCharacters.Where(r=> r.CharInfo.CharacterID == cnameee).FirstOrDefault(), NextPosTest);
        }
    }

    private void Start()
    {
        StartCoroutine(InstanciateAllChar());
    }


    #endregion


    #region SetCharacterOnBoard


    BattleTileScript temp_Bts = null;
    //Used to set the already created char on a random Position in the battlefield
    public BaseCharacter SetCharOnBoardOnRandomPos(ControllerType playerController, BaseCharacter cb, CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving, bool loop = false, bool isPlayer = true, bool selectChar = false)
    {
        temp_Bts = GridManagerScript.Instance.GetFreeBattleTile(cb.CharInfo.WalkingSide, cb.CharInfo.Pos);

        if (temp_Bts != null)
        {
            return SetCharOnBoard(playerController, cb, temp_Bts.Pos, anim, loop, isPlayer, fireArrivingPs_Audio: anim == CharacterAnimationStateType.Arriving);
        }
        return null;
    }

    public BaseCharacter SetCharOnBoardOnFixedPos(ControllerType playerController, BaseCharacter currentCharacter, Vector2Int pos, CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving, bool isOccupying = true)
    {
        return SetCharOnBoard(playerController, currentCharacter, pos, anim, isOccupying: isOccupying, fireArrivingPs_Audio: anim == CharacterAnimationStateType.Arriving);
    }

    public BaseCharacter SetCharOnBoard(ControllerType playerController, BaseCharacter currentCharacter, Vector2Int pos, CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving,
        bool loop = false, bool isPlayer = true, bool isOccupying = true, bool selectChar = false, bool fireArrivingPs_Audio = true)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
        currentCharacter.CharInfo.CurrentTilePos = bts.Pos;

        for (int i = 0; i < currentCharacter.CharInfo.Pos.Count; i++)
        {
            if (isOccupying)
            {
                //Debug.LogError(currentCharacter.CharInfo.CharacterID + "    occuping in pos" + currentCharacter.CharInfo.Pos[i] + "   " + Time.time);
                GridManagerScript.Instance.SetBattleTileState(currentCharacter.CharInfo.Pos[i], BattleTileStateType.Occupied);
            }
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.CharInfo.Pos[i]);
        }

    
        currentCharacter.gameObject.SetActive(true);
        currentCharacter.SetUpEnteringOnBattle(anim, loop, fireArrivingPs_Audio);
        StartCoroutine(MoveCharToBoardWithDelay(0.1f, currentCharacter, bts.transform.position));
        if (selectChar)
        {
            SelectCharacter(playerController, currentCharacter);
        }

        return currentCharacter;
    }



    public IEnumerator RemoveCharacterFromBaord(BaseCharacter currentCharacter, bool leaveEmpty, bool withAudio = true)
    {
        if (leaveEmpty)
        {
            for (int i = 0; i < currentCharacter.CharInfo.Pos.Count; i++)
            {
                if (GridManagerScript.Instance.IsPosOnField(currentCharacter.CharInfo.Pos[i]))
                {
                    GridManagerScript.Instance.SetBattleTileState(currentCharacter.CharInfo.Pos[i], BattleTileStateType.Empty);
                }
            }
        }
        currentCharacter.SetUpLeavingBattle(withAudio);
        yield return null;
    }
    public IEnumerator MoveCharToBoardWithDelay(float delay, BaseCharacter cb, Vector3 nextPos)
    {
        if (delay > 0)
        {
            yield return WaitFor(delay, () => CurrentBattleState == BattleState.Pause);
        }
        cb.transform.position = nextPos;
    }

    public IEnumerator MoveCharToBoardWithDelay(float delay, BaseCharacter cb, Vector3 nextPos, System.Func<bool> pauseCondition)
    {
        if (delay > 0)
        {
            yield return WaitFor(delay, () => CurrentBattleState == BattleState.Pause || pauseCondition());
        }
        cb.transform.position = nextPos;
    }

    #endregion


    #region Create Character

    /// <summary>
    /// Given a list of prefabs, returns the characters that need to be loaded from the addressables based on assigned attacks and masks, then adds the prefabs to the list of character prefabs
    /// </summary>
    CharacterNameType[] ValidateAndAddPrefabs(ref List<GameObject> preloadedPrefabs)
    {
        CharacterInfoScript temp_CharInfo = null;
        List<CharacterNameType> CharactersToLoad = new List<CharacterNameType>();
        foreach (GameObject prefab in preloadedPrefabs)
        {
            Debug.Log(prefab.name);
            temp_CharInfo = prefab.GetComponentInChildren<CharacterInfoScript>();

            if (temp_CharInfo == null || CharacterPrefabs.ContainsKey(temp_CharInfo.CharacterID))
                continue;

            AddCharacterPrefab(temp_CharInfo.CharacterID, prefab);

            foreach (ScriptableObjectAttackBase tileAtk in temp_CharInfo.CurrentAttackTypeInfo.Where(r => r != null && r.TilesAtk.BulletTrajectories.Where(f => f.BulletEffectTiles.Where(g => g.SpawnSummonOnTile).FirstOrDefault() != null).FirstOrDefault() != null).ToArray())
            {
                foreach (BulletBehaviourInfoClassOnBattleFieldClass bulletAtk in tileAtk.TilesAtk.BulletTrajectories.Where(f => f.BulletEffectTiles.Where(g => g.SpawnSummonOnTile).FirstOrDefault() != null).ToArray())
                {
                    foreach (BattleFieldAttackTileClass tileEffect in bulletAtk.BulletEffectTiles.Where(g => g.SpawnSummonOnTile).ToArray())
                    {
                        if (!CharactersToLoad.Contains(tileEffect.SummonToSpawn.CharToSummon))
                            CharactersToLoad.Add(tileEffect.SummonToSpawn.CharToSummon);
                    }
                }
            }

            //ADD MASK CHARS TO THE LIST OF CHARACTERS TO SPAWN
            //NOT BEING USED: MASK INFO IS STORED IN THE LOAD INFO OR BATTLE INFO MANAGER
            //if (temp_CharInfo.MaskInfo.Mask != null && !CharactersToLoad.Contains(temp_CharInfo.MaskInfo.Mask.BossToSpawn))
            //    CharactersToLoad.Add(temp_CharInfo.MaskInfo.Mask.BossToSpawn);
        }
        preloadedPrefabs.Clear();
        return CharactersToLoad.ToArray();
    }

    public IEnumerator InstanciateAllChar(float LoadStepDuration = 0f, string StageID = "")
    {
        //PRELOAD ALL POSSIBLE CHARACTERS HERE FROM ADDRESSABLES=========================================================================================================
        //LOAD all the designer set characters
        AsyncOperationHandle<IList<GameObject>> AllStagesCharsOperation = Addressables.LoadAssetsAsync<GameObject>("Stage_ALL", null);
        while (!AllStagesCharsOperation.IsDone)
        {
            yield return null;
        }
        List<GameObject> UnValidatedPreloadedPrefabs = AllStagesCharsOperation.Result.ToList();
        ScriptableObjectCharacterPrefab temp_CharSo;
        List<AsyncOperationHandle<GameObject>> PlayerTeamCharOperations = new List<AsyncOperationHandle<GameObject>>();
        foreach (TeamSideInformationClass charName in BattleInfoManagerScript.Instance.PlayerBattleInfo)
        {
            temp_CharSo = CharactersData.CharacterSo.Where(r => r.AbridgedCharInfo.CharacterID == charName.CharacterName).FirstOrDefault();
            PlayerTeamCharOperations.Add(Addressables.LoadAssetAsync<GameObject>(temp_CharSo.AbridgedCharInfo.AddressableLocation));
        }

        while (PlayerTeamCharOperations.Where(r => r.IsDone).Count() != PlayerTeamCharOperations.Count)
        {
            yield return null;
        }
        foreach (AsyncOperationHandle<GameObject> playerTeamCharOp in PlayerTeamCharOperations)
        {
            UnValidatedPreloadedPrefabs.Add(playerTeamCharOp.Result);
        }



        //GET ALL OF THE DEPENDANCIES FOR EACH CHARACTER BASED ON THEIR ATTACKS AND MASK AND CREATE THOSE TOO
        CharacterNameType[] RequiredCharacters = new CharacterNameType[0];
        while (UnValidatedPreloadedPrefabs.Count != 0)
        {
            RequiredCharacters = ValidateAndAddPrefabs(ref UnValidatedPreloadedPrefabs);
            UnValidatedPreloadedPrefabs.Clear();

            List<AsyncOperationHandle<GameObject>> characterLoadProcesses = new List<AsyncOperationHandle<GameObject>>();
            foreach (CharacterNameType character in RequiredCharacters)
            {
                if (character != CharacterNameType.None)
                {
                    temp_CharSo = CharactersData.CharacterSo.Where(r => r.AbridgedCharInfo.CharacterID == character).FirstOrDefault();
                    if (temp_CharSo == null)
                    {
                        Debug.LogError("Could not preload character " + character.ToString().ToUpper() + " because they do not exist in the CHARACTER list or their Scriptable Object isn't configured correctly... Skipping...");
                        continue;
                    }
                    characterLoadProcesses.Add(Addressables.LoadAssetAsync<GameObject>(temp_CharSo.AbridgedCharInfo.AddressableLocation));
                }
            }

            while (characterLoadProcesses.Where(r => r.IsDone).Count() != characterLoadProcesses.Count)
            {
                yield return null;
            }

            foreach (AsyncOperationHandle<GameObject> loadProcess in characterLoadProcesses)
            {
                UnValidatedPreloadedPrefabs.Add(loadProcess.Result);
            }
        }
        //============================================================================================================================================================================
        PlayerBattleInfo = BattleInfoManagerScript.Instance.PlayerBattleInfo;

        AddressableCharacterInstancer CharCreator = null;
        foreach (TeamSideInformationClass item in PlayerBattleInfo)
        {
            CharCreator = new AddressableCharacterInstancer(this, item, CharactersContainer, item.Team);
            while (CharCreator.IsWorking)
                yield return null;
            BaseCharacter playableCharOnScene = CharCreator.Result;
            ControllerType[] tempCo = new ControllerType[item.PlayerController.Count];

            item.PlayerController.CopyTo(tempCo);

            TeamInfo[item.Team].PlayerController = tempCo.ToList();
            playableCharOnScene.CharID = playableCharOnScene.CharInfo.Name;

            //Wait for the load step (prevents stutter during loading)
            yield return new WaitForSecondsRealtime(LoadStepDuration / 10f); //WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT___WAIT
        }
        CharacterCreatedEvent?.Invoke(null);
        tempCB = AllCharacters.Where(r => r.CharInfo.CharacterID == CharacterNameType.CrystalLeft).FirstOrDefault();
        tempCB.UMS.EnableBattleBars(false);
        SetCharOnBoardOnFixedPos(ControllerType.Player1, tempCB, new Vector2Int(0, 0), CharacterAnimationStateType.Idle);
        LeftCInfo.Charinfo = tempCB.CharInfo;

        tempCB = AllCharacters.Where(r => r.CharInfo.CharacterID == CharacterNameType.CrystalRight).FirstOrDefault();
        tempCB.UMS.EnableBattleBars(false);
        SetCharOnBoardOnFixedPos(ControllerType.Player2, tempCB, new Vector2Int(0, 11), CharacterAnimationStateType.Idle);
        RightCInfo.Charinfo = tempCB.CharInfo;


        StartCoroutine(ManaCoroutine(LeftMana));
        StartCoroutine(ManaCoroutine(RightMana));

        yield return null;
    }

    IEnumerator ManaCoroutine(ManaInfoClass mana)
    {
        while (true)
        {
            yield return null;
            mana.CurrentMana += (1 / ManaTime) * Time.fixedDeltaTime;
            if (mana.CurrentMana > MaxMana)
            {
                mana.CurrentMana = MaxMana;
            }
        }
    }

    public void PreLoadAttackParticles(ScriptableObjectAttackBase atk, CharacterNameType characterId)
    {

        if (atk == null || atk.Particles == null || atk.Particles.Right == null)
        {

        }

        if (atk.Particles.Right.CastAddress == "ShieldPotion_Hit" || atk.Particles.Right.BulletAddress == "ShieldPotion_Hit" || atk.Particles.Right.HitAddress == "ShieldPotion_Hit")
        {
            Debug.LogError("Check attack:  " + atk.name);
        }

        ParticleManagerScript.Instance.AddBaseAttackParticles(new AttackParticleInfoClass(atk.Particles.Right.CastAddress, null, characterId, AttackParticlePhaseTypes.Cast, atk.ParticlesInput));
        ParticleManagerScript.Instance.AddBaseAttackParticles(new AttackParticleInfoClass(atk.Particles.Right.BulletAddress, null, characterId, AttackParticlePhaseTypes.Bullet, atk.ParticlesInput));
        ParticleManagerScript.Instance.AddBaseAttackParticles(new AttackParticleInfoClass(atk.Particles.Right.HitAddress, null, characterId, AttackParticlePhaseTypes.Hit, atk.ParticlesInput));
    }



    int currentCreatedCharIndex = 0;
    /// <summary>
    /// Creation of the character with the basic info
    /// </summary>
    public BaseCharacter CreateChar(TeamSideInformationClass teamInfoC, Transform parent, TeamSideType teamSide, bool addToList = true, bool showHP = true, bool showEther = true, bool isBornOfWave = false)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = null;

        for (int i = 0; i < ListOfScriptableObjectCharacterPrefab.Count; i++)
        {
            if (ListOfScriptableObjectCharacterPrefab[i] != null)
            {

                if (ListOfScriptableObjectCharacterPrefab[i].AbridgedCharInfo.CharacterID == teamInfoC.CharacterName)
                {
                    soCharacterPrefab = ListOfScriptableObjectCharacterPrefab[i];
                    break;
                }
            }
            else
            {
                Debug.LogError("one of the scrptableObject is null check in the battlemanger and delete the element that is null");
            }
        }
        if (soCharacterPrefab == null)
        {
            Debug.LogError("scrptableObject of this char is missing ----" + teamInfoC.CharacterName);
            return null;
        }

        // Debug.LogError(charInfo.CharacterName);
        soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.AbridgedCharInfo.CharacterID == teamInfoC.CharacterName).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab, new Vector3(100, 100, 100), Quaternion.identity, parent);
        GameObject child = Instantiate(CharacterPrefabs[teamInfoC.CharacterName], characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        BaseCharacter currentCharacter = (BaseCharacter)characterBasePrefab.AddComponent(System.Type.GetType(child.GetComponentInChildren<CharacterInfoScript>().BaseChar.ToString()));
        if (addToList)
        {
            AllCharacters.Add(currentCharacter);
        }

        currentCharacter.UMS = currentCharacter.GetComponent<UnitManagementScript>();
        currentCharacter.UMS.CharOwner = currentCharacter;
        CharacterActionType[] copy = new CharacterActionType[teamInfoC.CharActionlist.Count];
        teamInfoC.CharActionlist.CopyTo(copy);
        currentCharacter.CharActionlist = copy.ToList();
        CharacterActionType[] B_copy = new CharacterActionType[teamInfoC.CharActionlist.Count];
        teamInfoC.CharActionlist.CopyTo(B_copy);
        currentCharacter.B_CharActionlist = B_copy.ToList();
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.CharInfo.OccupiedTiles.Add(item);
        }

      

        currentCharacter.CharInfo.PlayerController = teamInfoC.PlayerController;
        currentCharacter.CharInfo.Side = teamInfoC.Team;
        currentCharacter.CharInfo.Facing = teamInfoC.DefaultFacing;
        currentCharacter.UpdateSwappableSO(currentCharacter.CharInfo.Behaviour.MovementActionN);


        //Setting defaults, there are overriden if stated otherwise:
        currentCharacter.CharInfo.IsBornOfWave = isBornOfWave;

        currentCharacter.SetupCharacterSide(teamSide, showHP: showHP, showEther: showEther);

        currentCharacter.CharInfo.BaseColorHueSat = currentCharacter.SpineAnim.GetColorHueSat();
        currentCharacter.GetComponentInChildren<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        currentCharacter.CharInfo.NPCType = WaveNPCTypes.Recruitable;

        currentCharacter.CurrentCharIsDeadEvent += CurrentCharacter_CurrentCharIsDeadEvent;
        currentCharacter.CharBoxCollider = currentCharacter.GetComponentInChildren<BoxCollider>(true);
        if (currentCharacter.CharBoxCollider != null) currentCharacter.CharBoxCollider.enabled = false;
        UIBattleFieldManager.Instance.SetupCharListener(currentCharacter, showEther);

        currentCharacter.GetComponentInChildren<ParticleCharacterSpineCharAttacher>()?.Initialise(currentCharacter);
        currentCharacter.CharInfo.CharFormeInfo?.Initialize();


        currentCharacter.CharInfo.CharInstanceID = currentCreatedCharIndex;
        currentCreatedCharIndex++;

        PreLoadAttacksPs(currentCharacter);

        return currentCharacter;
    }


    public void PreLoadAttacksPs(BaseCharacter cb)
    {
        for (int a = 0; a < cb.CharInfo.CurrentAttackTypeInfo.Count; a++)
        {
            PreLoadAttackParticles(cb.CharInfo.CurrentAttackTypeInfo[a], cb.CharInfo.CharacterID);
        }

    }

    public void CallMatchLostEvent()
    {
        CurrentBattleState = BattleState.WinLose;
        MatchLostEvent?.Invoke();
    }

    private void CurrentCharacter_CurrentCharIsDeadEvent(BaseCharacter cb)
    {
        if(cb.CharInfo.CharacterID == CharacterNameType.CrystalRight || cb.CharInfo.CharacterID == CharacterNameType.CrystalLeft)
        {
            CallMatchLostEvent();
        }
    }

    #endregion


    #region Loading_Selection Character
 
 

    public List<BaseCharacter> zombiesList = new List<BaseCharacter>();
    public void Zombification(BaseCharacter zombie, float duration)
    {
        if (zombie.ZombificationCo != null) StopCoroutine(zombie.ZombificationCo);
        zombie.ZombificationCo = Zombification_Co(zombie, ParticlesType.None, duration);
        StartCoroutine(zombie.ZombificationCo);
    }

    private IEnumerator Zombification_Co(BaseCharacter character, ParticlesType ps, float duration)
    {
        //Debug.LogError("Zombie");

        if (character.died || character.CharInfo.HealthPerc <= 0)
        {
            yield break;
        }

        character.ResetBaseChar(false, false);
        character.zombiePs = ParticleManagerScript.Instance.GetParticle(ps);

        if (character.zombiePs != null)
        {
            character.zombiePs.SetActive(true);
            character.zombiePs.transform.parent = character.SpineAnim.transform;
            character.zombiePs.transform.localPosition = Vector3.zero;
            character.zombiePs.transform.localRotation = Quaternion.Euler(character.CharInfo.Side == TeamSideType.LeftSideTeam ? Vector3.zero : character.zombiePs.transform.eulerAngles);
        }

        character.BuffsDebuffsList.ForEach(r =>
        {
            if (r.Stat != BuffDebuffStatsType.Zombie)
            {
                r.Duration = 0;
                r.Stop_Co = true;
            }
        }
        );

        if (character.died || character.CharInfo.HealthPerc <= 0)
        {
            yield break;
        }


        yield return RemoveCharacterFromBaord(character, true);

        character.CharActionlist.Remove(CharacterActionType.SwitchCharacter);
        if (character.zombiePs != null)
        {
            character.zombiePs.transform.parent = null;
            character.zombiePs.SetActive(false);
        }

        while (character.IsOnField)
        {
            yield return null;
        }

        if (character.died || character.CharInfo.HealthPerc <= 0)
        {
            yield break;
        }

        InputBehaviourType input = character.CharInfo.Behaviour.InputBehaviour;
        DeathBehaviourType death = character.CharInfo.Behaviour.DeathBehaviour;


        ChangeSide(character, InputBehaviourType.AIInput, MovementActionType.None, death == DeathBehaviourType.None ? character.CharInfo.Behaviour.DeathBehaviour :
            !character.SpineAnim.HasAnimation("Reverse_Arriving") ? DeathBehaviourType.Explosion : death, false);
        if (character.DisableCharCo != null)
        {
            character.DisableCharCo = null;
        }

        //charming TODO
       // yield return WaveManagerScript.Instance.SetCharInPos(character, Vector2Int.zero, true);

        while (character.BuffsDebuffsList.Where(r => r.Stat == BuffDebuffStatsType.Zombie).ToList().Count > 0 && character.CharInfo.HealthPerc > 0 && (CurrentBattleState == BattleState.Battle || CurrentBattleState == BattleState.Pause))
        {
            yield return null;
        }

            while (character.isMoving)
            {
                yield return null;
            }
            if (character.zombiePs != null)
            {
                character.zombiePs.transform.parent = null;
                character.zombiePs.SetActive(false);
                character.zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter01_TohoraSea_Boss_MoonDrums_LoopCrumble);
                character.zombiePs.SetActive(true);
                character.zombiePs.transform.parent = character.SpineAnim.transform;
                character.zombiePs.transform.localPosition = Vector3.zero;
            }
            character.currentAttackProfile.InteruptAttack();

            if (character.CharInfo.HealthPerc > 0)
            {
                yield return RemoveCharacterFromBaord(character, true);

                for (int i = 0; i < character.CharInfo.Pos.Count; i++)
                {
                    GridManagerScript.Instance.SetBattleTileState(character.CharInfo.Pos[i], BattleTileStateType.Empty);
                }

                character.ResetBuffDebuff();
            }

            yield return WaitFor(1f, () => CurrentBattleState == BattleState.Pause);

            if (character.DisableCharCo != null)
            {
                character.DisableCharCo = null;
            }

        ChangeSide(character, input, MovementActionType.None, death == DeathBehaviourType.None ? character.CharInfo.Behaviour.DeathBehaviour : death, false);


        if (input == InputBehaviourType.PlayerInput && !TeamInfo[character.CharInfo.Side].PlayerController.GridFight_ContainsStruct(ControllerType.Enemy))
        {
            if (character.CharInfo.HealthPerc <= 0)
            {
                if (character.RespawnSequencerCo != null) StopCoroutine(character.RespawnSequencerCo);
                character.RespawnSequencerCo = character.ReviveSequencer();
                StartCoroutine(character.RespawnSequencerCo);
            }


            /* if (CurrentSelectedCharacters.Where(r => r.Value.Character == null).ToList().Count > 0)
                {
                    character.CurrentPlayerController = CurrentSelectedCharacters.Where(r => r.Value.Character == null).OrderBy(a => a.Value.NotPlayingTimer).First().Key;
                    SetCharOnBoardOnFixedPos(character.CurrentPlayerController, character, GridManagerScript.Instance.GetFreeBattleTile(character.CharInfo.WalkingSide).Pos);
                    character.currentInputProfile.SetCharSelected(true, character.CurrentPlayerController);
                    SelectCharacter(character.CurrentPlayerController, character);
                }*/
            character.UpdateVitalities();
            character.gameObject.SetActive(true);
            character.CharActionlist.Add(CharacterActionType.SwitchCharacter);

        }
        else
        {
            if (character.CharInfo.HealthPerc > 0)
            {
                character.gameObject.SetActive(true);
                //charming todo
               // yield return WaveManagerScript.Instance.SetCharInPos(character, Vector2Int.zero, true);
            }
        }

    }

    public void ChangeSide(BaseCharacter character, InputBehaviourType input, MovementActionType move, DeathBehaviourType death, bool restoreChar = true)
    {
        character.CharInfo.Side = character.CharInfo.Side == TeamSideType.LeftSideTeam ? TeamSideType.RightSideTeam : TeamSideType.LeftSideTeam;
        character.SpineAnim.gameObject.tag = character.CharInfo.Side.ToString();
        character.CharInfo.WalkingSide = character.CharInfo.Side == TeamSideType.RightSideTeam ? WalkingSideType.RightSide : WalkingSideType.LeftSide;
        character.CharInfo.Facing = character.CharInfo.Side == TeamSideType.LeftSideTeam ? FacingType.Right : FacingType.Left;
        character.UpdateSwappableSO(move);
        character.SetupCharacterSide(character.CharInfo.Side, restoreChar);
    }

    public void UpdateSwappables(BaseCharacter character, TeamSideType side, InputBehaviourType input, MovementActionType move, DeathBehaviourType death, WalkingSideType walk, FacingType facing, bool restoreChar = true)
    {
        character.CharInfo.Side = side;
        character.SpineAnim.gameObject.tag = character.CharInfo.Side.ToString();
        character.CharInfo.WalkingSide = walk;
        character.CharInfo.PlayerController = TeamInfo[side].GetPlayerController;
        character.CharInfo.Facing = facing;
        character.UpdateSwappableSO(move);
        character.SetupCharacterSide(character.CharInfo.Side, restoreChar);
    }


    public void CloneUnit(BaseCharacter original, ParticlesType ps, int amount, float strengthScale = 1f, ScriptableObjectAttackEffect cloneStartingEffect = null, bool isInClosePos = true)
    {
        if (original.CharInfo.Health <= 0) return;
        StartCoroutine(CloneUnit_Co(original, ps, amount, strengthScale, cloneStartingEffect, isInClosePos));
    }



    IEnumerator CloneUnit_Co(BaseCharacter original, ParticlesType ps, int amount, float strengthScale = 1f, ScriptableObjectAttackEffect cloneStartingEffect = null, bool isInClosePos = true)
    {
        List<BaseCharacter> ClonesList = new List<BaseCharacter>();

        BattleTileScript bts;
        CharacterInfoScript cloneInfo = original.CharInfo;
        BaseCharacter clone;
        for (int i = 0; i < amount; i++)
        {
            bts = isInClosePos ? GridManagerScript.Instance.GetRandomFreeAdjacentTile(original.CharInfo.CurrentTilePos, 1, false, original.CharInfo.WalkingSide) : GridManagerScript.Instance.GetFreeBattleTile(original.CharInfo.WalkingSide);
            if (bts == null)
            {
                break;
            }
            else
            {
                int processTrackingID = Random.Range(1, 999999999);
                yield return ItemSpawnerManagerScript.Instance.SpawnSummon(cloneInfo.CharacterID, bts, original, 0, trackingID: processTrackingID);
                clone = ItemSpawnerManagerScript.Instance.TrackedSummonOutputs[processTrackingID];
                ItemSpawnerManagerScript.Instance.TrackedSummonOutputs.Remove(processTrackingID);
                ClonesList.Add(clone);
                cloneInfo.DeathEvent += clone._CharInfo_DeathEvent;
               
                clone.CharInfo.SetupChar(strengthScaler: strengthScale);

                if (cloneStartingEffect != null)
                {
                    clone.Buff_DebuffCo(original, cloneStartingEffect, null);
                }

                clone.CharInfo.BaseColorHueSat = clone.SpineAnim.GetColorHueSat();
            }
        }

        while (original.HasBuffDebuff(BuffDebuffStatsType.Legion))
        {
            yield return null;
        }

        foreach (BaseCharacter item in ClonesList.Where(r => r.CharInfo.CharacterID == original.CharInfo.CharacterID && r.isActiveAndEnabled && r.IsOnField))
        {
            item.CharInfo.Health = -5;
        }

    }

    public void LoadingNewCharacterToGrid(CharacterNameType cName, TeamSideType side, ControllerType playerController, bool worksOnFungusPappets, bool isCharDead)
    {
		LoadingNewCharacterToGrid(CurrentSelectedCharacters[playerController], side, playerController, worksOnFungusPappets, isCharDead);
    }

    public void LoadingNewCharacterToGrid(BaseCharacter currentCharacter, TeamSideType side, ControllerType playerController, bool worksOnFungusPappets, bool isCharDead)
    {
        //Debug.LogError(currentCharacter.CharInfo.CharacterID + "    Selected   " + playerController + "    " + Time.time);
        SelectionDeselectinoCharacter(true, currentCharacter, playerController);
		CharacterLoadingIn(currentCharacter, playerController, isCharDead);
    }


    public void RemoveNamedCharacterFromBoard(CharacterNameType charToRemoveName, bool swapIfSelected = true, bool isOnField = false, bool withAudio = true, string charId = "")
    {
        BaseCharacter charToRemove = AllCharacters.Where(r => r.CharInfo.CharacterID == charToRemoveName && r.IsOnField && !r.CharInfo.IsSummon && (string.IsNullOrEmpty(charId) ? true : r.CharID == charId)).FirstOrDefault();
     
        charToRemove.SpineAnim.SetAnimationSpeed(2);
        StartCoroutine(RemoveCharacterFromBaord(charToRemove, true, withAudio));
    }

    public void DeselectCharacter(CharacterNameType charToDeselectName, TeamSideType side, ControllerType playerController)
    {
        if (CurrentBattleState != BattleState.Battle && CurrentBattleState != BattleState.Intro && CurrentBattleState != BattleState.FungusPuppets)
        {
            return;
        }

        BaseCharacter charToDeselect = AllCharacters.Where(r => r.CharInfo.CharacterID == charToDeselectName && r.CharInfo.Side == side).FirstOrDefault();

        if (charToDeselect != null)
        {
            SelectionDeselectinoCharacter(false, charToDeselect, playerController);
        }
    }

    public void SelectionDeselectinoCharacter(bool isSelected, BaseCharacter charToDeselect, ControllerType playerController)
    {
        if (charToDeselect == null) return;
        charToDeselect.Fire_CharacterSelectedEvent(isSelected, playerController);
    }

    //Used to select a char 
    public void SelectCharacter(ControllerType playerController, BaseCharacter currentCharacter)
    {
        if (currentCharacter != null && currentCharacter.CharInfo.HealthPerc > 0)
        {
        }
    }

    //Load char in a random pos
	private void CharacterLoadingIn(BaseCharacter cb, ControllerType playerController, bool isCharDead)
    {
        cb = SetCharOnBoardOnRandomPos(playerController, cb);
        if (cb != null)
        {
            SelectCharacter(playerController, cb);
            if (isCharDead)
            {
                cb.Buff_DebuffCo(cb, BaseEffect_InvulnerabilityOnDeath, null, pass: true);
            }
        }
    }
 
    #endregion


    #region Move Character

    //Move selected char under determinated player
    public void MoveSelectedCharacterInDirection(ControllerType playerController, InputDirectionType dir, float value)
    {
		StartCoroutine(CurrentSelectedCharacters[playerController].StartMovement(dir, value));
    }
    #endregion

    BaseCharacter[] currentPlayerSideChars;
    List<BaseCharacter> testedChars = new List<BaseCharacter>();
    public void PurgeBoard(System.Func<BaseCharacter, bool> condition, BaseCharacter[] specificList = null, bool rewardSigils = true)
    {
        BaseCharacter[] CharsToPurge = specificList != null ? specificList : AllCharacters.Where(r => condition(r)).ToArray();

        foreach (BaseCharacter item in CharsToPurge.Where(r => r.gameObject.activeInHierarchy).ToList())
        {
            item.CharInfo.CoinsMultiplier *= rewardSigils ? 1 : 0;
            item.CharInfo.Health = -500f;
            item.ResetBuffDebuff();
        }
    }

    private IEnumerator MoveSkillChars(TeamSideType side, ScriptableObjectSkillMask mask)
    {
        yield return WaitFor(mask.DelayPlayerDisappearing, () => CurrentBattleState == BattleState.Pause);
        foreach (BaseCharacter item in TeamInfo[side].charactersOnField.Where(r => !r.CharInfo.IsSummon))
        {
            // Debug.LogError(item.CharInfo.CharacterID + "    Test");
            item.SpineAnim.FadeInOut(false, 0, mask.SkillCharFadeInOutDuration);
            item.UMS.EnableBattleBars(false);
            chars.Add(item);
        }

        foreach (BaseCharacter item in TeamInfo[side].NPCsOnField.Where(r => r.CharID != mask.BossToSpawn.ToString() && !r.CharInfo.IsSummon))
        {
            // Debug.LogError(item.CharInfo.CharacterID + "    Test");
            item.SpineAnim.FadeInOut(false, 0, mask.SkillCharFadeInOutDuration);
            item.UMS.EnableBattleBars(false);
            chars.Add(item);
        }
    }

    private IEnumerator InOutPsSkillForChars(TeamSideType side, float delayParticlesPlayerDisappearing, bool inOut)
    {
        yield return WaitFor(delayParticlesPlayerDisappearing, () => CurrentBattleState == BattleState.Pause);
        GameObject go = null;
        foreach (BaseCharacter item in chars)
        {
            ParticleManagerScript.Instance.SetEmissivePsInPosition(ParticlesType.MaskCharOut, item.CharInfo.Facing, item.SpineAnim.transform.position, item.CharInfo.GetComponent<MeshRenderer>(), ref go);
        }
    }


    private IEnumerator WaitToInterruprAttack(TeamSideType side)
    {
        yield return WaitFor(0.2f, () => CurrentBattleState == BattleState.Pause);
        foreach (BaseCharacter item in TeamInfo[side].EnabledEnemiesCharacters)
        {
            item.currentAttackProfile.InteruptAttack();
        }

        foreach (BaseCharacter item in ItemSpawnerManagerScript.Instance.SpawnedDudes.Where(r => r.isActiveAndEnabled))
        {
            item.currentAttackProfile.InteruptAttack();
        }

        BulletManagerScript.Instance.ResetBullets();

    }


    List<BaseCharacter> chars = new List<BaseCharacter>();
    List<BaseCharacter> enemyChars = new List<BaseCharacter>();
 

    public BaseCharacter GetCharFromNameOrCharId(bool isCharOrTalkingChar, CharacterNameType cName, string identifier, CharacterSelectionType selectionPos, bool charName_Or_CharSelectionPos, ControllerType playerController)
    {
		tempCB = AllCharacters.Where(r => (!string.IsNullOrEmpty(identifier) ? r.CharID == identifier && r.CharInfo.CharacterID == cName : r.CharInfo.CharacterID == cName)).FirstOrDefault();

        if (tempCB != null)
        {
            tempCB.CharID = identifier;
        }
        else
        {
            return null;
        }
        return tempCB;
    }

    protected Dictionary<int, BaseCharacter> CreatedCharactersFromNameOrId = new Dictionary<int, BaseCharacter>();
    public BaseCharacter GetTrackedCreatedCharactersFromNameOrId(int processTrackerID)
    {
        if (!CreatedCharactersFromNameOrId.ContainsKey(processTrackerID))
            return null;
        BaseCharacter res = CreatedCharactersFromNameOrId[processTrackerID];
        CreatedCharactersFromNameOrId.Remove(processTrackerID);
        return res;
    }
    public IEnumerator CreateCharFromNameOrCharId(CharacterNameType cName, string identifier, int processTrackerID = 0)
    {
        AddressableCharacterInstancer CharCreator = new AddressableCharacterInstancer(this, new TeamSideInformationClass(
                 new List<ControllerType> { ControllerType.Player1 }, cName, TeamSideType.LeftSideTeam,
                 LevelType.Novice), transform, TeamSideType.LeftSideTeam, true);
        while (CharCreator.IsWorking)
            yield return null;

        CharCreator.Result.CharID = identifier;
        CharCreator.Result.gameObject.SetActive(true);
        if (processTrackerID != 0)
            CreatedCharactersFromNameOrId.Add(processTrackerID, CharCreator.Result);
    }



    public BaseCharacter GetCharInPos(Vector2Int pos)
    {
        tempCB = AllCharacters.Where(r => r.IsOnField && r.CharInfo.Pos.GridFight_ContainsStruct(pos)).FirstOrDefault();

        return tempCB;
    }

    public IDamageReceiver GetDamageReceiverInPos(Vector2Int pos)
    {
        IDamageReceiver tempDR = AllDamageReceivers.Where(r => r.IsOnField && r.GridPositions.GridFight_ContainsStruct(pos)).FirstOrDefault();

        return tempDR;
    }


    public BaseCharacter GetActiveCharNamed(CharacterNameType _name)
    {
        BaseCharacter chara = AllCharacters.Where(r => r.CharInfo.CharacterID == _name && r.IsOnField).FirstOrDefault();
        if (chara == null) Debug.LogError("Character with ID: " + _name.ToString() + " does not exist in the scene");
        return chara;
    }


    public IEnumerator WaitUpdate(System.Func<bool> pauseCondition)
    {
        yield return WaitUpdate(pauseCondition, () => false);
    }

    public IEnumerator WaitUpdate(System.Func<bool> pauseCondition, System.Func<bool> stopCondition)
    {
        yield return null;

        while (pauseCondition())
        {
            if (stopCondition())
            {
                yield break;
            }
            yield return null;
        }
    }

    public IEnumerator WaitUpdate(System.Action action, System.Func<bool> condition)
    {
        yield return null;

        while (condition())
        {
            action();
            yield return null;
        }
    }

    public IEnumerator WaitFor(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += DeltaTime;
        }
    }

    public IEnumerator WaitFor(float duration, System.Func<bool> pauseCondition)
    {
        float timer = 0;
        while (timer < duration)
        {
            yield return WaitUpdate(pauseCondition);
            timer += DeltaTime;
        }
    }

    public IEnumerator WaitFor(float duration, System.Func<bool> pauseCondition, System.Func<bool> stopCondition)
    {
        float timer = 0;
        while (timer < duration)
        {
            yield return WaitUpdate(pauseCondition, stopCondition);
            timer += DeltaTime;
            if (stopCondition())
            {
                yield break;
            }
        }
    }

    public IEnumerator WaitUntil(System.Func<bool> pauseCondition, System.Func<bool> stopCondition)
    {
        while (!stopCondition())
        {
            yield return WaitUpdate(pauseCondition);
        }
    }

    public IEnumerator WaitFixedUpdate(System.Func<bool> condition)
    {
        yield return new WaitForFixedUpdate();

        while (condition())
        {
            yield return null;
        }
    }

    public IEnumerator WaitFixedUpdate(System.Action action, System.Func<bool> condition)
    {
        yield return new WaitForFixedUpdate();

        while (condition())
        {
            yield return null;
            action();
        }
    }


    public void RemoveAllCharactersFromBoard()
    {
        foreach (BaseCharacter character in AllCharacters.Where(r => r.IsOnField).ToArray())
        {
            character.ResetBaseChar();
            character.RemoveFromBoard();
        }
    }


    public void HealPlayerCharacters()
    {
        foreach (BaseCharacter character in AllCharacters.Where(r => r.CurrentPlayerController != ControllerType.Enemy && r.CharInfo.Behaviour.InputBehaviour == InputBehaviourType.PlayerInput).ToArray())
        {
            if (character == null) continue;
            character.CharInfo?.ResetToBase();
            character.died = false;
        }
    }

    public void RefreshCharacter(BaseCharacter character)
    {
        character.gameObject.SetActive(false);
        character.ResetBuffDebuff();
        character.RefreshBuffDebuffIcons();
        character.transform.position = new Vector3(-1000f, -1000f, -1000f);

        character.IsOnField = false;
        character.died = false;
        character.CharInfo.CurrentTilePos = new Vector2Int(0, 0);
        character.CharInfo.Health = character.CharInfo.HealthStats.Current_B_Health;
    }
}

[System.Serializable]
public class CharacterLoadingInfoClass
{
    public CharacterNameType CName;
    public ControllerType PlayerController;
    public IEnumerator LoadingNewCharacterCo;

    public CharacterLoadingInfoClass()
    {

    }

    public CharacterLoadingInfoClass(CharacterNameType cName, ControllerType playerController, IEnumerator loadingNewCharacterCo)
    {
        CName = cName;
        PlayerController = playerController;
        LoadingNewCharacterCo = loadingNewCharacterCo;
    }
}

[System.Serializable]
public class PlayableCharOnScene
{
    public CharacterNameType CName;
    public List<ControllerType> PlayerController = new List<ControllerType>();
    public bool isUsed;
    public bool isAlive = true;
    public TeamSideType Side;

    public PlayableCharOnScene(CharacterNameType cname, List<ControllerType> playerController, bool isused, TeamSideType side)
    {
        CName = cname;
        PlayerController = playerController;
        isUsed = isused;
        Side = side;
    }
}

[System.Serializable]
public class CurrentSelectedCharacterClass
{
    public BaseCharacter _Character;

    public BaseCharacter Character
    {
        get
        {
            return _Character;
        }
        set
        {
            if (value == null)
            {

            }
            _Character = value;
        }
    }

    public BaseCharacter _NextChar;

    public BaseCharacter NextChar
    {
        get
        {
            return _NextChar;
        }
        set
        {
            if (value == null)
            {

            }
            _NextChar = value;
        }
    }


    public SwapStateType _SwapState;

    public SwapStateType SwapState
    {
        get
        {
            return _SwapState;
        }
        set
        {
            // Debug.LogError("prev  " + _SwapState + "   new  " + value);
            _SwapState = value;
        }
    }


    public float OffsetSwap;
    public float NotPlayingTimer = 0;

    public bool WeakAttackHolding
    {
        get
        {
            return WeakAttackOffset == -1 ? false : Time.time - WeakAttackOffset > 0.5f;
        }
    }
    public float WeakAttackOffset = 0;

    public CurrentSelectedCharacterClass()
    {
    }
}


[System.Serializable]
public class DisposableGameObjectClass : System.IDisposable
{
    public GameObject BaseGO;

    public DisposableGameObjectClass()
    {
    }

    public DisposableGameObjectClass(GameObject baseGO)
    {
        BaseGO = baseGO;
    }

    public void Dispose()
    {
    }
}

[System.Serializable]
public class TeamCharacterInfoClass
{
    public TeamSideType TeamType = TeamSideType.LeftSideTeam;

    public TeamSideType EnemyTeamType
    {
        get
        {
            return TeamType == TeamSideType.LeftSideTeam ? TeamSideType.RightSideTeam : TeamSideType.LeftSideTeam;
        }
    }

    public List<ControllerType> PlayerController = new List<ControllerType>()
    {
        ControllerType.Enemy
    };

    public List<ControllerType> GetPlayerController
    {
        get
        {
            ControllerType[] tempCo = new ControllerType[PlayerController.Count];

            PlayerController.CopyTo(tempCo);

            return tempCo.ToList();
        }
    }

    public WalkingSideType WalkingSide
    {
        get
        {
            return TeamType == TeamSideType.LeftSideTeam ? WalkingSideType.LeftSide : WalkingSideType.RightSide;
        }
    }

    public BaseCharacter[] characters
    {
        get
        {
            return BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Side == TeamType).ToArray();
        }
    }

    public BaseCharacter[] playerControlledCharacters
    {
        get
        {
            return characters.Where(r => r.CharInfo.isPlayerControlledCharacter).ToArray();
        }
    }

    public BaseCharacter[] charactersOnField
    {
        get
        {
            return BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Side == TeamType && r.IsOnField).ToArray();
        }
    }

    public BaseCharacter[] NPCsOnField
    {
        get
        {
            return BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Side == TeamType && r.IsOnField && r.CharInfo.PlayerController.GridFight_ContainsStruct(ControllerType.None)).ToArray();
        }
    }

    public BaseCharacter[] charactersNotOnField
    {
        get
        {
            return BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Side == TeamType && !r.IsOnField && r.CharInfo.PlayerController.GridFight_ContainsStruct(PlayerController[0])).ToArray();
        }
    }

    public BaseCharacter[] NPCsNotOnField
    {
        get
        {
            return BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Side == TeamType && !r.IsOnField && r.CharInfo.PlayerController.GridFight_ContainsStruct(ControllerType.None)).ToArray();
        }
    }

    public BaseCharacter[] enemiesCharactersOnField
    {
        get
        {
            return BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Side != TeamType && r.IsOnField && r.isActiveAndEnabled && r.CharInfo.Behaviour.InputBehaviour != InputBehaviourType.AIDumb).ToArray();
        }
    }

    public BaseCharacter[] EnabledEnemiesCharacters
    {
        get
        {
            return BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Side != TeamType && r.isActiveAndEnabled).ToArray();
        }
    }

    public TeamCharacterInfoClass()
    {

    }

    public TeamCharacterInfoClass(TeamSideType team)
    {
        TeamType = team;
    }
}


[System.Serializable]
public class SwapInfoClass
{
    public BaseCharacter Cb;
    public IEnumerator Co_SwapUi;
    public ControllerType PlayerController;
    public bool CharSelected = false;
    public bool IsCharDead = false;

    public SwapInfoClass()
    {

    }

    public SwapInfoClass(BaseCharacter cb, ControllerType playerController, bool isCharDead)
    {
        PlayerController = playerController;
        Cb = cb;
        IsCharDead = isCharDead;
    }
}
[System.Serializable]
public class ActiveSkillClass
{
    public ControllerType Controller;
    public bool Value;
    public float StartingTime;

    public ActiveSkillClass()
    {

    }

    public ActiveSkillClass(ControllerType controller, bool value)
    {
        Controller = controller;
        Value = value;
    }
}

[System.Serializable]
public class ManaInfoClass
{
    public float CurrentMana;
    public ManaInfoClass()
    {
        CurrentMana = 2;
    }
}