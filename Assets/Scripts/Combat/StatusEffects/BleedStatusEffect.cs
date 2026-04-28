using UnityEngine;

public class BleedStatusEffect : StatusEffect
{
    public int Power;
	public bool IsMagical;

	public override void Apply()
	{
        base.Apply();

		Target.OnUnitTurnEndEvent.AddListener(Bleed);
		Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Bleed);
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

		var damageData = damageManager.CalculateDamageData(damageEffect, Target);
		damageManager.DealDamage(damageData);

		Duration--;

		if (Duration == 0)
			EndEffect();
	}

	public override void EndEffect()
	{
        base.EndEffect();

		Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Bleed);
		Target.OnUnitTurnEndEvent.RemoveListener(Bleed);
	}
}
