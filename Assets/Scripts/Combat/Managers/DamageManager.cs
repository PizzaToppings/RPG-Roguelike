using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    [SerializeField] HealthCanvas healthCanvas;

    StatusEffectManager statusEffectManager;
    public static DamageManager Instance;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        statusEffectManager = StatusEffectManager.Instance;
    }

    public DamageDataCalculated CalculateDamageData(DamageData damageData, Unit target)
    {
        var caster = damageData.Caster;

        foreach (var modifier in damageData.Modifiers)
        {
            damageData = modifier.Apply(damageData);
        }
        
        var damage = CalculateDamage(damageData, caster, target);

        DamageDataCalculated data = new DamageDataCalculated
        {
            HitType = damageData.HitType,
            Caster = caster,
            Target = target,
            Damage = damage,
        };


        return data;
    }

    int CalculateDamage(DamageData damageEffect, Unit caster, Unit target)
	{
        var skillPower = damageEffect.Power;
        var casterPower = caster.Power;
        var bleedDamage = GetBleedBonusDamage(target);

        if (damageEffect.HitType == HitTypeEnum.Healing || damageEffect.HitType == HitTypeEnum.Shield)
        {
            var beneficial = Mathf.CeilToInt(skillPower);
            return beneficial < 0 ? 0 : beneficial;
        }

        var targetArmor = target.Armor;
        var damage = Mathf.CeilToInt(skillPower + bleedDamage + casterPower - targetArmor);
        if (damage < 0)
            damage = 0;

        return damage;
    }

    int GetBleedBonusDamage(Unit target)
    {
        if (statusEffectManager.UnitHasStatusEffect(target, StatusEffectEnum.Bleed))
            return 2;

        return 0;
    }


    public void DealDamageSetup(SO_Skillpart skillPart, float delay)
    {
        StartCoroutine(DealDamageWithDelay(skillPart, delay));
    }

    IEnumerator DealDamageWithDelay(SO_Skillpart skillPart, float delay)
    {
        // Snapshot and immediately reset so the multiplier only applies to this skillpart.
        var caster = skillPart.DamageEffects.Count > 0 ? skillPart.DamageEffects[0].Caster : null;
        var damageMultiplier = caster != null ? caster.OutgoingDamageMultiplier : 1f;
        if (caster != null) caster.OutgoingDamageMultiplier = 1f;

        yield return new WaitForSeconds(delay);

        List<DamageData> damageEffects = skillPart.DamageEffects;

        foreach (var target in skillPart.PartData.TargetsHit)
        {
            foreach (var damageEffect in damageEffects)
            {
                var canCast = true;
                foreach (var prerequisite in damageEffect.Prerequisites)
                {
                    canCast = prerequisite.HasPrerequisite(damageEffect.Caster, target);
                    if (canCast == false)
                        break;
                }
                if (canCast == false)
                    break;

                var data = CalculateDamageData(damageEffect, target);
                data.Damage = Mathf.CeilToInt(data.Damage * damageMultiplier);

                if (damageEffect.HitType == HitTypeEnum.Healing)
                    HealUnit(data);
                else if (damageEffect.HitType == HitTypeEnum.Shield)
                    ShieldUnit(data);
                else
                    DealDamage(data);
            }
        }
    }

    public void DealDamage(DamageDataCalculated data)
    {
        var caster = data.Caster;
        var target = data.Target;

        if (data.Damage == 0)
            return;

        var damageDataResolved = target.TakeDamage(data);
        
        healthCanvas.ShowDamageNumber(damageDataResolved);

        if (caster != null)
            caster.OnDealDamage?.Invoke(data);

        if (target.Hitpoints <= 0 && !target.Friendly && caster != null)
            caster.OnKillEnemyEvent.Invoke(target);

        if (statusEffectManager.UnitHasStatusEffect(target, StatusEffectEnum.Incapacitated))
        {
            var incapacitated = target.statusEffects.Find(x => x.statusEfectType == StatusEffectEnum.Incapacitated);
            target.statusEffects.Remove(incapacitated);
        }
    }

    public void HealUnit(DamageDataCalculated data)
    {
        var target = data.Target;

        var correctedHeal = target.Heal(data);
        
        healthCanvas.ShowHealNumber(correctedHeal);
    }

    public void ShieldUnit(DamageDataCalculated data)
    {
        var target = data.Target;
        var shieldsCalculated = target.Shield(data);

        healthCanvas.ShowHealNumber(shieldsCalculated);
    }
}
