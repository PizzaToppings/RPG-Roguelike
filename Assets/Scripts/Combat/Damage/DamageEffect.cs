using UnityEngine;

[System.Serializable]
public class DamageEffect
{
    [HideInInspector] public Unit Caster;

    public DamageTypeEnum DamageType;
    public int Power;
    public bool IsMagical;
}
