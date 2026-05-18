using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultTrinket))]
public class DefaultTrinketEditor : Editor
{
    // SO_Trinket base fields
    SerializedProperty trinketName;
    SerializedProperty image;
    SerializedProperty classes;
    SerializedProperty rarity;
    SerializedProperty description;

    // DefaultTrinket fields
    SerializedProperty triggerMoment;
    SerializedProperty triggerEffect;
    SerializedProperty chargesToTrigger;
    SerializedProperty triggerOnce;
    SerializedProperty value;
    SerializedProperty damageType;
    SerializedProperty isMagical;
    SerializedProperty target;
    SerializedProperty range;
    SerializedProperty statusEffects;
    SerializedProperty stat;
    SerializedProperty requiredSkillStyle;

    void OnEnable()
    {
        trinketName      = serializedObject.FindProperty("TrinketName");
        image            = serializedObject.FindProperty("Image");
        classes          = serializedObject.FindProperty("classes");
        rarity           = serializedObject.FindProperty("Rarity");
        description      = serializedObject.FindProperty("Description");

        triggerMoment    = serializedObject.FindProperty("TriggerMoment");
        triggerEffect    = serializedObject.FindProperty("TriggerEffect");
        chargesToTrigger = serializedObject.FindProperty("ChargesToTrigger");
        triggerOnce      = serializedObject.FindProperty("TriggerOnce");
        value            = serializedObject.FindProperty("Value");
        damageType       = serializedObject.FindProperty("DamageType");
        isMagical        = serializedObject.FindProperty("IsMagical");
        target           = serializedObject.FindProperty("Target");
        range            = serializedObject.FindProperty("Range");
        statusEffects    = serializedObject.FindProperty("StatusEffects");
        stat             = serializedObject.FindProperty("Stat");
        requiredSkillStyle = serializedObject.FindProperty("RequiredSkillStyle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var effect     = (TriggerEffectEnum)triggerEffect.enumValueIndex;
        var moment     = (TriggerMomentEnum)triggerMoment.enumValueIndex;
        var targetType = (TargetEnum)target.enumValueIndex;

        bool implicitlyOnce = moment == TriggerMomentEnum.Instant || moment == TriggerMomentEnum.StartOfCombat || moment == TriggerMomentEnum.EndOfCombat;
        bool showValue         = effect != TriggerEffectEnum.AddStatusEffect;
        bool showDamageType    = effect == TriggerEffectEnum.DealDamage;
        var  selectedDamageType = (DamageTypeEnum)damageType.enumValueIndex;
        bool showIsMagical     = showDamageType && selectedDamageType != DamageTypeEnum.Healing && selectedDamageType != DamageTypeEnum.Shield;
        bool showTarget        = effect == TriggerEffectEnum.DealDamage  || effect == TriggerEffectEnum.AddStatusEffect || effect == TriggerEffectEnum.ModifyStat;
        bool showRange         = showTarget && targetType != TargetEnum.Self;
        bool showStatusEffects = effect == TriggerEffectEnum.AddStatusEffect;
        bool showStat          = effect == TriggerEffectEnum.ModifyStat;
        bool showSkillStyle    = moment == TriggerMomentEnum.OnUseAbility;

        // SO_Trinket base fields
        EditorGUILayout.PropertyField(trinketName);
        EditorGUILayout.PropertyField(image);
        EditorGUILayout.PropertyField(classes);
        EditorGUILayout.PropertyField(rarity);
        EditorGUILayout.PropertyField(description);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Trigger", EditorStyles.boldLabel);

        // Always visible
        EditorGUILayout.PropertyField(triggerMoment);
        EditorGUILayout.PropertyField(triggerEffect);
        if (showSkillStyle)
        {
            EditorGUILayout.PropertyField(requiredSkillStyle, new GUIContent("Required Skill Style", "Filter by skill combat style. Set to None to trigger on any skill."));
        }
        if (!implicitlyOnce)
        {
            EditorGUILayout.PropertyField(chargesToTrigger);
            EditorGUILayout.PropertyField(triggerOnce);
        }

        EditorGUILayout.Space();
        if (showValue)
            EditorGUILayout.PropertyField(value);

        // Damage type
        if (showDamageType)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(damageType);
            if (showIsMagical)
                EditorGUILayout.PropertyField(isMagical);
        }

        // Target and range
        if (showTarget)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(target);
            if (showRange)
                EditorGUILayout.PropertyField(range);
        }

        // Status effects list
        if (showStatusEffects)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(statusEffects);
        }

        // Stat list
        if (showStat)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(stat);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
