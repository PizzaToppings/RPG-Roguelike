using UnityEngine;

/// <summary>
/// Per-enemy-type weights that control how the AI values different skill effects.
/// Aggressive enemies value damage highly; support enemies value healing and buffs.
/// </summary>
[CreateAssetMenu(fileName = "EffectWeightConfig", menuName = "ScriptableObjects/EnemyAI/EffectWeightConfig")]
public class SO_EffectWeightConfig : ScriptableObject
{
    [Header("Offensive")]
    [Tooltip("Value per point of damage dealt.")]
    public float DamageWeight = 1f;
    [Tooltip("Value per DoT status (Burn, Bleed, Poison).")]
    public float DoTWeight = 0.8f;
    [Tooltip("Value per debuff applied to a character.")]
    public float DebuffWeight = 0.6f;
    [Tooltip("Displacement (knockback/pull) value.")]
    public float DisplacementWeight = 0.4f;

    [Header("Defensive / Support")]
    [Tooltip("Value per point of healing applied to an ally.")]
    public float HealWeight = 1f;
    [Tooltip("Value per point of shield applied to an ally.")]
    public float ShieldWeight = 0.8f;
    [Tooltip("Value per buff applied to an ally.")]
    public float BuffWeight = 0.6f;
}
