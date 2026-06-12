using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultPrerequisite))]
public class DefaultPrerequisiteEditor : Editor
{
    SerializedProperty unit;
    SerializedProperty condition;
    SerializedProperty statusEffect;
    SerializedProperty op;
    SerializedProperty value;
    SerializedProperty adjacentFaction;
    SerializedProperty requiredCombatStyle;
    SerializedProperty skillPartIndex;

    void OnEnable()
    {
        unit                = serializedObject.FindProperty("Unit");
        condition           = serializedObject.FindProperty("Condition");
        statusEffect        = serializedObject.FindProperty("StatusEffect");
        op                  = serializedObject.FindProperty("Operator");
        value               = serializedObject.FindProperty("Value");
        adjacentFaction     = serializedObject.FindProperty("AdjacentFaction");
        requiredCombatStyle = serializedObject.FindProperty("RequiredCombatStyle");
        skillPartIndex      = serializedObject.FindProperty("SkillPartIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var conditionValue = (PrerequisiteConditionEnum)condition.enumValueIndex;

        bool showUnit            = conditionValue != PrerequisiteConditionEnum.None;
        bool showStatusEffect    = conditionValue == PrerequisiteConditionEnum.StatusEffect;
        bool showOperator        = conditionValue != PrerequisiteConditionEnum.None &&
                                   conditionValue != PrerequisiteConditionEnum.CombatStyle;
        bool showValue           = conditionValue == PrerequisiteConditionEnum.Damage ||
                                   conditionValue == PrerequisiteConditionEnum.DamagePercentage ||
                                   conditionValue == PrerequisiteConditionEnum.AdjacentUnits ||
                                   conditionValue == PrerequisiteConditionEnum.TargetsHit;
        bool showAdjacentFaction = conditionValue == PrerequisiteConditionEnum.AdjacentUnits;
        bool showCombatStyle     = conditionValue == PrerequisiteConditionEnum.CombatStyle;

        EditorGUILayout.PropertyField(condition);

        if (showUnit)
            EditorGUILayout.PropertyField(unit);

        if (showStatusEffect)
            EditorGUILayout.PropertyField(statusEffect);

        if (showOperator)
            EditorGUILayout.PropertyField(op, new GUIContent("Operator"));

        if (showValue)
            EditorGUILayout.PropertyField(value);

        if (conditionValue == PrerequisiteConditionEnum.TargetsHit)
        {
            EditorGUILayout.PropertyField(skillPartIndex, new GUIContent("Skill Part Index"));
        }

        if (showAdjacentFaction)
            EditorGUILayout.PropertyField(adjacentFaction, new GUIContent("Adjacent Faction"));

        if (showCombatStyle)
            EditorGUILayout.PropertyField(requiredCombatStyle, new GUIContent("Required Combat Style"));

        serializedObject.ApplyModifiedProperties();
    }
}
