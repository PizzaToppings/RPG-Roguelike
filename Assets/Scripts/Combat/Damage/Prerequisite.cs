using UnityEngine;

[CreateAssetMenu(fileName = "Prerequisite", menuName = "ScriptableObjects/Prerequisite")]
public class Prerequisite : ScriptableObject
{
    public PrerequisiteUnitEnum Unit;
    public PrerequisiteConditionEnum Condition;
    public PrerequisiteOperatorsEnum Operator;
    public StatusEfectEnum StatusEffect;
    public float Value;

    public bool HasPrerequisite(Unit caster, Unit target) // TODO change this. Can probably be obtained from damageData or something similar
    {
        Unit unit = GetUnit(caster, target);
        var statusEffectManager = StatusEffectManager.Instance;

        switch (Condition)
        {
            case PrerequisiteConditionEnum.StatusEffect:
                if (statusEffectManager.UnitHasStatusEffect(unit, StatusEffect))
                    return true;

                break;

            case PrerequisiteConditionEnum.Damage:
                var damage = unit.MaxHitpoints - unit.Hitpoints;
                return CheckWithOperators(damage, Value);

            case PrerequisiteConditionEnum.DamagePercentage:
                var damagePercentage = unit.Hitpoints / unit.MaxHitpoints * 100;
                return CheckWithOperators(damagePercentage, Value);
        }

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

    bool CheckWithOperators(float value, float comparisonValue)
    {
        switch (Operator)
        {
            case PrerequisiteOperatorsEnum.Equals:
                if (value == comparisonValue)
                    return true;

                break;

            case PrerequisiteOperatorsEnum.LessThen:
                if (value < comparisonValue)
                    return true;

                break;

            case PrerequisiteOperatorsEnum.MoreThan:
                if (value > comparisonValue)
                    return true;

                break;

            case PrerequisiteOperatorsEnum.LessThenOrEqual:
                if (value <= comparisonValue)
                    return true;

                break;

            case PrerequisiteOperatorsEnum.MoreThanOrEqual:
                if (value >= comparisonValue)
                    return true;

                break;
        }

        return false;
    }
}
