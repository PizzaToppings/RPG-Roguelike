using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "ScriptableObjects/GameLoop/Encounter")]
public class SO_Encounter : ScriptableObject
{
    public string EncounterName;

    [Tooltip("Whether this is a normal, elite, or boss encounter.")]
    public EncounterTierEnum Tier = EncounterTierEnum.Normal;

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
