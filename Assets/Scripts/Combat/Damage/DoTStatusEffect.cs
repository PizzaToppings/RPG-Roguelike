using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTStatusEffect : DefaultStatusEffect
{
    public DamageTypeEnum DamageType;
    public int Damage;

    [HideInInspector] public Unit Caster;
}
