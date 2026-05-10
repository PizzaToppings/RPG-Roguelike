using System;
using UnityEngine;

/// <summary>
/// One slot in an enemy's skill list. Holds the skill itself, how it picks a target,
/// and the rules that control when it can be selected.
/// </summary>
[Serializable]
public class SO_EnemySkill
{
    public string SkillName;
    public SO_Skillpart Skill;

    [Header("Intent (UI display)")]
    public IntentActionEnum IntentAction;

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
