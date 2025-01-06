using UnityEngine;

[System.Serializable]
public class DamageData
{
    [HideInInspector] public Unit Caster;

    public DamageTypeEnum DamageType;
    public int Power;
    public bool IsMagical;
}
