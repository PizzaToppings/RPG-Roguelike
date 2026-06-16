using UnityEngine;

[CreateAssetMenu(fileName = "Prerequisite_TargetsHit", menuName = "ScriptableObjects/Prerequisites/TargetsHitPrerequisite")]
public class Prerequisite_TargetsHit : SO_Prerequisite
{
    public PrerequisiteUnitEnum Unit;
    public PrerequisiteOperatorsEnum Operator;
    public float Value;
    public int SkillPartIndex = 0;

    public override bool HasPrerequisite(Unit caster, Unit target)
    {
        Unit unit = GetUnit(caster, target);
        if (unit == null)
            return false;

        // Count targets hit for the configured skill part index. If SkillData not set, return false.
        if (SkillData.CurrentActiveSkill == null)
            return false;

        var spIndex = Mathf.Clamp(SkillPartIndex, 0, int.MaxValue);
        var targets = new System.Collections.Generic.List<Unit>();
        // If the SkillPartGroupData is not initialized or index out of range, count as 0
        try
        {
            var currentTargets = SkillData.GetCurrentTargetsHit(spIndex);
            if (currentTargets != null)
                targets.AddRange(currentTargets);
        }
        catch
        {
            // ignore and treat as zero targets
        }

        return CheckWithOperators(targets.Count, Value);
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
