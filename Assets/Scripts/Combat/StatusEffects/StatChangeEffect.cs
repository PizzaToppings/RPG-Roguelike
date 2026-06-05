using UnityEngine;

public class StatChangeEffect : StatusEffect
{
    public StatsEnum Stat;
    public int Power;
    // Origin style for grouping in info panels
    public CombatStyle SourceCombatStyle = CombatStyle.None;

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
                Target.Power += Power;
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
                Target.Power -= Power;
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