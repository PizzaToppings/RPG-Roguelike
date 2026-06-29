using UnityEngine;

/// <summary>
/// True when the number of turns since the tracked skill was last used satisfies the comparison.
/// Useful for "cast debuff every 3 turns".
/// </summary>
[CreateAssetMenu(fileName = "TurnsSinceSkillCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/TurnsSinceSkillCondition")]
public class SO_TurnsSinceSkillCondition : SO_AICondition
{
    public SO_EnemySkill TrackedSkill;
    public int Threshold = 3;
    public AIComparisonEnum Comparison = AIComparisonEnum.GreaterThanOrEqual;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        int turns = ctx.TurnsSinceUsed(TrackedSkill);
        return AIConditionHelper.Compare(turns, Threshold, Comparison);
    }
}
