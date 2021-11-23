using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterDeath/Explode ")]
public class ScriptableObjectBaseCharacterDeathExplode : ScriptableObjectBaseCharacterDeath
{
    public override void SetCharDead()
    {
        CharOwner.StartCoroutine(Explode());
        base.SetCharDead();
    }

}



