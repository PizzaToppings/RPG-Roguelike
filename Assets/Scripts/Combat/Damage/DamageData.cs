using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageData
{
    [HideInInspector] public Unit Caster;

    public List<SkillModifier> Modifiers = new List<SkillModifier>();
    public List<SO_Prerequisite> Prerequisites = new List<SO_Prerequisite>();
    public HitTypeEnum HitType;
    public int Power;
}
