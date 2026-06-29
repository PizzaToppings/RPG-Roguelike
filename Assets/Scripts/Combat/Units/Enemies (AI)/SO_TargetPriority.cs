using UnityEngine;

/// <summary>
/// Configures how an enemy picks its target(s) for a skill.
/// Assigned per SO_EnemySkill so different skills can prioritize differently.
/// </summary>
[CreateAssetMenu(fileName = "TargetPriority", menuName = "ScriptableObjects/EnemyAI/TargetPriority")]
public class SO_TargetPriority : ScriptableObject
{
    [Tooltip("How targets are sorted/selected.")]
    public TargetPriorityKindEnum Priority = TargetPriorityKindEnum.Closest;

    [Tooltip("Skip targets that already have a buff active.")]
    public bool AvoidTargetsWithBuff = false;

    [Tooltip("Skip targets that already have a debuff active.")]
    public bool AvoidTargetsWithDebuff = false;

    [Tooltip("Allow this skill to target the enemy itself (only relevant for self-origin AoE).")]
    public bool AllowTargetingSelf = false;

    [Tooltip("Minimum number of valid targets required. If fewer exist the skill still executes (just with fewer targets).")]
    public int MinValidTargets = 0;
}
