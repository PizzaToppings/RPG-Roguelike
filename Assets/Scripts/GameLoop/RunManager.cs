using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent singleton that drives the roguelike gameplay loop across scenes.
/// Place this on a GameObject in MainMenuScene - it will persist via DontDestroyOnLoad.
///
/// Setup in Inspector:
///   - Character Roster : SO_CharacterRoster asset containing all SO_Character assets
///   - Skill Pool       : SO_SkillPool asset containing all selectable special skills
///   - Encounter Pool   : SO_EncounterPool asset containing all SO_Encounter assets
///   - Scene Names      : Match the exact names in your Build Settings
/// </summary>
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] SO_CharacterRoster characterRoster;
    [SerializeField] SO_SkillPool skillPool;
    [SerializeField] SO_EncounterPool encounterPool;

    [Header("Scene Names")]
    [SerializeField] string characterSelectScene = "CharacterSelectScene";
    [SerializeField] string skillSelectScene     = "SkillSelectScene";
    [SerializeField] string combatScene          = "TestCombatScene";
    [SerializeField] string mainMenuScene        = "MainMenuScene";

    [Header("Selection Pool Sizes")]
    [SerializeField] int characterOptionCount = 3;
    [SerializeField] int skillOptionCount     = 3;

    // -------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -------------------------------------------------------
    // Run flow
    // -------------------------------------------------------

    /// <summary>Reset run state and go to character selection.</summary>
    public void StartNewRun()
    {
        RunData.Reset();
        SceneManager.LoadScene(characterSelectScene);
    }

    /// <summary>Called by CharacterSelectCard when the player picks a character.</summary>
    public void SelectCharacter(SO_Character character)
    {
        RunData.SelectedCharacter = character;
        SceneManager.LoadScene(skillSelectScene);
    }

    /// <summary>Called by SkillSelectCard when the player picks a skill.</summary>
    public void SelectSkill(SO_MainSkill skill)
    {
        var skillInstance = new Skill();
        skillInstance.Init(skill);
        RunData.AcquiredSkills.Add(skillInstance);
        RunData.CurrentEncounter = PickRandomEncounter();
        SceneManager.LoadScene(combatScene);
    }

    /// <summary>Called by CombatManager.Win(). Proceeds to skill selection for the next combat.</summary>
    public void OnCombatWon()
    {
        SceneManager.LoadScene(skillSelectScene);
    }

    /// <summary>Called by CombatManager.Lose(). Returns to the main menu and resets the run.</summary>
    public void OnCombatLost()
    {
        RunData.Reset();
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Returns up to <see cref="characterOptionCount"/> randomly chosen characters from the roster.
    /// </summary>
    public SO_Character[] GetCharacterOptions()
    {
        if (characterRoster == null || characterRoster.Characters.Count == 0)
        {
            Debug.LogWarning("RunManager: CharacterRoster is empty or not assigned.");
            return new SO_Character[0];
        }

        return characterRoster.Characters
            .OrderBy(_ => Random.value)
            .Take(characterOptionCount)
            .ToArray();
    }

    public SO_MainSkill[] GetSkillOptions()
    {
        if (skillPool == null || skillPool.Skills.Count == 0)
        {
            Debug.LogWarning("RunManager: SkillPool is empty or not assigned.");
            return new SO_MainSkill[0];
        }

        if (RunData.SelectedCharacter == null)
        {
            Debug.LogWarning("RunManager: No character selected when requesting skill options.");
            return new SO_MainSkill[0];
        }

        var characterClasses  = RunData.SelectedCharacter.Classes;
        var alreadyOwned      = new HashSet<SO_MainSkill>(RunData.AcquiredSkills.Select(s => s.mainSkillSO));

        return skillPool.Skills
            .Where(s => !alreadyOwned.Contains(s) &&
                        (s.Classes.Count == 0 || s.Classes.Any(c => characterClasses.Contains(c))))
            .OrderBy(_ => Random.value)
            .Take(skillOptionCount)
            .ToArray();
    }

    SO_Encounter PickRandomEncounter()
    {
        if (encounterPool == null || encounterPool.Encounters.Count == 0)
        {
            Debug.LogWarning("RunManager: EncounterPool is empty or not assigned.");
            return null;
        }

        var encounter = encounterPool.Encounters[Random.Range(0, encounterPool.Encounters.Count)];
        return encounter;
    }
}
