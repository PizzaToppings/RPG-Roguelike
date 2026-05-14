# Pre-Combat Character Placement System - Setup Guide

## Overview
This system allows players to drag and position their characters on designated tiles before combat begins. Players confirm their placement by clicking a button or pressing spacebar. Each encounter can have a different map with specific placement tiles configured.

## Files Created/Modified

### New Files:
1. `CharacterPlacementManager.cs` - Main manager for the placement phase
2. `DraggableCharacter.cs` - Component that enables character dragging
3. `PlacementConfirmButton.cs` - UI button handler for confirming placement

### Modified Files:
1. `SO_Encounter.cs` - Added MapPrefab and PlayerPlacementTiles fields
2. `CombatManager.cs` - Added placement phase support
3. `PartyManager.cs` - Automatically adds DraggableCharacter component when needed

## Unity Scene Setup

### 1. Update CombatManager GameObject

In your combat scene, select the GameObject that has the `CombatManager` component:

1. Add a new field in the Inspector: **Character Placement Manager**
2. Create a new empty GameObject called "CharacterPlacementManager"
3. Add the `CharacterPlacementManager` component to it
4. Drag this GameObject into the CombatManager's Character Placement Manager field

### 2. Configure CharacterPlacementManager

Select the CharacterPlacementManager GameObject and configure:

1. **Character Parent**: Assign the same Transform that UnitManager uses (typically "Character Parent")
2. **Placement Confirm Button**: Create a UI Button (see step 3) and assign it here
3. **Placement Available Color**: Create/assign a TileColor ScriptableObject for available tiles (e.g., green/cyan tint)
4. **Placement Occupied Color**: Create/assign a TileColor ScriptableObject for occupied tiles (e.g., blue tint)

### 3. Create Placement Confirm Button

### 3. Create Placement Confirm Button

In your combat scene UI Canvas:

1. Create a new UI Button (Right-click Canvas > UI > Button)
2. Rename it to "Confirm Placement Button"
3. Add the `PlacementConfirmButton` component to it
4. Style the button as desired (text: "Confirm Placement" or "Start Battle")
5. Position it prominently on screen (e.g., bottom center)
6. The button will be automatically shown/hidden and enabled/disabled by CharacterPlacementManager

**Tip**: Players can also press **Spacebar** to confirm placement, just like ending a turn during combat!

### 4. Add DraggableCharacter Component to Character Prefab

Open your character prefab:

1. **Option A (Automatic)**: The PartyManager will automatically add the DraggableCharacter component when the placement phase is enabled
2. **Option B (Manual)**: Add the `DraggableCharacter` component to your character prefab
   - **Drag Alpha**: Controls transparency while dragging (default 0.7)
   - **Drag Offset**: Z-offset to bring character forward while dragging
   - No tilemap reference needed - automatically uses the one from TilemapInputHandler

## Creating Encounters with Placement

### 1. Create TileColor ScriptableObjects (if needed)

If you don't have TileColor assets for placement:

1. Right-click in Project > Create > ScriptableObjects > TileColor
2. Create two: "PlacementAvailable" (green/cyan tint) and "PlacementOccupied" (blue tint)
3. Configure the colors for each

### 2. Configure Your Encounter ScriptableObject

Open or create an `SO_Encounter` asset:

1. **Map Prefab**: 
   - Assign the tilemap grid prefab for this encounter's battlefield
   - (Note: Map prefabs don't exist yet, but the field is ready for future use)

2. **Player Placement Tiles**:
   - Add positions to the list where players can place characters
   - Format: X and Y coordinates based on your board grid
   - Example: For a 20x15 board, use positions like (0,6), (0,7), (0,8), (0,9) for the left side
   - **Leave empty to allow placement on ALL tiles** (entire board is available)
   - Add specific positions to restrict placement to certain areas

3. **Enemy Configuration**: Configure as normal

### Example Placement Tiles Configuration:
```
Player Placement Tiles (Size: 6)
- Element 0: X: 0, Y: 6
- Element 1: X: 0, Y: 7
- Element 2: X: 0, Y: 8
- Element 3: X: 0, Y: 9
- Element 4: X: 1, Y: 7
- Element 5: X: 1, Y: 8
```

This creates a 2-column area on the left side where players can position their 4 characters.

## How It Works

### Phase Flow:
1. **Combat Scene Loads**: PartyManager spawns characters, EncounterManager spawns enemies
2. **CombatManager.Start()**: Checks if placement should be enabled
3. **Placement Phase** (if enabled):
   - Available tiles are highlighted
   - Characters start at default positions (first tiles in the placement list)
   - Players can click and drag characters to different placement tiles
   - Characters follow the mouse cursor while being dragged
   - Dropping on invalid tiles snaps character back to original position
   - Confirm button enables only when all characters are placed
   - Players can press **Spacebar** or click the button to confirm
4. **Combat Starts**: Player confirms placement, normal combat begins

### Dragging Behavior:
- Click and hold on a character to start dragging
- Character follows mouse cursor and becomes semi-transparent
- Valid placement tiles are highlighted (green/cyan)
- Occupied tiles show different color (blue)
- Drop on a valid tile to place character there
- Drop on a tile with another character to swap positions
- Drop on invalid tile returns character to previous position
- **Press Spacebar** or click the confirm button when ready to start combat

## Placement Tile Configuration

The placement phase activates when there's an active encounter:

- **No tiles configured (empty list)**: Players can place characters on **ANY tile** on the entire board
- **Specific tiles configured**: Players can only place characters on the configured tiles
- **No encounter**: Placement phase is disabled, characters use fixed starting positions (testing mode)

## Testing

### Test in Editor Without Run Loop:
1. Open the combat scene directly
2. Make sure you have placeholder characters in the scene
3. If you want to test placement:
   - Manually add `DraggableCharacter` components to test characters
   - Set up the CharacterPlacementManager as described above
   - You'll need to mock RunData.CurrentEncounter in code for testing

### Test Through Normal Game Flow:
1. Set up your encounters with PlayerPlacementTiles
2. Start the game from the main menu
3. Select your party
4. Start an encounter
5. You should see the placement phase before combat begins

## Troubleshooting

### Characters aren't draggable:
- Check that DraggableCharacter component is on the character prefab/GameObject
- Verify CharacterPlacementManager.IsPlacementPhase is true
- Check that PlayerPlacementTiles is configured in the encounter

### Tiles aren't highlighted:
- Verify TileColor assets are assigned in CharacterPlacementManager
- Check that BoardManager and tilemap are properly set up
- Ensure placement tiles are valid board coordinates

### Confirm button doesn't appear:
- Check that the button is assigned in CharacterPlacementManager
- Verify the button GameObject is active in the scene
- CharacterPlacementManager should show/hide it automatically

### Spacebar doesn't confirm:
- Ensure all characters are placed on valid tiles first
- Check the console for the "Cannot confirm" message
- Verify CharacterPlacementManager is active and IsPlacementPhase is true

### Characters snap to wrong positions:
- Verify the Visual Tilemap is assigned in DraggableCharacter
- Check that BoardData.boardOffset is correctly calculated
- Ensure tile coordinates in SO_Encounter match your board layout

## Future Enhancements

Consider adding:
- Visual preview of which tiles are in attack range from placement positions
- Tooltips showing tile properties (cover, high ground, etc.)
- Save/load favorite formations
- Formation presets (defensive, offensive, balanced)
- Enemy placement preview before confirming
- Camera pan/zoom during placement phase
- Undo/redo for placement changes
