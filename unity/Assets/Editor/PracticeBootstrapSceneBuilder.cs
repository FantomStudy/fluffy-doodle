using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PracticeBootstrapSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Bootstrap.unity";

    [MenuItem("Tools/Practice/Rebuild Bootstrap Scene")]
    public static void RebuildBootstrapScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateLoaderRoot();
        CreateLoadingCanvas();

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateBuildSettings();
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.03f, 0.04f, 0.07f, 1f);

        cameraObject.AddComponent<AudioListener>();
    }

    private static void CreateLoaderRoot()
    {
        GameObject loaderObject = new GameObject("PracticeBootstrapLoader");
        loaderObject.AddComponent<PracticeBootstrapLoader>();
    }

    private static void CreateLoadingCanvas()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject canvasObject = new GameObject("BootstrapCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject backdropObject = new GameObject("Backdrop", typeof(RectTransform), typeof(Image));
        backdropObject.transform.SetParent(canvasObject.transform, false);
        RectTransform backdropRect = backdropObject.GetComponent<RectTransform>();
        Stretch(backdropRect);
        Image backdropImage = backdropObject.GetComponent<Image>();
        backdropImage.color = new Color(0.03f, 0.04f, 0.07f, 1f);

        GameObject textObject = new GameObject("LoadingText", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(backdropObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(900f, 120f);

        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.fontSize = 42;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = "Загрузка уровня...";
        text.supportRichText = false;
    }

    private static void UpdateBuildSettings()
    {
        string[] scenePaths =
        {
            "Assets/Scenes/Bootstrap.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity",
            "Assets/Scenes/Level4.unity",
        };

        var scenes = new List<EditorBuildSettingsScene>();
        foreach (string scenePath in scenePaths)
        {
            if (!System.IO.File.Exists(scenePath))
            {
                continue;
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
