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

    public static string GetDefault(StatusEffectEnum type, StatsEnum stat = StatsEnum.Power, int power = 0)
    {
        switch (type)
        {
            case StatusEffectEnum.Bleed:
                return power > 0
                    ? $"Deals {power} damage at the end of each turn."
                    : "Deals damage at the end of each turn.";
            case StatusEffectEnum.Poison:
                return power > 0
                    ? $"Deals {power} damage at the end of each turn."
                    : "Deals damage at the end of each turn.";
            case StatusEffectEnum.Burn:
                return power > 0
                    ? $"Deals {power} damage at the end of each turn, spreading to nearby units."
                    : "Deals damage at the end of each turn, spreading to nearby units.";
            case StatusEffectEnum.Regen:
                return power > 0
                    ? $"Heals the unit for {power} health at the end of each turn."
                    : "Heals the unit at the end of each turn.";
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
            case StatusEffectEnum.Blinded:        return "Cannot use Physical skills.";
            case StatusEffectEnum.Silenced:       return "Cannot use Magical skills.";
            case StatusEffectEnum.Frightened:     return "The unit is less likely to target the caster with skills.";
            case StatusEffectEnum.Incapacitated:  return "The unit cannot take any actions. Any damage done breaks the effect.";
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
            string statName = GetStatDisplayName(sce.Stat);
            string sign     = sce.Power >= 0 ? "+" : "";
            return $"{statName} {sign}{sce.Power}";
        }
        return effect.statusEfectType.ToString();
    }

    public static string GetStatDisplayName(StatsEnum stat)
    {
        switch (stat)
        {
            case StatsEnum.Power:        return "Power";
            case StatsEnum.Armor:        return "Armor";
            case StatsEnum.Shield:       return "Shield";
            case StatsEnum.Range:        return "Range";
            case StatsEnum.MaxHitpoints: return "Max Hitpoints";
            //case StatsEnum.MaxEnergy:  return "Max Energy";
            case StatsEnum.MoveSpeed:    return "Move Speed";
            case StatsEnum.Cooldown:     return "Cooldown";
            default:                     return stat.ToString();
        }
    }
}
