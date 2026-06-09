using UnityEngine;

[CreateAssetMenu(fileName = "DuplicateSkillPartAugment", menuName = "ScriptableObjects/SkillAugments/DuplicateSkillPartAugment")]
public class DuplicateSkillPartAugment : SO_SkillAugment
{
    public int SourceSkillPartGroupIndex = 0;
    public int SourceSkillPartIndex = 0;
    public int Copies = 1;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        if (skill == null)
            return;

        if (SourceSkillPartGroupIndex < 0 || SourceSkillPartGroupIndex >= skill.SkillPartGroups.Count)
            return;

        var sourceGroup = skill.SkillPartGroups[SourceSkillPartGroupIndex];
        if (sourceGroup?.skillParts == null)
            return;

        if (SourceSkillPartIndex < 0 || SourceSkillPartIndex >= sourceGroup.skillParts.Count)
            return;

        var sourcePart = sourceGroup.skillParts[SourceSkillPartIndex];
        if (sourcePart == null)
            return;

        int copyCount = Mathf.Max(1, Copies);
        for (int i = 0; i < copyCount; i++)
        {
            var clone = SkillAugmentRuntimeCloneUtility.CloneSkillPart(sourcePart);
            if (clone != null)
                sourceGroup.skillParts.Add(clone);
        }
    }
}
