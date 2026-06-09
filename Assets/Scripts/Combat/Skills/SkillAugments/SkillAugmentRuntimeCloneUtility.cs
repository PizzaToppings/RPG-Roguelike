using System.Collections.Generic;
using UnityEngine;

public static class SkillAugmentRuntimeCloneUtility
{
    public static SO_Skillpart CloneSkillPart(SO_Skillpart source)
    {
        if (source == null)
            return null;

        var clone = Object.Instantiate(source);

        // If this part self-references in displacement settings, remap to clone.
        var displacement = clone.displacementEffect;
        if (displacement != null && displacement.UseDisplacement)
        {
            if (displacement.Unit == source)
                displacement.Unit = clone;

            if (displacement.TargetPosition == source)
                displacement.TargetPosition = clone;
        }

        return clone;
    }

    public static SkillPartGroup CloneSkillPartGroup(SkillPartGroup source)
    {
        var clonedGroup = new SkillPartGroup();
        if (source?.skillParts == null)
            return clonedGroup;

        var originalToClone = new Dictionary<SO_Skillpart, SO_Skillpart>();

        foreach (var originalPart in source.skillParts)
        {
            if (originalPart == null)
                continue;

            var clonedPart = Object.Instantiate(originalPart);
            originalToClone[originalPart] = clonedPart;
            clonedGroup.skillParts.Add(clonedPart);
        }

        // Remap displacement references that point within the cloned group.
        foreach (var part in clonedGroup.skillParts)
        {
            var displacement = part.displacementEffect;
            if (displacement == null || !displacement.UseDisplacement)
                continue;

            if (displacement.Unit != null && originalToClone.TryGetValue(displacement.Unit, out var mappedUnit))
                displacement.Unit = mappedUnit;

            if (displacement.TargetPosition != null && originalToClone.TryGetValue(displacement.TargetPosition, out var mappedTarget))
                displacement.TargetPosition = mappedTarget;
        }

        return clonedGroup;
    }
}
