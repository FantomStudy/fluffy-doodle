using System;
using StarterAssets;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Level4SceneAssembler
{
    private const string ScenePath = "Assets/Scenes/Level4.unity";
    private const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";
    private const string FontAssetPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
    private const string MaterialFolder = "Assets/Materials/CyclesLevel";
    private const string StructureMaterialPath = MaterialFolder + "/CyclesStructure.mat";
    private const string PanelMaterialPath = MaterialFolder + "/CyclesPanel.mat";
    private const string GlowMaterialPath = MaterialFolder + "/CyclesGlow.mat";
    private const string BridgeMaterialPath = MaterialFolder + "/CyclesBridge.mat";
    private const string ScreenIdleMaterialPath = MaterialFolder + "/CyclesScreenIdle.mat";
    private const string ScreenRunningMaterialPath = MaterialFolder + "/CyclesScreenRunning.mat";
    private const string ScreenSuccessMaterialPath = MaterialFolder + "/CyclesScreenSuccess.mat";
    private const string ScreenErrorMaterialPath = MaterialFolder + "/CyclesScreenError.mat";
    private const string DarkBackdropMaterialPath = MaterialFolder + "/CyclesBackdrop.mat";
    private const float CorridorHalfWidth = 4.4f;

    public static void BuildLevel4()
    {
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        if (sceneAsset != null)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }
        else
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        BuildOpenScene();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Level4 cycles scene assembled.");
    }

    private static void BuildOpenScene()
    {
        TMP_FontAsset fontAsset = LoadRequiredAsset<TMP_FontAsset>(FontAssetPath);
        MaterialPalette materials = EnsureMaterials();

        GameObject playerRoot = EnsurePlayerRoot();
        GameObject fadeCanvasObject = EnsureFadeCanvas();
        ScreenFadePlayerLock screenFadePlayerLock = fadeCanvasObject.GetComponent<ScreenFadePlayerLock>();
        if (screenFadePlayerLock == null)
        {
            screenFadePlayerLock = fadeCanvasObject.AddComponent<ScreenFadePlayerLock>();
        }

        RemoveOldRoots(playerRoot, fadeCanvasObject);
        EnsureDirectionalLight();
        EnsureEventSystem();

        RectTransform fadeCanvasTransform = fadeCanvasObject.GetComponent<RectTransform>();
        if (fadeCanvasTransform == null)
        {
            throw new InvalidOperationException("FadeCanvas must contain a RectTransform.");
        }

        CanvasGroup fadeOverlayGroup = EnsureFadeOverlay(fadeCanvasTransform);
        CleanupFadeCanvas(fadeCanvasTransform);

        SetObjectReference(screenFadePlayerLock, "fadeCanvasGroup", fadeOverlayGroup);
        SetObjectReference(screenFadePlayerLock, "playerRoot", playerRoot);
        SetFloat(screenFadePlayerLock, "fadeDuration", 0.35f);

        ConfigurePlayer(playerRoot);

        UiBundle uiBundle = CreateCanvasUi(fadeCanvasTransform, fontAsset);
        ConfigurePlayerInteractor(playerRoot, uiBundle);

        GameObject root = new GameObject("Level4_Cycles");
        CreateEnvironment(root.transform, materials, fontAsset);

        GameObject section1 = new GameObject("Section1_For");
        section1.transform.SetParent(root.transform, false);
        section1.transform.localPosition = Vector3.zero;
        CreateSection1(section1.transform, materials, fontAsset);

        GameObject section2 = new GameObject("Section2_For");
        section2.transform.SetParent(root.transform, false);
        section2.transform.localPosition = new Vector3(0f, 0f, 10f);
        CreateSection2(section2.transform, materials, fontAsset);

        GameObject section3 = new GameObject("Section3_While");
        section3.transform.SetParent(root.transform, false);
        section3.transform.localPosition = new Vector3(0f, 0f, 25f);
        CreateSection3(section3.transform, materials, fontAsset);

        GameObject section4 = new GameObject("Section4_Final");
        section4.transform.SetParent(root.transform, false);
        section4.transform.localPosition = new Vector3(0f, 2.4f, 43f);
        CreateSection4(section4.transform, materials, fontAsset);

        CreateFinishTrigger(root.transform, materials, screenFadePlayerLock, uiBundle.FinishCanvasGroup);
    }

    private static void CreateEnvironment(Transform parent, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        GameObject environment = new GameObject("Environment");
        environment.transform.SetParent(parent, false);

        CreateCube("BackdropFloor", environment.transform, new Vector3(0f, -6.5f, 26f), new Vector3(20f, 1f, 76f), materials.DarkBackdrop);
        CreateCube("BackWall", environment.transform, new Vector3(0f, 2.2f, -9f), new Vector3(9.4f, 4.4f, 0.5f), materials.Structure);
        CreateCube("EndWall", environment.transform, new Vector3(0f, 3.2f, 61f), new Vector3(9.4f, 6.4f, 0.5f), materials.Structure);
        CreateCube("LeftWall", environment.transform, new Vector3(-CorridorHalfWidth, 3f, 26f), new Vector3(0.45f, 6f, 70f), materials.Structure);
        CreateCube("RightWall", environment.transform, new Vector3(CorridorHalfWidth, 3f, 26f), new Vector3(0.45f, 6f, 70f), materials.Structure);
        CreateCube("GlowStripLeft", environment.transform, new Vector3(-3.65f, 0.03f, 26f), new Vector3(0.08f, 0.04f, 64f), materials.Glow);
        CreateCube("GlowStripRight", environment.transform, new Vector3(3.65f, 0.03f, 26f), new Vector3(0.08f, 0.04f, 64f), materials.Glow);

        GameObject decorativeRoot = new GameObject("DecorativeTechBlocks");
        decorativeRoot.transform.SetParent(environment.transform, false);

        CreateCube("StartBlockLeft", decorativeRoot.transform, new Vector3(-2.7f, 0.7f, -6.8f), new Vector3(1f, 1.4f, 1f), materials.Structure);
        CreateCube("StartBlockRight", decorativeRoot.transform, new Vector3(2.7f, 0.7f, -5.2f), new Vector3(1f, 1.4f, 1f), materials.Structure);
        CreateCube("MidBlockLeft", decorativeRoot.transform, new Vector3(-2.8f, 0.6f, 22f), new Vector3(0.8f, 1.2f, 0.8f), materials.Structure);
        CreateCube("MidBlockRight", decorativeRoot.transform, new Vector3(2.8f, 0.6f, 37f), new Vector3(0.8f, 1.2f, 0.8f), materials.Structure);

        CreateSectionFrame(environment.transform, new Vector3(0f, 2.35f, -6f), "Урок 4", "Циклы for и while", materials, fontAsset);
        CreateSectionFrame(environment.transform, new Vector3(0f, 2.35f, 11f), "Секция 2", "FOR закрепление", materials, fontAsset);
        CreateSectionFrame(environment.transform, new Vector3(0f, 2.35f, 27f), "Секция 3", "WHILE обучение", materials, fontAsset);
        CreateSectionFrame(environment.transform, new Vector3(0f, 4.75f, 44f), "Финал", "FOR + WHILE", materials, fontAsset);
    }

    private static void CreateSection1(Transform parent, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        CreateCube("StartPlatform", parent, new Vector3(0f, 0.2f, -4f), new Vector3(6.6f, 0.4f, 8f), materials.Structure);
        CreateCube("LandingPlatform", parent, new Vector3(0f, 0.2f, 6.4f), new Vector3(6.6f, 0.4f, 7.2f), materials.Structure);

        GameObject gapRoot = new GameObject("Gap_01");
        gapRoot.transform.SetParent(parent, false);
        CreateCube("Abyss", gapRoot.transform, new Vector3(0f, -0.95f, 1.4f), new Vector3(6f, 2.1f, 2.8f), materials.DarkBackdrop);
        CreateCube("GapBorderLeft", gapRoot.transform, new Vector3(-1.55f, 0.15f, 1.4f), new Vector3(0.12f, 0.16f, 2.8f), materials.Glow);
        CreateCube("GapBorderRight", gapRoot.transform, new Vector3(1.55f, 0.15f, 1.4f), new Vector3(0.12f, 0.16f, 2.8f), materials.Glow);

        GameObject bridgeRoot = new GameObject("Bridge_01");
        bridgeRoot.transform.SetParent(parent, false);
        bridgeRoot.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        ForBridgeController bridgeController = bridgeRoot.AddComponent<ForBridgeController>();
        Transform[] segments = CreateBridgeSegments(bridgeRoot.transform, 3, 1.4f, materials.Bridge);
        ConfigureForBridge(bridgeController, segments, 0.2f, 0.12f, new Vector3(0f, -1.15f, 0f));

        GameObject terminal = CreateTerminalVisual(parent, "Terminal_For_01", new Vector3(2.45f, 0f, -1.55f), Quaternion.Euler(0f, -24f, 0f), materials, fontAsset, out TerminalIndicator indicator);
        ForTerminal forTerminal = terminal.AddComponent<ForTerminal>();
        ConfigureForTerminal(
            forTerminal,
            bridgeController,
            indicator,
            "Нажмите E, чтобы выполнить следующую итерацию for",
            "FOR / Обучение",
            new[] { 1, 2, 3 },
            2);

        CreateWorldNote(parent, new Vector3(-2.15f, 1.7f, -1.6f), new Vector3(1.7f, 1f, 0.2f), "for i in range(2):\n    extend_bridge()", materials, fontAsset);

        GameObject lowerReturn = new GameObject("LowerReturn_01");
        lowerReturn.transform.SetParent(parent, false);
        CreateCube("ReturnFloor", lowerReturn.transform, new Vector3(0f, -1.8f, -0.35f), new Vector3(5.8f, 0.4f, 6.3f), materials.Structure);
        CreateCube("ForwardBlocker", lowerReturn.transform, new Vector3(0f, -0.55f, 3.05f), new Vector3(6f, 2.1f, 0.35f), materials.Structure);
        CreateCube("ReturnWallLeft", lowerReturn.transform, new Vector3(-2.8f, -0.55f, -0.35f), new Vector3(0.35f, 2.1f, 6.4f), materials.Structure);
        CreateCube("ReturnWallRight", lowerReturn.transform, new Vector3(2.8f, -0.55f, -0.35f), new Vector3(0.35f, 2.1f, 6.4f), materials.Structure);
        CreateRamp("ReturnRamp", lowerReturn.transform, new Vector3(0f, -0.8f, -5.2f), new Vector3(3.2f, 0.35f, 4.8f), 24f, materials.Panel);
        CreateCube("RampGuide", lowerReturn.transform, new Vector3(0f, 0.2f, -7.05f), new Vector3(3.2f, 0.4f, 1.1f), materials.Structure);
    }

    private static void CreateSection2(Transform parent, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        CreateCube("ApproachPlatform", parent, new Vector3(0f, 0.2f, 2f), new Vector3(6.6f, 0.4f, 4f), materials.Structure);
        CreateCube("LandingPlatform", parent, new Vector3(0f, 0.2f, 11.6f), new Vector3(6.6f, 0.4f, 6.8f), materials.Structure);

        GameObject gapRoot = new GameObject("Gap_02");
        gapRoot.transform.SetParent(parent, false);
        CreateCube("Abyss", gapRoot.transform, new Vector3(0f, -1.1f, 6.1f), new Vector3(6f, 2.4f, 4.2f), materials.DarkBackdrop);
        CreateCube("GapBorderLeft", gapRoot.transform, new Vector3(-1.55f, 0.15f, 6.1f), new Vector3(0.12f, 0.16f, 4.2f), materials.Glow);
        CreateCube("GapBorderRight", gapRoot.transform, new Vector3(1.55f, 0.15f, 6.1f), new Vector3(0.12f, 0.16f, 4.2f), materials.Glow);

        GameObject bridgeRoot = new GameObject("Bridge_02");
        bridgeRoot.transform.SetParent(parent, false);
        bridgeRoot.transform.localPosition = new Vector3(0f, 0.1f, 4f);
        ForBridgeController bridgeController = bridgeRoot.AddComponent<ForBridgeController>();
        Transform[] segments = CreateBridgeSegments(bridgeRoot.transform, 4, 1.4f, materials.Bridge);
        ConfigureForBridge(bridgeController, segments, 0.2f, 0.12f, new Vector3(0f, -1.15f, 0f));

        GameObject terminal = CreateTerminalVisual(parent, "Terminal_For_02", new Vector3(-2.45f, 0f, 1.55f), Quaternion.Euler(0f, 24f, 0f), materials, fontAsset, out TerminalIndicator indicator);
        ForTerminal forTerminal = terminal.AddComponent<ForTerminal>();
        ConfigureForTerminal(
            forTerminal,
            bridgeController,
            indicator,
            "Нажмите E, чтобы выполнить следующую итерацию for",
            "FOR / Закрепление",
            new[] { 2, 3, 4 },
            3);

        CreateWorldNote(parent, new Vector3(2.15f, 1.7f, 1.6f), new Vector3(1.7f, 1f, 0.2f), "for i in range(3):\n    extend_bridge()", materials, fontAsset);

        GameObject lowerReturn = new GameObject("LowerReturn_02");
        lowerReturn.transform.SetParent(parent, false);
        CreateCube("ReturnFloor", lowerReturn.transform, new Vector3(0f, -1.95f, 4.9f), new Vector3(5.8f, 0.4f, 7.4f), materials.Structure);
        CreateCube("ForwardBlocker", lowerReturn.transform, new Vector3(0f, -0.65f, 8.55f), new Vector3(6f, 2.3f, 0.35f), materials.Structure);
        CreateCube("ReturnWallLeft", lowerReturn.transform, new Vector3(-2.8f, -0.65f, 4.9f), new Vector3(0.35f, 2.3f, 7.4f), materials.Structure);
        CreateCube("ReturnWallRight", lowerReturn.transform, new Vector3(2.8f, -0.65f, 4.9f), new Vector3(0.35f, 2.3f, 7.4f), materials.Structure);
        CreateRamp("ReturnRamp", lowerReturn.transform, new Vector3(0f, -0.9f, 0.9f), new Vector3(3.2f, 0.35f, 5.2f), 24f, materials.Panel);
        CreateCube("RampGuide", lowerReturn.transform, new Vector3(0f, 0.2f, -1.2f), new Vector3(3.2f, 0.4f, 1.2f), materials.Structure);
    }

    private static void CreateSection3(Transform parent, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        CreateCube("GroundPlatform", parent, new Vector3(0f, 0.2f, 2f), new Vector3(6.6f, 0.4f, 4f), materials.Structure);
        CreateCube("LiftBase", parent, new Vector3(0f, 0.2f, 5.3f), new Vector3(3f, 0.4f, 2.4f), materials.Structure);
        CreateCube("TargetPlatform_01", parent, new Vector3(0f, 2.6f, 12.8f), new Vector3(6.6f, 0.4f, 10.4f), materials.Structure);
        CreateCube("TargetGlow", parent, new Vector3(0f, 2.82f, 12.8f), new Vector3(2.8f, 0.05f, 5.8f), materials.Glow);
        CreateRail(parent, new Vector3(-3.05f, 3.05f, 12.8f), new Vector3(0.15f, 0.9f, 10.2f), materials.Panel);
        CreateRail(parent, new Vector3(3.05f, 3.05f, 12.8f), new Vector3(0.15f, 0.9f, 10.2f), materials.Panel);
        CreateSupportColumn(parent, new Vector3(-2.1f, 1.3f, 9.2f), 2.6f, materials.Structure);
        CreateSupportColumn(parent, new Vector3(2.1f, 1.3f, 14.8f), 2.6f, materials.Structure);

        GameObject terminal = CreateTerminalVisual(parent, "Terminal_While_01", new Vector3(2.45f, 0f, 1.55f), Quaternion.Euler(0f, -26f, 0f), materials, fontAsset, out TerminalIndicator indicator);

        GameObject liftRoot = new GameObject("LiftPlatform_01");
        liftRoot.transform.SetParent(parent, false);
        liftRoot.transform.localPosition = new Vector3(0f, 0f, 6.15f);
        CreateCube("LiftPedestal", liftRoot.transform, new Vector3(0f, -0.2f, 0f), new Vector3(1.6f, 0.4f, 1.6f), materials.Panel);
        GameObject liftVisual = CreateCube("PlatformVisual", liftRoot.transform, new Vector3(0f, 0.2f, 0f), new Vector3(2.3f, 0.4f, 2.1f), materials.Panel);
        WhileLiftController liftController = liftRoot.AddComponent<WhileLiftController>();
        ConfigureWhileLift(liftController, liftVisual.transform, 2.4f, 0.8f, 0.22f, 0.16f);

        WhileTerminal whileTerminal = terminal.AddComponent<WhileTerminal>();
        ConfigureWhileTerminal(
            whileTerminal,
            liftController,
            indicator,
            "Нажмите E, чтобы запустить подъём по условию",
            "WHILE / Подъём",
            "while высота < цель");

        CreateWorldNote(parent, new Vector3(-2.15f, 1.7f, 1.6f), new Vector3(1.9f, 1f, 0.2f), "while height < target:\n    lift_platform()", materials, fontAsset);
    }

    private static void CreateSection4(Transform parent, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        CreateCube("ForApproach", parent, new Vector3(0f, 0.2f, 1f), new Vector3(6.2f, 0.4f, 2f), materials.Structure);

        GameObject bridgeRoot = new GameObject("Bridge_Final");
        bridgeRoot.transform.SetParent(parent, false);
        bridgeRoot.transform.localPosition = new Vector3(0f, 0.1f, 2f);
        ForBridgeController bridgeController = bridgeRoot.AddComponent<ForBridgeController>();
        Transform[] segments = CreateBridgeSegments(bridgeRoot.transform, 3, 1.4f, materials.Bridge);
        ConfigureForBridge(bridgeController, segments, 0.2f, 0.12f, new Vector3(0f, -1.15f, 0f));

        CreateCube("FinalBridgeGap", parent, new Vector3(0f, -0.9f, 4.1f), new Vector3(5.8f, 2.1f, 4.2f), materials.DarkBackdrop);
        CreateCube("PostBridgePlatform", parent, new Vector3(0f, 0.2f, 7.9f), new Vector3(6.2f, 0.4f, 3.4f), materials.Structure);

        GameObject terminalFor = CreateTerminalVisual(parent, "Terminal_For_Final", new Vector3(2.25f, 0f, 0.75f), Quaternion.Euler(0f, -24f, 0f), materials, fontAsset, out TerminalIndicator forIndicator);
        ForTerminal forTerminal = terminalFor.AddComponent<ForTerminal>();
        ConfigureForTerminal(
            forTerminal,
            bridgeController,
            forIndicator,
            "Нажмите E, чтобы выполнить следующую итерацию for",
            "FOR / Финал",
            new[] { 1, 2, 3 },
            3);

        GameObject terminalWhile = CreateTerminalVisual(parent, "Terminal_While_Final", new Vector3(-2.25f, 0f, 7.35f), Quaternion.Euler(0f, 24f, 0f), materials, fontAsset, out TerminalIndicator whileIndicator);

        GameObject liftRoot = new GameObject("LiftPlatform_Final");
        liftRoot.transform.SetParent(parent, false);
        liftRoot.transform.localPosition = new Vector3(0f, 0f, 10.8f);
        CreateCube("LiftShaft", liftRoot.transform, new Vector3(0f, 0.8f, 0f), new Vector3(2.7f, 1.6f, 2.5f), materials.Structure);
        GameObject liftVisual = CreateCube("PlatformVisual", liftRoot.transform, new Vector3(0f, 0.2f, 0f), new Vector3(2.2f, 0.4f, 2.1f), materials.Panel);
        WhileLiftController liftController = liftRoot.AddComponent<WhileLiftController>();
        ConfigureWhileLift(liftController, liftVisual.transform, 1.8f, 0.6f, 0.2f, 0.14f);

        WhileTerminal whileTerminal = terminalWhile.AddComponent<WhileTerminal>();
        ConfigureWhileTerminal(
            whileTerminal,
            liftController,
            whileIndicator,
            "Нажмите E, чтобы завершить while-подъём",
            "WHILE / Финал",
            "while lift < finish");

        CreateCube("FinishPlatform", parent, new Vector3(0f, 2f, 14.4f), new Vector3(6.2f, 0.4f, 5.6f), materials.Panel);
        CreateCube("FinishGlow", parent, new Vector3(0f, 2.22f, 14.4f), new Vector3(3.2f, 0.05f, 3.2f), materials.Glow);
        CreateRail(parent, new Vector3(-3f, 2.45f, 13.8f), new Vector3(0.12f, 0.9f, 4.4f), materials.Structure);
        CreateRail(parent, new Vector3(3f, 2.45f, 13.8f), new Vector3(0.12f, 0.9f, 4.4f), materials.Structure);
        CreateSupportColumn(parent, new Vector3(-2.25f, -1.2f, 12.8f), 2.8f, materials.Structure);
        CreateSupportColumn(parent, new Vector3(2.25f, -1.2f, 15.8f), 2.8f, materials.Structure);

        CreateCube("FinishArchLeft", parent, new Vector3(-1.8f, 3.45f, 16.8f), new Vector3(0.35f, 2.5f, 0.35f), materials.Structure);
        CreateCube("FinishArchRight", parent, new Vector3(1.8f, 3.45f, 16.8f), new Vector3(0.35f, 2.5f, 0.35f), materials.Structure);
        CreateCube("FinishArchTop", parent, new Vector3(0f, 4.6f, 16.8f), new Vector3(4f, 0.3f, 0.35f), materials.Glow);

        CreateWorldNote(parent, new Vector3(0f, 1.75f, 4.15f), new Vector3(1.9f, 1f, 0.2f), "for -> bridge\nwhile -> lift", materials, fontAsset);
    }

    private static void CreateFinishTrigger(
        Transform parent,
        MaterialPalette materials,
        ScreenFadePlayerLock screenFadePlayerLock,
        CanvasGroup finishCanvasGroup)
    {
        GameObject finishTriggerObject = new GameObject("FinishTrigger");
        finishTriggerObject.transform.SetParent(parent, false);
        finishTriggerObject.transform.localPosition = new Vector3(0f, 4.4f, 58.4f);

        BoxCollider collider = finishTriggerObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = new Vector3(0f, 1.2f, 0f);
        collider.size = new Vector3(4.5f, 2.5f, 3f);

        LevelFinishTrigger finishTrigger = finishTriggerObject.AddComponent<LevelFinishTrigger>();
        SetObjectReference(finishTrigger, "screenFadePlayerLock", screenFadePlayerLock);
        SetObjectReference(finishTrigger, "finishCanvasGroup", finishCanvasGroup);
        SetFloat(finishTrigger, "fadeDuration", 0.8f);
        SetFloat(finishTrigger, "overlayAlphaWhenComplete", 0.16f);

        CreateCube("FinishPadGuide", parent, new Vector3(0f, 4.44f, 58.4f), new Vector3(3.2f, 0.05f, 2.2f), materials.Glow);
    }

    private static UiBundle CreateCanvasUi(Transform fadeCanvasTransform, TMP_FontAsset fontAsset)
    {
        RectTransform overview = CreatePanel(
            "LessonCard",
            fadeCanvasTransform,
            new Vector2(0.03f, 0.76f),
            new Vector2(0.29f, 0.95f),
            new Color(0.97f, 0.97f, 0.98f, 0.96f));

        CreatePanel(
            "Accent",
            overview,
            new Vector2(0f, 0f),
            new Vector2(0.04f, 1f),
            new Color(0.47f, 0.34f, 0.92f, 1f));

        CreateText(
            "LessonTitle",
            overview,
            new Vector2(0.08f, 0.62f),
            new Vector2(0.95f, 0.92f),
            Vector2.zero,
            Vector2.zero,
            "Урок 4: Циклы",
            fontAsset,
            30,
            new Color(0.14f, 0.16f, 0.2f, 1f),
            TextAlignmentOptions.Left);

        CreateText(
            "LessonBody",
            overview,
            new Vector2(0.08f, 0.14f),
            new Vector2(0.95f, 0.6f),
            Vector2.zero,
            Vector2.zero,
            "FOR повторяет действие заданное число раз.\nWHILE повторяет действие, пока условие истинно.",
            fontAsset,
            18,
            new Color(0.48f, 0.5f, 0.56f, 1f),
            TextAlignmentOptions.TopLeft);

        RectTransform progressCard = CreatePanel(
            "ProgressCard",
            fadeCanvasTransform,
            new Vector2(0.78f, 0.84f),
            new Vector2(0.96f, 0.94f),
            new Color(0.97f, 0.97f, 0.98f, 0.96f));

        CreateText(
            "ProgressTitle",
            progressCard,
            new Vector2(0.08f, 0.52f),
            new Vector2(0.92f, 0.88f),
            Vector2.zero,
            Vector2.zero,
            "Урок 4 из 5",
            fontAsset,
            20,
            new Color(0.14f, 0.16f, 0.2f, 1f),
            TextAlignmentOptions.Left);

        CreateText(
            "ProgressBody",
            progressCard,
            new Vector2(0.08f, 0.14f),
            new Vector2(0.92f, 0.5f),
            Vector2.zero,
            Vector2.zero,
            "Короткий прототип по Python-циклам",
            fontAsset,
            14,
            new Color(0.48f, 0.5f, 0.56f, 1f),
            TextAlignmentOptions.Left);

        RectTransform promptRoot = CreatePanel(
            "InteractionPrompt",
            fadeCanvasTransform,
            new Vector2(0.29f, 0.04f),
            new Vector2(0.71f, 0.14f),
            new Color(0.98f, 0.98f, 0.99f, 0.98f));

        CreatePanel(
            "PromptAccent",
            promptRoot,
            new Vector2(0f, 0f),
            new Vector2(0.03f, 1f),
            new Color(0.26f, 0.76f, 0.34f, 1f));

        CreateText(
            "PromptTitle",
            promptRoot,
            new Vector2(0.07f, 0.52f),
            new Vector2(0.93f, 0.9f),
            Vector2.zero,
            Vector2.zero,
            "Взаимодействие",
            fontAsset,
            20,
            new Color(0.14f, 0.16f, 0.2f, 1f),
            TextAlignmentOptions.Left);

        TextMeshProUGUI promptText = CreateText(
            "PromptText",
            promptRoot,
            new Vector2(0.07f, 0.12f),
            new Vector2(0.93f, 0.56f),
            Vector2.zero,
            Vector2.zero,
            "Нажмите E, чтобы использовать терминал",
            fontAsset,
            16,
            new Color(0.43f, 0.45f, 0.51f, 1f),
            TextAlignmentOptions.Left);

        promptRoot.gameObject.SetActive(false);

        RectTransform finishRoot = CreatePanel(
            "Level4FinishPanel",
            fadeCanvasTransform,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Color(0.97f, 0.97f, 0.98f, 0.98f),
            new Vector2(620f, 260f));

        CanvasGroup finishCanvasGroup = finishRoot.gameObject.AddComponent<CanvasGroup>();
        finishCanvasGroup.alpha = 0f;
        finishCanvasGroup.blocksRaycasts = false;
        finishCanvasGroup.interactable = false;

        CreatePanel(
            "FinishAccent",
            finishRoot,
            new Vector2(0.08f, 0.82f),
            new Vector2(0.92f, 0.88f),
            new Color(0.26f, 0.76f, 0.34f, 1f));

        CreateText(
            "FinishTitle",
            finishRoot,
            new Vector2(0.1f, 0.54f),
            new Vector2(0.9f, 0.78f),
            Vector2.zero,
            Vector2.zero,
            "Уровень пройден",
            fontAsset,
            40,
            new Color(0.14f, 0.16f, 0.2f, 1f),
            TextAlignmentOptions.Center);

        CreateText(
            "FinishBody",
            finishRoot,
            new Vector2(0.12f, 0.26f),
            new Vector2(0.88f, 0.5f),
            Vector2.zero,
            Vector2.zero,
            "Вы прошли короткий уровень и увидели разницу между for и while.",
            fontAsset,
            22,
            new Color(0.43f, 0.45f, 0.51f, 1f),
            TextAlignmentOptions.Center);

        return new UiBundle(promptRoot.gameObject, promptText, finishCanvasGroup);
    }

    private static GameObject CreateTerminalVisual(
        Transform parent,
        string name,
        Vector3 localPosition,
        Quaternion localRotation,
        MaterialPalette materials,
        TMP_FontAsset fontAsset,
        out TerminalIndicator indicator)
    {
        GameObject terminalRoot = new GameObject(name);
        terminalRoot.transform.SetParent(parent, false);
        terminalRoot.transform.localPosition = localPosition;
        terminalRoot.transform.localRotation = localRotation;

        indicator = terminalRoot.AddComponent<TerminalIndicator>();

        CreateCube("Base", terminalRoot.transform, new Vector3(0f, 0.18f, 0f), new Vector3(1.15f, 0.36f, 0.92f), materials.Structure);
        CreateCube("Stand", terminalRoot.transform, new Vector3(0f, 0.92f, 0f), new Vector3(0.24f, 1.42f, 0.24f), materials.Structure);
        GameObject screen = CreateCube("Screen", terminalRoot.transform, new Vector3(0f, 1.82f, 0.18f), new Vector3(1.5f, 0.94f, 0.12f), materials.ScreenIdle);
        CreateCube("ScreenHood", terminalRoot.transform, new Vector3(0f, 2.28f, 0.05f), new Vector3(1.1f, 0.14f, 0.45f), materials.Structure);

        GameObject idleIndicator = CreateCube("IdleIndicator", terminalRoot.transform, new Vector3(-0.36f, 2.48f, 0.14f), new Vector3(0.18f, 0.11f, 0.11f), materials.ScreenIdle);
        GameObject runningIndicator = CreateCube("RunningIndicator", terminalRoot.transform, new Vector3(-0.12f, 2.48f, 0.14f), new Vector3(0.18f, 0.11f, 0.11f), materials.ScreenRunning);
        GameObject successIndicator = CreateCube("SuccessIndicator", terminalRoot.transform, new Vector3(0.12f, 2.48f, 0.14f), new Vector3(0.18f, 0.11f, 0.11f), materials.ScreenSuccess);
        GameObject errorIndicator = CreateCube("ErrorIndicator", terminalRoot.transform, new Vector3(0.36f, 2.48f, 0.14f), new Vector3(0.18f, 0.11f, 0.11f), materials.ScreenError);

        TextMeshPro titleText = CreateWorldText(
            "TitleText",
            terminalRoot.transform,
            new Vector3(0f, 2.06f, 0.245f),
            "Терминал",
            fontAsset,
            4.8f,
            new Color(0.13f, 0.16f, 0.2f, 1f),
            new Vector2(1.28f, 0.24f),
            TextAlignmentOptions.Center,
            new Vector3(0.08f, 0.08f, 0.08f));

        TextMeshPro bodyText = CreateWorldText(
            "BodyText",
            terminalRoot.transform,
            new Vector3(0f, 1.78f, 0.245f),
            "for i in range(2)",
            fontAsset,
            3.8f,
            new Color(0.29f, 0.33f, 0.4f, 1f),
            new Vector2(1.26f, 0.44f),
            TextAlignmentOptions.Center,
            new Vector3(0.072f, 0.072f, 0.072f));

        TextMeshPro statusText = CreateWorldText(
            "StatusText",
            terminalRoot.transform,
            new Vector3(0f, 1.42f, 0.245f),
            "Ожидание",
            fontAsset,
            4f,
            new Color(0.26f, 0.76f, 0.34f, 1f),
            new Vector2(1.18f, 0.22f),
            TextAlignmentOptions.Center,
            new Vector3(0.072f, 0.072f, 0.072f));

        CreateInteractionZone(terminalRoot.transform, new Vector3(0f, 1.5f, 1.8f), new Vector3(4.2f, 3.4f, 4.2f));

        SetObjectReference(indicator, "screenRenderer", screen.GetComponent<Renderer>());
        SetObjectReference(indicator, "idleMaterial", materials.ScreenIdle);
        SetObjectReference(indicator, "runningMaterial", materials.ScreenRunning);
        SetObjectReference(indicator, "successMaterial", materials.ScreenSuccess);
        SetObjectReference(indicator, "errorMaterial", materials.ScreenError);
        SetObjectReference(indicator, "titleText", titleText);
        SetObjectReference(indicator, "bodyText", bodyText);
        SetObjectReference(indicator, "statusText", statusText);
        SetObjectReference(indicator, "idleStateRoot", idleIndicator);
        SetObjectReference(indicator, "runningStateRoot", runningIndicator);
        SetObjectReference(indicator, "successStateRoot", successIndicator);
        SetObjectReference(indicator, "errorStateRoot", errorIndicator);

        return terminalRoot;
    }

    private static Transform[] CreateBridgeSegments(Transform parent, int segmentCount, float segmentLength, Material material)
    {
        Transform[] segments = new Transform[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            float z = segmentLength * 0.5f + (i * segmentLength);
            GameObject segment = CreateCube(
                $"Segment_{i + 1:00}",
                parent,
                new Vector3(0f, 0.09f, z),
                new Vector3(2.3f, 0.18f, segmentLength),
                material);
            segments[i] = segment.transform;
        }

        return segments;
    }

    private static void ConfigureForBridge(ForBridgeController bridgeController, Transform[] segments, float moveDuration, float stepDelay, Vector3 hiddenOffset)
    {
        SetObjectArray(bridgeController, "bridgeSegments", segments);
        SetFloat(bridgeController, "segmentMoveDuration", moveDuration);
        SetFloat(bridgeController, "delayBetweenSegments", stepDelay);
        SetVector3(bridgeController, "hiddenLocalOffset", hiddenOffset);
        SetBool(bridgeController, "disableCollidersWhileHidden", true);
        bridgeController.CaptureExtendedPositions();
        bridgeController.ResetBridgeInstantly();
        EditorUtility.SetDirty(bridgeController);
    }

    private static void ConfigureForTerminal(
        ForTerminal terminal,
        ForBridgeController bridgeController,
        TerminalIndicator indicator,
        string interactionPrompt,
        string sectionTitle,
        int[] availableCounts,
        int requiredCount)
    {
        SetObjectReference(terminal, "bridgeController", bridgeController);
        SetObjectReference(terminal, "terminalIndicator", indicator);
        SetString(terminal, "interactionPrompt", interactionPrompt);
        SetString(terminal, "sectionTitle", sectionTitle);
        SetIntegerArray(terminal, "availableCounts", availableCounts);
        SetInteger(terminal, "requiredCount", requiredCount);
        SetBool(terminal, "createInteractionTrigger", true);
        SetVector3(terminal, "triggerLocalPosition", new Vector3(0f, 1.5f, 1.8f));
        SetVector3(terminal, "triggerSize", new Vector3(4.2f, 3.4f, 4.2f));
    }

    private static void ConfigureWhileLift(WhileLiftController liftController, Transform platformTransform, float targetHeight, float stepHeight, float moveDuration, float stepDelay)
    {
        SetObjectReference(liftController, "platformTransform", platformTransform);
        SetFloat(liftController, "targetHeight", targetHeight);
        SetFloat(liftController, "stepHeight", stepHeight);
        SetFloat(liftController, "stepMoveDuration", moveDuration);
        SetFloat(liftController, "stepDelay", stepDelay);
    }

    private static void ConfigureWhileTerminal(
        WhileTerminal terminal,
        WhileLiftController liftController,
        TerminalIndicator indicator,
        string interactionPrompt,
        string sectionTitle,
        string conditionLabel)
    {
        SetObjectReference(terminal, "liftController", liftController);
        SetObjectReference(terminal, "terminalIndicator", indicator);
        SetString(terminal, "interactionPrompt", interactionPrompt);
        SetString(terminal, "sectionTitle", sectionTitle);
        SetString(terminal, "conditionLabel", conditionLabel);
        SetBool(terminal, "createInteractionTrigger", true);
        SetVector3(terminal, "triggerLocalPosition", new Vector3(0f, 1.5f, 1.8f));
        SetVector3(terminal, "triggerSize", new Vector3(4.2f, 3.4f, 4.2f));
    }

    private static void ConfigurePlayer(GameObject playerRoot)
    {
        playerRoot.transform.SetParent(null);
        playerRoot.transform.SetPositionAndRotation(new Vector3(0f, 1.45f, -6.5f), Quaternion.identity);
    }

    private static void ConfigurePlayerInteractor(GameObject playerRoot, UiBundle uiBundle)
    {
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
        SetBool(interactor, "useRaycastFallback", false);
        SetFloat(interactor, "interactionDistance", 4.5f);
        SetFloat(interactor, "interactionRadius", 0.35f);
        SetFloat(interactor, "interactionAssistDepth", 1.1f);
        SetLayerMask(interactor, "interactionMask", ~0);
        SetObjectReference(interactor, "promptRoot", uiBundle.PromptRoot);
        SetObjectReference(interactor, "promptText", uiBundle.PromptText);
        SetString(interactor, "defaultPrompt", "Нажмите E, чтобы взаимодействовать");
    }

    private static GameObject EnsurePlayerRoot()
    {
        ThirdPersonController playerController = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>(FindObjectsInactive.Include);
        if (playerController != null)
        {
            GameObject existingRoot = playerController.transform.root.gameObject;
            existingRoot.transform.SetParent(null);
            return existingRoot;
        }

        GameObject prefab = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null)
        {
            throw new InvalidOperationException("Player prefab could not be instantiated.");
        }

        instance.transform.SetParent(null);
        return instance;
    }

    private static GameObject EnsureFadeCanvas()
    {
        ScreenFadePlayerLock existingFade = UnityEngine.Object.FindFirstObjectByType<ScreenFadePlayerLock>(FindObjectsInactive.Include);
        if (existingFade != null)
        {
            GameObject fadeCanvas = existingFade.gameObject;
            fadeCanvas.transform.SetParent(null);
            fadeCanvas.name = "FadeCanvas";
            return fadeCanvas;
        }

        Transform existingByName = FindSceneObject("FadeCanvas");
        if (existingByName != null)
        {
            existingByName.SetParent(null);
            if (existingByName.GetComponent<ScreenFadePlayerLock>() == null)
            {
                existingByName.gameObject.AddComponent<ScreenFadePlayerLock>();
            }

            return existingByName.gameObject;
        }

        GameObject canvasObject = new GameObject("FadeCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(ScreenFadePlayerLock));
        canvasObject.layer = GetUiLayer();

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = false;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvasObject;
    }

    private static CanvasGroup EnsureFadeOverlay(RectTransform fadeCanvasTransform)
    {
        Transform overlayTransform = fadeCanvasTransform.Find("FadeOverlay");
        if (overlayTransform == null)
        {
            GameObject overlayObject = new GameObject("FadeOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            overlayObject.layer = GetUiLayer();
            overlayTransform = overlayObject.transform;
            overlayTransform.SetParent(fadeCanvasTransform, false);
        }

        RectTransform rectTransform = overlayTransform.GetComponent<RectTransform>();
        ConfigureRectTransform(rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        Image image = overlayTransform.GetComponent<Image>();
        image.color = new Color(0.03f, 0.05f, 0.08f, 1f);

        CanvasGroup canvasGroup = overlayTransform.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        return canvasGroup;
    }

    private static void CleanupFadeCanvas(Transform fadeCanvasTransform)
    {
        for (int i = fadeCanvasTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = fadeCanvasTransform.GetChild(i);
            if (child.name == "FadeOverlay")
            {
                continue;
            }

            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }
    }

    private static void RemoveOldRoots(GameObject playerRoot, GameObject fadeCanvasObject)
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        foreach (GameObject root in activeScene.GetRootGameObjects())
        {
            if (root == playerRoot || root == fadeCanvasObject)
            {
                continue;
            }

            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static void EnsureDirectionalLight()
    {
        Light directionalLight = UnityEngine.Object.FindFirstObjectByType<Light>(FindObjectsInactive.Include);
        if (directionalLight == null)
        {
            GameObject lightObject = new GameObject("Directional Light", typeof(Light));
            directionalLight = lightObject.GetComponent<Light>();
        }

        directionalLight.type = LightType.Directional;
        directionalLight.intensity = 1.15f;
        directionalLight.color = new Color(0.91f, 0.95f, 1f, 1f);
        directionalLight.shadows = LightShadows.Soft;
        directionalLight.transform.position = new Vector3(0f, 10f, 0f);
        directionalLight.transform.rotation = Quaternion.Euler(38f, -32f, 0f);
    }

    private static void EnsureEventSystem()
    {
        EventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    private static void CreateSectionFrame(Transform parent, Vector3 position, string title, string subtitle, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        CreateCube($"{title}_FrameTop", parent, position + new Vector3(0f, 0.95f, 0f), new Vector3(4.6f, 0.18f, 0.22f), materials.Glow);
        CreateCube($"{title}_FrameLeft", parent, position + new Vector3(-2.2f, 0f, 0f), new Vector3(0.18f, 1.8f, 0.22f), materials.Structure);
        CreateCube($"{title}_FrameRight", parent, position + new Vector3(2.2f, 0f, 0f), new Vector3(0.18f, 1.8f, 0.22f), materials.Structure);
        CreateWorldText($"{title}_Title", parent, position + new Vector3(0f, 0.3f, 0.12f), title, fontAsset, 7f, new Color(0.9f, 0.94f, 1f, 1f), new Vector2(5.4f, 0.6f), TextAlignmentOptions.Center, new Vector3(0.12f, 0.12f, 0.12f));
        CreateWorldText($"{title}_Subtitle", parent, position + new Vector3(0f, -0.2f, 0.12f), subtitle, fontAsset, 4.2f, new Color(0.65f, 0.75f, 0.84f, 1f), new Vector2(5.4f, 0.36f), TextAlignmentOptions.Center, new Vector3(0.1f, 0.1f, 0.1f));
    }

    private static void CreateWorldNote(Transform parent, Vector3 localPosition, Vector3 panelScale, string body, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        GameObject note = CreateCube("CodeNote", parent, localPosition, panelScale, materials.Panel);
        TextMeshPro bodyText = CreateWorldText("CodeText", note.transform, new Vector3(0f, 0.06f, panelScale.z * 0.52f), body, fontAsset, 4.4f, new Color(0.19f, 0.22f, 0.27f, 1f), new Vector2(2f, 0.9f), TextAlignmentOptions.Center, new Vector3(0.085f, 0.085f, 0.085f));
        bodyText.enableWordWrapping = false;
    }

    private static void CreateInteractionZone(Transform parent, Vector3 localPosition, Vector3 size)
    {
        GameObject zoneObject = new GameObject("InteractionTriggerZone", typeof(BoxCollider), typeof(InteractionTriggerZone));
        zoneObject.transform.SetParent(parent, false);
        zoneObject.transform.localPosition = localPosition;

        BoxCollider collider = zoneObject.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = Vector3.zero;
        collider.size = size;
    }

    private static void CreateSupportColumn(Transform parent, Vector3 position, float height, Material material)
    {
        CreateCube("SupportColumn", parent, position, new Vector3(0.45f, height, 0.45f), material);
    }

    private static void CreateRail(Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        CreateCube("Rail", parent, position, scale, material);
    }

    private static void CreateRamp(string name, Transform parent, Vector3 localPosition, Vector3 localScale, float angle, Material material)
    {
        GameObject ramp = CreateCube(name, parent, localPosition, localScale, material);
        ramp.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        primitive.name = name;
        primitive.transform.SetParent(parent, false);
        primitive.transform.localPosition = localPosition;
        primitive.transform.localRotation = Quaternion.identity;
        primitive.transform.localScale = localScale;

        Renderer renderer = primitive.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.sharedMaterial = material;
        }

        return primitive;
    }

    private static RectTransform CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color, Vector2? sizeDelta = null)
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

    private static TextMeshProUGUI CreateText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition, string content, TMP_FontAsset fontAsset, int fontSize, Color color, TextAlignmentOptions alignment)
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
        text.enableWordWrapping = true;
        return text;
    }

    private static TextMeshPro CreateWorldText(string name, Transform parent, Vector3 localPosition, string content, TMP_FontAsset fontAsset, float fontSize, Color color, Vector2 rectSize, TextAlignmentOptions alignment, Vector3 localScale)
    {
        GameObject textObject = new GameObject(name, typeof(TextMeshPro));
        textObject.transform.SetParent(parent, false);
        textObject.transform.localPosition = localPosition;
        textObject.transform.localRotation = Quaternion.identity;
        textObject.transform.localScale = localScale;

        TextMeshPro text = textObject.GetComponent<TextMeshPro>();
        text.font = fontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.text = content;
        text.enableWordWrapping = true;
        text.rectTransform.sizeDelta = rectSize;
        text.margin = new Vector4(0.08f, 0.04f, 0.08f, 0.04f);
        return text;
    }

    private static void ConfigureRectTransform(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition, Vector2 pivot)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.pivot = pivot;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    private static MaterialPalette EnsureMaterials()
    {
        if (!AssetDatabase.IsValidFolder(MaterialFolder))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "CyclesLevel");
        }

        return new MaterialPalette(
            CreateOrUpdateMaterial(StructureMaterialPath, new Color(0.14f, 0.18f, 0.25f), 0.42f, 0.02f, new Color(0.02f, 0.03f, 0.04f)),
            CreateOrUpdateMaterial(PanelMaterialPath, new Color(0.94f, 0.95f, 0.98f), 0.18f, 0f, new Color(0.03f, 0.03f, 0.03f)),
            CreateOrUpdateMaterial(GlowMaterialPath, new Color(0.38f, 0.86f, 1f), 0.1f, 0f, new Color(0.18f, 0.85f, 1.9f)),
            CreateOrUpdateMaterial(BridgeMaterialPath, new Color(0.27f, 0.87f, 0.88f), 0.14f, 0.02f, new Color(0.16f, 0.92f, 1.55f)),
            CreateOrUpdateMaterial(ScreenIdleMaterialPath, new Color(0.96f, 0.97f, 1f), 0.1f, 0f, new Color(0.08f, 0.1f, 0.16f)),
            CreateOrUpdateMaterial(ScreenRunningMaterialPath, new Color(1f, 0.9f, 0.72f), 0.1f, 0f, new Color(1.1f, 0.58f, 0.12f)),
            CreateOrUpdateMaterial(ScreenSuccessMaterialPath, new Color(0.83f, 0.98f, 0.86f), 0.1f, 0f, new Color(0.15f, 1.2f, 0.42f)),
            CreateOrUpdateMaterial(ScreenErrorMaterialPath, new Color(1f, 0.85f, 0.86f), 0.1f, 0f, new Color(1.35f, 0.22f, 0.26f)),
            CreateOrUpdateMaterial(DarkBackdropMaterialPath, new Color(0.03f, 0.05f, 0.08f), 0.4f, 0.02f, Color.black));
    }

    private static Material CreateOrUpdateMaterial(string path, Color baseColor, float smoothness, float metallic, Color emissionColor)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                throw new InvalidOperationException("URP Lit shader was not found.");
            }

            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_Color", baseColor);
        material.SetFloat("_Smoothness", smoothness);
        material.SetFloat("_Metallic", metallic);
        material.SetColor("_EmissionColor", emissionColor);
        material.globalIlluminationFlags = emissionColor.maxColorComponent > 0.001f ? MaterialGlobalIlluminationFlags.RealtimeEmissive : MaterialGlobalIlluminationFlags.EmissiveIsBlack;

        if (emissionColor.maxColorComponent > 0.001f)
        {
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            material.DisableKeyword("_EMISSION");
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    private static Transform FindSceneObject(string name)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform transform in transforms)
        {
            if (transform.name == name)
            {
                return transform;
            }
        }

        return null;
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

    private static int GetUiLayer()
    {
        int uiLayer = LayerMask.NameToLayer("UI");
        return uiLayer >= 0 ? uiLayer : 5;
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

    private static void SetString(UnityEngine.Object target, string propertyName, string value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).stringValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetBool(UnityEngine.Object target, string propertyName, bool value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).boolValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetVector3(UnityEngine.Object target, string propertyName, Vector3 value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).vector3Value = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetLayerMask(UnityEngine.Object target, string propertyName, int value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).intValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetObjectArray(UnityEngine.Object target, string propertyName, UnityEngine.Object[] values)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        property.arraySize = values.Length;

        for (int i = 0; i < values.Length; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetIntegerArray(UnityEngine.Object target, string propertyName, int[] values)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        property.arraySize = values.Length;

        for (int i = 0; i < values.Length; i++)
        {
            property.GetArrayElementAtIndex(i).intValue = values[i];
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private readonly struct UiBundle
    {
        public UiBundle(GameObject promptRoot, TMP_Text promptText, CanvasGroup finishCanvasGroup)
        {
            PromptRoot = promptRoot;
            PromptText = promptText;
            FinishCanvasGroup = finishCanvasGroup;
        }

        public GameObject PromptRoot { get; }
        public TMP_Text PromptText { get; }
        public CanvasGroup FinishCanvasGroup { get; }
    }

    private readonly struct MaterialPalette
    {
        public MaterialPalette(Material structure, Material panel, Material glow, Material bridge, Material screenIdle, Material screenRunning, Material screenSuccess, Material screenError, Material darkBackdrop)
        {
            Structure = structure;
            Panel = panel;
            Glow = glow;
            Bridge = bridge;
            ScreenIdle = screenIdle;
            ScreenRunning = screenRunning;
            ScreenSuccess = screenSuccess;
            ScreenError = screenError;
            DarkBackdrop = darkBackdrop;
        }

        public Material Structure { get; }
        public Material Panel { get; }
        public Material Glow { get; }
        public Material Bridge { get; }
        public Material ScreenIdle { get; }
        public Material ScreenRunning { get; }
        public Material ScreenSuccess { get; }
        public Material ScreenError { get; }
        public Material DarkBackdrop { get; }
    }
}
