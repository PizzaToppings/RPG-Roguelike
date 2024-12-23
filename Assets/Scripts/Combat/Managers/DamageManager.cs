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

    public DamageData GetDamageData(SO_Skillpart skill, Unit target)
    {
        var caster = SkillData.Caster;
        var damage = 0;

        if (skill.DamageType == DamageTypeEnum.Healing || skill.DamageType == DamageTypeEnum.Shield)
            damage = CalculateShieldOrHeals(skill, caster);
        else
            damage = CalculateDamage(skill, caster, target);

        DamageData data = new DamageData()
        {
            DamageType = skill.DamageType,
            Caster = caster,
            MagicalDamage = skill.MagicalDamage,
            Target = target,
            Damage = damage,
            statusEffects = AddDefaultStatusEffects(skill)
        };

        data.statusEffects.AddRange(skill.StatusEfects);

        if (caster.OnDealDamage != null)
            caster.OnDealDamage.Invoke(data);

        return data;
    }

    int CalculateDamage(SO_Skillpart skill, Unit caster, Unit target)
	{
        var skillPower = skill.Power;
        var power = skill.MagicalDamage ? caster.MagicalPower : caster.PhysicalPower;
        var defense = skill.MagicalDamage ? target.MagicalDefense : target.PhysicalDefense;
        var typeModifier = GetTypeModifyer(skill, target);

        var damage = Mathf.CeilToInt(((10 * skillPower * power / defense) / 50) * typeModifier);
        return damage;
    }

    int CalculateShieldOrHeals(SO_Skillpart skill, Unit caster)
    {
        var skillPower = skill.Power;
        var power = skill.MagicalDamage ? caster.MagicalPower : caster.PhysicalPower;

        var healOrShieldAmount = Mathf.CeilToInt(10 * skillPower * power / 50);
        return healOrShieldAmount;
    }

    float GetTypeModifyer(SO_Skillpart skill, Unit target)
	{
        if (target.Resistances.Contains(skill.DamageType))
            return 0.5f;

        if (target.Vulnerabilities.Contains(skill.DamageType))
            return 2;

        return 1;
    }

    List<SO_StatusEffect> AddDefaultStatusEffects(SO_Skillpart skillshot)
    {
        var statusEffects = new List<SO_StatusEffect>();

        foreach (var dse in skillshot.defaultStatusEffects)
        {
            var statusEffect = ScriptableObject.CreateInstance<SO_StatusEffect>();
            statusEffect.statusEfectType = dse.statusEfectType;
            statusEffect.duration = dse.Duration;

            statusEffects.Add(statusEffect);
        }
        return statusEffects;
    }


    public void DealDamageSetup(SO_Skillpart skillPart, float delay)
    {
        StartCoroutine(DealDamageWithDelay(skillPart, delay));
    }

    IEnumerator DealDamageWithDelay(SO_Skillpart skillPart, float delay)
    {
        var index = skillPart.SkillPartIndex;

        yield return new WaitForSeconds(delay);
        foreach (var target in SkillData.GetCurrentTargetsHit(index))
        {
            if (skillPart.Power > 0)
            {
                var data = GetDamageData(skillPart, target);

                if (skillPart.DamageType == DamageTypeEnum.Healing)
                    Heal(data);
                else if (skillPart.DamageType == DamageTypeEnum.Shield)
                    Shield(data);
                else
                    DealDamage(data);
            }

        }
    }

    public void DealDamage(DamageData data)
    {
        var caster = data.Caster;
        var target = data.Target;

        Debug.Log(caster.UnitName + " hit " + target.UnitName + " for " + data.Damage + " " + data.DamageType.ToString() + " damage.");
        healthCanvas.ShowDamageNumber(data);

        if (data.Damage == 0)
            return;

        target.TakeDamage(data.Damage);

        var incapacitated = target.statusEffects.Find(x => x.statusEfectType == StatusEfectEnum.Incapacitated);
        if (statusEffectManager.UnitHasStatuseffect(target, StatusEfectEnum.Incapacitated))
            caster.statusEffects.Remove(incapacitated);

        foreach (var statusEffect in data.statusEffects)
            statusEffect.Apply(caster, target);
    }

    public void TakeDotDamage(Unit target)
    {
        var DoTs = statusEffectManager.GetDoTEffects(target);

        foreach (var DoT in DoTs)
        {
            var damage = DoT.Damage;
            if (target.Resistances.Contains(DoT.DamageType))
                damage = Mathf.CeilToInt(damage / 2f);

            Debug.Log(DoT.Caster.UnitName + " dotted " + target.UnitName + " for " + damage + " damage.");
        }
    }

    void Heal(DamageData data)
    {
        var caster = data.Caster;
        var target = data.Target;

        var correctedHeal = target.Heal(data.Damage);

        data.Damage = correctedHeal;
        
        Debug.Log(caster.UnitName + " heals " + target.UnitName + " for " + data.Damage + ".");
        healthCanvas.ShowDamageNumber(data);

        if (data.Damage == 0)
            return;

        foreach (var statusEffect in data.statusEffects)
            statusEffect.Apply(caster, target);
    }

    void Shield(DamageData data)
    {
        var caster = data.Caster;
        var target = data.Target;

        target.Shield(data.Damage);

        Debug.Log(caster.UnitName + " shields " + target.UnitName + " for " + data.Damage + ".");
        healthCanvas.ShowDamageNumber(data);

        if (data.Damage == 0)
            return;

        foreach (var statusEffect in data.statusEffects)
            statusEffect.Apply(caster, target);
    }
}
