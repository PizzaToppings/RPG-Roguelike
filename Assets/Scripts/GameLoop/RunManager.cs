using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent singleton that drives the roguelike gameplay loop across scenes.
/// Place this on a GameObject in MainMenuScene - it will persist via DontDestroyOnLoad.
///
/// Setup in Inspector:
///   - Character Roster      : SO_CharacterRoster asset containing all SO_Character assets
///   - Skill Pool            : SO_SkillPool asset containing all selectable special skills
///   - Encounter Pool        : SO_EncounterPool asset for normal combats
///   - Elite Encounter Pool  : SO_EncounterPool asset for elite combats
///   - Boss Encounter Pool   : SO_EncounterPool asset for boss fights
///   - Trait Pool          : SO_TraitPool asset for trait rewards
///   - Progression           : Ordered list of ProgressionStep values defining the full run
///   - Scene Names           : Match the exact names in your Build Settings
/// </summary>
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] SO_CharacterRoster characterRoster;
    [SerializeField] SO_SkillPool skillPool;
    [SerializeField] SO_EncounterPool encounterPool;
    [SerializeField] SO_EncounterPool eliteEncounterPool;
    [SerializeField] SO_EncounterPool bossEncounterPool;
    [SerializeField] SO_TraitPool traitPool;

    [Header("Progression")]
    [SerializeField] ProgressionStep[] progression = new ProgressionStep[]
    {
        ProgressionStep.SelectCharacter,
        ProgressionStep.SelectSkill,
        ProgressionStep.Combat,
        ProgressionStep.SelectSkill,
        ProgressionStep.SelectTrait,
        ProgressionStep.Combat,
        ProgressionStep.SelectCharacter,
        ProgressionStep.SelectSkill,
        ProgressionStep.Combat,
        ProgressionStep.SelectSkill,
        ProgressionStep.SelectTrait,
        ProgressionStep.Combat,
        ProgressionStep.SelectSkill,
        ProgressionStep.RestZone,
        ProgressionStep.Boss,
    };

    [Header("Rest Zone")]
    [SerializeField] int restHealAmount = 20;
    [SerializeField] bool restHealIsPercentage = false;

    [Header("Shop")]
    [SerializeField] int shopSkillCount = 2;
    [SerializeField] int shopTraitCount = 2;
    [SerializeField] int shopSkillAugmentCount = 2;

    [Header("Treasure Room")]
    [SerializeField] int treasureOptionCount = 3;

    [Header("Scene Names")]
    [SerializeField] string characterSelectScene = "CharacterSelectScene";
    [SerializeField] string skillSelectScene     = "SkillSelectScene";
    [SerializeField] string traitSelectScene    = "TraitSelectScene";
    [SerializeField] string combatScene          = "2DTestCombatScene";
    [SerializeField] string restZoneScene        = "RestZoneScene";
    [SerializeField] string shopScene            = "ShopScene";
    [SerializeField] string eventScene           = "EventScene";
    [SerializeField] string treasureRoomScene    = "TreasureRoomScene";
    [SerializeField] string mainMenuScene        = "MainMenuScene";

    [Header("Selection Pool Sizes")]
    [SerializeField] int characterOptionCount = 3;
    [SerializeField] int skillOptionCount     = 3;
    [SerializeField] int traitOptionCount   = 2;

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

    /// <summary>Reset run state and begin at step 0 of the progression.</summary>
    public void StartNewRun()
    {
        RunData.Reset();
        LoadCurrentStep();
    }

    /// <summary>Called by CharacterSelectCard when the player picks a character.</summary>
    public void SelectCharacter(SO_Character character)
    {
        RunData.Party.Add(new RunDataPartyMember(character));
        AdvanceStep();
    }

    /// <summary>Called by SkillSelectCard when the player picks a skill for a specific party member.</summary>
    public void SelectSkill(SO_MainSkill skill, int partyMemberIndex)
    {
        var skillInstance = new Skill();
        skillInstance.Init(skill);
        RunData.Party[partyMemberIndex].Skills.Add(skillInstance);
        AdvanceStep();
    }

    /// <summary>Called by TraitSelectAssignButton when the player assigns a trait to a party member.</summary>
    public void SelectTrait(SO_Trait trait, int partyMemberIndex)
    {
        RunData.Party[partyMemberIndex].Traits.Add(trait);
        AdvanceStep();
    }

    /// <summary>Called by CombatManager when the player wins a combat.</summary>
    public void OnCombatWon()
    {
        RunData.CombatWins++;
        AdvanceStep();
    }

    /// <summary>Called by RestZoneUI when the player leaves the rest zone.</summary>
    public void OnRestZoneLeft()
    {
        AdvanceStep();
    }

    /// <summary>Called by CombatManager when the player loses. Returns to the main menu.</summary>
    public void OnCombatLost()
    {
        RunData.Reset();
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>Increments the step index and loads the next step's scene.</summary>
    void AdvanceStep()
    {
        RunData.StepIndex++;
        LoadCurrentStep();
    }

    /// <summary>Reads the current step from the progression and loads the appropriate scene.</summary>
    void LoadCurrentStep()
    {
        if (RunData.StepIndex >= progression.Length)
        {
            Debug.Log("RunManager: Progression complete.");
            RunData.Reset();
            SceneManager.LoadScene(mainMenuScene);
            return;
        }

        var step = progression[RunData.StepIndex];
        switch (step)
        {
            case ProgressionStep.SelectCharacter:
                SceneManager.LoadScene(characterSelectScene);
                break;

            case ProgressionStep.SelectSkill:
                SceneManager.LoadScene(skillSelectScene);
                break;

            case ProgressionStep.SelectTrait:
                SceneManager.LoadScene(traitSelectScene);
                break;

            case ProgressionStep.Combat:
                RunData.CurrentEncounter = PickEncounter(encounterPool);
                RunData.CurrentNodeType  = NodeTypeEnum.Combat;
                SceneManager.LoadScene(combatScene);
                break;

            case ProgressionStep.EliteCombat:
                RunData.CurrentEncounter = PickEncounter(eliteEncounterPool);
                RunData.CurrentNodeType  = NodeTypeEnum.EliteCombat;
                SceneManager.LoadScene(combatScene);
                break;

            case ProgressionStep.Boss:
                RunData.CurrentEncounter = PickEncounter(bossEncounterPool);
                RunData.CurrentNodeType  = NodeTypeEnum.Boss;
                SceneManager.LoadScene(combatScene);
                break;

            case ProgressionStep.RestZone:
                RunData.CurrentNodeType = NodeTypeEnum.RestZone;
                SceneManager.LoadScene(restZoneScene);
                break;

            case ProgressionStep.Shop:
                PrepShop();
                RunData.CurrentNodeType = NodeTypeEnum.Shop;
                SceneManager.LoadScene(shopScene);
                break;

            case ProgressionStep.Event:
                RunData.CurrentNodeType = NodeTypeEnum.Event;
                SceneManager.LoadScene(eventScene);
                break;

            case ProgressionStep.TreasureRoom:
                PrepTreasureRoom();
                RunData.CurrentNodeType = NodeTypeEnum.TreasureRoom;
                SceneManager.LoadScene(treasureRoomScene);
                break;
        }
    }

    // -------------------------------------------------------
    // Node preparation  (call these before loading a node scene)
    // -------------------------------------------------------

    /// <summary>How much HP each party member recovers at a Rest Zone.</summary>
    public int RestHealAmount => restHealAmount;

    /// <summary>Whether <see cref="RestHealAmount"/> is a percentage of max HP.</summary>
    public bool RestHealIsPercentage => restHealIsPercentage;

    /// <summary>
    /// Randomly picks traits from the pool and stores them in
    /// <see cref="RunData.CurrentTreasureOptions"/> ready for the Treasure Room scene.
    /// </summary>
    public void PrepTreasureRoom()
    {
        RunData.CurrentTreasureOptions.Clear();

        if (traitPool == null || traitPool.Traits.Count == 0)
        {
            Debug.LogWarning("RunManager: Trait pool is empty or not assigned.");
            return;
        }

        var picks = traitPool.Traits
            .OrderBy(_ => Random.value)
            .Take(treasureOptionCount)
            .ToList();

        RunData.CurrentTreasureOptions.AddRange(picks);
    }

    /// <summary>
    /// Randomly picks skills and traits from their pools and stores them in
    /// <see cref="RunData.CurrentShopSkills"/> and <see cref="RunData.CurrentShopTraits"/>
    /// ready for the Shop scene.
    /// </summary>
    public void PrepShop()
    {
        RunData.CurrentShopSkills.Clear();
        RunData.CurrentShopTraits.Clear();
        RunData.CurrentShopSkillAugments.Clear();
        RunData.CurrentShopSkillAugmentOffers.Clear();

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

        if (traitPool != null && traitPool.Traits.Count > 0)
        {
            var traitPicks = traitPool.Traits
                .OrderBy(_ => Random.value)
                .Take(shopTraitCount)
                .ToList();

            RunData.CurrentShopTraits.AddRange(traitPicks);
        }

        PrepShopSkillAugments();
    }

    void PrepShopSkillAugments()
    {
        var ownedSkills = GetAllOwnedSkillSOs();
        if (ownedSkills.Count == 0 || shopSkillAugmentCount <= 0)
            return;

        var candidateOffers = new List<ShopSkillAugmentOffer>();

        foreach (var skillSO in ownedSkills)
        {
            if (skillSO == null || skillSO.AvailableAugments == null || skillSO.AvailableAugments.Count == 0)
                continue;

            var alreadyOwnedAugments = new HashSet<SO_SkillAugment>(
                RunData.Party
                    .Where(m => m.SkillAugments.TryGetValue(skillSO, out _))
                    .SelectMany(m => m.SkillAugments[skillSO])
                    .Where(a => a != null));

            foreach (var augmentSO in skillSO.AvailableAugments)
            {
                if (augmentSO == null || alreadyOwnedAugments.Contains(augmentSO))
                    continue;

                candidateOffers.Add(new ShopSkillAugmentOffer(skillSO, augmentSO));
            }
        }

        var picks = candidateOffers
            .OrderBy(_ => Random.value)
            .Take(shopSkillAugmentCount)
            .ToList();

        RunData.CurrentShopSkillAugmentOffers.AddRange(picks);
        RunData.CurrentShopSkillAugments.AddRange(picks.Select(x => x.Augment));
    }

    HashSet<SO_MainSkill> GetAllOwnedSkillSOs()
    {
        var owned = new HashSet<SO_MainSkill>();

        foreach (var member in RunData.Party)
        {
            if (member?.Character?.basicAttack != null)
                owned.Add(member.Character.basicAttack);

            if (member?.Character?.basicSkill != null)
                owned.Add(member.Character.basicSkill);

            foreach (var skill in member.Skills)
            {
                if (skill?.mainSkillSO != null)
                    owned.Add(skill.mainSkillSO);
            }
        }

        return owned;
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

    /// <summary>
    /// Returns up to <see cref="traitOptionCount"/> traits from the pool.
    /// Traits already owned by any party member are excluded.
    /// </summary>
    public SO_Trait[] GetTraitOptions()
    {
        if (traitPool == null || traitPool.Traits.Count == 0)
        {
            Debug.LogWarning("RunManager: TraitPool is empty or not assigned.");
            return new SO_Trait[0];
        }

        var alreadyOwned = new HashSet<SO_Trait>(
            RunData.Party.SelectMany(m => m.Traits));

        return traitPool.Traits
            .Where(t => !alreadyOwned.Contains(t))
            .OrderBy(_ => Random.value)
            .Take(traitOptionCount)
            .ToArray();
    }

    SO_Encounter PickEncounter(SO_EncounterPool pool)
    {
        if (pool == null || pool.Encounters.Count == 0)
        {
            Debug.LogWarning("RunManager: EncounterPool is empty or not assigned.");
            return null;
        }

        return pool.Encounters[Random.Range(0, pool.Encounters.Count)];
    }
}
