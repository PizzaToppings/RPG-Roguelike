using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all traits available during a run, used for shop stock and treasure room rewards.
/// Create via: right-click in Project window > ScriptableObjects > GameLoop > TraitPool
/// </summary>
[CreateAssetMenu(fileName = "TraitPool", menuName = "ScriptableObjects/GameLoop/TraitPool")]
public class SO_TraitPool : ScriptableObject
{
    [Tooltip("All traits that can appear as rewards or shop stock. Add every SO_Trait asset here.")]
    public List<SO_Trait> Traits = new List<SO_Trait>();
}
