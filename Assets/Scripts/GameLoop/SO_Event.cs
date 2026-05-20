using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for an Event node. Presents the player with a written story and several choices,
/// each with its own reward or consequence.
/// Create via: right-click in Project window > ScriptableObjects > GameLoop > Event
/// </summary>
[CreateAssetMenu(fileName = "Event", menuName = "ScriptableObjects/GameLoop/Event")]
public class SO_Event : ScriptableObject
{
    public string EventName;

    [TextArea(5, 12)]
    [Tooltip("The story text shown to the player.")]
    public string StoryText;

    [Tooltip("The choices the player can make. Each choice has its own reward or consequence.")]
    public List<EventOption> Options = new List<EventOption>();
}

[Serializable]
public class EventOption
{
    [Tooltip("Button label shown to the player.")]
    public string OptionText;

    [TextArea(2, 5)]
    [Tooltip("Flavour text shown after the player picks this option.")]
    public string OutcomeText;

    public EventRewardType RewardType;

    [Tooltip("Used when RewardType is HealParty. Flat HP restored to each member.")]
    public int HealAmount;

    [Tooltip("If true, HealAmount is a percentage of max HP.")]
    public bool HealIsPercentage;

    [Tooltip("Used when RewardType is GainGold.")]
    public int GoldAmount;

    [Tooltip("Used when RewardType is GainTrait.")]
    public SO_Trait TraitReward;

    [Tooltip("Used when RewardType is GainSkill.")]
    public SO_MainSkill SkillReward;

    [Tooltip("Used when RewardType is LoseHP. Flat damage dealt to each party member.")]
    public int DamageAmount;

    [Tooltip("If true, DamageAmount is a percentage of max HP.")]
    public bool DamageIsPercentage;
}

public enum EventRewardType
{
    None,
    HealParty,
    GainGold,
    GainTrait,
    GainSkill,
    LoseHP
}
