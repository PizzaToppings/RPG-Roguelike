using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prerequisite_Damage))]
public class Prerequisite_Damage_Editor : Editor
{
    SerializedProperty unit;
    SerializedProperty op;
    SerializedProperty usePercentage;
    SerializedProperty value;

    void OnEnable()
    {
        unit = serializedObject.FindProperty("Unit");
        op = serializedObject.FindProperty("Operator");
        usePercentage = serializedObject.FindProperty("UsePercentage");
        value = serializedObject.FindProperty("Value");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(unit);
        EditorGUILayout.PropertyField(op, new GUIContent("Operator"));
        EditorGUILayout.PropertyField(usePercentage, new GUIContent("Use Percentage"));
        EditorGUILayout.PropertyField(value);

        serializedObject.ApplyModifiedProperties();
    }
}
