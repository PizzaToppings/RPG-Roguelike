using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages combat testing mode.
/// Place this on a GameObject in the Combat scene.
/// 
/// Execution order: This must run in Awake BEFORE EncounterManager and PartyManager,
/// so it can populate RunData with test data when starting directly in the Combat scene.
/// 
/// Set Script Execution Order in Unity to run before Default Time (e.g., -100).
/// </summary>
public class CombatTestManager : MonoBehaviour
{
    [Header("Test Configuration")]
    [Tooltip("Test configuration asset. If null, test mode is disabled.")]
    [SerializeField] SO_CombatTestConfig testConfig;

    void Awake()
    {
        // Only activate test mode if:
        // 1. A test config is assigned
        // 2. Test mode is enabled in the config
        // 3. RunData is empty (indicating we started directly in Combat scene)
        if (testConfig == null || !testConfig.EnableTestMode)
        {
            Debug.Log("[CombatTest] Test mode disabled or no config assigned.");
            return;
        }

        if (RunData.Party.Count > 0 || RunData.CurrentEncounter != null)
        {
            Debug.Log("[CombatTest] RunData already populated (normal game flow). Skipping test initialization.");
            return;
        }

        Debug.Log("[CombatTest] Initializing combat test mode...");
        InitializeTestData();
    }

    void InitializeTestData()
    {
        // Reset RunData to clean state
        RunData.Reset();

        // Setup encounter
        SetupEncounter();

        // Setup party
        SetupParty();

        // Setup starting gold
        RunData.Gold = testConfig.StartingGold;

        Debug.Log($"[CombatTest] Test initialization complete. Party size: {RunData.Party.Count}, Encounter: {RunData.CurrentEncounter?.EncounterName ?? "None"}");
    }

    void SetupEncounter()
    {
        if (testConfig.PresetEncounter != null)
        {
            RunData.CurrentEncounter = testConfig.PresetEncounter;
            Debug.Log($"[CombatTest] Using preset encounter: {testConfig.PresetEncounter.EncounterName}");
        }
        else if (testConfig.RandomizeEncounter && testConfig.EncounterPool != null && testConfig.EncounterPool.Encounters.Count > 0)
        {
            RunData.CurrentEncounter = testConfig.EncounterPool.Encounters[Random.Range(0, testConfig.EncounterPool.Encounters.Count)];
            Debug.Log($"[CombatTest] Randomized encounter: {RunData.CurrentEncounter?.EncounterName ?? "None"}");
        }
        else
        {
            Debug.LogWarning("[CombatTest] No encounter configured. Using scene default enemies.");
        }
    }

    void SetupParty()
    {
        List<SO_Character> charactersToUse = new List<SO_Character>();

        // Determine which characters to use
        if (testConfig.PresetCharacters != null && testConfig.PresetCharacters.Count > 0)
        {
            charactersToUse = testConfig.PresetCharacters.Where(c => c != null).ToList();
            Debug.Log($"[CombatTest] Using {charactersToUse.Count} preset character(s).");
        }
        else if (testConfig.RandomizeCharacters && testConfig.CharacterRoster != null && testConfig.CharacterRoster.Characters.Count > 0)
        {
            int count = Random.Range(testConfig.MinCharacterCount, testConfig.MaxCharacterCount + 1);
            charactersToUse = testConfig.CharacterRoster.Characters
                .Where(c => c != null)
                .OrderBy(_ => Random.value)
                .Take(count)
                .ToList();
            Debug.Log($"[CombatTest] Randomized {charactersToUse.Count} character(s).");
        }
        else
        {
            Debug.LogWarning("[CombatTest] No characters configured. Using scene default characters.");
            return;
        }

        // Limit to 4 characters max
        if (charactersToUse.Count > 4)
        {
            charactersToUse = charactersToUse.Take(4).ToList();
            Debug.LogWarning("[CombatTest] Limited party to 4 characters.");
        }

        // Create party members
        // Track already assigned skills and traits to avoid duplicates across the whole party
        var alreadyAssignedSkills = new HashSet<SO_MainSkill>();
        var alreadyAssignedTraits = new HashSet<SO_Trait>();

        for (int i = 0; i < charactersToUse.Count; i++)
        {
            var character = charactersToUse[i];
            var member = new RunDataPartyMember(character);

            // Setup skills
            SetupSkillsForCharacter(member, i, alreadyAssignedSkills);

            // Setup traits
            SetupTraitsForCharacter(member, i, alreadyAssignedTraits);

            // Setup starting HP
            if (testConfig.StartingHPPercentage > 0 && testConfig.StartingHPPercentage < 100)
            {
                member.CurrentHitpoints = Mathf.RoundToInt(character.MaxHealth * testConfig.StartingHPPercentage / 100f);
            }

            RunData.Party.Add(member);
        }
    }

    void SetupSkillsForCharacter(RunDataPartyMember member, int characterIndex, HashSet<SO_MainSkill> alreadyAssignedSkills)
    {
        List<SO_MainSkill> skillsToAssign = new List<SO_MainSkill>();

        if (testConfig.RandomizeSkills && testConfig.SkillPool != null && testConfig.SkillPool.Skills.Count > 0)
        {
            // Randomize skills
            int skillCount = Random.Range(testConfig.MinSkillsPerCharacter, testConfig.MaxSkillsPerCharacter + 1);
            
            // Filter skills by character class if the character has classes
            var availableSkills = testConfig.SkillPool.Skills.Where(s => s != null).ToList();
            
            if (member.Character.Classes != null && member.Character.Classes.Count > 0)
            {
                availableSkills = availableSkills
                    .Where(s => s.Classes == null || s.Classes.Count == 0 || s.Classes.Any(c => member.Character.Classes.Contains(c)))
                    .ToList();
            }

            // Exclude skills already assigned to other party members
            if (alreadyAssignedSkills != null && alreadyAssignedSkills.Count > 0)
            {
                availableSkills = availableSkills.Where(s => !alreadyAssignedSkills.Contains(s)).ToList();
            }

            skillsToAssign = availableSkills
                .OrderBy(_ => Random.value)
                .Take(skillCount)
                .ToList();

            Debug.Log($"[CombatTest] Character {characterIndex} ({member.Character.Name}): Randomized {skillsToAssign.Count} skill(s).");
        }
        else if (testConfig.PresetSkills != null && characterIndex < testConfig.PresetSkills.Count)
        {
            // Use preset skills
            var preset = testConfig.PresetSkills[characterIndex];
            if (preset != null && preset.Skills != null)
            {
                // Respect class eligibility and avoid duplicates across party
                skillsToAssign = preset.Skills
                    .Where(s => s != null)
                    .Where(s => (s.Classes == null || s.Classes.Count == 0 || member.Character.Classes == null || member.Character.Classes.Count == 0 || s.Classes.Any(c => member.Character.Classes.Contains(c))))
                    .Where(s => alreadyAssignedSkills == null || !alreadyAssignedSkills.Contains(s))
                    .ToList();
                Debug.Log($"[CombatTest] Character {characterIndex} ({member.Character.Name}): Using {skillsToAssign.Count} preset skill(s).");
            }
        }

        // Limit to 4 skills max
        if (skillsToAssign.Count > 4)
        {
            skillsToAssign = skillsToAssign.Take(4).ToList();
        }

        // Convert SO_MainSkill to Skill instances
        foreach (var skillSO in skillsToAssign)
        {
            var skill = new Skill();
            skill.Init(skillSO);
            member.Skills.Add(skill);
            if (alreadyAssignedSkills != null && skillSO != null)
                alreadyAssignedSkills.Add(skillSO);
        }
    }

    void SetupTraitsForCharacter(RunDataPartyMember member, int characterIndex, HashSet<SO_Trait> alreadyAssignedTraits)
    {
        List<SO_Trait> traitsToAssign = new List<SO_Trait>();

        if (testConfig.RandomizeTraits && testConfig.TraitPool != null && testConfig.TraitPool.Traits.Count > 0)
        {
            // Randomize traits
            int traitCount = Random.Range(testConfig.MinTraitsPerCharacter, testConfig.MaxTraitsPerCharacter + 1);
            
            // Filter traits by character class if applicable
            var availableTraits = testConfig.TraitPool.Traits.Where(t => t != null).ToList();
            
            if (member.Character.Classes != null && member.Character.Classes.Count > 0)
            {
                availableTraits = availableTraits
                    .Where(t => t.classes == null || t.classes.Count == 0 || t.classes.Any(c => member.Character.Classes.Contains(c)))
                    .ToList();
            }

            // Exclude traits already assigned to other party members
            if (alreadyAssignedTraits != null && alreadyAssignedTraits.Count > 0)
            {
                availableTraits = availableTraits.Where(t => !alreadyAssignedTraits.Contains(t)).ToList();
            }

            traitsToAssign = availableTraits
                .OrderBy(_ => Random.value)
                .Take(traitCount)
                .ToList();

            Debug.Log($"[CombatTest] Character {characterIndex} ({member.Character.Name}): Randomized {traitsToAssign.Count} trait(s).");
        }
        else if (testConfig.PresetTraits != null && characterIndex < testConfig.PresetTraits.Count)
        {
            // Use preset traits
            var preset = testConfig.PresetTraits[characterIndex];
            if (preset != null && preset.Traits != null)
            {
                // Respect class restrictions and avoid duplicates across party
                traitsToAssign = preset.Traits
                    .Where(t => t != null)
                    .Where(t => t.classes == null || t.classes.Count == 0 || member.Character.Classes == null || member.Character.Classes.Count == 0 || t.classes.Any(c => member.Character.Classes.Contains(c)))
                    .Where(t => alreadyAssignedTraits == null || !alreadyAssignedTraits.Contains(t))
                    .ToList();
                Debug.Log($"[CombatTest] Character {characterIndex} ({member.Character.Name}): Using {traitsToAssign.Count} preset trait(s).");
            }
        }

        // Add traits to member
        member.Traits.AddRange(traitsToAssign);
        if (alreadyAssignedTraits != null)
        {
            foreach (var t in traitsToAssign)
                if (t != null)
                    alreadyAssignedTraits.Add(t);
        }
    }
}
