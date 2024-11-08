using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

	const float rowIncrease = 1.5f;
	const float columnIncrease = 1.5f;
	Quaternion rotation = Quaternion.Euler(90, 0, 0); 

    [Space]
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform BoardParent;

    // temp
    [SerializeField] Color originalColor;

    LineRenderer movementLR;
    public List<BoardTile> Path = new List<BoardTile>();

    public Color MovementColor;
    

    public void Init()
    {
        Instance = this;
        movementLR = GetComponent<LineRenderer>();
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
                BoardData.BoardTiles[x,y] = tile;
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
            tile.gameObject.GetComponent<Renderer>().materials[0].color = originalColor;
        }
        StopShowingMovement();
    }

    //public void ClearMovementLeftPerTile()
    //{
    //    foreach (var tile in BoardData.BoardTiles)
    //    {
    //        tile.movementLeft = -1;
    //    }
    //}

     public void SetAOE(float movementLeft, List<BoardTile> startingTiles, SO_Skillpart data)
     {
        foreach (var tile in startingTiles)
        {
            SetAOE(movementLeft, tile, MovementColor, data);
        }
     }

    public void SetAOE(float movementLeft, List<BoardTile> startingTiles,  Color color, SO_Skillpart data)
    {
        foreach (var tile in startingTiles)
        {
            SetAOE(movementLeft, tile, color, data);
        }
    }

    public void SetAOE(float movementLeft, BoardTile startingTile, SO_Skillpart data)
    {
        SetAOE(movementLeft, startingTile, MovementColor, data);
    }

    public void SetAOE(float movementLeft, BoardTile currentTile, Color color, SO_Skillpart skillshotData)
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

        List<BoardTile> usedTiles = new List<BoardTile>();

        foreach(var tile in currentTile.connectedTiles)
        {
            if (tile == null)
                continue;

            var nextMovementLeft = movementLeft;

            if (tile.xPosition != currentTile.xPosition 
                && tile.yPosition != currentTile.yPosition)
			{
                nextMovementLeft -= 1.5f;
			}
            else
			{
                nextMovementLeft--;
			}

            if (tile.movementLeft < nextMovementLeft)
            {
                if (UnitData.CurrentActiveUnit.Friendly)
                    tile.gameObject.GetComponent<Renderer>().materials[0].color = color;

                tile.movementLeft = nextMovementLeft;
                usedTiles.Add(tile);
                
                var target = FindTarget(tile, skillshotData);
                if (target != null && skillshotData != null && !skillshotData.TargetsHit.Contains(target))
                    skillshotData.TargetsHit.Add(target);

                tile.PreviousTile = currentTile;

                SetAOE(nextMovementLeft, tile, color, skillshotData);
            }
        }
        if (skillshotData != null)
            skillshotData.TilesHit.AddRange(usedTiles);
    }

    public void PreviewLineCast(int[] directions, SO_LineSkill skillData)
    {
        BoardTile nextTile = skillData.OriginTiles[0];

        int pierceAmount = skillData.PierceAmount;

        foreach (var originTile in skillData.OriginTiles)
        {
            var target = FindTarget(nextTile, skillData);
            foreach (var dir in directions)
            {
                nextTile = originTile;
                skillData.TilesHit.Add(nextTile);
                for (int i = 0; i < skillData.Range; i++)
                {
                    var direction = dir % nextTile.connectedTiles.Length;

                    nextTile.gameObject.GetComponent<Renderer>().materials[0].color = skillData.tileColor;
                    nextTile = nextTile.connectedTiles[direction];

                    if (nextTile == null)
                        break;

                    skillData.TilesHit.Add(nextTile);
                    target = FindTarget(nextTile, skillData);
                    if (target != null) 
                    {
                        AddTarget(target, skillData);
                        if (pierceAmount != -1)
                        {
                            if (pierceAmount == 0)
                                break;
                            pierceAmount--;
                        }
                    }
                }
            }
        }
    }

    public void PreviewConeCast(int direction, SO_ConeSkill data)
    {
        BoardTile nextTile = data.OriginTiles[0];

        foreach (var originTile in data.OriginTiles)
        {
            nextTile = originTile;
            data.TilesHit.Add(nextTile);
            for (int i = 0; i < data.Range; i++)
            {
                int nextRange = i+2;
                var dir = direction % 6;

                if (!nextTile.connectedTiles[dir])
                {
                    if (nextTile.connectedTiles[(dir+1)%6])
                    {
                        nextTile = nextTile.connectedTiles[(dir+1)%6];
                        data.TilesHit.Add(nextTile);
                        nextRange--;
                    }
                    break;
                }

                nextTile = nextTile.connectedTiles[dir];
                data.TilesHit.Add(nextTile);

                nextTile.gameObject.GetComponent<Renderer>().materials[0].color = data.tileColor;
                ContinueConeCast(nextTile, dir+2, data, nextRange);
                if (data.isWide)
                    ContinueConeCast(nextTile, dir+4, data, nextRange);


                var target = FindTarget(nextTile, data);
                if (target != null) 
                {
                    AddTarget(target, data);
                }
            }
        }
    }

    void ContinueConeCast(BoardTile tile, int direction, SO_ConeSkill data, int range)
    {
        BoardTile nextTile = tile;
        for (int i = 0; i < range; i++)
        {
            var dir = direction % 6;

            nextTile.gameObject.GetComponent<Renderer>().materials[0].color = data.tileColor;
            nextTile = nextTile.connectedTiles[dir];

            if (nextTile == null)
                break;

            data.TilesHit.Add(nextTile);

            var target = FindTarget(nextTile, data);
            if (target != null) 
            {
                AddTarget(target, data);
            }
        }
    }


    Unit FindTarget(BoardTile tile, SO_Skillpart data)
    {
        if (UnitData.CurrentActiveUnit == null)
            return null;

        foreach (var target in UnitData.Units)
        {
            if (IsCorrectTarget(target, data))
                return target;
        }
        return null;
    }

    bool IsCorrectTarget(Unit target, SO_Skillpart data)
    {
        if (data == null)
            return false;

        var friendly = UnitData.CurrentActiveUnit.Friendly;

        if (SkillshotData.CurrentMainSkillshot.TargetKind == SO_MainSkill.TargetKindEnum.Allies)
        {
            if (target.Friendly == friendly)
                return true;
        }

        if (SkillshotData.CurrentMainSkillshot.TargetKind == SO_MainSkill.TargetKindEnum.All)
            return true;

        return (target.Friendly != friendly);
    }

    void AddTarget(Unit target, SO_Skillpart data)
    {
        if (data.TargetsHit.Contains(target))
            return;

        data.TargetsHit.Add(target);
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

        //movementLR.positionCount++;
        //movementLR.SetPosition(Path.Count + 1, UnitData.CurrentActiveUnit.currentTile.position);
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
}
