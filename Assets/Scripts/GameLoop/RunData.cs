using System.Collections.Generic;

/// <summary>
/// Static store for the current run's state. Persists between scene loads.
/// Reset at the start of each new run.
/// </summary>
public static class RunData
{
    /// <summary>The character the player chose at the start of the run.</summary>
    public static SO_Character SelectedCharacter;

    /// <summary>All special skills acquired so far this run (one added after each combat victory).</summary>
    public static List<Skill> AcquiredSkills = new List<Skill>();

    /// <summary>The encounter that should be loaded in the next combat scene.</summary>
    public static SO_Encounter CurrentEncounter;

    /// <summary>Call this at the start of each new run.</summary>
    public static void Reset()
    {
        SelectedCharacter = null;
        AcquiredSkills.Clear();
        CurrentEncounter = null;
    }
}
