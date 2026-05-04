using System.Collections.Generic;

/// <summary>
/// Represents a single member of the player's party during a run.
/// Each member has their own character and individually assigned skills.
/// </summary>
public class RunDataPartyMember
{
    public SO_Character Character;
    public List<Skill> Skills = new List<Skill>();

    /// <summary>HP carried over from the last combat. 0 means unset (use MaxHealth).</summary>
    public int CurrentHitpoints = 0;

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

    /// <summary>The encounter that should be loaded in the next combat scene.</summary>
    public static SO_Encounter CurrentEncounter;

    /// <summary>Convenience accessor – the character chosen at the start of the run.</summary>
    public static SO_Character SelectedCharacter => Party.Count > 0 ? Party[0].Character : null;

    /// <summary>Call this at the start of each new run.</summary>
    public static void Reset()
    {
        Party.Clear();
        CombatWins = 0;
        CurrentEncounter = null;
    }
}
