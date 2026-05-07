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
///   - Trinket Pool     : all SO_Trinket assets available as rewards or shop stock
///   - Scene Names      : Match the exact names in your Build Settings
/// </summary>
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] SO_CharacterRoster characterRoster;
    [SerializeField] SO_SkillPool skillPool;
    [SerializeField] SO_EncounterPool encounterPool;
    [SerializeField] List<SO_Trinket> trinketPool;

    [Header("Rest Zone")]
    [SerializeField] int restHealAmount = 20;
    [SerializeField] bool restHealIsPercentage = false;

    [Header("Shop")]
    [SerializeField] int shopSkillCount = 2;
    [SerializeField] int shopTrinketCount = 2;

    [Header("Treasure Room")]
    [SerializeField] int treasureOptionCount = 3;

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
        RunData.Party.Add(new RunDataPartyMember(character));
        SceneManager.LoadScene(skillSelectScene);
    }

    /// <summary>Called by SkillSelectCard when the player picks a skill for a specific party member.</summary>
    public void SelectSkill(SO_MainSkill skill, int partyMemberIndex)
    {
        var skillInstance = new Skill();
        skillInstance.Init(skill);
        RunData.Party[partyMemberIndex].Skills.Add(skillInstance);
        RunData.CurrentEncounter = PickRandomEncounter();
        SceneManager.LoadScene(combatScene);
    }

    /// <summary>
    /// Called by CombatManager.Win().
    /// Every 2 wins a new character joins the party (up to the 4-member cap).
    /// Otherwise the player proceeds to skill selection.
    /// </summary>
    public void OnCombatWon()
    {
        RunData.CombatWins++;
        bool partyFull         = RunData.Party.Count >= 4;
        bool shouldAddCharacter = (RunData.CombatWins % 2 == 0) && !partyFull;

        if (shouldAddCharacter)
            SceneManager.LoadScene(characterSelectScene);
        else
            SceneManager.LoadScene(skillSelectScene);
    }

    /// <summary>Called by CombatManager.Lose(). Returns to the main menu and resets the run.</summary>
    public void OnCombatLost()
    {
        RunData.Reset();
        SceneManager.LoadScene(mainMenuScene);
    }

    // -------------------------------------------------------
    // Node preparation  (call these before loading a node scene)
    // -------------------------------------------------------

    /// <summary>How much HP each party member recovers at a Rest Zone.</summary>
    public int RestHealAmount => restHealAmount;

    /// <summary>Whether <see cref="RestHealAmount"/> is a percentage of max HP.</summary>
    public bool RestHealIsPercentage => restHealIsPercentage;

    /// <summary>
    /// Randomly picks trinkets from the pool and stores them in
    /// <see cref="RunData.CurrentTreasureOptions"/> ready for the Treasure Room scene.
    /// </summary>
    public void PrepTreasureRoom()
    {
        RunData.CurrentTreasureOptions.Clear();

        if (trinketPool == null || trinketPool.Count == 0)
        {
            Debug.LogWarning("RunManager: Trinket pool is empty or not assigned.");
            return;
        }

        var picks = trinketPool
            .OrderBy(_ => Random.value)
            .Take(treasureOptionCount)
            .ToList();

        RunData.CurrentTreasureOptions.AddRange(picks);
    }

    /// <summary>
    /// Randomly picks skills and trinkets from their pools and stores them in
    /// <see cref="RunData.CurrentShopSkills"/> and <see cref="RunData.CurrentShopTrinkets"/>
    /// ready for the Shop scene.
    /// </summary>
    public void PrepShop()
    {
        RunData.CurrentShopSkills.Clear();
        RunData.CurrentShopTrinkets.Clear();

        if (skillPool != null && skillPool.Skills.Count > 0)
        {
            var alreadyOwned = new HashSet<SO_MainSkill>(
                RunData.Party.SelectMany(m => m.Skills.Select(s => s.mainSkillSO)));

            var allPartyClasses = new HashSet<ClassEnum>(
                RunData.Party.SelectMany(m => m.Character.Classes));

            var skillPicks = skillPool.Skills
                .Where(s => !alreadyOwned.Contains(s) &&
                            (s.Classes.Count == 0 || s.Classes.Any(c => allPartyClasses.Contains(c))))
                .OrderBy(_ => Random.value)
                .Take(shopSkillCount)
                .ToList();

            RunData.CurrentShopSkills.AddRange(skillPicks);
        }

        if (trinketPool != null && trinketPool.Count > 0)
        {
            var trinketPicks = trinketPool
                .OrderBy(_ => Random.value)
                .Take(shopTrinketCount)
                .ToList();

            RunData.CurrentShopTrinkets.AddRange(trinketPicks);
        }
    }

    /// <summary>
    /// Returns up to <see cref="characterOptionCount"/> randomly chosen characters from the roster,
    /// excluding characters already in the party.
    /// </summary>
    public SO_Character[] GetCharacterOptions()
    {
        if (characterRoster == null || characterRoster.Characters.Count == 0)
        {
            Debug.LogWarning("RunManager: CharacterRoster is empty or not assigned.");
            return new SO_Character[0];
        }

        var alreadyInParty = new HashSet<SO_Character>(RunData.Party.Select(m => m.Character));
        return characterRoster.Characters
            .Where(c => !alreadyInParty.Contains(c))
            .OrderBy(_ => Random.value)
            .Take(characterOptionCount)
            .ToArray();
    }

    /// <summary>
    /// Returns up to <see cref="skillOptionCount"/> skills valid for the given party member.
    /// Skills already owned by <em>any</em> party member are excluded to prevent duplicates.
    /// </summary>
    public SO_MainSkill[] GetSkillOptions(int partyMemberIndex = 0)
    {
        if (skillPool == null || skillPool.Skills.Count == 0)
        {
            Debug.LogWarning("RunManager: SkillPool is empty or not assigned.");
            return new SO_MainSkill[0];
        }

        if (partyMemberIndex >= RunData.Party.Count)
        {
            Debug.LogWarning("RunManager: Invalid party member index when requesting skill options.");
            return new SO_MainSkill[0];
        }

        var targetMember     = RunData.Party[partyMemberIndex];
        var characterClasses = targetMember.Character.Classes;

        // Exclude all skills already assigned to any party member.
        var alreadyOwned = new HashSet<SO_MainSkill>(
            RunData.Party.SelectMany(m => m.Skills.Select(s => s.mainSkillSO)));

        return skillPool.Skills
            .Where(s => !alreadyOwned.Contains(s) &&
                        (s.Classes.Count == 0 || s.Classes.Any(c => characterClasses.Contains(c))))
            .OrderBy(_ => Random.value)
            .Take(skillOptionCount)
            .ToArray();
    }

    /// <summary>
    /// Returns up to <see cref="skillOptionCount"/> skills that at least one party member can use
    /// and that are not yet owned by anyone. Used by the skill-select screen when each card
    /// shows per-character assign buttons instead of a single select button.
    /// </summary>
    public SO_MainSkill[] GetSkillOptionsForParty()
    {
        if (skillPool == null || skillPool.Skills.Count == 0)
        {
            Debug.LogWarning("RunManager: SkillPool is empty or not assigned.");
            return new SO_MainSkill[0];
        }

        if (RunData.Party.Count == 0)
        {
            Debug.LogWarning("RunManager: Party is empty when requesting skill options.");
            return new SO_MainSkill[0];
        }

        var alreadyOwned = new HashSet<SO_MainSkill>(
            RunData.Party.SelectMany(m => m.Skills.Select(s => s.mainSkillSO)));

        var allPartyClasses = new HashSet<ClassEnum>(
            RunData.Party.SelectMany(m => m.Character.Classes));

        return skillPool.Skills
            .Where(s => !alreadyOwned.Contains(s) &&
                        (s.Classes.Count == 0 || s.Classes.Any(c => allPartyClasses.Contains(c))))
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
