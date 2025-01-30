using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageData
{
    [HideInInspector] public Unit Caster;

    public List<Prerequisite> Prerequisites = new List<Prerequisite>();
    public DamageTypeEnum DamageType;
    public int Power;
    public bool IsMagical;
}
