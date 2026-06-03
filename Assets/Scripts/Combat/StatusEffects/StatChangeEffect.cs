using UnityEngine;

public class StatChangeEffect : StatusEffect
{
    public StatsEnum Stat;
    public int Power;

    public override void Apply()
    {
        Target.statusEffects.Add(this);

        // Show a floating status change unless suppressed (e.g. when part of a stance application)
        if (!SuppressFloating)
        {
            string sign  = Power >= 0 ? "+" : "";
            string label = $"{StatusEffectDescriptions.GetStatDisplayName(Stat)} {sign}{Power}";
            healthCanvas.ShowStatusEffect(label, Target, IsBuff);
        }

        ChangeStat();
        SubscribeDurationTrigger();
        //Target.ThisHealthbar.AddStatusEffect(StatusEfectEnum.Thorns);
    }

    public void ChangeStat()
    {
        switch (Stat)
        {
            case StatsEnum.Power:
                Target.Power += Power;
                break;
            case StatsEnum.Defense:
                Target.Defense += Power;
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
            case StatsEnum.Defense:
                Target.Defense -= Power;
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