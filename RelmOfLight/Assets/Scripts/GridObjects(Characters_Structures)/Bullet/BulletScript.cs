using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the component that take care of all the Bullet behaviours in the game
/// </summary>
public class BulletScript : MonoBehaviour
{
    //Public____Public____Public____Public____Public____Public____Public____Public____Public____Public____Public____Public____Public
    //Source
    public BaseCharacter CharOwner;
    public SpriteRenderer Indicator;
    public Gradient BaseGradient;
    //Targetting
    public enum TargettingType { Point, Missile }
    public TargettingType targettingType = TargettingType.Point;
    public BaseCharacter TargetCharacter = null; //If targetting is set to Missile
    public Vector2Int DestinationTile; //If targetting is set to Point

    //Characteristics
    public float Damage;
    public TeamSideType Side;
    public FacingType Facing;
    public ElementalType Elemental;
    public CharacterNameType CharacterID;
    public ScriptableObjectAttackBase SOAttack;
    public BulletBehaviourInfoClass BulletBehaviourInfo;
    public BulletBehaviourInfoClassOnBattleFieldClass BulletBehaviourInfoTile;
    public BattleTileScript bts;
    public List<ParticlesChildExplosionClass> ChildrenExplosions = new List<ParticlesChildExplosionClass>();
    public CurrentAttackInfoClass nextAttack;
    public BattleFieldAttackTileClass TileAtk;
    public List<BulletType> BulletT = new List<BulletType>();


    //State

    bool isActive;
    public bool _isMoving = false;
    public bool isMoving
    {
        get
        {
            return _isMoving;
        }
        set
        {
            _isMoving = value;
        }
    }

    public bool isColliding = true;
    public bool isReflectedBullet = false;
    public bool SkillHit = false;
    public float BulletDuration;
    public bool IsFungus;

    //Effects
    public GameObject ParticleSystem;
    public float Chances;
    public GameObject TargetIndicator;
    public List<ScriptableObjectAttackEffect> BulletEffects = new List<ScriptableObjectAttackEffect>();
    public TileEffectClass BulletTileEffects;
    public TileSummonClass BulletTileSummons;
    public GrenadeSetupInfoScript Grenade;
    public BulletLevelType BulletLevel;
    //Private_____Private_____Private_____Private_____Private_____Private_____Private_____Private_____Private_____Private_____Private
    private VFXBulletSpeedController vfx;
    private BoxCollider col;
    private bool hitTarget = false;
    private Vector2Int StartingTile;
    private GameObject targetIndicator;
    private List<GameObject> targetDisplays = new List<GameObject>();
    private List<BattleTileScript> PoopedTiles = new List<BattleTileScript>();
    private BattleTileScript temp_bts;
    private BattleTileScript lastTileHitted;
    private IEnumerator childExplosion_Co = null;
    public bool OverrideIndicatorPosition;
    public Vector2Int OverrideBulletDistanceInTileIndicator;


    #region LifeCycle

    private void Awake()
    {
        vfx = GetComponentInChildren<VFXBulletSpeedController>();
        col = GetComponent<BoxCollider>();
       
    }

    private void OnEnable()
    {
        //On enabled setup the collision avoidance for determinated layers 
        childExplosion_Co = null;
        timer = 0;
        Physics.IgnoreLayerCollision(Side == TeamSideType.LeftSideTeam ? 9 : 10, Side == TeamSideType.LeftSideTeam ? 11 : 12);
    }

    #endregion
    #region Movement

    public void StartMoveToTile()
    {
        transform.localRotation = Quaternion.identity;
        hitTarget = false;
        StopAllCoroutines();
        StartCoroutine("StartMove");
    }

    public IEnumerator StartMove()
    {
        //Reset Values
        SkillHit = false;
        isMoving = true;
        arrived = false;
        isActive = true;
        //Start Things
        StartingTile = CharOwner.CharInfo.CurrentTilePos;
        lastTileHitted = GridManagerScript.Instance.GetBattleTile(StartingTile);

        if (vfx != null)
        {
            vfx.BulletTargetTime = SOAttack.AttackInput == AttackInputType.Weak ? CharOwner.CharInfo.SpeedStats.WeakBulletS : CharOwner.CharInfo.SpeedStats.StrongBulletS;
            vfx.ApplyTargetTime();
        }

        SetupTargetDisplay();

        //Wait for the move
        if (targettingType == TargettingType.Missile && TargetCharacter != null && isColliding)
            yield return MissileTargetting();
        else
            yield return MoveToTile();

        //End it
        isMoving = false;
        //Wait of one frame because of quick bullet 
        yield return null;
        if(isActive)
        {
            if (BulletT.GridFight_ContainsStruct(BulletType.Base) && hitTarget)
            {
            }
            else if (!BulletT.GridFight_ContainsStruct(BulletType.Homing) && targettingType != TargettingType.Missile)
            {
                FireEffectParticles(transform.position);
                childExplosion_Co = ChildExplosionParticlesAtk(lastTileHitted.Pos, BulletT);
                bts.SetupEffect(BulletTileEffects, CharOwner, SOAttack);
                StartCoroutine(childExplosion_Co);
            }

            if (BulletTileSummons != null)
            {
                if (BulletTileSummons.SummonSpawnPositions.Length == 0)
                    BulletTileSummons.SummonSpawnPositions = new Vector2Int[] { Vector2Int.zero };
                BattleTileScript offsetBts = null;
                foreach (Vector2Int spawnPos in BulletTileSummons.SummonSpawnPositions)
                {
                    offsetBts = GridManagerScript.Instance.GetBattleTile(bts.Pos + spawnPos);
                    if (offsetBts == null || (offsetBts.WalkingSide != WalkingSideType.Both &&
                        (SOAttack.AttackTargetSide == AttackTargetSideType.EnemySide && offsetBts.WalkingSide == BattleManagerScript.Instance.TeamInfo[CharOwner.CharInfo.Side].WalkingSide) ||
                                         (SOAttack.AttackTargetSide == AttackTargetSideType.FriendlySide && offsetBts.WalkingSide != BattleManagerScript.Instance.TeamInfo[CharOwner.CharInfo.Side].WalkingSide))) continue;

                    offsetBts.SpawnSummonFromAttack(BulletTileSummons, CharOwner);
                }
            }
            EndBullet(1f);
        }
    }


    //Move the bullet towards a target defined by the targetting information in the attack
    bool targetDead = false;
    protected void TargetDies(BaseInfoScript charInfo) { targetDead = true; }
    public IEnumerator MissileTargetting()
    {
        //Create Bullet Particle
        ParticleSystem = ParticleManagerScript.Instance.FireAttackParticlesInTransform(SOAttack.Particles.Right.BulletAddress, CharacterID,
            AttackParticlePhaseTypes.Bullet, transform, CharOwner.CharInfo.Facing, SOAttack.ParticlesInput, 99999f
            );
        
        //Check for death of target
        targetDead = false;
        if (TargetCharacter != null) TargetCharacter.CharInfo.DeathEvent += TargetDies; //THIS IS ESSENTIAL, KEEP WHEN REFACTORING

        float timeTillExpire = SOAttack.Homing_TimeTillExpiry;

        //HomingPeriod
        while (timeTillExpire > 0f && isMoving)
        {
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            timeTillExpire -= Time.deltaTime;

            float rotation = targetDead || TargetCharacter == null || !TargetCharacter.IsOnField ? 0f : Vector3.Cross((TargetCharacter.SpineAnim.transform.position - transform.position).normalized, transform.right).z;
            transform.Rotate(Vector3.forward, (Facing == FacingType.Right ? -1f : 1f) * rotation * SOAttack.Homing_TurningSpeed);
            transform.position += ((Facing == FacingType.Right ? 1f : -1f)*transform.right) * SOAttack.Homing_SlowDownMultiplier * Time.deltaTime;
        }
    }

    static AnimationCurve GetSpeedCurve(BulletBehaviourInfoClass bulletBehaviourInfo, BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfoTile)
    {
        AnimationCurve res = bulletBehaviourInfo != null ? bulletBehaviourInfo.Trajectory_Speed : bulletBehaviourInfoTile.Trajectory_Speed;
        if (res.keys.Length == 0)
            res = new AnimationCurve(AnimationCurve.Linear(0, 0, 1, 1).keys);
        return res;
    }

    float timer = 0;
    Vector3 res;
    Vector3 firedFrom;
    Vector3 destination;
    //Move the bullet on a determinated tile using the BulletInfo.Trajectory
    public IEnumerator MoveToTile()
    {
        //Create Bullet Particle
        ParticleSystem = ParticleManagerScript.Instance.FireAttackParticlesInTransform(SOAttack.Particles.Right.BulletAddress, CharacterID,
            AttackParticlePhaseTypes.Bullet, transform, CharOwner.CharInfo.Facing, SOAttack.ParticlesInput, BulletDuration + 0.5f
            );


        //setup the base offset for the movement
        firedFrom = transform.position;
        //Destination position
        destination = bts.transform.position + new Vector3(Side == TeamSideType.LeftSideTeam ? 0.2f : -0.2f, 0, 0);
        //Timer used to set up the coroutine
        timer = 0;
       
        float tempF = 0;
        bool arrived = false;
        Vector3 indicatorStartPoint = Vector3.zero;
        if ((BulletBehaviourInfo != null ? BulletBehaviourInfo.Trajectory_Z.Evaluate(0.5f) : BulletBehaviourInfoTile.Trajectory_Z.Evaluate(0.5f)) > 0)
        {
            Indicator.gameObject.SetActive(true);
            indicatorStartPoint = new Vector3(firedFrom.x, GridManagerScript.Instance.GetBattleTile(CharOwner.CharInfo.CurrentTilePos).transform.position.y, 0);
        }
        while (!arrived && isMoving && BulletDuration > 0)
        {
            if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Intro) isMoving = false;
            //Calutation for next world position of the bullet
            res = GetNextPos();

            if (timer >= 1)
            {
                arrived = true;
                destination.y += BulletBehaviourInfo != null ? BulletBehaviourInfo.Trajectory_Y.Evaluate(1) : BulletBehaviourInfoTile.Trajectory_Y.Evaluate(1);
                destination.z += BulletBehaviourInfo != null ? BulletBehaviourInfo.Trajectory_Z.Evaluate(1) : BulletBehaviourInfoTile.Trajectory_Z.Evaluate(1);
                transform.position = destination;

            }
            else
            {
                transform.position = res;
            }

            if (Indicator.gameObject.activeInHierarchy)
            {
                Indicator.transform.position = Vector3.Lerp(indicatorStartPoint, destination, GetSpeedCurve(BulletBehaviourInfo, BulletBehaviourInfoTile).Evaluate(timer));
                tempF = BulletBehaviourInfo != null ? BulletBehaviourInfo.Trajectory_Z.Evaluate(timer) : BulletBehaviourInfoTile.Trajectory_Z.Evaluate(timer);
                Indicator.color = BaseGradient.Evaluate(tempF);

                Indicator.transform.localScale = new Vector3(1 - Mathf.Clamp(tempF * 0.3f, 0 , 0.3f), 1 - Mathf.Clamp(tempF * 0.3f, 0, 0.3f), 1);
            }
            // CollisionCheck();
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

            timer += BattleManagerScript.Instance.DeltaTime / BulletDuration;
        }
        Indicator.gameObject.SetActive(false);
    }

    private Vector3 GetNextPos(float simulationTimer = 0f)
        => LerpBulletTravelPosition(firedFrom, destination, timer + (simulationTimer / BulletDuration), BulletBehaviourInfo, BulletBehaviourInfoTile);

    //GETTING THE BULLET TRAVEL CURVE_____GETTING THE BULLET TRAVEL CURVE_____GETTING THE BULLET TRAVEL CURVE_____GETTING THE BULLET TRAVEL CURVE_____GETTING THE BULLET TRAVEL CURVE
    static Vector3 temp_Vec3;
    static AnimationCurve temp_TrajectoryYCurve;
    static AnimationCurve temp_TrajectoryZCurve;
    public static Vector3 LerpBulletTravelPosition(Vector3 start, Vector3 end, float timer, BulletBehaviourInfoClass bulletBehaviourInfo)
        => LerpBulletTravelPosition(start, end, timer, bulletBehaviourInfo, null);
    public static Vector3 LerpBulletTravelPosition(Vector3 start, Vector3 end, float timer, BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfoTile)
        => LerpBulletTravelPosition(start, end, timer, null, bulletBehaviourInfoTile);
    public static Vector3 LerpBulletTravelPosition(Vector3 start, Vector3 end, float timer, BulletBehaviourInfoClass bulletBehaviourInfo, BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfoTile)
    {
        temp_TrajectoryYCurve = bulletBehaviourInfo != null ? bulletBehaviourInfo.Trajectory_Y : bulletBehaviourInfoTile.Trajectory_Y;
        temp_TrajectoryZCurve = bulletBehaviourInfo != null ? bulletBehaviourInfo.Trajectory_Z : bulletBehaviourInfoTile.Trajectory_Z;
        return LerpBulletTravelPosition(start, end, timer, GetSpeedCurve(bulletBehaviourInfo, bulletBehaviourInfoTile), temp_TrajectoryYCurve, temp_TrajectoryZCurve);
    }
    public static Vector3 LerpBulletTravelPosition(Vector3 start, Vector3 end, float timer, AnimationCurve speedCurve, AnimationCurve trajectoryYCurve, AnimationCurve trajectoryZCurve)
    {
        temp_Vec3 = Vector3.Lerp(start, end, speedCurve.Evaluate(timer));
        temp_Vec3.y = trajectoryYCurve.Evaluate(timer) + temp_Vec3.y;
        temp_Vec3.z = trajectoryZCurve.Evaluate(timer) + temp_Vec3.z;
        return temp_Vec3;
    }
    public static Vector3[] GetBulletPathNodes(Vector3 start, Vector3 end, int lod, BulletBehaviourInfoClass bulletBehaviourInfo, BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfoTile)
    {
        if(lod < 2)
        {
            Debug.LogError("Attempted to get a bullet path with a level of detail that is impossibly low! MUST BE MORE THAN 2");
            return new Vector3[0];
        }
        List<Vector3> res = new List<Vector3>();
        for (int i = 0; i < lod + 1; i++)
        {
            res.Add(LerpBulletTravelPosition(start, end, (float)i / (float)lod, bulletBehaviourInfo, bulletBehaviourInfoTile));
        }
        return res.ToArray();
    }
    public struct BulletPath
    {
        public Vector3[] PathNodes;

        public BulletPath(Vector3 start, Vector3 end, int lod, BulletBehaviourInfoClass bulletBehaviourInfo)
        {
            PathNodes = GetBulletPathNodes(start, end, lod, bulletBehaviourInfo, null);
        }
        public BulletPath(Vector3 start, Vector3 end, int lod, BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfoTile)
        {
            PathNodes = GetBulletPathNodes(start, end, lod, null, bulletBehaviourInfoTile);
        }
    }
    public static BulletPath[] GetBulletPaths(BaseCharacter originChar, ScriptableObjectAttackBase attackInfo, bool fromSpinePosition = false)
    {
        List<BulletPath> res = new List<BulletPath>();
        Vector3 startPos = fromSpinePosition ? new Vector3(originChar.spineT.position.x, originChar.spineT.position.y + 0.5f, originChar.spineT.position.z) : originChar.transform.position;

        foreach (BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo in attackInfo.TilesAtk.BulletTrajectories.Where(r => r.toFire))
        {
            foreach (BattleFieldAttackTileClass item in bulletBehaviourInfo.BulletEffectTiles.Where(a => a.ToFire))
            {
                res.Add(new BulletPath(startPos, originChar.currentAttackProfile.GetAttackTargetBattleTileScript(originChar.currentInputProfile.nextAttackPos, item, new CurrentAttackInfoClass(attackInfo, originChar.currentAttackProfile)).transform.position, 25, bulletBehaviourInfo));
            }
        }
        return res.ToArray();
    }
    //________________________________________________________________________________________________________________________________________________________________________________
    #endregion

    #region Displaying Target FX

    public void SetupTargetDisplay()
    {
        col.enabled = BulletT.GridFight_ContainsStruct(BulletType.IgnoresCollision) ? false : true;
        targetDisplays.Clear();
        PoopedTiles.Clear();

        if (!(targettingType == TargettingType.Missile && TargetCharacter != null && isColliding)) return;

        if (targettingType == TargettingType.Missile && TargetCharacter != null && isColliding)
        {
            //CreateTrackingTarget(TargetCharacter.SpineAnim.transform);
        }
        else if (targettingType == TargettingType.Point && SOAttack.AttackInput == AttackInputType.Strong)
        {
            CreateTileTarget();
        }
    }

    private void CreateTileTarget()
    {
        targetIndicator = TargetIndicatorManagerScript.Instance.GetTargetIndicator();
        targetIndicator.transform.position = OverrideIndicatorPosition ? GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(new Vector2Int(bts.Pos.x, bts.Pos.y + OverrideBulletDistanceInTileIndicator.y), Facing).transform.position : bts.transform.position;
        targetIndicator.SetActive(true);
        targetIndicator.GetComponent<BattleTileTargetScript>().StartTarget(SOAttack.AttackInput == AttackInputType.Weak ? CharOwner.CharInfo.SpeedStats.WeakBulletS : CharOwner.CharInfo.SpeedStats.StrongBulletS);
        targetDisplays.Add(targetIndicator);


        for (int a = 0; a < BulletBehaviourInfo.ChildrenExplosion.Count; a++)
        {
            for (int i = 0; i < BulletBehaviourInfo.ChildrenExplosion[a].BulletEffectTiles.Count; i++)
            {
                if (GridManagerScript.Instance.IsPosOnField((bts.Pos * (Side == TeamSideType.LeftSideTeam ? 1 : -1)) + BulletBehaviourInfo.ChildrenExplosion[a].BulletEffectTiles[i].Pos) && Random.Range(0, 100) < BulletBehaviourInfo.ChildrenExplosion[a].BulletEffectTiles[i].ExposionChances)
                {
                    temp_bts = GridManagerScript.Instance.GetBattleTile((bts.Pos * (Side == TeamSideType.LeftSideTeam ? 1 : -1)) + BulletBehaviourInfo.ChildrenExplosion[a].BulletEffectTiles[i].Pos);
                    if (temp_bts != null)
                    {
                        targetIndicator = TargetIndicatorManagerScript.Instance.GetTargetIndicator();
                        targetIndicator.transform.position = temp_bts.transform.position;
                        targetIndicator.SetActive(true);
                        targetIndicator.GetComponent<BattleTileTargetScript>().StartTarget(CharOwner.CharInfo.SpeedStats.StrongBulletS);
                        targetDisplays.Add(targetIndicator);
                    }
                }
            }
        }
    }

    private bool CreateTrackingTarget(Transform trackingTarget)
    {
        if (!trackingTarget.gameObject.activeInHierarchy) return false;

        targetIndicator = TargetIndicatorManagerScript.Instance.GetTargetIndicator(true).gameObject;
        if (targetIndicator == null) return false;

        targetIndicator.transform.parent = trackingTarget;
        targetIndicator.transform.localPosition = Vector3.zero;
        targetIndicator.SetActive(true);
        targetDisplays.Add(targetIndicator);
        return true;
    }

    private void ClearTilesTarget(bool withChildran = true)
    {
        if (targetDisplays.Count > 0)
        {
            for (int i = 0; i < (withChildran ? targetDisplays.Count : 1); i++)
            {
                targetDisplays[i].SetActive(false);
                targetDisplays[i].transform.parent = TargetIndicatorManagerScript.Instance.transform;
            }
            if (!withChildran)
            {
                targetDisplays.RemoveAt(0);
            }
        }
    }

    #endregion

    public IEnumerator ChildExplosionTilesAtk(Vector2Int basePos, List<BulletType> bulletT)
    {
        BattleTileScript temp_bts = null;
        for (int a = 0; a < ChildrenExplosions.Count; a++)
        {
            if (childExplosion_Co != null)
            {
                for (int i = 0; i < ChildrenExplosions[a].BulletEffectTiles.Count; i++)
                {
                    if(GridManagerScript.Instance.IsPosOnField(basePos + ChildrenExplosions[a].BulletEffectTiles[i].Pos))
                    {
                        temp_bts = GridManagerScript.Instance.GetBattleTile(basePos + ChildrenExplosions[a].BulletEffectTiles[i].Pos);

                        if (temp_bts != null && Random.Range(0, 100) < ChildrenExplosions[a].BulletEffectTiles[i].ExposionChances)
                        {
                            temp_bts.SetAttack(CharOwner, nextAttack, TileAtk, Damage * ChildrenExplosions[a].ChildrenDamageMultiplier, ChildrenExplosions[a].ChildrenBulletDelay,
                                         false, true);
                        }
                    }
                }
            }
        }
        childExplosion_Co = null;
        yield return null;
    }


    public IEnumerator ChildExplosionParticlesAtk(Vector2Int basePos, List<BulletType> bulletT)
    {
        IDamageReceiver target;
		BattleTileScript temp_bts = null;

        yield return null;
      
        for (int a = 0; a < ChildrenExplosions.Count; a++)
        {
            yield return BattleManagerScript.Instance.WaitFor(ChildrenExplosions[a].ChildrenBulletDelay, () => (!IsFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle) || (IsFungus && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets));

            if(childExplosion_Co != null)
            {
                for (int i = 0; i < ChildrenExplosions[a].BulletEffectTiles.Count; i++)
                {
                    if (GridManagerScript.Instance.IsPosOnField(Side == TeamSideType.LeftSideTeam ? basePos + ChildrenExplosions[a].BulletEffectTiles[i].Pos : basePos - ChildrenExplosions[a].BulletEffectTiles[i].Pos) && Random.Range(0, 100) < ChildrenExplosions[a].BulletEffectTiles[i].ExposionChances)
                    {
                        temp_bts = GridManagerScript.Instance.GetBattleTile(Side == TeamSideType.LeftSideTeam ? basePos + ChildrenExplosions[a].BulletEffectTiles[i].Pos : basePos - ChildrenExplosions[a].BulletEffectTiles[i].Pos);
                        target = BattleManagerScript.Instance.GetDamageReceiverInPos(Side == TeamSideType.LeftSideTeam ? basePos + ChildrenExplosions[a].BulletEffectTiles[i].Pos : basePos - ChildrenExplosions[a].BulletEffectTiles[i].Pos);
                        if (GridManagerScript.Instance.IsBtsUsableWithAtsandWalk(temp_bts, SOAttack.AttackTargetSide, CharOwner.CharInfo.WalkingSide))
                        {
                            FireEffectParticles(GridManagerScript.Instance.GetBattleTile(Side == TeamSideType.LeftSideTeam ? basePos + ChildrenExplosions[a].BulletEffectTiles[i].Pos : basePos - ChildrenExplosions[a].BulletEffectTiles[i].Pos).transform.position, SOAttack.EffectTimeOnImpact && SOAttack.SlowDownOnHit.TimeEffectChildExplosion);
                            if (target != null && target.ReferenceCharacter != null && !target.ReferenceCharacter.HasBuffDebuff(BuffDebuffStatsType.ShadowForm))
                            {
                                MakeDamage(target, Damage * 0.3f, target.ReferenceCharacter != null ? target.ReferenceCharacter.SpineAnim.transform.position : target.ReceiverGO.transform.position,
                                ChildrenExplosions[a].BulletEffectTiles[i].Effects.Where(r => !r.StatsToAffect.ToString().Contains("Tile")).ToList(), bulletT.GridFight_ContainsStruct(BulletType.Unstoppable));
                            }
                            else
                            {
                            }
                            if (ChildrenExplosions[a].BulletEffectTiles[i].IsEffectOnTile)
                            {
                                temp_bts.SetupEffect(ChildrenExplosions[a].BulletEffectTiles[i].EffectsOnTile, CharOwner, SOAttack);
                            }
                            if (ChildrenExplosions[a].BulletEffectTiles[i].SpawnSummonOnTile)
                            {
                                if (ChildrenExplosions[a].BulletEffectTiles[i].SummonToSpawn.SummonSpawnPositions.Length == 0) ChildrenExplosions[a].BulletEffectTiles[i].SummonToSpawn.SummonSpawnPositions = new Vector2Int[] { Vector2Int.zero };
                                BattleTileScript offsetBts = null;
                                foreach (Vector2Int spawnPos in ChildrenExplosions[a].BulletEffectTiles[i].SummonToSpawn.SummonSpawnPositions)
                                {
                                    offsetBts = GridManagerScript.Instance.GetBattleTile(temp_bts.Pos + spawnPos);
                                    if (offsetBts == null || offsetBts.WalkingSide != temp_bts.WalkingSide) continue;

                                    offsetBts.SpawnSummonFromAttack(ChildrenExplosions[a].BulletEffectTiles[i].SummonToSpawn, CharOwner);
                                }
                            }
                        }
                    }
                }
            }

        }
        childExplosion_Co = null;
        ClearTilesTarget();
    }

    public DefendingActionType MakeDamage(IDamageReceiver target, float baseDamage, Vector3 vPos,List<ScriptableObjectAttackEffect> effects, bool indefensible = false)
    {
        DefendingActionType hitPs = DefendingActionType.None;
        if (target != null)
        {
            //Debug.Log(target.tag + "    " + Side.ToString());
            if (target.ReceiverGO.tag.Contains("Side") && target.ReceiverGO.tag != Side.ToString())
            {
                hitTarget = true;

                //Set damage to the hitting character
                if (SOAttack.AttackInput != AttackInputType.Weak)
                {
                    CameraManagerScript.Shaker.PlayShake("Enemy_Getting_Hit_ChargedAttack");
                }
                hitPs = target.SetDamage(new DamageInfoClass(CharOwner,
                    this,
                    SOAttack,
                    effects,
                    Elemental,
                    false,
                    CharOwner.CharInfo.ClassType == CharacterClassType.Spellbinder && SOAttack.AttackInput != AttackInputType.Weak ? true : false,
                    true,
                    isReflectedBullet,
                    vPos), Damage);

                if (!SkillHit && SOAttack.AttackInput > AttackInputType.Strong)
                {
                    SkillHit = true;
                }
            }
        }
     
        return hitPs;
    }

    private IEnumerator BlockTileForTime(float duration, Vector2Int pos)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
        float timer = 0;
        bts.BattleTileState = BattleTileStateType.NonUsable;
        while (timer < duration)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
        }
        bts.BattleTileState = BattleTileStateType.Empty;
    }

    RaycastHit[] hits;
    Vector3 nextPos;
    BulletScript BulletInteraction = null;
    IDamageReceiver target;
    bool arrived = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Tile") && !arrived)
        {
            temp_bts = other.GetComponent<BattleTileScript>();
            lastTileHitted = temp_bts;
            arrived = temp_bts == bts;
            if (isMoving && BulletT.GridFight_ContainsStruct(BulletType.Pooping) && Random.Range(0, 100) <= SOAttack.ChancesOfPooping && temp_bts.BattleTileState != BattleTileStateType.NonUsable && temp_bts.BattleTileState != BattleTileStateType.Bound)
            {
                if (CharOwner.CharInfo.CurrentTilePos != temp_bts.Pos && (SOAttack.BothSides || (SOAttack.InEnemySide && (Side == TeamSideType.LeftSideTeam ? WalkingSideType.LeftSide : WalkingSideType.RightSide) != temp_bts.WalkingSide) ||
                    (!SOAttack.InEnemySide && (Side == TeamSideType.LeftSideTeam ? WalkingSideType.LeftSide : WalkingSideType.RightSide) == temp_bts.WalkingSide)))
                {
                    
                    PoopedTiles.Add(temp_bts);
                    if(SOAttack.isPoopingEffectOnTile)
                    {
                        temp_bts.SetupEffect(SOAttack.PoopingEffectsOnTile, CharOwner, SOAttack);
                    }

                    if(SOAttack.isPoopingSpawnSummonOnTile)
                    {
                        if (SOAttack.PoopingSummonToSpawn.SummonSpawnPositions.Length == 0)
                            SOAttack.PoopingSummonToSpawn.SummonSpawnPositions = new Vector2Int[] { Vector2Int.zero };
                        
                        foreach (Vector2Int spawnPos in SOAttack.PoopingSummonToSpawn.SummonSpawnPositions)
                        {
                            temp_bts.SpawnSummonFromAttack(SOAttack.PoopingSummonToSpawn, CharOwner);
                        }
                    }
                }
            }
        }
        else if (other.tag.Contains("Side") && other.tag != Side.ToString() && isMoving)
        {
            //BULLET T RULES_____BULLET T RULES_____BULLET T RULES_____BULLET T RULES_____BULLET T RULES_____BULLET T RULES_____BULLET T RULES_____BULLET T RULES_____BULLET T RULES_____BULLET T RULES

            if(!BulletT.GridFight_ContainsStruct(BulletType.Piercing))
            {
                isMoving = false;
                ClearTilesTarget(false);

            }
            else
            {
                isMoving = true;
            }

            //=========================================================================================================================================================================================

            target = other.GetComponentInParent<IDamageReceiver>();

            float dis = 10;
            float d = 0;

            dis = Vector3.Distance(lastTileHitted.transform.position, transform.position);

            foreach (Vector2Int item in target.ReferenceCharacter.CharInfo.Pos)
            {
                if (GridManagerScript.Instance.IsPosOnField(item))
                {
                    d = Vector3.Distance(GridManagerScript.Instance.GetBattleTile(item).transform.position, transform.position);
                    if (d < dis)
                    {
                        dis = d;
                        lastTileHitted = GridManagerScript.Instance.GetBattleTile(item);
                    }
                }
            }


            if(MakeDamage(target, Damage, transform.position, BulletEffects, BulletT.GridFight_ContainsStruct(BulletType.Unstoppable)) != DefendingActionType.Reflected)
            {
                FireEffectParticles(transform.position, SOAttack.EffectTimeOnImpact);

                lastTileHitted.SetupEffect(BulletTileEffects, CharOwner, SOAttack);

                //SPECIAL EFFECTS_______SPECIAL EFFECTS_______SPECIAL EFFECTS_______SPECIAL EFFECTS_______SPECIAL EFFECTS_______SPECIAL EFFECTS_______SPECIAL EFFECTS_______SPECIAL EFFECTS_______SPECIAL EFFECTS

                childExplosion_Co = ChildExplosionParticlesAtk(lastTileHitted.Pos, BulletT);
                StartCoroutine(childExplosion_Co);
            }
        }
    }

    public void FireEffectParticles(Vector3 pos, bool slowDown = false)
    {
        //fire the Effect
        if (SOAttack.HitParticlesT != HitParticlesType.None || !hitTarget ||
            (lastTileHitted.BattleTileState == BattleTileStateType.Bound))
        {
            GameObject HitPs = ParticleManagerScript.Instance.FireAttackParticlesInPosition(SOAttack.Particles.Right.HitAddress,
                CharacterID, AttackParticlePhaseTypes.Hit, pos, CharOwner.CharInfo.Facing, SOAttack.ParticlesInput, SOAttack.HitParticlesT, SOAttack.HitResizeMultiplier);
           
            if (slowDown)
            {
                SlowDownPs(HitPs);
            }
        }
    }


    private void SlowDownPs(GameObject ps)
    {
        ParticleHelperScript particleHelperScript = ps.GetComponent<ParticleHelperScript>();

        if (particleHelperScript == null) return;

        particleHelperScript.SetSimulationSpeedOverTime(SOAttack.SlowDownOnHit);
    }

    public void EndBullet(float timer, bool stopBullet = false, bool useDirectionalExplosion = false)
    {
        if(useDirectionalExplosion)
            ParticleManagerScript.Instance.FireParticlesInPosition(Facing == FacingType.Right ? ParticlesType.BulletImpactRight : ParticlesType.BulletImpactLeft, transform.position);
        isActive = false;
        ClearTilesTarget();
        if (stopBullet && childExplosion_Co != null)
        {
            StopAllCoroutines();
            childExplosion_Co = null;
        }
        StartCoroutine(RestoreBullet(timer));
        ParticleSystem.GetComponent<ParticleHelperScript>().StopPs();
        ParticleSystem.transform.parent = null;
  
    }

    IEnumerator RestoreBullet(float timer)
    {
        col.enabled = false;

        yield return BattleManagerScript.Instance.WaitFor(timer, ()=> BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        while (childExplosion_Co != null)
        {
            yield return null;
        }
        Indicator.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}

