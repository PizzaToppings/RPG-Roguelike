using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void ApplyStatusEffect(SO_StatusEffect statusEffectSO, List<Unit> targets)
	{
        foreach (var target in targets)
		{
		    switch (statusEffectSO.StatusEfectType)
		    {
                case StatusEfectEnum.Bleed:
                    ApplyBleedEffect(statusEffectSO, target);
                    return;
                case StatusEfectEnum.Poison:
                    ApplyPoisonEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Burn:
                    ApplyBurnEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Manaburn:
                    ApplyManaBurnEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Thorns:
                    ApplyThornsEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.StatChange:
                    ApplyStatChangeEffect(statusEffectSO, target);
                    break;
            }

            // all other cases:
            ApplyDefaultEffect(statusEffectSO, target);
        }
	}

    public void ApplyDefaultEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        if (UnitHasStatusEffect(target, statusEffectSO.StatusEfectType))
        {
            var existingStatusEffect = GetUnitStatusEffect(target, statusEffectSO.StatusEfectType);

            if (existingStatusEffect.Duration < statusEffectSO.Duration)
                existingStatusEffect.Duration = statusEffectSO.Duration;
        }

        var statusEffect = new DefaultStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            Caster = UnitData.ActiveUnit,
            Target = target
        };

        statusEffect.Apply();
    }

    public void ApplyBleedEffect(SO_StatusEffect statusEffectSO, Unit target)
	{
        var bleedStatusEffect = new BleedStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            IsMagical = statusEffectSO.IsMagical,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        bleedStatusEffect.Apply();
	}

    public void ApplyPoisonEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        var poisonStatusEffect = new PoisonStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            IsMagical = statusEffectSO.IsMagical,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        poisonStatusEffect.Apply();
    }

    public void ApplyBurnEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        var burnStatusEffect = new BurnStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            IsMagical = statusEffectSO.IsMagical,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        burnStatusEffect.Apply();
    }

    public void ApplyThornsEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        var thornsStatusEffect = new ThornsStatusEffect
        {
            IsBuff = true,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            IsMagical = statusEffectSO.IsMagical,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        thornsStatusEffect.Apply();
    }

    public void ApplyManaBurnEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        var manaBurnStatusEffect = new ManaburnStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        manaBurnStatusEffect.Apply();
    }

    public void ApplyStatChangeEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        switch (statusEffectSO.Stat)
        {
            case StatsEnum.PhysicalPower:
                target.PhysicalPower += statusEffectSO.Power;
                break;
            case StatsEnum.PhysicalDefense:
                target.PhysicalDefense += statusEffectSO.Power;
                break;
            case StatsEnum.MagicalPower:
                target.MagicalPower += statusEffectSO.Power;
                break;
            case StatsEnum.MagicalDefense:
                target.MagicalDefense += statusEffectSO.Power;
                break;
            case StatsEnum.MoveSpeed:
                target.MoveSpeed += statusEffectSO.Power;
                break;
            case StatsEnum.MaxHitpoints:
                target.MaxHitpoints += statusEffectSO.Power;
                break;
            case StatsEnum.MaxEnergy:
                var character = target as Character;
                character.MaxEnergy += statusEffectSO.Power;
                break;
        }
    }

    public bool UnitHasStatusEffect(Unit unit, StatusEfectEnum statusEfect)
    {
        return unit.statusEffects.Find(x => x.statusEfectType == statusEfect) != null;
    }

    public StatusEffect GetUnitStatusEffect(Unit unit, StatusEfectEnum statusEfect)
    {
        return unit.statusEffects.First(x => x.statusEfectType == statusEfect);
    }

    public void CleanseAll(Unit target)
    {
        var cleanses = target.statusEffects.FindAll(x => !x.IsBuff);
        cleanses.ForEach(x =>target.statusEffects.Remove(x));

        ResetCleanseStatusEffects(target);
    }

    public void CleanseType(Unit target, StatusEfectEnum type)
    {
        var cleanses = target.statusEffects.FindAll(x => x.statusEfectType == type && !x.IsBuff);
        cleanses.ForEach(x =>target.statusEffects.Remove(x));

        ResetCleanseStatusEffects(target);
    }

    public void ResetCleanseStatusEffects(Unit unit /**, List<StatusEfectEnum> types**/)
    {
        StartCoroutine(unit.StartTurn());
    }
}
