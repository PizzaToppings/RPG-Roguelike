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
    [SerializeField] Color activeColor;

    LineRenderer movementIndicator;

    void Start()
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
                tile.xPosition = x;
                tile.yPosition = y;
            }
        }

        foreach (BoardTile tile in BoardData.BoardTiles)
            tile.SetConnectedTiles();
    }

    public void ClearMovement()
    {
        foreach (var tile in BoardData.BoardTiles)
        {
            tile.movementLeft = -1;
            tile.gameObject.GetComponent<Renderer>().materials[1].color = originalColor;
        }
    }

    public void SetMovementLeft(int movementLeft, BoardTile startingTile)
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
            
            if (tile.movementLeft <= movementLeft)
            {
                tile.gameObject.GetComponent<Renderer>().materials[1].color = activeColor;
                tile.movementLeft = movementLeft;
                usedTiles.Add(tile);
            }
        }

        foreach (var tile in usedTiles)
            SetMovementLeft(movementLeft, tile);

        // StartCoroutine(foo(usedTiles, movementLeft));
    }

    public void ShowMovementLine(BoardTile finaltile, int movementAmount)
    {
        int movementUsed = CombatData.CurrentActiveUnit.MoveSpeed - movementAmount;
        Debug.Log( movementUsed);
        movementIndicator.positionCount = movementUsed + 1;
        movementIndicator.SetPosition(0, finaltile.transform.position + Vector3.up);
        movementIndicator.SetPosition(movementUsed, CombatData.CurrentActiveUnit.currentTile.transform.position + Vector3.up);

        BoardTile currentTile = finaltile;

        for (int i = 0; i < movementUsed; i++)
        {
            foreach (var tile in currentTile.connectedTiles)
            {
                if (tile == null)
                    continue;

                if (tile.movementLeft == currentTile.movementLeft + 1)
                {
                    movementIndicator.SetPosition(i+1, tile.transform.position + Vector3.up);
                    currentTile = tile;
                    continue;
                }
            }
        }
    }

    IEnumerator foo(List<BoardTile> tiles, int moveLeft)
    {
        foreach (var tile in tiles)
        {
            yield return new WaitForSeconds(1);
            SetMovementLeft(moveLeft, tile);
        }
    }
}
