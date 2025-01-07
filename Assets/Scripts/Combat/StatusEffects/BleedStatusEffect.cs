using UnityEngine;

public class BleedStatusEffect : StatusEffect
{
    public int Power;

	public override void Apply()
	{
		Target.OnUnitTurnEndEvent.AddListener(Bleed);
		Target.ThisHealthbar.AddStatusEffect(StatusEfectEnum.Bleed);
	}

	void Bleed()
	{
		var damageEffect = new DamageData
		{
			Caster = Caster,
			DamageType = DamageTypeEnum.Physical,
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
		Target.ThisHealthbar.RemoveStatusEffect(StatusEfectEnum.Bleed);
		Target.OnUnitTurnEndEvent.RemoveListener(Bleed);
	}
}
