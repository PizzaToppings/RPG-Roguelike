using UnityEngine;

[CreateAssetMenu(fileName = "ModifyRangeAugment", menuName = "ScriptableObjects/SkillAugments/ModifyRangeAugment")]
public class ModifyRangeAugment : SO_SkillAugment
{
    [Tooltip("-1 applies to all groups.")]
    public int SkillPartGroupIndex = -1;

    [Tooltip("-1 applies to all parts inside the selected group(s).")]
    public int SkillPartIndex = -1;

    public float MinRangeDelta = 0f;
    public float MaxRangeDelta = 1f;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        foreach (var part in GetSkillParts(skill, SkillPartGroupIndex, SkillPartIndex))
        {
            part.MinRange = Mathf.Max(0f, part.MinRange + MinRangeDelta);
            part.MaxRange = Mathf.Max(0f, part.MaxRange + MaxRangeDelta);
        }
    }
}
