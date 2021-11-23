using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterDeath/DefeatAndRevive ")]
public class ScriptableObjectBaseCharacterDeathDefeatAndRevive : ScriptableObjectBaseCharacterDeath
{
    public override void SetCharDead()
    {
        CharOwner.StartCoroutine(DefeatAnimPlayer());
        base.SetCharDead();
    }

    protected IEnumerator DefeatAnimPlayer()
    {
        CharOwner.CharBoxCollider.enabled = true;

        CharOwner.SetAnimation(CharacterAnimationStateType.Defeat_IdleToLoop);

        yield return ReviveAfter(CharOwner.CharInfo.CharacterRespawnLengthNew);

    }

}



