using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prerequisite_StatusEffect))]
public class Prerequisite_StatusEffect_Editor : Editor
{
    SerializedProperty unit;
    SerializedProperty statusEffect;
    SerializedProperty op;

    void OnEnable()
    {
        unit = serializedObject.FindProperty("Unit");
        statusEffect = serializedObject.FindProperty("StatusEffect");
        op = serializedObject.FindProperty("Operator");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(unit);
        EditorGUILayout.PropertyField(statusEffect);
        EditorGUILayout.PropertyField(op, new GUIContent("Operator"));

        serializedObject.ApplyModifiedProperties();
    }
}
