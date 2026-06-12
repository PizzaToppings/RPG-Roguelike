using UnityEngine;

[CreateAssetMenu(fileName = "ReplaceSkillPartAugment", menuName = "ScriptableObjects/SkillAugments/ReplaceSkillPartAugment")]
public class ReplaceSkillPartAugment : SO_SkillAugment
{
    [Tooltip("Index of the group containing the part to replace. If negative, no-op.")]
    public int SourceSkillPartGroupIndex = 0;

    [Tooltip("Index of the part inside the source group to replace. If negative, no-op.")]
    public int SourceSkillPartIndex = 0;

    [Tooltip("Index inside the target group to insert the replacement part. -1 to append. If the target index points to an existing part it will be replaced; if out-of-range the replacement will be deferred (and inserted when processed).")]
    public int TargetSkillPartIndex = -1;

    [Tooltip("Group to place the replacement into. If out-of-range the last group will be used.")]
    public int TargetSkillPartGroupIndex = 0;

    public SO_Skillpart ReplacementPart;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        if (skill == null || ReplacementPart == null)
            return;

        if (SourceSkillPartGroupIndex < 0)
            return;

        int effectiveSourceGroupIndex = SourceSkillPartGroupIndex;
        if (effectiveSourceGroupIndex >= skill.SkillPartGroups.Count)
            return; // source group doesn't exist

        var sourceGroup = skill.SkillPartGroups[effectiveSourceGroupIndex];
        if (sourceGroup?.skillParts == null)
            return;

        if (SourceSkillPartIndex < 0 || SourceSkillPartIndex >= sourceGroup.skillParts.Count)
            return; // source part doesn't exist

        var sourcePart = sourceGroup.skillParts[SourceSkillPartIndex];
        if (sourcePart == null)
            return;

        // Clone the replacement so runtime instance is independent
        var clone = SkillAugmentRuntimeCloneUtility.CloneSkillPart(ReplacementPart);
        if (clone == null)
            return;

        // If target group is negative, no-op
        if (TargetSkillPartGroupIndex < 0)
            return;

        int effectiveTargetGroupIndex = TargetSkillPartGroupIndex;
        if (effectiveTargetGroupIndex >= skill.SkillPartGroups.Count)
            effectiveTargetGroupIndex = skill.SkillPartGroups.Count - 1;

        var targetGroup = skill.SkillPartGroups[effectiveTargetGroupIndex];
        if (targetGroup?.skillParts == null)
            return;

        // If TargetSkillPartIndex < 0 -> append and remove original source part
        if (TargetSkillPartIndex < 0)
        {
            // Remove the original part reference from its group
            RemoveOriginalPartReference(skill, sourceGroup, sourcePart);
            targetGroup.skillParts.Add(clone);
            return;
        }

        // If the desired index is within current bounds, replace now
        if (TargetSkillPartIndex <= targetGroup.skillParts.Count - 1)
        {
            int replaceAt = Mathf.Min(TargetSkillPartIndex, targetGroup.skillParts.Count - 1);

            // Remove original part reference
            RemoveOriginalPartReference(skill, sourceGroup, sourcePart);

            // If replacing in same group and the removed index is before replaceAt, adjust replaceAt
            if (sourceGroup == targetGroup)
            {
                int sourceIdx = sourceGroup.skillParts.IndexOf(sourcePart);
                if (sourceIdx >= 0 && sourceIdx < replaceAt)
                    replaceAt--; // after removal indices shift left
            }

            // If replaceAt is at end, perform insert
            if (replaceAt >= targetGroup.skillParts.Count)
                targetGroup.skillParts.Add(clone);
            else
                targetGroup.skillParts[replaceAt] = clone;

            return;
        }

        // Otherwise the configured index is out-of-range relative to the current
        // parts. Defer insertion so multiple out-of-range operations can be ordered by
        // their configured index. Use the same deferred queue used by AddSkillPartAugment.
        AddSkillPartAugment.RegisterDeferredAdd(skill, effectiveTargetGroupIndex, TargetSkillPartIndex, clone);

        // Also remove the original part reference now so it doesn't linger.
        RemoveOriginalPartReference(skill, sourceGroup, sourcePart);
    }

    void RemoveOriginalPartReference(Skill skill, SkillPartGroup sourceGroup, SO_Skillpart sourcePart)
    {
        // If the source part still exists in its original group, remove it.
        if (sourceGroup != null && sourceGroup.skillParts != null)
        {
            // Remove the first matching reference only
            for (int i = 0; i < sourceGroup.skillParts.Count; i++)
            {
                if (sourceGroup.skillParts[i] == sourcePart)
                {
                    sourceGroup.skillParts.RemoveAt(i);
                    break;
                }
            }
        }

        // If the source was in a different group and the same SO instance exists elsewhere,
        // we don't attempt to remove those; we only remove the original runtime reference.
    }
}
