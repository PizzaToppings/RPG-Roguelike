using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all possible encounters. The RunManager picks one at random after each skill selection.
/// Create via: right-click in Project window > ScriptableObjects > GameLoop > EncounterPool
/// </summary>
[CreateAssetMenu(fileName = "EncounterPool", menuName = "ScriptableObjects/GameLoop/EncounterPool")]
public class SO_EncounterPool : ScriptableObject
{
    [Tooltip("All encounters that can be randomly selected for a combat. Add SO_Encounter assets here.")]
    public List<SO_Encounter> Encounters = new List<SO_Encounter>();
}
