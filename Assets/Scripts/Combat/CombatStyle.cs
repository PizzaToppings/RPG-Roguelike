using System.Collections.Generic;
using UnityEngine;

public enum CombatStyle
{
    None,
    Aggression,
    Defense,
    Focus,
    Mobility,
    Control,
    Support
}

public static class CombatStyleUtility
{
    /// <summary>
    /// Applies stance stat buffs/debuffs when the caster ends their turn.
    /// Self-targeting stances (Aggression, Defense, Mobility, Focus) affect the caster.
    /// Aura stances (Control, Support) affect all living enemies or allies respectively.
    /// All effects use Duration = 1 (EndOfTurn trigger) so they expire at the end of the unit's next turn.
    /// </summary>
    public static void ApplyStanceEffects(CombatStyle stance, Unit caster)
    {
        if (stance == CombatStyle.None) return;

        switch (stance)
        {
            case CombatStyle.Aggression:
                // +2 Power, -2 Defense
                ApplyStatChange(caster, caster, StatsEnum.Power,   +2, isBuff: true);
                ApplyStatChange(caster, caster, StatsEnum.Defense, -2, isBuff: false);
                break;

            case CombatStyle.Defense:
                // -2 Power, +4 Shield
                ApplyStatChange(caster, caster, StatsEnum.Power,  -2, isBuff: false);
                ApplyStatChange(caster, caster, StatsEnum.Shield, +4, isBuff: true);
                break;

            case CombatStyle.Mobility:
                // +2 Speed, -2 Defense
                ApplyStatChange(caster, caster, StatsEnum.MoveSpeed, +2, isBuff: true);
                ApplyStatChange(caster, caster, StatsEnum.Defense,   -2, isBuff: false);
                break;

            case CombatStyle.Focus:
                // +2 Range, -1 Speed
                ApplyStatChange(caster, caster, StatsEnum.Range,    +2, isBuff: true);
                ApplyStatChange(caster, caster, StatsEnum.MoveSpeed, -1, isBuff: false);
                break;

            case CombatStyle.Control:
                // All living enemies get -1 Power and -1 Speed
                var enemies = caster.Friendly ? (System.Collections.IEnumerable)UnitData.Enemies : UnitData.Characters;
                foreach (Unit enemy in enemies)
                {
                    ApplyStatChange(caster, enemy, StatsEnum.Power,    -1, isBuff: false);
                    ApplyStatChange(caster, enemy, StatsEnum.MoveSpeed, -1, isBuff: false);
                }
                break;

            case CombatStyle.Support:
                // All living allies get +1 Defense; caster gets -1 Power
                var allies = caster.Friendly ? (System.Collections.IEnumerable)UnitData.Characters : UnitData.Enemies;
                foreach (Unit ally in allies)
                    ApplyStatChange(caster, ally, StatsEnum.Defense, +1, isBuff: true);
                ApplyStatChange(caster, caster, StatsEnum.Power, -1, isBuff: false);
                break;
        }
    }

    /// <summary>
    /// Creates and applies a StatChangeEffect with Duration = 1 (expires at end of target's next turn).
    /// </summary>
    private static void ApplyStatChange(Unit caster, Unit target, StatsEnum stat, int power, bool isBuff)
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
            CombatStyle.Aggression => "+2 Power, -2 Defense",
            CombatStyle.Defense    => "-2 Power, +4 Shield",
            CombatStyle.Mobility   => "+2 Speed, -2 Defense",
            CombatStyle.Focus      => "+2 Range, -1 Speed",
            CombatStyle.Control    => "Enemies: -1 Power, -1 Speed",
            CombatStyle.Support    => "Allies: +1 Defense. Self: -1 Power",
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
            case CombatStyle.Defense:    return new Color(0.2f, 0.5f, 1f);  // Blue
            case CombatStyle.Focus:      return new Color(1f, 1f, 0.2f);    // Yellow
            case CombatStyle.Mobility:   return new Color(0.2f, 1f, 0.2f);  // Green
            case CombatStyle.Control:    return new Color(0.8f, 0.2f, 1f);  // Purple
            case CombatStyle.Support:    return new Color(0.2f, 1f, 0.8f);  // Cyan
            default:                     return Color.white;
        }
    }
}
