using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharaqcterInput/AIInput")]
public class ScriptableObjectAIInput : ScriptableObjectBaseCharacterInput
{

    //AI STATE
    public IEnumerator AICo = null;

    protected BaseCharacter target = null;

    private float HPperc = 100;
  

    //PATHFINDING
    public BattleTileScript _possiblePos = null;
    public BattleTileScript possiblePos
    {
        get
        {
            return _possiblePos;
        }
        set
        {
            //Debug.LogError(value);
            _possiblePos = value;
        }
    }
    public List<BattleTileScript> possiblePositions = new List<BattleTileScript>();

    //Temp
    ScriptableObjectAttackBase tempAtk = null;

    public override void SetUpEnteringOnBattle(CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving, bool loop = false)
    {
        target = null;
        CharOwner.SetAnimation(anim, loop);
        base.SetUpEnteringOnBattle(anim, loop);
    }

    public override void CharArrivedOnBattleField(bool overrideAnimAndPos = true, CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving)
    {
        target = null;
        if (overrideAnimAndPos)
        {
            CharOwner.SetAnimation(anim, anim == CharacterAnimationStateType.Idle);
            CharOwner.StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(0, CharOwner, GridManagerScript.Instance.GetBattleTile(CharOwner.CharInfo.CurrentTilePos).transform.position));
        }
    }


    public override void SetUpLeavingBattle(bool withAudio = true)
    {
        target = null;
       
        if (CharOwner.SpineAnim.HasAnimation("Reverse_Arriving"))
        {
            CharOwner.SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
           
        }
        else
        {
            BattleManagerScript.Instance.StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(0.1f, CharOwner, new Vector3(100, 100, 100)));
            CharOwner.SpineAnim.CurrentAnim = "";
            CharOwner.SetAnimation(CharacterAnimationStateType.Idle);
            CharOwner.SetAttackReady(false);
        }
        base.SetUpLeavingBattle();
    }

    public override void StartInput()
    {
        if (AICo == null)
        {
            AICo = AI();
            CharOwner.StartCoroutine(AICo);
        }
    }
    public override void Reset()
    {
        base.Reset();
    }


    public override void EndInput()
    {
        if (possiblePos != null)
        {
            possiblePos.isTaken = false;
            possiblePos = null;
        }
        if (AICo != null)
        {
            CharOwner.StopCoroutine(AICo);
            AICo = null;
        }
    }

    public override void SetupCharacterSide(bool restoreChar = true)
    {
        base.SetupCharacterSide(restoreChar);
    }
 
    IEnumerator AIAttackAction(ScriptableObjectAttackBase currentAtk)
    {
        yield return CharOwner.currentAttackProfile.Attack(currentAtk);
        while (CharOwner.HasBuffDebuff(BuffDebuffStatsType.MeleeAttack))
        {
            yield return null;
        }
    }

    float actionoffset = 0;
   
    public virtual IEnumerator AI()
    {
        bool val = true;
        while (val)
        {
            yield return null;

            if (CharOwner.IsOnField && !CharOwner.died && CharOwner.CanAttack && !BattleManagerScript.Instance.isSkillHappening.Value)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle ||
                    CharOwner.CharInfo.BaseSpeed == 0 || (Time.time < (actionoffset + CharOwner.CharInfo.SpeedStats.CurrentActionTime)
                    && !UseStrong && !UseDir))
                {
                    yield return null;
                }

                if (target == null || target.CharInfo.HealthPerc <= 0 || target.died || !target.IsOnField)
                {
                    tempAtk = null;
                    tempAtk = GetRandomAttack(out target);
                }

                if (!UseDir && (target != null || UseStrong))
                {

                    if (UseStrong)
                    {
                        tempAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r => r.AttackInput == AttackInputType.Strong);
                        target = null;
                    }
                    else
                    {
                        nextAttackPos = target.CharInfo.CharacterID == CharacterNameType.CrystalLeft || target.CharInfo.CharacterID == CharacterNameType.CrystalRight ?
                            new Vector2Int(CharOwner.CharInfo.CurrentTilePos.x, target.CharInfo.CurrentTilePos.y) : target.CharInfo.CurrentTilePos;
                    }

                    yield return AIAttackAction(tempAtk);
                    if (tempAtk.AttackInput == AttackInputType.Strong && UseStrong)
                    {
                        UseStrong = false;
                    }
                    actionoffset = Time.time;
                }
                else
                {

                    if (CharOwner.CharActionlist.Contains(CharacterActionType.Move))
                    {
                        if (UseDir)
                        {
                            UseDir = false;
                            possiblePos = GridManagerScript.Instance.GetBattleTile(CharOwner.currentMoveProfile.GetDirectionVectorAndAnimationCurve(NewDir));
                            if (possiblePos != null && !possiblePos.isTaken && possiblePos.BattleTileState == BattleTileStateType.Empty)
                            {
                                possiblePos.isTaken = true;
                                yield return CharOwner.currentMoveProfile.StartMovement(possiblePos.Pos);
                                actionoffset = Time.time;
                            }
                           
                        }
                        else
                        {
                            possiblePositions = CharOwner.currentMoveProfile.GetPossibleTiles();
                            if (possiblePositions.Count > 0)
                            {
                                possiblePos = possiblePositions.First();

                                if (possiblePos.BattleTileState == BattleTileStateType.Empty && !possiblePos.isTaken)
                                {
                                    possiblePos.isTaken = true;

                                    yield return BattleManagerScript.Instance.WaitFor(CharOwner.CharInfo.SpeedStats.ReactionTimeValue, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

                                    yield return CharOwner.currentMoveProfile.StartMovement(possiblePos.Pos);
                                    actionoffset = Time.time;
                                }
                            }
                        }
                       
                    }
                    yield return null;
                }
            }
            else
            {
                if (possiblePos != null)
                {
                    possiblePos.isTaken = false;
                    possiblePos = null;
                }
            }
        }
    }

    public override IEnumerator AttackSequence()
    {
        yield return null;
    }

    public override void SetAttackReady(bool value)
    {
        if (value)
        {
            StartInput();
        }
        else
        {
            EndInput();
        }
        
    }

    public override IEnumerator ReloadDefending_Co()
    {
        yield return null;
    }


    public override void SetCharDead()
    {
        if (AICo != null)
        {
            CharOwner.StopCoroutine(AICo);
            AICo = null;
        }
        base.SetCharDead();
        CharOwner.SpineAnim.CurrentAnim = "";
        CharOwner.SetAnimation(CharacterAnimationStateType.GettingHit);
    }

    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if(CharOwner.CharInfo.Behaviour.InputBehaviour != InputBehaviourType.AIInput)
        {
            return true;
        }
        if (completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString() ||
            completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            CharOwner.transform.position = new Vector3(100, 100, 100);
            CharOwner.SpineAnim.CurrentAnim = "";
            CharOwner.SetAnimation(CharacterAnimationStateType.Idle);
            CharOwner.SetAttackReady(false);
            //CharOwner.DisableChar(CharOwner.CharInfo.DeathDisableing_Delay);
            return true;
        }

       
        return base.SpineAnimationState_Complete(completedAnim);
    }

    public override bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if ((BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle && CharOwner.SpineAnim.CurrentAnim.Contains("Defeat")) ||
            (animState == CharacterAnimationStateType.Defending.ToString() && CharOwner.SpineAnim.CurrentAnim.Contains("Defending")) ||
            (animState.Contains("GettingHit") && CharOwner.SpineAnim.CurrentAnim.Contains("GettingHit"))) 
        {
            return true;
        }

        return base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }
}


[System.Serializable]
public class HitInfoClass
{
    public IDamageMaker hitter = null;
    public CharacterNameType CharacterId;
    public float Damage;
    public float TimeLastHit = 0;

    public HitInfoClass()
    {
        TimeLastHit = Time.time;
    }

    public HitInfoClass(IDamageMaker character, float damage)
    {
        hitter = character;
        CharacterId = character.CharName;
        Damage = damage;
        TimeLastHit = Time.time;
    }

    public void UpdateLastHitTime()
    {
        TimeLastHit = Time.time;
    }
}

[System.Serializable]
public class AggroInfoClass
{
    public ControllerType PlayerController;
    public int Hit;

    public AggroInfoClass()
    {

    }

    public AggroInfoClass(ControllerType playerController, int hit)
    {
        PlayerController = playerController;
        Hit = hit;
    }
}
