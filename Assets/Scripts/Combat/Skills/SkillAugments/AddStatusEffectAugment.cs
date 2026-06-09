using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddStatusEffectAugment", menuName = "ScriptableObjects/SkillAugments/AddStatusEffectAugment")]
public class AddStatusEffectAugment : SO_SkillAugment
{
    [Tooltip("-1 applies to all groups.")]
    public int SkillPartGroupIndex = -1;

    [Tooltip("-1 applies to all parts inside the selected group(s).")]
    public int SkillPartIndex = -1;

    public List<SO_StatusEffect> StatusEffectsToAdd = new List<SO_StatusEffect>();

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        foreach (var part in GetSkillParts(skill, SkillPartGroupIndex, SkillPartIndex))
        {
            if (part.StatusEffects == null)
                part.StatusEffects = new List<SO_StatusEffect>();

            foreach (var status in StatusEffectsToAdd)
            {
                if (status != null)
                    part.StatusEffects.Add(status);
            }
        }
    }
}
