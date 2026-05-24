/// <summary>
/// Resolves the display description for a status effect.
/// Uses the SO's Description field as an override; falls back to built-in defaults.
/// </summary>
public static class StatusEffectDescriptions
{
    /// <param name="powerOverride">Pass the powerOverride value from StatusEffectManager so the shown number matches the applied power.</param>
    public static string Resolve(SO_StatusEffect so, int powerOverride = 0)
    {
        if (!string.IsNullOrEmpty(so.Description))
            return so.Description;

        int power = powerOverride > 0 ? powerOverride : so.Power;
        return GetDefault(so.StatusEffectType, so.Stat, power);
    }

    public static string GetDefault(StatusEffectEnum type, StatsEnum stat = StatsEnum.PhysicalPower, int power = 0)
    {
        switch (type)
        {
            case StatusEffectEnum.Bleed:
                return power > 0
                    ? $"Deals {power} physical damage at the end of each turn."
                    : "Deals physical damage at the end of each turn.";
            case StatusEffectEnum.Poison:
                return power > 0
                    ? $"Deals {power} magical damage at the end of each turn."
                    : "Deals magical damage at the end of each turn.";
            case StatusEffectEnum.Burn:
                return power > 0
                    ? $"Deals {power} fire damage at the end of each turn, spreading to nearby units."
                    : "Deals fire damage at the end of each turn, spreading to nearby units.";
            case StatusEffectEnum.Fatique:
                return power > 0
                    ? $"Reduces the unit's combat effectiveness by {power}."
                    : "Reduces the unit's combat effectiveness.";
            case StatusEffectEnum.Thorns:
                return power > 0
                    ? $"Reflects {power} damage back to attackers."
                    : "Reflects damage back to attackers.";
            case StatusEffectEnum.Lifedrain:
                return power > 0
                    ? $"Drains {power} life from the target at the end of each turn."
                    : "Drains life from the target at the end of each turn.";
            case StatusEffectEnum.StatChange:
                string statName  = GetStatDisplayName(stat);
                string direction = power >= 0 ? "Increases" : "Decreases";
                int    amount    = power >= 0 ? power : -power;
                return amount > 0
                    ? $"{direction} {statName} by {amount}."
                    : $"Modifies {statName}.";
            case StatusEffectEnum.Hidden:         return "The unit is concealed and cannot be targeted.";
            case StatusEffectEnum.Blinded:        return "The unit's vision is impaired, reducing accuracy.";
            case StatusEffectEnum.Silenced:       return "The unit cannot cast spells or use abilities.";
            case StatusEffectEnum.Frightened:     return "Fear reduces the unit's combat effectiveness.";
            case StatusEffectEnum.Incapacitated:  return "The unit cannot take any actions.";
            case StatusEffectEnum.Stunned:        return "The unit loses its next turn.";
            case StatusEffectEnum.Rooted:         return "The unit cannot move.";
            case StatusEffectEnum.Taunt:          return "Forced to target the unit that applied this effect.";
            case StatusEffectEnum.Dodge:          return "Automatically dodges the next incoming attack.";
            default:
                return string.Empty;
        }
    }

    public static string GetDisplayName(StatusEffect effect)
    {
        if (effect is StatChangeEffect sce)
        {
            string statName  = GetStatDisplayName(sce.Stat);
            string direction = sce.Power >= 0 ? "Up" : "Down";
            return $"{statName} {direction}";
        }
        return effect.statusEfectType.ToString();
    }

    public static string GetStatDisplayName(StatsEnum stat)
    {
        switch (stat)
        {
            case StatsEnum.PhysicalPower:   return "Physical Power";
            case StatsEnum.MagicalPower:    return "Magical Power";
            case StatsEnum.PhysicalDefense: return "Physical Defense";
            case StatsEnum.MagicalDefense:  return "Magical Defense";
            case StatsEnum.MaxHitpoints:    return "Max Hitpoints";
            case StatsEnum.MaxEnergy:       return "Max Energy";
            case StatsEnum.MoveSpeed:       return "Move Speed";
            default:                        return stat.ToString();
        }
    }
}
