using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    [SerializeField] HealthCanvas healthCanvas;

    StatusEffectManager statusEffectManager;
    public static DamageManager Instance;

    public void Init()
    {
        Instance = this;
        statusEffectManager = StatusEffectManager.Instance;
    }

    public DamageData GetDamageData(SO_Skillpart skill, Unit target)
    {
        var caster = SkillData.Caster;
        var damage = CalculateDamage(skill, caster, target);

        DamageData data = new DamageData()
        {
            DamageType = skill.DamageType,
            Caster = caster,
            MagicalDamage = SkillData.CurrentMainSkill.MagicalDamage,
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
        var power = SkillData.CurrentMainSkill.MagicalDamage ? caster.MagicalPower : caster.PhysicalPower;
        var defense = SkillData.CurrentMainSkill.MagicalDamage ? target.MagicalDefense : target.PhysicalDefense;
        var typeModifier = GetTypeModifyer(skill, target);

        var damage = Mathf.CeilToInt(((10 * skillPower * power / defense) / 50) * typeModifier);
        return damage;
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
            statusEffect.statusEfectType = dse.Type;
            statusEffect.duration = dse.Duration;

            statusEffects.Add(statusEffect);
        }
        return statusEffects;
    }

    public void TakeDamage(DamageData data)
    {
        var caster = data.Caster;
        var target = data.Target;

        if (target.Resistances.Contains(data.DamageType))
            data.Damage = Mathf.CeilToInt(data.Damage / 2f);

        statusEffectManager.Blinded(target, data);

        Debug.Log(caster.UnitName + " hit " + target.UnitName + " for " + data.Damage + " damage.");
        healthCanvas.ShowDamageNumber(data);

        if (data.Damage == 0)
            return;
        
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
}
