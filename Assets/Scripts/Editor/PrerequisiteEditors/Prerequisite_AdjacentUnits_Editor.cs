using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prerequisite_AdjacentUnits))]
public class Prerequisite_AdjacentUnits_Editor : Editor
{
    SerializedProperty unit;
    SerializedProperty op;
    SerializedProperty value;
    SerializedProperty adjacentFaction;

    void OnEnable()
    {
        unit = serializedObject.FindProperty("Unit");
        op = serializedObject.FindProperty("Operator");
        value = serializedObject.FindProperty("Value");
        adjacentFaction = serializedObject.FindProperty("AdjacentFaction");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(unit);
        EditorGUILayout.PropertyField(op, new GUIContent("Operator"));
        EditorGUILayout.PropertyField(value);
        EditorGUILayout.PropertyField(adjacentFaction, new GUIContent("Adjacent Faction"));

        serializedObject.ApplyModifiedProperties();
    }
}
