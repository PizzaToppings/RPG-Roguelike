using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "ScriptableObjects/Passives/DefaultPassive")]
public class DefaultPassiveEffect : SO_PassiveEffect
{
    public TriggerMomentEnum TriggerMoment;
    public TriggerEffectEnum TriggerEffect;
}
