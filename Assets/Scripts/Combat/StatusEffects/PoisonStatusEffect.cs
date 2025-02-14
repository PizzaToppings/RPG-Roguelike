using UnityEngine;

public class PoisonStatusEffect : StatusEffect
{
	public int Power;
	public bool IsMagical;

	public override void Apply()
	{
        base.Apply();
		
		Target.OnUnitTurnEndEvent.AddListener(Poison);
		Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Poison);
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

		var damageData = damageManager.CalculateDamageData(damageEffect, Target);
		damageManager.DealDamage(damageData);

		Duration--;

		if (Duration == 0)
			EndEffect();
	}

	public override void EndEffect()
	{
        base.EndEffect();
		
		Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Poison);
		Target.OnUnitTurnEndEvent.RemoveListener(Poison);
	}
}
