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
		Target.ThisHealthbar.AddStatusEffect(StatusEfectEnum.Burn);
	}

	void Burn()
	{
		Debug.Log("Burning " + Target.UnitName);

		var damageEffect = new DamageData
		{
			Caster = Caster,
			DamageType = DamageTypeEnum.Fire,
			Power = Power,
			IsMagical = IsMagical
		};

		var boardManager = BoardManager.Instance;
		var damageTiles = boardManager.GetTilesWithinDirectRange(Target.Tile, 2.5f, false);

        var tileColor = new TileColor
        { 
			Color = Color.red,
			FillCenter = true, 
			Priority = 1
		};

        damageTiles.ForEach(x => x.SetColor(tileColor));

		var targets = new List<Unit>();
		targets.AddRange(damageTiles.Where(tile => tile.currentUnit != null).Select(tile => tile.currentUnit));

		var damageData = damageManager.GetDamageData(damageEffect, Target);
		damageManager.DealDamage(damageData);

		foreach (var target in targets)
		{
			damageData = damageManager.GetDamageData(damageEffect, target);
			damageManager.DealDamage(damageData);
		}

		Duration--;

		if (Duration == 0)
			EndEffect();
	}

	public override void EndEffect()
	{
        base.EndEffect();
		
		Target.ThisHealthbar.RemoveStatusEffect(StatusEfectEnum.Burn);
		Target.OnUnitTurnEndEvent.RemoveListener(Burn);
	}
}
