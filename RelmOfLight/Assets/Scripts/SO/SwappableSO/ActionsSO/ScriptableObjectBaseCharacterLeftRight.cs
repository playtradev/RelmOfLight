using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/LeftRight")]
public class ScriptableObjectBaseCharacterLeftRight : ScriptableObjectBaseCharacterBaseMove
{
    public override Vector2Int[] GetMovesTo(BattleTileScript[] poses)
    {
        return new Vector2Int[] { poses[0].Pos };
    }
    public override IEnumerator StartMovement(Vector2Int nextPos)
    {
        tempVector2Int = nextPos - CharOwner.CharInfo.CurrentTilePos;
        GetDirectionVectorAndAnimationCurve(tempVector2Int == Vector2Int.right ? InputDirectionType.Down : tempVector2Int == Vector2Int.left ? InputDirectionType.Up : tempVector2Int == Vector2Int.up ? InputDirectionType.Right : InputDirectionType.Left);
        if (GridManagerScript.Instance.IsWalkableAndFree(CharOwner.CharInfo.Pos, dir, CharOwner.CharInfo.WalkingSide) && CharOwner.CharActionlist.Contains(CharacterActionType.Move))
        {
            CurrentAttackInfoClass caic = CharOwner.currentAttackProfile.CurrentAttack(AttackInputType.Weak);
            if (caic != null)
            {
                caic.InteruptAttack(false);
            }

            CharOwner.isMoving = true;
            yield return StartMovement_Co(dir);
        }
    }

    List<BattleTileScript> possibleMoves = new List<BattleTileScript>();
    public override List<BattleTileScript> GetPossibleTiles()
    {
        switch (CharOwner.CharInfo.Facing)
        {
            case FacingType.Left:
                possibleMoves = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.down);
                break;
            case FacingType.Right:
                possibleMoves = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.up);
                break;
        }

        return possibleMoves;
    }

    public override List<BattleTileScript> CheckTileAvailabilityUsingDir(Vector2Int dir)
    {
        tempList_Vector2int = CalculateNextPosUsingDir(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(CharOwner.CharInfo.Pos, tempList_Vector2int, CharOwner.CharInfo.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(tempList_Vector2int, CharOwner.CharInfo.WalkingSide);
        }
        return base.CheckTileAvailabilityUsingDir(dir);
    }


    public override bool CanIStartMoveAction()
    {
        return AreTileNearEmpty();
    }

}


