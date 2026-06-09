using UnityEngine;

public enum SkillAugmentHitTypeFilter
{
    Any,
    Damage,
    Healing,
    Shield
}

[CreateAssetMenu(fileName = "ModifyDamageAugment", menuName = "ScriptableObjects/SkillAugments/ModifyDamageAugment")]
public class ModifyDamageAugment : SO_SkillAugment
{
    [Tooltip("-1 applies to all groups.")]
    public int SkillPartGroupIndex = -1;

    [Tooltip("-1 applies to all parts inside the selected group(s).")]
    public int SkillPartIndex = -1;

    public SkillAugmentHitTypeFilter HitTypeFilter = SkillAugmentHitTypeFilter.Any;
    public int PowerDelta = 1;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        foreach (var part in GetSkillParts(skill, SkillPartGroupIndex, SkillPartIndex))
        {
            if (part.DamageEffects == null)
                continue;

            foreach (var damage in part.DamageEffects)
            {
                if (damage == null || !MatchesFilter(damage.HitType))
                    continue;

                damage.Power = Mathf.Max(0, damage.Power + PowerDelta);
            }
        }
    }

    bool MatchesFilter(HitTypeEnum hitType)
    {
        switch (HitTypeFilter)
        {
            case SkillAugmentHitTypeFilter.Damage:
                return hitType == HitTypeEnum.Damage;
            case SkillAugmentHitTypeFilter.Healing:
                return hitType == HitTypeEnum.Healing;
            case SkillAugmentHitTypeFilter.Shield:
                return hitType == HitTypeEnum.Shield;
            default:
                return true;
        }
    }
}
