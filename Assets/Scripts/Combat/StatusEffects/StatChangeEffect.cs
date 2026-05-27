using UnityEngine;

public class StatChangeEffect : StatusEffect
{
    public StatsEnum Stat;
    public int Power;

    public override void Apply()
    {
        Target.statusEffects.Add(this);

        string sign  = Power >= 0 ? "+" : "";
        string label = $"{StatusEffectDescriptions.GetStatDisplayName(Stat)} {sign}{Power}";
        healthCanvas.ShowStatusEffect(label, Target, IsBuff);

        ChangeStat();
        SubscribeDurationTrigger();
        //Target.ThisHealthbar.AddStatusEffect(StatusEfectEnum.Thorns);
    }

    public void ChangeStat()
    {
        switch (Stat)
        {
            case StatsEnum.PhysicalPower:
                Target.PhysicalPower += Power;
                break;
            case StatsEnum.PhysicalDefense:
                Target.PhysicalDefense += Power;
                break;
            case StatsEnum.MagicalPower:
                Target.MagicalPower += Power;
                break;
            case StatsEnum.MagicalDefense:
                Target.MagicalDefense += Power;
                break;
            case StatsEnum.MoveSpeed:
                Target.MoveSpeed += Power;
                break;
            case StatsEnum.MaxHitpoints:
                Target.MaxHitpoints += Power;
                break;
            case StatsEnum.MaxEnergy:
                var character = Target as Character;
                Debug.Log($"Increasing MaxEnergy of {character.name} by {Power}");
                character.MaxEnergy += Power;
                break;
        }
    }

    public void RemoveStat()
    {
        switch (Stat)
        {
            case StatsEnum.PhysicalPower:
                Target.PhysicalPower -= Power;
                break;
            case StatsEnum.PhysicalDefense:
                Target.PhysicalDefense -= Power;
                break;
            case StatsEnum.MagicalPower:
                Target.MagicalPower -= Power;
                break;
            case StatsEnum.MagicalDefense:
                Target.MagicalDefense -= Power;
                break;
            case StatsEnum.MoveSpeed:
                Target.MoveSpeed -= Power;
                break;
            case StatsEnum.MaxHitpoints:
                Target.MaxHitpoints -= Power;
                break;
            case StatsEnum.MaxEnergy:
                var character = Target as Character;
                character.MaxEnergy -= Power;
                break;
        }
    }

    public override void EndEffect()
    {
        base.EndEffect();

        RemoveStat();
        UnsubscribeDurationTrigger();
    }
}