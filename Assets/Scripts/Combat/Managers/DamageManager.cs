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

    public DamageDataCalculated GetDamageData(DamageData damageEffect, Unit target)
    {
        var caster = damageEffect.Caster;
        
        var damage = CalculateDamage(damageEffect, caster, target);

        DamageDataCalculated data = new DamageDataCalculated()
        {
            DamageType = damageEffect.DamageType,
            Caster = caster,
            MagicalDamage = damageEffect.IsMagical,
            Target = target,
            Damage = damage,
            //statusEffects = AddDefaultStatusEffects(skill)
        };

        //data.statusEffects.AddRange(skill.StatusEfects);

        return data;
    }

    int CalculateDamage(DamageData damageEffect, Unit caster, Unit target)
	{
        var skillPower = damageEffect.Power;
        var casterPower = damageEffect.IsMagical ? caster.MagicalPower : caster.PhysicalPower;
        var targetDefense = damageEffect.IsMagical ? target.MagicalDefense : target.PhysicalDefense;
        var typeModifier = GetTypeModifyer(damageEffect, target);

        var damage = Mathf.CeilToInt((skillPower + casterPower - targetDefense) * typeModifier);

        return damage;
    }

    float GetTypeModifyer(DamageData damageEffect, Unit target)
	{
        if (target.Resistances.Contains(damageEffect.DamageType))
            return 0.5f;

        if (target.Vulnerabilities.Contains(damageEffect.DamageType))
            return 2;

        return 1;
    }

    //List<SO_StatusEffect> AddDefaultStatusEffects(SO_Skillpart skill)
    //{
    //    var statusEffects = new List<SO_StatusEffect>();

    //    foreach (var dse in skill.defaultStatusEffects)
    //    {
    //        var statusEffect = ScriptableObject.CreateInstance<SO_StatusEffect>();
    //        statusEffect.statusEfectType = dse.statusEfectType;
    //        statusEffect.duration = dse.Duration;

    //        statusEffects.Add(statusEffect);
    //    }
    //    return statusEffects;
    //}


    public void DealDamageSetup(SO_Skillpart skillPart, float delay)
    {
        StartCoroutine(DealDamageWithDelay(skillPart, delay));
    }

    IEnumerator DealDamageWithDelay(SO_Skillpart skillPart, float delay)
    {
        yield return new WaitForSeconds(delay);

        DamageData damageEffect = skillPart.DamageEffect;

        foreach (var target in skillPart.PartData.TargetsHit)
        {
            if (damageEffect.Power > 0)
            {
                var data = GetDamageData(damageEffect, target);

                if (damageEffect.DamageType == DamageTypeEnum.Healing)
                    Heal(data);
                else if (damageEffect.DamageType == DamageTypeEnum.Shield)
                    Shield(data);
                else
                    DealDamage(data);
            }
        }
    }

    public void DealDamage(DamageDataCalculated data)
    {
        var caster = data.Caster;
        var target = data.Target;

        Debug.Log(caster.UnitName + " hit " + target.UnitName + " for " + data.Damage + " " + data.DamageType.ToString() + " damage.");
        healthCanvas.ShowDamageNumber(data);

        if (data.Damage == 0)
            return;

        target.TakeDamage(data.Damage);

        var incapacitated = target.statusEffects.Find(x => x.statusEfectType == StatusEfectEnum.Incapacitated);
        if (statusEffectManager.UnitHasStatusEffect(target, StatusEfectEnum.Incapacitated))
            caster.statusEffects.Remove(incapacitated);
    }

    void Heal(DamageDataCalculated data)
    {
        var caster = data.Caster;
        var target = data.Target;

        var correctedHeal = target.Heal(data.Damage);

        data.Damage = correctedHeal;
        
        Debug.Log(caster.UnitName + " heals " + target.UnitName + " for " + data.Damage + ".");
        healthCanvas.ShowDamageNumber(data);

        if (data.Damage == 0)
            return;
    }

    void Shield(DamageDataCalculated data)
    {
        var caster = data.Caster;
        var target = data.Target;

        target.Shield(data.Damage);

        Debug.Log(caster.UnitName + " shields " + target.UnitName + " for " + data.Damage + ".");
        healthCanvas.ShowDamageNumber(data);

        if (data.Damage == 0)
            return;
    }
}
