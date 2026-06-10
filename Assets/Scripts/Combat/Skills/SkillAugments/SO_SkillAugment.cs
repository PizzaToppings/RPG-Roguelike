using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SO_SkillAugment : ScriptableObject
{
    public string AugmentName;

    [TextArea(5, 10)]
    public string Description;

    public virtual void Init(Skill skill, SkillAugment augment, Character character)
    {
    }

    protected static IEnumerable<SO_Skillpart> GetSkillParts(Skill skill, int groupIndex = -1, int partIndex = -1)
    {
        if (skill == null || skill.SkillPartGroups == null)
            yield break;

        for (int g = 0; g < skill.SkillPartGroups.Count; g++)
        {
            if (groupIndex >= 0 && g != groupIndex)
                continue;

            var group = skill.SkillPartGroups[g];
            if (group?.skillParts == null)
                continue;

            for (int p = 0; p < group.skillParts.Count; p++)
            {
                if (partIndex >= 0 && p != partIndex)
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
