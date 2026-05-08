using UnityEngine;

[CreateAssetMenu(fileName = "SharpeningStoneTrinket", menuName = "ScriptableObjects/Trinkets/SharpeningStoneTrinket")]
public class SharpeningStoneTrinket : SO_Trinket
{
    public float DamageMultiplier = 2f;

    public override void Init(Character character, Trinket trinket)
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
