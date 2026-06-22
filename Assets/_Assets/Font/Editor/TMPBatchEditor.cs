using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class TMPBatchEditor : EditorWindow
{
    [Header("Font")]
    private bool changeFont;
    private TMP_FontAsset targetFont;

    [Header("Size")]
    private bool changeFontSize;
    private float targetFontSize = 36f;

    [Header("Style")]
    private bool changeBold;
    private bool bold;

    private bool changeItalic;
    private bool italic;

    [Header("Alignment")]
    private bool changeAlignment;
    private TextAlignmentOptions alignment =
        TextAlignmentOptions.Center;

    [Header("Auto Size")]
    private bool changeAutoSize;
    private bool autoSize;

    [Header("Search")]
    private bool processScenes = true;
    private bool processPrefabs = true;

    [MenuItem("Tools/TMP Batch Editor")]
    public static void Open()
    {
        GetWindow<TMPBatchEditor>("TMP Batch Editor");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Font Settings", EditorStyles.boldLabel);

        changeFont = EditorGUILayout.ToggleLeft("Change Font", changeFont);

        if (changeFont)
        {
            targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
                "Target Font",
                targetFont,
                typeof(TMP_FontAsset),
                false);
        }

        GUILayout.Space(5);

        changeFontSize = EditorGUILayout.ToggleLeft(
            "Change Font Size",
            changeFontSize);

        if (changeFontSize)
        {
            targetFontSize =
                EditorGUILayout.FloatField("Font Size", targetFontSize);
        }

        GUILayout.Space(5);

        changeBold = EditorGUILayout.ToggleLeft(
            "Change Bold",
            changeBold);

        if (changeBold)
        {
            bold = EditorGUILayout.Toggle("Bold", bold);
        }

        GUILayout.Space(5);

        changeItalic = EditorGUILayout.ToggleLeft(
            "Change Italic",
            changeItalic);

        if (changeItalic)
        {
            italic = EditorGUILayout.Toggle("Italic", italic);
        }

        GUILayout.Space(5);

        changeAlignment = EditorGUILayout.ToggleLeft(
            "Change Alignment",
            changeAlignment);

        if (changeAlignment)
        {
            alignment =
                (TextAlignmentOptions)EditorGUILayout.EnumPopup(
                    "Alignment",
                    alignment);
        }

        GUILayout.Space(5);

        changeAutoSize = EditorGUILayout.ToggleLeft(
            "Change Auto Size",
            changeAutoSize);

        if (changeAutoSize)
        {
            autoSize = EditorGUILayout.Toggle(
                "Enable Auto Size",
                autoSize);
        }

        GUILayout.Space(15);

        EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);

        processScenes =
            EditorGUILayout.Toggle("Update Scenes", processScenes);

        processPrefabs =
            EditorGUILayout.Toggle("Update Prefabs", processPrefabs);

        GUILayout.Space(20);

        if (GUILayout.Button("Apply Changes", GUILayout.Height(40)))
        {
            ApplyChanges();
        }
    }

    private void ApplyChanges()
    {
        int totalUpdated = 0;

        if (processScenes)
            totalUpdated += UpdateScenes();

        if (processPrefabs)
            totalUpdated += UpdatePrefabs();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Finished. Updated {totalUpdated} TMP components.");
    }

    private int UpdateScenes()
    {
        int updated = 0;

        string[] sceneGuids =
            AssetDatabase.FindAssets("t:Scene");

        foreach (string guid in sceneGuids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            Scene scene =
                EditorSceneManager.OpenScene(
                    path,
                    OpenSceneMode.Single);

            TMP_Text[] texts =
                Resources.FindObjectsOfTypeAll<TMP_Text>();

            foreach (TMP_Text text in texts)
            {
                if (!text.gameObject.scene.IsValid())
                    continue;

                ApplyToText(text);

                EditorUtility.SetDirty(text);
                updated++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        return updated;
    }

    private int UpdatePrefabs()
    {
        int updated = 0;

        string[] prefabGuids =
            AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab =
                PrefabUtility.LoadPrefabContents(path);

            bool modified = false;

            TMP_Text[] texts =
                prefab.GetComponentsInChildren<TMP_Text>(true);

            foreach (TMP_Text text in texts)
            {
                ApplyToText(text);

                modified = true;
                updated++;
            }

            if (modified)
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
            }

            PrefabUtility.UnloadPrefabContents(prefab);
        }

        return updated;
    }

    private void ApplyToText(TMP_Text text)
    {
        if (changeFont && targetFont != null)
        {
            text.font = targetFont;
        }

        if (changeFontSize)
        {
            text.fontSize = targetFontSize;
        }

        if (changeAlignment)
        {
            text.alignment = alignment;
        }

        if (changeAutoSize)
        {
            text.enableAutoSizing = autoSize;
        }

        FontStyles styles = text.fontStyle;

        if (changeBold)
        {
            if (bold)
                styles |= FontStyles.Bold;
            else
                styles &= ~FontStyles.Bold;
        }

        if (changeItalic)
        {
            if (italic)
                styles |= FontStyles.Italic;
            else
                styles &= ~FontStyles.Italic;
        }

        text.fontStyle = styles;
    }
}