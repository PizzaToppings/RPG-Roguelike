using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base for all skill selection strategies.
/// Receives only the skills that already passed their condition checks (eligible list).
/// Returns the skill that becomes the enemy's next intent.
/// </summary>
public abstract class SO_SkillSelectionStrategy : ScriptableObject
{
    public abstract SO_EnemySkill SelectNextSkill(EnemyAIContext ctx, List<SO_EnemySkill> eligibleSkills);
}
