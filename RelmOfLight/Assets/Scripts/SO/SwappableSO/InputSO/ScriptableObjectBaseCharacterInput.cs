using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


public class ScriptableObjectBaseCharacterInput : ScriptableObjectSwappableBase
{
    public delegate void CurrentCharSkillCompleted(AttackInputType inputSkill, float duration);
    public event CurrentCharSkillCompleted CurrentCharSkillCompletedEvent;
    protected GameObject temp_go;
    public bool WaitingForHit = false;
    public bool UseStrong = false;
    public bool UseDir = false;
    public InputDirectionType NewDir = InputDirectionType.Down;
    [HideInInspector] public Vector2Int nextAttackPos = Vector2Int.zero;


    //TEMP
    protected bool temp_Bool;
    protected GameObject tempGameObject = null;
    protected float tempFloat_1;
    protected int tempInt_1, tempInt_2, tempInt_3;
    protected Vector2Int tempVector2Int;
    protected Vector3 tempVector3;
    protected string tempString;
    protected List<Vector2Int> tempList_Vector2int = new List<Vector2Int>();
    [HideInInspector] public List<ScriptableObjectAttackBase> availableAtks = new List<ScriptableObjectAttackBase>();
    [HideInInspector] public List<ScriptableObjectAttackBase> currentTypedAtks = new List<ScriptableObjectAttackBase>();

    public override void SetCharDead()
    {
        base.SetCharDead();
    }

    public virtual void StartInput()
    {
    }

    public virtual void EndInput()
    {
    }

    public virtual IEnumerator AttackSequence()
    {
        yield return null;
    }

    public virtual void CharacterInputHandler(InputActionType action)
    {
    }

    public virtual void SetupCharacterSide(bool restoreChar = true)
    {
        CharOwner.UMS.SetUnit();
        if(restoreChar)
        {
            CharOwner.CharInfo.SetupChar();
        }
    }

    public virtual IEnumerator ReloadDefending_Co()
    {
        yield return null;
    }

    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if (completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString() ||
         completedAnim == CharacterAnimationStateType.JumpTransition_OUT.ToString() ||
         completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            /*  for (int i = 0; i < CharInfo.Pos.Count; i++)
              {
                  GridManagerScript.Instance.SetBattleTileState(CharInfo.Pos[i], BattleTileStateType.Empty);
                  CharInfo.Pos[i] = Vector2Int.zero;
              }*/
            CharOwner.SetAttackReady(false);
            CharOwner.transform.position = new Vector3(100, 100, 100);
            if (BattleManagerScript.Instance.TeamInfo[CharOwner.CharInfo.Side].PlayerController.Contains(ControllerType.Enemy))
            {
                CharOwner.gameObject.SetActive(false);
            }
            else
            {
                //CharOwner.SpineAnim.skeletonAnimation.enabled = false;
            }
        }

        if (completedAnim == CharacterAnimationStateType.Arriving.ToString() && CharOwner.CurrentPlayerController != ControllerType.None && CharOwner.CurrentPlayerController != ControllerType.Enemy)
        {
           
        }
        return base.SpineAnimationState_Complete(completedAnim);
    }





    BattleTileScript bts;
    public virtual ScriptableObjectAttackBase GetRandomAttack(out BaseCharacter target)
    {
        ScriptableObjectAttackBase nextAtk = null;

        currentTypedAtks = CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r != null && CharOwner.CharActionlist.Contains((CharacterActionType)System.Enum.Parse(typeof(CharacterActionType), r.AttackInput.ToString()))).ToList();
        availableAtks.Clear();
        availableAtks = currentTypedAtks;

        for (int i = 0; i < availableAtks.Count; i++)
        {
            nextAtk = availableAtks[i];

            for (int a = 0; a < nextAtk.NewFovSystemList.Count; a++)
            {
                bts = GridManagerScript.Instance.GetBattleTile(CharOwner.CharInfo.CurrentTilePos +
                    (CharOwner.CharInfo.Side == TeamSideType.LeftSideTeam ? nextAtk.NewFovSystemList[a] : new Vector2Int(nextAtk.NewFovSystemList[a].x, -nextAtk.NewFovSystemList[a].y)));
                if (bts != null && bts.cb != null && bts.cb.CharInfo.Side != CharOwner.CharInfo.Side)
                {
                    target = bts.cb;
                    return nextAtk;
                }
            }
        }
        target = null;
        return null;
    }

    public override void Reset()
    {
        base.Reset();
    }

    public void CallCurrentCharSkillCompleted(AttackInputType inputSkill, float duration)
    {
        CurrentCharSkillCompletedEvent(inputSkill, duration);
    }

    public void RemoveCurrentCharSkillCompleted()
    {
        CurrentCharSkillCompletedEvent = null;
    }


    public virtual void SetFinalDamage(BaseCharacter attacker, ref float damage, HitInfoClass hic = null)
    {
    }

    public virtual DefendingActionType CheckAction(DamageInfoClass damageInfo, ref float damage)
    {
        return DefendingActionType.None;
    }

}



