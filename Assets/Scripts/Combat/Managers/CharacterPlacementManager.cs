using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterPlacementManager : MonoBehaviour
{
    public static CharacterPlacementManager Instance;

    [Header("References")]
    [SerializeField] Transform characterParent;
    [SerializeField] GameObject placementConfirmButton;

    [Header("Visual Settings")]
    [SerializeField] TileColor placementAvailableColor;
    [SerializeField] TileColor placementOccupiedColor;

    BoardManager boardManager;

    [HideInInspector] public bool IsPlacementPhase = false;
    [HideInInspector] public List<Vector2Int> AvailablePlacementTiles = new List<Vector2Int>();
    [HideInInspector] public UnityEvent OnPlacementConfirmed = new UnityEvent();

    List<Character> charactersToPlace = new List<Character>();
    Dictionary<Character, Vector2Int> characterPlacements = new Dictionary<Character, Vector2Int>();

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        boardManager = BoardManager.Instance;
    }

    /// <summary>
    /// Starts the placement phase with the given characters and available tiles.
    /// </summary>
    public void StartPlacementPhase(List<Character> characters, List<Vector2Int> availableTiles)
    {
        IsPlacementPhase = true;
        charactersToPlace = new List<Character>(characters);
        AvailablePlacementTiles = new List<Vector2Int>(availableTiles);
        characterPlacements.Clear();

        // Register characters at their current positions (already placed by UnitManager)
        foreach (var character in charactersToPlace)
        {
            if (character.Tile != null)
            {
                characterPlacements[character] = character.Tile.Coordinates;
            }
        }

        // Highlight available placement tiles
        HighlightPlacementTiles();

        // Show the confirm button
        if (placementConfirmButton != null)
        {
            placementConfirmButton.SetActive(true);
            UpdateConfirmButtonState();
        }
    }

    /// <summary>
    /// Highlights all available placement tiles.
    /// </summary>
    void HighlightPlacementTiles()
    {
        if (placementAvailableColor == null)
            return;

        foreach (var tilePos in AvailablePlacementTiles)
        {
            var tile = boardManager.GetBoardTile(tilePos);
            if (tile != null && tile.currentUnit == null)
            {
                if (IsTilePlaceable(tilePos))
                    tile.SetColor(placementAvailableColor);
            }
        }
    }

    /// <summary>
    /// Places a character at the specified tile position.
    /// </summary>
    public void PlaceCharacterAtTile(Character character, Vector2Int tilePos)
    {
        // Check if this is a valid placement tile
        if (!AvailablePlacementTiles.Contains(tilePos))
        {
            Debug.LogWarning($"[CharacterPlacement] Tile {tilePos} is not a valid placement tile.");
            return;
        }

        var tile = boardManager.GetBoardTile(tilePos);
        if (tile == null)
        {
            Debug.LogError($"[CharacterPlacement] No tile found at position {tilePos}.");
            return;
        }

        // If tile is occupied by another character, swap them
        if (tile.currentUnit != null && tile.currentUnit is Character otherCharacter)
        {
            // Remove the other character from this tile
            if (characterPlacements.ContainsKey(otherCharacter))
            {
                tile.currentUnit = null;
                characterPlacements.Remove(otherCharacter);
                
                // Move the other character to where the current character was
                if (character.Tile != null)
                {
                    otherCharacter.Tile = character.Tile;
                    character.Tile.currentUnit = otherCharacter;
                    otherCharacter.transform.position = character.Tile.position;
                    characterPlacements[otherCharacter] = character.Tile.Coordinates;
                }
            }
        }

        // Clear the character's old tile
        if (character.Tile != null)
        {
            character.Tile.currentUnit = null;
        }

        // Place the character at the new tile
        character.Tile = tile;
        tile.currentUnit = character;
        character.transform.position = tile.position;
        characterPlacements[character] = tilePos;

        // Update tile colors
        ClearPlacementHighlights();
        HighlightPlacementTiles();
        UpdateConfirmButtonState();
    }

    /// <summary>
    /// Updates the colors of placement tiles based on occupancy.
    /// </summary>
    void UpdatePlacementTileColors()
    {
        if (placementAvailableColor == null)
            return;
            
        foreach (var tilePos in AvailablePlacementTiles)
        {
            var tile = boardManager.GetBoardTile(tilePos);
            if (tile != null)
            {
                if (IsTilePlaceable(tilePos))
                    tile.SetColor(placementAvailableColor);
            }
        }
    }

    /// <summary>
    /// Checks if the tile at the given position is a valid placement tile.
    /// </summary>
    public bool IsTilePlaceable(Vector2Int tilePos)
    {
        return IsPlacementPhase && AvailablePlacementTiles.Contains(tilePos);
    }

    /// <summary>
    /// Checks if the tile at the given position is a valid and available placement tile.
    /// </summary>
    public bool IsTilePlaceableAndAvailable(Vector2Int tilePos)
    {
        if (!IsTilePlaceable(tilePos))
            return false;

        var tile = boardManager.GetBoardTile(tilePos);
        return tile != null; // Allow placement even if occupied (for swapping)
    }

    /// <summary>
    /// Confirms the current placement and starts combat.
    /// </summary>
    public void ConfirmPlacement()
    {
        if (!AllCharactersPlaced())
        {
            Debug.LogWarning("[CharacterPlacement] Not all characters have been placed!");
            return;
        }

        // Clear the placement tile highlights
        ClearPlacementHighlights();

        // Hide the confirm button
        if (placementConfirmButton != null)
            placementConfirmButton.SetActive(false);

        IsPlacementPhase = false;

        // Invoke the event to signal that placement is complete
        OnPlacementConfirmed.Invoke();
    }

    /// <summary>
    /// Checks if all characters have been placed on valid tiles.
    /// </summary>
    bool AllCharactersPlaced()
    {
        foreach (var character in charactersToPlace)
        {
            if (!characterPlacements.ContainsKey(character))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Updates the confirm button's interactable state based on placement status.
    /// </summary>
    void UpdateConfirmButtonState()
    {
        if (placementConfirmButton != null)
        {
            var button = placementConfirmButton.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.interactable = AllCharactersPlaced();
            }
        }
    }

    void Update()
    {
        // Allow spacebar to confirm placement (like ending a turn)
        if (IsPlacementPhase && Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            if (AllCharactersPlaced())
            {
                ConfirmPlacement();
            }
            else
            {
                Debug.Log("[CharacterPlacement] Cannot confirm - not all characters are placed!");
            }
        }
    }

    /// <summary>
    /// Clears the highlight colors from all placement tiles.
    /// </summary>
    void ClearPlacementHighlights()
    {
        foreach (var tilePos in AvailablePlacementTiles)
        {
            var tile = boardManager.GetBoardTile(tilePos);
            if (tile != null)
            {
                tile.SetColor(boardManager.originalColor);
            }
        }
    }

    /// <summary>
    /// Gets the currently dragged character (if any).
    /// </summary>
    public Character GetCharacterAtTile(BoardTile tile)
    {
        if (tile == null || tile.currentUnit == null)
            return null;

        return tile.currentUnit as Character;
    }

    /// <summary>
    /// Restores the placement color for a specific tile if it's a placement tile.
    /// Used when clearing overlays like enemy threat ranges during placement phase.
    /// </summary>
    public void RestorePlacementColor(BoardTile tile)
    {
        if (!IsPlacementPhase || tile == null)
            return;

        if (!AvailablePlacementTiles.Contains(tile.Coordinates))
            return;

        // Check if tile is occupied by a character
        if (tile.currentUnit != null && tile.currentUnit is Character)
        {
            if (placementOccupiedColor != null)
                tile.OverrideColor(placementOccupiedColor);
            else if (placementAvailableColor != null)
                tile.OverrideColor(placementAvailableColor);
        }
        else
        {
            if (placementAvailableColor != null)
                tile.OverrideColor(placementAvailableColor);
        }
    }

    /// <summary>
    /// Hides all placement tile colors (sets them to transparent/original).
    /// Used when showing enemy intent to ensure only the intent is visible.
    /// </summary>
    public void HideAllPlacementColors()
    {
        if (!IsPlacementPhase)
            return;

        foreach (var tilePos in AvailablePlacementTiles)
        {
            var tile = boardManager.GetBoardTile(tilePos);
            if (tile != null)
            {
                tile.OverrideColor(boardManager.originalColor);
            }
        }
    }

    /// <summary>
    /// Restores all placement tile colors.
    /// Used when hiding enemy intent to show placement colors again.
    /// </summary>
    public void RestoreAllPlacementColors()
    {
        if (!IsPlacementPhase)
            return;

        HighlightPlacementTiles();
    }
}
