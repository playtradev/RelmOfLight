using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterDeath/Reverse_Arrives")]
public class ScriptableObjectBaseCharacterDeathReverseArrives : ScriptableObjectBaseCharacterDeath
{
    public override void SetCharDead()
    {
        for (int i = 0; i < CharOwner.CharInfo.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(CharOwner.CharInfo.Pos[i], BattleTileStateType.Empty);
        }
        CharOwner.SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
        base.SetCharDead();
    }

}



