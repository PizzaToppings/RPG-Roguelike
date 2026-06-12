using UnityEngine;

[CreateAssetMenu(fileName = "ModifyCooldownAugment", menuName = "ScriptableObjects/SkillAugments/ModifyCooldownAugment")]
public class ModifyCooldownAugment : SO_SkillAugment
{
    [Header("Trigger")]
    public SkillAugmentTriggerEnum TriggerMoment = SkillAugmentTriggerEnum.OnInit;
    public bool TriggerOnce = false;
    public int ChargesToTrigger = 1;
    [Header("Optional filter (for OnStanceChange)")]
    public CombatStyle RequiredSkillStyle = CombatStyle.None;

    [Header("Effect")]
    [Tooltip("Delta to apply to cooldown (can be negative)")]
    public int Value = 1;

    [Tooltip("Which cooldown variant to affect")]
    public CooldownVariant Target = CooldownVariant.Default;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        Debug.Log($"[ModifyCooldownAugment] '{AugmentName}' Init on skill '{skill?.mainSkillSO?.SkillName}' Trigger: {TriggerMoment} Target: {Target}");

        switch (TriggerMoment)
        {
            case SkillAugmentTriggerEnum.OnInit:
                Apply(skill, character, augment);
                break;
            case SkillAugmentTriggerEnum.OnCast:
                character.OnSkillCastEvent.AddListener(castSkill =>
                {
                    if (castSkill == skill)
                        OnTrigger(skill, character, augment);
                });
                break;
            case SkillAugmentTriggerEnum.OnCastPerTarget:
                character.OnSkillCastEvent.AddListener(castSkill =>
                {
                    if (castSkill != skill) return;
                    OnTrigger(skill, character, augment);
                });
                break;
            case SkillAugmentTriggerEnum.OnStanceChange:
                character.OnStanceChangeEvent.AddListener((oldStyle, newStyle) =>
                {
                    if (RequiredSkillStyle != CombatStyle.None && newStyle != RequiredSkillStyle) return;
                    OnTrigger(skill, character, augment);
                });
                break;
        }
    }

    void OnTrigger(Skill skill, Character character, SkillAugment augment)
    {
        if (TriggerOnce && augment.hasTriggered) return;

        if (ChargesToTrigger > 1)
        {
            augment.chargeCount++;
            if (augment.chargeCount < ChargesToTrigger) return;
            augment.chargeCount = 0;
        }

        Apply(skill, character, augment);
        augment.hasTriggered = true;
    }

    void Apply(Skill skill, Character character, SkillAugment augment)
    {
        if (skill == null) return;

        Debug.Log($"[ModifyCooldownAugment] applying cooldown delta {Value} to '{skill.mainSkillSO.SkillName}' ({Target})");

        if (Target == CooldownVariant.Default || Target == CooldownVariant.Both)
        {
            skill.DefaultCooldown = Mathf.Max(0, skill.DefaultCooldown + Value);
        }

        if (Target == CooldownVariant.Active || Target == CooldownVariant.Both)
        {
            int current = SkillData.GetCooldown(skill);
            SkillData.SetCooldown(skill, Mathf.Max(0, current + Value));
        }
    }
}
