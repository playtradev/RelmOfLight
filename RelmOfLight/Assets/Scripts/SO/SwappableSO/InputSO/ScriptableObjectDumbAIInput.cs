using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharaqcterInput/DumbAIInput")]
public class ScriptableObjectDumbAIInput : ScriptableObjectBaseCharacterInput
{
    public IEnumerator AICo = null;
    protected float totDamage = 0;




    public override void SetUpEnteringOnBattle(CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving, bool loop = false)
    {
        CharOwner.SetAnimation(anim, loop);
        base.SetUpEnteringOnBattle(anim, loop);
    }

    public override void SetUpLeavingBattle(bool withAudio = true)
    {
        if (CharOwner.SpineAnim.HasAnimation("Reverse_Arriving"))
        {
            CharOwner.SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        }
        else
        {
            BattleManagerScript.Instance.StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(0.1f, CharOwner, new Vector3(100, 100, 100)));
        }
        base.SetUpLeavingBattle();
    }

    public override void StartInput()
    {
        if(CharOwner.isActiveAndEnabled)
        {
            AICo = AI();
            CharOwner.StartCoroutine(AICo);
        }
       
    }

    public override void EndInput()
    {
        if (AICo != null)
        {
            CharOwner.StopCoroutine(AICo);
        }
    }

    public override void SetupCharacterSide(bool restoreChar = true)
    {
        base.SetupCharacterSide(restoreChar);
    }

    public virtual IEnumerator AI()
    {
        bool val = true;
        while (val)
        {
            yield return null;
            if (CharOwner.IsOnField && !CharOwner.died)
            {
                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
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

    public override void SetCharDead()
    {
        if (AICo != null)
        {
            CharOwner.StopCoroutine(AICo);
            AICo = null;
        }
        totDamage = 0;
       
        base.SetCharDead();
    }

    public override void SetFinalDamage(BaseCharacter attacker, ref float damage, HitInfoClass hic = null)
    {
        totDamage += damage;
        base.SetFinalDamage(attacker, ref damage, hic);
    }


    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if (completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString() ||
            completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            CharOwner.transform.position = new Vector3(100, 100, 100);
            CharOwner.SpineAnim.CurrentAnim = "";
            CharOwner.SetAnimation(CharacterAnimationStateType.Idle);
            CharOwner.SetAttackReady(false);
            CharOwner.DisableChar(CharOwner.CharInfo.DeathDisableing_Delay);
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