public class StatusEffect
{
    public DamageManager damageManager = DamageManager.Instance;
    public HealthCanvas healthCanvas = HealthCanvas.Instance;

    public bool IsBuff;
    
    public StatusEfectEnum statusEfectType;
    public int Duration;

    public Unit Caster;
    public Unit Target;

    public virtual void Apply()
	{
        Target.statusEffects.Add(this);
        healthCanvas.ShowStatusEffect(statusEfectType.ToString(), Target, IsBuff);
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
        healthCanvas.ShowStatusEffect(statusEfectType.ToString() + " faded", Target, IsBuff);
    }
}

