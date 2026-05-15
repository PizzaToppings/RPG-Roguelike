using UnityEngine;

public class SO_SkillAugment : ScriptableObject
{
    public string AugmentName;
    public Sprite Image;

    [TextArea(5, 10)]
    public string Description;

    public virtual void Init(Skill skill, SkillAugment augment, Character character)
    {
    }
}
