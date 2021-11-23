using MyBox;
using PlaytraGamesLtd;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharaqcterInput/PlayerInput")]
public class ScriptableObjectPlayerInput : ScriptableObjectBaseCharacterInput
{
 

    public IEnumerator SkillActivation = null;

    public override void Reset()
    {
      
        base.Reset();
    }

    public override void StartInput()
    {
    }

    public override void EndInput()
    {
    }

   


    public override void CharacterInputHandler(InputActionType action)
        => CharOwner.StartCoroutine(CharacterInputQueue(action));

    ScriptableObjectAttackBase tempAtk = null;
    BuffDebuffStatsType[] BuffDebuffInputBlockers = new BuffDebuffStatsType[] { BuffDebuffStatsType.Rage, BuffDebuffStatsType.MeleeAttack };
    IEnumerator CharacterInputQueue(InputActionType action)
    {
        //No Error Sound
        if (BattleManagerScript.Instance.isSkillHappening.Value || !CharOwner.IsOnField || CharOwner.isTeleporting || BattleManagerScript.Instance.CurrentBattleState == BattleState.WaveEnd)
            yield break;


        CharacterActionType charAction = CharacterActionType.None;
        bool isCharAction = System.Enum.TryParse(action.ToString(), out charAction);
        AttackInputType attackAction = AttackInputType.Weak;
        bool isAttackAction = System.Enum.TryParse(action.ToString(), out attackAction);
		switch (attackAction)
		{
			case AttackInputType.Weak:
				tempAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r => r.AttackInput == AttackInputType.Weak);
				break;
			case AttackInputType.Strong:
				tempAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r => r.AttackInput == AttackInputType.Weak);
                break;
			case AttackInputType.Skill1:
				tempAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r => r.AttackInput == AttackInputType.Weak);
                break;
			case AttackInputType.Skill2:
				tempAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r => r.AttackInput == AttackInputType.Weak);
                break;
			case AttackInputType.Skill3:
				tempAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r => r.AttackInput == AttackInputType.Weak);
                break;
		}
    }

    public override IEnumerator AttackSequence()
    {
        /*  yield return CharacterInputQueue(InputActionType.Weak);
          while (CharOwner.currentAttackProfile.Attacking)
          {
              yield return null;
          }*/

        yield return null;
    }

    public override void SetAttackReady(bool value)
    {

    }

    public override void SetCharDead()
    {
        CharOwner.BuffsDebuffsList.ForEach(r =>
        {
            if (r.Stat != BuffDebuffStatsType.Zombie && r.Stat != BuffDebuffStatsType.Cursed)
            {
                r.Duration = 0;
                r.Stop_Co = true;
            }
        }
        );
        for (int i = 0; i < CharOwner.CharInfo.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(CharOwner.CharInfo.Pos[i], BattleTileStateType.Empty);
        }

        BattleManagerScript.Instance.SelectionDeselectinoCharacter(false, CharOwner, CharOwner.CurrentPlayerController);
        
        CharOwner.SetAttackReady(false);
        base.SetCharDead();
        UpdateVitalities();

        if (CharOwner.RespawnSequencerCo != null) CharOwner.StopCoroutine(CharOwner.RespawnSequencerCo);
        CharOwner.RespawnSequencerCo = CharOwner.ReviveSequencer();
        CharOwner.StartCoroutine(CharOwner.RespawnSequencerCo);
    }

    public override void SetUpLeavingBattle(bool withAudio = true)
    {
        
        // Debug.LogError(CharOwner.CharInfo.CharacterID + "    SetUpLeavingBattle   " + CharOwner.CurrentPlayerController + "    " + Time.time);
        CharOwner.CurrentPlayerController = ControllerType.None;
             
        CharOwner.SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
       
        base.SetUpLeavingBattle();
    }

    public override void SetUpEnteringOnBattle(CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving, bool loop = false)
    {
       
        CharOwner.UMS.EnableBattleBars(true);
        CharOwner.SpineAnim.skeletonAnimation.enabled = true;
        CharOwner.SetAnimation(anim, loop);
       
        base.SetUpEnteringOnBattle(anim, loop);
    }



    public override void UpdateVitalities()
    {
        base.UpdateVitalities();
    }

    public override void Destroy()
    {
        base.Destroy();
        RemoveCurrentCharSkillCompleted();
    }

    public override bool SpineAnimationState_Complete(string completedAnim)
    {
      
        return base.SpineAnimationState_Complete(completedAnim);
    }

    public override bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        return base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }


}

public class SkillCoolDownClass
{
    public AttackInputType Skill;
    public bool IsCoGoing;

    public SkillCoolDownClass()
    {

    }

    public SkillCoolDownClass(AttackInputType skill, bool isCoGoing)
    {
        Skill = skill;
        IsCoGoing = isCoGoing;
    }
}

