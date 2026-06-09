using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "ScriptableObjects/GameLoop/Encounter")]
public class SO_Encounter : ScriptableObject
{
    public string EncounterName;

    [Tooltip("Whether this is a normal, elite, or boss encounter.")]
    public EncounterTierEnum Tier = EncounterTierEnum.Normal;

    [Header("Map Configuration")]
    [Tooltip("The tilemap grid prefab to use for this encounter's battlefield.")]
    public GameObject MapPrefab;

    [Tooltip("Grid positions (X, Y) where player characters can be placed before combat. Leave empty to use default positions.")]
    public List<Vector2Int> PlayerPlacementTiles = new List<Vector2Int>();

    [Tooltip("Starting positions for the characters.")]
    [SerializeField]
    public Vector2Int[] PartyStartPositions =
    {
        new Vector2Int(0, 7),
        new Vector2Int(0, 8),
        new Vector2Int(0, 9),
        new Vector2Int(0, 10)
    };

    [Header("Enemy Configuration")]
    [Tooltip("Each entry is an enemy prefab paired with its starting board position (X, Y).")]
    public List<EncounterEnemy> Enemies = new List<EncounterEnemy>();

    [Header("Turn Order Configuration")]
    [Tooltip("Define the turn order for enemies. Each number represents the index of an enemy in the Enemies list. Leave empty for default order (enemies act in list order).")]
    public List<int> EnemyTurnOrder = new List<int>();
}

[Serializable]
public class EncounterEnemy
{
    [Tooltip("Enemy data (stats + sprite).")]
    public SO_Enemy EnemySO;

    [Tooltip("Column index (X) on the 20x15 board.")]
    public int StartX = 10;

    [Tooltip("Row index (Y) on the 20x15 board.")]
    public int StartY = 7;

    [HideInInspector]
    public int TurnOrderIndex = 0; // Set dynamically based on encounter configuration
}
