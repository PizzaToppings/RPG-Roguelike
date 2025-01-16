using UnityEngine;

public class StatChangeEffect : StatusEffect
{
    public StatsEnum Stat;
    public int Power;
    public bool IsPermanent;

    public override void Apply()
    {
        base.Apply();

        ChangeStat();
        Target.OnUnitTurnEndEvent.AddListener(ReduceDuration);
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
    }
}