using UnityEngine;

/// <summary>
/// True when the last skill used by this enemy was the specified skill.
/// Use this to chain skills: e.g. "Wind Up was the previous skill → allow Massive Slam".
/// </summary>
[CreateAssetMenu(fileName = "PreviousSkillCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/PreviousSkillCondition")]
public class SO_PreviousSkillCondition : SO_AICondition
{
    public SO_EnemySkill RequiredPreviousSkill;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        return ctx.LastUsedSkill == RequiredPreviousSkill;
    }
}
