using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BurnStatusEffect : StatusEffect
{
	public int Power;
	public bool IsMagical;

	public override void Apply()
	{
        base.Apply();
	
		Target.OnUnitTurnEndEvent.AddListener(Burn);
		SubscribeDurationTrigger();
		Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Burn);
	}

	void Burn()
	{
		Debug.Log("Burning " + Target.UnitName);

		var damageEffect = new DamageData
		{
			Caster = Caster,
			HitType = HitTypeEnum.Damage,
			Power = Power,
			IsMagical = IsMagical
		};

		var boardManager = BoardManager.Instance;
		var damageTiles = boardManager.GetTilesWithinDirectRange(Target.Tile, 2.5f, false);

        var tileColor = BoardManager.Instance.GetTileColor(TileColorKind.EnemyIntent);

        damageTiles.ForEach(x => x.SetColor(tileColor));

		var targets = new List<Unit>();
		targets.AddRange(damageTiles.Where(tile => tile.currentUnit != null).Select(tile => tile.currentUnit));

		var damageData = damageManager.CalculateDamageData(damageEffect, Target);
		damageManager.DealDamage(damageData);

		foreach (var target in targets)
		{
			damageData = damageManager.CalculateDamageData(damageEffect, target);
			damageManager.DealDamage(damageData);
		}
	}

	public override void EndEffect()
	{
        base.EndEffect();
		
		Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Burn);
		Target.OnUnitTurnEndEvent.RemoveListener(Burn);
		UnsubscribeDurationTrigger();
	}
}
