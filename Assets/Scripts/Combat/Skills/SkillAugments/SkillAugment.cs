public class SkillAugment
{
    public SO_SkillAugment augmentSO;

    public int chargeCount;
    public bool hasTriggered;

    public void Init(SO_SkillAugment so, Skill skill, Character character)
    {
        augmentSO = so;
        chargeCount = 0;
        hasTriggered = false;

        so.Init(skill, this, character);
    }
}
