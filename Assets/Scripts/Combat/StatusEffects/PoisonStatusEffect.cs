using UnityEngine;

public class PoisonStatusEffect : StatusEffect
{
	public int Power;

	public override void Apply()
	{
		base.Apply();

		Target.OnUnitTurnEndEvent.AddListener(Poison);
		Target.ThisHealthbar.AddStatusEffect(StatusEfectEnum.Poison);
	}

	void Poison()
	{
		var damageEffect = new DamageData
		{
			Caster = Caster,
			DamageType = DamageTypeEnum.Poison,
			Power = Power,
			IsMagical = IsMagical
		};

		var damageData = damageManager.GetDamageData(damageEffect, Target);
		damageManager.DealDamage(damageData);

		Duration--;

		if (Duration == 0)
			EndEffect();
	}

	public override void EndEffect()
	{
		base.EndEffect();

		Target.ThisHealthbar.RemoveStatusEffect(statusEfectType);
		Target.OnUnitTurnEndEvent.RemoveListener(Poison);
	}
}
