using UnityEngine;

public enum CombatStyle
{
    None,
    Aggression,
    Defense,
    Precision,
    Mobility,
    Control
}

public static class CombatStyleUtility
{
    /// <summary>
    /// Gets the damage multiplier based on attacker's style vs defender's style.
    /// Returns 1.5 for strong matchup, 0.67 for weak matchup, 1.0 for neutral.
    /// </summary>
    public static float GetStyleMultiplier(CombatStyle attackerStyle, CombatStyle defenderStyle)
    {
        // No style means no modifier
        if (attackerStyle == CombatStyle.None || defenderStyle == CombatStyle.None)
            return 1f;

        // Same style is neutral
        if (attackerStyle == defenderStyle)
            return 1f;

        // Check if attacker has advantage
        if (IsStrongAgainst(attackerStyle, defenderStyle))
            return 1.5f;

        // Check if attacker has disadvantage
        if (IsWeakAgainst(attackerStyle, defenderStyle))
            return 0.67f;

        // Neutral matchup
        return 1f;
    }

    /// <summary>
    /// Returns true if attackerStyle is strong against defenderStyle.
    /// </summary>
    public static bool IsStrongAgainst(CombatStyle attackerStyle, CombatStyle defenderStyle)
    {
        switch (attackerStyle)
        {
            case CombatStyle.Aggression:
                return defenderStyle == CombatStyle.Control;
            case CombatStyle.Defense:
                return defenderStyle == CombatStyle.Precision;
            case CombatStyle.Precision:
                return defenderStyle == CombatStyle.Aggression;
            case CombatStyle.Mobility:
                return defenderStyle == CombatStyle.Defense;
            case CombatStyle.Control:
                return defenderStyle == CombatStyle.Mobility;
            default:
                return false;
        }
    }

    /// <summary>
    /// Returns true if attackerStyle is weak against defenderStyle.
    /// </summary>
    public static bool IsWeakAgainst(CombatStyle attackerStyle, CombatStyle defenderStyle)
    {
        switch (attackerStyle)
        {
            case CombatStyle.Aggression:
                return defenderStyle == CombatStyle.Defense;
            case CombatStyle.Defense:
                return defenderStyle == CombatStyle.Aggression;
            case CombatStyle.Precision:
                return defenderStyle == CombatStyle.Mobility;
            case CombatStyle.Mobility:
                return defenderStyle == CombatStyle.Control;
            case CombatStyle.Control:
                return defenderStyle == CombatStyle.Precision;
            default:
                return false;
        }
    }

    /// <summary>
    /// Gets a color associated with a combat style for UI purposes.
    /// </summary>
    public static Color GetStyleColor(CombatStyle style)
    {
        switch (style)
        {
            case CombatStyle.Aggression:
                return new Color(1f, 0.2f, 0.2f); // Red
            case CombatStyle.Defense:
                return new Color(0.2f, 0.5f, 1f); // Blue
            case CombatStyle.Precision:
                return new Color(1f, 1f, 0.2f); // Yellow
            case CombatStyle.Mobility:
                return new Color(0.2f, 1f, 0.2f); // Green
            case CombatStyle.Control:
                return new Color(0.8f, 0.2f, 1f); // Purple
            default:
                return Color.white;
        }
    }
}
