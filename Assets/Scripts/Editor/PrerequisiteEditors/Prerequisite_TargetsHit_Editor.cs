using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prerequisite_TargetsHit))]
public class Prerequisite_TargetsHit_Editor : Editor
{
    SerializedProperty unit;
    SerializedProperty op;
    SerializedProperty value;
    SerializedProperty skillPartIndex;

    void OnEnable()
    {
        unit = serializedObject.FindProperty("Unit");
        op = serializedObject.FindProperty("Operator");
        value = serializedObject.FindProperty("Value");
        skillPartIndex = serializedObject.FindProperty("SkillPartIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(unit);
        EditorGUILayout.PropertyField(op, new GUIContent("Operator"));
        EditorGUILayout.PropertyField(value);
        EditorGUILayout.PropertyField(skillPartIndex, new GUIContent("Skill Part Index"));

        serializedObject.ApplyModifiedProperties();
    }
}
