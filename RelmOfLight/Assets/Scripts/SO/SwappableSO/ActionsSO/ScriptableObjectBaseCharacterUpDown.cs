using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/UpDown")]
public class ScriptableObjectBaseCharacterUpDown : ScriptableObjectBaseCharacterBaseMove
{

    

    InputDirectionType direction = InputDirectionType.Right;

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

        if(direction == InputDirectionType.Right)
        {
            direction = (InputDirectionType)Random.Range(0, 2);
        }
        switch (direction)
        {
            case InputDirectionType.Up:
                possibleMoves = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.left);
                break;
            case InputDirectionType.Down:
                possibleMoves = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.right);
                break;
        }

        if(possibleMoves == null || possibleMoves.Count == 0)
        {
            switch (direction)
            {
                case InputDirectionType.Up:
                    possibleMoves = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.right);
                    break;
                case InputDirectionType.Down:
                    possibleMoves = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.left);
                    break;
            }

            direction = direction == InputDirectionType.Up ? InputDirectionType.Down : InputDirectionType.Up;
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


