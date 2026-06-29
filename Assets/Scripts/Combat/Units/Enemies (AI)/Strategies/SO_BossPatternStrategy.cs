using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss strategy composed of multiple phase blocks.
/// Each block has a phase condition and an inner strategy.
/// The first block whose condition passes is used; if none pass, falls back to the last block.
/// </summary>
[CreateAssetMenu(fileName = "BossPatternStrategy", menuName = "ScriptableObjects/EnemyAI/Strategies/BossPattern")]
public class SO_BossPatternStrategy : SO_SkillSelectionStrategy
{
    [Serializable]
    public class BossPhaseBlock
    {
        [Tooltip("Condition that must pass for this phase to be active. Leave null for 'always'.")]
        public SO_AICondition PhaseCondition;
        public SO_SkillSelectionStrategy InnerStrategy;
    }

    public List<BossPhaseBlock> PhaseBlocks = new List<BossPhaseBlock>();

    public override SO_EnemySkill SelectNextSkill(EnemyAIContext ctx, List<SO_EnemySkill> eligibleSkills)
    {
        if (PhaseBlocks == null || PhaseBlocks.Count == 0) return null;

        BossPhaseBlock activeBlock = null;
        foreach (var block in PhaseBlocks)
        {
            if (block.InnerStrategy == null) continue;
            if (block.PhaseCondition == null || block.PhaseCondition.Evaluate(ctx))
            {
                activeBlock = block;
                break;
            }
        }

        // Fallback to the last block with a valid strategy
        if (activeBlock == null)
        {
            for (int i = PhaseBlocks.Count - 1; i >= 0; i--)
            {
                if (PhaseBlocks[i].InnerStrategy != null)
                {
                    activeBlock = PhaseBlocks[i];
                    break;
                }
            }
        }

        return activeBlock?.InnerStrategy.SelectNextSkill(ctx, eligibleSkills);
    }
}
