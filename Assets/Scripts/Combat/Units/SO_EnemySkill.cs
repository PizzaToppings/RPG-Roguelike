using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One slot in an enemy's skill list. Holds the skill reference, targeting config,
/// selection weight, availability conditions, and optional forced follow-up.
/// </summary>
[Serializable]
public class SO_EnemySkill
{
    public string SkillName;

    [Tooltip("The actual skill (with SkillPartGroups) this entry executes.")]
    public SO_MainSkill Skill;

    // Intent display
    public IntentActionEnum IntentAction;

    [Header("Targeting")]
    [Tooltip("Legacy field kept for compatibility. Use TargetPriority instead.")]
    public TargetEnum TargetPreference;
    [Tooltip("Data-driven target selection config. Overrides TargetPreference when assigned.")]
    public SO_TargetPriority TargetPriority;

    [Header("Skill Selection")]
    [Tooltip("Relative weight for weighted-random strategy. Higher = more likely.")]
    public int Weight = 1;

    [Tooltip("All conditions must pass (AND logic) for this skill to be eligible. Empty = always eligible.")]
    public List<SO_AICondition> Conditions = new List<SO_AICondition>();

    [Tooltip("When set, the enemy is forced to use this skill on the next turn regardless of strategy or conditions.")]
    public SO_EnemySkill ForcedNextSkill;

    [Header("Multi-Target")]
    [Tooltip("Maximum number of separate targets this skill can hit. Use > 1 for multi-hit TargetUnit chains.")]
    public int MaxTargetCount = 1;
    [Tooltip("Minimum number of valid targets required. Skill still executes with fewer when 0.")]
    public int MinTargetCount = 0;

    /// <summary>Returns true when all configured conditions pass for this skill.</summary>
    public bool AreConditionsMet(EnemyAIContext ctx)
    {
        if (Conditions == null || Conditions.Count == 0) return true;
        foreach (var condition in Conditions)
            if (condition != null && !condition.Evaluate(ctx)) return false;
        return true;
    }

    /// <summary>Derives an IntentTargetEnum from the configured TargetPriority for intent display.</summary>
    public IntentTargetEnum GetIntentTarget()
    {
        if (TargetPriority == null) return IntentTargetEnum.Nearest;
        switch (TargetPriority.Priority)
        {
            case TargetPriorityKindEnum.Closest:      return IntentTargetEnum.Nearest;
            case TargetPriorityKindEnum.LowestHealth: return IntentTargetEnum.LowestHealth;
            case TargetPriorityKindEnum.Random:       return IntentTargetEnum.Random;
            default:                                  return IntentTargetEnum.Nearest;
        }
    }
}
