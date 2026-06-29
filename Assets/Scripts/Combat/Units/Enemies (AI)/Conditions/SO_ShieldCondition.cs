using UnityEngine;

/// <summary>
/// True when the target unit's shield amount satisfies the comparison.
/// </summary>
[CreateAssetMenu(fileName = "ShieldCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/ShieldCondition")]
public class SO_ShieldCondition : SO_AICondition
{
    public ConditionTargetEnum Target = ConditionTargetEnum.Self;
    public int ThresholdAmount = 0;
    public AIComparisonEnum Comparison = AIComparisonEnum.Equal;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        var unit = AIConditionHelper.ResolveUnit(ctx, Target);
        if (unit == null) return false;
        return AIConditionHelper.Compare(unit.ShieldPoints, ThresholdAmount, Comparison);
    }
}
