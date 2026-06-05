public class StatusEffect
{
    public DamageManager damageManager = DamageManager.Instance;
    public HealthCanvas healthCanvas = HealthCanvas.Instance;

    // When true, this effect will not spawn floating status text notifications.
    public bool SuppressFloating = false;
    // When true, this effect will be omitted from the character/enemy info panel list.
    public bool HideInInfoPanel = false;

    // If this status effect originated from a combat style application, this is set
    // to the originating style (e.g. Control/Support). Used to surface a compact
    // entry in info panels for externally-applied stances.
    public CombatStyle SourceCombatStyle = CombatStyle.None;
    // When true, the duration ticks on the caster's turn events instead of the target's.
    public bool UseCasterTurnForDuration = false;

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

        // Show floating text unless suppressed
        if (!SuppressFloating)
            healthCanvas.ShowStatusEffect(statusEfectType.ToString(), Target, IsBuff);

        // Subscribe duration reduction on either the caster or the target as configured
        if (UseCasterTurnForDuration && Caster != null)
        {
            if (DurationTrigger == TriggerMomentEnum.EndOfTurn)
                Caster.OnUnitTurnEndEvent.AddListener(ReduceDuration);
            else if (DurationTrigger == TriggerMomentEnum.StartOfTurn)
                Caster.OnUnitTurnStartEvent.AddListener(ReduceDuration);
            else if (DurationTrigger == TriggerMomentEnum.StartOfRound)
                CombatData.onRoundStart.AddListener(ReduceDuration);
            else if (DurationTrigger == TriggerMomentEnum.EndOfRound)
                CombatData.onRoundEnd.AddListener(ReduceDuration);
        }
        else
        {
            SubscribeDurationTrigger();
        }
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
        // Unsubscribe from whichever event was used when applying the effect
        if (UseCasterTurnForDuration && Caster != null)
        {
            switch (DurationTrigger)
            {
                case TriggerMomentEnum.StartOfTurn:
                    Caster.OnUnitTurnStartEvent.RemoveListener(ReduceDuration);
                    break;
                case TriggerMomentEnum.EndOfTurn:
                    Caster.OnUnitTurnEndEvent.RemoveListener(ReduceDuration);
                    break;
                case TriggerMomentEnum.StartOfRound:
                    CombatData.onRoundStart.RemoveListener(ReduceDuration);
                    break;
                case TriggerMomentEnum.EndOfRound:
                    CombatData.onRoundEnd.RemoveListener(ReduceDuration);
                    break;
            }
        }
        else
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
    }

    public void ReduceDuration()
    {
        if (IsPermanent) return;

        // If this effect is configured to tick on the caster's turn, ensure the
        // currently active unit is the caster to avoid decrementing on other units.
        if (UseCasterTurnForDuration)
        {
            if (UnitData.ActiveUnit != Caster)
                return;
        }
        else
        {
            if (DurationTrigger == TriggerMomentEnum.EndOfTurn || DurationTrigger == TriggerMomentEnum.StartOfTurn)
            {
                if (UnitData.ActiveUnit != Target)
                    return;
            }
        }

        Duration--;

        if (Duration == 0)
            EndEffect();
    }

    public virtual void EndEffect()
    {
        Target.statusEffects.Remove(this);
        if (!SuppressFloating)
            healthCanvas.ShowStatusEffect(statusEfectType.ToString() + " faded", Target, IsBuff);
    }
}

