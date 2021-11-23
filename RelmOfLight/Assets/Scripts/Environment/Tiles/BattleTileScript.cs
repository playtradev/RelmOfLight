using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the component that take care of all the tile behaviours in the game
/// </summary>
public class BattleTileScript : MonoBehaviour
{

    public int TileHeatMapPoints;

    public bool test = false;
    public bool inout = true;

    public BattleTileStateType BattleTileState
    {
        get
        {
            return _BattleTileState;
        }
        set
        {
            if(_BattleTileState != BattleTileStateType.Blocked)
            {
                _BattleTileState = value;
            }
        }
    }



    public struct Edge
    {
        public Vector2Int TileOrigin;
        public Vector3[] _Vertices;
        public Vector3[] Vertices
        {
            get => _Vertices;
            set
            {
                if (value.Length != 2)
                {
                    Debug.LogError("Tried to set the edge vertices but was given more than 2 vertices which cannot be allowed!");
                    return;
                }
                _Vertices = value;
            }
        }
        public Vector3 Last => Vertices.Last();
        public Vector3 First => Vertices.First();


        public bool IsSharedBy(Edge otherEdge)
        {
            foreach (Vector3 point in Vertices)
            {
                if (!otherEdge.Vertices.Contains(point))
                    return false;
            }
            return true;
        }

        public Edge(Vector3 point1, Vector3 point2, Vector2Int tileOrigin)
        {
            _Vertices = new Vector3[] { point1, point2 };
            TileOrigin = tileOrigin;
        }
    }

    public Edge[] Edges
    {
        get
        {
            return new Edge[]
            {
                new Edge(transform.position + new Vector3(-0.5f, 0.5f, 0f), transform.position + new Vector3(0.5f, 0.5f, 0f), Pos),
                new Edge(transform.position + new Vector3(0.5f, 0.5f, 0f), transform.position + new Vector3(0.5f, -0.5f, 0f), Pos),
                new Edge(transform.position + new Vector3(0.5f, -0.5f, 0f), transform.position + new Vector3(-0.5f, -0.5f, 0f), Pos),
                new Edge(transform.position + new Vector3(-0.5f, -0.5f, 0f), transform.position + new Vector3(-0.5f, 0.5f, 0f), Pos)
            };
        }
    }

    public Vector2Int Pos;
    public BattleTileStateType _BattleTileState;
    //public BattleTileType BattleTileT;
    public WalkingSideType WalkingSide;
    public WalkingSideType B_WalkingSide;
    public SpriteRenderer SP;
    public PortalInfoClass PortalInfo;
    public BaseCharacter cb = null;
    public bool isCharAvailable = false;
    //public Vector2 TileADStats = Vector2.one;

    public bool isTaken = false;
    private CurrenrActiveBuffDebuff currentBuffDebuffPs;
    private TileEffectClass baseTileEffectC;
    //Private
    bool ContinueBaseEffect = false;

    private void Awake()
    {
        SP = GetComponent<SpriteRenderer>();
    }
    //Setup tile info
    public void SetupTileFromBattleTileInfo(BattleTileInfo info, TileEffectClass tileEffect = null)
    {
        WalkingSide = info.WalkingSide;
        B_WalkingSide = info.WalkingSide;
        BattleTileState = info.BattleTileState;
      
        //TileADStats = info.TileAD;
        ResetEffect();
        if (BattleTileState == BattleTileStateType.Empty && (info.HasEffect || tileEffect != null))
        {
            SetupBaseEffect(tileEffect != null && tileEffect.Effects.Count > 0 ? tileEffect : info.BaseEffectsOnTile);
        }
        else
        {
            StopBaseEffect();
        }

        //BattleTileT = info.BattleTileT;

        /*if (BattleTileT == BattleTileType.Portal)
        {
            PortalInfo = new PortalInfoClass(this, info.Portal, info.IDPortal);
            GridManagerScript.Instance.Portals.Add(PortalInfo);
        }*/
    }

    public void ResetEffect()
    {
        if (currentBuffDebuffPs != null)
        {
            currentBuffDebuffPs.Duration = 0;
            currentBuffDebuffPs.PsHelper?.StopPs();
            currentBuffDebuffPs = null;
        }
    }

    public ParticleHelperScript SetTilePs(TileEffectClass effect)
    {
        ParticleHelperScript particleHelper = ParticleManagerScript.Instance.FireParticlesInTransform(effect.TileParticlesID, transform).GetComponent<ParticleHelperScript>();
        particleHelper.UpdatePSTime(effect.DurationOnTile);
        particleHelper.gameObject.SetActive(true);
        particleHelper.gameObject.SetActive(false);
        particleHelper.gameObject.SetActive(true);
        return particleHelper;
    }
    StatsToAffectClass temp_STA;
    int temp_Int;
    float temp_float;
  /*  public void SetupEffect(TileEffectClass effect, BaseCharacter effectGiver, float baseDamage)
    {
        if (effect == null || effect.Effects.Count == 0 || Random.Range(0, 100) > effect.EffectChances)
        {
            return;
        }

        temp_STA = null;
        temp_Int = -1;
        for (int i = 0; i < effect.Effects.Count; i++)
        {
            if (effect.Effects[i].OldSystem)
            {
                if (effect.Effects[i].StatsToAffect == BuffDebuffStatsType.Tile_ChangeSide)
                {
                    temp_STA = new StatsToAffectClass(effect.Effects[i]);
                }
            }
            else
            {
                for (int a = 0; a < effect.Effects[i].StatsToAffectList.Count; a++)
                {
                    if (effect.Effects[i].StatsToAffectList[a].StatsToAffect == BuffDebuffStatsType.Tile_ChangeSide)
                    {
                        temp_STA = effect.Effects[i].StatsToAffectList[a];
                        temp_Int = i;
                    }
                }
            }
        }

        if(temp_STA != null && temp_Int != -1)
        {
            effect.Effects[temp_Int].StatsToAffectList.Remove(temp_STA);
        }


        if (currentBuffDebuffPs == null)
        {
            currentBuffDebuffPs = new CurrenrActiveBuffDebuff(new TileEffectClass(effect), effect.TileParticlesID != ParticlesType.None ? SetTilePs(effect) : null, baseDamage, effectGiver);
        }
        else
        {
            currentBuffDebuffPs.PsHelper?.StopPs();
            currentBuffDebuffPs.Duration = currentBuffDebuffPs.timer;
            currentBuffDebuffPs.Stop = true;
            currentBuffDebuffPs = new CurrenrActiveBuffDebuff(effect, effect.TileParticlesID != ParticlesType.None ? SetTilePs(effect) : null, baseDamage, effectGiver);
        }

        if(temp_STA != null)
        {
            Tile_ChangeSide = Tile_ChangeSideCo(currentBuffDebuffPs);
            StartCoroutine(Tile_ChangeSide);
        }
        StartCoroutine(EffectCo(currentBuffDebuffPs));
    }*/


    public void SetupEffect(TileEffectClass effect, BaseCharacter effectGiver, ScriptableObjectAttackBase atk)
    {
        if (effect == null || effect.Effects.Count == 0 || Random.Range(0, 100) > effect.EffectChances)
        {
            return;
        }

        if (currentBuffDebuffPs == null)
        {
            currentBuffDebuffPs = new CurrenrActiveBuffDebuff(new TileEffectClass(effect), effect.TileParticlesID != ParticlesType.None ? SetTilePs(effect) : null, effectGiver, atk);
        }
        else
        {
            currentBuffDebuffPs.PsHelper?.StopPs();
            currentBuffDebuffPs.Duration = currentBuffDebuffPs.timer;
            currentBuffDebuffPs.Stop = true;
            currentBuffDebuffPs = new CurrenrActiveBuffDebuff(effect, effect.TileParticlesID != ParticlesType.None ? SetTilePs(effect) : null, effectGiver, atk);
        }
        StartCoroutine(EffectCo(currentBuffDebuffPs));
    }

    public void SpawnSummonFromAttack(TileSummonClass summon, BaseCharacter Summoner)
    {
        if (summon == null || summon.CharToSummon == CharacterNameType.None || Random.Range(0, 100) > summon.SpawnChances || _BattleTileState != BattleTileStateType.Empty)
        {
            return;
        }

        StartCoroutine(ItemSpawnerManagerScript.Instance.SpawnSummon(summon.CharToSummon, this, Summoner, !summon.UncappedDuration ? Random.Range(summon.DurationOnField.x, summon.DurationOnField.y) : 0f, summon.hasCharOverrides ? summon.CharOverrides : null));
    }

    /* private IEnumerator Tile_ChangeSideCo(CurrenrActiveBuffDebuff effect)
     {
         prevSide = WalkingSide;
         WalkingSide = effect.EffectMaker.CharInfo.WalkingSide;
         SP.color = WalkingSide == WalkingSideType.LeftSide ? BattleManagerScript.Instance.LeftSide_Color : BattleManagerScript.Instance.RightSide_Color;
         float timer = 0;


         while (effect.timer <= effect.Duration)
         {
             yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

             timer += BattleManagerScript.Instance.DeltaTime;
         }

         WalkingSide = prevSide;
         SP.color = WalkingSide == WalkingSideType.LeftSide ? BattleManagerScript.Instance.LeftSide_Color : BattleManagerScript.Instance.RightSide_Color;
         if (cb != null && cb.CharInfo.WalkingSide != WalkingSide && cb.CharInfo.WalkingSide != WalkingSideType.Both)
         {
             cb.Buff_DebuffCo(cb, BattleManagerScript.Instance.BaseTeleport, 0, false);
         }
     }

     private IEnumerator EffectCo(CurrenrActiveBuffDebuff effect)
     {
         List<BaseCharacter> charsEffected = new List<BaseCharacter>();
         float ticker = 0;
         if (cb != null)
         {
             foreach (ScriptableObjectAttackEffect eff in effect.Effect.Effects)
             {
                 //Creation of the Buff/Debuff
                 if(cb.CanBeAffectedByTilesEffect)
                 {
                     cb.Buff_DebuffCo(effect.EffectMaker, eff, currentBuffDebuffPs.BaseDamage, !charsEffected.GridFight_ContainsStruct(cb));
                 }
             }

             if (!charsEffected.GridFight_ContainsStruct(cb)) charsEffected.Add(cb);
         }


         while (effect.timer <= effect.Duration)
         {
             yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
             if(cb != null && effect.Effect.TileAction == TileActionType.OverTime && ticker >= effect.Effect.HitTime)
             {
                 ticker = 0;

                 foreach (ScriptableObjectAttackEffect eff in effect.Effect.Effects)
                 {
                     if (cb.CanBeAffectedByTilesEffect)
                     {
                         cb.Buff_DebuffCo(effect.EffectMaker, eff, currentBuffDebuffPs.BaseDamage, !charsEffected.GridFight_ContainsStruct(cb));
                     }
                 }

                 if (!charsEffected.GridFight_ContainsStruct(cb)) charsEffected.Add(cb);
             }

             effect.timer += BattleManagerScript.Instance.DeltaTime;
             ticker += BattleManagerScript.Instance.DeltaTime;
         }

         if (!effect.Stop)
         {
             currentBuffDebuffPs = null;
         }
     }
     */

    float ticker = 0;

    private IEnumerator EffectCo(CurrenrActiveBuffDebuff effect)
    {
        List<BaseCharacter> charsEffected = new List<BaseCharacter>();
        //  bool blockTile = effect.Effect.Effects.Where(r => r.StatsToAffect == BuffDebuffStatsType.Tile_Blocked).ToList().Count > 0;
        for (int i = 0; i < effect.Effect.Effects.Count; i++)
        {
            if (effect.Effect.Effects[i].OldSystem)
            {
                if(!effect.Effect.Effects[i].StatsToAffect.ToString().Contains("Tile"))
                {
                    TileHeatMapPoints += (effect.Effect.Effects[i].classification == StatusEffectType.Buff ? 1 : -1) * effect.Effect.Effects[i].level;
                }
            }
            else
            {
                for (int a = 0; a < effect.Effect.Effects[i].StatsToAffectList.Count; a++)
                {
                    if (!effect.Effect.Effects[i].StatsToAffectList[a].StatsToAffect.ToString().Contains("Tile"))
                    {
                        TileHeatMapPoints += (effect.Effect.Effects[i].classification == StatusEffectType.Buff ? 1 : -1) * effect.Effect.Effects[i].level;
                    }
                }
            }
        }

        if (isCharAvailable && cb != null && !cb.died && cb.CharInfo.HealthPerc > 0)
        {
            foreach (ScriptableObjectAttackEffect eff in effect.Effect.Effects)
            {
                //Creation of the Buff/Debuff
                if (cb.CanBeAffectedByTilesEffect)
                {
                    cb.Buff_DebuffCo(effect.EffectMaker, eff, currentBuffDebuffPs.Atk, !charsEffected.GridFight_Contains(cb));
                }
            }

            if (!charsEffected.GridFight_Contains(cb)) charsEffected.Add(cb);
        }


        while (effect.timer <= effect.Duration)
        {
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            if (isCharAvailable && cb != null && cb.CharInfo.Pos.GridFight_ContainsStruct(Pos) && effect.Effect.TileAction == TileActionType.OverTime && ticker >= effect.Effect.HitTime && !cb.died && cb.CharInfo.HealthPerc > 0)
            {
                ticker = 0;
                foreach (ScriptableObjectAttackEffect eff in effect.Effect.Effects)
                {
                    if (cb.CanBeAffectedByTilesEffect)
                    {
                        cb.Buff_DebuffCo(effect.EffectMaker, eff, currentBuffDebuffPs.Atk, !charsEffected.GridFight_Contains(cb));
                    }
                }

                if (!charsEffected.GridFight_Contains(cb)) charsEffected.Add(cb);
            }

            effect.timer += BattleManagerScript.Instance.DeltaTime;
            ticker += BattleManagerScript.Instance.DeltaTime;
        }

        for (int i = 0; i < effect.Effect.Effects.Count; i++)
        {
            if (effect.Effect.Effects[i].OldSystem)
            {
                if (!effect.Effect.Effects[i].StatsToAffect.ToString().Contains("Tile"))
                {
                    TileHeatMapPoints += (effect.Effect.Effects[i].classification == StatusEffectType.Buff ? -1 : 1) * effect.Effect.Effects[i].level;
                }
            }
            else
            {
                for (int a = 0; a < effect.Effect.Effects[i].StatsToAffectList.Count; a++)
                {
                    if (!effect.Effect.Effects[i].StatsToAffectList[a].StatsToAffect.ToString().Contains("Tile"))
                    {
                        TileHeatMapPoints += (effect.Effect.Effects[i].classification == StatusEffectType.Buff ? -1 : 1) * effect.Effect.Effects[i].level;
                    }
                }
            }
        }


        if (!effect.Stop)
        {
            currentBuffDebuffPs = null;
        }
    }



    public void SetupBaseEffect(TileEffectClass effect)
    {
        if (effect == null || effect.Effects.Count == 0 || Random.Range(0, 100) > effect.EffectChances)
        {
            return;
        }
       
        StartCoroutine(BaseEffectCo(effect));
    }
    public void StopBaseEffect()
    {
        ContinueBaseEffect = false;
        baseTileEffectC = null;
    }

    private IEnumerator BaseEffectCo(TileEffectClass effect)
    {
        ContinueBaseEffect = true;
        baseTileEffectC = effect;
        List<BaseCharacter> charsEffected = new List<BaseCharacter>();
        float ticker = 0;

        while (ContinueBaseEffect)
        {
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            if (isCharAvailable && cb != null && effect.TileAction == TileActionType.OverTime && ticker >= effect.HitTime)
            {
                ticker = 0;

                foreach (ScriptableObjectAttackEffect eff in effect.Effects)
                {
                    if (cb.CanBeAffectedByTilesEffect)
                    {
                        cb.Buff_DebuffCo(cb, eff, null, !charsEffected.GridFight_Contains(cb));
                    }
                }

                if (!charsEffected.GridFight_Contains(cb)) charsEffected.Add(cb);
            }

            ticker += BattleManagerScript.Instance.DeltaTime;
        }
    }
    //Reset the tile to the default values
    public void ResetTile()
    {
        BattleTileState =  BattleTileStateType.NonUsable;
        //BattleTileT =  BattleTileType.Base;
        //SP.color = Color.white;
    }


    #if !UNITY_SWITCH && !UNITY_XBOXONE
   /* private void Update()
    {
        if(test)
        {
            test = false;
            //SetTileColor(inout);
        }
    }*/
#endif

    BaseCharacter temp_cb;
    private void OnTriggerEnter(Collider other)
    {
        //If collides with a character and the tile type is not base
        if (other.tag.Contains("Side"))//&& BattleTileT != BattleTileType.Base
        {
            temp_cb = other.GetComponentInParent<BaseCharacter>();

            if(temp_cb != null && temp_cb.CharInfo.Pos.GridFight_ContainsStruct(Pos) && temp_cb.isMoving)
            {
                cb = temp_cb;
                //Subscribe to the TargetCharacter_TileMovementCompleteEvent event
                cb.TileMovementCompleteEvent += TargetCharacter_TileMovementCompleteEvent;
            }
            else if(temp_cb != null && temp_cb.CharInfo.Pos.GridFight_ContainsStruct(Pos))
            {
                cb = temp_cb;
                TargetCharacter_TileMovementCompleteEvent(cb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("Side"))
        {
            temp_cb = other.GetComponentInParent<BaseCharacter>();
            if(temp_cb == cb)
            {
               // Debug.LogError("Cleaning   " + cb.CharInfo.CharacterID);
                cb = null;
                isCharAvailable = false;
            }
        }
    }

    //Setup the Tile effect
    private void TargetCharacter_TileMovementCompleteEvent(BaseCharacter movingChar)
    {
        if(movingChar != cb)
        {
            return;
        }

        if(movingChar == null)
        {

        }

        isCharAvailable = true;
        cb.TileMovementCompleteEvent -= TargetCharacter_TileMovementCompleteEvent;
        if(currentBuffDebuffPs != null && !movingChar.died && movingChar.CharInfo.HealthPerc > 0)
        {
            foreach (ScriptableObjectAttackEffect effect in currentBuffDebuffPs.Effect.Effects)
            {
                //Creation of the Buff/Debuff
                if (movingChar.CanBeAffectedByTilesEffect)
                {
                    ticker = 0;
                    movingChar.Buff_DebuffCo(currentBuffDebuffPs.EffectMaker, effect, currentBuffDebuffPs.Atk);
                }
            }

            if (currentBuffDebuffPs.Effect.TileAction == TileActionType.SingleHit)
            {
                currentBuffDebuffPs.PsHelper?.StopPs();
                currentBuffDebuffPs.Duration = currentBuffDebuffPs.timer;
                currentBuffDebuffPs = null;
            }
        }

        if (baseTileEffectC != null)
        {
            foreach (ScriptableObjectAttackEffect effect in baseTileEffectC.Effects)
            {
                //Creation of the Buff/Debuff
                if (movingChar.CanBeAffectedByTilesEffect)
                {
                    movingChar.Buff_DebuffCo(null, effect, null);
                }
            }
        }
    }


    GameObject effect = null;
    IDamageReceiver target = null;
    public void SetAttack(BaseCharacter attacker, CurrentAttackInfoClass nextAttack, BattleFieldAttackTileClass atkEffects, float damage, float duration, bool hasBullet = true, bool isDamageTileBased = false)
    {
        StartCoroutine(FireTarget(attacker, nextAttack, atkEffects, damage, hasBullet, isDamageTileBased));
    }

    private IEnumerator FireTarget(BaseCharacter attacker, CurrentAttackInfoClass nextAttack, BattleFieldAttackTileClass atkEffects, float damage, bool hasBullet = true, bool isDamageTileBased = false)
    {
        if (nextAttack == null || nextAttack.CurrentAttack == null || nextAttack.CurrentAttack.TilesAtk == null)
        {

        }

        float startTime = Time.time;

        yield return LerpTargetDisplay(0.1f, () => ((attacker.died || attacker.currentAttackProfile.isCurrentAttackInterupted(nextAttack.CurrentAttack.AttackInput)) &&
        (attacker.currentAttackProfile.CurrentAttackPhase(nextAttack.CurrentAttack.AttackInput) < AttackPhasesType.Shoot)) || BattleManagerScript.Instance.CurrentBattleState == BattleState.WaveEnd ||
                     BattleManagerScript.Instance.isSkillHappening.Value, attacker, nextAttack);

        effect = ParticleManagerScript.Instance.FireAttackParticlesInPosition(nextAttack.CurrentAttack.Particles.Right.HitAddress, attacker.IsStealingOn ? attacker.StolenAttackOwner : attacker.CharInfo.CharacterID,
                    AttackParticlePhaseTypes.Hit, transform.position, attacker.CharInfo.Facing, nextAttack.CurrentAttack.ParticlesInput, nextAttack.CurrentAttack.HitParticlesT, nextAttack.CurrentAttack.HitResizeMultiplier);
        effect.SetActive(true);
        target = BattleManagerScript.Instance.GetDamageReceiverInPos(Pos);
        if (target != null)
        {
            if (isDamageTileBased && !target.ReferenceCharacter.HasBuffDebuff(BuffDebuffStatsType.ShadowForm))
            {
                target.SetDamage(
                    new DamageInfoClass(attacker,
                    null,
                    nextAttack.CurrentAttack,
                    (atkEffects.HasEffect && Random.Range(0, 100) <= atkEffects.EffectChances ? atkEffects.Effects.Where(r => !r.StatsToAffect.ToString().Contains("Tile")).ToList() : new List<ScriptableObjectAttackEffect>()),
                    attacker.CharInfo.Elemental,
                    false,
                    false,
                    false,
                    false,
                    target.ReferenceCharacter != null ? target.ReferenceCharacter.SpineAnim.transform.position : target.ReceiverGO.transform.position),
                    damage);
            }
        }

        if (atkEffects.IsEffectOnTile)
        {
            SetupEffect(atkEffects.EffectsOnTile, attacker, nextAttack.CurrentAttack);
        }
        if (atkEffects.SpawnSummonOnTile)
        {
            if (atkEffects.SummonToSpawn.SummonSpawnPositions.Length == 0) atkEffects.SummonToSpawn.SummonSpawnPositions = new Vector2Int[] { Vector2Int.zero };
            BattleTileScript offsetBts = null;
            foreach (Vector2Int spawnPos in atkEffects.SummonToSpawn.SummonSpawnPositions)
            {
                offsetBts = GridManagerScript.Instance.GetBattleTile(Pos + spawnPos);
                if (offsetBts == null) continue;

                offsetBts.SpawnSummonFromAttack(atkEffects.SummonToSpawn, attacker);
            }
        }
    }

    public IEnumerator LerpTargetDisplay(float duration, System.Func<bool> breakCheck, BaseCharacter attacker, CurrentAttackInfoClass attackerPhase)
    {

        float timer = 0f;

        while (timer < duration)
        {
            yield return null;

            timer += BattleManagerScript.Instance.DeltaTime * (attackerPhase != null &&
                attackerPhase.currentAttackPhase <= AttackPhasesType.Firing ? attacker.CharInfo.BaseSpeed : 1);
            //Debug.Log(timer);
            if (breakCheck())
            {
                if (timer < duration)
                {

                }
                yield break;
            }
        }
    }

}

[System.Serializable]
public class CurrenrActiveBuffDebuff
{
    public TileEffectClass Effect;
    public ParticleHelperScript PsHelper;
    public float timer = 0;
    public float Duration = 0;
    public float BaseDamage
    {
        get
        {
            return EffectMaker.GetBaseDamage(Atk.AttackInput);
        }
    }
    public bool Stop = false;
    public BaseCharacter EffectMaker;
    public ScriptableObjectAttackBase Atk;

    public CurrenrActiveBuffDebuff()
    {

    }
     
    public CurrenrActiveBuffDebuff(TileEffectClass effect, ParticleHelperScript psHelper, BaseCharacter effectMaker, ScriptableObjectAttackBase atk)
    {
        Effect = effect;
        PsHelper = psHelper;
        Duration = effect.DurationOnTile;
        EffectMaker = effectMaker;
        Atk = atk;
    }
}