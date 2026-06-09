using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Place on a GameObject in the combat scene.
/// On Awake (before CombatManager.Start runs), this:
///   1. Clears any placeholder enemies under the enemy parent transform.
///   2. Instantiates enemy prefabs defined in RunData.CurrentEncounter.
///
/// If RunData.CurrentEncounter is null (e.g. testing directly in the editor),
/// the scene's existing enemy GameObjects are used unchanged.
///
/// Setup in Inspector:
///   - Enemy Parent : the same Transform that UnitManager uses as its enemyParent.
/// </summary>
public class EncounterManager : MonoBehaviour
{
    [SerializeField] Transform  enemyParent;
    [SerializeField] GameObject enemyPrefab;

    [Space]
    [SerializeField] Transform boardParent;
    [SerializeField] Transform visualGridRoot;

    void Awake()
    {
        if (RunData.CurrentEncounter == null)
        {
            Debug.Log("EncounterManager: No current encounter in RunData — using scene placeholders.");
            return;
        }

        SwapMapPrefab();

        if (enemyPrefab == null)
        {
            Debug.LogWarning("EncounterManager: enemyPrefab is not assigned in the Inspector.");
            return;
        }

        ClearExistingEnemies();
        SpawnEncounterEnemies();
    }

    /// <summary>
    /// Swaps encounter map content.
    /// - If the prefab contains BoardTile components, it replaces boardParent children.
    /// - If it is visual-only, it replaces visualGridRoot while leaving board tiles intact.
    /// </summary>
    void SwapMapPrefab()
    {
        var mapPrefab = RunData.CurrentEncounter.MapPrefab;
        if (mapPrefab == null)
            return;

        bool prefabHasBoardTiles = mapPrefab.GetComponentInChildren<BoardTile>(true) != null;

        // Full board swap path for prefabs that actually define BoardTiles.
        if (prefabHasBoardTiles)
        {
            if (boardParent == null)
            {
                Debug.LogWarning("EncounterManager: boardParent is not assigned - cannot swap BoardTile map prefab.");
                return;
            }

            for (int i = boardParent.childCount - 1; i >= 0; i--)
                DestroyImmediate(boardParent.GetChild(i).gameObject);

            Instantiate(mapPrefab, boardParent);
            Debug.Log($"EncounterManager: Swapped board map to '{mapPrefab.name}' for encounter '{RunData.CurrentEncounter.EncounterName}'.");
            return;
        }

        // Visual-only swap path (keeps BoardTile hierarchy and highlight grid untouched).
        if (visualGridRoot == null)
        {
            Debug.LogWarning("EncounterManager: MapPrefab has no BoardTile components and visualGridRoot is not assigned - skipping visual map swap.");
            return;
        }

        var targetTilemap = visualGridRoot.GetComponent<Tilemap>();
        if (targetTilemap == null)
        {
            Debug.LogWarning("EncounterManager: visualGridRoot does not have a Tilemap component - skipping visual map swap.");
            return;
        }

        // Instantiate a temporary prefab instance to read its tilemap data, then copy
        // into the existing visual grid object so existing references remain valid.
        var tempInstance = Instantiate(mapPrefab);
        var sourceTilemap = tempInstance.GetComponentInChildren<Tilemap>(true);
        if (sourceTilemap == null)
        {
            DestroyImmediate(tempInstance);
            Debug.LogWarning("EncounterManager: MapPrefab does not contain a Tilemap - skipping visual map swap.");
            return;
        }

        CopyTilemapData(sourceTilemap, targetTilemap);
        DestroyImmediate(tempInstance);

        Debug.Log($"EncounterManager: Copied visual map from '{mapPrefab.name}' for encounter '{RunData.CurrentEncounter.EncounterName}'.");
    }

    void CopyTilemapData(Tilemap source, Tilemap target)
    {
        target.ClearAllTiles();
        var bounds = source.cellBounds;
        var sourceTiles = source.GetTilesBlock(bounds);
        target.SetTilesBlock(bounds, sourceTiles);
    }

    void ClearExistingEnemies()
    {
        // DestroyImmediate is used here so removals take effect before
        // CombatManager.Start() / UnitManager.Init() iterate the children.
        for (int i = enemyParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(enemyParent.GetChild(i).gameObject);
    }

    void SpawnEncounterEnemies()
    {
        var encounterData = RunData.CurrentEncounter;
        
        // encounterTurnOrder is set to the enemy's index by default.
        // The full encounter-level TurnOrder sequence is interpreted later by CombatManager when building initiative.

        // Spawn each enemy
        for (int i = 0; i < encounterData.Enemies.Count; i++)
        {
            var entry = encounterData.Enemies[i];
            
            if (entry.EnemySO == null)
            {
                Debug.LogWarning($"EncounterManager: An enemy entry in '{encounterData.EncounterName}' has no SO_Enemy assigned.");
                continue;
            }

            if (enemyPrefab == null)
            {
                Debug.LogWarning("EncounterManager: No enemy prefab assigned.");
                continue;
            }

            var instance = Instantiate(enemyPrefab, enemyParent);

            var spriteRenderer = instance.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && entry.EnemySO.Image != null)
                spriteRenderer.sprite = entry.EnemySO.Image;

            var enemy = instance.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.enemySO        = entry.EnemySO;
                enemy.startXPosition = entry.StartX;
                enemy.startYPosition = entry.StartY;
                // Default encounterTurnOrder is the enemy's index. CombatManager will build final initiative from SO_Encounter.TurnOrder.
                enemy.encounterTurnOrder = i;
            }
            else
            {
                Debug.LogWarning("EncounterManager: Enemy prefab does not have an Enemy component.");
            }
        }
    }
}
