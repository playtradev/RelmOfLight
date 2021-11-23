using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterDeath/Defeat ")]
public class ScriptableObjectBaseCharacterDeathDefeat : ScriptableObjectBaseCharacterDeath
{
    public override void SetCharDead()
    {
        CharOwner.SetAnimation(CharacterAnimationStateType.Defeat_IdleToLoop);
       
        base.SetCharDead();

    }
}



