using UnityEngine;

/// <summary>
/// True when the current turn number satisfies the comparison.
/// Modulo mode: true when (TurnCount % TurnNumber == 0), e.g. "every 3 turns".
/// </summary>
[CreateAssetMenu(fileName = "TurnCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/TurnCondition")]
public class SO_TurnCondition : SO_AICondition
{
    public int TurnNumber = 1;
    public AIComparisonEnum Comparison = AIComparisonEnum.Equal;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        return AIConditionHelper.Compare(ctx.TurnCount, TurnNumber, Comparison);
    }
}
