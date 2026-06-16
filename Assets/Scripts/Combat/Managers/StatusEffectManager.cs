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
                case StatusEffectEnum.Regen:
                    ApplyRegenEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Thorns:
                    ApplyThornsEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Exposed:
                case StatusEffectEnum.Guarded:
                    ApplyExposeGuardEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Empowered:
                case StatusEffectEnum.Weakened:
                    ApplyEmpowerWeakenEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.StatChange:
                    ApplyStatChangeEffect(statusEffectSO, target, powerOverride);
                    break;
                case StatusEffectEnum.Rooted:
                    ApplyRootedEffect(statusEffectSO, target);
                    break;
                case StatusEffectEnum.Taunt:
                    ApplyTauntEffect(statusEffectSO, target);
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
            Target = target,
            UseCasterTurnForDuration = statusEffectSO.DurationOwner == DurationOwnerEnum.Caster,
            DurationOwner = statusEffectSO.DurationOwner
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
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power,
            UseCasterTurnForDuration = statusEffectSO.DurationOwner == DurationOwnerEnum.Caster,
            DurationOwner = statusEffectSO.DurationOwner
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
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power,
            UseCasterTurnForDuration = statusEffectSO.DurationOwner == DurationOwnerEnum.Caster,
            DurationOwner = statusEffectSO.DurationOwner
        };

        burnStatusEffect.Apply();
    }

    public void ApplyRegenEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var regenStatusEffect = new RegenStatusEffect
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

        regenStatusEffect.Apply();
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
            Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power,
            UseCasterTurnForDuration = statusEffectSO.DurationOwner == DurationOwnerEnum.Caster,
            DurationOwner = statusEffectSO.DurationOwner
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
            Stat = statusEffectSO.Stat,
            UseCasterTurnForDuration = statusEffectSO.DurationOwner == DurationOwnerEnum.Caster,
            DurationOwner = statusEffectSO.DurationOwner
        };

        // Pass cooldown target preference from the SO to the runtime effect
        statChangeEffect.CooldownTarget = statusEffectSO.CooldownTarget;

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

    public void ApplyTauntEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        if (UnitHasStatusEffect(target, statusEffectSO.StatusEffectType))
        {
            var existingStatusEffect = GetUnitStatusEffect(target, statusEffectSO.StatusEffectType);

            if (existingStatusEffect.Duration < statusEffectSO.Duration)
                existingStatusEffect.Duration = statusEffectSO.Duration;

            return;
        }

        var tauntStatusEffect = new TauntStatusEffect
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

        tauntStatusEffect.Apply();
    }

    public void ApplyUniqueEffect(SO_StatusEffect statusEffectSO, Unit target)
    {

    }

    public void ApplyExposeGuardEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var eg = new ExposeGuardStatusEffect
        {
            IsBuff = false,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            UseCasterTurnForDuration = statusEffectSO.DurationOwner == DurationOwnerEnum.Caster,
            DurationOwner = statusEffectSO.DurationOwner,
            // Power stored in the runtime effect instance if present
        };

        // Set Power if available on the SO
        try { (eg as ExposeGuardStatusEffect).Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power; } catch {}

        eg.Apply();
    }

    public void ApplyEmpowerWeakenEffect(SO_StatusEffect statusEffectSO, Unit target, int powerOverride = 0)
    {
        var ew = new EmpowerWeakenStatusEffect
        {
            IsBuff = true,
            statusEfectType = statusEffectSO.StatusEffectType,
            Duration = statusEffectSO.Duration,
            IsPermanent = statusEffectSO.Permanent,
            DurationTrigger = statusEffectSO.DurationTrigger,
            Description = StatusEffectDescriptions.Resolve(statusEffectSO, powerOverride),
            Caster = UnitData.ActiveUnit,
            Target = target,
            UseCasterTurnForDuration = statusEffectSO.DurationOwner == DurationOwnerEnum.Caster,
            DurationOwner = statusEffectSO.DurationOwner,
        };

        try { (ew as EmpowerWeakenStatusEffect).Power = powerOverride > 0 ? powerOverride : statusEffectSO.Power; } catch {}

        ew.Apply();
    }

    /// <summary>
    /// Consume a single Empowered/Weakened stack from the caster and return its flat power.
    /// Prefers Empowered over Weakened if both are present.
    /// </summary>
    public int ConsumeOutgoingDamageBonus(Unit caster)
    {
        if (caster == null) return 0;

        // Prefer Empowered
        var emp = caster.statusEffects.FirstOrDefault(x => x.statusEfectType == StatusEffectEnum.Empowered) as EmpowerWeakenStatusEffect;
        if (emp != null)
        {
            int p = emp.Power;
            emp.EndEffect();
            return p;
        }

        var weak = caster.statusEffects.FirstOrDefault(x => x.statusEfectType == StatusEffectEnum.Weakened) as EmpowerWeakenStatusEffect;
        if (weak != null)
        {
            int p = weak.Power;
            weak.EndEffect();
            return p;
        }

        return 0;
    }

    /// <summary>
    /// Consume a single Exposed/Guarded stack from the target and return its flat modifier.
    /// Prefers Exposed over Guarded if both are present.
    /// </summary>
    public int ConsumeIncomingDamageModifier(Unit target)
    {
        if (target == null) return 0;

        var exp = target.statusEffects.FirstOrDefault(x => x.statusEfectType == StatusEffectEnum.Exposed) as ExposeGuardStatusEffect;
        if (exp != null)
        {
            int p = exp.Power;
            exp.EndEffect();
            return p;
        }

        var grd = target.statusEffects.FirstOrDefault(x => x.statusEfectType == StatusEffectEnum.Guarded) as ExposeGuardStatusEffect;
        if (grd != null)
        {
            int p = grd.Power;
            grd.EndEffect();
            return p;
        }

        return 0;
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
