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
    public int PhysicalPower;
    public int MagicalPower;
    public int PhysicalDefense;
    public int MagicalDefense;

    [Space]
    [Tooltip("If true, the first skill in the Skills list is always used on the first turn.")]
    public bool AlwaysStartWithFirstSkill = false;
    public List<SO_EnemySkill> Skills = new List<SO_EnemySkill>();

    [Space]
    public List<SO_EnemyAbility> Abilities = new List<SO_EnemyAbility>();
}
