using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData 
{
    public DamageTypeEnum DamageType;
    public bool MagicalDamage;
    public int Damage;
    public Unit Caster;
    public Unit Target;
    public List<SO_StatusEffect> statusEffects;

    // status effects
}
