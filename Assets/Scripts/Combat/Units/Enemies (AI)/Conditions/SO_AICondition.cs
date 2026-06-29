using UnityEngine;

/// <summary>
/// Abstract base for all AI skill conditions.
/// Subclass and implement Evaluate to create new condition types.
/// All conditions are ScriptableObjects so they can be reused across many enemies.
/// </summary>
public abstract class SO_AICondition : ScriptableObject
{
    [TextArea(2, 4)]
    public string Description;

    public abstract bool Evaluate(EnemyAIContext ctx);
}
