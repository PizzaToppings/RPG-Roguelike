using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class BoardGenerator : MonoBehaviour
{
    /// <summary>
    /// The visual isometric Tilemap. BoardTile GameObjects are placed at each
    /// cell's world-space centre so their positions match the rendered tiles.
    /// </summary>
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject tilePrefab;

    public Vector2Int[] Directions;

    private Vector2Int[] GetDirections()
    {
        return new Vector2Int[]
        {
            new Vector2Int(0, 1),   new Vector2Int(1, 1),
            new Vector2Int(1, 0),   new Vector2Int(1, -1),
            new Vector2Int(0, -1),  new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),  new Vector2Int(-1, 1)
        };
    }

    [ContextMenu("Generate Board")]
    public void CreateBoard()
    {
        Directions = GetDirections();

        // Delete previously generated tiles
        var existing = new List<GameObject>();
        foreach (Transform child in transform)
            existing.Add(child.gameObject);
        foreach (var go in existing)
            DestroyImmediate(go);

        // Collect all occupied cells from the Tilemap
        BoundsInt bounds = tilemap.cellBounds;
        var occupiedCells = new List<Vector3Int>();
        foreach (var cellPos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(cellPos))
                occupiedCells.Add(cellPos);
        }

        if (occupiedCells.Count == 0)
        {
            Debug.LogWarning("No tiles found in the Tilemap. Make sure the Tilemap reference is correct.");
            return;
        }

        // Compute offset so array indices start at 0
        int minX = int.MaxValue, minY = int.MaxValue;
        foreach (var c in occupiedCells)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
        }
        int rows    = 0;
        int columns = 0;
        foreach (var c in occupiedCells)
        {
            int ax = c.x - minX, ay = c.y - minY;
            if (ax + 1 > rows)    rows    = ax + 1;
            if (ay + 1 > columns) columns = ay + 1;
        }

        BoardTile[,] tileList = new BoardTile[rows, columns];

        foreach (var cellPos in occupiedCells)
        {
            int ax = cellPos.x - minX;
            int ay = cellPos.y - minY;

            Vector3 position = tilemap.GetCellCenterWorld(cellPos);

            var newTile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            if (newTile == null)
            {
                Debug.LogError($"Failed to instantiate tile prefab at cell {cellPos}!");
                continue;
            }

            Undo.RegisterCreatedObjectUndo(newTile, "Create Tile");

            BoardTile tile = newTile.GetComponent<BoardTile>();
            SetBoardTileValues(tile, ax, ay);

            tileList[ax, ay] = tile;
            EditorUtility.SetDirty(newTile);
        }

        foreach (BoardTile tile in tileList)
        {
            if (tile == null) continue;
            SetConnectedTiles(tile, tileList);
        }

        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        Debug.Log($"Board generation complete: {occupiedCells.Count} tiles, offset ({minX},{minY}). Don't forget to save the scene!");
    }

    public void SetConnectedTiles(BoardTile mainTile, BoardTile[,] tileList)
    {
        for (int i = 0; i < mainTile.connectedTiles.Length; i++)
        {
            var direction = Directions[i];
            mainTile.connectedTiles[i] = GetBoardTile(mainTile.Coordinates + direction, tileList);
        }
        EditorUtility.SetDirty(mainTile.gameObject);
    }

    // Keep ExistingTile overload without array for any external callers
    private bool ExistingTile(int x, int y)
    {
        if (BoardData.BoardTiles == null) return false;
        return x >= 0 && x < BoardData.BoardTiles.GetLength(0)
            && y >= 0 && y < BoardData.BoardTiles.GetLength(1);
    }

    private BoardTile GetBoardTile(Vector2Int position, BoardTile[,] tileList)
    {
        if (!ExistingTile(position.x, position.y, tileList)) return null;
        return tileList[position.x, position.y];
    }

    public void SetBoardTileValues(BoardTile tile, int xPosition, int yPosition)
    {
        tile.xPosition = xPosition;
        tile.yPosition = yPosition;
        tile.Coordinates = new Vector2Int(xPosition, yPosition);

        tile.gameObject.name = $"{xPosition}, {yPosition}";
        EditorUtility.SetDirty(tile.gameObject);
    }

    private bool ExistingTile(int x, int y, BoardTile[,] tileList)
    {
        return x >= 0 && x < tileList.GetLength(0) && y >= 0 && y < tileList.GetLength(1);
    }
}
