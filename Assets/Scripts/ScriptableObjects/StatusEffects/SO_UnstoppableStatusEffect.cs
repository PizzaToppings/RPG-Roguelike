using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnstoppableStatusEffect", menuName = "ScriptableObjects/DefaultStatuses/UnstoppableStatusEffect")]
public class SO_UnstoppableStatusEffect : SO_StatusEffect
{
    public override void Apply(Unit caster, Unit target)
    {
        base.Apply(caster, target);

        StatusEffectManager statusEffectManager = StatusEffectManager.Instance;
        statusEffectManager.CleanseAll(target);
    }
}
