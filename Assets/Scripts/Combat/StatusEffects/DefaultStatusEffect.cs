using UnityEngine;

public class DefaultStatusEffect : StatusEffect
{
    public override void Apply()
    {
        base.Apply();

        SubscribeDurationTrigger();
        Target.ThisHealthbar.AddStatusEffect(statusEfectType);
    }

    public override void EndEffect()
    {
        base.EndEffect();

        Target.ThisHealthbar.RemoveStatusEffect(statusEfectType);
        UnsubscribeDurationTrigger();
    }
}
