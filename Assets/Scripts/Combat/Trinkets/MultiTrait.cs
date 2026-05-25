using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Composes multiple SO_Trait assets into a single trait.
/// Each sub-trait gets its own runtime Trait instance, so chargeCount
/// and hasTriggered are tracked independently per sub-trait.
/// </summary>
[CreateAssetMenu(fileName = "MultiTrait", menuName = "ScriptableObjects/Traits/MultiTrait")]
public class MultiTrait : SO_Trait
{
    [Tooltip("All traits that will be initialised when this trait is applied.")]
    public List<SO_Trait> Traits = new List<SO_Trait>();

    public override void Init(Character character, Trait trait)
    {
        foreach (var subSO in Traits)
        {
            if (subSO == null) continue;

            var subTrait = new Trait();
            subTrait.Init(subSO, character);
        }
    }
}
