using UnityEngine;

public class PoisonStatusEffect : StatusEffect
{
	public int Power;

	public override void Apply()
	{
        base.Apply();
		
		Target.OnUnitTurnEndEvent.AddListener(Poison);
		SubscribeDurationTrigger();
		Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Poison);
	}

	void Poison()
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
		
		Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Poison);
		Target.OnUnitTurnEndEvent.RemoveListener(Poison);
		UnsubscribeDurationTrigger();
	}
}
