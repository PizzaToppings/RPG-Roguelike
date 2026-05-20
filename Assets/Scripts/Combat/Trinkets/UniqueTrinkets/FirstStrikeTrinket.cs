using UnityEngine;

[CreateAssetMenu(fileName = "SharpeningStoneTrait", menuName = "ScriptableObjects/Traits/SharpeningStoneTrait")]
public class SharpeningStoneTrait : SO_Trait
{
    public float DamageMultiplier = 2f;

    public override void Init(Character character, Trait trait)
    {
        character.OutgoingDamageMultiplier = DamageMultiplier;

        SkillsManager.Instance.OnSkillCastComplete.AddListener(OnFirstSkillCast);

        void OnFirstSkillCast()
        {
            character.OutgoingDamageMultiplier = 1f;
            SkillsManager.Instance.OnSkillCastComplete.RemoveListener(OnFirstSkillCast);
        }
    }
}
