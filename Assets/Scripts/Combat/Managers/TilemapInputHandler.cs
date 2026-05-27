using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

/// <summary>
/// Handles mouse-to-tile detection for the 2D isometric tilemap.
/// Replaces the 3D OnMouseDown/OnMouseEnter/OnMouseExit callbacks that
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
        // During placement phase only update unit highlights; skip all other tile interaction.
        if (placementManager != null && placementManager.IsPlacementPhase)
        {
            HandleUnitHighlight();
            return;
        }

        // On right-mouse-down, clear current hover so the camera can pan freely.
        if (Input.GetMouseButtonDown(1))
        {
            ClearHover();
            return;
        }

        // While right mouse is held, suppress tile interaction.
        if (Input.GetMouseButton(1))
            return;

        HandleHover();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            HandleClick();
    }

    /// <summary>
    /// Updates only the unit highlight based on which tile the mouse is over.
    /// Used during the placement phase where full tile interaction is suppressed.
    /// </summary>
    void HandleUnitHighlight()
    {
        if (tilemap == null || EventSystem.current.IsPointerOverGameObject())
        {
            if (currentHighlightedUnit != null)
            {
                currentHighlightedUnit.SetHighlight(false);
                currentHighlightedUnit = null;
            }
            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        int ax = cellPos.x - BoardData.boardOffset.x;
        int ay = cellPos.y - BoardData.boardOffset.y;
        BoardTile tile = boardManager.GetBoardTile(new Vector2Int(ax, ay));

        Unit hoveredUnit = tile != null ? tile.currentUnit : null;
        UnitHighlighter highlighter = hoveredUnit != null ? hoveredUnit.GetComponent<UnitHighlighter>() : null;

        if (highlighter == currentHighlightedUnit) return;

        if (currentHighlightedUnit != null)
        {
            currentHighlightedUnit.SetHighlight(false);
            currentHighlightedUnit = null;
            InitiativeTracker.Instance?.ClearHighlight();
        }

        if (highlighter != null)
        {
            highlighter.SetHighlight(true);
            currentHighlightedUnit = highlighter;
            InitiativeTracker.Instance?.HighlightUnit(hoveredUnit);
        }
    }

    void HandleHover()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ClearHover();
            return;
        }

        if (tilemap == null)
        {
            Debug.LogWarning("[TilemapInputHandler] Tilemap is not assigned in the Inspector!");
            return;
        }

        // Convert screen position → world position → tilemap cell → BoardTile array index
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        // Subtract the board offset to convert from tilemap space to array indices
        int ax = cellPos.x - BoardData.boardOffset.x;
        int ay = cellPos.y - BoardData.boardOffset.y;
        BoardTile tile = boardManager.GetBoardTile(new Vector2Int(ax, ay));

        if (tile == currentHoveredTile)
            return;

        // Un-hover the previous tile
        if (currentHoveredTile != null)
            currentHoveredTile.UnTarget();

        // Clear enemy hover state when moving to a different tile
        if (currentHoveredEnemy != null)
        {
            EnemyInfoPanelManager.Instance?.HidePanel();
            boardManager.ClearEnemyThreatRange();
            currentHoveredEnemy = null;
        }

        // Remove highlight from the previously hovered unit
        if (currentHighlightedUnit != null)
        {
            currentHighlightedUnit.SetHighlight(false);
            currentHighlightedUnit = null;
            InitiativeTracker.Instance?.ClearHighlight();
        }

        currentHoveredTile = tile;

        if (currentHoveredTile != null)
        {
            BoardData.CurrentMouseTile = currentHoveredTile;
            currentHoveredTile.Target();

            // Highlight any unit on the newly hovered tile
            if (currentHoveredTile.currentUnit != null)
            {
                var highlighter = currentHoveredTile.currentUnit.GetComponent<UnitHighlighter>();
                if (highlighter != null)
                {
                    highlighter.SetHighlight(true);
                    currentHighlightedUnit = highlighter;
                }
                InitiativeTracker.Instance?.HighlightUnit(currentHoveredTile.currentUnit);
            }

            // If the tile has an enemy, show threat range and info panel (player turn only, not during skillshot aim)
            if (currentHoveredTile.currentUnit is Enemy enemy)
            {
                bool isPlayerTurn = UnitData.ActiveUnit != null && UnitData.ActiveUnit.Friendly;
                bool playerIsCastingSkillshot = UnitData.CurrentAction == CurrentActionKind.CastingSkillshot;
                if (isPlayerTurn && !playerIsCastingSkillshot)
                {
                    currentHoveredEnemy = enemy;
                    EnemyInfoPanelManager.Instance?.ShowPanel(enemy);
                    boardManager.ShowEnemyThreatRange(enemy);
                }
            }
        }
        else
        {
            BoardData.CurrentMouseTile = null;
        }
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
        if (currentHoveredEnemy != null)
        {
            EnemyInfoPanelManager.Instance?.HidePanel();
            boardManager.ClearEnemyThreatRange();
            currentHoveredEnemy = null;
        }
        if (currentHighlightedUnit != null)
        {
            currentHighlightedUnit.SetHighlight(false);
            currentHighlightedUnit = null;
            InitiativeTracker.Instance?.ClearHighlight();
        }
        BoardData.CurrentMouseTile = null;
    }
}
