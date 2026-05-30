using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    TargetSkillsManager targetSkillsManager;

    [Space]
    [SerializeField] Transform BoardParent;

    [Header("2D Highlight Tilemap")]
    [SerializeField] Tilemap highlightTilemap;
    [SerializeField] TileBase highlightTile;

    public TileColor originalColor;
    public TileColor MovementColor;
    public TileColor MouseOverColor;

    [Header("Tile Colors")]
    public TileColorConfig TileColorConfig;

    LineRenderer movementLR;
    // In 2D, tiles lie in the XY plane; use a slight negative Z so the line
    // renders in front of sprites (camera looks along +Z in 2D).
    Vector3 MovementLineOffset = new Vector3(0f, 0f, -0.5f);

    [HideInInspector] public List<BoardTile> Path = new List<BoardTile>();

    private List<BoardTile> _threatRangeTiles = new List<BoardTile>();

    public Vector2Int[] Directions;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        movementLR = GetComponent<LineRenderer>();
        movementLR.positionCount = 0;
        movementLR.sortingOrder = 10;
        Directions = GetDirections();
        targetSkillsManager = TargetSkillsManager.Instance;
    }

    public TileColor GetTileColor(TileColorKind kind) => new TileColor { Kind = kind };

    public void AddBoardTilesToList()
    {
        // First pass: find bounds using the CellPosition stored during generation.
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        var tiles = new System.Collections.Generic.List<BoardTile>();
        foreach (Transform child in BoardParent.transform)
        {
            var tile = child.GetComponent<BoardTile>();
            if (tile == null) continue;
            tiles.Add(tile);

            if (tile.CellPosition.x < minX) minX = tile.CellPosition.x;
            if (tile.CellPosition.y < minY) minY = tile.CellPosition.y;
            if (tile.CellPosition.x > maxX) maxX = tile.CellPosition.x;
            if (tile.CellPosition.y > maxY) maxY = tile.CellPosition.y;
        }

        BoardData.boardOffset  = new Vector2Int(minX, minY);
        BoardData.rowAmount    = maxX - minX + 1;
        BoardData.columnAmount = maxY - minY + 1;
        BoardData.BoardTiles   = new BoardTile[BoardData.rowAmount, BoardData.columnAmount];

        foreach (var tile in tiles)
        {
            int ax = tile.CellPosition.x - minX;
            int ay = tile.CellPosition.y - minY;
            tile.Init();
            tile.xPosition   = ax;
            tile.yPosition   = ay;
            tile.Coordinates = new Vector2Int(ax, ay);
            BoardData.BoardTiles[ax, ay] = tile;
        }
    }

    /// <summary>
    /// Places a transparent highlight tile at every board cell in the overlay
    /// tilemap. Call once after AddBoardTilesToList.
    /// </summary>
    public void InitHighlightTilemap()
    {
        foreach (var tile in BoardData.BoardTiles)
        {
            if (tile == null) continue;
            highlightTilemap.SetTile(tile.CellPosition, highlightTile);
            // Unlock color so SetColor works at runtime
            highlightTilemap.SetTileFlags(tile.CellPosition, TileFlags.None);
            highlightTilemap.SetColor(tile.CellPosition, Color.clear);
        }
    }

    /// <summary>Sets the color of a cell in the overlay highlight tilemap.</summary>
    public void SetHighlightColor(Vector3Int cellPos, Color color)
    {
        highlightTilemap.SetColor(cellPos, color);
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

    public float GetRangeBetweenTiles(BoardTile startTile, BoardTile EndTile)
    {
        return GetRangeBetweenTiles(startTile.Coordinates, EndTile.Coordinates);
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
        int maxIterations = BoardData.BoardTiles.GetLength(0) + BoardData.BoardTiles.GetLength(1);

        while (startTile != endTile && path.Count <= maxIterations)
		{
            var d = GetDirectionBetweenTiles(startTile, endTile);
            var direction = Directions[d];

            var newCoordinates = startTile.Coordinates + direction;
            startTile = GetBoardTile(newCoordinates);

            if (startTile == null)
                break;

            path.Add(startTile);
        }

        return path;
	}

	int GetDirectionBetweenTiles(BoardTile startTile, BoardTile endTile)
	{
		int bestIndex = 0;
		Vector3 dir = (endTile.position - startTile.position).normalized;
		float bestDot = float.MinValue;

		for (int i = 0; i < startTile.connectedTiles.Length; i++)
		{
			var neighbor = startTile.connectedTiles[i];
			if (neighbor == null) continue;

			Vector3 neighborDir = (neighbor.position - startTile.position).normalized;
			float dot = Vector3.Dot(dir, neighborDir);
			if (dot > bestDot)
			{
				bestDot = dot;
				bestIndex = i;
			}
		}

		return bestIndex;
	}

    public List<BoardTile> GetTilesWithinDirectRange(BoardTile starttile, float range, bool FreeSpacesOnly)
    {
        var tileList = new List<BoardTile>();
        foreach (var tile in BoardData.BoardTiles)
        {
            if (tile == null)
                continue;

            if (starttile == tile)
                continue;

            if (GetRangeBetweenTiles(starttile.Coordinates, tile.Coordinates) <= range)
			{
                if (!FreeSpacesOnly || tile.currentUnit == null || tile.currentUnit == UnitData.ActiveUnit)
                {
                    tileList.Add(tile);
                }
            }
        }

        return tileList;
    }

    public List<BoardTile> GetTilesInDirectAttackRange(BoardTile tile, float attackRange, bool includeInMoveRange)
    {
        var attackTilesInRange = GetTilesWithinDirectRange(tile, attackRange, true);

        if (includeInMoveRange)
		{
            attackTilesInRange = attackTilesInRange.Where(x => x.movementLeft > -1f).ToList();
		}

        if (attackTilesInRange.Count == 0)
            return null;

        var attackerTile = UnitData.ActiveUnit.Tile;
        var tilesOrdened = attackTilesInRange.OrderBy(
            x => GetRangeBetweenTiles(attackerTile.Coordinates, x.Coordinates)).ToList();

        return tilesOrdened;
    }

    public void ShowEnemyThreatRange(Enemy enemy)
    {
        ClearEnemyThreatRange();

        var placementManager = CharacterPlacementManager.Instance;
        if (UnitData.CurrentAction == CurrentActionKind.CharacterPlacement)
        {
            // During placement phase, hide placement colors so only threat is visible
            placementManager.HideAllPlacementColors();
        }
        else
        {
            // During combat, hide movement range so only threat is visible
            if (BoardData.BoardTiles != null)
                foreach (var tile in BoardData.BoardTiles)
                    if (tile != null && tile.movementLeft > -1f)
                        tile.OverrideColor(originalColor);
        }

        var aiEnemy = enemy as EnemyBaseAI;
        float attackRange = aiEnemy?.CurrentSkill?.OptimalRange ?? 0f;
        float totalRange = enemy.MoveSpeed + attackRange;

        var tileColor = GetTileColor(TileColorKind.EnemyIntent);
        _threatRangeTiles = GetTilesWithinDirectRange(enemy.Tile, totalRange, false);
        _threatRangeTiles.ForEach(t => t.SetColor(tileColor));
    }

    public void ClearEnemyThreatRange()
    {
        var placementManager = CharacterPlacementManager.Instance;

        foreach (var tile in _threatRangeTiles)
        {
            tile.OverrideColor(originalColor);
            if (tile.hasTileEffect)
                tile.SetColor(tile.tileEffectColor);
        }
        _threatRangeTiles.Clear();

        if (UnitData.CurrentAction == CurrentActionKind.CharacterPlacement)
        {
            placementManager.RestoreAllPlacementColors();
        }
        else
        {
            if (UnitData.CurrentAction == CurrentActionKind.Basic)
            {
                // Restore movement range colors for all movement tiles
                if (BoardData.BoardTiles != null)
                    foreach (var tile in BoardData.BoardTiles)
                    {
                        if (tile == null || tile.movementLeft <= -1f) continue;
                        tile.OverrideColor(MovementColor);
                        if (tile.hasTileEffect)
                            tile.SetColor(tile.tileEffectColor);
                    }
            }
        }
    }

    public void Clear()
    {
        if (BoardData.BoardTiles == null) return;
        foreach (var tile in BoardData.BoardTiles)
        {
            if (tile == null) continue;
            tile.movementLeft = -1;
            tile.PreviousTile = null;
			tile.skillshotsRangeLeft = new List<float>();
            Path = new List<BoardTile>();

            tile.OverrideColor(originalColor);
            
            if (tile.hasTileEffect)
                tile.SetColor(tile.tileEffectColor);
        }
        StopShowingMovementLine();
    }

    public void VisualClear()
	{
        if (BoardData.BoardTiles == null) return;
        foreach (var tile in BoardData.BoardTiles)
        {
            if (tile == null) continue;
            
			tile.skillshotsRangeLeft = new List<float>();

            tile.OverrideColor(originalColor);

            if (tile.hasTileEffect)
                tile.SetColor(tile.tileEffectColor);
        }
        StopShowingMovementLine();
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

    void SetMovementAOE(float movementLeft, BoardTile currentTile)
    {
        if (movementLeft <= 0)
            return;

        // Mark the tile as reachable but don't color it if it has a unit on it
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
            if (tile == null || tile.IsBlocked)
                continue;

            if (tile.currentUnit != null)
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

        if (data.TargetKind == TargetKindEnum.Allies)
            return target.Friendly == friendly;

        if (data.TargetKind == TargetKindEnum.All)
            return true;

        return target.Friendly != friendly;
    }

    public void SetMovementLine(BoardTile finaltile, bool showLine)
    {
        movementLR.positionCount = 0;
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
        if (showLine == false)
            return;

        movementLR.positionCount++;
        movementLR.SetPosition(Path.Count, UnitData.ActiveUnit.Tile.position + MovementLineOffset);
    }

    public void StopShowingMovementLine()
    {
        movementLR.positionCount = 0;
    }

    public void SetPath(BoardTile endTile)
	{
        var currentTile = endTile;

        if (currentTile == null)
            return;

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
        VisualClear();
        Path.Reverse();

        yield return StartCoroutine(UnitData.ActiveUnit.Move(Path));
    }
}
