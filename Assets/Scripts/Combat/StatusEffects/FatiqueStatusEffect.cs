using UnityEngine;

public class FatiqueStatusEffect : StatusEffect
{
    public int Power;

    public override void Apply()
    {
        base.Apply();

        Target.OnUnitTurnStartEvent.AddListener(BurnMana);
        SubscribeDurationTrigger();
        Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Fatique);
    }

    public void BurnMana()
    {
        // Energy system disabled
        //var character = Target as Character;
        //character.Energy -= Power;
        //character.ThisHealthbar.UpdateHealthbar();
    }

    public override void EndEffect()
    {
        base.EndEffect();

        Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Fatique);
        Target.OnUnitTurnStartEvent.RemoveListener(BurnMana);
        UnsubscribeDurationTrigger();
    }
}
