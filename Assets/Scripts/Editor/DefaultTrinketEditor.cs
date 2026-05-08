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
    SerializedProperty target;
    SerializedProperty range;
    SerializedProperty statusEffects;
    SerializedProperty stat;

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
        target           = serializedObject.FindProperty("Target");
        range            = serializedObject.FindProperty("Range");
        statusEffects    = serializedObject.FindProperty("StatusEffects");
        stat             = serializedObject.FindProperty("Stat");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var effect     = (TriggerEffectEnum)triggerEffect.enumValueIndex;
        var targetType = (TargetEnum)target.enumValueIndex;

        bool showDamageType    = effect == TriggerEffectEnum.DealDamage  || effect == TriggerEffectEnum.TakeDamage;
        bool showTarget        = effect == TriggerEffectEnum.DealDamage  || effect == TriggerEffectEnum.AddStatusEffect || effect == TriggerEffectEnum.ModifyStat;
        bool showRange         = showTarget && targetType != TargetEnum.Self;
        bool showStatusEffects = effect == TriggerEffectEnum.AddStatusEffect;
        bool showStat          = effect == TriggerEffectEnum.ModifyStat;

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
        EditorGUILayout.PropertyField(chargesToTrigger);
        EditorGUILayout.PropertyField(triggerOnce);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(value);

        // Damage type
        if (showDamageType)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(damageType);
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
