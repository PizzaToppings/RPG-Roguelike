using System.Collections.Generic;

/// <summary>
/// Runtime state for a single enemy's AI. Lives on EnemyBaseAI.
/// Tracks turn count, skill usage history, rotation/sequence indices, and forced follow-up skills.
/// </summary>
public class EnemyAIContext
{
    public EnemyBaseAI Enemy;
    public int TurnCount;
    public SO_EnemySkill LastUsedSkill;
    public SO_EnemySkill ForcedNextSkill;
    public int RotationIndex;
    public int SequenceIndex;

    private readonly Dictionary<SO_EnemySkill, int> _lastUsedTurnBySkill = new Dictionary<SO_EnemySkill, int>();

    public void RecordSkillUsed(SO_EnemySkill skill, int currentTurn)
    {
        if (skill == null) return;
        LastUsedSkill = skill;
        _lastUsedTurnBySkill[skill] = currentTurn;
    }

    /// <summary>Returns the number of turns since the given skill was last used, or int.MaxValue if never used.</summary>
    public int TurnsSinceUsed(SO_EnemySkill skill)
    {
        if (skill == null) return int.MaxValue;
        return _lastUsedTurnBySkill.TryGetValue(skill, out int turn)
            ? TurnCount - turn
            : int.MaxValue;
    }
}
