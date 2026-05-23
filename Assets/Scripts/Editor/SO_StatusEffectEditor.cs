using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_StatusEffect), true)]
public class SO_StatusEffectEditor : Editor
{
    SerializedProperty statusEffectType;
    SerializedProperty damageType;
    SerializedProperty stat;
    SerializedProperty power;
    SerializedProperty duration;
    SerializedProperty isMagical;
    SerializedProperty permanent;
    SerializedProperty description;

    void OnEnable()
    {
        statusEffectType = serializedObject.FindProperty("StatusEffectType");
        damageType       = serializedObject.FindProperty("DamageType");
        stat             = serializedObject.FindProperty("Stat");
        power            = serializedObject.FindProperty("Power");
        duration         = serializedObject.FindProperty("Duration");
        isMagical        = serializedObject.FindProperty("IsMagical");
        permanent        = serializedObject.FindProperty("Permanent");
        description      = serializedObject.FindProperty("Description");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var type = (StatusEffectEnum)statusEffectType.enumValueIndex;

        bool hasPower      = type == StatusEffectEnum.Bleed     || type == StatusEffectEnum.Poison  ||
                             type == StatusEffectEnum.Burn       || type == StatusEffectEnum.Thorns  ||
                             type == StatusEffectEnum.Fatique    || type == StatusEffectEnum.StatChange ||
                             type == StatusEffectEnum.Lifedrain;
        bool hasDamageType = type == StatusEffectEnum.Bleed     || type == StatusEffectEnum.Burn;
        bool hasIsMagical  = type == StatusEffectEnum.Bleed     || type == StatusEffectEnum.Poison  ||
                             type == StatusEffectEnum.Burn       || type == StatusEffectEnum.Thorns  ||
                             type == StatusEffectEnum.Fatique;
        bool hasStat       = type == StatusEffectEnum.StatChange;

        EditorGUILayout.PropertyField(statusEffectType);

        if (hasPower)      EditorGUILayout.PropertyField(power);
        if (hasDamageType) EditorGUILayout.PropertyField(damageType);
        if (hasIsMagical)  EditorGUILayout.PropertyField(isMagical);
        if (hasStat)       EditorGUILayout.PropertyField(stat);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(permanent);
        if (!permanent.boolValue)
            EditorGUILayout.PropertyField(duration);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Description Override", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(description, GUIContent.none);

        var so = (SO_StatusEffect)serializedObject.targetObject;
        if (string.IsNullOrEmpty(so.Description))
        {
            string defaultDesc = StatusEffectDescriptions.GetDefault(
                (StatusEffectEnum)statusEffectType.enumValueIndex,
                (StatsEnum)stat.enumValueIndex,
                power.intValue);
            if (!string.IsNullOrEmpty(defaultDesc))
                EditorGUILayout.HelpBox($"Default: {defaultDesc}", MessageType.None);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
