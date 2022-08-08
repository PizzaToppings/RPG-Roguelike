using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlindedStatus", menuName = "ScriptableObjects/DefaultStatuses/DefaultStatusEffect")]
public class SO_StatusEffect : ScriptableObject
{
    [HideInInspector] public Unit target;

    public StatusEfectEnum statusEfectType;
    public int duration;

    public virtual void Apply(Unit caster, Unit target)
    {
        var status = target.statusEffects.Find(x => x.statusEfectType == statusEfectType);

        if (status == null)
        {
            status = new StatusEffect();
            status.statusEfectType = statusEfectType;
            status.duration = duration;
            
            target.statusEffects.Add(status);
        }
        else
        {
            status.duration += duration;
        }

        Debug.LogWarning(target.statusEffects[0].statusEfectType);
    }

    public void Remove()
    {
        Destroy(this);
    }
}
