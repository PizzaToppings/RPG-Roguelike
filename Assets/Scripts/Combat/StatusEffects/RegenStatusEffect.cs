public class RegenStatusEffect : StatusEffect
{
    public int Power;

    public override void Apply()
    {
        base.Apply();

        Target.OnUnitTurnEndEvent.AddListener(Regen);
        SubscribeDurationTrigger();
        Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Regen);
    }

    void Regen()
    {
        var healEffect = new DamageData
        {
            Caster = Caster,
            HitType = HitTypeEnum.Healing,
            Power = Power
        };

        var healData = damageManager.CalculateDamageData(healEffect, Target);
        damageManager.DealDamage(healData);
    }

    public override void EndEffect()
    {
        base.EndEffect();

        Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Regen);
        Target.OnUnitTurnEndEvent.RemoveListener(Regen);
        UnsubscribeDurationTrigger();
    }
}
