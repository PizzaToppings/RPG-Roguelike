using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    SkillsManager skillsManager;

    [Space]
    [SerializeField] Transform BoardParent;

    public TileColor originalColor;
    public TileColor MovementColor;
    public TileColor MouseOverColor;

    LineRenderer movementLR;
    Vector3 MovementLineOffset = Vector3.up * 0.2f;

    [HideInInspector] public List<BoardTile> Path = new List<BoardTile>();
    BoardTile currentMouseTile;

    public Vector2Int[] Directions;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        movementLR = GetComponent<LineRenderer>();
        Directions = GetDirections();
        skillsManager = SkillsManager.Instance;
    }

    public void AddBoardTilesToList()
    {
        BoardData.BoardTiles = new BoardTile[BoardData.rowAmount, BoardData.columnAmount];

        foreach (Transform child in BoardParent.transform)
        {
            var tile = child.GetComponent<BoardTile>();
            tile.Init();
            BoardData.BoardTiles[tile.xPosition, tile.yPosition] = tile;
        }
    }

    public BoardTile GetBoardTile(int xPosition, int yPosition)
    {
        if (Existingtile(xPosition, yPosition) == false)
            return null;

        return BoardData.BoardTiles[xPosition, yPosition];
    }

    public BoardTile GetBoardTile(Vector2Int position)
    {
        if (Existingtile(position.x, position.y) == false)
            return null;

        return BoardData.BoardTiles[position.x, position.y];
    }

    bool Existingtile(int x, int y)
    {
        return (x >= 0 && x < BoardData.rowAmount &&
                    y >= 0 && y < BoardData.columnAmount);
    }

    public float GetRangeBetweenTiles(Vector2 startTile, Vector2 EndTile)
    {
        var range = 0f;

        var xDifference = (int)Mathf.Abs(startTile.x - EndTile.x);
        var yDifference = (int)Mathf.Abs(startTile.y - EndTile.y);

        var lowerValue = xDifference < yDifference ? xDifference : yDifference;
        var HigherValue = xDifference > yDifference ? xDifference : yDifference;

        var diagonalTiles = lowerValue;
        var straightTiles = HigherValue - lowerValue;

        range += diagonalTiles * 1.5f;
        range += straightTiles;

        return range;
    }

    public List<BoardTile> GetDirectPathBetweenTiles(BoardTile startTile, BoardTile endTile)
	{
        var path = new List<BoardTile>();

        while (startTile != endTile)
		{
            var d = GetDirectionBetweenTiles(startTile, endTile);
            var direction = Directions[d];

            var newCoordinates = startTile.Coordinates + direction;
            startTile = GetBoardTile(newCoordinates);

            path.Add(startTile);
        }

        return path;
	}

	int GetDirectionBetweenTiles(BoardTile startTile, BoardTile endTile)
	{
		var tileDirectionIndex = 0;
		Vector3 dir = (endTile.position - startTile.position).normalized;
		Vector2 direction = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.z));

		for (int i = 0; i < Directions.Length; i++)
		{
			if (direction == Directions[i])
			{
				tileDirectionIndex = i;
				break;
			}
		}

		return tileDirectionIndex;
	}

    public List<BoardTile> getTilesWithinDirectRange(BoardTile starttile, float range)
    {
        var tileList = new List<BoardTile>();
        foreach (var tile in BoardData.BoardTiles)
        {
            if (starttile == tile)
                continue;

            if (GetRangeBetweenTiles(starttile.Coordinates, tile.Coordinates) <= range)
			{
                if (tile.currentUnit == null || tile.currentUnit == UnitData.ActiveUnit)
                    tileList.Add(tile);
			}
        }

        return tileList;
    }

    public List<BoardTile> GetTilesInDirectAttackRange(BoardTile tile, float attackRange, bool includeInMoveRange)
    {
        var attackTilesInRange = getTilesWithinDirectRange(tile, attackRange);

        if (includeInMoveRange)
		{
            attackTilesInRange = attackTilesInRange.Where(x => x.movementLeft > -1f).ToList();
            if (attackTilesInRange.Count == 0)
                return null;
		}

        var attackerTile = UnitData.ActiveUnit.Tile;
        var tilesOrdened = attackTilesInRange.OrderBy(
            x => GetRangeBetweenTiles(attackerTile.Coordinates, x.Coordinates)).ToList();

        if (attackTilesInRange.Count == 0)
            return null;

        return tilesOrdened;
    }

    public int GetRangeBetweenTiles(BoardTile startTile, BoardTile endTile)
    {
        List<BoardTile> openSet = new List<BoardTile>();
        HashSet<BoardTile> closedSet = new HashSet<BoardTile>();

        startTile.DistanceToTarget = Vector2.Distance(startTile.Coordinates, endTile.Coordinates);
        startTile.DistanceTraveled = 0;

        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            BoardTile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].DistanceToTarget < currentTile.DistanceToTarget ||
                    (openSet[i].DistanceToTarget == currentTile.DistanceToTarget &&
                     openSet[i].DistanceToTarget < currentTile.DistanceToTarget))
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if (currentTile.Coordinates == endTile.Coordinates)
            {
                return (int)currentTile.DistanceTraveled;
            }

            foreach (var neighbor in currentTile.connectedTiles)
            {
                if (closedSet.Contains(neighbor) || neighbor == null) 
                    continue;

                float newDistanceTraveled = currentTile.DistanceTraveled + 1;

                if (newDistanceTraveled < neighbor.DistanceTraveled || !openSet.Contains(neighbor))
                {
                    neighbor.DistanceTraveled = newDistanceTraveled;
                    neighbor.DistanceToTarget = Vector2.Distance(neighbor.Coordinates, endTile.Coordinates);
                    neighbor.PreviousTile = currentTile;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return-1;
    }

    public void Clear()
    {
        foreach (var tile in BoardData.BoardTiles)
        {
            tile.movementLeft = -1;
            tile.PreviousTile = null;
            Path = new List<BoardTile>();
			tile.OverrideColor(originalColor);
			tile.skillshotsRangeLeft = new List<float>();
        }
        StopShowingMovement();
    }

    public void VisualClear()
	{
        foreach (var tile in BoardData.BoardTiles)
        {
			tile.OverrideColor(originalColor);
			tile.skillshotsRangeLeft = new List<float>();
		}
        StopShowingMovement();
    }

    public void SetAOE(float movementLeft, List<BoardTile> startingTiles, SO_Skillpart data)
    {
        foreach (var tile in startingTiles)
        {
            SetAOE(movementLeft, tile, data);
        }
    }

    public void SetAOE(float movementLeft, BoardTile startingTile, SO_Skillpart data)
    {
        if (data == null)
        {
            SetMovementAOE(movementLeft, startingTile);
        }
        else
		{
            SetSkillAOE(data.MaxRange, startingTile, data);
            FilterAOESkillTileList(data.SkillPartIndex, data);
        }
    }

    public void SetMovementAOE(float movementLeft, BoardTile currentTile)
    {
        if (movementLeft <= 0)
            return;

        // can't move to current space 
        if (currentTile.movementLeft == -1)
            currentTile.movementLeft = 0;

        if (UnitData.ActiveUnit.Friendly == false)
		{
            var enemy = UnitData.ActiveUnit as Enemy;

            if (!enemy.PossibleMovementTiles.Contains(currentTile))
                enemy.PossibleMovementTiles.Add(currentTile);
		}

        foreach(var tile in currentTile.connectedTiles)
        {
            if (tile == null || tile.currentUnit != null || tile.IsBlocked)
                continue;

			var nextMovementLeft = movementLeft;
            nextMovementLeft -= GetRangeReduction(currentTile, tile);

            if (tile.movementLeft < nextMovementLeft)
            {
                if (UnitData.ActiveUnit.Friendly)
                    tile.SetColor(MovementColor);

                tile.movementLeft = nextMovementLeft;

                tile.PreviousTile = currentTile;

                SetMovementAOE(nextMovementLeft, tile);
            }
        }
    }

    public void SetSkillAOE(float skillRangeLeft, BoardTile currentTile, SO_Skillpart skillData)
    {
        if (skillRangeLeft <= 0)
            return;

        var skillPartIndex = skillData.SkillPartIndex;

		foreach (var tile in currentTile.connectedTiles)
        {
            if (tile == null)
                continue;

            var nextSkillRange = skillRangeLeft;
            nextSkillRange -= GetRangeReduction(currentTile, tile);

            var index = skillData.SkillPartIndex;
            while (tile.skillshotsRangeLeft.Count <= index)
                tile.skillshotsRangeLeft.Add(-0.5f);

            if (nextSkillRange > tile.skillshotsRangeLeft[index])
            {
                tile.skillshotsRangeLeft[index] = nextSkillRange;

				SkillData.AddTileToCurrentList(skillPartIndex, tile);

                tile.PreviousTile = currentTile;

                SetSkillAOE(nextSkillRange, tile, skillData);
            }
        }
    }

    public void FilterAOESkillTileList(int skillPartIndex, SO_Skillpart skillData)
    {
        var tiles = new List<BoardTile>();

        foreach (var tile in SkillData.GetCurrentTilesHit(skillPartIndex))
        {
            if (skillData.MaxRange - tile.skillshotsRangeLeft[skillPartIndex] < skillData.MinRange)
                continue;

            if (tile.IsBlocked || (TileIsBehindClosedTile(tile, skillData.OriginTiles[0]) && skillData.AffectedByBlockedTiles))
                continue;

            if (UnitData.ActiveUnit.Friendly)
                tile.SetColor(skillData.tileColor);

            var target = FindTarget(tile, skillData);
            SkillData.AddTargetToCurrentList(skillPartIndex, target);

            tiles.Add(tile);
        }

        skillData.PartData.TilesHit = tiles;
    }

    public bool TileIsBehindClosedTile(BoardTile startTile, BoardTile endTile)
	{
        var path = GetDirectPathBetweenTiles(startTile, endTile);
        return path.Any(x => x.IsClosed);
            
	}

	public float GetRangeReduction(BoardTile currentTile, BoardTile nextTile)
	{
        if (nextTile.xPosition != currentTile.xPosition
                && nextTile.yPosition != currentTile.yPosition)
        {
            return 1.5f;
        }
        else
        {
            return 1;
        }
    }

    public float GetRangeReduction(Vector2 currentCoordinates, Vector2 nextCoordinates)
    {
        if (currentCoordinates.x != nextCoordinates.x
                && currentCoordinates.y != nextCoordinates.y)
        {
            return 1.5f;
        }
        else
        {
            return 1;
        }
    }

    public void PreviewLineCast(BoardTile originTile, int[] directions, SO_LineSkill skillData)
    {
        Vector2Int startCoordinates = originTile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillData.SkillPartIndex;
        int pierceAmount = skillData.PierceAmount;
        
        foreach (var dir in directions)
        {
            curentCoordinates = nextCoordinates = startCoordinates;
            var direction = GetCorrectedDirection(dir);
                
            for (float r = 0; r < skillData.MaxRange;)
			{
                curentCoordinates = nextCoordinates;
                nextCoordinates = curentCoordinates + Directions[direction];

                if (r >= skillData.MinRange)
                    line.Add(nextCoordinates);

                r += GetRangeReduction(curentCoordinates, nextCoordinates);
                curentCoordinates = nextCoordinates;
            }

            foreach (var coordinate in line)
			{
                var tile = GetBoardTile(coordinate);

                if (tile.IsBlocked && skillData.AffectedByBlockedTiles)
                    break;

                if (tile == null)
                    continue;

                tile.SetColor(skillData.tileColor);

                SkillData.AddTileToCurrentList(skillPartIndex, tile);

                var target = FindTarget(tile, skillData);
                if (target != null)
                {
                    SkillData.AddTargetToCurrentList(skillPartIndex, target);
                    if (pierceAmount != -1)
                    {
                        if (pierceAmount == 0)
                            break;
                        pierceAmount--;
                    }
                }
            }
            line.Clear();
        }
    }

    public void PreviewConeCast(int direction, SO_ConeSkill skillpart)
    {
        Vector2Int startCoordinates = SkillData.Caster.Tile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<List<Vector2Int>> lines = new List<List<Vector2Int>>();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillpart.SkillPartIndex;

        var width = skillpart.isWide ? 2 : 1;

        foreach (var originTile in skillpart.OriginTiles)
        {
            for (int i = -1; i < width; i++)
            {
                var dir = GetCorrectedDirection(direction + i);

                line = new List<Vector2Int>();
                curentCoordinates = nextCoordinates = startCoordinates;

                for (float r = 0; r < skillpart.MaxRange;)
                {
                    curentCoordinates = nextCoordinates;
                    nextCoordinates = curentCoordinates + Directions[dir];
                    line.Add(nextCoordinates);

                    r += GetRangeReduction(curentCoordinates, nextCoordinates);
                    curentCoordinates = nextCoordinates;
                }

                foreach (var coordinate in line)
                {
                    var tile = GetBoardTile(coordinate);

                    if (tile == null)
                        continue;

                    if (GetRangeBetweenTiles(originTile.Coordinates, coordinate) < skillpart.MinRange)
                        continue;

                    if (tile.IsBlocked || (TileIsBehindClosedTile(tile, skillpart.OriginTiles[0]) && skillpart.AffectedByBlockedTiles))
                        continue;

                    SkillData.AddTileToCurrentList(skillpart.SkillPartIndex, tile);
                }
                lines.Add(line);
            }
            
            if (skillpart.isWide == false)
			{
                FillConeCast(lines, skillpart, originTile);
			}
            else
			{
                var firstList = new List<List<Vector2Int>> { lines[0], lines[1] };
                FillConeCast(firstList, skillpart, originTile);
                var secondList = new List<List<Vector2Int>> { lines[1], lines[2] };
                FillConeCast(secondList, skillpart, originTile);
            }

            lines.Clear();
        }
    }

    void FillConeCast(List<List<Vector2Int>> lines, SO_Skillpart skillpart, BoardTile originTile)
    {
		List<BoardTile> tileList = new List<BoardTile>();

        var skillPartIndex = skillpart.SkillPartIndex;

        lines.Sort((x, y) => x.Count.CompareTo(y.Count));
		var shortLine = lines[0];
		var longLine = lines[1];

        var direction = shortLine[0] - longLine[0];
        var range = 0;

        for (int i = 1; i < shortLine.Count; i++)
		{
            range = i + 1;
            for (int r = 1; r < range; r++)
			{
                var coordinates = longLine[i] + direction * r;
                var tile = GetBoardTile(coordinates);

                if (tile != null && tileList.Contains(tile) == false)
                    tileList.Add(tile);
            }
		}

        var rangeReductor = 1;
        for (int i = shortLine.Count; i < longLine.Count; i++)
        {
            range = Mathf.RoundToInt(range / rangeReductor);
            for (int r = 1; r < range; r++)
            {
                var coordinates = longLine[i] + direction * r;
                var tile = GetBoardTile(coordinates);

                if (tile != null)
                    tileList.Add(tile);
            }
            rangeReductor++;
        }

        for (int t = 0; t < tileList.Count; t++)
		{
            if (GetRangeBetweenTiles(originTile, tileList[t]) < skillpart.MinRange)
                continue;

            var tile = tileList[t];
           
            if (tile.IsBlocked || (TileIsBehindClosedTile(tile, skillpart.OriginTiles[0]) && skillpart.AffectedByBlockedTiles))
                continue;

            SkillData.AddTileToCurrentList(skillpart.SkillPartIndex, tile);
        }
	}

    public void PreviewHalfCircleCast(int direction, SO_HalfCircleSkill skillpart)
    {
        Vector2Int startCoordinates = SkillData.Caster.Tile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<List<Vector2Int>> lines = new List<List<Vector2Int>>();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillpart.SkillPartIndex;

        foreach (var originTile in skillpart.OriginTiles)
        {
            for (int i = -2; i < 3; i++)
            {
                var dir = GetCorrectedDirection(direction + i);

                line = new List<Vector2Int>();
                curentCoordinates = nextCoordinates = startCoordinates;

                for (float r = 0; r < skillpart.MaxRange;)
                {
                    curentCoordinates = nextCoordinates;
                    nextCoordinates = curentCoordinates + Directions[dir];
                    line.Add(nextCoordinates);

                    r += GetRangeReduction(curentCoordinates, nextCoordinates);
                    curentCoordinates = nextCoordinates;
                }

                foreach (var coordinate in line)
                {
                    var tile = GetBoardTile(coordinate);

                    if (tile == null)
                        continue;

                    if (GetRangeBetweenTiles(originTile.Coordinates, coordinate) < skillpart.MinRange)
                        continue;

                    SkillData.AddTileToCurrentList(skillpart.SkillPartIndex, tile);
                }
                lines.Add(line);
            }

            for (int i = 1; i < 5; i++)
			{
                var fillLines = new List<List<Vector2Int>> { lines[i-1], lines[i] };
                FillConeCast(fillLines, skillpart, originTile);
			}

            lines.Clear();
        }
    }

    public void CorrectConeCast(BoardTile tile, SO_Skillpart data)
	{
        var tilesHitOld = new List<BoardTile>(data.PartData.TilesHit);
        
        data.PartData.TilesHit.Clear();
        SetSkillAOE(data.MaxRange, tile, data);

		var finalPath = tilesHitOld.Intersect(data.PartData.TilesHit).ToList();
		data.PartData.TilesHit = finalPath;
        data.PartData.TargetsHit.Clear();

		data.PartData.TilesHit.ForEach(t => skillsManager.TargetTileWithSkill(t, data));
	}

    int GetCorrectedDirection(int dir)
	{
        var direction = dir % 8;
        while (direction < 0)
            direction += 8;

        return direction;
    }

    public Unit FindTarget(BoardTile tile, SO_Skillpart data)
    {
        if (UnitData.ActiveUnit == null)
            return null;

        if (tile.currentUnit == null)
            return null;

        if (IsCorrectTarget(tile.currentUnit, data))
            return tile.currentUnit;

        return null;
    }

    bool IsCorrectTarget(Unit target, SO_Skillpart data)
    {
        if (data == null)
            return false;

        var friendly = UnitData.ActiveUnit.Friendly;

        if (SkillData.CurrentActiveSkill.TargetKind == TargetKindEnum.Allies)
        {
            if (target.Friendly == friendly)
                return true;
        }

        if (SkillData.CurrentActiveSkill.TargetKind == TargetKindEnum.All)
            return true;

        return (target.Friendly != friendly);
    }

    public void PreviewMovementLine(BoardTile finaltile)
    {
        movementLR.positionCount++;
        movementLR.SetPosition(0, finaltile.position + MovementLineOffset);

        BoardTile currentTile = finaltile;
		Path.Add(currentTile);

		for (int i = 0; i < Path.Count; i++)
        {
            foreach (var tile in currentTile.connectedTiles)
            {
                if (tile == null)
                    continue;

                if (tile.movementLeft == currentTile.movementLeft + 1)
                {
                    movementLR.positionCount++;
                    movementLR.SetPosition(i+1, tile.position + MovementLineOffset);
                    tile.PreviousTile = currentTile;
                    currentTile = tile;
					Path.Add(tile);

                    continue;
                }

                if (tile.movementLeft == currentTile.movementLeft + 1.5f)
                {
                    movementLR.positionCount++;
                    movementLR.SetPosition(i + 1, tile.position + MovementLineOffset);
                    tile.PreviousTile = currentTile;
                    currentTile = tile;
                    Path.Add(tile);

                    continue;
                }
            }
        }
        movementLR.positionCount++;
        movementLR.SetPosition(Path.Count, UnitData.ActiveUnit.Tile.position + MovementLineOffset);
    }

    public void StopShowingMovement()
    {
        movementLR.positionCount = 0;
    }

    public void SetPath(BoardTile endTile)
	{
        var currentTile = endTile;

        if (currentTile == null)
            return;

        //      while (currentTile != null)
        //{
        //          Path.Add(currentTile);
        //          currentTile = currentTile.PreviousTile;
        //      }

        Path = GetDirectPathBetweenTiles(UnitData.ActiveUnit.Tile, endTile);
    }

    Vector2Int[] GetDirections()
	{
        // going in a circle
        Vector2Int[] directions = new Vector2Int[8];
        directions[0] = new Vector2Int(0, 1);
        directions[1] = new Vector2Int(1, 1);
        directions[2] = new Vector2Int(1, 0);
        directions[3] = new Vector2Int(1, -1);
        directions[4] = new Vector2Int(0, -1);
        directions[5] = new Vector2Int(-1, -1);
        directions[6] = new Vector2Int(-1, 0);
        directions[7] = new Vector2Int(-1, 1);

        return directions;
    }

    public IEnumerator MoveToTile()
    {
        UnitData.CurrentAction = CurrentActionKind.Animating;
        Path.Reverse();

        yield return StartCoroutine(UnitData.ActiveUnit.Move(Path));
    }

    public BoardTile GetCurrentMouseTile()
    {
        return currentMouseTile;
    }
    
    public void SetCurrentMouseTile(BoardTile tile)
	{
        currentMouseTile = tile;
    }

    public IEnumerator DeselectCurrentMouseTile(BoardTile tile)
	{
        yield return new WaitForSeconds(0.1f);

        if (currentMouseTile == tile)
            currentMouseTile = null;
	}
}
