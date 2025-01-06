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
                    statusEffect = CreateBleedEffect(statusEffectSO, target);
                    break;
                case StatusEfectEnum.Poison:
                    statusEffect = CreatePoisonEffect(statusEffectSO, target);
                    break;
            }

            statusEffect.Apply();
		}
	}

    public BleedStatusEffect CreateBleedEffect(SO_StatusEffect statusEffectSO, Unit target)
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

        return bleedStatusEffect;
	}

    public PoisonStatusEffect CreatePoisonEffect(SO_StatusEffect statusEffectSO, Unit target)
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

        return poisonStatusEffect;
    }

    public bool UnitHasStatuseffect(Unit unit, StatusEfectEnum statusEfect)
    {
        return unit.statusEffects.Find(x => x.statusEfectType == statusEfect) != null;
    }

    public bool CantTakeTurn(Unit unit)
    {
        var incapacitated = UnitHasStatuseffect(unit, StatusEfectEnum.Incapacitated);
        var stunned = UnitHasStatuseffect(unit, StatusEfectEnum.Blinded);

        return incapacitated || stunned;
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
