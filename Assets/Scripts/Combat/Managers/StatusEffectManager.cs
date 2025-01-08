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
        var statusEffect = new StatusEffect();

        foreach (var target in targets)
		{
		    switch (statusEffectSO.StatusEfectType)
		    {
                case StatusEfectEnum.Bleed:
                    ApplyBleedEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Poison:
                    ApplyPoisonEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Burn:
                    ApplyBurnEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Manaburn:
                    ApplyManaBurnEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Blinded:
                case StatusEfectEnum.Silenced:
                case StatusEfectEnum.Stunned:
                case StatusEfectEnum.Incapacitated:
                case StatusEfectEnum.Hidden:
                case StatusEfectEnum.Lifedrain:
                    ApplyDefaultEffect(statusEffectSO, target);
                    break;
            }

            // add stat-increase system for stats
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
            Buff = false,
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
            Buff = false,
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
            Buff = false,
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
            Buff = false,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            IsMagical = statusEffectSO.IsMagical,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        burnStatusEffect.Apply();
    }

    public void ApplyManaBurnEffect(SO_StatusEffect statusEffectSO, Unit target)
    {
        var manaBurnStatusEffect = new ManaburnStatusEffect
        {
            Buff = false,
            statusEfectType = statusEffectSO.StatusEfectType,
            Duration = statusEffectSO.Duration,
            Caster = UnitData.ActiveUnit,
            Target = target,
            Power = statusEffectSO.Power
        };

        manaBurnStatusEffect.Apply();
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
        var cleanses = target.statusEffects.FindAll(x => !x.Buff);
        cleanses.ForEach(x =>target.statusEffects.Remove(x));

        ResetCleanseStatusEffects(target);
    }

    public void CleanseType(Unit target, StatusEfectEnum type)
    {
        var cleanses = target.statusEffects.FindAll(x => x.statusEfectType == type && !x.Buff);
        cleanses.ForEach(x =>target.statusEffects.Remove(x));

        ResetCleanseStatusEffects(target);
    }

    public void ResetCleanseStatusEffects(Unit unit /**, List<StatusEfectEnum> types**/)
    {
        StartCoroutine(unit.StartTurn());
    }
}
