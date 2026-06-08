using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

/// <summary>
/// Handles mouse-to-tile detection for the 2D isometric tilemap.
/// previously lived on individual BoardTile MonoBehaviours.
/// Assign the main (visual) Tilemap in the Inspector.
/// </summary>
public class TilemapInputHandler : MonoBehaviour
{
    public static TilemapInputHandler Instance;

    [SerializeField] Tilemap tilemap;

    /// <summary>
    /// Public accessor for the visual tilemap used for coordinate conversion.
    /// </summary>
    public Tilemap VisualTilemap => tilemap;

    BoardManager boardManager;
    CharacterPlacementManager placementManager;
    BoardTile currentHoveredTile;
    Enemy currentHoveredEnemy;
    UnitHighlighter currentHighlightedUnit;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        boardManager = BoardManager.Instance;
        placementManager = CharacterPlacementManager.Instance;
    }

    void Update()
    {
        // On right-mouse-down, clear current hover so the camera can pan freely.
        if (Input.GetMouseButtonDown(1))
        {
            ClearHover();
            return;
        }

        // While right mouse is held, suppress tile interaction.
        if (Input.GetMouseButton(1))
            return;

        if (UnitMouseProxy.MouseOverUnit != null)
            return;

        HandleHover();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            HandleClick();
    }

    void HandleHover()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ClearHover();
            return;
        }

        var tile = FindTile();

        if (tile == currentHoveredTile)
            return;

        // Un-hover the previous tile
        if (currentHoveredTile != null)
            currentHoveredTile.UnTarget();

        currentHoveredTile = tile;

        if (currentHoveredTile != null)
        {
            BoardData.CurrentMouseTile = currentHoveredTile;
            currentHoveredTile.Target();
        }
        else
        {
            BoardData.CurrentMouseTile = null;
        }
    }

    BoardTile FindTile()
    {
        if (tilemap == null || boardManager == null || Camera.main == null)
            return null;

        // Convert screen position → world position → tilemap cell → BoardTile array index
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        // Subtract the board offset to convert from tilemap space to array indices
        int ax = cellPos.x - BoardData.boardOffset.x;
        int ay = cellPos.y - BoardData.boardOffset.y;
        return boardManager.GetBoardTile(new Vector2Int(ax, ay));
    }

    void HandleClick()
    {
        if (currentHoveredTile != null)
            currentHoveredTile.OnClick();
    }

    /// <summary>
    /// Clears the currently hovered tile (e.g. when the camera starts panning).
    /// </summary>
    public void ClearHover()
    {
        if (currentHoveredTile != null)
        {
            currentHoveredTile.UnTarget();
            currentHoveredTile = null;
        }
        
        BoardData.CurrentMouseTile = null;
    }
}
