using UnityEngine;

/// <summary>
/// Movement and positioning preferences for an enemy AI role.
/// Assigned at the SO_Enemy level and applied during all movement phases.
/// </summary>
[CreateAssetMenu(fileName = "AIProfile", menuName = "ScriptableObjects/EnemyAI/AIProfile")]
public class SO_AIProfile : ScriptableObject
{
    public AIRoleEnum Role = AIRoleEnum.Melee;

    [Tooltip("Preferred standing distance (in tiles) from the primary target. " +
             "Melee: ~1.5, Ranged: ~4, Support: ~2.")]
    public float OptimalRange = 1.5f;

    [Tooltip("Never move closer than this distance (useful for ranged enemies). Set to 0 for melee.")]
    public float MinRange = 0f;

    [Tooltip("Support role: prefer tiles closer to allies over tiles closer to the target.")]
    public bool PreferNearAllies = false;
}
