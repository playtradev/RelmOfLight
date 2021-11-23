using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/CrossMovement")]
public class ScriptableObjectBaseCharacterCrossMovement : ScriptableObjectBaseCharacterBaseMove
{

    InputDirectionType prevMove = InputDirectionType.UpRight;
    bool backHome = false;

    public override Vector2Int[] GetMovesTo(BattleTileScript[] poses)
    {
        return new Vector2Int[] { poses[0].Pos };
    }

    Dictionary<InputDirectionType, List<BattleTileScript>> possiblePos = new Dictionary<InputDirectionType, List<BattleTileScript>>()
    {
        {InputDirectionType.Up, new List<BattleTileScript>() },
        {InputDirectionType.Down, new List<BattleTileScript>() },
        {InputDirectionType.Left, new List<BattleTileScript>() },
        {InputDirectionType.Right, new List<BattleTileScript>() }
    };


    public override List<BattleTileScript> GetPossibleTiles()
    {
        if (!backHome)
        {
            possiblePos[InputDirectionType.Up] = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.right);
            possiblePos[InputDirectionType.Down] = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.left);
            possiblePos[InputDirectionType.Left] = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.down);
            possiblePos[InputDirectionType.Right] = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.up);
            prevMove = (InputDirectionType)Random.Range(0, 4);
            return possiblePos[prevMove];
        }
        else
        {
            switch (prevMove)
            {
                case InputDirectionType.Up:
                    return CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.left);
                case InputDirectionType.Down:
                    return CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.right);
                case InputDirectionType.Left:
                    return CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.up);
                case InputDirectionType.Right:
                    return CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.down);
            }
        }
        return null;
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
            backHome = !backHome;
            yield return StartMovement_Co(dir);
        }
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


