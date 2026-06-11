using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddOrModifyDamageAugment", menuName = "ScriptableObjects/SkillAugments/AddOrModifyDamageAugment")]
public class AddOrModifyDamageAugment : SO_SkillAugment
{
    [Tooltip("-1 applies to all groups.")]
    public int SkillPartGroupIndex = -1;

    [Tooltip("-1 applies to all parts inside the selected group(s).")]
    public int SkillPartIndex = -1;

    public HitTypeEnum HitType = HitTypeEnum.Damage;

    [Tooltip("If true, modify all matching damage entries in a part. If false, modify only the first match.")]
    public bool AffectAllMatches = true;

    [Tooltip("Delta to apply to existing matching damage entries.")]
    public int PowerDelta = 1;

    [Tooltip("When no matching damage entry exists, optionally add one with this base power.")]
    public bool AddIfMissing = true;

    [Tooltip("Base power for a newly added DamageData when missing.")]
    public int BasePower = 1;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        foreach (var part in GetSkillParts(skill, SkillPartGroupIndex, SkillPartIndex))
        {
            if (part.DamageEffects == null)
                part.DamageEffects = new List<DamageData>();

            bool found = false;
            for (int i = 0; i < part.DamageEffects.Count; i++)
            {
                var damage = part.DamageEffects[i];
                if (damage == null)
                    continue;

                if (damage.HitType != HitType)
                    continue;

                damage.Power = Mathf.Max(0, damage.Power + PowerDelta);
                found = true;

                if (!AffectAllMatches)
                    break;
            }

            if (!found && AddIfMissing)
            {
                var newDamage = new DamageData
                {
                    Modifiers = new List<SkillModifier>(),
                    Prerequisites = new List<SO_Prerequisite>(),
                    HitType = HitType,
                    Power = Mathf.Max(0, BasePower),
                    IsMagical = false
                };

                part.DamageEffects.Add(newDamage);
            }
        }
    }
}
