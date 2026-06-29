using UnityEngine;

/// <summary>
/// True when the enemy is in the given phase (determined by HP thresholds).
/// Phase 1 = above 66% HP, Phase 2 = 33–66% HP, Phase 3 = below 33% HP.
/// Configure custom thresholds per-phase by adding multiple conditions in a group.
/// </summary>
[CreateAssetMenu(fileName = "PhaseCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/PhaseCondition")]
public class SO_PhaseCondition : SO_AICondition
{
    [Tooltip("1 = full HP. Each phase begins below this HP percent.")]
    [Range(0f, 1f)]
    public float PhaseStartsBelow = 0.66f;

    [Tooltip("This phase ends above this HP percent (set to 0 for the last phase).")]
    [Range(0f, 1f)]
    public float PhaseEndsAbove = 0f;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        if (ctx.Enemy == null || ctx.Enemy.MaxHitpoints <= 0) return false;
        float hpPercent = (float)ctx.Enemy.Hitpoints / ctx.Enemy.MaxHitpoints;
        return hpPercent < PhaseStartsBelow && hpPercent >= PhaseEndsAbove;
    }
}
