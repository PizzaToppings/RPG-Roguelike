using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "ScriptableObjects/SkillEffects/StatusEffect")]
public class SO_StatusEffect : ScriptableObject
{
    [HideInInspector] public StatusEffectManager statusEffectManager = StatusEffectManager.Instance;
    [HideInInspector] public bool Buff;

    public StatusEffectEnum StatusEffectType;
    public DamageTypeEnum DamageType;
    public StatsEnum Stat;
    public int Power;
    public int Duration;
    public bool IsMagical;
    public bool Permanent;

    public virtual void Apply(Unit target)
    {
    }
}
