using System;

/// <summary>
/// Static helpers shared by all SO_AICondition implementations.
/// </summary>
public static class AIConditionHelper
{
    /// <summary>Resolves a unit from the context based on the ConditionTargetEnum.</summary>
    public static Unit ResolveUnit(EnemyAIContext ctx, ConditionTargetEnum target)
    {
        switch (target)
        {
            case ConditionTargetEnum.Self:
                return ctx.Enemy;

            case ConditionTargetEnum.AnyAlly:
                foreach (var e in UnitData.Enemies)
                    if (e != ctx.Enemy && e.Hitpoints > 0) return e;
                return null;

            case ConditionTargetEnum.AnyEnemy:
            case ConditionTargetEnum.AnyCharacter:
                foreach (var c in UnitData.Characters)
                    if (c.Hitpoints > 0) return c;
                return null;

            default:
                return null;
        }
    }

    /// <summary>Compares two floats using the given operator. Modulo uses integer cast.</summary>
    public static bool Compare(float a, float b, AIComparisonEnum op)
    {
        switch (op)
        {
            case AIComparisonEnum.Equal:              return Math.Abs(a - b) < 0.001f;
            case AIComparisonEnum.NotEqual:           return Math.Abs(a - b) >= 0.001f;
            case AIComparisonEnum.LessThan:           return a < b;
            case AIComparisonEnum.LessThanOrEqual:    return a <= b;
            case AIComparisonEnum.GreaterThan:        return a > b;
            case AIComparisonEnum.GreaterThanOrEqual: return a >= b;
            case AIComparisonEnum.Modulo:             return b > 0 && ((int)a % (int)b) == 0;
            default:                                  return false;
        }
    }

    /// <summary>Integer overload for convenience.</summary>
    public static bool Compare(int a, int b, AIComparisonEnum op) => Compare((float)a, (float)b, op);
}
