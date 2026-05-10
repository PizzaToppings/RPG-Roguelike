using UnityEngine;

/// <summary>
/// Base class for all enemy passive/triggered abilities.
/// Subclass this (or use DefaultEnemyAbility for data-driven effects)
/// and override Init to hook into enemy events.
/// </summary>
public abstract class SO_EnemyAbility : ScriptableObject
{
    public string AbilityName;
    [TextArea(3, 6)] public string Description;

    /// <summary>Called once when the enemy initialises in combat.</summary>
    public virtual void Init(Enemy enemy, EnemyAbility ability) { }
}
