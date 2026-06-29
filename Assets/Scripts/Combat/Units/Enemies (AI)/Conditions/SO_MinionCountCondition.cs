using UnityEngine;

/// <summary>
/// True when the number of living enemy-allied summons satisfies the comparison.
/// Useful for "summon minions when too few minions are alive" patterns.
/// </summary>
[CreateAssetMenu(fileName = "MinionCountCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/MinionCountCondition")]
public class SO_MinionCountCondition : SO_AICondition
{
    public int ThresholdCount = 1;
    public AIComparisonEnum Comparison = AIComparisonEnum.LessThan;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        int count = 0;
        foreach (var enemy in UnitData.Enemies)
            if (enemy != ctx.Enemy && enemy.Summon && enemy.Hitpoints > 0)
                count++;
        return AIConditionHelper.Compare(count, ThresholdCount, Comparison);
    }
}
