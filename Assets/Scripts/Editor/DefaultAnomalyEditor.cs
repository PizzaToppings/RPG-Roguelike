using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAnomaly))]
public class DefaultAnomalyEditor : Editor
{
    SerializedProperty anomalyName;
    SerializedProperty image;
    SerializedProperty rarity;
    SerializedProperty description;

    SerializedProperty triggerMoment;
    SerializedProperty triggerOnce;
    SerializedProperty chargesToTrigger;
    SerializedProperty target;
    SerializedProperty triggerEffect;
    SerializedProperty value;
    SerializedProperty hitType;
    SerializedProperty statusEffects;
    SerializedProperty stat;
    SerializedProperty requiredSkillStyle;

    void OnEnable()
    {
        anomalyName = serializedObject.FindProperty("AnomalyName");
        image = serializedObject.FindProperty("Image");
        rarity = serializedObject.FindProperty("Rarity");
        description = serializedObject.FindProperty("Description");

        triggerMoment = serializedObject.FindProperty("TriggerMoment");
        triggerOnce = serializedObject.FindProperty("TriggerOnce");
        chargesToTrigger = serializedObject.FindProperty("ChargesToTrigger");
        target = serializedObject.FindProperty("Target");
        triggerEffect = serializedObject.FindProperty("TriggerEffect");
        value = serializedObject.FindProperty("Value");
        hitType = serializedObject.FindProperty("HitType");
        statusEffects = serializedObject.FindProperty("StatusEffects");
        stat = serializedObject.FindProperty("Stat");
        requiredSkillStyle = serializedObject.FindProperty("RequiredSkillStyle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Base fields
        EditorGUILayout.PropertyField(anomalyName);
        EditorGUILayout.PropertyField(image);
        EditorGUILayout.PropertyField(rarity);
        EditorGUILayout.PropertyField(description);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Trigger", EditorStyles.boldLabel);

        var moment = (AnomalyTriggerEnum)triggerMoment.enumValueIndex;

        EditorGUILayout.PropertyField(triggerMoment);
        EditorGUILayout.PropertyField(triggerEffect);

        // Show optional required stance when OnStanceChange selected
        if (moment == AnomalyTriggerEnum.OnStanceChange)
        {
            EditorGUILayout.PropertyField(requiredSkillStyle, new GUIContent("Required Skill Stance", "Filter by the new stance when a character switches stance. Set to None to trigger on any stance change."));
        }

        // Show charges/once for non-implicit moments
        bool implicitlyOnce = moment == AnomalyTriggerEnum.OnCombatStart || moment == AnomalyTriggerEnum.OnCombatEnd;
        if (!implicitlyOnce)
        {
            EditorGUILayout.PropertyField(chargesToTrigger);
            EditorGUILayout.PropertyField(triggerOnce);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(value);

        if ((TriggerEffectEnum)triggerEffect.enumValueIndex == TriggerEffectEnum.DealDamage)
            EditorGUILayout.PropertyField(hitType);

        if ((TriggerEffectEnum)triggerEffect.enumValueIndex == TriggerEffectEnum.AddStatusEffect)
            EditorGUILayout.PropertyField(statusEffects);

        if ((TriggerEffectEnum)triggerEffect.enumValueIndex == TriggerEffectEnum.ModifyStat)
            EditorGUILayout.PropertyField(stat);

        serializedObject.ApplyModifiedProperties();
    }
}
