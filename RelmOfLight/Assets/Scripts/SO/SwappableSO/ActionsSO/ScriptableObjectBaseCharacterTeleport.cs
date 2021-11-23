using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/Teleport")]
public class ScriptableObjectBaseCharacterTeleport : ScriptableObjectBaseCharacterBaseMove
{
    public ParticlesType overrideTeleportParticleIn = ParticlesType.Chapter01_TohoraSea_Boss_TeleportationIn;
    public ParticlesType overrideTeleportParticleOut = ParticlesType.Chapter01_TohoraSea_Boss_TeleportationOut;

    [HideInInspector] public GameObject MovementPsIn;
    [HideInInspector] public GameObject MovementPsOut;

    public float TeleportDuration = 0.1f;

    public override bool CanIStartMoveAction()
    {
        return true;
    }


    public override IEnumerator StartMovement(Vector2Int nextPos)
    {
        if (GridManagerScript.Instance.IsWalkableAndFree(CharOwner.CharInfo.Pos, nextPos, CharOwner.CharInfo.WalkingSide) && CharOwner.CharActionlist.Contains(CharacterActionType.Move))
        {
            CurrentAttackInfoClass caic = CharOwner.currentAttackProfile.CurrentAttack(AttackInputType.Weak);
            if (caic != null)
            {
                caic.InteruptAttack(false);
            }

            CharOwner.isMoving = true;
            yield return StartMovement_Co(nextPos, false);
        }
    }



    public override Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        return new Vector2Int[] { poses.First() };
    }

    public override Vector2Int[] GetMovesTo(BattleTileScript[] poses)
    {

        return new Vector2Int[] { poses[0].Pos };
    }


    public override IEnumerator MoveByTileSpace(BattleTileScript nextPos, AnimationCurve curve, float animPerc)
    {
        if (overrideTeleportParticleOut != ParticlesType.None)
        {
            ParticleManagerScript.Instance.SetEmissivePsInPosition(overrideTeleportParticleOut, CharOwner.CharInfo.Facing, CharOwner.transform.position, CharOwner.CharInfo.GetComponent<MeshRenderer>(), ref MovementPsOut);
            MovementPsOut.SetActive(true);
        }

        float timer = 0;
        while (TeleportDuration > timer)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
        }

        CharOwner.transform.position = new Vector3(100, 100, 100);

        timer = 0;
        if (overrideTeleportParticleIn != ParticlesType.None)
        {
            ParticleManagerScript.Instance.SetEmissivePsInPosition(overrideTeleportParticleIn, CharOwner.CharInfo.Facing, nextPos.transform.position, CharOwner.CharInfo.GetComponent<MeshRenderer>(), ref MovementPsIn);
            MovementPsIn.SetActive(true);
        }
        while (TeleportDuration > timer)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
        }
        CharOwner.isMoving = false;
        CharOwner.SetAnimation(animState, animState.Contains("Idle"));
        CharOwner.transform.position = nextPos.transform.position;
        nextPos.cb = CharOwner;
    }
}


