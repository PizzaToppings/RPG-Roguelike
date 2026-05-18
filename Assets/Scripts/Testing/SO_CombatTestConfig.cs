using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration for testing the combat scene directly without going through the full game loop.
/// Allows presetting or randomizing encounters, characters, skills, and trinkets.
/// </summary>
[CreateAssetMenu(fileName = "CombatTestConfig", menuName = "ScriptableObjects/Testing/CombatTestConfig")]
public class SO_CombatTestConfig : ScriptableObject
{
    [Header("Test Mode")]
    [Tooltip("Enable test mode. When true, this config will populate RunData when starting directly in Combat scene.")]
    public bool EnableTestMode = true;

    [Header("Encounter Configuration")]
    [Tooltip("If set, this specific encounter will be used. If null and RandomizeEncounter is true, one will be picked from the pool.")]
    public SO_Encounter PresetEncounter;

    [Tooltip("If true and PresetEncounter is null, randomly select an encounter from the pool below.")]
    public bool RandomizeEncounter = false;

    [Tooltip("Pool of encounters to randomly select from when RandomizeEncounter is true.")]
    public SO_EncounterPool EncounterPool;

    [Header("Character Configuration")]
    [Tooltip("Preset characters to use for testing. Leave empty to randomize from the roster.")]
    public List<SO_Character> PresetCharacters = new List<SO_Character>();

    [Tooltip("If true and PresetCharacters is empty, randomly select characters from the roster.")]
    public bool RandomizeCharacters = false;

    [Tooltip("Minimum number of characters when randomizing (1-4).")]
    [Range(1, 4)]
    public int MinCharacterCount = 1;

    [Tooltip("Maximum number of characters when randomizing (1-4).")]
    [Range(1, 4)]
    public int MaxCharacterCount = 4;

    [Tooltip("Pool of characters to randomly select from when RandomizeCharacters is true.")]
    public SO_CharacterRoster CharacterRoster;

    [Header("Skills Configuration")]
    [Tooltip("If true, each character will be assigned random skills from the skill pool.")]
    public bool RandomizeSkills = false;

    [Tooltip("Minimum number of skills per character when randomizing (0-4).")]
    [Range(0, 4)]
    public int MinSkillsPerCharacter = 2;

    [Tooltip("Maximum number of skills per character when randomizing (0-4).")]
    [Range(0, 4)]
    public int MaxSkillsPerCharacter = 4;

    [Tooltip("Pool of skills to randomly select from when RandomizeSkills is true.")]
    public SO_SkillPool SkillPool;

    [Tooltip("Preset skills per character. Use when not randomizing. Index matches character index.")]
    public List<CharacterSkillPreset> PresetSkills = new List<CharacterSkillPreset>();

    [Header("Trinkets Configuration")]
    [Tooltip("If true, each character will be assigned random trinkets from the trinket pool.")]
    public bool RandomizeTrinkets = false;

    [Tooltip("Minimum number of trinkets per character when randomizing (0-4).")]
    [Range(0, 4)]
    public int MinTrinketsPerCharacter = 0;

    [Tooltip("Maximum number of trinkets per character when randomizing (0-4).")]
    [Range(0, 4)]
    public int MaxTrinketsPerCharacter = 2;

    [Tooltip("Pool of trinkets to randomly select from when RandomizeTrinkets is true.")]
    public SO_TrinketPool TrinketPool;

    [Tooltip("Preset trinkets per character. Use when not randomizing. Index matches character index.")]
    public List<CharacterTrinketPreset> PresetTrinkets = new List<CharacterTrinketPreset>();

    [Header("Starting Conditions")]
    [Tooltip("Starting HP percentage for all characters (0-100). 0 means use default MaxHealth.")]
    [Range(0, 100)]
    public int StartingHPPercentage = 100;

    [Tooltip("Starting gold amount for testing shop interactions.")]
    public int StartingGold = 0;
}

[System.Serializable]
public class CharacterSkillPreset
{
    [Tooltip("Skills to assign to this character (max 4).")]
    public List<SO_MainSkill> Skills = new List<SO_MainSkill>();
}

[System.Serializable]
public class CharacterTrinketPreset
{
    [Tooltip("Trinkets to assign to this character.")]
    public List<SO_Trinket> Trinkets = new List<SO_Trinket>();
}
