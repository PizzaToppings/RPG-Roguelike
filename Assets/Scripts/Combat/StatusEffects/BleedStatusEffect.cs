using UnityEngine;

public class BleedStatusEffect : StatusEffect
{
    public int Power;

	public override void Apply()
	{
        base.Apply();

		Target.OnUnitTurnEndEvent.AddListener(Bleed);
		SubscribeDurationTrigger();
		Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Bleed);
	}

	void Bleed()
	{
		var damageEffect = new DamageData
		{
			Caster = Caster,
			HitType = HitTypeEnum.Damage,
			Power = Power
		};

		var damageData = damageManager.CalculateDamageData(damageEffect, Target);
		damageManager.DealDamage(damageData);
	}

	public override void EndEffect()
	{
        base.EndEffect();

		Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Bleed);
		Target.OnUnitTurnEndEvent.RemoveListener(Bleed);
		UnsubscribeDurationTrigger();
	}
}
