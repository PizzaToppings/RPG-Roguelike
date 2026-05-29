using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Component that makes a character draggable during the placement phase.
/// Attach this to character GameObjects to enable drag-and-drop placement.
/// </summary>
[RequireComponent(typeof(Character))]
public class DraggableCharacter : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] float dragAlpha = 0.7f;

    Character character;
    CharacterPlacementManager placementManager;
    BoardManager boardManager;
    TilemapInputHandler tilemapInputHandler;
    
    SpriteRenderer spriteRenderer;
    Vector3 originalPosition;
    Color originalColor;
    bool isDragging = false;
    Vector2Int originalTilePos;
    
    bool initialized = false;

    void Awake()
    {
        character = GetComponent<Character>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void InitializeManagers()
    {
        if (initialized)
            return;
            
        placementManager = CharacterPlacementManager.Instance;
        boardManager = BoardManager.Instance;
        tilemapInputHandler = TilemapInputHandler.Instance;
        
        if (placementManager != null && boardManager != null && tilemapInputHandler != null)
        {
            initialized = true;
        }
    }

    void Update()
    {
        // Lazy initialization - get instances when they're available
        if (!initialized)
            InitializeManagers();
            
        // Only allow dragging during placement phase
        if (!initialized || UnitData.CurrentAction != CurrentActionKind.CharacterPlacement)
            return;

        HandleDragging();
    }

    void HandleDragging()
    {
        // Start dragging on left mouse button down
        if (Input.GetMouseButtonDown(0) && !isDragging && !EventSystem.current.IsPointerOverGameObject())
        {
            if (IsMouseOverCharacter())
            {
                StartDragging();
            }
        }

        // Update position while dragging
        if (isDragging)
        {
            UpdateDragPosition();

            // Release on mouse button up
            if (Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }
    }

    bool IsMouseOverCharacter()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Check if mouse is over this character's sprite
        if (spriteRenderer != null)
        {
            Bounds bounds = spriteRenderer.bounds;
            return bounds.Contains(mouseWorldPos);
        }

        // Fallback: check distance
        float distance = Vector2.Distance(new Vector2(mouseWorldPos.x, mouseWorldPos.y), 
                                          new Vector2(transform.position.x, transform.position.y));
        return distance < 1f; // Adjust this threshold as needed
    }

    void StartDragging()
    {
        isDragging = true;
        originalPosition = transform.position;
        
        if (character.Tile != null)
        {
            originalTilePos = character.Tile.Coordinates;
        }

        // Visual feedback: make semi-transparent and bring forward
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            Color dragColor = originalColor;
            dragColor.a = dragAlpha;
            spriteRenderer.color = dragColor;
        }
    }

    void UpdateDragPosition()
    {
        // Follow mouse position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = originalPosition.z;
        transform.position = mouseWorldPos;
    }

    void StopDragging()
    {
        isDragging = false;

        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Determine the tile the character was dropped on
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        BoardTile targetTile = GetTileAtPosition(mouseWorldPos);

        // Check if it's a valid placement tile
        if (targetTile != null && placementManager.IsTilePlaceable(targetTile.Coordinates))
        {
            // Place the character at this tile
            placementManager.PlaceCharacterAtTile(character, targetTile.Coordinates);
        }
        else
        {
            // Invalid drop location - snap back to original position
            if (character.Tile != null)
            {
                transform.position = character.Tile.position;
            }
            else
            {
                transform.position = originalPosition;
            }
        }
    }

    BoardTile GetTileAtPosition(Vector3 worldPosition)
    {
        if (tilemapInputHandler == null || tilemapInputHandler.VisualTilemap == null)
        {
            Debug.LogWarning("[DraggableCharacter] TilemapInputHandler or its tilemap is not available.");
            return null;
        }

        Vector3Int cellPos = tilemapInputHandler.VisualTilemap.WorldToCell(worldPosition);
        
        // Convert from tilemap cell to board array indices
        int ax = cellPos.x - BoardData.boardOffset.x;
        int ay = cellPos.y - BoardData.boardOffset.y;
        
        return boardManager.GetBoardTile(new Vector2Int(ax, ay));
    }

    void OnDisable()
    {
        // If we're dragging and get disabled, clean up
        if (isDragging)
        {
            isDragging = false;
            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
    }
}
