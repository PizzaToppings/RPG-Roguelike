using UnityEngine;

/// <summary>
/// True when the target unit has (or doesn't have) the specified status effect.
/// </summary>
[CreateAssetMenu(fileName = "StatusPresentCondition", menuName = "ScriptableObjects/EnemyAI/Conditions/StatusPresentCondition")]
public class SO_StatusPresentCondition : SO_AICondition
{
    public StatusEffectEnum Status;
    public ConditionTargetEnum Target = ConditionTargetEnum.Self;
    [Tooltip("When true, condition passes if status IS present. When false, passes if status is NOT present.")]
    public bool MustBePresent = true;

    public override bool Evaluate(EnemyAIContext ctx)
    {
        var unit = AIConditionHelper.ResolveUnit(ctx, Target);
        if (unit == null) return false;
        bool hasStatus = StatusEffectManager.Instance.UnitHasStatusEffect(unit, Status);
        return MustBePresent ? hasStatus : !hasStatus;
    }
}
