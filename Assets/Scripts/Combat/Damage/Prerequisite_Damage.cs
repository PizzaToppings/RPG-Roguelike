using UnityEngine;

[CreateAssetMenu(fileName = "Prerequisite_Damage", menuName = "ScriptableObjects/Prerequisites/DamagePrerequisite")]
public class Prerequisite_Damage : SO_Prerequisite
{
    public PrerequisiteUnitEnum Unit;
    public PrerequisiteOperatorsEnum Operator;

    // If true, compare current HP percentage (0-100), otherwise compare absolute damage (MaxHitpoints - Hitpoints)
    public bool UsePercentage = false;
    public float Value;

    public override bool HasPrerequisite(Unit caster, Unit target)
    {
        Unit unit = GetUnit(caster, target);
        if (unit == null)
            return false;

        float comparisonValue;
        if (UsePercentage)
        {
            comparisonValue = unit.Hitpoints / (float)unit.MaxHitpoints * 100f;
        }
        else
        {
            comparisonValue = unit.MaxHitpoints - unit.Hitpoints;
        }

        return CheckWithOperators(comparisonValue, Value);
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
                return value == comparisonValue;
            case PrerequisiteOperatorsEnum.LessThen:
                return value < comparisonValue;
            case PrerequisiteOperatorsEnum.MoreThan:
                return value > comparisonValue;
            case PrerequisiteOperatorsEnum.LessThenOrEqual:
                return value <= comparisonValue;
            case PrerequisiteOperatorsEnum.MoreThanOrEqual:
                return value >= comparisonValue;
            case PrerequisiteOperatorsEnum.NotEquals:
                return value != comparisonValue;
        }

        return false;
    }
}
