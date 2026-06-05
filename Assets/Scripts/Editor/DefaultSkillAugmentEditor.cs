using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultSkillAugment))]
public class DefaultSkillAugmentEditor : Editor
{
    SerializedProperty augmentName;
    SerializedProperty image;
    SerializedProperty description;

    SerializedProperty triggerMoment;
    SerializedProperty triggerOnce;
    SerializedProperty chargesToTrigger;
    SerializedProperty requiredSkillStyle;
    SerializedProperty triggerEffect;
    SerializedProperty value;
    SerializedProperty statusEffects;
    SerializedProperty skillToReset;

    void OnEnable()
    {
        augmentName = serializedObject.FindProperty("AugmentName");
        image = serializedObject.FindProperty("Image");
        description = serializedObject.FindProperty("Description");

        triggerMoment = serializedObject.FindProperty("TriggerMoment");
        triggerOnce = serializedObject.FindProperty("TriggerOnce");
        chargesToTrigger = serializedObject.FindProperty("ChargesToTrigger");
        requiredSkillStyle = serializedObject.FindProperty("RequiredSkillStyle");
        triggerEffect = serializedObject.FindProperty("TriggerEffect");
        value = serializedObject.FindProperty("Value");
        statusEffects = serializedObject.FindProperty("StatusEffects");
        skillToReset = serializedObject.FindProperty("SkillToReset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(augmentName);
        EditorGUILayout.PropertyField(image);
        EditorGUILayout.PropertyField(description);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Trigger", EditorStyles.boldLabel);

        var moment = (SkillAugmentTriggerEnum)triggerMoment.enumValueIndex;

        EditorGUILayout.PropertyField(triggerMoment);
        EditorGUILayout.PropertyField(triggerEffect);

        // Show optional required stance when OnStanceChange selected
        if (moment == SkillAugmentTriggerEnum.OnStanceChange)
        {
            EditorGUILayout.PropertyField(requiredSkillStyle, new GUIContent("Required Skill Stance", "Filter by the new stance when a character switches stance. Set to None to trigger on any stance change."));
        }

        bool implicitlyOnce = moment == SkillAugmentTriggerEnum.OnInit;
        if (!implicitlyOnce)
        {
            EditorGUILayout.PropertyField(chargesToTrigger);
            EditorGUILayout.PropertyField(triggerOnce);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(value);

        if ((SkillAugmentEffectEnum)triggerEffect.enumValueIndex == SkillAugmentEffectEnum.AddStatusEffectToTargets ||
            (SkillAugmentEffectEnum)triggerEffect.enumValueIndex == SkillAugmentEffectEnum.AddStatusEffectToCaster)
        {
            EditorGUILayout.PropertyField(statusEffects);
        }

        if ((SkillAugmentEffectEnum)triggerEffect.enumValueIndex == SkillAugmentEffectEnum.ResetSkill)
        {
            EditorGUILayout.PropertyField(skillToReset);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
