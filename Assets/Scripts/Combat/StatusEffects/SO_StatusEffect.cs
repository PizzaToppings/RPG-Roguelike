using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultStatusEffect", menuName = "ScriptableObjects/DefaultStatuses/DefaultStatusEffect")]
public class SO_StatusEffect : ScriptableObject
{
    [HideInInspector] public Unit target;

    public StatusEfectEnum statusEfectType;
    public int duration;
    public bool Buff;

    public virtual void Apply(Unit caster, Unit target)
    {
        StatusEffectManager statusEffectManager = StatusEffectManager.Instance;
        if (statusEffectManager.UnitHasStatuseffect(target, StatusEfectEnum.Unstoppable) && !Buff)
            return;

        var status = target.statusEffects.Find(x => x.statusEfectType == statusEfectType);

        if (status == null)
        {
            status = new DefaultStatusEffect();
            status.statusEfectType = statusEfectType;
            status.Duration = duration;
            status.Buff = Buff;
            
            target.statusEffects.Add(status);
        }
    }
}
