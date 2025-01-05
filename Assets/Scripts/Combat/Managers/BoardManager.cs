using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    SkillsManager skillsManager;
    TargetSkillsManager targetSkillsManager;

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
        targetSkillsManager = TargetSkillsManager.Instance;
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
			tile.skillshotsRangeLeft = new List<float>();
            Path = new List<BoardTile>();

            if (tile.hasTileEffect)
                tile.OverrideColor(tile.tileEffectColor);
            else
                tile.OverrideColor(originalColor);
        }
        StopShowingMovement();
    }

    public void VisualClear()
	{
        foreach (var tile in BoardData.BoardTiles)
        {
			tile.skillshotsRangeLeft = new List<float>();

            if (tile.hasTileEffect)
                tile.OverrideColor(tile.tileEffectColor);
            else
                tile.OverrideColor(originalColor);
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
            targetSkillsManager.SetSkillAOE(data.MaxRange, startingTile, data);
            targetSkillsManager.FilterAOESkillTileList(data.SkillPartIndex, data);
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
				{
                    if (tile.hasTileEffect == true)
                        tile.SetColor(tile.tileEffectColor);
                    else
                        tile.SetColor(MovementColor);
				}

                tile.movementLeft = nextMovementLeft;

                tile.PreviousTile = currentTile;

                SetMovementAOE(nextMovementLeft, tile);
            }
        }
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

    public Unit FindTarget(BoardTile tile, SO_Skillpart data)
    {
        if (tile == null)
            return null;

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
