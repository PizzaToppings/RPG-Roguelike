using UnityEngine;

public class DefaultStatusEffect : StatusEffect
{
    public override void Apply()
    {
        base.Apply();

        Target.OnUnitTurnEndEvent.AddListener(ReduceDuration);
        Target.ThisHealthbar.AddStatusEffect(statusEfectType);
    }

    public override void EndEffect()
    {
        base.EndEffect();

        Target.ThisHealthbar.RemoveStatusEffect(statusEfectType);
        Target.OnUnitTurnEndEvent.RemoveListener(ReduceDuration);
    }
}
