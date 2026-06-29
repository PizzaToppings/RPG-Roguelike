using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Combines multiple conditions with AND or OR logic.
/// Use this to build composite conditions without extra code.
/// </summary>
[CreateAssetMenu(fileName = "ConditionGroup", menuName = "ScriptableObjects/EnemyAI/Conditions/ConditionGroup")]
public class SO_ConditionGroup : SO_AICondition
{
    public ConditionLogicEnum Logic = ConditionLogicEnum.And;
    public List<SO_AICondition> Conditions = new List<SO_AICondition>();

    public override bool Evaluate(EnemyAIContext ctx)
    {
        if (Conditions == null || Conditions.Count == 0) return true;

        if (Logic == ConditionLogicEnum.And)
        {
            foreach (var c in Conditions)
                if (c != null && !c.Evaluate(ctx)) return false;
            return true;
        }
        else
        {
            foreach (var c in Conditions)
                if (c != null && c.Evaluate(ctx)) return true;
            return false;
        }
    }
}
