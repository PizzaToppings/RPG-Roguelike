public class StatusEffect
{
    public DamageManager damageManager = DamageManager.Instance;
    public HealthCanvas healthCanvas = HealthCanvas.Instance;

    public bool IsBuff;
    public bool IsPermanent;
    public TriggerMomentEnum DurationTrigger = TriggerMomentEnum.EndOfTurn;

    public StatusEffectEnum statusEfectType;
    public int Duration;
    public string Description;

    public Unit Caster;
    public Unit Target;

    public virtual void Apply()
	{
        Target.statusEffects.Add(this);
        healthCanvas.ShowStatusEffect(statusEfectType.ToString(), Target, IsBuff);
    }

    public void SubscribeDurationTrigger()
    {
        switch (DurationTrigger)
        {
            case TriggerMomentEnum.StartOfTurn:
                Target.OnUnitTurnStartEvent.AddListener(ReduceDuration);
                break;
            case TriggerMomentEnum.EndOfTurn:
                Target.OnUnitTurnEndEvent.AddListener(ReduceDuration);
                break;
            case TriggerMomentEnum.StartOfRound:
                CombatData.onRoundStart.AddListener(ReduceDuration);
                break;
            case TriggerMomentEnum.EndOfRound:
                CombatData.onRoundEnd.AddListener(ReduceDuration);
                break;
        }
    }

    public void UnsubscribeDurationTrigger()
    {
        switch (DurationTrigger)
        {
            case TriggerMomentEnum.StartOfTurn:
                Target.OnUnitTurnStartEvent.RemoveListener(ReduceDuration);
                break;
            case TriggerMomentEnum.EndOfTurn:
                Target.OnUnitTurnEndEvent.RemoveListener(ReduceDuration);
                break;
            case TriggerMomentEnum.StartOfRound:
                CombatData.onRoundStart.RemoveListener(ReduceDuration);
                break;
            case TriggerMomentEnum.EndOfRound:
                CombatData.onRoundEnd.RemoveListener(ReduceDuration);
                break;
        }
    }

    public void ReduceDuration()
    {
        if (IsPermanent) return;

        Duration--;

        if (Duration == 0)
            EndEffect();
    }

    public virtual void EndEffect()
    {
        Target.statusEffects.Remove(this);
        healthCanvas.ShowStatusEffect(statusEfectType.ToString() + " faded", Target, IsBuff);
    }
}

