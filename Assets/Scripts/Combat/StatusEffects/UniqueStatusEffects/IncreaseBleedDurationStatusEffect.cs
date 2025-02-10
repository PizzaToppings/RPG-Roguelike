using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseBleedDuration", menuName = "ScriptableObjects/UniqueStatusEffect/IncreaseBleedDuration")]
public class IncreaseBleedDurationStatusEffect : SO_StatusEffect
{
    public override void Apply(Unit target)
    {
        foreach (var statusEffect in target.statusEffects)
        {
            if (statusEffect.statusEfectType == StatusEffectEnum.Bleed)
            {
                statusEffect.Duration += 1;
            }
        }
    }
}
