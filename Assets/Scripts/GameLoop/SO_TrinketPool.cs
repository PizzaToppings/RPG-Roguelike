using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all trinkets available during a run, used for shop stock and treasure room rewards.
/// Create via: right-click in Project window > ScriptableObjects > GameLoop > TrinketPool
/// </summary>
[CreateAssetMenu(fileName = "TrinketPool", menuName = "ScriptableObjects/GameLoop/TrinketPool")]
public class SO_TrinketPool : ScriptableObject
{
    [Tooltip("All trinkets that can appear as rewards or shop stock. Add every SO_Trinket asset here.")]
    public List<SO_Trinket> Trinkets = new List<SO_Trinket>();
}
