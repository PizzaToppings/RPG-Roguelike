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
	}

    public virtual void EndEffect()
    {
    }
}

