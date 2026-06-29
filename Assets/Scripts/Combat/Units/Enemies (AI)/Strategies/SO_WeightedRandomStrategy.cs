using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selects a skill randomly, weighted by each skill's Weight field.
/// Higher Weight = more likely to be chosen.
/// </summary>
[CreateAssetMenu(fileName = "WeightedRandomStrategy", menuName = "ScriptableObjects/EnemyAI/Strategies/WeightedRandom")]
public class SO_WeightedRandomStrategy : SO_SkillSelectionStrategy
{
    public override SO_EnemySkill SelectNextSkill(EnemyAIContext ctx, List<SO_EnemySkill> eligibleSkills)
    {
        if (eligibleSkills == null || eligibleSkills.Count == 0) return null;

        int totalWeight = 0;
        foreach (var skill in eligibleSkills)
            totalWeight += Mathf.Max(1, skill.Weight);

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var skill in eligibleSkills)
        {
            cumulative += Mathf.Max(1, skill.Weight);
            if (roll < cumulative)
                return skill;
        }

        return eligibleSkills[eligibleSkills.Count - 1];
    }
}
