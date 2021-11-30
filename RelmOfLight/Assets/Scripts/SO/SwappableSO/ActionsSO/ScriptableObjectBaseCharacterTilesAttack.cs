using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/TilesAttack")]
public class ScriptableObjectBaseCharacterTilesAttack : ScriptableObjectBaseCharacterBaseAttack
{

    protected float tempFloat_1, tempFloat_2;
    protected int tempInt_1, tempInt_2, tempInt_3;
    protected Vector2Int tempVector2Int;
    protected Vector3 tempVector3;
    protected string tempString;
    private Spine.Animation tempAnimation;
    private List<Spine.Timeline> tempTimeLine;
    private Spine.EventTimeline tempEventTimeLine;
    private List<float> ChargingTime = new List<float>();
    private BaseCharacter temp_cb;

    public override IEnumerator Attack(ScriptableObjectAttackBase atk, bool isFungus = false)
    {
        yield return base.Attack(atk, isFungus);
    }

    public override bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if(animState.Contains(CharacterAnimationStateType.Defending.ToString()))
    {
       
        }
        if (((animState.Contains("GettingHit") ||
           (animState.Contains("Buff") && (!animState.Contains("S_Buff") && !animState.Contains("S_DeBuff"))) ||
           animState.Contains("Defending") ||
           (animState.Contains("Debuff") && !animState.Contains("S_DeBuff"))) && Attacking) ||
           (animState.Contains("Defending") && CharOwner.SpineAnim.CurrentAnim.Contains("Defending")))
        {
            return true;
        }

        return base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }


    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if (completedAnim.Contains("IdleToAtk") && CharOwner.SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            temp_caic = CurrentAttack(completedAnim);
            if (temp_caic != null)
            {
                temp_caic.currentAttackPhase = AttackPhasesType.Charging;
            }
        }

        if (completedAnim.Contains(CharacterAnimationStateType.Atk1_Loop.ToString()) &&
       CharOwner.SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Atk1_Loop.ToString()))
        {
            temp_caic = CurrentAttack(completedAnim);
            if (temp_caic != null)
            {
                temp_caic.currentAttackPhase = AttackPhasesType.Reset;
                return true;
            }
        }

        if(completedAnim.Contains("Charging") && !CharOwner.SpineAnim.CurrentAnim.Contains("Charging"))
        {
            return true;
        }


        if (completedAnim.Contains("AtkToIdle"))
        {
            InteruptAttack();
        }

        if(completedAnim.Contains("Dash") && CharOwner.isMoving)
        {
            CharOwner.isMoving = false;
        }

        return base.SpineAnimationState_Complete(completedAnim);
    }

    public override void CreateAttack(Vector2Int nextAttackPos, CurrentAttackInfoClass cAtk)
    {
        if(cAtk == null || cAtk.CurrentAttack == null)
        {
            return;
        }

        CreateTileAttack(nextAttackPos, cAtk);
    }


    public override void CastAttackParticles(CurrentAttackInfoClass cAtk)
    {
        cAtk.CurrentAttack.taPhase = TileAttackPhaseType.Cast;
        base.CastAttackParticles(cAtk);
    }

    BulletBehaviourInfoClassOnBattleFieldClass[] bulletsBehaviourInfo = new BulletBehaviourInfoClassOnBattleFieldClass[0];


    public void CreateTileAttackBullets(Vector2Int nextAttackPos, CurrentAttackInfoClass cAtk)
    {
        if (cAtk == null)
        {
            return;
        }


        if (cAtk.CurrentAttack.TilesAtk.EffectOnCaster)
        {
            for (int i = 0; i < cAtk.CurrentAttack.TilesAtk.EffectsOnCaster.Count; i++)
            {
                CharOwner.Buff_DebuffCo(CharOwner, cAtk.CurrentAttack.TilesAtk.EffectsOnCaster[i], cAtk.CurrentAttack);
            }
        }

        if (cAtk.CurrentAttack.AttackInput == AttackInputType.Strong)
        {
            return;
        }

        nextAttackPos = CharOwner.HasBuffDebuff(BuffDebuffStatsType.Confusion) ?
                       GridManagerScript.Instance.GetFreeBattleTile(GridManagerScript.Instance.GetBattleTile(nextAttackPos).WalkingSide).Pos : 
                       nextAttackPos;

        bulletsBehaviourInfo = cAtk.CurrentAttack.TilesAtk.BulletTrajectories.Where(r => r.toFire).ToList().OrderBy(a => a.Delay).ToArray();
        //Debug.Log("Bullets   " + bulletsBehaviourInfo.Length);

        if(bulletsBehaviourInfo.Length > 0)
        {
            cAtk.CurrentAttack.taPhase = TileAttackPhaseType.Bullet;
            foreach (BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo in bulletsBehaviourInfo.Where(r => r.Delay == bulletsBehaviourInfo[0].Delay).ToArray())
            {
                CreateBullet(nextAttackPos, bulletBehaviourInfo, cAtk);
            }
        }

       
    }


    public override BattleTileScript[] GetAttackTargetBattleTileScripts(Vector2Int nextAttackPos, CurrentAttackInfoClass cAtk, bool includeChildrenExplosions = false)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        BattleTileScript curBts = null;
        BattleTileScript tempBts = null;
        foreach (BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo in bulletsBehaviourInfo.Where(r => r.Delay == bulletsBehaviourInfo[0].Delay).ToArray())
        {
            foreach (BattleFieldAttackTileClass item in bulletBehaviourInfo.BulletEffectTiles.Where(a => a.ToFire))
            {
                res.Add(GetAttackTargetBattleTileScript(nextAttackPos, item, cAtk));
                if (res.Last() == null)
                    res.RemoveAt(res.Count - 1);

                if (includeChildrenExplosions)
                {
                    curBts = res.Last();
                    for (int a = 0; a < item.ChildrenExplosion.Count; a++)
                    {
                        for (int i = 0; i < item.ChildrenExplosion[a].BulletEffectTiles.Count; i++)
                        {
                            tempBts = GridManagerScript.Instance.GetBattleTile((curBts.Pos * (CharOwner.CharInfo.Side == TeamSideType.LeftSideTeam ? 1 : -1)) + item.ChildrenExplosion[a].BulletEffectTiles[i].Pos, false, true);
                            if (tempBts != null)
                            {
                                res.Add(tempBts);
                            }
                        }
                    }
                }
            }
        }
        return res.ToArray();
    }

    public override BattleTileScript GetAttackTargetBattleTileScript(Vector2Int nextAttackPos, BattleFieldAttackTileClass bfAttackTile, CurrentAttackInfoClass cAtk)
    {
        BattleTileScript res = null;
        if (cAtk.CurrentAttack.TilesAtk.AtkType != BattleFieldAttackType.OnItSelf)
        {
            if (cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack && !GridManagerScript.Instance.IsPosOnField(bfAttackTile.Pos))
            {
                return null;
            }
            else if (cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack && GridManagerScript.Instance.IsPosOnField(bfAttackTile.Pos))
            {
                tempVector2Int = bfAttackTile.Pos;
            }

            if (cAtk.CurrentAttack.TilesAtk.AtkType != BattleFieldAttackType.OnAreaAttack && !GridManagerScript.Instance.IsPosOnField(nextAttackPos + bfAttackTile.Pos))
            {
                return null;
            }
            else if (cAtk.CurrentAttack.TilesAtk.AtkType != BattleFieldAttackType.OnAreaAttack)
            {
                if (cAtk.CurrentAttack.TilesAtk.UseBoundaries)
                {
                    tempVector2Int = GridManagerScript.Instance.GetBoundInRow(nextAttackPos + bfAttackTile.Pos, CharOwner.CharInfo.Facing).Pos;
                }
                else
                {
                    tempVector2Int = nextAttackPos + bfAttackTile.Pos;
                }
            }

            res = GridManagerScript.Instance.GetBattleTile(tempVector2Int, cAtk.CurrentAttack.TilesAtk.UseBoundaries);
            if (cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack)
            {
                switch (cAtk.CurrentAttack.AttackTargetSide)
                {
                    case AttackTargetSideType.EnemySide:
                        if(temp_bts == null || CharOwner == null)
                        {

                        }
                        if (res.WalkingSide == CharOwner.CharInfo.WalkingSide)
                        {
                            return null;
                        }
                        break;
                    case AttackTargetSideType.FriendlySide:
                        if (res.WalkingSide != CharOwner.CharInfo.WalkingSide)
                        {
                            return null;
                        }
                        break;
                }
            }
        }
        else
        {
            tempVector2Int = CharOwner.CharInfo.CurrentTilePos + bfAttackTile.Pos;
            res = GridManagerScript.Instance.GetBattleTile(tempVector2Int);

            if (res == null)
            {
                return null;
            }
            switch (cAtk.CurrentAttack.AttackTargetSide)
            {
                case AttackTargetSideType.EnemySide:
                    if (res.WalkingSide == CharOwner.CharInfo.WalkingSide)
                    {
                        return null;
                    }
                    break;
                case AttackTargetSideType.FriendlySide:
                    if (res.WalkingSide != CharOwner.CharInfo.WalkingSide)
                    {
                        return null;
                    }
                    break;
            }
        }
        return res;
    }

    public override void CreateBullet(Vector2Int nextAttackPos, BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo, CurrentAttackInfoClass cAtk)
    {
        //Debug.Log("Creation bullet");

        bulletBehaviourInfo.toFire = false;
        if(cAtk.CurrentAttack.TilesAtk.AtkType != BattleFieldAttackType.OnItSelf)
        {
            foreach (BattleFieldAttackTileClass item in bulletBehaviourInfo.BulletEffectTiles.Where(a => a.ToFire))
            {
                temp_bts = GetAttackTargetBattleTileScript(nextAttackPos, item, cAtk);
                if (temp_bts == null)
                    continue;

                bullet = BulletManagerScript.Instance.GetBullet().GetComponent<BulletScript>();
                bullet.isColliding = true;
                bullet.isReflectedBullet = false;
                bullet.BulletBehaviourInfo = null;
                bullet.BulletBehaviourInfoTile = bulletBehaviourInfo;
                ParticlesChildExplosionClass[] cE = new ParticlesChildExplosionClass[item.ChildrenExplosion.Count];
                item.ChildrenExplosion.CopyTo(cE);
                bullet.ChildrenExplosions = cE.ToList();
                bullet.nextAttack = cAtk;
                bullet.TileAtk = item;
                bullet.BulletLevel = bulletBehaviourInfo.OverrideBulletLevel ? bulletBehaviourInfo.BulletLevel : cAtk.CurrentAttack.TilesAtk.BulletLevel;
                bullet.Chances = item.EffectChances;
                float duration = ((cAtk.CurrentAttack.AttackInput == AttackInputType.Weak
                    ? CharOwner.CharInfo.SpeedStats.WeakBulletS : CharOwner.CharInfo.SpeedStats.StrongBulletS) / 12f) * (cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack || temp_bts.BattleTileState == BattleTileStateType.Bound ? 4 :
                    CharOwner.CharInfo.CurrentTilePos.y == nextAttackPos.y ? Mathf.Abs(CharOwner.CharInfo.CurrentTilePos.x - nextAttackPos.x) : Mathf.Abs(CharOwner.CharInfo.CurrentTilePos.y - nextAttackPos.y));
                bullet.BulletDuration = duration * bulletBehaviourInfo.TimeMultiplier;
                if (item.HasEffect)
                {
                    ScriptableObjectAttackEffect[] abAtkBase = new ScriptableObjectAttackEffect[item.Effects.Count];
                    item.Effects.CopyTo(abAtkBase);
                    bullet.BulletEffects = abAtkBase.ToList();
                }
                else
                {
                    bullet.BulletEffects.Clear();
                }


                if (cAtk.CurrentAttack.BulletT.Contains(BulletType.Grenade))
                {
                    if (bulletBehaviourInfo.OverrideBulletInfo)
                    {
                        bullet.Grenade = new GrenadeSetupInfoScript(
                        CharOwner,
                        bulletBehaviourInfo.Grenade_ExplosionDelay,
                        bulletBehaviourInfo.Grenade_ExplosionDamageMultiplier * cAtk.GetCurrentAttackDamage,
                        bulletBehaviourInfo.Grenade_Defensible,
                        CharOwner.CharInfo.RivalSide,
                        null,
                        bulletBehaviourInfo.Grenade_CanBeDestroyed,
                        bulletBehaviourInfo.Grenade_Health,
                        bulletBehaviourInfo.Grenade_ExplosionTiles.ToArray(),
                        bulletBehaviourInfo.Grenade_DirtyBombEffects.ToArray(),
                        new DamageInfoClass(
                            CharOwner,
                            bullet,
                            cAtk.CurrentAttack,
                            new List<ScriptableObjectAttackEffect>(),
                            CharOwner.CharInfo.DamageStats.CurrentElemental, false, CharOwner.CharInfo.ClassType == CharacterClassType.Spellbinder &&
                            cAtk.CurrentAttack.AttackInput != AttackInputType.Weak, true, false, Vector3.zero, true),
                            cAtk.CurrentAttack.ParticlesInput);
                        bullet.Grenade.SetParticleSystems(bulletBehaviourInfo.Grenade_OverrideBulletEffects, cAtk.CurrentAttack.Particles.Right.BulletAddress, cAtk.CurrentAttack.Particles.Right.HitAddress, bulletBehaviourInfo.Grenade_ObjectPS, bulletBehaviourInfo.Grenade_ExplosionPS);
                    }
                    else
                    {
                        bullet.Grenade = new GrenadeSetupInfoScript(
                        CharOwner,
                        cAtk.CurrentAttack.Grenade_ExplosionDelay,
                        cAtk.CurrentAttack.Grenade_ExplosionDamageMultiplier * cAtk.GetCurrentAttackDamage,
                        cAtk.CurrentAttack.Grenade_Defensible,
                        CharOwner.CharInfo.RivalSide,
                        null,
                        cAtk.CurrentAttack.Grenade_CanBeDestroyed,
                        cAtk.CurrentAttack.Grenade_Health,
                        cAtk.CurrentAttack.Grenade_ExplosionTiles.ToArray(),
                        cAtk.CurrentAttack.Grenade_DirtyBombEffects.ToArray(),
                        new DamageInfoClass(
                            CharOwner,
                            bullet,
                            cAtk.CurrentAttack,
                            new List<ScriptableObjectAttackEffect>(),
                            CharOwner.CharInfo.DamageStats.CurrentElemental, false, CharOwner.CharInfo.ClassType == CharacterClassType.Spellbinder &&
                            cAtk.CurrentAttack.AttackInput != AttackInputType.Weak, true, false, Vector3.zero, true),
                            cAtk.CurrentAttack.ParticlesInput);
                        bullet.Grenade.SetParticleSystems(cAtk.CurrentAttack.Grenade_OverrideBulletEffects, cAtk.CurrentAttack.Particles.Right.BulletAddress, cAtk.CurrentAttack.Particles.Right.HitAddress, cAtk.CurrentAttack.Grenade_ObjectPS, cAtk.CurrentAttack.Grenade_ExplosionPS);
                    }
                }

                bullet.BulletTileEffects = item.IsEffectOnTile ? item.EffectsOnTile : null;
                bullet.BulletTileSummons = item.SpawnSummonOnTile ? item.SummonToSpawn : null;
                bullet.Damage = cAtk.GetCurrentAttackDamage * item.DamagePerc;
                bullet.bts = temp_bts;
                bullet.DestinationTile = tempVector2Int;
                bullet.transform.position = CharOwner.SpineAnim.FiringPints[(int)cAtk.CurrentAttack.AttackAnim].position;
                CompleteBulletSetup(cAtk, CharOwner.CharInfo.CharacterID);
            }
        }
        else
        {
            foreach (BattleFieldAttackTileClass item in bulletBehaviourInfo.BulletEffectTiles.Where(a => a.ToFire))
            {
                temp_bts = GetAttackTargetBattleTileScript(nextAttackPos, item, cAtk);
                if (temp_bts == null)
                    continue;

                ParticleManagerScript.Instance.FireAttackParticlesInPosition(cAtk.CurrentAttack.Particles.Right.HitAddress,
                cAtk.CurrentAttack.AttackOwner, AttackParticlePhaseTypes.Hit, temp_bts.transform.position, CharOwner.CharInfo.Facing, cAtk.CurrentAttack.ParticlesInput, cAtk.CurrentAttack.HitParticlesT, cAtk.CurrentAttack.HitResizeMultiplier);
                IDamageReceiver temp_dm = BattleManagerScript.Instance.GetDamageReceiverInPos(temp_bts.Pos);
                if (temp_dm != null)
                {
                    temp_cb = temp_dm.ReferenceCharacter;
                    temp_cb.SetDamage(new DamageInfoClass(CharOwner,
                        null,
                        cAtk.CurrentAttack,
                        item.Effects,
                        CharOwner.CharInfo.Elemental,
                        false,
                        false,
                        false,
                        false,
                        CharOwner.transform.position), 
                        cAtk.GetCurrentAttackDamage);
                }

                if (item.IsEffectOnTile)
                {
                    temp_bts.SetupEffect(item.EffectsOnTile, CharOwner, cAtk.CurrentAttack);
                }

                if (item.SpawnSummonOnTile)
                {
                    if (item.SummonToSpawn.SummonSpawnPositions.Length == 0) item.SummonToSpawn.SummonSpawnPositions = new Vector2Int[] { Vector2Int.zero };
                    BattleTileScript offsetBts = null;
                    foreach (Vector2Int spawnPos in item.SummonToSpawn.SummonSpawnPositions)
                    {
                        offsetBts = GridManagerScript.Instance.GetBattleTile(temp_bts.Pos + spawnPos);
                        if (offsetBts == null || (offsetBts.WalkingSide != WalkingSideType.Both && (cAtk.CurrentAttack.AttackTargetSide == AttackTargetSideType.EnemySide && offsetBts.WalkingSide == temp_bts.WalkingSide) ||
                                (cAtk.CurrentAttack.AttackTargetSide == AttackTargetSideType.FriendlySide && offsetBts.WalkingSide != temp_bts.WalkingSide))) continue;

                        offsetBts.SpawnSummonFromAttack(item.SummonToSpawn, CharOwner);
                    }
                }

                {
                    /*  if(item.ChildrenExplosion.Count > 0)
                        {
                            foreach (ParticlesChildExplosionClass child in item.ChildrenExplosion)
                            {
                                foreach (BattleFieldAttackChildTileClass childTile in child.BulletEffectTiles)
                                {
                                    tempVector2Int = CharOwner.CharInfo.CurrentTilePos + childTile.Pos;
                                    temp_bts = GridManagerScript.Instance.GetBattleTile(tempVector2Int);
                                    switch (cAtk.CurrentAttack.AttackTargetSide)
                                    {
                                        case AttackTargetSideType.EnemySide:
                                            if (temp_bts.WalkingSide == CharOwner.CharInfo.WalkingSide)
                                            {
                                                continue;
                                            }
                                            break;
                                        case AttackTargetSideType.FriendlySide:
                                            if (temp_bts.WalkingSide != CharOwner.CharInfo.WalkingSide)
                                            {
                                                continue;
                                            }
                                            break;
                                    }

                                    if (childTile.HasEffect)
                                    {
                                        IDamageReceiver temp_dm = BattleManagerScript.Instance.GetDamageReceiverInPos(temp_bts.Pos);
                                        if (temp_dm != null)
                                            temp_cb = temp_dm.ReferenceCharacter;
                                        else
                                            continue;
                                        if (temp_cb != null)
                                        {
                                            foreach (ScriptableObjectAttackEffect effect in childTile.Effects)
                                            {
                                                if (item != null)
                                                {
                                                    temp_cb.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(), ElementalType.Neutral, CharOwner, effect));
                                                }
                                            }
                                        }
                                    }


                                    if (childTile.IsEffectOnTile)
                                    {
                                        temp_bts.SetupEffect(childTile.EffectsOnTile);
                                    }

                                    if (childTile.SpawnSummonOnTile)
                                    {
                                        if (childTile.SummonToSpawn.SummonSpawnPositions.Length == 0) childTile.SummonToSpawn.SummonSpawnPositions = new Vector2Int[] { Vector2Int.zero };
                                        BattleTileScript offsetBts = null;
                                        foreach (Vector2Int spawnPos in childTile.SummonToSpawn.SummonSpawnPositions)
                                        {
                                            offsetBts = GridManagerScript.Instance.GetBattleTile(temp_bts.Pos + spawnPos);
                                            if (offsetBts == null || offsetBts.WalkingSide != temp_bts.WalkingSide) continue;

                                            offsetBts.SpawnSummonFromAttack(childTile.SummonToSpawn, CharOwner);
                                        }
                                    }
                                }

                            }
                        }*/
                }
            }
        }
    }

    BattleTileScript temp_bts;
    public void CreateTileAttack(Vector2Int nextAttackPos, CurrentAttackInfoClass cAtk)
    {
        if (cAtk.CurrentAttack != null && CharOwner.CharInfo.Health > 0 && CharOwner.IsOnField && cAtk.CurrentAttack.taPhase == TileAttackPhaseType.None)
        {
            cAtk.CurrentAttack.taPhase = TileAttackPhaseType.TileAttack;
            ChargingTime.Clear();
            bool hasChildren = cAtk.CurrentAttack.TilesAtk.BulletTrajectories.Where(r => r.HasABullet).ToList().Count > 1 ? false : true;
            for (int i = 0; i < cAtk.CurrentAttack.TilesAtk.BulletTrajectories.Count; i++)
            {
                cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].toFire = false;
                if (cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet || cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf || cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack)
                {
                    tempString = CharOwner.GetAttackAnimName(cAtk);
                    tempAnimation = CharOwner.SpineAnim.skeleton.Data.FindAnimation(tempString);
                    tempTimeLine = tempAnimation?.Timelines?.Items?.Where(r => r is Spine.EventTimeline).ToList();
                    if(tempTimeLine == null)
                    {
                        InteruptAttack();
                        return;
                    }
                    tempEventTimeLine = tempTimeLine.Where(r => ((Spine.EventTimeline)r).Events.Where(p => p.Data.Name == "FireBulletParticle").ToList().Count > 0).First() as Spine.EventTimeline;
                    tempFloat_1 = tempEventTimeLine.Events.Where(r => r.Data.Name == "FireBulletParticle").First().Time;
                    //CharOwner.StartCoroutine(TileAttackDelay(cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].Delay - ChargingTime, cAtk));
                    cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].toFire = cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf || (!cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet && cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack) ? false : true;
                }
                cAtk.shotsLeftInAttack = 1;
                foreach (BattleFieldAttackTileClass target in cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].BulletEffectTiles)
                {
                    tempInt_1 = Random.Range(0, 100);
                    target.ToFire = tempInt_1 <= cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].ExplosionChances;
                    if (target.ToFire)
                    {
                        if (cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget && GridManagerScript.Instance.IsPosOnField(nextAttackPos + (CharOwner.CharInfo.Side == TeamSideType.LeftSideTeam ? target.Pos : -target.Pos)))
                        {
                            tempVector2Int = nextAttackPos + (CharOwner.CharInfo.Side == TeamSideType.LeftSideTeam ? target.Pos : -target.Pos);
                        }
                        else if (cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf && GridManagerScript.Instance.IsPosOnField((CharOwner.CharInfo.Side == TeamSideType.LeftSideTeam ? target.Pos : -target.Pos) + CharOwner.CharInfo.CurrentTilePos))
                        {
                            tempVector2Int = (CharOwner.CharInfo.Side == TeamSideType.LeftSideTeam ? target.Pos : -target.Pos) + CharOwner.CharInfo.CurrentTilePos;
                        }
                        else if(cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget && !GridManagerScript.Instance.IsPosOnField(nextAttackPos + (CharOwner.CharInfo.Side == TeamSideType.LeftSideTeam ? target.Pos : -target.Pos)))
                        {
                            continue;

                            /*temp_bts = GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(nextAttackPos + target.Pos, CharOwner.CharInfo.Facing);
                            if (temp_bts != null)
                            {
                                tempVector2Int = temp_bts.Pos;
                            }
                            else
                            {
                                continue;
                            }*/
                        }

                        tempVector2Int = cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget || cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf ? tempVector2Int :
                        cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnRandom ? GridManagerScript.Instance.GetFreeBattleTile(
                        cAtk.CurrentAttack.AttackTargetSide == AttackTargetSideType.EnemySide ?
                        CharOwner.CharInfo.WalkingSide == WalkingSideType.LeftSide ? WalkingSideType.RightSide : WalkingSideType.LeftSide : CharOwner.CharInfo.WalkingSide).Pos : target.Pos;

                        temp_bts = GridManagerScript.Instance.GetBattleTile(tempVector2Int);

                        if (GridManagerScript.Instance.IsPosOnField(tempVector2Int))
                        {
                            if (temp_bts._BattleTileState != BattleTileStateType.NonUsable && temp_bts._BattleTileState != BattleTileStateType.Bound &&
                                (cAtk.CurrentAttack.AttackTargetSide == AttackTargetSideType.FriendlySide ? temp_bts.WalkingSide == CharOwner.CharInfo.WalkingSide :
                                cAtk.CurrentAttack.AttackTargetSide == AttackTargetSideType.EnemySide ? temp_bts.WalkingSide != CharOwner.CharInfo.WalkingSide : true) || cAtk.CurrentAttack.TilesAtk.CanAffectBothSide)
                            {

                                if (cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].IsIndicatingOntTile)
                                {
                                    if (cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet && cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack)
                                    {
                                        tempFloat_2 = ((((cAtk.CurrentAttack.AttackInput >= AttackInputType.Strong ? CharOwner.CharInfo.SpeedStats.StrongBulletS : CharOwner.CharInfo.SpeedStats.WeakBulletS) / 12)
                                            * 4) + (tempFloat_1)) * cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].TimeMultiplier;
                                    }
                                    else if (cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet)
                                    {
                                        tempFloat_2 = (((cAtk.CurrentAttack.AttackInput >= AttackInputType.Strong ? CharOwner.CharInfo.SpeedStats.StrongBulletS : CharOwner.CharInfo.SpeedStats.WeakBulletS) / 12)
                                            * (Mathf.Abs(CharOwner.CharInfo.CurrentTilePos.y - nextAttackPos.y)) + (tempFloat_1)) * cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].TimeMultiplier;
                                    }
                                    else if (!cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet && cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnAreaAttack)
                                    {
                                        tempFloat_2 = tempFloat_1;
                                    }
                                    else if (!cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet && cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf)
                                    {
                                        tempFloat_2 = tempFloat_1;
                                    }
                                    temp_bts.SetAttack(CharOwner, cAtk, target, cAtk.GetCurrentAttackDamage * target.DamagePerc, (i == 0 ? 0 : hasChildren ?
                                      cAtk.CurrentAttack.TilesAtk.BulletTrajectories[0].Delay : 0) + cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].Delay + tempFloat_2, cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet, cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].IsIndicatingOntTile);

                                }
                            }
                            else
                            {
                                target.ToFire = false;
                            }
                        }
                        else
                        {
                            target.ToFire = false;
                        }
                    }
                }

                if ((cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].HasABullet && cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].BulletEffectTiles.Where(r=> r.ToFire).ToList().Count > 0)
                    || cAtk.CurrentAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf)
                {
                    cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].toFire = true;
                    ChargingTime.Add(cAtk.CurrentAttack.TilesAtk.BulletTrajectories[i].Delay);
                }
            }

            if(ChargingTime.Count > 0)
            {
                ChargingTime = ChargingTime.Distinct().ToList();
            }


            if (cAtk.shotsLeftInAttack == 0)
            {
               InteruptAttack();
            }
            else
            {
                strongAttackTimer = 0;
            }
        }
    }

    private IEnumerator TileAttackDelay(float delay, CurrentAttackInfoClass cAtk)
    {

        //Debug.Log("DElay   " + Time.time + "    " + delay);
        yield return BattleManagerScript.Instance.WaitFor(delay, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause, ()=> cAtk.shotsLeftInAttack == 0);
        //Debug.Log("DElay   " + Time.time);
        if (cAtk.currentAttackPhase != AttackPhasesType.End)
        {
            cAtk.currentAttackPhase = AttackPhasesType.Firing;
        }
    }

    public override IEnumerator StartIdleToAtk(CurrentAttackInfoClass cAtk)
    {
         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }
    public override IEnumerator IdleToAtk(CurrentAttackInfoClass cAtk)
    {
         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }
    public override IEnumerator StartCharging(CurrentAttackInfoClass cAtk)
    {
         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }
    public override IEnumerator Charging(CurrentAttackInfoClass cAtk)
    {
         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
        strongAttackTimer += BattleManagerScript.Instance.DeltaTime * (CharOwner.CharInfo.SpeedStats.B_BaseSpeed * CharOwner.CharInfo.BaseSpeed);
        if (ChargingTime.Count == 0 || strongAttackTimer > ChargingTime[0] && CharOwner.CharInfo.Health > 0f)
        {
            cAtk.currentAttackPhase = AttackPhasesType.Firing;
            if(ChargingTime.Count > 0)
            {
                ChargingTime.RemoveAt(0);
            }
        }
    }
    public override IEnumerator StartLoop(CurrentAttackInfoClass cAtk)
    {
         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }
    public override IEnumerator Loop(CurrentAttackInfoClass cAtk)
    {
        if(ChargingTime.Count > 0)
        {
            if(cAtk.shotsLeftInAttack == 0)
            {
                cAtk.shotsLeftInAttack = 1;
            }
            strongAttackTimer += BattleManagerScript.Instance.DeltaTime;
        }

         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }
    public override IEnumerator StartAtkToIdle(CurrentAttackInfoClass cAtk)
    {
         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }
    public override IEnumerator AtkToIdle(CurrentAttackInfoClass cAtk)
    {
         yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }


}


