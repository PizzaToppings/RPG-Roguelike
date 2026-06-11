using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data-driven skill augment. Configure entirely in the Inspector.
/// For complex behaviour create a custom SO_SkillAugment subclass instead.
/// </summary>
[CreateAssetMenu(fileName = "SkillAugment", menuName = "ScriptableObjects/Skills/DefaultSkillAugment")]
public class DefaultSkillAugment : SO_SkillAugment
{
    [Header("Trigger")]
    public SkillAugmentTriggerEnum TriggerMoment;
    public bool TriggerOnce;
    public int ChargesToTrigger = 1;
    [Header("Optional filter (for OnStanceChange)")]
    public CombatStyle RequiredSkillStyle = CombatStyle.None;

    [Header("Effect")]
    public SkillAugmentEffectEnum TriggerEffect;

    [Tooltip("General-purpose value (energy cost delta, range delta, damage delta, etc.)")]
    public int Value = 1;

    [Header("Status Effects")]
    public List<SO_StatusEffect> StatusEffects;

    [Header("Reset Skill (for ResetSkill effect)")]
    [Tooltip("Leave null to reset the basic attack. Set to a specific SO to reset that skill.")]
    public SO_MainSkill SkillToReset;

    // ?????????????????????????????????????????????????????????????????

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        Debug.Log($"[SkillAugment] '{AugmentName}' Init on skill '{skill.mainSkillSO.SkillName}' � Trigger: {TriggerMoment}, Effect: {TriggerEffect}");

        switch (TriggerMoment)
        {
            case SkillAugmentTriggerEnum.OnInit:
                ApplyEffect(skill, null, character, augment);
                break;

            case SkillAugmentTriggerEnum.OnCast:
                character.OnSkillCastEvent.AddListener(castSkill =>
                {
                    if (castSkill == skill)
                        OnTrigger(skill, null, character, augment);
                });
                break;

            case SkillAugmentTriggerEnum.OnCastPerTarget:
                character.OnSkillCastEvent.AddListener(castSkill =>
                {
                    if (castSkill != skill) return;
                    var targets = GetAllSkillTargets(skill);
                    foreach (var target in targets)
                        OnTrigger(skill, target, character, augment);
                });
                break;
            case SkillAugmentTriggerEnum.OnStanceChange:
                character.OnStanceChangeEvent.AddListener((oldStyle, newStyle) =>
                {
                    if (RequiredSkillStyle != CombatStyle.None && newStyle != RequiredSkillStyle) return;
                    OnTrigger(skill, null, character, augment);
                });
                break;
        }
    }

    // ?????????????????????????????????????????????????????????????????

    void OnTrigger(Skill skill, Unit target, Character character, SkillAugment augment)
    {
        // If a prerequisite SO is configured on the augment, evaluate it first.
        if (augment?.augmentSO != null && augment.augmentSO.Prerequisite != null)
        {
            // For per-target triggers, pass the specific target; otherwise pass null target and let prerequisite handle caster-only checks.
            var prereqTarget = target;
            var has = augment.augmentSO.Prerequisite.HasPrerequisite(character, prereqTarget);
            if (!has)
                return;
        }

        if (TriggerOnce && augment.hasTriggered) return;

        if (ChargesToTrigger > 1)
        {
            augment.chargeCount++;
            if (augment.chargeCount < ChargesToTrigger) return;
            augment.chargeCount = 0;
        }

        ApplyEffect(skill, target, character, augment);
        augment.hasTriggered = true;
    }

    void ApplyEffect(Skill skill, Unit target, Character character, SkillAugment augment)
    {
        Debug.Log($"[SkillAugment] '{AugmentName}' applying effect '{TriggerEffect}' on '{character.UnitName}'");

        switch (TriggerEffect)
        {
            case SkillAugmentEffectEnum.AddStatusEffectToTargets:
            {
                var targets = target != null
                    ? new List<Unit> { target }
                    : GetAllSkillTargets(skill);

                foreach (var se in StatusEffects)
                {
                    if (se == null) continue;

                    // Check for any ImproveStatusEffectAugment instances applied to this skill
                    int totalPowerDelta = 0;
                    int totalDurationDelta = 0;
                    bool anyModifier = false;

                    foreach (var sa in skill.Augments)
                    {
                        if (sa?.augmentSO is ImproveStatusEffectAugment imp)
                        {
                            if (imp.StatusEffectType == se.StatusEffectType)
                            {
                                totalPowerDelta += imp.PowerDelta;
                                totalDurationDelta += imp.DurationDelta;
                                anyModifier = true;
                                if (!imp.AffectAllMatches)
                                    break;
                            }
                        }
                    }

                    if (!anyModifier)
                    {
                        StatusEffectManager.Instance.ApplyStatusEffect(se, targets);
                    }
                    else
                    {
                        var clone = Object.Instantiate(se);
                        clone.Power = Mathf.Max(0, clone.Power + totalPowerDelta);
                        clone.Duration = Mathf.Max(0, clone.Duration + totalDurationDelta);
                        StatusEffectManager.Instance.ApplyStatusEffect(clone, targets);
                    }
                }
                break;
            }

            case SkillAugmentEffectEnum.AddStatusEffectToCaster:
            {
                var casterList = new List<Unit> { character };
                foreach (var se in StatusEffects)
                {
                    if (se == null) continue;

                    int totalPowerDelta = 0;
                    int totalDurationDelta = 0;
                    bool anyModifier = false;

                    foreach (var sa in skill.Augments)
                    {
                        if (sa?.augmentSO is ImproveStatusEffectAugment imp)
                        {
                            if (imp.StatusEffectType == se.StatusEffectType)
                            {
                                totalPowerDelta += imp.PowerDelta;
                                totalDurationDelta += imp.DurationDelta;
                                anyModifier = true;
                                if (!imp.AffectAllMatches)
                                    break;
                            }
                        }
                    }

                    if (!anyModifier)
                    {
                        StatusEffectManager.Instance.ApplyStatusEffect(se, casterList);
                    }
                    else
                    {
                        var clone = Object.Instantiate(se);
                        clone.Power = Mathf.Max(0, clone.Power + totalPowerDelta);
                        clone.Duration = Mathf.Max(0, clone.Duration + totalDurationDelta);
                        StatusEffectManager.Instance.ApplyStatusEffect(clone, casterList);
                    }
                }
                break;
            }

            case SkillAugmentEffectEnum.AddEnergy:
                //character.SetEnergy(character.Energy + Value);
                break;

            case SkillAugmentEffectEnum.ModifyEnergyCost:
                //skill.EnergyCost = Mathf.Max(0, skill.EnergyCost + Value);
                break;

            case SkillAugmentEffectEnum.ModifyRange:
                foreach (var spg in skill.SkillPartGroups)
                    foreach (var sp in spg.skillParts)
                        sp.MaxRange = Mathf.Max(0, sp.MaxRange + Value);
                break;

            case SkillAugmentEffectEnum.ModifyDamage:
                foreach (var spg in skill.SkillPartGroups)
                    foreach (var sp in spg.skillParts)
                        foreach (var dmg in sp.DamageEffects)
                            dmg.Power = Mathf.Max(0, dmg.Power + Value);
                break;

            case SkillAugmentEffectEnum.ResetSkill:
            {
                Skill skillToReset = FindSkillToReset(character);
                if (skillToReset != null)
                    SkillData.SetCharges(skillToReset, skillToReset.DefaultCharges);
                break;
            }
        }
    }

    // ?????????????????????????????????????????????????????????????????

    static List<Unit> GetAllSkillTargets(Skill skill)
    {
        var targets = new List<Unit>();
        foreach (var spg in skill.SkillPartGroups)
            foreach (var sp in spg.skillParts)
                if (sp.PartData?.TargetsHit != null)
                    targets.AddRange(sp.PartData.TargetsHit.Where(t => !targets.Contains(t)));
        return targets;
    }

    Skill FindSkillToReset(Character character)
    {
        if (SkillToReset == null)
            return character.basicAttack;

        if (character.basicAttack.mainSkillSO == SkillToReset) return character.basicAttack;
        if (character.basicSkill.mainSkillSO == SkillToReset)  return character.basicSkill;

        return character.skills.FirstOrDefault(s => s.mainSkillSO == SkillToReset);
    }
}
