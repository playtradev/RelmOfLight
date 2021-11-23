using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManagerScript : MonoBehaviour
{
    public delegate void InitializationComplete();
    public event InitializationComplete InitializationCompleteEvent;


    public static GridManagerScript Instance;

    public static Pathfinding Pathfinding = new Pathfinding();

    public List<BattleTileScript> BattleTiles = new List<BattleTileScript>();
    public Vector2Int BattleFieldSize = new Vector2Int(6, 12);
    public GameObject TargetIndicator;
    BattleTileScript Temp_bts;

    public Animator GridAnimator;

    public bool[,] GetWalkableTilesLayout(WalkingSideType side)
    {
        bool[,] tilesGrid = new bool[/*BattleFieldSize.x, BattleFieldSize.y*/ 6, 12];
        for (int x = 0; x < /*tilesGrid.GetLength(0)*/ 6; x++)
        {
            for (int y = 0; y < /*tilesGrid.GetLength(1)*/ 12; y++)
            {
                BattleTileScript curTile = BattleTiles.Where(r => r.Pos == new Vector2Int(x, y)).First();
                if (curTile.BattleTileState == BattleTileStateType.Empty && (curTile.WalkingSide == side || curTile.WalkingSide == WalkingSideType.Both))
                {
                    tilesGrid[x, y] = true;
                }
                else
                {
                    tilesGrid[x, y] = false;
                }
            }
        }
        return tilesGrid;
    }


    public List<PortalInfoClass> Portals = new List<PortalInfoClass>();
    public ScriptableObjectGridStructure currentGridStructureObject = null;
    private void Awake()
    {
        Instance = this;
        //Getting all the BattleTileScript
        foreach (BattleTileScript item in FindObjectsOfType<BattleTileScript>(true))
        {
            BattleTiles.Add(item);
        }

    }

    //Setup each single tiles of the grid
    public void SetupGrid(ScriptableObjectGridStructure gridStructure)
    {
        if (gridStructure != null)
        {
            foreach (BattleTileInfo tile in gridStructure.GridInfo)
            {
                BattleTiles.Where(r => r.Pos == tile.Pos).First().SetupTileFromBattleTileInfo(tile, gridStructure.HasBaseTileEffect ? gridStructure.BaseEffectsOnTile : null);
            }
        }
        currentGridStructureObject = gridStructure;
        InitializationCompleteEvent?.Invoke();

    }

    public BattleTileScript GetBoundInRow(Vector2Int pos, FacingType facing)
    {
        Temp_bts = BattleTiles.Where(r => r.Pos.x == pos.x && r.BattleTileState == BattleTileStateType.Bound && (facing == FacingType.Left ? r.Pos.y <= pos.y : r.Pos.y >= pos.y)).FirstOrDefault();
        if (Temp_bts == null)
        {
            Temp_bts = BattleTiles.Where(r => r.Pos.x == pos.x && r.BattleTileState != BattleTileStateType.Bound && r.BattleTileState != BattleTileStateType.NonUsable && (facing == FacingType.Left ? r.Pos.y <= pos.y : r.Pos.y >= pos.y)).First();
        }
        return Temp_bts;
    }

    public void ResetEffectOnTilesTiles()
    {
        foreach (BattleTileScript item in BattleTiles)
        {
            item.ResetEffect();
        }
    }

    public void MoveGrid_ToWorldPosition(Vector3 newGridPos)
    {
        transform.position = newGridPos;
    }

    public void ResetGrid()
    {
        foreach (BattleTileScript tile in BattleTiles)
        {
            BattleTiles.Where(r => r.Pos == tile.Pos).First().ResetTile();
        }
    }

    //Checking if the given positions are part of the desired movent area
    public bool AreBattleTilesInControllerArea(List<Vector2Int> oldPos, List<Vector2Int> newPos, WalkingSideType walkingSide)
    {
        bool AreInControlledArea = false;
        BattleTileScript bts;

        foreach (Vector2Int item in newPos)
        {
            bts = BattleTiles.Where(r => r.Pos == item).FirstOrDefault();
            if (bts == null)
            {
                AreInControlledArea = false;
            }
            else
            {
                if (bts.WalkingSide == walkingSide || bts.WalkingSide == WalkingSideType.Both || walkingSide == WalkingSideType.Both)
                {
                    if (bts.BattleTileState == BattleTileStateType.Empty || (bts.BattleTileState == BattleTileStateType.Occupied && oldPos.Contains(item)))
                    {
                        AreInControlledArea = true;
                    }
                    else
                    {
                        AreInControlledArea = false;

                    }
                }
                else
                {
                    AreInControlledArea = false;
                }
            }
            if (!AreInControlledArea)
            {
                break;
            }
        }
        return AreInControlledArea;
    }


    public bool IsWalkableAndFree(List<Vector2Int> oldPos, Vector2Int pos, WalkingSideType wSide)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        oldPos.ForEach(r => res.Add((r - oldPos[0]) + pos));
        if (!AreBattleTilesInControllerArea(oldPos, res, wSide))
        {
            return false;
        }
        return true;
    }

    public bool IsPosOnField(Vector2Int pos, bool useBoundaries = false)
        => BattleTiles.Where(r => r.Pos == pos && r.BattleTileState > BattleTileStateType.NonUsable && (useBoundaries || r.BattleTileState != BattleTileStateType.Bound)).ToList().Count > 0 ? true : false;
    public bool IsPosOnFieldByHeight(Vector2Int pos)
        => BattleTiles.Where(r => r.Pos.x == pos.x && r.BattleTileState != BattleTileStateType.NonUsable && r.BattleTileState != BattleTileStateType.Bound).ToList().Count > 0 ? true : false;

    public Vector2Int GetVectorFromDirection(InputDirectionType dir)
    {
        switch (dir)
        {
            case InputDirectionType.Up:
                return new Vector2Int(-1, 0);
            case InputDirectionType.Down:
                return new Vector2Int(1, 0);
            case InputDirectionType.Left:
                return new Vector2Int(0, 1);
            case InputDirectionType.Right:
                return new Vector2Int(0, -1);
        }
        return new Vector2Int(0, -1);
    }

    public List<BattleTileScript> GetBattleTiles(List<Vector2Int> pos, WalkingSideType walkingSide)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        foreach (Vector2Int item in pos)
        {
            res.Add(GetBattleTile(item, walkingSide));
        }
        return res;
    }

    public List<BattleTileScript> GetBattleTiles(List<Vector2Int> pos)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        foreach (Vector2Int item in pos)
        {
            res.Add(GetBattleTile(item));
        }
        return res;
    }

    public BattleTileScript GetBattleTile(Vector2Int pos, bool useBoundaries = false, bool excludeNonPlayable = false)
    {
        if (excludeNonPlayable && !IsPosOnField(pos, useBoundaries))
            return null;
        return BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
    }


    public BattleTileScript GetBattleTileClosest(Vector3 pos)
    {
        BattleTileScript closestTile = null;
        foreach (BattleTileScript bts in BattleTiles)
        {
            if (closestTile == null || Vector3.Distance(pos, bts.transform.position) < Vector3.Distance(pos, closestTile.transform.position)) closestTile = bts;
        }
        return closestTile;
    }

    public bool IsPosFree(Vector2Int pos)
        => GetBattleTile(pos).BattleTileState == BattleTileStateType.Empty ? true : false;

    public List<BattleTileScript> GetListUsableTile(Vector2Int startingPos, FacingType facing, WalkingSideType walkSide)
    {
        List<BattleTileScript> res = BattleTiles.Where(r => r.Pos.x == startingPos.x && r.WalkingSide != walkSide && r.BattleTileState != BattleTileStateType.NonUsable && r.BattleTileState != BattleTileStateType.Bound).ToList();

        if (facing == FacingType.Right)
        {
            return res.OrderBy(r => r.Pos.y).ToList();
        }
        else
        {
            return res.OrderByDescending(r => r.Pos.y).ToList();
        }
    }

    public List<BattleTileScript> GetListUsableTile(WalkingSideType walkSide)
    {
        return BattleTiles.Where(r => (r.WalkingSide == walkSide || r.WalkingSide == WalkingSideType.Both || walkSide == WalkingSideType.Both) && r.BattleTileState != BattleTileStateType.NonUsable && r.BattleTileState != BattleTileStateType.Bound).ToList();
    }

    public BattleTileScript GetBattleBestTileInsideTheBattlefield(Vector2Int pos, FacingType facing, AttackTargetSideType ats, WalkingSideType walkSide)
    {
        BattleTileScript res = BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();

        if (IsBtsUsableWithAtsandWalk(res, ats, walkSide))
        {
            return res;
        }

        if (pos.x < 0 || pos.x > 5)
        {
            return null;
        }

        if (ats == AttackTargetSideType.EnemySide)
        {
            List<BattleTileScript> resL = GetListUsableTile(pos, facing, walkSide);
            if ((facing == FacingType.Right && resL.Count > 0 && resL[0].Pos.y < pos.y) || (facing == FacingType.Left && resL.Count > 0 && resL[0].Pos.y > pos.y))
            {
                resL.Reverse();
            }
            for (int i = 0; i < resL.Count; i++)
            {
                res = resL[i];
                if (IsBtsUsableWithAtsandWalk(res, ats, walkSide))
                {
                    return res;
                }
            }
        }
        else if (ats == AttackTargetSideType.FriendlySide)
        {
            List<BattleTileScript> resL = GetListUsableTile(pos, facing, walkSide == WalkingSideType.LeftSide ? WalkingSideType.RightSide : WalkingSideType.LeftSide);
            if ((facing == FacingType.Right && resL.Count > 0 && resL[0].Pos.y < pos.y) || (facing == FacingType.Left && resL.Count > 0 && resL[0].Pos.y > pos.y))
            {
                resL.Reverse();
            }
            for (int i = 0; i < resL.Count; i++)
            {
                res = resL[i];
                if (IsBtsUsableWithAtsandWalk(res, ats, walkSide))
                {
                    return res;
                }
            }
        }
        else if (ats == AttackTargetSideType.BothSides)
        {
            List<BattleTileScript> resL = GetListUsableTile(pos, facing, walkSide);
            if ((facing == FacingType.Right && resL.Count > 0 && resL[0].Pos.y < pos.y) || (facing == FacingType.Left && resL.Count > 0 && resL[0].Pos.y > pos.y))
            {
                resL.Reverse();
            }
            for (int i = 0; i < resL.Count; i++)
            {
                res = resL[i];
                if (IsBtsUsableWithAtsandWalk(res, ats, walkSide))
                {
                    return res;
                }
            }
        }

        return null;
    }


    BaseCharacter temp_cb;
    Vector2Int temp_v2I;
    public Vector2Int GetMeleeAttackBestTile(Vector2Int startingTile, FacingType facing, TeamSideType side, out BaseCharacter target)
    {
        Vector2Int res = temp_v2I = startingTile;
        target = null;
        for (int i = startingTile.y + 1; i <= (facing == FacingType.Right ? 11 : 0); i += ((facing == FacingType.Right ? 1 : -1)))
        {
            Temp_bts = BattleTiles.Where(r => r.Pos == new Vector2Int(startingTile.x, i)).FirstOrDefault();
            if (Temp_bts.BattleTileState == BattleTileStateType.Empty)
            {
                res = new Vector2Int(startingTile.x, i);
            }
            else if (Temp_bts.BattleTileState == BattleTileStateType.Occupied)
            {
                if (temp_v2I == res)
                {
                    if (Temp_bts.cb != null && Temp_bts.cb.CharInfo.Side != side)
                    {
                        target = Temp_bts.cb;
                    }
                    else
                    {
                        temp_cb = BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.Pos.Contains(Temp_bts.Pos) && r.CharInfo.Side != side).FirstOrDefault();
                        if (temp_cb != null)
                        {
                            target = temp_cb;
                        }
                    }
                }

                if (target != null && !target.HasBuffDebuff(BuffDebuffStatsType.ShadowForm))
                {
                    break;
                }
                else
                {
                    target = null;
                }
            }
            temp_v2I = new Vector2Int(startingTile.x, i);
        }

        return res;
    }



    public bool IsBtsUsableWithAtsandWalk(BattleTileScript res, AttackTargetSideType ats, WalkingSideType walkSide)
    {
        if (res != null && res.BattleTileState != BattleTileStateType.NonUsable && res.BattleTileState != BattleTileStateType.Bound)
        {
            if ((ats == AttackTargetSideType.EnemySide && walkSide != res.WalkingSide) ||
                (ats == AttackTargetSideType.FriendlySide && walkSide == res.WalkingSide) ||
                (ats == AttackTargetSideType.BothSides))
            {
                return true;
            }
        }
        return false;
    }


    public BattleTileScript GetBattleBestTileInsideTheBattlefield(Vector2Int pos, FacingType facing)
    {
        pos = new Vector2Int(Mathf.Clamp(pos.x, 0, 5), Mathf.Clamp(pos.y, 0, 11));
        BattleTileScript res = BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
        if (res != null && res.BattleTileState != BattleTileStateType.NonUsable && res.BattleTileState != BattleTileStateType.Bound)
        {
            return res;
        }
        int startValue = pos.y;
        if (facing == FacingType.Left)
        {
            for (int i = pos.y; i < 11; i++)
            {
                res = BattleTiles.Where(r => r.Pos == new Vector2Int(pos.x, i)).FirstOrDefault();
                if (res != null && res.BattleTileState != BattleTileStateType.NonUsable && res.BattleTileState != BattleTileStateType.Bound)// && res.WalkingSide != WalkingSideType.RightSide
                {
                    return res;
                }
            }
        }
        else
        {
            for (int i = pos.y; i > 0; i--)
            {
                res = BattleTiles.Where(r => r.Pos == new Vector2Int(pos.x, i)).FirstOrDefault();
                if (res != null && res.BattleTileState != BattleTileStateType.NonUsable && res.BattleTileState != BattleTileStateType.Bound)// && res.WalkingSide != WalkingSideType.LeftSide
                {
                    return res;
                }
            }
        }

        return null;
    }

    //Get BattleTileScript of the tile
    public BattleTileScript GetBattleTile(Vector2Int pos, WalkingSideType walkingSide)//isEnemyOrPlayer = true/Player false/Enemy
    {
        return BattleTiles.Where(r => r.Pos == pos && (r.WalkingSide == walkingSide || walkingSide == WalkingSideType.Both || r.WalkingSide == WalkingSideType.Both)).FirstOrDefault();
    }

    public BattleTileScript[] GetTilesAdjacentTo(Vector2Int originPos, int withinRadius = 1, bool circularRadius = false, WalkingSideType side = WalkingSideType.Both)
    {
        BattleTileScript originTile = BattleTiles.Where(r => r.Pos == originPos).FirstOrDefault();
        if (originTile == null) return null;

        // Vector2Int curTilePos = new Vector2Int();
        // BattleTileScript curTile = null;

        List<BattleTileScript> adjTiles = BattleTiles.Where(r => r.WalkingSide == side && r.BattleTileState == BattleTileStateType.Empty && Mathf.Abs(originPos.x - r.Pos.x) <= withinRadius
        && Mathf.Abs(originPos.y - r.Pos.y) <= withinRadius).ToList();

        if (adjTiles.Count == 0)
        {
            return null;
        }
        else
        {
            return adjTiles.Distinct().ToArray().OrderBy(r => Mathf.Abs(originPos.x - r.Pos.x) + Mathf.Abs(originPos.y - r.Pos.y)).ToArray();
        }
    }

    public BattleTileScript GetFreeInRowTileAdjacentTo(Vector2Int originPos, WalkingSideType side = WalkingSideType.Both)
    {
        for (int i = 1; i < 12; i++)
        {
            Temp_bts = BattleTiles.Where(r => r.Pos == new Vector2Int(originPos.x, originPos.y + i)).FirstOrDefault();
            if (Temp_bts != null && Temp_bts.BattleTileState == BattleTileStateType.Empty && Temp_bts.WalkingSide == side)
            {
                return Temp_bts;
            }

            Temp_bts = BattleTiles.Where(r => r.Pos == new Vector2Int(originPos.x, originPos.y - i)).FirstOrDefault();
            if (Temp_bts != null && Temp_bts.BattleTileState == BattleTileStateType.Empty && Temp_bts.WalkingSide == side)
            {
                return Temp_bts;
            }
        }
        return null;
    }

    public BattleTileScript[] GetFreeTilesAdjacentTo(Vector2Int originPos, int withinRadius = 1, bool circularRadius = false, WalkingSideType side = WalkingSideType.Both)
    {
        List<BattleTileScript> adjFreeTiles = new List<BattleTileScript>();
        BattleTileScript[] possibleFreeTiles = GetTilesAdjacentTo(originPos, withinRadius, circularRadius, side);
        if (possibleFreeTiles != null)
        {
            foreach (BattleTileScript tile in possibleFreeTiles)
            {
                if (tile._BattleTileState == BattleTileStateType.Empty)
                {
                    adjFreeTiles.Add(tile);
                }
            }
            if (adjFreeTiles.Count == 0)
            {
                return null;
            }
            else
            {
                return adjFreeTiles.ToArray();
            }
        }
        return null;
    }

    public BattleTileScript GetRandomFreeAdjacentTile(Vector2Int originPos, int withinRadius = 1, bool circularRadius = false, WalkingSideType side = WalkingSideType.Both)
    {
        BattleTileScript[] freeAdjTiles = GetFreeTilesAdjacentTo(originPos, withinRadius, circularRadius, side);
        if (freeAdjTiles != null) return freeAdjTiles[Random.Range(0, freeAdjTiles.Length)];
        else return null;
    }

    public void SetBattleTileState(Vector2Int pos, BattleTileStateType battleTileState)
    {

        //Debug.LogError(pos + "    " + battleTileState);

        Temp_bts = BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
        if (Temp_bts == null)
        {
            return;
        }

        Temp_bts.BattleTileState = battleTileState;
        
        if (Temp_bts.BattleTileState == BattleTileStateType.Empty)
        {
            Temp_bts.isTaken = false;
            Temp_bts.cb = null;
        }        
    }

    public BattleTileScript GetFreeBattleTile(TeamSideType side)
    {
        return GetFreeBattleTile(side == TeamSideType.LeftSideTeam ? TeamSideType.LeftSideTeam : TeamSideType.RightSideTeam);
    }

    //Get free tile for a one tile character
    public BattleTileScript GetFreeBattleTile(WalkingSideType walkingSide)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && (r.WalkingSide == walkingSide || walkingSide == WalkingSideType.Both)).ToList();
        int battletileCount = emptyBattleTiles.Count;
        if (emptyBattleTiles.Count == 0) return null;
        return emptyBattleTiles[Random.Range(0, battletileCount)];
    }
    //Get free tile for a more than one tile character
    public BattleTileScript GetFreeBattleTile(WalkingSideType walkingSide, List<Vector2Int> occupiedTiles)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && (r.WalkingSide == walkingSide || walkingSide == WalkingSideType.Both)).ToList();
        bool areOccupiedTileFree = true;
        BattleTileScript emptyTile = null;
        while (emptyBattleTiles.Count > 0)
        {
            emptyTile = emptyBattleTiles[Random.Range(0, emptyBattleTiles.Count)];
            emptyBattleTiles.Remove(emptyTile);
            areOccupiedTileFree = true;
            foreach (Vector2Int item in occupiedTiles)
            {
                BattleTileScript tileToCheck = BattleTiles.Where(r => r.Pos == emptyTile.Pos + (item - occupiedTiles[0])).FirstOrDefault();
                if (tileToCheck == null)
                {
                    areOccupiedTileFree = false;
                    break;
                }
                else
                {
                    if (tileToCheck.BattleTileState != BattleTileStateType.Empty)
                    {
                        areOccupiedTileFree = false;
                        break;
                    }
                }
            }
            if (areOccupiedTileFree)
            {
                return emptyTile;
            }
        }
        return null;
    }






    List<BattleTileScript.Edge> Temp_EdgeList = new List<BattleTileScript.Edge>();
    List<BattleTileScript.Edge> Temp_BoundryList = new List<BattleTileScript.Edge>();
    public BoundryLine[] GetBattleTilesBoundryLines(BattleTileScript[] battleTiles)
    {
        if (battleTiles == null || battleTiles.Length < 1)
            return new BoundryLine[0];

        List<BoundryLine> res = new List<BoundryLine>();

        Temp_EdgeList = new List<BattleTileScript.Edge>();
        List<Vector2Int> completedTilePositions = new List<Vector2Int>();
        for (int i = 0; i < battleTiles.Length; i++)
        {
            if (completedTilePositions.Contains(battleTiles[i].Pos))
                continue;
            completedTilePositions.Add(battleTiles[i].Pos);
            Temp_EdgeList.AddRange(battleTiles[i].Edges);
        }

        Temp_BoundryList = new List<BattleTileScript.Edge>();
        for (int i = 0; i < Temp_EdgeList.Count; i++)
        {
            if (Temp_BoundryList.Where(r => r.IsSharedBy(Temp_EdgeList[i])).Count() == 0 && Temp_EdgeList.Where(r => r.IsSharedBy(Temp_EdgeList[i])).Count() < 2)
                Temp_BoundryList.Add(Temp_EdgeList[i]);
        }

        List<Vector3> temp_Line = new List<Vector3>();
        Vector2Int lastOrigin = Vector2Int.one * -500;
        BattleTileScript.Edge temp_Edge;
        while (Temp_BoundryList.Count > 0)
        {
            while (true)
            {
                temp_Edge = temp_Line.Count == 0 ? Temp_BoundryList[0] : Temp_BoundryList.Where(r => r.First == temp_Line.Last() /*&& r.TileOrigin == lastOrigin*/).FirstOrDefault();
                if (temp_Line.Count > 0 && temp_Line.First() == temp_Line.Last())
                    break;
                if (temp_Edge.Vertices == null || temp_Edge.Vertices.Length == 0)
                    temp_Edge = temp_Edge;//Temp_BoundryList.Where(r => (r.Last == temp_Line[0] || r.Last == temp_Line[1]) && r.TileOrigin == lastOrigin).FirstOrDefault();
                if (temp_Edge.Vertices == null || temp_Edge.Vertices.Length == 0)
                    temp_Edge = temp_Edge;//Temp_BoundryList.Where(r => r.First == temp_Line.Last()).FirstOrDefault();
                if (temp_Edge.Vertices == null || temp_Edge.Vertices.Length == 0)
                    break;
                lastOrigin = temp_Edge.TileOrigin;
                temp_Line.AddRange(temp_Edge.Vertices);
                Temp_BoundryList.Remove(temp_Edge);
            }
            res.Add(new BoundryLine(temp_Line.ToArray()));
            temp_Line = new List<Vector3>();
        }

        //Debug.LogError(Temp_EdgeList.Count.ToString() + " -> " + Temp_BoundryList.Count.ToString());
        //for (int i = 0; i < Temp_BoundryList.Count; i++)
        //{
        //    Debug.LogError(Temp_BoundryList[i].Vertices[0].ToString() + " ---> " + Temp_BoundryList[i].Vertices[1].ToString());
        //}

        return res.ToArray();
    }

    public struct BoundryLine
    {
        public Vector3[] LineCoords;
        public Vector3[] LineCoordsNoDupes
        {
            get
            {
                List<Vector3> res = new List<Vector3>();
                for (int i = 0; i < LineCoords.Length; i++)
                {
                    if (!res.Contains(LineCoords[i]))
                        res.Add(LineCoords[i]);
                }
                return res.ToArray();
            }
        }

        public BoundryLine(Vector3[] LineCoords)
        {
            this.LineCoords = LineCoords;
            //Debug.LogError(LogContents());
        }

        public string LogContents()
        {
            string res = "LINE:\n";
            foreach (Vector3 lineCoord in LineCoords)
            {
                res += lineCoord.ToString() + ",\n";
            }
            return res;
        }
    }







 
}


public class PortalInfoClass
{
    public BattleTileScript PortalPos;
    public PortalType Portal;
    public int IDPortal;

    public PortalInfoClass()
    {
    }

    public PortalInfoClass(BattleTileScript portalPos, PortalType portal, int idPortal)
    {
        PortalPos = portalPos;
        Portal = portal;
        IDPortal = idPortal;
    }
}