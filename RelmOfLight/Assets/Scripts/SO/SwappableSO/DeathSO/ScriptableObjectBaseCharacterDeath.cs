using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


public class ScriptableObjectBaseCharacterDeath : ScriptableObjectSwappableBase
{
    //Helper variables
    protected GameObject tempGameObject;

    public bool overrideDeathParticles = false;

    public IEnumerator Explode(bool fireIdleAnim = true)
    {
        if (!overrideDeathParticles)
        {
            GameObject go = null;
            tempGameObject = ParticleManagerScript.Instance.SetEmissivePsInPosition(CharOwner.CharInfo.DeathExplosion_OverrideParticles != ParticlesType.None ? CharOwner.CharInfo.DeathExplosion_OverrideParticles : ParticlesType.Death, CharOwner.CharInfo.Facing, CharOwner.SpineAnim.transform.position, CharOwner.CharInfo.GetComponent<MeshRenderer>(), ref go);
        }

        CameraManagerScript.Shaker.PlayShake("Arriving_Impact");
        for (int i = 0; i < CharOwner.CharInfo.Pos.Count; i++)
        {
            if (CharOwner.IsOnField || (!CharOwner.IsOnField && GridManagerScript.Instance.GetBattleTile(CharOwner.CharInfo.Pos[i]).BattleTileState == BattleTileStateType.Occupied))
            {
                GridManagerScript.Instance.SetBattleTileState(CharOwner.CharInfo.Pos[i], BattleTileStateType.Empty);
            }
        }


        BattleManagerScript.Instance.StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(CharOwner.CharInfo.DeathDisableing_Delay, CharOwner, new Vector3(100, 100, 100)));
        if(fireIdleAnim)
        {
            CharOwner.SpineAnim.CurrentAnim = "";
            CharOwner.SetAnimation(CharacterAnimationStateType.NoMesh);
        }
        CharOwner.DisableChar(CharOwner.CharInfo.DeathDisableing_Delay);
        yield return null;
    }

    public IEnumerator ReviveAfter(float duration)
    {
        yield return BattleManagerScript.Instance.WaitFor(duration, ()=> BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

        if(CharOwner.CharInfo.Behaviour.DeathBehaviour == DeathBehaviourType.Defeat_And_Revive)
        {
            CharOwner.CharBackFromDeath();
            CharOwner.currentInputProfile.StartInput();
        }
    }

    public override bool SpineAnimationState_Complete(string completedAnim)
    { 
          return base.SpineAnimationState_Complete(completedAnim);
    }
}



