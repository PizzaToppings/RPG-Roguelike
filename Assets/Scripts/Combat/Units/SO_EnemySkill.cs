using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One slot in an enemy's skill list. Holds the skill itself, how it picks a target,
/// and the rules that control when it can be selected.
/// </summary>
[Serializable]
public class SO_EnemySkill
{
    public string SkillName;
    public List<SO_Skillpart> Skill;

    /// <summary>Returns the first skill part, or null if the list is empty. Used for range calculations shared across all parts.</summary>
    public SO_Skillpart FirstPart => Skill != null && Skill.Count > 0 ? Skill[0] : null;

    [Header("Intent (UI display)")]
    public IntentActionEnum IntentAction;

    [Header("Combat Style")]
    [Tooltip("The combat style this skill represents. Overwrites the enemy's style when selected. Use None to keep the enemy's current style.")]
    public CombatStyle CombatStyle = CombatStyle.None;

    [Header("Targeting")]
    public TargetEnum    TargetPreference;
    public float         OptimalRange = 1f;

    /// <summary>Derives the intent target icon from the targeting rule, so both don't need to be set manually.</summary>
    public IntentTargetEnum GetIntentTarget()
    {
        switch (TargetPreference)
        {
            case TargetEnum.Self:               return IntentTargetEnum.Self;
            case TargetEnum.closestTarget:      return IntentTargetEnum.Nearest;
            case TargetEnum.LowestHealthTarget: return IntentTargetEnum.LowestHealth;
            case TargetEnum.AllAllies:
            case TargetEnum.AllEnemies:
            case TargetEnum.AllUnits:           return IntentTargetEnum.Area;
            default:                            return IntentTargetEnum.Unknown;
        }
    }

    [Header("Selection")]
    [Range(0, 100)]
    public float Chance = 100f;

    /// <summary>How many times in a row this skill may be selected. 0 = unlimited.</summary>
    public int MaxConsecutiveUses = 0;

    /// <summary>
    /// Index into SO_Enemy.Skills. When >= 0, the sequencer will always pick that
    /// skill next turn after this one (e.g. charge-up → heavy attack).
    /// -1 means no forced follow-up.
    /// </summary>
    public int ForcedNextSkillIndex = -1;
}
