using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a single combat encounter: a named set of enemies with their starting board positions.
/// Create via: right-click in Project window > ScriptableObjects > GameLoop > Encounter
/// </summary>
[CreateAssetMenu(fileName = "Encounter", menuName = "ScriptableObjects/GameLoop/Encounter")]
public class SO_Encounter : ScriptableObject
{
    public string EncounterName;

    [Tooltip("Each entry is an enemy prefab paired with its starting board position (X, Y).")]
    public List<EncounterEnemy> Enemies = new List<EncounterEnemy>();
}

[Serializable]
public class EncounterEnemy
{
    [Tooltip("Enemy prefab to spawn. Must have an Enemy component.")]
    public GameObject EnemyPrefab;

    [Tooltip("Column index (X) on the 20x15 board.")]
    public int StartX = 10;

    [Tooltip("Row index (Y) on the 20x15 board.")]
    public int StartY = 7;
}
