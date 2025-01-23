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

    SerializedProperty directionAnchor;
    SerializedProperty directionAnchorSkillPart;

    SerializedProperty directionInput;
    SerializedProperty directionInputSkillPart;

    SerializedProperty addProjectileLine;
    SerializedProperty projectileLineOffset;

    private void OnEnable()
    {
        originTileKind = serializedObject.FindProperty("OriginTileKind");
        originTileSkillParts = serializedObject.FindProperty("OriginTileSkillParts");

        originTargetKind = serializedObject.FindProperty("OriginTargetKind");
        originTargetSkillParts = serializedObject.FindProperty("OriginTargetSkillParts");

        targetTileKind = serializedObject.FindProperty("TargetTileKind");
        targetTileSkillParts = serializedObject.FindProperty("TargetTileSkillParts");

        directionAnchor = serializedObject.FindProperty("DirectionAnchor");
        directionAnchorSkillPart = serializedObject.FindProperty("DirectionAnchorSkillPart");

        directionInput = serializedObject.FindProperty("DirectionInput");
        directionInputSkillPart = serializedObject.FindProperty("DirectionInputSkillPart");

        addProjectileLine = serializedObject.FindProperty("AddProjectileLine");
        projectileLineOffset = serializedObject.FindProperty("ProjectileLineOffset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "OriginTileKind", "OriginTargetKind", "TargetTileKind", 
            "OriginTileSkillParts", "OriginTargetSkillParts", "TargetTileSkillParts", "DirectionAnchor", "DirectionAnchorSkillPart",
            "AddProjectileLine", "ProjectileLineOffset", "DirectionInput", "DirectionInputSkillPart");

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

        // DirectionCenterTile
        if (target is SO_ConeSkill || target is SO_HalfCircleSkill || target is SO_LineSkill)
        {
            EditorGUILayout.PropertyField(directionAnchor, new GUIContent("Direction Anchor"));
            EditorGUILayout.PropertyField(directionInput, new GUIContent("Direction Input"));
        }

        switch ((OriginTileEnum)directionAnchor.enumValueIndex)
        {
            case OriginTileEnum.GetFromSkillPart:
                EditorGUILayout.PropertyField(directionAnchorSkillPart, new GUIContent("Direction Anchor Skillpart"));
                break;
        }

        //switch ((DirectionInputEnum)directionInput.enumValueIndex)
        //{
        //    case DirectionInputEnum.OriginTile:
        //        EditorGUILayout.PropertyField(directionInputSkillPart, new GUIContent("Direction Input Skillpart"));
        //        break;
        //}

        // ProjectileLine
        if (target is SO_TargetBoardtileSkill || target is SO_TargetUnitSkill)
        {
            EditorGUILayout.PropertyField(addProjectileLine, new GUIContent("Add Projectile Line"));
        }

        switch ((bool)addProjectileLine.boolValue)
        {
            case true:
                EditorGUILayout.PropertyField(projectileLineOffset, new GUIContent("Projectile line Offset"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
