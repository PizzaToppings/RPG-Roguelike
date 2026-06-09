using UnityEngine;

[CreateAssetMenu(fileName = "ImproveStatusEffectAugment", menuName = "ScriptableObjects/SkillAugments/ImproveStatusEffectAugment")]
public class ImproveStatusEffectAugment : SO_SkillAugment
{
    [Tooltip("-1 applies to all groups.")]
    public int SkillPartGroupIndex = -1;

    [Tooltip("-1 applies to all parts inside the selected group(s).")]
    public int SkillPartIndex = -1;

    public StatusEffectEnum StatusEffectType = StatusEffectEnum.Bleed;
    public int PowerDelta = 1;
    public int DurationDelta = 1;
    public bool AffectAllMatches = true;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        foreach (var part in GetSkillParts(skill, SkillPartGroupIndex, SkillPartIndex))
        {
            if (part.StatusEffects == null || part.StatusEffects.Count == 0)
                continue;

            for (int i = 0; i < part.StatusEffects.Count; i++)
            {
                var status = part.StatusEffects[i];
                if (status == null || status.StatusEffectType != StatusEffectType)
                    continue;

                // Clone before editing so shared status assets remain unchanged.
                var clone = Object.Instantiate(status);
                clone.Power = Mathf.Max(0, clone.Power + PowerDelta);
                clone.Duration = Mathf.Max(0, clone.Duration + DurationDelta);
                part.StatusEffects[i] = clone;

                if (!AffectAllMatches)
                    break;
            }
        }
    }
}
