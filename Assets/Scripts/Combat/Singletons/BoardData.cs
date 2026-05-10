using UnityEngine;

public class BoardData
{
    public static BoardTile[,] BoardTiles;

    public static int rowAmount = 20;
    public static int columnAmount = 15;

    // Offset between tilemap cell coordinates and BoardTiles array indices.
    // arrayIndex = cellCoord - boardOffset
    public static Vector2Int boardOffset = Vector2Int.zero;

    public static BoardTile CurrentMouseTile;

    public static void Reset()
    {
        BoardTiles = null;
        boardOffset = Vector2Int.zero;
    }
}
