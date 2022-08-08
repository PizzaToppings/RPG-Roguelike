using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager damageManager;

    public void Init()
    {
        damageManager = this;
    }

    public DamageData DealDamage(SO_Skillshot skillshot, Unit target)
    {
        var caster = skillshot.Caster;

        var roll = Random.Range(1, skillshot.Damage);
        var addition = SkillshotData.CurrentMainSkillshot.MagicalDamage ? caster.MagicalPower : caster.PhysicalPower;
        var damage = roll + addition;

        DamageData data = new DamageData()
        {
            DamageType = skillshot.DamageType,
            Caster = caster,
            MagicalDamage = SkillshotData.CurrentMainSkillshot.MagicalDamage,
            Target = target,
            Damage = damage,
            statusEffects = AddDefaultStatusEffects(skillshot)
        };

        data.statusEffects.AddRange(skillshot.StatusEfects);

        if (caster.OnDealDamage != null)
            caster.OnDealDamage.Invoke(data);

        return data;
    }

    List<SO_StatusEffect> AddDefaultStatusEffects(SO_Skillshot skillshot)
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

        foreach (var statusEffect in data.statusEffects)
            statusEffect.Apply(caster, target);

        var blinded = caster.statusEffects.Find(x => x.statusEfectType == StatusEfectEnum.Blinded);
        if (blinded != null)
            Blinded(data);

        Debug.Log(caster.UnitName + " hit " + target.UnitName + " for " + data.Damage + " damage.");
    }

    public void Blinded(DamageData data)
    {
        if (data.MagicalDamage)
            return;

        var succes = Random.Range(0f, 1f) > 0.5f;
        if (succes)
        {
            data.Damage = 0;
        }
    }
}
