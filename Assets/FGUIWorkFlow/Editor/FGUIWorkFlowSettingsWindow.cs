using UnityEditor;
using UnityEngine;

public class FGUIWorkFlowSettingsWindow : EditorWindow
{

    [MenuItem("FGUI Work Flow/Settings")]
    public static void ShowWindow()
    {
        FGUIWorkFlowSettings settings = AssetDatabase.LoadAssetAtPath<FGUIWorkFlowSettings>(SettingsAssetPath);

        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<FGUIWorkFlowSettings>();
            AssetDatabase.CreateAsset(settings, SettingsAssetPath);
            AssetDatabase.SaveAssets();
        }

        EditorWindow.GetWindow<FGUIWorkFlowSettingsWindow>("FGUI Work Flow Settings");
    }
    private const string SettingsAssetPath = "Assets/FGUIWorkFlow/FGUIWorkFlowSettings.asset";

    private FGUIWorkFlowSettings _settings;
    private SerializedObject _serializedSettings;

    private Vector2 _scrollPos;

    private bool _showLocalPackages = true;
    private bool _showCommonPackages = true;

    private void OnEnable()
    {
        _settings = AssetDatabase.LoadAssetAtPath<FGUIWorkFlowSettings>(SettingsAssetPath);

        if (_settings == null)
        {
            _settings = CreateInstance<FGUIWorkFlowSettings>();
            AssetDatabase.CreateAsset(_settings, SettingsAssetPath);
        }

        _serializedSettings = new SerializedObject(_settings);
    }
    private void OnGUI()
    {
        GUILayout.Label("FGUI Work Flow Settings", EditorStyles.boldLabel);

        _serializedSettings.Update();

        EditorGUILayout.PropertyField(_serializedSettings.FindProperty("resourceDir"));
        EditorGUILayout.PropertyField(_serializedSettings.FindProperty("outputDir"));
        EditorGUILayout.PropertyField(_serializedSettings.FindProperty("localOutputDir"));

        EditorGUILayout.Space();

        _showLocalPackages = EditorGUILayout.Foldout(_showLocalPackages, "Local Packages");
        if (_showLocalPackages)
        {
            SerializedProperty localPackagesProperty = _serializedSettings.FindProperty("localPackages");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            for (int i = 0; i < localPackagesProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = localPackagesProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(elementProperty, GUIContent.none);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    localPackagesProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                localPackagesProperty.InsertArrayElementAtIndex(localPackagesProperty.arraySize);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        _showCommonPackages = EditorGUILayout.Foldout(_showCommonPackages, "Common Packages");
        if (_showCommonPackages)
        {
            SerializedProperty commonPackagesProperty = _serializedSettings.FindProperty("commonPackages");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            for (int i = 0; i < commonPackagesProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = commonPackagesProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(elementProperty, GUIContent.none);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    commonPackagesProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                commonPackagesProperty.InsertArrayElementAtIndex(commonPackagesProperty.arraySize);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        _serializedSettings.ApplyModifiedProperties();
    }

}