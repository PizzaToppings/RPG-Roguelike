using UnityEngine;

[CreateAssetMenu(fileName = "AddSkillPartAugment", menuName = "ScriptableObjects/SkillAugments/AddSkillPartAugment")]
public class AddSkillPartAugment : SO_SkillAugment
{
    [Tooltip("Index of the group to add to.")]
    public int TargetSkillPartGroupIndex = 0;

    public SO_Skillpart SkillPartToAdd;
    public int Copies = 1;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        if (skill == null || SkillPartToAdd == null)
            return;

        if (TargetSkillPartGroupIndex < 0 || TargetSkillPartGroupIndex >= skill.SkillPartGroups.Count)
            return;

        var targetGroup = skill.SkillPartGroups[TargetSkillPartGroupIndex];
        if (targetGroup?.skillParts == null)
            return;

        int copyCount = Mathf.Max(1, Copies);
        for (int i = 0; i < copyCount; i++)
        {
            var clone = SkillAugmentRuntimeCloneUtility.CloneSkillPart(SkillPartToAdd);
            if (clone != null)
                targetGroup.skillParts.Add(clone);
        }
    }
}
