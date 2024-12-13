using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_Skillpart), true)]
public class ConditionalVisibilitySkillPropertiesEditor : Editor
{
    SerializedProperty originTileKind;
    SerializedProperty originTileSkillParts;

    SerializedProperty originTargetKind;
    SerializedProperty originTargetSkillParts;

    SerializedProperty targetTileKind;
    SerializedProperty targetTileSkillParts;


    private void OnEnable()
    {
        originTileKind = serializedObject.FindProperty("OriginTileKind");
        originTileSkillParts = serializedObject.FindProperty("OriginTileSkillParts");

        originTargetKind = serializedObject.FindProperty("OriginTargetKind");
        originTargetSkillParts = serializedObject.FindProperty("OriginTargetSkillParts");

        targetTileKind = serializedObject.FindProperty("TargetTileKind");
        targetTileSkillParts = serializedObject.FindProperty("TargetTileSkillParts");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "OriginTileKind", "OriginTargetKind", "TargetTileKind", 
            "OriginTileSkillParts", "OriginTargetSkillParts", "TargetTileSkillParts");

        // OriginTileKind
        if (target is SO_AOE_Skill || target is SO_ConeSkill || target is SO_HalfCircleSkill || 
            target is SO_LineSkill || target is SO_TargetBoardtileSkill || target is SO_TargetUnitSkill)
        {
            EditorGUILayout.PropertyField(originTileKind, new GUIContent("Origin Tile Kind"));
        }

		switch ((OriginTileEnum)originTileKind.enumValueIndex)
		{
            case OriginTileEnum.GetFromSkillPart:
                EditorGUILayout.PropertyField(originTileSkillParts, new GUIContent("Origin Tile Skillparts"));
                break;
        }

        // OriginTargetKind
        //if (target is SO_AOE_Skill)
        //{
        //    EditorGUILayout.PropertyField(originTargetKind, new GUIContent("Origin Target Kind"));
        //}

        //switch ((OriginTileEnum)originTileKind.enumValueIndex)
        //{
        //    case OriginTileEnum.GetFromSkillPart:
        //        EditorGUILayout.PropertyField(originTileSkillParts, new GUIContent("Origin Tile Skillparts"));
        //        break;
        //}


        serializedObject.ApplyModifiedProperties();
    }
}
