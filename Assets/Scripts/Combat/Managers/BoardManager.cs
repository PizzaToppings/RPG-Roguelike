using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    const float rowIncrease = 1.8f;
    const float columnIncrease = 1.55f;
    Quaternion rotation = Quaternion.Euler(90, 0, 0); 

    [Space]
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform BoardParent;

    void Start()
    {

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

        Debug.LogWarning(BoardData.BoardTiles);
        Debug.LogWarning(BoardData.BoardTiles.Length);

        foreach (BoardTile tile in BoardData.BoardTiles)
        {
            tile.SetConnectedTiles();
        }

        // StartCoroutine(connect());
    }

    IEnumerator connect()
    {
        foreach (BoardTile tile in BoardData.BoardTiles)
        {
            tile.SetConnectedTiles();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
