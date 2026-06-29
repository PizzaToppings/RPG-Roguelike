using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cycles through eligible skills in list order, advancing the rotation index each turn.
/// Skips skills that are not currently eligible, but always advances.
/// </summary>
[CreateAssetMenu(fileName = "RotationStrategy", menuName = "ScriptableObjects/EnemyAI/Strategies/Rotation")]
public class SO_RotationStrategy : SO_SkillSelectionStrategy
{
    public override SO_EnemySkill SelectNextSkill(EnemyAIContext ctx, List<SO_EnemySkill> eligibleSkills)
    {
        if (eligibleSkills == null || eligibleSkills.Count == 0) return null;

        ctx.RotationIndex = ctx.RotationIndex % eligibleSkills.Count;
        var selected = eligibleSkills[ctx.RotationIndex];
        ctx.RotationIndex = (ctx.RotationIndex + 1) % eligibleSkills.Count;
        return selected;
    }
}
