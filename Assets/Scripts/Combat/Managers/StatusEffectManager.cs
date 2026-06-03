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

    public void ApplyStatusEffect(SO_StatusEffect statusEffectSO, List<Unit> targets, int powerOverride = 0)
	{
        foreach (var target in targets)
		{
		    switch (statusEffectSO.StatusEffectType)
		    {
                case StatusEffectEnum.Bleed:
                    ApplyBleedEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Poison:
                    ApplyPoisonEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Burn:
                    ApplyBurnEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Fatique:
                    ApplyFatiqueEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Thorns:
                    ApplyThornsEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.StatChange:
                    ApplyStatChangeEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Rooted:
                    ApplyRootedEffect(statusEffectSO, target);
                    break;
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

            return; // added by AI
        }

        var statusEffect = new DefaultStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO),
            Caster = UnitData.ActiveUnit,
            Target = target
        };

        statusEffect.Apply();
    }

    public void ApplyBleedEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
	{
        var bleedStatusEffect = new BleedStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power
        };

        bleedStatusEffect.Apply();
	}

    public void ApplyPoisonEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var poisonStatusEffect = new PoisonStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power
        };

        poisonStatusEffect.Apply();
    }

    public void ApplyBurnEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var burnStatusEffect = new BurnStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power
        };

        burnStatusEffect.Apply();
    }

    public void ApplyThornsEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var thornsStatusEffect = new ThornsStatusEffect
        {
            IsBuff = true,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power
        };

        thornsStatusEffect.Apply();
    }

    public void ApplyFatiqueEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var fatiqueStatusEffect = new FatiqueStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power
        };

        fatiqueStatusEffect.Apply();
    }

    public void ApplyStatChangeEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var statChangeEffect = new StatChangeEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power,
            Stat = statusEffectSO.Stat
        };

        statChangeEffect.Apply();
    }

    public void ApplyRootedEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        if (UnitHasStatusEffect(target, statusEffectSO.StatusEffectType))
        {
            var existingStatusEffect = GetUnitStatusEffect(target, statusEffectSO.StatusEffectType);

            if (existingStatusEffect.Duration < statusEffectSO.Duration)
                existingStatusEffect.Duration = statusEffectSO.Duration;

            return;
        }

        var rootedStatusEffect = new RootedStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO),
            Caster = UnitData.ActiveUnit,
            Target = target
        };

        rootedStatusEffect.Apply();
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
        cleanses.ForEach(x => x.EndEffect());
    }

    public void CleanseType(Unit target, StatusEffectEnum type)
    {
        var cleanses = target.statusEffects.FindAll(x => x.statusEfectType == type && !x.IsBuff);
        cleanses.ForEach(x => x.EndEffect());
    }
}
