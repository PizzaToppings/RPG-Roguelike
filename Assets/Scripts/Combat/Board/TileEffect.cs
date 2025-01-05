using UnityEngine;

public class TileEffect 
{
    SO_TileEffect _originalTileEffect;
    BoardTile _tile;
    DamageEffect _damageEffect;
    int _duration;

    public void Init(SO_TileEffect originalTileEffect, BoardTile tile)
	{
        _originalTileEffect = originalTileEffect;
        _tile = tile;
        _damageEffect = originalTileEffect.damageEffect;
        _damageEffect.Caster = UnitData.ActiveUnit;
        _duration = originalTileEffect.MaxDuration;

        _tile.hasTileEffect = true;
        _tile.tileEffectColor = originalTileEffect.tileColor;

        SetTriggerMoment();

        CombatData.onTurnStart.AddListener(ReduceDuration);
    }

    void ReduceDuration()
	{
        if (UnitData.ActiveUnit != _damageEffect.Caster)
            return;

        _duration--;

        if (_duration == 0)
            RemoveEffect();
	}

    void SetTriggerMoment()
	{
		switch (_originalTileEffect.TriggerMoment)
		{
            case TileTriggerMomentEnum.OnEnterTile:
                _tile.OnEnterTileEvent.AddListener(SetTriggerEffect);
                return;

            case TileTriggerMomentEnum.Aura:
                _tile.OnEnterTileEvent.AddListener(SetTriggerEffect);
                _tile.OnExitTileEvent.AddListener(RemoveTriggerEffect);
                return;

            case TileTriggerMomentEnum.StartOfTurn:
                CombatData.onTurnStart.AddListener(SetTriggerEffect);
                return;

            case TileTriggerMomentEnum.EndOfTurn:
                CombatData.onTurnEnd.AddListener(SetTriggerEffect);
                return;
        }
    }

    void SetTriggerEffect()
	{
        switch (_originalTileEffect.TriggerEffect)
		{
            case TileTriggerEffectEnum.DealDamage:
                DamageManager damageManager = DamageManager.Instance;
                var damageData = damageManager.GetDamageData(_damageEffect, _tile.currentUnit); 
                damageManager.DealDamage(damageData);
                return;
		}
	}
    
    void RemoveTriggerEffect()
	{
        // TODO
	}

    void RemoveEffect()
    {
        if (_tile.tileEffectColor == _originalTileEffect.tileColor)
		{
            _tile.hasTileEffect = false;
            _tile.tileEffectColor = null;
		}

        switch (_originalTileEffect.TriggerMoment)
        {
            case TileTriggerMomentEnum.OnEnterTile:
                Debug.Log("adding OnEnterTile effect");
                _tile.OnEnterTileEvent.RemoveListener(SetTriggerEffect);
                return;

            case TileTriggerMomentEnum.Aura:
                _tile.OnEnterTileEvent.RemoveListener(SetTriggerEffect);
                _tile.OnExitTileEvent.RemoveListener(RemoveTriggerEffect);
                return;

            case TileTriggerMomentEnum.StartOfTurn:
                CombatData.onTurnStart.RemoveListener(SetTriggerEffect);
                return;

            case TileTriggerMomentEnum.EndOfTurn:
                CombatData.onTurnEnd.RemoveListener(SetTriggerEffect);
                return;
        }
    }
}
