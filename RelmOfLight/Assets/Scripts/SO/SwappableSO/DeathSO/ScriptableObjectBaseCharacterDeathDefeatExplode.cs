using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterDeath/Defeat_Explode ")]
public class ScriptableObjectBaseCharacterDeathDefeatExplode : ScriptableObjectBaseCharacterDeath
{
    public override void SetCharDead()
    {
        CharOwner.StartCoroutine(DefeatAnimPlayer());
        base.SetCharDead();
    }

    protected IEnumerator DefeatAnimPlayer()
    {
        CharOwner.SetAnimation(CharacterAnimationStateType.Defeat);

        while (!CharOwner.SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Defeat.ToString())) yield return null;
        while (CharOwner.SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Defeat.ToString())) yield return null;

        CharOwner.CharBoxCollider.enabled = true;
        yield return Explode(fireIdleAnim: false);

    }

}



