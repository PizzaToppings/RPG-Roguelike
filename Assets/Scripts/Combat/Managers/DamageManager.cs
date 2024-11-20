using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    StatusEffectManager statusEffectManager;
    public static DamageManager Instance;

    public void Init()
    {
        Instance = this;
        statusEffectManager = StatusEffectManager.Instance;
    }

    public DamageData DealDamage(SO_Skillpart skillshot, Unit target)
    {
        var caster = SkillData.Caster;

        var roll = Random.Range(1, skillshot.Damage);
        var addition = SkillData.CurrentMainSkillshot.MagicalDamage ? caster.MagicalPower : caster.PhysicalPower;
        var damage = roll + addition;

        DamageData data = new DamageData()
        {
            DamageType = skillshot.DamageType,
            Caster = caster,
            MagicalDamage = SkillData.CurrentMainSkillshot.MagicalDamage,
            Target = target,
            Damage = damage,
            statusEffects = AddDefaultStatusEffects(skillshot)
        };

        data.statusEffects.AddRange(skillshot.StatusEfects);

        if (caster.OnDealDamage != null)
            caster.OnDealDamage.Invoke(data);

        return data;
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

        //Debug.Log(caster.UnitName + " hit " + target.UnitName + " for " + data.Damage + " damage.");

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
