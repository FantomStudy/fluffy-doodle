using System;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class Level2SceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Level2.unity";
    private const string BlackMaterialPath = "Assets/Materials/BlackOutside.mat";
    private const string BridgeMaterialPath = "Assets/Materials/BridgeGlow.mat";
    private const string TerminalMaterialPath = "Assets/Materials/TerminalAccent.mat";
    private const string FontAssetPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

    [MenuItem("Tools/Build Level2 Prototype")]
    public static void BuildLevel2()
    {
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        if (sceneAsset == null)
        {
            throw new InvalidOperationException("Level2 scene was not found.");
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        BuildOpenScene();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Level2 prototype scene rebuilt.");
    }

    private static void BuildOpenScene()
    {
        Material blackMaterial = LoadRequiredAsset<Material>(BlackMaterialPath);
        Material bridgeMaterial = LoadRequiredAsset<Material>(BridgeMaterialPath);
        Material terminalMaterial = LoadRequiredAsset<Material>(TerminalMaterialPath);
        TMP_FontAsset fontAsset = LoadRequiredAsset<TMP_FontAsset>(FontAssetPath);

        ThirdPersonController playerController = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>(FindObjectsInactive.Include);
        if (playerController == null)
        {
            throw new InvalidOperationException("A ThirdPersonController player is required in Level2.");
        }

        GameObject playerRoot = playerController.transform.root.gameObject;
        if (playerRoot.transform.parent != null)
        {
            playerRoot.transform.SetParent(null);
        }
        playerRoot.transform.SetPositionAndRotation(new Vector3(0f, 1.5f, -8.5f), Quaternion.identity);

        GameObject fadeCanvasObject = FindSceneObject("FadeCanvas");
        if (fadeCanvasObject == null)
        {
            throw new InvalidOperationException("FadeCanvas was not found in Level2.");
        }

        if (fadeCanvasObject.transform.parent != null)
        {
            fadeCanvasObject.transform.SetParent(null);
        }
        ScreenFadePlayerLock screenFadePlayerLock = fadeCanvasObject.GetComponent<ScreenFadePlayerLock>();
        if (screenFadePlayerLock == null)
        {
            throw new InvalidOperationException("FadeCanvas must contain ScreenFadePlayerLock.");
        }

        SetObjectReference(screenFadePlayerLock, "playerRoot", playerRoot);

        RemoveSceneObjects(
            "Level2Prototype",
            "Room1",
            "Room2",
            "Room3",
            "Room4",
            "Room5",
            "Global Volume",
            "TerminalPrompt",
            "TerminalWindow",
            "ResultPopup");

        EnsureEventSystem();

        GameObject prototypeRoot = new GameObject("Level2Prototype");

        CreateCube("StartFloor", prototypeRoot.transform, new Vector3(0f, -0.25f, -6f), new Vector3(7f, 0.5f, 10f), blackMaterial);
        CreateCube("Section2BaseFloor", prototypeRoot.transform, new Vector3(0f, -0.25f, 6.5f), new Vector3(7f, 0.5f, 6f), blackMaterial);
        CreateCube("UpperWalkway", prototypeRoot.transform, new Vector3(0f, 1.75f, 15.25f), new Vector3(5f, 0.5f, 7f), blackMaterial);
        CreateCube("ExitWalkway", prototypeRoot.transform, new Vector3(0f, 1.75f, 26f), new Vector3(5f, 0.5f, 5f), blackMaterial);
        CreateCube("AbyssCore", prototypeRoot.transform, new Vector3(0f, -3f, 11f), new Vector3(7f, 5f, 24f), blackMaterial);
        CreateCube("LeftWall", prototypeRoot.transform, new Vector3(-4.25f, 1.25f, 9.25f), new Vector3(0.5f, 3f, 40.5f), blackMaterial);
        CreateCube("RightWall", prototypeRoot.transform, new Vector3(4.25f, 1.25f, 9.25f), new Vector3(0.5f, 3f, 40.5f), blackMaterial);
        CreateCube("BackWall", prototypeRoot.transform, new Vector3(0f, 1.25f, -11.25f), new Vector3(8.5f, 3f, 0.5f), blackMaterial);
        CreateCube("EndWall", prototypeRoot.transform, new Vector3(0f, 1.25f, 29.75f), new Vector3(8.5f, 3f, 0.5f), blackMaterial);
        CreateCube("UpperSupport", prototypeRoot.transform, new Vector3(0f, 0.85f, 15.25f), new Vector3(1f, 1.7f, 1f), blackMaterial);
        CreateCube("RotationSupport", prototypeRoot.transform, new Vector3(0f, 0.8f, 21f), new Vector3(1.1f, 1.6f, 1.1f), blackMaterial);

        BridgeController bridgeController = CreateBridge(prototypeRoot.transform, bridgeMaterial);
        HeightPlatformController heightPlatformController = CreateHeightPlatform(prototypeRoot.transform, terminalMaterial);
        RotationPlatformController rotationPlatformController = CreateRotationPlatform(prototypeRoot.transform, bridgeMaterial);
        CreateExitFrame(prototypeRoot.transform, terminalMaterial);

        RectTransform fadeCanvasTransform = fadeCanvasObject.GetComponent<RectTransform>();
        GameObject promptRoot = CreateTerminalPrompt(fadeCanvasTransform, fontAsset);
        TerminalUiBundle terminalUiBundle = CreateTerminalWindow(fadeCanvasTransform, fontAsset);
        ResultPopupBundle resultPopupBundle = CreateResultPopup(fadeCanvasTransform, fontAsset);

        CreateTerminal(
            prototypeRoot.transform,
            "BridgeTerminal",
            new Vector3(-2.45f, 0f, -7f),
            LevelVariableType.BridgeLength,
            terminalUiBundle.TerminalUI,
            promptRoot,
            blackMaterial,
            terminalMaterial,
            bridgeMaterial);

        CreateTerminal(
            prototypeRoot.transform,
            "HeightTerminal",
            new Vector3(2.45f, 0f, 6.9f),
            LevelVariableType.PlatformHeight,
            terminalUiBundle.TerminalUI,
            promptRoot,
            blackMaterial,
            terminalMaterial,
            bridgeMaterial);

        CreateTerminal(
            prototypeRoot.transform,
            "AngleTerminal",
            new Vector3(-2.45f, 2f, 16f),
            LevelVariableType.PlatformAngle,
            terminalUiBundle.TerminalUI,
            promptRoot,
            blackMaterial,
            terminalMaterial,
            bridgeMaterial);

        VariableLevelController levelController = CreateLevelController(
            prototypeRoot.transform,
            bridgeController,
            heightPlatformController,
            rotationPlatformController,
            terminalUiBundle.TerminalUI,
            resultPopupBundle.ResultPopupUI);

        ConfigureTerminalUI(terminalUiBundle, levelController, screenFadePlayerLock);
        ConfigureResultPopup(resultPopupBundle, screenFadePlayerLock);
        CreateExitZone(prototypeRoot.transform, levelController);
    }

    private static VariableLevelController CreateLevelController(
        Transform parent,
        BridgeController bridgeController,
        HeightPlatformController heightPlatformController,
        RotationPlatformController rotationPlatformController,
        TerminalUI terminalUI,
        ResultPopupUI resultPopupUI)
    {
        GameObject controllerObject = new GameObject("VariableLevelController");
        controllerObject.transform.SetParent(parent, false);
        VariableLevelController levelController = controllerObject.AddComponent<VariableLevelController>();

        SetObjectReference(levelController, "bridgeController", bridgeController);
        SetObjectReference(levelController, "heightPlatformController", heightPlatformController);
        SetObjectReference(levelController, "rotationPlatformController", rotationPlatformController);
        SetObjectReference(levelController, "terminalUI", terminalUI);
        SetObjectReference(levelController, "resultPopupUI", resultPopupUI);
        SetInteger(levelController, "defaultBridgeLength", 1);
        SetInteger(levelController, "defaultPlatformHeight", 1);
        SetInteger(levelController, "defaultPlatformAngle", 0);
        return levelController;
    }

    private static BridgeController CreateBridge(Transform parent, Material bridgeMaterial)
    {
        GameObject bridgeRoot = new GameObject("BridgeRoot");
        bridgeRoot.transform.SetParent(parent, false);
        bridgeRoot.transform.localPosition = new Vector3(0f, 0.05f, -1f);

        GameObject bridgeVisual = CreateCube(
            "BridgeVisual",
            bridgeRoot.transform,
            Vector3.zero,
            new Vector3(1.8f, 0.1f, 1.4f),
            bridgeMaterial);

        BridgeController bridgeController = bridgeRoot.AddComponent<BridgeController>();
        SetObjectReference(bridgeController, "bridgeVisual", bridgeVisual.transform);
        SetFloat(bridgeController, "lengthForOne", 1.4f);
        SetFloat(bridgeController, "lengthForTwo", 2.8f);
        SetFloat(bridgeController, "lengthForThree", 4.6f);
        SetFloat(bridgeController, "animationSpeed", 4.5f);
        return bridgeController;
    }

    private static HeightPlatformController CreateHeightPlatform(Transform parent, Material platformMaterial)
    {
        GameObject platformRoot = new GameObject("HeightPlatformRoot");
        platformRoot.transform.SetParent(parent, false);
        platformRoot.transform.localPosition = new Vector3(0f, 0f, 10.8f);

        GameObject platformVisual = CreateCube(
            "HeightPlatformVisual",
            platformRoot.transform,
            new Vector3(0f, 0.2f, 0f),
            new Vector3(1.8f, 0.4f, 1.6f),
            platformMaterial);

        HeightPlatformController controller = platformRoot.AddComponent<HeightPlatformController>();
        SetObjectReference(controller, "platformTransform", platformVisual.transform);
        SetFloat(controller, "heightForOne", 0.2f);
        SetFloat(controller, "heightForTwo", 0.45f);
        SetFloat(controller, "heightForThree", 0.95f);
        SetFloat(controller, "moveSpeed", 2.6f);
        return controller;
    }

    private static RotationPlatformController CreateRotationPlatform(Transform parent, Material bridgeMaterial)
    {
        GameObject platformRoot = new GameObject("RotationPlatformRoot");
        platformRoot.transform.SetParent(parent, false);
        platformRoot.transform.localPosition = new Vector3(0f, 1.825f, 21f);

        GameObject platformVisual = CreateCube(
            "RotationPlatformVisual",
            platformRoot.transform,
            Vector3.zero,
            new Vector3(5f, 0.35f, 1.6f),
            bridgeMaterial);

        RotationPlatformController controller = platformRoot.AddComponent<RotationPlatformController>();
        SetObjectReference(controller, "platformTransform", platformVisual.transform);
        SetFloat(controller, "angleForZero", 0f);
        SetFloat(controller, "angleForFortyFive", 45f);
        SetFloat(controller, "angleForNinety", 90f);
        SetFloat(controller, "rotationSpeed", 120f);
        return controller;
    }

    private static void CreateExitFrame(Transform parent, Material accentMaterial)
    {
        CreateCube("ExitFrameLeft", parent, new Vector3(-1.55f, 3f, 27.4f), new Vector3(0.35f, 2f, 0.45f), accentMaterial);
        CreateCube("ExitFrameRight", parent, new Vector3(1.55f, 3f, 27.4f), new Vector3(0.35f, 2f, 0.45f), accentMaterial);
        CreateCube("ExitFrameTop", parent, new Vector3(0f, 3.95f, 27.4f), new Vector3(3.45f, 0.3f, 0.45f), accentMaterial);
    }

    private static void CreateExitZone(Transform parent, VariableLevelController levelController)
    {
        GameObject exitZoneObject = new GameObject("ExitZone");
        exitZoneObject.transform.SetParent(parent, false);
        exitZoneObject.transform.localPosition = new Vector3(0f, 2f, 27.6f);

        BoxCollider exitCollider = exitZoneObject.AddComponent<BoxCollider>();
        exitCollider.isTrigger = true;
        exitCollider.size = new Vector3(3f, 2.4f, 1.5f);
        exitCollider.center = new Vector3(0f, 1f, 0f);

        ExitZone exitZone = exitZoneObject.AddComponent<ExitZone>();
        SetObjectReference(exitZone, "levelController", levelController);
    }

    private static TerminalInteraction CreateTerminal(
        Transform parent,
        string name,
        Vector3 position,
        LevelVariableType variableType,
        TerminalUI terminalUI,
        GameObject promptRoot,
        Material baseMaterial,
        Material screenMaterial,
        Material glowMaterial)
    {
        GameObject terminalRoot = new GameObject(name);
        terminalRoot.transform.SetParent(parent, false);
        terminalRoot.transform.localPosition = position;

        BoxCollider triggerCollider = terminalRoot.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(2.6f, 2.2f, 2.6f);
        triggerCollider.center = new Vector3(0f, 1f, 0.25f);

        TerminalInteraction terminalInteraction = terminalRoot.AddComponent<TerminalInteraction>();
        SetObjectReference(terminalInteraction, "terminalUI", terminalUI);
        SetObjectReference(terminalInteraction, "promptRoot", promptRoot);
        SetEnum(terminalInteraction, "variableType", (int)variableType);

        CreateCube("TerminalBase", terminalRoot.transform, new Vector3(0f, 0.65f, 0f), new Vector3(0.7f, 1.3f, 0.7f), baseMaterial);

        GameObject terminalScreen = CreateCube(
            "TerminalScreen",
            terminalRoot.transform,
            new Vector3(0f, 1.45f, 0.2f),
            new Vector3(1f, 0.55f, 0.16f),
            screenMaterial);
        UnityEngine.Object.DestroyImmediate(terminalScreen.GetComponent<BoxCollider>());

        GameObject terminalGlow = CreateCube(
            "TerminalGlow",
            terminalRoot.transform,
            new Vector3(0f, 1.45f, 0.29f),
            new Vector3(0.8f, 0.35f, 0.05f),
            glowMaterial);
        UnityEngine.Object.DestroyImmediate(terminalGlow.GetComponent<BoxCollider>());

        return terminalInteraction;
    }

    private static GameObject CreateTerminalPrompt(Transform parent, TMP_FontAsset fontAsset)
    {
        RectTransform promptRoot = CreatePanel(
            "TerminalPrompt",
            parent,
            new Vector2(0.36f, 0.06f),
            new Vector2(0.64f, 0.12f),
            new Color(0.04f, 0.08f, 0.11f, 0.92f));

        CreateText(
            "PromptText",
            promptRoot,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            Vector2.zero,
            Vector2.zero,
            "Press E to interact",
            fontAsset,
            24,
            new Color(0.72f, 0.97f, 1f, 1f),
            TextAlignmentOptions.Center);

        promptRoot.gameObject.SetActive(false);
        return promptRoot.gameObject;
    }

    private static TerminalUiBundle CreateTerminalWindow(Transform parent, TMP_FontAsset fontAsset)
    {
        RectTransform windowRoot = CreatePanel(
            "TerminalWindow",
            parent,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Color(0.04f, 0.08f, 0.11f, 0.96f),
            new Vector2(420f, 220f));

        CanvasGroup canvasGroup = windowRoot.gameObject.AddComponent<CanvasGroup>();
        TerminalUI terminalUI = windowRoot.gameObject.AddComponent<TerminalUI>();

        CreatePanel(
            "HeaderAccent",
            windowRoot,
            new Vector2(0.04f, 0.88f),
            new Vector2(0.96f, 0.94f),
            new Color(0.15f, 0.75f, 0.93f, 0.95f));

        CreateText(
            "HeaderText",
            windowRoot,
            new Vector2(0.08f, 0.74f),
            new Vector2(0.62f, 0.88f),
            Vector2.zero,
            Vector2.zero,
            "VARIABLE TERMINAL",
            fontAsset,
            20,
            new Color(0.64f, 0.96f, 1f, 1f),
            TextAlignmentOptions.MidlineLeft);

        TextMeshProUGUI expressionText = CreateText(
            "ExpressionText",
            windowRoot,
            new Vector2(0.08f, 0.52f),
            new Vector2(0.62f, 0.74f),
            Vector2.zero,
            Vector2.zero,
            "bridgeLength =",
            fontAsset,
            30,
            new Color(0.76f, 0.95f, 1f, 1f),
            TextAlignmentOptions.MidlineLeft);

        Button selectedValueButton = CreateButton(
            "SelectedValueButton",
            windowRoot,
            new Vector2(0.66f, 0.54f),
            new Vector2(0.9f, 0.82f),
            new Color(0.1f, 0.22f, 0.28f, 1f),
            "1",
            fontAsset,
            30,
            new Color(0.65f, 0.96f, 1f, 1f),
            out TextMeshProUGUI selectedValueText);

        RectTransform optionListRoot = CreatePanel(
            "OptionListRoot",
            windowRoot,
            new Vector2(0.66f, 0.08f),
            new Vector2(0.9f, 0.5f),
            new Color(0.06f, 0.11f, 0.14f, 0.98f));
        optionListRoot.gameObject.SetActive(false);

        Button optionOneButton = CreateButton(
            "OptionOneButton",
            optionListRoot,
            new Vector2(0.08f, 0.69f),
            new Vector2(0.92f, 0.95f),
            new Color(0.1f, 0.22f, 0.28f, 1f),
            "1",
            fontAsset,
            24,
            new Color(0.65f, 0.96f, 1f, 1f),
            out _);

        Button optionTwoButton = CreateButton(
            "OptionTwoButton",
            optionListRoot,
            new Vector2(0.08f, 0.37f),
            new Vector2(0.92f, 0.63f),
            new Color(0.1f, 0.22f, 0.28f, 1f),
            "2",
            fontAsset,
            24,
            new Color(0.65f, 0.96f, 1f, 1f),
            out _);

        Button optionThreeButton = CreateButton(
            "OptionThreeButton",
            optionListRoot,
            new Vector2(0.08f, 0.05f),
            new Vector2(0.92f, 0.31f),
            new Color(0.1f, 0.22f, 0.28f, 1f),
            "3",
            fontAsset,
            24,
            new Color(0.65f, 0.96f, 1f, 1f),
            out _);

        return new TerminalUiBundle
        {
            TerminalUI = terminalUI,
            CanvasGroup = canvasGroup,
            ExpressionText = expressionText,
            SelectedValueText = selectedValueText,
            OptionListRoot = optionListRoot.gameObject,
            SelectedValueButton = selectedValueButton,
            OptionOneButton = optionOneButton,
            OptionTwoButton = optionTwoButton,
            OptionThreeButton = optionThreeButton,
        };
    }

    private static ResultPopupBundle CreateResultPopup(Transform parent, TMP_FontAsset fontAsset)
    {
        RectTransform popupRoot = CreatePanel(
            "ResultPopup",
            parent,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Color(0.03f, 0.07f, 0.1f, 0.98f),
            new Vector2(520f, 300f));

        CanvasGroup canvasGroup = popupRoot.gameObject.AddComponent<CanvasGroup>();
        ResultPopupUI resultPopupUI = popupRoot.gameObject.AddComponent<ResultPopupUI>();

        CreatePanel(
            "PopupAccent",
            popupRoot,
            new Vector2(0.06f, 0.88f),
            new Vector2(0.94f, 0.94f),
            new Color(0.15f, 0.75f, 0.93f, 0.95f));

        CreateText(
            "ResultTitle",
            popupRoot,
            new Vector2(0.08f, 0.64f),
            new Vector2(0.92f, 0.86f),
            Vector2.zero,
            Vector2.zero,
            "Variables Synchronized",
            fontAsset,
            34,
            new Color(0.82f, 0.92f, 0.96f, 1f),
            TextAlignmentOptions.Center);

        CreateText(
            "ResultSubtitle",
            popupRoot,
            new Vector2(0.1f, 0.42f),
            new Vector2(0.9f, 0.62f),
            Vector2.zero,
            Vector2.zero,
            "Bridge, height and angle now match the world.",
            fontAsset,
            22,
            new Color(0.66f, 0.96f, 1f, 1f),
            TextAlignmentOptions.Center);

        Button restartButton = CreateButton(
            "RestartButton",
            popupRoot,
            new Vector2(0.14f, 0.12f),
            new Vector2(0.44f, 0.3f),
            new Color(0.11f, 0.24f, 0.3f, 1f),
            "Restart",
            fontAsset,
            24,
            new Color(0.66f, 0.96f, 1f, 1f),
            out _);

        Button backButton = CreateButton(
            "BackButton",
            popupRoot,
            new Vector2(0.56f, 0.12f),
            new Vector2(0.86f, 0.3f),
            new Color(0.11f, 0.24f, 0.3f, 1f),
            "Back",
            fontAsset,
            24,
            new Color(0.66f, 0.96f, 1f, 1f),
            out _);

        return new ResultPopupBundle
        {
            ResultPopupUI = resultPopupUI,
            CanvasGroup = canvasGroup,
            RestartButton = restartButton,
            BackButton = backButton,
        };
    }

    private static void ConfigureTerminalUI(TerminalUiBundle bundle, VariableLevelController levelController, ScreenFadePlayerLock screenFadePlayerLock)
    {
        SetObjectReference(bundle.TerminalUI, "levelController", levelController);
        SetObjectReference(bundle.TerminalUI, "screenFadePlayerLock", screenFadePlayerLock);
        SetObjectReference(bundle.TerminalUI, "canvasGroup", bundle.CanvasGroup);
        SetObjectReference(bundle.TerminalUI, "expressionText", bundle.ExpressionText);
        SetObjectReference(bundle.TerminalUI, "selectedValueText", bundle.SelectedValueText);
        SetObjectReference(bundle.TerminalUI, "optionListRoot", bundle.OptionListRoot);
        SetObjectReference(bundle.TerminalUI, "selectedValueButton", bundle.SelectedValueButton);
        SetObjectReference(bundle.TerminalUI, "optionOneButton", bundle.OptionOneButton);
        SetObjectReference(bundle.TerminalUI, "optionTwoButton", bundle.OptionTwoButton);
        SetObjectReference(bundle.TerminalUI, "optionThreeButton", bundle.OptionThreeButton);
        SetFloat(bundle.TerminalUI, "fadeDuration", 0.18f);
    }

    private static void ConfigureResultPopup(ResultPopupBundle bundle, ScreenFadePlayerLock screenFadePlayerLock)
    {
        SetObjectReference(bundle.ResultPopupUI, "screenFadePlayerLock", screenFadePlayerLock);
        SetObjectReference(bundle.ResultPopupUI, "canvasGroup", bundle.CanvasGroup);
        SetObjectReference(bundle.ResultPopupUI, "restartButton", bundle.RestartButton);
        SetObjectReference(bundle.ResultPopupUI, "backButton", bundle.BackButton);
        SetFloat(bundle.ResultPopupUI, "fadeDuration", 0.25f);
        SetString(bundle.ResultPopupUI, "restartSceneName", "Level2");
        SetString(bundle.ResultPopupUI, "backSceneName", "Level1");
    }

    private static void EnsureEventSystem()
    {
        EventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

        InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
        {
            inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = localScale;

        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        return cube;
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
        panelObject.layer = LayerMask.NameToLayer("UI");
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
        textObject.layer = LayerMask.NameToLayer("UI");
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        ConfigureRectTransform(rectTransform, anchorMin, anchorMax, sizeDelta, anchoredPosition, new Vector2(0.5f, 0.5f));

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.font = fontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
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
        out TextMeshProUGUI labelText)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.layer = LayerMask.NameToLayer("UI");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        ConfigureRectTransform(rectTransform, anchorMin, anchorMax, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

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
        Navigation navigation = button.navigation;
        navigation.mode = Navigation.Mode.None;
        button.navigation = navigation;

        labelText = CreateText(
            name.Replace("Button", "Text"),
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

    private static T LoadRequiredAsset<T>(string path) where T : UnityEngine.Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            throw new InvalidOperationException($"Required asset was not found: {path}");
        }

        return asset;
    }

    private static void RemoveSceneObjects(params string[] names)
    {
        HashSet<string> nameSet = new HashSet<string>(names);
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (Transform transform in transforms)
        {
            if (nameSet.Contains(transform.name))
            {
                objectsToRemove.Add(transform.gameObject);
            }
        }

        foreach (GameObject gameObject in objectsToRemove)
        {
            UnityEngine.Object.DestroyImmediate(gameObject);
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

    private static void SetInteger(UnityEngine.Object target, string propertyName, int value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).intValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetEnum(UnityEngine.Object target, string propertyName, int value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).enumValueIndex = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetString(UnityEngine.Object target, string propertyName, string value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).stringValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private sealed class TerminalUiBundle
    {
        public TerminalUI TerminalUI;
        public CanvasGroup CanvasGroup;
        public TextMeshProUGUI ExpressionText;
        public TextMeshProUGUI SelectedValueText;
        public GameObject OptionListRoot;
        public Button SelectedValueButton;
        public Button OptionOneButton;
        public Button OptionTwoButton;
        public Button OptionThreeButton;
    }

    private sealed class ResultPopupBundle
    {
        public ResultPopupUI ResultPopupUI;
        public CanvasGroup CanvasGroup;
        public Button RestartButton;
        public Button BackButton;
    }
}
