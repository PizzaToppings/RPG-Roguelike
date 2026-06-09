using UnityEngine;

public class StatChangeEffect : StatusEffect
{
    public StatsEnum Stat;
    public int Power;
    // Origin style for grouping in info panels
    public CombatStyle SourceCombatStyle = CombatStyle.None;
    // When true and Stat == Power, apply to the typed power bonus instead of the generic Power value.
    public bool ApplyToPowerType = false;
    // When ApplyToPowerType == true, this selects magical (true) or physical (false) bonus.
    public bool IsMagicalPower = false;

    public override void Apply()
    {
        // Let the base handle registration and duration subscription
        base.Apply();

        // Apply the stat change immediately
        ChangeStat();
    }

    public void ChangeStat()
    {
        switch (Stat)
        {
            case StatsEnum.Power:
                if (ApplyToPowerType)
                {
                    if (IsMagicalPower)
                        Target.MagicalPowerBonus += Power;
                    else
                        Target.PhysicalPowerBonus += Power;
                }
                else
                {
                    Target.Power += Power;
                }
                break;
            case StatsEnum.Armor:
                Target.Armor += Power;
                break;
            case StatsEnum.Shield:
                Target.ShieldPoints += Power;
                break;
            case StatsEnum.Range:
                Target.Range += Power;
                break;
            case StatsEnum.MoveSpeed:
                Target.MoveSpeed += Power;
                break;
            case StatsEnum.MaxHitpoints:
                Target.MaxHitpoints += Power;
                break;
            //case StatsEnum.MaxEnergy:
            //    var character = Target as Character;
            //    character.MaxEnergy += Power;
            //    break;
        }
    }

    public void RemoveStat()
    {
        switch (Stat)
        {
            case StatsEnum.Power:
                if (ApplyToPowerType)
                {
                    if (IsMagicalPower)
                        Target.MagicalPowerBonus -= Power;
                    else
                        Target.PhysicalPowerBonus -= Power;
                }
                else
                {
                    Target.Power -= Power;
                }
                break;
            case StatsEnum.Armor:
                Target.Armor -= Power;
                break;
            case StatsEnum.Shield:
                Target.ShieldPoints -= Power;
                if (Target.ShieldPoints < 0) Target.ShieldPoints = 0;
                break;
            case StatsEnum.Range:
                Target.Range -= Power;
                break;
            case StatsEnum.MoveSpeed:
                Target.MoveSpeed -= Power;
                break;
            case StatsEnum.MaxHitpoints:
                Target.MaxHitpoints -= Power;
                break;
            //case StatsEnum.MaxEnergy:
            //    var character = Target as Character;
            //    character.MaxEnergy -= Power;
            //    break;
        }
    }

    public override void EndEffect()
    {
        base.EndEffect();

        RemoveStat();
        UnsubscribeDurationTrigger();
    }
}