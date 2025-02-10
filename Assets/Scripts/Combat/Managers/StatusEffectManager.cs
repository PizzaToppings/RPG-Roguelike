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
		    switch (statusEffectSO.StatusEffectType)
		    {
                case StatusEffectEnum.Bleed:
                    ApplyBleedEffect(statusEffectSO, target);
                    return;
                case StatusEffectEnum.Poison:
                    ApplyPoisonEffect(statusEffectSO, target);
                    return;
                case StatusEffectEnum.Burn:
                    ApplyBurnEffect(statusEffectSO, target);
                    return;
                case StatusEffectEnum.Fatique:
                    ApplyFatiqueEffect(statusEffectSO, target);
                    return;
                case StatusEffectEnum.Thorns:
                    ApplyThornsEffect(statusEffectSO, target);
                    return;
                case StatusEffectEnum.StatChange:
                    ApplyStatChangeEffect(statusEffectSO, target);
                    return;
                case StatusEffectEnum.Unique:
                    
                    return;
                default:
                    ApplyDefaultEffect(statusEffectSO, target);
                    return;
            }
        }
	}

    public void ApplyDefaultEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        if (UnitHasStatusEffect(target, statusEffectSO.StatusEffectType))
        {
            var existingStatusEffect = GetUnitStatusEffect(target, statusEffectSO.StatusEffectType);

            if (existingStatusEffect.Duration < statusEffectSO.Duration)
                existingStatusEffect.Duration = statusEffectSO.Duration;
        }

        var statusEffect = new DefaultStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
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
            statusEfectType = statusEffectSO.StatusEffectType,
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
            statusEfectType = statusEffectSO.StatusEffectType,
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
            statusEfectType = statusEffectSO.StatusEffectType,
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
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsMagical = statusEffectSO.IsMagical,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        thornsStatusEffect.Apply();
    }

    public void ApplyFatiqueEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        var fatiqueStatusEffect = new FatiqueStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        fatiqueStatusEffect.Apply();
    }

    public void ApplyStatChangeEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        var statChangeEffect = new StatChangeEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power,
            Stat = statusEffectSO.Stat
        };

        statChangeEffect.Apply();
    }

    public void ApplyUniqueEffect(SO_StatusEffect statusEffectSO, Unit target)
    {

    }

    public bool UnitHasStatusEffect(Unit unit, StatusEffectEnum statusEfect)
    {
        return unit.statusEffects.Find(x => x.statusEfectType == statusEfect) != null;
    }

    public StatusEffect GetUnitStatusEffect(Unit unit, StatusEffectEnum statusEfect)
    {
        return unit.statusEffects.First(x => x.statusEfectType == statusEfect);
    }

    public void CleanseAll(Unit target)
    {
        var cleanses = target.statusEffects.FindAll(x => !x.IsBuff);
        cleanses.ForEach(x =>target.statusEffects.Remove(x));

        ResetCleanseStatusEffects(target);
    }

    public void CleanseType(Unit target, StatusEffectEnum type)
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
