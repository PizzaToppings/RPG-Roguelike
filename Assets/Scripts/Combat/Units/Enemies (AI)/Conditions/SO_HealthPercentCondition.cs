using UnityEngine;

/// <summary>
/// True when the target unit's HP percent satisfies the comparison.
/// Target = Self checks the enemy itself; AnyCharacter checks the nearest character.
/// </summary>
[CreateAssetMenu(fileName = "HealthPercentCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/HealthPercentCondition")]
public class SO_HealthPercentCondition : SO_AICondition
{
    [Tooltip("Which unit to evaluate health on.")]
    public ConditionTargetEnum Target = ConditionTargetEnum.Self;

    [Range(0f, 1f)]
    public float ThresholdPercent = 0.5f;

    public AIComparisonEnum Comparison = AIComparisonEnum.LessThanOrEqual;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        var unit = AIConditionHelper.ResolveUnit(ctx, Target);
        if (unit == null) return false;
        float hpPercent = unit.MaxHitpoints > 0 ? (float)unit.Hitpoints / unit.MaxHitpoints : 0f;
        return AIConditionHelper.Compare(hpPercent, ThresholdPercent, Comparison);
    }
}
