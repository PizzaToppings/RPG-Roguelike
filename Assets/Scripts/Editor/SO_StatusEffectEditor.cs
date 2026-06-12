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
    SerializedProperty permanent;
    SerializedProperty durationTrigger;
    SerializedProperty durationOwner;
    SerializedProperty cooldownTarget;
    SerializedProperty description;

    void OnEnable()
    {
        statusEffectType = serializedObject.FindProperty("StatusEffectType");
        damageType       = serializedObject.FindProperty("DamageType");
        stat             = serializedObject.FindProperty("Stat");
        power            = serializedObject.FindProperty("Power");
        duration         = serializedObject.FindProperty("Duration");
        permanent        = serializedObject.FindProperty("Permanent");
        durationTrigger  = serializedObject.FindProperty("DurationTrigger");
        durationOwner    = serializedObject.FindProperty("DurationOwner");
        cooldownTarget   = serializedObject.FindProperty("CooldownTarget");
        description      = serializedObject.FindProperty("Description");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Re-fetch any property that failed to resolve during OnEnable (e.g. on subclasses).
        if (statusEffectType == null) statusEffectType = serializedObject.FindProperty("StatusEffectType");
        if (damageType       == null) damageType       = serializedObject.FindProperty("DamageType");
        if (stat             == null) stat             = serializedObject.FindProperty("Stat");
        if (power            == null) power            = serializedObject.FindProperty("Power");
        if (duration         == null) duration         = serializedObject.FindProperty("Duration");
        if (permanent        == null) permanent        = serializedObject.FindProperty("Permanent");
        if (durationTrigger  == null) durationTrigger  = serializedObject.FindProperty("DurationTrigger");
        if (cooldownTarget   == null) cooldownTarget   = serializedObject.FindProperty("CooldownTarget");
        if (description      == null) description      = serializedObject.FindProperty("Description");

        if (statusEffectType == null || permanent == null) return;

        var type = (StatusEffectEnum)statusEffectType.enumValueIndex;

        bool hasPower      = type == StatusEffectEnum.Bleed     || type == StatusEffectEnum.Poison  ||
                             type == StatusEffectEnum.Burn       || type == StatusEffectEnum.Thorns  ||
                             type == StatusEffectEnum.Regen    || type == StatusEffectEnum.StatChange ||
                             type == StatusEffectEnum.Lifedrain;
        bool hasDamageType = type == StatusEffectEnum.Bleed     || type == StatusEffectEnum.Burn;
        bool hasStat       = type == StatusEffectEnum.StatChange;

        EditorGUILayout.PropertyField(statusEffectType);

        if (hasPower      && power      != null) EditorGUILayout.PropertyField(power);
        if (hasDamageType && damageType != null) EditorGUILayout.PropertyField(damageType);
        if (hasStat       && stat       != null) EditorGUILayout.PropertyField(stat);

        // If the configured stat is Cooldown, expose which cooldown variant this SO should affect.
        if (hasStat && stat != null && cooldownTarget != null)
        {
            int statIdx = stat.enumValueIndex;
            if (statIdx == (int)StatsEnum.Cooldown)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(cooldownTarget, new GUIContent("Cooldown Target", "Choose which cooldown variant the status effect affects: Active = current active cooldown (applies immediately), Default = skill's default cooldown (can be reverted when effect ends), Both = both variants."));

                // Helpful note about semantics
                EditorGUILayout.HelpBox("Note: Active cooldown changes apply immediately to the skill's current cooldown value and are effectively instant.\nDefault cooldown changes modify the skill's DefaultCooldown; if the status effect has a duration (and is not permanent), the modification will be removed when the effect ends.", MessageType.Info);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(permanent);
        if (!permanent.boolValue)
        {
            if (duration != null) EditorGUILayout.PropertyField(duration);
            if (durationTrigger != null) EditorGUILayout.PropertyField(durationTrigger);

            // Show which unit's turn the duration should tick on for turn-based triggers
            if (durationTrigger != null && durationOwner != null)
            {
                // TriggerMomentEnum: Instant, StartOfCombat, StartOfTurn, EndOfTurn, StartOfRound, EndOfRound, ...
                int idx = durationTrigger.enumValueIndex;
                if (idx == (int)TriggerMomentEnum.StartOfTurn || idx == (int)TriggerMomentEnum.EndOfTurn)
                {
                    EditorGUILayout.PropertyField(durationOwner);
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Description Override", EditorStyles.boldLabel);
        if (description != null) EditorGUILayout.PropertyField(description, GUIContent.none);

        var so = (SO_StatusEffect)serializedObject.targetObject;
        if (description == null || string.IsNullOrEmpty(so.Description))
        {
            string defaultDesc = StatusEffectDescriptions.GetDefault(
                (StatusEffectEnum)statusEffectType.enumValueIndex,
                stat  != null ? (StatsEnum)stat.enumValueIndex : default,
                power != null ? power.intValue : 0);
            if (!string.IsNullOrEmpty(defaultDesc))
                EditorGUILayout.HelpBox($"Default: {defaultDesc}", MessageType.None);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
