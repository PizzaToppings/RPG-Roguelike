using UnityEngine;

/// <summary>
/// True when the distance from this enemy to the nearest character satisfies the comparison.
/// Useful for "only use this ranged ability when far away".
/// </summary>
[CreateAssetMenu(fileName = "DistanceCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/DistanceCondition")]
public class SO_DistanceCondition : SO_AICondition
{
    public float ThresholdDistance = 3f;
    public AIComparisonEnum Comparison = AIComparisonEnum.LessThanOrEqual;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        if (ctx.Enemy == null || ctx.Enemy.Tile == null) return false;
        float minDist = float.MaxValue;
        foreach (var character in UnitData.Characters)
        {
            if (character == null || character.Tile == null) continue;
            float dist = BoardManager.Instance.GetRangeBetweenTiles(ctx.Enemy.Tile, character.Tile);
            if (dist < minDist) minDist = dist;
        }
        if (minDist == float.MaxValue) return false;
        return AIConditionHelper.Compare(minDist, ThresholdDistance, Comparison);
    }
}
