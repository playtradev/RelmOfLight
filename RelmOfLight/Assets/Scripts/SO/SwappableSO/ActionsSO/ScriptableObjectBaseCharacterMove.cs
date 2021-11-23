using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/Move")]
public class ScriptableObjectBaseCharacterMove : ScriptableObjectBaseCharacterBaseMove
{


    public override Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        Vector2Int[] path;
        for (int i = 0; i < poses.Length; i++)
        {
            path = GridManagerScript.Pathfinding.GetPathTo(poses[i], CharOwner.CharInfo.Pos, CharOwner.CharInfo.WalkingSide);
            if(path.Length > 0)
            {
                return path;
            }
        }
        return GridManagerScript.Pathfinding.GetPathTo(poses[0], CharOwner.CharInfo.Pos, CharOwner.CharInfo.WalkingSide);
    }

    public override Vector2Int[] GetMovesTo(BattleTileScript[] poses)
    {
        Vector2Int[] path;
        for (int i = 0; i < poses.Length; i++)
        {
            path = GridManagerScript.Pathfinding.GetPathTo(poses[i].Pos, CharOwner.CharInfo.Pos, CharOwner.CharInfo.WalkingSide);
            if (path.Length > 0)
            {
                return path;
            }
        }
        return GridManagerScript.Pathfinding.GetPathTo(poses[0].Pos, CharOwner.CharInfo.Pos, CharOwner.CharInfo.WalkingSide);
    }

    public override Vector2Int[] GetMovesTo(BattleTileScript pos)
    {
        return GridManagerScript.Pathfinding.GetPathTo(pos.Pos, CharOwner.CharInfo.Pos, CharOwner.CharInfo.WalkingSide); ;
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

}


