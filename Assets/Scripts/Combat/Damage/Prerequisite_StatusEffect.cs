using UnityEngine;

[CreateAssetMenu(fileName = "Prerequisite_StatusEffect", menuName = "ScriptableObjects/Prerequisites/StatusEffectPrerequisite")]
public class Prerequisite_StatusEffect : SO_Prerequisite
{
    public PrerequisiteUnitEnum Unit;
    public StatusEffectEnum StatusEffect;
    public PrerequisiteOperatorsEnum Operator;

    public override bool HasPrerequisite(Unit caster, Unit target)
    {
        Unit unit = GetUnit(caster, target);
        if (unit == null)
            return false;

        var statusIsPresent = StatusEffectManager.Instance.UnitHasStatusEffect(unit, StatusEffect);
        if (statusIsPresent && Operator == PrerequisiteOperatorsEnum.Equals)
            return true;
        if (!statusIsPresent && Operator == PrerequisiteOperatorsEnum.NotEquals)
            return true;

        return false;
    }

    Unit GetUnit(Unit caster, Unit target)
    {
        if (Unit == PrerequisiteUnitEnum.Caster)
            return caster;

        if (Unit == PrerequisiteUnitEnum.Target)
            return target;

        return null;
    }
}
