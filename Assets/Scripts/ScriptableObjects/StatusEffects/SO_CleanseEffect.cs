using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CleanseEffect", menuName = "ScriptableObjects/DefaultStatuses/CleanseEffect")]
public class SO_CleanseEffect : SO_StatusEffect
{
    public override void Apply(Unit caster, Unit target)
    {
        StatusEffectManager statusEffectManager = StatusEffectManager.Instance;
        statusEffectManager.CleanseAll(target);
    }  
}
