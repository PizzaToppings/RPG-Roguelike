using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trinket", menuName = "ScriptableObjects/Trinkets/DefaultTrinket")]
public class DefaultTrinket : SO_Trinket
{
    public TriggerMomentEnum TriggerMoment;
    public TriggerEffectEnum TriggerEffect;
    public int ChargesToTrigger;
    public bool TriggerOnce;

    [Space]
    public int Power;

    [Space]
    public TargetEnum Target;
    public float Range;

    [Space]
    public StatusEffectEnum StatusEffect;
    public List<StatsEnum> Stat;

    public override void Init(Character character)
    {

    }
}
