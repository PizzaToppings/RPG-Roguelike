using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class BoardGenerator : MonoBehaviour
{
    const float rowIncrease = 1.5f;
    const float columnIncrease = 1.5f;
    Quaternion rotation = Quaternion.Euler(-90, 0, 0);

    [SerializeField] private GameObject tilePrefab;

    public Vector2Int[] Directions;

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Generating Board...");
            CreateBoard();
        }
    }

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

        BoardTile[,] tileList = new BoardTile[BoardData.rowAmount, BoardData.columnAmount];

        for (int x = 0; x < BoardData.rowAmount; x++)
        {
            for (int y = 0; y < BoardData.columnAmount; y++)
            {
                Vector3 position = new Vector3(x * rowIncrease, 0, y * columnIncrease);

                var newTile = PrefabUtility.InstantiatePrefab(tilePrefab, transform) as GameObject;
                if (newTile == null)
                {
                    Debug.LogError("Tile prefab is null or failed to instantiate!");
                    continue;
                }

                Undo.RegisterCreatedObjectUndo(newTile, "Create Tile");

                newTile.transform.position = position;
                newTile.transform.rotation = rotation;

                BoardTile tile = newTile.GetComponent<BoardTile>();
                SetBoardTileValues(tile, x, y);

                tileList[x, y] = tile;

                EditorUtility.SetDirty(newTile);
            }
        }

        foreach (BoardTile tile in tileList)
        {
            SetConnectedTiles(tile, tileList);
        }

        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        Debug.Log("Board generation complete. Don't forget to save the scene!");
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

    private BoardTile GetBoardTile(Vector2Int position, BoardTile[,] tileList)
    {
        if (!ExistingTile(position.x, position.y)) return null;
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

    private bool ExistingTile(int x, int y)
    {
        return x >= 0 && x < BoardData.rowAmount && y >= 0 && y < BoardData.columnAmount;
    }
}
