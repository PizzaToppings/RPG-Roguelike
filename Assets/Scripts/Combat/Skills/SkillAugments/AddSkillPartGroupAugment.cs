using UnityEngine;

[CreateAssetMenu(fileName = "AddSkillPartGroupAugment", menuName = "ScriptableObjects/SkillAugments/AddSkillPartGroupAugment")]
public class AddSkillPartGroupAugment : SO_SkillAugment
{
    [Tooltip("When enabled, clone an existing runtime group by index. Otherwise clone TemplateGroup.")]
    public bool DuplicateExistingGroup = true;

    public int SourceSkillPartGroupIndex = 0;
    public SkillPartGroup TemplateGroup;
    public int Copies = 1;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        if (skill == null || skill.SkillPartGroups == null)
            return;

        int copyCount = Mathf.Max(1, Copies);

        for (int i = 0; i < copyCount; i++)
        {
            SkillPartGroup clonedGroup = null;

            if (DuplicateExistingGroup)
            {
                if (SourceSkillPartGroupIndex < 0 || SourceSkillPartGroupIndex >= skill.SkillPartGroups.Count)
                    continue;

                clonedGroup = SkillAugmentRuntimeCloneUtility.CloneSkillPartGroup(skill.SkillPartGroups[SourceSkillPartGroupIndex]);
            }
            else
            {
                clonedGroup = SkillAugmentRuntimeCloneUtility.CloneSkillPartGroup(TemplateGroup);
            }

            if (clonedGroup != null)
                skill.SkillPartGroups.Add(clonedGroup);
        }
    }
}
