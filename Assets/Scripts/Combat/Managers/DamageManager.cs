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
        var addition = skillshot.MagicalDamage ? caster.MagicalPower : caster.PhysicalPower;
        var damage = roll + addition;

        DamageData data = new DamageData();
        data.DamageType = skillshot.DamageType;
        data.Caster = caster;
        data.MagicalDamage = skillshot.MagicalDamage;
        data.Target = target;
        data.Damage = damage;
        data.statusEffects = skillshot.StatusEfects;

        if (caster.OnDealDamage != null)
            caster.OnDealDamage.Invoke(data);

        return data;
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
