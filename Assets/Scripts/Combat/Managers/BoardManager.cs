using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

	const float rowIncrease = 1.5f;
	const float columnIncrease = 1.5f;
	Quaternion rotation = Quaternion.Euler(-90, 0, 0); 

    [Space]
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform BoardParent;

    public TileColor originalColor;
    public TileColor MovementColor;
    public TileColor MouseOverColor;

    LineRenderer movementLR;
    public List<BoardTile> Path = new List<BoardTile>();
    public BoardTile currentMouseTile;

    public Vector2Int[] Directions;

    public void Init()
    {
        Instance = this;
        movementLR = GetComponent<LineRenderer>();
        Directions = GetDirections();
    }

    public void CreateBoard() 
    {
        BoardData.BoardTiles = new BoardTile[BoardData.rowAmount, BoardData.columnAmount];

        for (int x = 0; x < BoardData.rowAmount; x++)
        {
            for (int y = 0; y < BoardData.columnAmount; y++)
            {
                Vector3 position = new Vector3(
                    x * rowIncrease,
                    0,
                    y * columnIncrease
                    );

                var newTile = Instantiate(tilePrefab, position, rotation, BoardParent);
				BoardTile tile = newTile.GetComponent<BoardTile>();
				BoardData.BoardTiles[x, y] = tile;
				tile.Init(x, y);
			}
        }

		foreach (BoardTile tile in BoardData.BoardTiles)
			tile.SetConnectedTiles();
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
            SetSkillAOE(data.Range + 1, startingTile, data);
		}
    }

    public void SetMovementAOE(float movementLeft, BoardTile currentTile)
    {
        if (movementLeft <= 0)
            return;

        // can't move to current space
        if (currentTile.movementLeft == -1)
            currentTile.movementLeft = 0;

        if (UnitData.CurrentActiveUnit.Friendly == false)
		{
            var enemy = UnitData.CurrentActiveUnit as Enemy;

            if (!enemy.PossibleMovementTiles.Contains(currentTile))
                enemy.PossibleMovementTiles.Add(currentTile);
		}

        foreach(var tile in currentTile.connectedTiles)
        {
            if (tile == null || tile.currentUnit != null)
                continue;

			var nextMovementLeft = movementLeft;
            nextMovementLeft -= GetRangeReduction(currentTile, tile);

            if (tile.movementLeft < nextMovementLeft)
            {
                if (UnitData.CurrentActiveUnit.Friendly)
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

        // can't move to current space
        if (currentTile.movementLeft == -1)
            currentTile.movementLeft = 0;

        var skillPartIndex = skillData.SkillPartIndex;

		foreach (var tile in currentTile.connectedTiles)
        {
            if (tile == null)
                continue;

            var nextSkillRange = skillRangeLeft;
            nextSkillRange -= GetRangeReduction(currentTile, tile);

            var index = skillData.SkillPartIndex;
            while (tile.skillshotsRangeLeft.Count <= index)
                tile.skillshotsRangeLeft.Add(0);

            if (tile.skillshotsRangeLeft[index] < nextSkillRange)
            {
                if (UnitData.CurrentActiveUnit.Friendly)
                    tile.SetColor(skillData.tileColor);

                tile.skillshotsRangeLeft[index] = nextSkillRange;

                SkillData.AddTileToCurrentList(skillPartIndex, tile);

                var target = FindTarget(tile, skillData);
                SkillData.AddTargetToCurrentList(skillPartIndex, target);

                tile.PreviousTile = currentTile;

                SetSkillAOE(nextSkillRange, tile, skillData);
            }
        }
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
                
            for (float r = 0; r < skillData.Range;)
			{
                curentCoordinates = nextCoordinates;
                nextCoordinates = curentCoordinates + Directions[direction];
                line.Add(nextCoordinates);

                r += GetRangeReduction(curentCoordinates, nextCoordinates);
                curentCoordinates = nextCoordinates;
            }

            foreach (var coordinate in line)
			{
                var tile = GetBoardTile(coordinate);

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

    public void PreviewConeCast(int direction, SO_ConeSkill skillData)
    {
        Vector2Int startCoordinates = SkillData.Caster.currentTile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<List<Vector2Int>> lines = new List<List<Vector2Int>>();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillData.SkillPartIndex;

        var width = skillData.isWide ? 2 : 1;

        foreach (var originTile in skillData.OriginTiles)
        {
            for (int i = -1; i < width; i++)
            {
                var dir = GetCorrectedDirection(direction + i);

                line = new List<Vector2Int>();
                curentCoordinates = nextCoordinates = startCoordinates;

                for (float r = 0; r < skillData.Range;)
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

                    tile.SetColor(skillData.tileColor);

                    SkillData.AddTileToCurrentList(skillPartIndex, tile);

                    var target = FindTarget(tile, skillData);
                    SkillData.AddTargetToCurrentList(skillPartIndex, target);
                }
                lines.Add(line);
            }
            
            if (skillData.isWide == false)
			{
                FillConeCast(lines, skillData);
			}
            if (skillData.isWide)
			{
                var firstList = new List<List<Vector2Int>> { lines[0], lines[1] };
                FillConeCast(firstList, skillData);
                var secondList = new List<List<Vector2Int>> { lines[1], lines[2] };
                FillConeCast(secondList, skillData);
            }

            lines.Clear();
        }
    }

    void FillConeCast(List<List<Vector2Int>> lines, SO_Skillpart skillData)
    {
		List<BoardTile> tileList = new List<BoardTile>();

        var skillPartIndex = skillData.SkillPartIndex;

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
			var tile = tileList[t];

			tile.SetColor(skillData.tileColor);

            SkillData.AddTileToCurrentList(skillPartIndex, tile);

			var target = FindTarget(tile, skillData);
            SkillData.AddTargetToCurrentList(skillPartIndex, target);
        }
	}

    public void PreviewHalfCircleCast(int direction, SO_HalfCircleSkill skillData)
    {
        Vector2Int startCoordinates = SkillData.Caster.currentTile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<List<Vector2Int>> lines = new List<List<Vector2Int>>();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillData.SkillPartIndex;

        foreach (var originTile in skillData.OriginTiles)
        {
            for (int i = -2; i < 3; i++)
            {
                var dir = GetCorrectedDirection(direction + i);

                line = new List<Vector2Int>();
                curentCoordinates = nextCoordinates = startCoordinates;

                for (float r = 0; r < skillData.Range;)
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

                    tile.SetColor(skillData.tileColor);

                    SkillData.AddTileToCurrentList(skillPartIndex, tile);

                    var target = FindTarget(tile, skillData);
                    SkillData.AddTargetToCurrentList(skillPartIndex, target);
                }
                lines.Add(line);
            }

            for (int i = 1; i < 5; i++)
			{
                var fillLines = new List<List<Vector2Int>> { lines[i-1], lines[i] };
                FillConeCast(fillLines, skillData);
			}

            lines.Clear();
        }
    }

    int GetCorrectedDirection(int dir)
	{
        var direction = dir % 8;
        while (direction < 0)
            direction += 8;

        return direction;
    }

    Unit FindTarget(BoardTile tile, SO_Skillpart data)
    {
        if (UnitData.CurrentActiveUnit == null)
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

        var friendly = UnitData.CurrentActiveUnit.Friendly;

        if (SkillData.CurrentActiveSkill.TargetKind == SO_MainSkill.TargetKindEnum.Allies)
        {
            if (target.Friendly == friendly)
                return true;
        }

        if (SkillData.CurrentActiveSkill.TargetKind == SO_MainSkill.TargetKindEnum.All)
            return true;

        return (target.Friendly != friendly);
    }

    public void PreviewMovementLine(BoardTile finaltile)
    {
        movementLR.positionCount++;
        movementLR.SetPosition(0, finaltile.position);

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
                    movementLR.SetPosition(i+1, tile.position);
                    tile.PreviousTile = currentTile;
                    currentTile = tile;
					Path.Add(tile);

                    continue;
                }

                if (tile.movementLeft == currentTile.movementLeft + 1.5f)
                {
                    movementLR.positionCount++;
                    movementLR.SetPosition(i + 1, tile.position);
                    tile.PreviousTile = currentTile;
                    currentTile = tile;
                    Path.Add(tile);

                    continue;
                }
            }
        }
        movementLR.positionCount++;
        movementLR.SetPosition(Path.Count, UnitData.CurrentActiveUnit.currentTile.position);
    }

    public void SetPath(BoardTile endTile)
	{
        var currentTile = endTile;

        if (currentTile == null)
            return;

        while (currentTile != null)
		{
            Path.Add(currentTile);
            currentTile = currentTile.PreviousTile;
        }
    }

    public void StopShowingMovement()
    {
        movementLR.positionCount = 0;
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

        yield return StartCoroutine(UnitData.CurrentActiveUnit.Move(Path));
    }
}
