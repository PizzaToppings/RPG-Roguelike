using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoTStatusEffect", menuName = "ScriptableObjects/DefaultStatuses/DoTStatusEffect")]
public class SO_DoTEffect : SO_StatusEffect
{
    public DamageTypeEnum DamageType;
    public int Damage;

    public override void Apply(Unit caster, Unit target)
    {
        var status = new DoTStatusEffect();
        status.statusEfectType = statusEfectType;
        status.Duration = duration;
        status.Caster = caster;
        status.Damage = Damage;
        status.DamageType = DamageType;
        
        target.statusEffects.Add(status);
    }  
}
