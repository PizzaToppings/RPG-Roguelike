public class StatusEffect
{
    public DamageManager damageManager = DamageManager.Instance;

    public bool Buff;
    
    public StatusEfectEnum statusEfectType;
    public int Duration;
    public bool IsMagical;

    public Unit Caster;
    public Unit Target;

    public virtual void Apply()
	{
        Target.OnUnitTurnEndEvent.AddListener(ReduceDuration);
        Target.ThisHealthbar.AddStatusEffect(statusEfectType);
    }

    void ReduceDuration()
    {
        Duration--;

        if (Duration == 0)
            EndEffect();
    }

    public virtual void EndEffect()
    {
        Target.ThisHealthbar.RemoveStatusEffect(statusEfectType);
        Target.OnUnitTurnEndEvent.RemoveListener(ReduceDuration);
    }
}

