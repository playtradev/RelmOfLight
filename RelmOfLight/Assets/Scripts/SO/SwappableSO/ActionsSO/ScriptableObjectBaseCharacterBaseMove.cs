using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ScriptableObjectBaseCharacterBaseMove : ScriptableObjectBaseCharacterAction
{
    protected Vector2Int dir;
    protected AnimationCurve curve;
    public string animState;

    protected List<Vector2Int> tempList_Vector2int = new List<Vector2Int>();

    protected float tempFloat_1;
    protected Vector2Int tempVector2Int;

    public virtual IEnumerator MoveTo(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        yield return null;
    }


    public virtual List<BattleTileScript> GetPossibleTiles()
    {
        return GridManagerScript.Instance.BattleTiles.Where(r => (CharOwner.CharInfo.WalkingSide == WalkingSideType.Both || r.WalkingSide == WalkingSideType.Both || r.WalkingSide == CharOwner.CharInfo.WalkingSide) &&
                                    r.BattleTileState != BattleTileStateType.NonUsable && r.BattleTileState != BattleTileStateType.Bound && !r.isTaken).ToList();
    }

    public virtual Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        return new Vector2Int[0];
    }

    public virtual Vector2Int[] GetMovesTo(BattleTileScript[] poses)
    {
        return new Vector2Int[0];
    }

    public virtual Vector2Int[] GetMovesTo(BattleTileScript pos)
    {
        return new Vector2Int[0];
    }

    public virtual IEnumerator StartMovement(InputDirectionType inDir)
    {
        GetDirectionVectorAndAnimationCurve(inDir);
        if(GridManagerScript.Instance.IsWalkableAndFree(CharOwner.CharInfo.Pos, dir, CharOwner.CharInfo.WalkingSide) && CharOwner.CharActionlist.Contains(CharacterActionType.Move))
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



    public BattleTileScript temp_bts = null;

    BaseCharacter temp_pbc = null;

    public virtual IEnumerator StartMovement(Vector2Int nextPos)
    {
        yield return null;
    }

    public virtual IEnumerator StartMovement_Co(Vector2Int newPos, bool overrideAnim = true)
    {

        if (CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Arriving.ToString() ||
            CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            CharOwner.isMoving = false;
            yield break;
        }


        //Debug.Log(CharOwner.CharInfo.CharacterID + "    " + newPos + "    " + Time.time + "    " + CharOwner.isMoving);
        if(overrideAnim)
        {
            if (animState.ToString() == CharOwner.SpineAnim.CurrentAnim)
            {
                tempFloat_1 = ((CharOwner.SpineAnim.GetAnimLenght(animState) * CharOwner.CharInfo.SpeedStats.LoopPerc) / CharOwner.CharInfo.SpeedStats.TileMovementTime) * CharOwner.CharInfo.SpeedStats.MovementSpeed * CharOwner.CharInfo.BaseSpeed;
                CharOwner.SpineAnim.SetAnimationSpeed(tempFloat_1);
                CharOwner.SpineAnim.skeletonAnimation.state.GetCurrent(0).TrackTime = CharOwner.SpineAnim.GetAnimLenght(animState) * CharOwner.CharInfo.SpeedStats.IntroPerc;
            }
            else
            {
                CharOwner.SetAnimation(animState.ToString());
                CharOwner.SpineAnim.SetAnimationSpeed(CharOwner.GetMovementAnimSpeed(animState));
            }
        }
      
        for (int i = 0; i < CharOwner.CharInfo.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(CharOwner.CharInfo.Pos[i], BattleTileStateType.Empty);
        }
        
        CharOwner.CharInfo.CurrentTilePos = newPos;
        CharOwner.SetLayer();

        for (int i = 0; i < CharOwner.CharInfo.Pos.Count; i++)
        {
            //Debug.LogError(CharOwner.CharInfo.CharacterID + "    moving in pos" + CharOwner.CharInfo.Pos[i] + "   " + Time.time);
            GridManagerScript.Instance.SetBattleTileState(CharOwner.CharInfo.Pos[i], BattleTileStateType.Occupied);
        }
        if(moveCo != null)
        {
            CharOwner.StopCoroutine(moveCo);
        }
        moveCo = MoveByTileSpace(GridManagerScript.Instance.GetBattleTile(newPos), curve, CharOwner.CharInfo.SpeedStats.CuttingPerc);

        yield return moveCo;
        moveCo = null;
    }
    IEnumerator moveCo = null;
    public virtual IEnumerator MoveByTileSpace(BattleTileScript nextPos, AnimationCurve curve, float animPerc)
    {
        float timer = 0f;
        float spaceTimer = 0;
        bool isMovCheck = false;
        string moveAnim = animState.ToString();
        Vector3 offset = CharOwner.spineT.position;
        CharOwner.transform.position = nextPos.transform.position;
        CharOwner.spineT.position = offset;
        Vector3 localoffset = CharOwner.spineT.localPosition;
        animPerc = animPerc == 1f ? 0.95f : animPerc;
        while (timer < 1)
        {
            //Debug.Log(CharOwner.CharInfo.CharacterID + "     moving    " + timer);

            yield return BattleManagerScript.Instance.WaitFixedUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            //Debug.Log(CharOwner.CharInfo.CharacterID + "     moving    " + timer);

            timer += (BattleManagerScript.Instance.FixedDeltaTime / (CharOwner.CharInfo.SpeedStats.TileMovementTime / (CharOwner.CharInfo.SpeedStats.MovementSpeed * CharOwner.CharInfo.BaseSpeed))) * BattleManagerScript.Instance.BattleSpeed;
            //Debug.Log(CharOwner.CharInfo.CharacterID + "     moving    " + timer);

            spaceTimer = curve.Evaluate(timer);
            // Debug.Log(CharOwner.CharInfo.CharacterID + "     moving    " + timer);

            CharOwner.spineT.localPosition = Vector3.Lerp(localoffset, CharOwner.LocalSpinePosoffset, spaceTimer);
            //Debug.Log(CharOwner.CharInfo.CharacterID + "     moving    " + timer);

            if (timer > animPerc && !isMovCheck && CharOwner.isMoving)
            {
                // Debug.Log(CharOwner.CharInfo.CharacterID + "     perch completed    " + timer);
                isMovCheck = true;
                CharOwner.isMoving = false;
                CharOwner.Invoke_TileMovementCompleteEvent();
            }
            // Debug.Log(CharOwner.CharInfo.CharacterID + "     moving    " + timer);

            if (CharOwner.isMoving && CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
            {
                // Debug.Log(CharOwner.CharInfo.CharacterID + "     reverse arrivibg    " + timer);
                CharOwner.isMoving = false;
            }
            // Debug.Log(CharOwner.CharInfo.CharacterID + "     moving   " + timer);

        }
        // Debug.Log(CharOwner.CharInfo.CharacterID + "     moving    " + timer);

        CharOwner.spineT.localPosition = CharOwner.LocalSpinePosoffset;
        // Debug.Log(CharOwner.CharInfo.CharacterID + "     moving   end    " + timer);
        while (CharOwner.SpineAnim.CurrentAnim == moveAnim)
        {
            yield return null;
        }
    }


    public Vector2Int GetDirectionVectorAndAnimationCurve(InputDirectionType nextDir)
    {
        animState = CharacterAnimationStateType.Idle.ToString();
        curve = new AnimationCurve();
        dir = CharOwner.CharInfo.CurrentTilePos;
        switch (nextDir)
        {
            case InputDirectionType.Up:
                dir += new Vector2Int(-1, 0);
                curve = CharOwner.SpineAnim.Space_Time_Curves.UpMovement;
                animState = CharacterAnimationStateType.DashUp.ToString();
                break;
            case InputDirectionType.Down:
                dir += new Vector2Int(1, 0);
                curve = CharOwner.SpineAnim.Space_Time_Curves.DownMovement;
                animState = CharacterAnimationStateType.DashDown.ToString();
                break;
            case InputDirectionType.Right:
                dir += new Vector2Int(0, 1);
                animState = CharOwner.CharInfo.Facing == FacingType.Left ? CharacterAnimationStateType.DashRight.ToString() : CharacterAnimationStateType.DashLeft.ToString();
                if (animState == CharacterAnimationStateType.DashLeft.ToString())
                {
                    curve = CharOwner.SpineAnim.Space_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = CharOwner.SpineAnim.Space_Time_Curves.ForwardMovement;
                }
                break;
            case InputDirectionType.Left:
                dir += new Vector2Int(0, -1);
                animState = CharOwner.CharInfo.Facing == FacingType.Left ? CharacterAnimationStateType.DashLeft.ToString() : CharacterAnimationStateType.DashRight.ToString();
                if (animState == CharacterAnimationStateType.DashLeft.ToString())
                {
                    curve = CharOwner.SpineAnim.Space_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = CharOwner.SpineAnim.Space_Time_Curves.ForwardMovement;
                }
                break;
        }

        return dir;
    }

  

    protected List<BattleTileScript> CheckTileAvailabilityUsingPos(Vector2Int dir)
    {
        tempList_Vector2int = CalculateNextPosUsinPos(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(CharOwner.CharInfo.Pos, tempList_Vector2int, CharOwner.CharInfo.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(tempList_Vector2int, CharOwner.CharInfo.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsingDir(Vector2Int direction)
    {
        tempList_Vector2int.Clear();
        for (int i = 0; i < CharOwner.CharInfo.Pos.Count; i++)
        {
            tempList_Vector2int.Add(CharOwner.CharInfo.Pos[i] + direction);
        }

        return tempList_Vector2int;
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsinPos(Vector2Int direction)
    {
        tempList_Vector2int.Clear();

        for (int i = 0; i < CharOwner.CharInfo.Pos.Count; i++)
        {
            tempList_Vector2int.Add((CharOwner.CharInfo.Pos[i] - CharOwner.CharInfo.CurrentTilePos) + direction);
        }
        return tempList_Vector2int;
    }

    public virtual List<BattleTileScript> CheckTileAvailabilityUsingDir(Vector2Int dir)
    {
        return new List<BattleTileScript>();
    }


    public virtual bool CanIStartMoveAction()
    {
        return false;
    }

    public bool AreTileNearEmpty()
    {
        List<BattleTileScript> res = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.up);
        res.AddRange(CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.down));
        res.AddRange(CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.left));
        res.AddRange(CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.right));

        if (res.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


