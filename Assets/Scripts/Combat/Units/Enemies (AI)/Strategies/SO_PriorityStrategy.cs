using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Picks the first eligible skill in the list (order = priority).
/// Skills earlier in SO_Enemy.Skills have higher priority.
/// </summary>
[CreateAssetMenu(fileName = "PriorityStrategy", menuName = "ScriptableObjects/EnemyAI/Strategies/Priority")]
public class SO_PriorityStrategy : SO_SkillSelectionStrategy
{
    public override SO_EnemySkill SelectNextSkill(EnemyAIContext ctx, List<SO_EnemySkill> eligibleSkills)
    {
        if (eligibleSkills == null || eligibleSkills.Count == 0) return null;
        return eligibleSkills[0];
    }
}
