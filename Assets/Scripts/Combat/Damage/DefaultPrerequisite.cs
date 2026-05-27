using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Prerequisite", menuName = "ScriptableObjects/Prerequisites/DefaultPrerequisite")]
public class DefaultPrerequisite : SO_Prerequisite
{
    public PrerequisiteUnitEnum Unit;
    public PrerequisiteConditionEnum Condition;

    // StatusEffect condition
    public StatusEffectEnum StatusEffect;
    public PrerequisiteOperatorsEnum Operator;

    // Damage / DamagePercentage / AdjacentUnits conditions
    public float Value;

    // AdjacentUnits condition
    public PrerequisiteAdjacentFactionEnum AdjacentFaction;

    // CombatStyle condition
    public CombatStyle RequiredCombatStyle;

    public override bool HasPrerequisite(Unit caster, Unit target)
    {
        Unit unit = GetUnit(caster, target);
        if (unit == null)
            return false;

        switch (Condition)
        {
            case PrerequisiteConditionEnum.StatusEffect:
                var statusIsPresent = StatusEffectManager.Instance.UnitHasStatusEffect(unit, StatusEffect);
                if (statusIsPresent && Operator == PrerequisiteOperatorsEnum.Equals)
                    return true;
                if (!statusIsPresent && Operator == PrerequisiteOperatorsEnum.NotEquals)
                    return true;
                break;

            case PrerequisiteConditionEnum.Damage:
                var damage = unit.MaxHitpoints - unit.Hitpoints;
                return CheckWithOperators(damage, Value);

            case PrerequisiteConditionEnum.DamagePercentage:
                var damagePercentage = unit.Hitpoints / (float)unit.MaxHitpoints * 100f;
                return CheckWithOperators(damagePercentage, Value);

            case PrerequisiteConditionEnum.AdjacentUnits:
                var adjacentCount = GetAdjacentUnitCount(unit);
                return CheckWithOperators(adjacentCount, Value);

            case PrerequisiteConditionEnum.CombatStyle:
                return unit.CurrentCombatStyle == RequiredCombatStyle;
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
