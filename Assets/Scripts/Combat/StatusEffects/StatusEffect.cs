public class StatusEffect
{
    public DamageManager damageManager = DamageManager.Instance;

    public bool Buff;
    
    public StatusEfectEnum statusEfectType;
    public int Duration;

    public Unit Caster;
    public Unit Target;

    public virtual void Apply()
	{
        Target.statusEffects.Add(this);
    }

    public void ReduceDuration()
    {
        Duration--;

        if (Duration == 0)
            EndEffect();
    }

    public virtual void EndEffect()
    {
        Target.statusEffects.Remove(this);
    }
}

