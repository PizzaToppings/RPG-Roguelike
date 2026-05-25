using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Composes multiple SO_SkillAugment assets into a single augment.
/// Each sub-augment gets its own runtime SkillAugment instance, so chargeCount
/// and hasTriggered are tracked independently per sub-augment.
/// </summary>
[CreateAssetMenu(fileName = "MultiAugment", menuName = "ScriptableObjects/SkillAugments/MultiAugment")]
public class MultiAugment : SO_SkillAugment
{
    [Tooltip("All augments that will be initialised when this augment is applied.")]
    public List<SO_SkillAugment> Augments = new List<SO_SkillAugment>();

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        foreach (var subSO in Augments)
        {
            if (subSO == null) continue;

            var subAugment = new SkillAugment();
            subAugment.Init(subSO, skill, character);
        }
    }
}
