using UnityEngine;

public class ThornsStatusEffect : StatusEffect
{
    public int Power;
    public bool IsMagical;

    public override void Apply()
    {
        base.Apply();

        Target.OnUnitTakeDamageEvent.AddListener(Thorns);
        Target.OnUnitTurnEndEvent.AddListener(ReduceDuration);
        Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Thorns);
    }

    public void Thorns(DamagaDataResolved damagaDataResolved)
    {
        if (damagaDataResolved.AttackRange > 1.5f)
            return;

        var damageEffect = new DamageData
        {
            Caster = Target,
            DamageType = DamageTypeEnum.Physical,
            Power = Power,
            IsMagical = IsMagical
        };

        var damageData = damageManager.CalculateDamageData(damageEffect, damagaDataResolved.Attacker);
        damageManager.DealDamage(damageData);
    }

    public override void EndEffect()
    {
        base.EndEffect();

        Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Thorns);
        Target.OnUnitTakeDamageEvent.RemoveListener(Thorns);
    }
}
