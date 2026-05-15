using System.Collections.Generic;

/// <summary>
/// Represents a single member of the player's party during a run.
/// Each member has their own character and individually assigned skills.
/// </summary>
public class RunDataPartyMember
{
    public SO_Character Character;
    public List<Skill> Skills = new List<Skill>();
    public List<SO_Trinket> Trinkets = new List<SO_Trinket>();

    /// <summary>Augments assigned per skill SO.</summary>
    public Dictionary<SO_MainSkill, List<SO_SkillAugment>> SkillAugments = new Dictionary<SO_MainSkill, List<SO_SkillAugment>>();

    /// <summary>HP carried over from the last combat. 0 means unset (use MaxHealth).</summary>
    public int CurrentHitpoints = 0;

    /// <summary>Permanent MaxHitpoints bonus accumulated from Instant trinkets this run.</summary>
    public int BonusMaxHitpoints = 0;

    /// <summary>Permanent MaxEnergy bonus accumulated from Instant trinkets this run.</summary>
    public int BonusMaxEnergy = 0;

    /// <summary>Trinket names whose Instant effect has already been applied this run (prevents re-apply each combat).</summary>
    public List<string> AppliedInstantTrinkets = new List<string>();

    public RunDataPartyMember(SO_Character character)
    {
        Character = character;
    }
}

/// <summary>
/// Static store for the current run's state. Persists between scene loads.
/// Reset at the start of each new run.
/// </summary>
public static class RunData
{
    /// <summary>
    /// All party members acquired so far this run. Max 4.
    /// Party[0] is the character chosen at the start of the run.
    /// </summary>
    public static List<RunDataPartyMember> Party = new List<RunDataPartyMember>();

    /// <summary>Number of combat encounters won so far this run.</summary>
    public static int CombatWins = 0;

    /// <summary>Index into RunManager's progression sequence. Advances after each step completes.</summary>
    public static int StepIndex = 0;

    /// <summary>The type of the next node the player will enter.</summary>
    public static NodeTypeEnum CurrentNodeType;

    /// <summary>The encounter that should be loaded in the next combat scene (Combat, EliteCombat, or Boss).</summary>
    public static SO_Encounter CurrentEncounter;

    /// <summary>The event to present at the next Event node.</summary>
    public static SO_Event CurrentEvent;

    /// <summary>The trinkets randomly selected for the current Treasure Room.</summary>
    public static List<SO_Trinket> CurrentTreasureOptions = new List<SO_Trinket>();

    /// <summary>The skills randomly selected for the current Shop.</summary>
    public static List<SO_MainSkill> CurrentShopSkills = new List<SO_MainSkill>();

    /// <summary>The trinkets randomly selected for the current Shop.</summary>
    public static List<SO_Trinket> CurrentShopTrinkets = new List<SO_Trinket>();

    /// <summary>The skill augments randomly selected for the current Shop.</summary>
    public static List<SO_SkillAugment> CurrentShopSkillAugments = new List<SO_SkillAugment>();

    /// <summary>How much gold the party has accumulated this run.</summary>
    public static int Gold;

    /// <summary>Convenience accessor – the character chosen at the start of the run.</summary>
    public static SO_Character SelectedCharacter => Party.Count > 0 ? Party[0].Character : null;

    /// <summary>Call this at the start of each new run.</summary>
    public static void Reset()
    {
        Party.Clear();
        CombatWins = 0;
        StepIndex = 0;
        CurrentNodeType = NodeTypeEnum.Combat;
        CurrentEncounter = null;
        CurrentEvent = null;
        CurrentTreasureOptions.Clear();
        CurrentShopSkills.Clear();
        CurrentShopTrinkets.Clear();
        CurrentShopSkillAugments.Clear();
        Gold = 0;
    }
}
