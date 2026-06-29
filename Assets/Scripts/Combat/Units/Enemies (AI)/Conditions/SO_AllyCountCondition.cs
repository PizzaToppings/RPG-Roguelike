using UnityEngine;

/// <summary>
/// True when the number of living allies (other enemies) satisfies the comparison.
/// </summary>
[CreateAssetMenu(fileName = "AllyCountCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/AllyCountCondition")]
public class SO_AllyCountCondition : SO_AICondition
{
    public int ThresholdCount = 1;
    public AIComparisonEnum Comparison = AIComparisonEnum.LessThanOrEqual;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        int count = 0;
        foreach (var enemy in UnitData.Enemies)
            if (enemy != ctx.Enemy && enemy.Hitpoints > 0)
                count++;
        return AIConditionHelper.Compare(count, ThresholdCount, Comparison);
    }
}
