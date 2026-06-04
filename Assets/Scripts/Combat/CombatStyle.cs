using System.Collections.Generic;
using UnityEngine;

public enum CombatStyle
{
    None,
    Aggression,
    Defensive,
    Focus,
    Mobility,
    Control,
    Support
}

public static class CombatStyleUtility
{
    /// <summary>
    /// Applies stance stat buffs/debuffs when the caster ends their turn.
    /// Self-targeting stances (Aggression, Armor, Mobility, Focus) affect the caster.
    /// Aura stances (Control, Support) affect all living enemies or allies respectively.
    /// All effects use Duration = 1 (EndOfTurn trigger) so they expire at the end of the unit's next turn.
    /// </summary>
    public static void ApplyStanceEffects(CombatStyle stance, Unit caster)
    {
        if (stance == CombatStyle.None) return;

        // When applying stance effects, suppress the per-stat floating texts
        // and instead show a single floating text with the stance name in style color.
        Color styleColor = CombatStyleUtility.GetStyleColor(stance);
        string stanceLabel = $"{stance.ToString()}";

        // Apply effects while suppressing float texts

        switch (stance)
        {
            case CombatStyle.Aggression:
                ApplyStatChange(caster, caster, StatsEnum.Power,   +2, isBuff: true, suppressFloating: true);
                ApplyStatChange(caster, caster, StatsEnum.Armor, -2, isBuff: false, suppressFloating: true);
                break;

            case CombatStyle.Defensive:
                ApplyStatChange(caster, caster, StatsEnum.Power,  -2, isBuff: false, suppressFloating: true);
                ApplyStatChange(caster, caster, StatsEnum.Shield, +4, isBuff: true, suppressFloating: true);
                break;

            case CombatStyle.Mobility:
                ApplyStatChange(caster, caster, StatsEnum.MoveSpeed, +2, isBuff: true, suppressFloating: true);
                ApplyStatChange(caster, caster, StatsEnum.Armor,   -2, isBuff: false, suppressFloating: true);
                break;

            case CombatStyle.Focus:
                ApplyStatChange(caster, caster, StatsEnum.Range,    +2, isBuff: true, suppressFloating: true);
                ApplyStatChange(caster, caster, StatsEnum.MoveSpeed, -1, isBuff: false, suppressFloating: true);
                break;

            case CombatStyle.Control:
                var enemies = caster.Friendly ? (System.Collections.IEnumerable)UnitData.Enemies : UnitData.Characters;
                foreach (Unit enemy in enemies)
                {
                    ApplyStatChange(caster, enemy, StatsEnum.Power,    -1, isBuff: false, suppressFloating: true);
                    ApplyStatChange(caster, enemy, StatsEnum.MoveSpeed, -1, isBuff: false, suppressFloating: true);
                }
                break;

            case CombatStyle.Support:
                var allies = caster.Friendly ? (System.Collections.IEnumerable)UnitData.Characters : UnitData.Enemies;
                foreach (Unit ally in allies)
                    ApplyStatChange(caster, ally, StatsEnum.Armor, +1, isBuff: true, suppressFloating: true);
                ApplyStatChange(caster, caster, StatsEnum.Power, -1, isBuff: false, suppressFloating: true);
                break;
        }

        // Show single stance floating text using the style color
        if (HealthCanvas.Instance != null)
            HealthCanvas.Instance.ShowStance(stanceLabel, caster, styleColor);
    }

    /// <summary>
    /// Creates and applies a StatChangeEffect with Duration = 1 (expires at end of target's next turn).
    /// </summary>
    private static void ApplyStatChange(Unit caster, Unit target, StatsEnum stat, int power, bool isBuff, bool suppressFloating = false)
    {
        string sign  = power >= 0 ? "+" : "";
        string label = $"{StatusEffectDescriptions.GetStatDisplayName(stat)} {sign}{power}";

        var effect = new StatChangeEffect
        {
            IsBuff          = isBuff,
            statusEfectType = StatusEffectEnum.StatChange,
            Duration        = 1,
            IsPermanent     = false,
            DurationTrigger = TriggerMomentEnum.EndOfTurn,
            Description     = label,
            Caster          = caster,
            Target          = target,
            Stat            = stat,
            Power           = power,
            SuppressFloating = suppressFloating,
            HideInInfoPanel  = suppressFloating,
        };

        effect.Apply();
    }

    /// <summary>
    /// Returns a short human-readable description of the stat changes a stance applies.
    /// </summary>
    public static string GetStanceDescription(CombatStyle stance)
    {
        return stance switch
        {
            CombatStyle.Aggression => "+2 Power, -2 Armor",
            CombatStyle.Defensive    => "-2 Power, +4 Shield",
            CombatStyle.Mobility   => "+2 Speed, -2 Armor",
            CombatStyle.Focus      => "+2 Range, -1 Speed",
            CombatStyle.Control    => "Enemies: -1 Power, -1 Speed",
            CombatStyle.Support    => "Allies: +1 Armor. Self: -1 Power",
            _                      => string.Empty,
        };
    }

    /// <summary>
    /// Gets a color associated with a combat style for UI purposes.
    /// </summary>
    public static Color GetStyleColor(CombatStyle style)
    {
        switch (style)
        {
            case CombatStyle.Aggression: return new Color(1f, 0.2f, 0.2f);  // Red
            case CombatStyle.Defensive:    return new Color(0.2f, 0.5f, 1f);  // Blue
            case CombatStyle.Focus:      return new Color(1f, 1f, 0.2f);    // Yellow
            case CombatStyle.Mobility:   return new Color(0.2f, 1f, 0.2f);  // Green
            case CombatStyle.Control:    return new Color(0.8f, 0.2f, 1f);  // Purple
            case CombatStyle.Support:    return new Color(0.2f, 1f, 0.8f);  // Cyan
            default:                     return Color.white;
        }
    }
}
