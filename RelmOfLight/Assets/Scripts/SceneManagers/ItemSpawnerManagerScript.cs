using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawnerManagerScript : MonoBehaviour
{
    public delegate void ItemPickedUp();
    public event ItemPickedUp ItemPickedUpEvent;

    public static ItemSpawnerManagerScript Instance;
    
    [Header("POWER UP INFORMATION")]
    [Header("Configuration")]
    [Tooltip("The prefab for spawning potions")] public GameObject ItemGO;

    protected List<ScriptableObjectItemPowerUps> CurrentSpecifiedPowerups = new List<ScriptableObjectItemPowerUps>();
    [Tooltip("The starting powerups, this can be replaced at runtime using fungus")] public List<ScriptableObjectItemPowerUps> DefaultPowerUps = new List<ScriptableObjectItemPowerUps>();

    [Tooltip("The time between powerup spawns, randomised between these two values in seconds")] public Vector2 SpawningTimeRange = new Vector2(5f,10f);
    [Tooltip("The odds that the next spawn will be a smokolokom out of 100%")] [Range(0f, 100f)] public float smokolokoSpawningOdds = 5f;
    [Tooltip("The limit on how many smokolokos can be on the field at any time")] public int maxSmokolokosOnField = 1;

    [Header("Runtime Values")]
    public List<ItemsPowerUPsInfoScript> SpawnedItems = new List<ItemsPowerUPsInfoScript>();
    public List<BaseCharacter> SpawnedDudes = new List<BaseCharacter>();
    public int SmokolokoOnField
    {
        get
        {
            return SpawnedDudes.Where(r => r != null && r.isActiveAndEnabled && r.IsOnField).ToArray().Length;
        }
    }

    public bool CoStopper = false;
    private bool spawningCoPaused = false;
    public float spawningTime
    {
        get
        { 
            return Random.Range(SpawningTimeRange.x, SpawningTimeRange.y);
        }
    }
    private IEnumerator SpawningCo;

    [Space(20)]
    [Header("'SUMMONED' INFORMATION")]
    public List<BaseCharacter> SpawnedSummons = new List<BaseCharacter>();



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentSpecifiedPowerups = DefaultPowerUps;
        if (DefaultPowerUps.Count > 0)
        {
            StartSpawningCo(SpawningTimeRange);
        }
        BattleManagerScript.Instance.CurrentBattleSpeedChangedEvent += Instance_CurrentBattleSpeedChangedEvent;
    }

    private void Instance_CurrentBattleSpeedChangedEvent(float currentBattleSpeed)
    {
        foreach (BaseCharacter item in SpawnedSummons)
        {
            ParticleHelperScript psh = item.CharInfo.GetComponentInChildren<ParticleHelperScript>();
            if(psh != null)
            {
                if (currentBattleSpeed == 1)
                {
                    psh.SetSimulationSpeedToBase();
                }
                else
                {
                    psh.SetSimulationSpeed(currentBattleSpeed);
                }
            }
            
        }
    }

    #region Power Up & Smokoloko Handling

    public void PauseSpawning()
    {
        spawningCoPaused = true;
    }

    public void PlaySpawning(ScriptableObjectItemPowerUps[] specifiedPowerUpSpawns = null)
    {
        CurrentSpecifiedPowerups = specifiedPowerUpSpawns != null && specifiedPowerUpSpawns.Length > 0 ? specifiedPowerUpSpawns.ToList() : DefaultPowerUps;
        spawningCoPaused = false;
    }
    SummonScript temp_summon;
    public void KillAllSummons()
    {
        for (int i = 0; i < SpawnedSummons.Count; i++)
        {
            temp_summon = SpawnedSummons[i].GetComponent<SummonScript>();
            if(temp_summon != null)
            {
                temp_summon.countingDown = false;
            }
        }
    }

    public void KillAllSmokoloko()
    {
        for (int i = 0; i < SpawnedDudes.Count; i++)
        {
            temp_summon = SpawnedDudes[i].GetComponent<SummonScript>();
            if (temp_summon != null)
            {
                temp_summon.countingDown = false;
            }
        }
    }

    public void StartSpawningCo(Vector2 spawningTimeRange)
    {
        SpawningTimeRange = spawningTimeRange;
        if(SpawningCo != null)
        {
            StopCoroutine(SpawningCo);
        }
        SpawningCo = Spawning_Co();
        StartCoroutine(SpawningCo);
    }

    private IEnumerator Spawning_Co()
    {
        while (!CoStopper)
        {
            yield return BattleManagerScript.Instance.WaitFor(spawningTime, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || BattleManagerScript.Instance.isSkillHappening.Value || spawningCoPaused);

            bool spawnSmokoloko = Random.Range(0f, 100f) <= smokolokoSpawningOdds && SmokolokoOnField < maxSmokolokosOnField;
            WalkingSideType walkingSide = WalkingSideType.LeftSide;
            walkingSide = spawnSmokoloko ? walkingSide == WalkingSideType.LeftSide ? WalkingSideType.RightSide : walkingSide == WalkingSideType.RightSide ? WalkingSideType.LeftSide : WalkingSideType.Both : walkingSide;

            SpawnPowerUpRandomPos(CurrentSpecifiedPowerups[Random.Range(0, CurrentSpecifiedPowerups.Count)], walkingSide, spawnSmokoloko);
        }
    }

    public void SpawnPowerUpRandomPos(ScriptableObjectItemPowerUps nextItemPowerUp, WalkingSideType walkingSide, bool spawnSmokoloko = false, float smokolokoDurationOnField = 4f, bool canAttack = true)
    {
        BattleTileScript bts = null;
        for (int i = 0; i < 100; i++)
        {
            bts = GridManagerScript.Instance.GetFreeBattleTile(walkingSide);
            if(bts != null)
            {
                if (bts.BattleTileState == BattleTileStateType.Empty && SpawnedItems.Where(r => r.isActiveAndEnabled && r.Pos == bts.Pos).ToList().Count == 0)
                {
                    break;
                }
                else if (i == 99)
                {
                    Debug.Log("Attempted to spawn power in a random pos, but was unable to after " + i.ToString() + " tries, conditions were not met!!!! Aborting!!!!");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        if (spawnSmokoloko)
        {
            #if !TRAIL
            StartCoroutine(SpawnPotionDude(nextItemPowerUp, bts, smokolokoDurationOnField, canAttack));
            #endif
        }
        else
        {
            SpawnPowerUpAtBts(nextItemPowerUp, bts);
        }
    }

    public void SpawnPotionFromTo(Vector3 spawnOriginPos, Vector2Int finalSpawnTile, ScriptableObjectItemPowerUps nextItemPowerUp, float duration, GameObject fromToParticles = null, float throwDelay = 0f, MeshRenderer mesh = null)
    {
        StartCoroutine(SpawnPotionFromToCo(spawnOriginPos, finalSpawnTile, nextItemPowerUp, duration, fromToParticles, throwDelay, mesh));
    }

    IEnumerator SpawnPotionFromToCo(Vector3 spawnOriginPos, Vector2Int finalSpawnTile, ScriptableObjectItemPowerUps nextItemPowerUp, float duration, GameObject fromToParticles = null, float throwDelay = 0f, MeshRenderer mesh = null)
    {
        yield return new WaitForSeconds(throwDelay);

        bool useParticles = fromToParticles != null && fromToParticles.GetComponentInChildren<VFXOffsetToTargetVOL>() != null;

        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(finalSpawnTile);

        if(bts == null)
        {
            Debug.LogError("COULDN'T GET TILE TO SPAWN POTION ON FROM DEATHDROPS, BLAME BELT... ABORTING");
            yield break;
        }

        if (useParticles)
        {
            fromToParticles = Instantiate(fromToParticles, spawnOriginPos, Quaternion.identity, transform);
            if(mesh != null)
            {
                ParticleManagerScript.Instance.SetEmissivePsInTransform(ParticlesType.None, FacingType.Left, null, mesh, ref fromToParticles, false);
            }

            foreach (VFXOffsetToTargetVOL item in fromToParticles.GetComponentsInChildren<VFXOffsetToTargetVOL>(true))
            {
                ParticleSystem.MainModule travelParticleMain = item.GetComponent<ParticleSystem>().main;
                item.GetComponent<ParticleSystem>().Stop();
                travelParticleMain.duration = duration;
                travelParticleMain.startColor = nextItemPowerUp.activeParticles.GetComponentsInChildren<ParticleSystem>().Last().main.startColor.colorMin;
                item.gameObject.SetActive(true);
                item.GetComponent<ParticleSystem>().Play();
                item.Target = bts.transform;
            }
        }
        yield return new WaitForSeconds(duration);

        SpawnPowerUpAtBts(nextItemPowerUp, bts);
    }


    public void SpawnPowerUpAtGridPos(ScriptableObjectItemPowerUps powerUp, Vector2Int pos, float duration = 0f)
    {
        SpawnPowerUpAtBts(powerUp, GridManagerScript.Instance.GetBattleTile(pos));
    }

    public void SpawnPowerUpAtBts(ScriptableObjectItemPowerUps nextItemPowerUp, BattleTileScript bts)
    {
        if (bts.BattleTileState != BattleTileStateType.Empty || SpawnedItems.Where(r => r.isActiveAndEnabled && r.Pos == bts.Pos).ToList().Count != 0)
        {
            Debug.Log("Attempted to spawn power up in pos: " + bts.Pos.ToString() + " but that position was already occupied by another power up or character!");
            return;
        }
        ItemsPowerUPsInfoScript item = SpawnedItems.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (item == null)
        {
            item = Instantiate(ItemGO, transform).GetComponent<ItemsPowerUPsInfoScript>();
            SpawnedItems.Add(item);
        }
        item.ItemPickedUpEvent += Item_ItemPickedUpEvent;
        item.gameObject.SetActive(true);
        item.SetItemPowerUp(nextItemPowerUp, bts.transform.position, bts.Pos);
    }

    public IEnumerator SpawnPotionDude(ScriptableObjectItemPowerUps nextItemPowerUp, BattleTileScript bts, float durationOnField = 0f, bool canAttack = true)
    {
        TeamSideType team = bts.WalkingSide == WalkingSideType.LeftSide ? TeamSideType.LeftSideTeam : TeamSideType.RightSideTeam;

        BaseCharacter dudeToSpawn = SpawnedDudes.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (dudeToSpawn == null)
        {
            AddressableCharacterInstancer CharCreator = new AddressableCharacterInstancer(this, new TeamSideInformationClass(new List<ControllerType>() { ControllerType.Enemy }, CharacterNameType.Potion_Smokoloko, team, LevelType.Novice), transform, team);
            while (CharCreator.IsWorking)
                yield return null;
            dudeToSpawn = CharCreator.Result;
            SpawnedDudes.Add(dudeToSpawn);
        }
        dudeToSpawn.gameObject.SetActive(true);

        dudeToSpawn.CharInfo.CurrentTilePos = bts.Pos;
        for (int i = 0; i < dudeToSpawn.CharInfo.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(dudeToSpawn.CharInfo.Pos[i], BattleTileStateType.Occupied);
        }
        dudeToSpawn.CharInfo.DeathDrops[0].powerUp = nextItemPowerUp;

        //smokolokoMat.SetColor("Color_Multiplier_2", Color.blue);

        dudeToSpawn.ResetBaseChar();
        dudeToSpawn.CharInfo.ResetToBase();
        dudeToSpawn.CharInfo.SetupChar();
        dudeToSpawn.SetUpEnteringOnBattle(fireArrivingPs_Audio: false);
        if(!canAttack)
        {
            dudeToSpawn.CharInfo._CurrentAttackTypeInfo.Clear();
        }
        yield return BattleManagerScript.Instance.MoveCharToBoardWithDelay(0.2f, dudeToSpawn, bts.transform.position);

        if (dudeToSpawn.GetComponent<SmokolokoScript>() == null)
        {
            dudeToSpawn.gameObject.AddComponent<SmokolokoScript>();
        }
        dudeToSpawn.GetComponent<SmokolokoScript>().Init(nextItemPowerUp, durationOnField);

        Color potionColor = nextItemPowerUp.activeParticles.GetComponentsInChildren<ParticleSystem>().Last().main.startColor.colorMin;
        Material smokolokoMat = dudeToSpawn.GetComponentInChildren<MeshRenderer>().material;
        smokolokoMat.SetColor("Color_Multiplier_1", potionColor);

    }

    public void CollectPowerUp(ScriptableObjectItemPowerUps powerUp, BaseCharacter receiver, Vector3 CollectionParticlePos)
    {
        if(powerUp == null || receiver == null)
        {
            Debug.LogError("UNABLE TO COLLECT POWERUP, IS THE POWERUP OR RECEIVER INFORMATION NULL?");
            return;
        }

        receiver.Buff_DebuffCo(receiver, powerUp, null);

      
        if (powerUp.terminationParticles != null)
        {
            ParticleManagerScript.Instance.FireParticlesInPosition(powerUp.terminationParticles, CollectionParticlePos);
        }
    }

    private void Item_ItemPickedUpEvent()
    {
        ItemPickedUpEvent?.Invoke();
    }

    public void SpawnPowerUpAtRandomPointOnSide(ScriptableObjectItemPowerUps powerUp, WalkingSideType side, float duration = 0f)
    {
        SpawnPowerUpAtGridPos(powerUp, GridManagerScript.Instance.GetFreeBattleTile(side).Pos, duration);
    }

#endregion


#region Summon Spawning


    public IEnumerator SpawnSummonOnRandomTile(CharacterNameType charToSpawn, WalkingSideType walkingSide, BaseCharacter summoner, float durationOnField = 0f, BaseInfoInjectorClass overrideInfo = null)
    {
        if(GridManagerScript.Instance.BattleTiles.Where(r=> r.WalkingSide == walkingSide && r.BattleTileState == BattleTileStateType.Empty).ToList().Count > 0)
        {
            BattleTileScript bts = null;
            while (true)
            {
                bts = GridManagerScript.Instance.GetFreeBattleTile(walkingSide);
                if (bts != null)
                {
                    if (bts.BattleTileState == BattleTileStateType.Empty && SpawnedItems.Where(r => r.isActiveAndEnabled && r.Pos == bts.Pos).ToList().Count == 0)
                    {
                        break;
                    }
                }
                else
                {
                    yield break;
                }
            }
            yield return SpawnSummon(charToSpawn, bts, summoner, durationOnField, overrideInfo);
        }
    }

    public Dictionary<int, BaseCharacter> TrackedSummonOutputs = new Dictionary<int, BaseCharacter>();
    public IEnumerator SpawnSummon(CharacterNameType charToSpawn, BattleTileScript bts, BaseCharacter summoner, float durationOnField = 0f, BaseInfoInjectorClass overrideInfo = null, bool showHp = true, int trackingID = 0)
    {
        TeamSideType team = summoner == null ? bts.WalkingSide == WalkingSideType.LeftSide ? TeamSideType.LeftSideTeam : TeamSideType.RightSideTeam : summoner.CharInfo.Side;

        BaseCharacter summon = SpawnedSummons.Where(r => !r.gameObject.activeInHierarchy && r.CharInfo.CharacterID == charToSpawn).FirstOrDefault();
        TeamSideInformationClass teamInfoC = new TeamSideInformationClass(
                new List<ControllerType>() { (BattleManagerScript.Instance.TeamInfo[team].PlayerController.Contains(ControllerType.Enemy) ? ControllerType.Enemy : ControllerType.None) },
                charToSpawn, team, LevelType.Novice);

        if (summon == null)
        {
            AddressableCharacterInstancer CharCreator = new AddressableCharacterInstancer(this, teamInfoC, transform, team, showHP: showHp);
            while (CharCreator.IsWorking)
                yield return null;
            summon = CharCreator.Result;
            SpawnedSummons.Add(summon);
        }
   
        summon.CharInfo.ResetToBase();
        summon.UpdateSwappableSO(MovementActionType.None);
        if(summon.CharInfo.Behaviour.MovementActionN == MovementActionType.Idle || summon.CharInfo.Behaviour.MovementActionN == MovementActionType.None)
        {
            GridManagerScript.Instance.GetBattleTile(bts.Pos + (team == TeamSideType.LeftSideTeam ? new Vector2Int(0,-1) : new Vector2Int(0, 1))).TileHeatMapPoints += 3;
        }
        summon.CharInfo.IsSummon = true;
        summon.CharInfo.SetupChar(useSigils: false);
        summon.CharInfo.Side = teamInfoC.Team;
        summon.CharInfo.Facing = teamInfoC.DefaultFacing;
        summon.SetupCharacterSide(teamInfoC.Team, false, showHp, false);
        summon.gameObject.SetActive(true);
        if (overrideInfo != null) summon.SetCharacterStats(overrideInfo, useSigils: false);
        BattleManagerScript.Instance.SetCharOnBoardOnFixedPos(summon.CharInfo.PlayerController[0], summon, bts.Pos, summon.SpineAnim.HasAnimation("Arriving") ? CharacterAnimationStateType.Arriving : CharacterAnimationStateType.Idle);
        summon.SetLayer();
        summon.CharInfo.PlayerController = teamInfoC.PlayerController;
        summon.currentDeathProfile?.Reset();
        summon.currentDeathProfile = (ScriptableObjectBaseCharacterDeath)summon.CharInfo.SwappableBases.RuntimeBases.Where(r => r.BaseName == DeathBehaviourType.Explosion.ToString()).First().Swappable;
        summon.currentDeathProfile.CharOwner = summon;
        //Add the summon script for controlling summon specific things
        if (summon.GetComponent<SummonScript>() == null)
            summon.gameObject.AddComponent<SummonScript>();
        summon.GetComponent<SummonScript>().Init(durationOnField);
        //===========================================================

        if (trackingID != 0)
            TrackedSummonOutputs.Add(trackingID, summon);
    }


    #endregion
    public bool test = false;
    public Vector2Int pos;
    private void Update()
    {
        if(test)
        {
            test = false;
            StartCoroutine(SpawnSummon(CharacterNameType.Structure_Barrier_PaiRock, GridManagerScript.Instance.GetBattleTile(pos),null));
        }
    }
}



public class GrenadeSetupInfoScript
{
    public BaseCharacter Attacker = null;

    public ParticlesType GrenadePS;
    public ParticlesType ExplosionPS;
    public string GrenadePSAddress = null;
    public string ExplosionPSAddress = null;
    public AttackParticlesInputType AtkPsInput;
    public float DurationOnField = 5f;
    public float Damage = 600f;
    public bool Defensible = false;
    public TeamSideType ExplosionReceivingTeam = TeamSideType.LeftSideTeam;
    public Sprite Image = null;
    public bool Destructable = true;
    public float Health = 100f;
    public DamageInfoClass DamageInfo;
    public Vector2Int[] ExplosionTiles = new Vector2Int[0];
    public DirtyBombEffectsList[] DirtyBombEffects = new DirtyBombEffectsList[0];


    public GrenadeSetupInfoScript(BaseCharacter attacker, float duration, float damage, bool defensible, TeamSideType explosionReceivingTeam, Sprite image,
        bool destructable, float health, Vector2Int[] explosionTiles, DirtyBombEffectsList[] dirtyBombEffects, DamageInfoClass DIC, AttackParticlesInputType atkPsInput)
    {
        Attacker = attacker;
        DurationOnField = duration;
        Damage = damage;
        Defensible = defensible;
        ExplosionReceivingTeam = explosionReceivingTeam;
        Image = image;
        Destructable = destructable;
        Health = health;
        ExplosionTiles = explosionTiles;
        DirtyBombEffects = dirtyBombEffects;
        DamageInfo = DIC;
        AtkPsInput = atkPsInput;
    }

    public void SetParticleSystems(bool overrideEffects, string grenadePS, string explsionPS, ParticlesType override_grenadePS, ParticlesType override_explsionPS)
    {
        if (overrideEffects)
        {
            GrenadePS = override_grenadePS;
            ExplosionPS = override_explsionPS;
        }
        GrenadePSAddress = overrideEffects ? "" : grenadePS;
        ExplosionPSAddress = overrideEffects ? "" : explsionPS;
    }
}
