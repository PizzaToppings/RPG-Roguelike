using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SO_SkillAugment : ScriptableObject
{
    public string AugmentName;

    [Tooltip("Optional prerequisite that must be satisfied for this augment to trigger.")]
    public SO_Prerequisite Prerequisite;

    [TextArea(5, 10)]
    public string Description;

    public virtual void Init(Skill skill, SkillAugment augment, Character character)
    {
    }

    protected static IEnumerable<SO_Skillpart> GetSkillParts(Skill skill, int groupIndex = -1, int partIndex = -1)
    {
        if (skill == null || skill.SkillPartGroups == null)
            yield break;

        // If a positive groupIndex was provided but is out-of-range, treat it as the last group.
        int effectiveGroupIndex = groupIndex;
        if (effectiveGroupIndex >= 0 && effectiveGroupIndex >= skill.SkillPartGroups.Count)
            effectiveGroupIndex = skill.SkillPartGroups.Count - 1;

        for (int g = 0; g < skill.SkillPartGroups.Count; g++)
        {
            if (effectiveGroupIndex >= 0 && g != effectiveGroupIndex)
                continue;

            var group = skill.SkillPartGroups[g];
            if (group?.skillParts == null || group.skillParts.Count == 0)
                continue;

            // Determine effective part index for this group: if a positive partIndex was provided
            // but is out-of-range for this group's parts, fall back to the last part.
            int effectivePartIndex = partIndex;
            if (effectivePartIndex >= 0 && effectivePartIndex >= group.skillParts.Count)
                effectivePartIndex = group.skillParts.Count - 1;

            for (int p = 0; p < group.skillParts.Count; p++)
            {
                if (effectivePartIndex >= 0 && p != effectivePartIndex)
                    continue;

                var part = group.skillParts[p];
                if (part != null)
                    yield return part;
            }
        }
    }

    protected static List<Unit> GetAllSkillTargets(Skill skill)
    {
        var targets = new List<Unit>();
        foreach (var part in GetSkillParts(skill))
        {
            if (part.PartData?.TargetsHit == null)
                continue;

            targets.AddRange(part.PartData.TargetsHit.Where(t => t != null && !targets.Contains(t)));
        }

        return targets;
    }
}
