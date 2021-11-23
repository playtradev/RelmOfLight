using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScriptableObjectBaseCharacterBaseAttack : ScriptableObjectBaseCharacterAction
{

    public ScriptableObjectAttackEffect[] abAtkBase;
    
    [HideInInspector]
    public CurrentAttackInfoClass temp_caic = null;
    CurrentAttackInfoClass catk;

    #region Audio

    #endregion

    #region WeakAtk
    #endregion

    #region Strong Attack
    public float strongAttackTimer = 0f;
    public bool isStrongChargingParticlesOn = false;
    public bool isStrongLoading
    {
        get
        {
            return isStrongStop ? false : _isStrongLoading;
        }
        set
        {
            _isStrongLoading = value;
        }
    }
    public bool _isStrongLoading = false;
    public bool isStrongStop = false;
    #endregion

    #region Phase
   
    public virtual bool Attacking
    {
        get
        {
            return AllAttacks.Where(r=> r.currentAttackPhase != AttackPhasesType.End).FirstOrDefault() != null;
        }
    }
   
    #endregion

    #region Temp
    [HideInInspector] public ScriptableObjectAttackBase atkToCheck;
    [HideInInspector] public BulletScript bullet = null;
    protected GameObject tempGameObject = null;
    #endregion

    public List<CurrentAttackInfoClass> AllAttacks = new List<CurrentAttackInfoClass>();

    public CurrentAttackInfoClass CurrentAttack(string value)
    {
        return AllAttacks.Where(r => value.Contains(r.CurrentAttack.PrefixAnim.ToString())).FirstOrDefault();
    }

    public AttackPhasesType CurrentAttackPhase(AttackInputType inputT)
    {
        catk = AllAttacks.Where(r => r.CurrentAttack.AttackInput == inputT).FirstOrDefault();
        if (catk != null)
        {
            return catk.currentAttackPhase;
        }
        return AttackPhasesType.End;
    }
    public bool isCurrentAttackInterupted(AttackInputType inputT)
    {
        catk = AllAttacks.Where(r => r.CurrentAttack.AttackInput == inputT).FirstOrDefault();
        if (catk != null)
        {
            return catk.isAttackInterupt;
        }
        return true;
    }

    public CurrentAttackInfoClass CurrentAttack(AttackInputType inputT)
    {
        return AllAttacks.Where(r => r.CurrentAttack.AttackInput == inputT).FirstOrDefault();
    }

    public void InterruptSelectedAttack(AttackInputType inputT)
    {
        catk = AllAttacks.Where(r => r.CurrentAttack.AttackInput == inputT).FirstOrDefault();
        if(catk != null)
        {
            catk.InteruptAttack();
        }
    }

    public virtual IEnumerator AttackSequence()
    {
        yield return null;
    }

    public virtual IEnumerator Attack(ScriptableObjectAttackBase atk, bool isFungus = false)
    {

        //Debug.LogError(atk.AttackInput + "   0");
        temp_caic = CurrentAttack(AttackInputType.Strong);
        if (temp_caic != null)
        {
            //Debug.LogError(atk.AttackInput + "   1");
            yield break;
        }
        else
        {
            //Debug.LogError(atk.AttackInput + "   2");
            temp_caic = CurrentAttack(AttackInputType.Skill1);
            if (temp_caic != null)
            {
                //Debug.LogError(atk.AttackInput + "   3");
                yield break;
            }
            else
            {
                //Debug.LogError(atk.AttackInput + "   4");
                temp_caic = CurrentAttack(AttackInputType.Skill2);
                if (temp_caic != null)
                {
                    //Debug.LogError(atk.AttackInput + "   5");
                    yield break;
                }
            }
        }

        //Debug.LogError(atk.AttackInput + "   6");
        temp_caic = CurrentAttack(AttackInputType.Weak);
        if (temp_caic != null && atk.AttackInput > AttackInputType.Weak)
        { 
            //Debug.LogError(atk.AttackInput + "   7");
            temp_caic.InteruptAttack(false);
        }


        CurrentAttackInfoClass currentAtk = AllAttacks.Where(r => r.CurrentAttack.AttackInput == atk.AttackInput).FirstOrDefault();
        if (currentAtk == null)// || (currentAtk.currentAttackPhase == AttackPhasesType.Reset))
        {
            if (currentAtk != null)
            {
                CharOwner.StopCoroutine(currentAtk.AttackCo(isFungus));
                AllAttacks.Remove(currentAtk);
                if(currentAtk.CurrentAttack.AttackInput == AttackInputType.Weak)
                {
                    currentAtk.currentAttackPhase = AttackPhasesType.Charging;
                }
                else
                {
                    currentAtk.currentAttackPhase = AttackPhasesType.End;
                }
            }
            currentAtk = new CurrentAttackInfoClass(atk, CharOwner.currentAttackProfile);
            //Debug.Log(1 + "     " + Time.time);
            currentAtk.shotsLeftInAttack = 1;
         
            AllAttacks.Add(currentAtk);
           // Debug.LogError(atk.AttackInput + "   10");
            yield return currentAtk.AttackCo(isFungus);
            AllAttacks.Remove(currentAtk);
        }
        else if (currentAtk.CurrentAttack.AttackInput == AttackInputType.Weak)
        {
            //Debug.Log(1);
            currentAtk.shotsLeftInAttack = currentAtk.shotsLeftInAttack == 1 ? 2 : 1;
        }
    }

    public virtual IEnumerator StartCharging(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));

    }

    public virtual IEnumerator Charging(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }

    public virtual IEnumerator StartIdleToAtk(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));

    }

    public virtual IEnumerator IdleToAtk(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }

    public virtual IEnumerator StartLoop(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));

    }

    public virtual IEnumerator Loop(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }

    public virtual IEnumerator StartAtkToIdle(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));

    }

    public virtual IEnumerator AtkToIdle(CurrentAttackInfoClass cAtk)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || (BattleManagerScript.Instance.isSkillHappening.Value && !CharOwner.isMaskSkillBoss));
    }

    public virtual void CastAttackParticles(CurrentAttackInfoClass cAtk)
    {
        if(cAtk == null)
        {
            return;
        }

        if (cAtk.CurrentAttack == null) return;

        tempGameObject = ParticleManagerScript.Instance.FireAttackParticlesInPosition(cAtk.CurrentAttack.Particles.Right.CastAddress, cAtk.CurrentAttack.AttackOwner, AttackParticlePhaseTypes.Cast,
        CharOwner.SpineAnim.FiringPints[(int)cAtk.CurrentAttack.AttackAnim].position, CharOwner.CharInfo.Facing, cAtk.CurrentAttack.ParticlesInput, HitParticlesType.Normal, 1);
        //tempGameObject.GetComponent<ParticleHelperScript>().SetSimulationSpeed(CharOwner.CharInfo.BaseSpeed);
    }

    public virtual void CreateAttack(Vector2Int nextAttackPos, CurrentAttackInfoClass cAtk)
    {
        if (cAtk.CurrentAttack == null)
        {
            return;
        }
    }



    public virtual BattleTileScript[] GetAttackTargetBattleTileScripts(CurrentAttackInfoClass cAtk, bool includeChildrenExplosions = false)
    {
        return new BattleTileScript[0];
    }

    public virtual BattleTileScript[] GetAttackTargetBattleTileScripts(Vector2Int nextAttackPos, CurrentAttackInfoClass cAtk, bool includeChildrenExplosions = false)
    {
        return new BattleTileScript[0];
    }

    public virtual BattleTileScript GetAttackTargetBattleTileScript(BulletBehaviourInfoClass bulletBehaviourInfo, CurrentAttackInfoClass cAtk)
    {
        return null;
    }

    public virtual BattleTileScript GetAttackTargetBattleTileScript(Vector2Int nextAttackPos, BattleFieldAttackTileClass bulletBehaviourInfo, CurrentAttackInfoClass cAtk)
    {
        return null;
    }

    public virtual void CreateBullet(BulletBehaviourInfoClass bulletBehaviourInfo, CurrentAttackInfoClass cAtk)
    {
    }

    public virtual void CreateBullet(Vector2Int nextAttackPos, BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo, CurrentAttackInfoClass cAtk)
    {
    }

    public void CompleteBulletSetup(CurrentAttackInfoClass cAtk, CharacterNameType characterID, BaseCharacter lockOnTarget = null)
    {
        if (CharOwner.HasBuffDebuff(BuffDebuffStatsType.Backfire))
        {
            CharOwner.BackfireEffect(cAtk);
            return;
        }
        bullet.CharacterID =  cAtk.CurrentAttack.AttackOwner;
        bullet.SOAttack = cAtk.CurrentAttack;
        bullet.Facing = CharOwner.CharInfo.Facing;
        bullet.Elemental = CharOwner.CharInfo.Elemental;
        bullet.Side = CharOwner.CharInfo.Side;
        bullet.CharOwner = CharOwner;
        bullet.IsFungus = BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets ? true : false;
        BulletType[] temp_bulletT;


        if (bullet.BulletBehaviourInfo != null && bullet.BulletBehaviourInfo.OverrideBulletInfo)
        {
            temp_bulletT = new BulletType[bullet.BulletBehaviourInfo.BulletT.Count];
            bullet.BulletBehaviourInfo.BulletT.CopyTo(temp_bulletT);
            bullet.BulletT = temp_bulletT.ToList();
        }
        else if (bullet.BulletBehaviourInfoTile != null && bullet.BulletBehaviourInfoTile.OverrideBulletInfo)
        {
            temp_bulletT = new BulletType[bullet.BulletBehaviourInfoTile.BulletT.Count];
            bullet.BulletBehaviourInfoTile.BulletT.CopyTo(temp_bulletT);
            bullet.BulletT = temp_bulletT.ToList();
        }
        else
        {
            temp_bulletT = new BulletType[bullet.SOAttack.BulletT.Count];
            bullet.SOAttack.BulletT.CopyTo(temp_bulletT);
            bullet.BulletT = temp_bulletT.ToList();
        }

        for (int i = 0; i < CharOwner.CharInfo.BulletTypeModifier.Count; i++)
        {
            if (CharOwner.CharInfo.BulletTypeModifier[i].InputType == cAtk.CurrentAttack.AttackInput)
            {
                bullet.BulletT.Add(CharOwner.CharInfo.BulletTypeModifier[i].BulletTypeModifier);
            }
        }

        bullet.BulletT.Distinct();

        bullet.gameObject.SetActive(true);

        //TEMPORARY GETTING TARGET, SHOULD HAPPEN BEFORE THIS
        bullet.targettingType = cAtk.CurrentAttack.IsHoming ? BulletScript.TargettingType.Missile : BulletScript.TargettingType.Point;
        if(lockOnTarget == null && bullet.targettingType == BulletScript.TargettingType.Missile)
        {
            BaseCharacter[] potentialTargets = BattleManagerScript.Instance.TeamInfo[CharOwner.CharInfo.Side].enemiesCharactersOnField;
            lockOnTarget = potentialTargets.Length > 0 ? potentialTargets[Random.Range(0, potentialTargets.Length)] : null;
        }
        bullet.TargetCharacter = lockOnTarget;
        //===================

        bullet.StartMoveToTile();
    }

    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        return false;
    }

    public virtual void InteruptAttack(bool toIdle = true)
    {
        for (int i = 0; i < AllAttacks.Count; i++)
        {
            AllAttacks[i].InteruptAttack(toIdle);
        }
        AllAttacks.Clear();
    }

    public override void SetAttackReady(bool value)
    {
        InteruptAttack();
        base.SetAttackReady(value);
    }

    public override void SetCharDead()
    {
        InteruptAttack();
        base.SetCharDead();
    }
   
}
