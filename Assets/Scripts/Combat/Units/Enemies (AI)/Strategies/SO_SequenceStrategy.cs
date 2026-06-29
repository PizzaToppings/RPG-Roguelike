using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Executes skills in a fixed, looping sequence regardless of what is in the eligible list.
/// The sequence overrides condition filtering — if a sequence skill is unavailable (null), it is skipped.
/// Ideal for scripted encounters and boss intros.
/// </summary>
[CreateAssetMenu(fileName = "SequenceStrategy", menuName = "ScriptableObjects/EnemyAI/Strategies/Sequence")]
public class SO_SequenceStrategy : SO_SkillSelectionStrategy
{
    [Tooltip("Skills are used in this order, looping when the end is reached.")]
    public List<SO_EnemySkill> Sequence = new List<SO_EnemySkill>();

    public override SO_EnemySkill SelectNextSkill(EnemyAIContext ctx, List<SO_EnemySkill> eligibleSkills)
    {
        if (Sequence == null || Sequence.Count == 0) return null;

        // Find the next non-null entry, advancing SequenceIndex
        for (int attempt = 0; attempt < Sequence.Count; attempt++)
        {
            int index = ctx.SequenceIndex % Sequence.Count;
            ctx.SequenceIndex = (index + 1) % Sequence.Count;
            var skill = Sequence[index];
            if (skill != null) return skill;
        }

        return null;
    }
}
