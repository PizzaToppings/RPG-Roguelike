using UnityEngine;

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

    void Awake()
    {
        if (RunData.CurrentEncounter == null)
            return;

        ClearExistingEnemies();
        SpawnEncounterEnemies();
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
        foreach (var entry in RunData.CurrentEncounter.Enemies)
        {
            if (entry.EnemySO == null)
            {
                Debug.LogWarning($"EncounterManager: An enemy entry in '{RunData.CurrentEncounter.EncounterName}' has no SO_Enemy assigned.");
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
            }
            else
            {
                Debug.LogWarning("EncounterManager: Enemy prefab does not have an Enemy component.");
            }
        }
    }
}
