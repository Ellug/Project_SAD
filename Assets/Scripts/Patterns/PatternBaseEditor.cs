using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatternBase), true)]
public class PatternBaseEditor : Editor
{
    SerializedProperty mode;
    SerializedProperty warningArea;
    SerializedProperty warningTime;
    SerializedProperty warningDTime;

    SerializedProperty warningMaxLength;
    SerializedProperty warningWidth;
    SerializedProperty lengthScaleModifier;
    SerializedProperty groundLayer;

    void OnEnable()
    {
        mode = serializedObject.FindProperty("mode");

        warningArea = serializedObject.FindProperty("_WarnningArea");
        warningTime = serializedObject.FindProperty("_WarnningTime");
        warningDTime = serializedObject.FindProperty("_WarnningDTime");

        warningMaxLength = serializedObject.FindProperty("_WarnningMaxLength");
        warningWidth = serializedObject.FindProperty("_WarnningWidth");
        lengthScaleModifier = serializedObject.FindProperty("_lengthScaleModifier");
        groundLayer = serializedObject.FindProperty("_groundLayer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 🔹 모드 선택
        EditorGUILayout.PropertyField(mode);
        EditorGUILayout.Space();

        var currentMode = (PatternBase.OptionMode)mode.enumValueIndex;

        switch (currentMode)
        {
            case PatternBase.OptionMode.None:
                DrawNoneMode();
                break;

            case PatternBase.OptionMode.직선:
                DrawCommonWarning();
                DrawLineWarning();
                break;

            case PatternBase.OptionMode.원형:
                DrawCommonWarning();
                break;
        }

        EditorGUILayout.Space();
        DrawRemainingProperties();

        serializedObject.ApplyModifiedProperties();
    }

    // =======================
    // Mode Draw Functions
    // =======================

    void DrawNoneMode()
    {
        EditorGUILayout.HelpBox(
            "경고장판이 비활성화되어 있습니다.",
            MessageType.Info);

        ResetWarningValues();
    }

    void DrawCommonWarning()
    {
        EditorGUILayout.LabelField("경고장판 공통 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(warningArea);
        EditorGUILayout.PropertyField(warningTime);
        EditorGUILayout.PropertyField(warningDTime);
    }

    void DrawLineWarning()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("직선 장판 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(warningMaxLength);
        EditorGUILayout.PropertyField(warningWidth);
        EditorGUILayout.PropertyField(lengthScaleModifier);
        EditorGUILayout.PropertyField(groundLayer);
    }

    // =======================
    // Utilities
    // =======================

    void ResetWarningValues()
    {
        warningTime.floatValue = 0f;
        warningDTime.floatValue = 0f;

        warningMaxLength.floatValue = 0f;
        warningWidth.floatValue = 0f;
        lengthScaleModifier.floatValue = 0f;
        groundLayer.intValue = 0;
    }

    void DrawRemainingProperties()
    {
        // mode 이후의 나머지 SerializeField 자동 출력
        DrawPropertiesExcluding(
            serializedObject,
            "mode",
            "_WarnningArea",
            "_WarnningTime",
            "_WarnningDTime",
            "_WarnningMaxLength",
            "_WarnningWidth",
            "_lengthScaleModifier",
            "_groundLayer"
        );
    }
}
