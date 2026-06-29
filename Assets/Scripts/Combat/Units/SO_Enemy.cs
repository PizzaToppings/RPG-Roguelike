using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy")]
public class SO_Enemy : ScriptableObject
{
    public string Name;
    public Sprite Image;

    [Space]
    public int MaxHealth;
    public int MoveSpeed;
    public int Power;
    public int Armor;

    [Space]
    [Tooltip("If true, the first skill in the Skills list is always used on the first turn.")]
    public bool AlwaysStartWithFirstSkill = false;
    public List<SO_EnemySkill> Skills = new List<SO_EnemySkill>();

    [Space]
    [Header("AI Configuration")]
    [Tooltip("Determines which eligible skill is chosen each turn. Assign a strategy asset.")]
    public SO_SkillSelectionStrategy SkillSelectionStrategy;

    [Tooltip("Movement and positioning profile (Melee / Ranged / Support).")]
    public SO_AIProfile AIProfile;

    [Tooltip("Per-effect value weights used when evaluating skill targets. Leave null for defaults.")]
    public SO_EffectWeightConfig EffectWeights;

    [Space]
    public List<SO_EnemyAbility> Abilities = new List<SO_EnemyAbility>();
}
