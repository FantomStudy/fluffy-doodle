using System;
using StarterAssets;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Level3SceneUiTools
{
    private const string FontAssetPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

    [MenuItem("Tools/Level3/Add Puzzle UI To Current Scene")]
    public static void AddPuzzleUiToCurrentScene()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (fontAsset == null)
        {
            throw new InvalidOperationException("TMP font asset for Level3 UI was not found.");
        }

        GameObject fadeCanvasObject = FindSceneObject("FadeCanvas");
        if (fadeCanvasObject == null)
        {
            throw new InvalidOperationException("FadeCanvas was not found in the open scene.");
        }

        ScreenFadePlayerLock screenFadePlayerLock = fadeCanvasObject.GetComponent<ScreenFadePlayerLock>();
        EnsureEventSystem();
        CleanupPreviousUi(fadeCanvasObject.transform);

        PromptBundle promptBundle = CreateInteractionPrompt(fadeCanvasObject.transform, fontAsset);
        HelpUiBundle helpBundle = CreateHelpUi(fadeCanvasObject.transform, fontAsset);

        ConfigureHelpUi(helpBundle, screenFadePlayerLock);
        ConfigurePlayerInteractor(promptBundle);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Level3 puzzle UI added to the current scene.");
    }

    private static void ConfigurePlayerInteractor(PromptBundle promptBundle)
    {
        ThirdPersonController playerController = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>(FindObjectsInactive.Include);
        if (playerController == null)
        {
            return;
        }

        GameObject playerRoot = playerController.transform.root.gameObject;
        PlayerInteractor interactor = playerRoot.GetComponent<PlayerInteractor>();
        if (interactor == null)
        {
            interactor = playerRoot.AddComponent<PlayerInteractor>();
        }

        Camera interactionCamera = Camera.main;
        if (interactionCamera == null)
        {
            interactionCamera = UnityEngine.Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        }

        SetObjectReference(interactor, "interactionCamera", interactionCamera);
        SetFloat(interactor, "interactionDistance", 4.5f);
        SetLayerMask(interactor, "interactionMask", ~0);
        SetObjectReference(interactor, "promptRoot", promptBundle.Root);
        SetObjectReference(interactor, "promptText", promptBundle.Text);
        SetString(interactor, "defaultPrompt", "Нажмите E для взаимодействия");
    }

    private static void ConfigureHelpUi(HelpUiBundle helpBundle, ScreenFadePlayerLock screenFadePlayerLock)
    {
        SetObjectReference(helpBundle.HelpUI, "screenFadePlayerLock", screenFadePlayerLock);
        SetObjectReference(helpBundle.HelpUI, "openButton", helpBundle.OpenButton);
        SetObjectReference(helpBundle.HelpUI, "closeButton", helpBundle.CloseButton);
        SetObjectReference(helpBundle.HelpUI, "panelCanvasGroup", helpBundle.PanelCanvasGroup);
        SetObjectReference(helpBundle.HelpUI, "titleText", helpBundle.TitleText);
        SetObjectReference(helpBundle.HelpUI, "bodyText", helpBundle.BodyText);
        SetString(helpBundle.HelpUI, "title", "КАК ИГРАТЬ");
        SetString(
            helpBundle.HelpUI,
            "body",
            "В каждой комнате выбери один или несколько блоков.\n\n" +
            "Нажми E у терминала, чтобы проверить условие.\n\n" +
            "Дверь откроется только тогда, когда выражение истинно.\n\n" +
            "Дальше условия усложняются: сначала простой if, потом OR, а затем комбинации AND + OR.\n\n" +
            "Если условие не выполнено, измени выбор и попробуй снова.");
    }

    private static PromptBundle CreateInteractionPrompt(Transform parent, TMP_FontAsset fontAsset)
    {
        RectTransform promptRoot = CreatePanel(
            "InteractionPrompt",
            parent,
            new Vector2(0.36f, 0.06f),
            new Vector2(0.64f, 0.12f),
            new Color(0.97f, 0.97f, 0.99f, 0.96f));

        TextMeshProUGUI promptText = CreateText(
            "PromptText",
            promptRoot,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            Vector2.zero,
            Vector2.zero,
            "Нажмите E для взаимодействия",
            fontAsset,
            24,
            new Color(0.18f, 0.17f, 0.24f, 1f),
            TextAlignmentOptions.Center);

        promptRoot.gameObject.SetActive(false);
        return new PromptBundle(promptRoot.gameObject, promptText);
    }

    private static HelpUiBundle CreateHelpUi(Transform parent, TMP_FontAsset fontAsset)
    {
        GameObject helpRoot = new GameObject("Level3HelpUIRoot");
        helpRoot.layer = GetUiLayer();
        helpRoot.transform.SetParent(parent, false);

        Level3HelpUI helpUI = helpRoot.AddComponent<Level3HelpUI>();

        Button openButton = CreateButton(
            "HelpButton",
            helpRoot.transform,
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Color(0.47f, 0.31f, 0.89f, 1f),
            "?",
            fontAsset,
            28,
            Color.white,
            out _,
            new Vector2(56f, 56f),
            new Vector2(-42f, -42f),
            new Vector2(1f, 1f));

        RectTransform helpPanel = CreatePanel(
            "HelpPanel",
            helpRoot.transform,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Color(0.975f, 0.974f, 0.988f, 0.985f),
            new Vector2(640f, 360f));
        CanvasGroup panelCanvasGroup = helpPanel.gameObject.AddComponent<CanvasGroup>();
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.blocksRaycasts = false;
        panelCanvasGroup.interactable = false;

        CreatePanel(
            "HelpAccent",
            helpPanel,
            new Vector2(0.06f, 0.89f),
            new Vector2(0.94f, 0.94f),
            new Color(0.92f, 0.9f, 0.99f, 1f));

        TextMeshProUGUI titleText = CreateText(
            "HelpTitle",
            helpPanel,
            new Vector2(0.08f, 0.72f),
            new Vector2(0.78f, 0.88f),
            Vector2.zero,
            Vector2.zero,
            "КАК ИГРАТЬ",
            fontAsset,
            28,
            new Color(0.18f, 0.17f, 0.24f, 1f),
            TextAlignmentOptions.MidlineLeft);

        TextMeshProUGUI bodyText = CreateText(
            "HelpBody",
            helpPanel,
            new Vector2(0.08f, 0.18f),
            new Vector2(0.92f, 0.66f),
            Vector2.zero,
            Vector2.zero,
            string.Empty,
            fontAsset,
            22,
            new Color(0.48f, 0.49f, 0.57f, 1f),
            TextAlignmentOptions.TopLeft);

        Button closeButton = CreateButton(
            "CloseHelpButton",
            helpPanel,
            new Vector2(0.72f, 0.72f),
            new Vector2(0.9f, 0.88f),
            new Color(0.31f, 0.76f, 0.36f, 1f),
            "Закрыть",
            fontAsset,
            22,
            Color.white,
            out _);

        return new HelpUiBundle(helpUI, openButton, closeButton, panelCanvasGroup, titleText, bodyText);
    }

    private static void CleanupPreviousUi(Transform fadeCanvasTransform)
    {
        RemoveChildIfExists(fadeCanvasTransform, "InteractionPrompt");
        RemoveChildIfExists(fadeCanvasTransform, "Level3HelpUIRoot");
    }

    private static void RemoveChildIfExists(Transform parent, string childName)
    {
        Transform existingChild = parent.Find(childName);
        if (existingChild != null)
        {
            UnityEngine.Object.DestroyImmediate(existingChild.gameObject);
        }
    }

    private static void EnsureEventSystem()
    {
        EventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

        UnityEngine.InputSystem.UI.InputSystemUIInputModule inputModule =
            eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (inputModule == null)
        {
            inputModule = eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }
    }

    private static GameObject FindSceneObject(string name)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform transform in transforms)
        {
            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }

        return null;
    }

    private static RectTransform CreatePanel(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Color color,
        Vector2? sizeDelta = null)
    {
        GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        panelObject.layer = GetUiLayer();
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        ConfigureRectTransform(rectTransform, anchorMin, anchorMax, sizeDelta ?? Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        return rectTransform;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 sizeDelta,
        Vector2 anchoredPosition,
        string content,
        TMP_FontAsset fontAsset,
        int fontSize,
        Color color,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = GetUiLayer();
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        ConfigureRectTransform(rectTransform, anchorMin, anchorMax, sizeDelta, anchoredPosition, new Vector2(0.5f, 0.5f));

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.font = fontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private static Button CreateButton(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Color backgroundColor,
        string label,
        TMP_FontAsset fontAsset,
        int fontSize,
        Color textColor,
        out TextMeshProUGUI labelText,
        Vector2? sizeDelta = null,
        Vector2? anchoredPosition = null,
        Vector2? pivot = null)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.layer = GetUiLayer();
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        ConfigureRectTransform(
            rectTransform,
            anchorMin,
            anchorMax,
            sizeDelta ?? Vector2.zero,
            anchoredPosition ?? Vector2.zero,
            pivot ?? new Vector2(0.5f, 0.5f));

        Image image = buttonObject.GetComponent<Image>();
        image.color = backgroundColor;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
        colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
        colors.selectedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
        colors.disabledColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        labelText = CreateText(
            name + "Text",
            buttonObject.transform,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            Vector2.zero,
            Vector2.zero,
            label,
            fontAsset,
            fontSize,
            textColor,
            TextAlignmentOptions.Center);

        return button;
    }

    private static void ConfigureRectTransform(
        RectTransform rectTransform,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 sizeDelta,
        Vector2 anchoredPosition,
        Vector2 pivot)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.pivot = pivot;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    private static int GetUiLayer()
    {
        int layer = LayerMask.NameToLayer("UI");
        return layer < 0 ? 5 : layer;
    }

    private static void SetObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(UnityEngine.Object target, string propertyName, float value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).floatValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetString(UnityEngine.Object target, string propertyName, string value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).stringValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetLayerMask(UnityEngine.Object target, string propertyName, int value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).intValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private readonly struct PromptBundle
    {
        public PromptBundle(GameObject root, TMP_Text text)
        {
            Root = root;
            Text = text;
        }

        public GameObject Root { get; }
        public TMP_Text Text { get; }
    }

    private readonly struct HelpUiBundle
    {
        public HelpUiBundle(Level3HelpUI helpUI, Button openButton, Button closeButton, CanvasGroup panelCanvasGroup, TMP_Text titleText, TMP_Text bodyText)
        {
            HelpUI = helpUI;
            OpenButton = openButton;
            CloseButton = closeButton;
            PanelCanvasGroup = panelCanvasGroup;
            TitleText = titleText;
            BodyText = bodyText;
        }

        public Level3HelpUI HelpUI { get; }
        public Button OpenButton { get; }
        public Button CloseButton { get; }
        public CanvasGroup PanelCanvasGroup { get; }
        public TMP_Text TitleText { get; }
        public TMP_Text BodyText { get; }
    }
}
