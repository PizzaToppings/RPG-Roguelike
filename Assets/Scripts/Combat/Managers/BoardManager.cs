using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager boardManager; 

    const float rowIncrease = 1.8f;
    const float columnIncrease = 1.55f;
    Quaternion rotation = Quaternion.Euler(90, 0, 0); 

    [Space]
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform BoardParent;

    // temp
    [SerializeField] Color originalColor;

    LineRenderer movementIndicator;
    public List<BoardTile> Path = new List<BoardTile>();
    

    public void Init()
    {
        boardManager = this;
        movementIndicator = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
    }

    public void CreateBoard() 
    {
        BoardData.BoardTiles = new BoardTile[BoardData.rowAmount, BoardData.columnAmount];

        for (int x = 0; x < BoardData.rowAmount; x++)
        {
            for (int y = 0; y < BoardData.columnAmount; y++)
            {
                float secondRowOffset = 0;
                if (y % 2 == 0)
                    secondRowOffset = rowIncrease/2; 

                Vector3 position = new Vector3(
                    x * rowIncrease + secondRowOffset,
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

    public void Clear()
    {
        foreach (var tile in BoardData.BoardTiles)
        {
            tile.movementLeft = -1;
            tile.gameObject.GetComponent<Renderer>().materials[1].color = originalColor;
        }
        StopShowingMovement();
    }

    public void SetMovementLeft(int movementLeft, List<BoardTile> startingTiles,  Color color)
    {
        foreach (var tile in startingTiles)
        {
            SetMovementLeft(movementLeft, tile, color);
        }
    }

    public void SetMovementLeft(int movementLeft, BoardTile startingTile, Color color)
    {
        if (movementLeft == 0)
            return;

        if (startingTile.movementLeft == -1)
            startingTile.movementLeft = 0;

        List<BoardTile> usedTiles = new List<BoardTile>();
        movementLeft--;
        foreach(var tile in startingTile.connectedTiles)
        {
            if (tile == null)
                continue;
            
            if (tile.movementLeft < movementLeft)
            {
                tile.gameObject.GetComponent<Renderer>().materials[1].color = color;
                tile.movementLeft = movementLeft;
                usedTiles.Add(tile);
                SetMovementLeft(movementLeft, tile, color);
            }
        }
    }

    public void PreviewLineCast(int[] directions, SO_Skillshot data)
    {
        BoardTile nextTile = data.OriginTiles[0];

        foreach (var originTile in data.OriginTiles)
        {
            var target = FindTarget(nextTile);

            foreach (var dir in directions)
            {
                nextTile = originTile;
                for (int i = 0; i < data.Range; i++)
                {
                    var direction = dir % 6;

                    nextTile.gameObject.GetComponent<Renderer>().materials[1].color = data.tileColor;
                    nextTile = nextTile.connectedTiles[direction];

                    if (nextTile == null)
                        break;

                    target = FindTarget(nextTile);
                    if (target != null)
                        data.TargetsHit.Add(target);
                }
            }
        }
    }

    Unit FindTarget(BoardTile tile)
    {
        foreach (var target in UnitData.Enemies)
        {
            if (target.currentTile == tile)
            {
                return target;
            }
        }
        return null;
    }

    public void PreviewMovementLine(BoardTile finaltile, int movementAmount)
    {
        int movementUsed = CombatData.CurrentActiveUnit.MoveSpeedLeft - movementAmount;
        movementIndicator.positionCount = movementUsed + 1;
        movementIndicator.SetPosition(0, finaltile.position + Vector3.up);
        movementIndicator.SetPosition(movementUsed, CombatData.CurrentActiveUnit.currentTile.position + Vector3.up);

        BoardTile currentTile = finaltile;
        Path.Add(currentTile);

        for (int i = 0; i < movementUsed; i++)
        {
            foreach (var tile in currentTile.connectedTiles)
            {
                if (tile == null)
                    continue;

                if (tile.movementLeft == currentTile.movementLeft + 1)
                {
                    movementIndicator.SetPosition(i+1, tile.position + Vector3.up);
                    currentTile = tile;
                    Path.Add(tile);
                    continue;
                }
            }
        }
    }

    public void StopShowingMovement()
    {
        movementIndicator.positionCount = 0;
    }
}
