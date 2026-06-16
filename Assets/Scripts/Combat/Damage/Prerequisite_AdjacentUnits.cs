using UnityEngine;

[CreateAssetMenu(fileName = "Prerequisite_AdjacentUnits", menuName = "ScriptableObjects/Prerequisites/AdjacentUnitsPrerequisite")]
public class Prerequisite_AdjacentUnits : SO_Prerequisite
{
    public PrerequisiteUnitEnum Unit;
    public PrerequisiteOperatorsEnum Operator;
    public float Value;
    public PrerequisiteAdjacentFactionEnum AdjacentFaction;

    public override bool HasPrerequisite(Unit caster, Unit target)
    {
        Unit unit = GetUnit(caster, target);
        if (unit == null)
            return false;

        int adjacentCount = GetAdjacentUnitCount(unit);
        return CheckWithOperators(adjacentCount, Value);
    }

    Unit GetUnit(Unit caster, Unit target)
    {
        if (Unit == PrerequisiteUnitEnum.Caster)
            return caster;

        if (Unit == PrerequisiteUnitEnum.Target)
            return target;

        return null;
    }

    int GetAdjacentUnitCount(Unit unit)
    {
        if (unit.Tile == null)
            return 0;

        int count = 0;
        foreach (var tile in unit.Tile.connectedTiles)
        {
            if (tile == null || tile.currentUnit == null)
                continue;

            var adjacentUnit = tile.currentUnit;

            switch (AdjacentFaction)
            {
                case PrerequisiteAdjacentFactionEnum.Any:
                    count++;
                    break;
                case PrerequisiteAdjacentFactionEnum.Friendly:
                    if (adjacentUnit.Friendly == unit.Friendly)
                        count++;
                    break;
                case PrerequisiteAdjacentFactionEnum.Enemy:
                    if (adjacentUnit.Friendly != unit.Friendly)
                        count++;
                    break;
            }
        }

        return count;
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
