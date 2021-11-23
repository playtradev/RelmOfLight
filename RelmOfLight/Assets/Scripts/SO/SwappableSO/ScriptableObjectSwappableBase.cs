using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSwappableBase : ScriptableObject
{

    public BaseCharacter CharOwner;

    public SwappableActionType SwappableType;

    public virtual bool SpineAnimationState_Complete(string completedAnim)
    {
       

        return false;
    }
    public virtual bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (CharOwner.isMoving && (animState.ToString() != CharacterAnimationStateType.Reverse_Arriving.ToString() && !animState.ToString().Contains("Defeat")) && (!animState.ToString().Contains("Dash")))
        {
            return true;
        }

        return false;
    }


    public virtual void Reset()
    {

    }

    public virtual void SetUpEnteringOnBattle(CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving, bool loop = false)
    {

    }

    public virtual void SetUpLeavingBattle(bool withAudio = true)
    {

    }

    public virtual void SetAttackReady(bool value)
    {

    }

    public virtual void SetCharDead()
    {

    }

    public virtual void CharArrivedOnBattleField(bool overrideAnimAndPos = true, CharacterAnimationStateType anim = CharacterAnimationStateType.Arriving)
    {

    }



    public virtual void UpdateVitalities()
    {

    }

    public virtual void Destroy()
    {
    }
}