using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/Idle")]
public class ScriptableObjectBaseCharacterIdle : ScriptableObjectBaseCharacterBaseMove
{
    public override bool CanIStartMoveAction()
    {
        return false;
    }


    public override IEnumerator StartMovement(Vector2Int nextPos)
    {
        yield break;
    }



    public override Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        return null;
    }

    public override Vector2Int[] GetMovesTo(BattleTileScript[] poses)
    {

        return null;
    }


    public override IEnumerator MoveByTileSpace(BattleTileScript nextPos, AnimationCurve curve, float animPerc)
    {
        CharOwner.isMoving = false;
        yield break;
    }


}


