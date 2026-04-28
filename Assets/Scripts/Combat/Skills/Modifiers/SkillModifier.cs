using UnityEngine;

public abstract class SkillModifier : ScriptableObject
{
    public abstract DamageData Apply(DamageData damageData);
}
