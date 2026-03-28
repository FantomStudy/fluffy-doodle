using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static partial class Level4SceneAssembler
{
    public static void BuildLevel4RoomGameplay()
    {
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        if (sceneAsset == null)
        {
            throw new InvalidOperationException($"Scene was not found: {ScenePath}");
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        BuildRoomGameplayOpenScene();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Level4 room gameplay assembled.");
    }

    private static void BuildRoomGameplayOpenScene()
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

        EnsureDirectionalLight();
        EnsureEventSystem();

        RectTransform fadeCanvasTransform = fadeCanvasObject.GetComponent<RectTransform>();
        if (fadeCanvasTransform == null)
        {
            throw new InvalidOperationException("FadeCanvas must contain a RectTransform.");
        }

        CanvasGroup fadeOverlayGroup = EnsureFadeOverlay(fadeCanvasTransform);
        CleanupFadeCanvas(fadeCanvasTransform);
        UiBundle uiBundle = CreateRoomUi(fadeCanvasTransform, fontAsset);

        SetObjectReference(screenFadePlayerLock, "fadeCanvasGroup", fadeOverlayGroup);
        SetObjectReference(screenFadePlayerLock, "playerRoot", playerRoot);
        SetFloat(screenFadePlayerLock, "fadeDuration", 0.35f);

        ConfigurePlayerInteractor(playerRoot, uiBundle);

        Transform floorsRoot = FindSceneObject("Floors");
        Transform roofsRoot = FindSceneObject("Roofs");
        if (floorsRoot == null || roofsRoot == null)
        {
            throw new InvalidOperationException("Level4 room shell is incomplete. Floors and Roofs roots are required.");
        }

        Bounds floorBounds = CalculateWorldBounds(floorsRoot.gameObject);
        Bounds roofBounds = CalculateWorldBounds(roofsRoot.gameObject);

        float floorTopY = floorBounds.max.y;
        float roofBottomY = roofBounds.min.y;
        float roomMinZ = floorBounds.min.z;
        float roomMaxZ = floorBounds.max.z;
        float laneX = floorBounds.center.x;
        float leftTerminalX = laneX - 2.55f;
        float displayX = laneX - 1.2f;
        float noteX = laneX + 2.55f;
        float deckWidth = Mathf.Clamp(floorBounds.size.x * 0.32f, 2.45f, 2.8f);
        float lowDeckTop = floorTopY + 2.2f;
        float highDeckTop = Mathf.Min(floorTopY + 4.2f, roofBottomY - 1.15f);
        float lowDeckY = lowDeckTop - 0.175f;
        float highDeckY = highDeckTop - 0.175f;
        float dividerWidth = Mathf.Min(floorBounds.size.x - 0.4f, 7.4f);

        ConfigureRoomPlayer(playerRoot, laneX, lowDeckTop, roomMinZ);

        Transform existingGameplay = FindSceneObject("Level4_Gameplay");
        if (existingGameplay != null)
        {
            UnityEngine.Object.DestroyImmediate(existingGameplay.gameObject);
        }

        Transform existingFinishTrigger = FindSceneObject("FinishTrigger");
        if (existingFinishTrigger != null)
        {
            UnityEngine.Object.DestroyImmediate(existingFinishTrigger.gameObject);
        }

        GameObject gameplayRoot = new GameObject("Level4_Gameplay");
        GameObject sectionsRoot = new GameObject("Sections");
        sectionsRoot.transform.SetParent(gameplayRoot.transform, false);
        GameObject displaysRoot = new GameObject("TerminalDisplays");
        displaysRoot.transform.SetParent(gameplayRoot.transform, false);
        GameObject zonesRoot = new GameObject("InteractionZones");
        zonesRoot.transform.SetParent(gameplayRoot.transform, false);
        GameObject signageRoot = new GameObject("Signage");
        signageRoot.transform.SetParent(gameplayRoot.transform, false);
        GameObject dividersRoot = new GameObject("Dividers");
        dividersRoot.transform.SetParent(gameplayRoot.transform, false);

        float divider1Z = roomMinZ + 9.8f;
        float divider2Z = roomMinZ + 20f;
        float divider3Z = roomMinZ + 28.7f;

        CreateRoomDivider(dividersRoot.transform, "Divider_01", laneX, divider1Z, dividerWidth, floorTopY, roofBottomY, lowDeckTop - 0.15f, lowDeckTop + 2.1f, materials);
        CreateRoomDivider(dividersRoot.transform, "Divider_02", laneX, divider2Z, dividerWidth, floorTopY, roofBottomY, lowDeckTop - 0.15f, lowDeckTop + 2.1f, materials);
        CreateRoomDivider(dividersRoot.transform, "Divider_03", laneX, divider3Z, dividerWidth, floorTopY, roofBottomY, highDeckTop - 0.2f, highDeckTop + 1.8f, materials);

        GameObject terminalFor01 = PrepareRoomTerminal(new[] { "Terminal_For_01", "terminal1" }, "Terminal_For_01", new Vector3(leftTerminalX, lowDeckTop, roomMinZ + 2.3f));
        GameObject terminalFor02 = PrepareRoomTerminal(new[] { "Terminal_For_02", "terminal2" }, "Terminal_For_02", new Vector3(leftTerminalX, lowDeckTop, roomMinZ + 12.15f));
        GameObject terminalWhile01 = PrepareRoomTerminal(new[] { "Terminal_While_01", "terminal3" }, "Terminal_While_01", new Vector3(leftTerminalX, lowDeckTop, roomMinZ + 21.95f));
        GameObject terminalFinal = PrepareRoomTerminal(new[] { "Terminal_Final", "Terminal_Combined_Final", "terminal4" }, "Terminal_Final", new Vector3(leftTerminalX, lowDeckTop, roomMinZ + 32.2f));

        BuildSection1(sectionsRoot.transform, displaysRoot.transform, zonesRoot.transform, signageRoot.transform, materials, fontAsset, laneX, leftTerminalX, displayX, noteX, floorTopY, lowDeckY, lowDeckTop, deckWidth, roomMinZ, terminalFor01);
        BuildSection2(sectionsRoot.transform, displaysRoot.transform, zonesRoot.transform, signageRoot.transform, materials, fontAsset, laneX, leftTerminalX, displayX, noteX, floorTopY, lowDeckY, lowDeckTop, deckWidth, roomMinZ, terminalFor02);
        BuildSection3(sectionsRoot.transform, displaysRoot.transform, zonesRoot.transform, signageRoot.transform, materials, fontAsset, laneX, leftTerminalX, displayX, noteX, floorTopY, lowDeckY, lowDeckTop, highDeckY, highDeckTop, deckWidth, roomMinZ, terminalWhile01);
        BuildSection4(sectionsRoot.transform, displaysRoot.transform, zonesRoot.transform, signageRoot.transform, materials, fontAsset, laneX, leftTerminalX, displayX, noteX, floorTopY, lowDeckY, lowDeckTop, highDeckY, highDeckTop, deckWidth, roomMinZ, roomMaxZ, terminalFinal, screenFadePlayerLock, uiBundle.FinishCanvasGroup, gameplayRoot.transform);
    }

    private static void BuildSection1(
        Transform sectionsRoot,
        Transform displaysRoot,
        Transform zonesRoot,
        Transform signageRoot,
        MaterialPalette materials,
        TMP_FontAsset fontAsset,
        float laneX,
        float leftTerminalX,
        float displayX,
        float noteX,
        float floorTopY,
        float lowDeckY,
        float lowDeckTop,
        float deckWidth,
        float roomMinZ,
        GameObject terminalRoot)
    {
        GameObject sectionRoot = new GameObject("Section1_For");
        sectionRoot.transform.SetParent(sectionsRoot, false);

        CreateTechPlatform("StartPlatform", sectionRoot.transform, new Vector3(laneX, lowDeckY, roomMinZ + 2.6f), new Vector3(deckWidth, 0.35f, 3.4f), materials, floorTopY, true);
        CreateTechPlatform("LandingPlatform", sectionRoot.transform, new Vector3(laneX, lowDeckY, roomMinZ + 8.05f), new Vector3(deckWidth, 0.35f, 2.5f), materials, floorTopY, true);
        CreateBleacherStairs("ReturnSteps", sectionRoot.transform, new Vector3(laneX + 1.9f, 0f, roomMinZ + 0.95f), 5, 1.3f, floorTopY, lowDeckTop - floorTopY, 0.56f, materials);
        CreateRotatedWorldNote("Section1Note", signageRoot, new Vector3(noteX, lowDeckTop + 1.05f, roomMinZ + 2.55f), Quaternion.Euler(0f, -90f, 0f), new Vector3(2.1f, 1.05f, 0.18f), "for i in range(2):\n    extend_bridge()", materials, fontAsset);

        GameObject bridgeRoot = new GameObject("Bridge_01");
        bridgeRoot.transform.SetParent(sectionRoot.transform, false);
        bridgeRoot.transform.position = new Vector3(laneX, lowDeckTop - 0.18f, roomMinZ + 4.9f);
        ForBridgeController bridgeController = bridgeRoot.AddComponent<ForBridgeController>();
        Transform[] segments = CreateBridgeSegments(bridgeRoot.transform, 2, 1.2f, materials.Bridge);
        ConfigureForBridge(bridgeController, segments, 0.16f, 0.08f, new Vector3(0f, -2f, 0f));

        TerminalIndicator indicator = CreateRoomTerminalDisplay(displaysRoot, "Terminal_For_01_Display", new Vector3(displayX, lowDeckTop + 1.22f, roomMinZ + 2.45f), Quaternion.Euler(0f, 90f, 0f), materials, fontAsset);
        ForTerminal terminal = terminalRoot.AddComponent<ForTerminal>();
        ConfigureRoomForTerminal(terminal, bridgeController, indicator, "\u0421\u0435\u043a\u0446\u0438\u044f 1 / FOR", 2);

        CreateRoomInteractionZone(zonesRoot, "Terminal_For_01_Zone", new Vector3(leftTerminalX + 0.95f, lowDeckTop + 1.2f, roomMinZ + 2.45f), new Vector3(2.4f, 2.7f, 2.3f), terminal);
    }

    private static void BuildSection2(
        Transform sectionsRoot,
        Transform displaysRoot,
        Transform zonesRoot,
        Transform signageRoot,
        MaterialPalette materials,
        TMP_FontAsset fontAsset,
        float laneX,
        float leftTerminalX,
        float displayX,
        float noteX,
        float floorTopY,
        float lowDeckY,
        float lowDeckTop,
        float deckWidth,
        float roomMinZ,
        GameObject terminalRoot)
    {
        GameObject sectionRoot = new GameObject("Section2_For");
        sectionRoot.transform.SetParent(sectionsRoot, false);

        CreateTechPlatform("StartPlatform", sectionRoot.transform, new Vector3(laneX, lowDeckY, roomMinZ + 12.55f), new Vector3(deckWidth, 0.35f, 3f), materials, floorTopY, true);
        CreateTechPlatform("LandingPlatform", sectionRoot.transform, new Vector3(laneX, lowDeckY, roomMinZ + 18.55f), new Vector3(deckWidth, 0.35f, 2.1f), materials, floorTopY, true);
        CreateBleacherStairs("ReturnSteps", sectionRoot.transform, new Vector3(laneX + 1.9f, 0f, roomMinZ + 10.75f), 5, 1.3f, floorTopY, lowDeckTop - floorTopY, 0.56f, materials);
        CreateRotatedWorldNote("Section2Note", signageRoot, new Vector3(noteX, lowDeckTop + 1.05f, roomMinZ + 12.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(2.1f, 1.05f, 0.18f), "for i in range(3):\n    extend_bridge()", materials, fontAsset);

        GameObject bridgeRoot = new GameObject("Bridge_02");
        bridgeRoot.transform.SetParent(sectionRoot.transform, false);
        bridgeRoot.transform.position = new Vector3(laneX, lowDeckTop - 0.18f, roomMinZ + 14.1f);
        ForBridgeController bridgeController = bridgeRoot.AddComponent<ForBridgeController>();
        Transform[] segments = CreateBridgeSegments(bridgeRoot.transform, 3, 1.15f, materials.Bridge);
        ConfigureForBridge(bridgeController, segments, 0.16f, 0.08f, new Vector3(0f, -2f, 0f));

        TerminalIndicator indicator = CreateRoomTerminalDisplay(displaysRoot, "Terminal_For_02_Display", new Vector3(displayX, lowDeckTop + 1.22f, roomMinZ + 12.3f), Quaternion.Euler(0f, 90f, 0f), materials, fontAsset);
        ForTerminal terminal = terminalRoot.AddComponent<ForTerminal>();
        ConfigureRoomForTerminal(terminal, bridgeController, indicator, "\u0421\u0435\u043a\u0446\u0438\u044f 2 / FOR", 3);

        CreateRoomInteractionZone(zonesRoot, "Terminal_For_02_Zone", new Vector3(leftTerminalX + 0.95f, lowDeckTop + 1.2f, roomMinZ + 12.3f), new Vector3(2.4f, 2.7f, 2.3f), terminal);
    }

    private static void BuildSection3(
        Transform sectionsRoot,
        Transform displaysRoot,
        Transform zonesRoot,
        Transform signageRoot,
        MaterialPalette materials,
        TMP_FontAsset fontAsset,
        float laneX,
        float leftTerminalX,
        float displayX,
        float noteX,
        float floorTopY,
        float lowDeckY,
        float lowDeckTop,
        float highDeckY,
        float highDeckTop,
        float deckWidth,
        float roomMinZ,
        GameObject terminalRoot)
    {
        GameObject sectionRoot = new GameObject("Section3_While");
        sectionRoot.transform.SetParent(sectionsRoot, false);

        CreateTechPlatform("GroundPlatform", sectionRoot.transform, new Vector3(laneX, lowDeckY, roomMinZ + 21.9f), new Vector3(deckWidth, 0.35f, 3f), materials, floorTopY, true);
        CreateTechPlatform("TargetPlatform_01", sectionRoot.transform, new Vector3(laneX, highDeckY, roomMinZ + 26.95f), new Vector3(deckWidth, 0.35f, 3.1f), materials, floorTopY, true);
        CreateBleacherStairs("ReturnSteps", sectionRoot.transform, new Vector3(laneX + 1.9f, 0f, roomMinZ + 20.75f), 5, 1.3f, floorTopY, lowDeckTop - floorTopY, 0.56f, materials);
        CreateRotatedWorldNote("Section3Note", signageRoot, new Vector3(noteX, lowDeckTop + 1.05f, roomMinZ + 21.9f), Quaternion.Euler(0f, -90f, 0f), new Vector3(2.15f, 1.05f, 0.18f), "while height < target:\n    lift_platform()", materials, fontAsset);

        GameObject liftRoot = new GameObject("LiftPlatform_01");
        liftRoot.transform.SetParent(sectionRoot.transform, false);
        liftRoot.transform.position = new Vector3(laneX, lowDeckTop - 0.35f, roomMinZ + 24.55f);
        CreateVisualCube("LiftFrame", liftRoot.transform, new Vector3(0f, 1.08f, 0f), new Vector3(2.65f, 2.16f, 2.65f), materials.DarkBackdrop);
        CreateVisualCube("LiftGlowLeft", liftRoot.transform, new Vector3(-1.08f, 1.08f, 0f), new Vector3(0.08f, 2f, 2.3f), materials.Glow);
        CreateVisualCube("LiftGlowRight", liftRoot.transform, new Vector3(1.08f, 1.08f, 0f), new Vector3(0.08f, 2f, 2.3f), materials.Glow);
        GameObject platform = CreateCube("PlatformVisual", liftRoot.transform, new Vector3(0f, 0.175f, 0f), new Vector3(2.2f, 0.35f, 2.2f), materials.Panel);

        WhileLiftController liftController = liftRoot.AddComponent<WhileLiftController>();
        ConfigureWhileLift(liftController, platform.transform, highDeckTop - lowDeckTop, 0.5f, 0.18f, 0.12f);

        TerminalIndicator indicator = CreateRoomTerminalDisplay(displaysRoot, "Terminal_While_01_Display", new Vector3(displayX, lowDeckTop + 1.22f, roomMinZ + 22.05f), Quaternion.Euler(0f, 90f, 0f), materials, fontAsset);
        WhileTerminal terminal = terminalRoot.AddComponent<WhileTerminal>();
        ConfigureRoomWhileTerminal(terminal, liftController, indicator, "\u0421\u0435\u043a\u0446\u0438\u044f 3 / WHILE", "while height < target");

        CreateRoomInteractionZone(zonesRoot, "Terminal_While_01_Zone", new Vector3(leftTerminalX + 0.95f, lowDeckTop + 1.2f, roomMinZ + 22.05f), new Vector3(2.4f, 2.7f, 2.3f), terminal);
    }

    private static void BuildSection4(
        Transform sectionsRoot,
        Transform displaysRoot,
        Transform zonesRoot,
        Transform signageRoot,
        MaterialPalette materials,
        TMP_FontAsset fontAsset,
        float laneX,
        float leftTerminalX,
        float displayX,
        float noteX,
        float floorTopY,
        float lowDeckY,
        float lowDeckTop,
        float highDeckY,
        float highDeckTop,
        float deckWidth,
        float roomMinZ,
        float roomMaxZ,
        GameObject terminalRoot,
        ScreenFadePlayerLock screenFadePlayerLock,
        CanvasGroup finishCanvasGroup,
        Transform gameplayRoot)
    {
        GameObject sectionRoot = new GameObject("Section4_Final");
        sectionRoot.transform.SetParent(sectionsRoot, false);

        CreateTechPlatform("HighEntryPlatform", sectionRoot.transform, new Vector3(laneX, highDeckY, roomMinZ + 29.35f), new Vector3(deckWidth, 0.35f, 1.6f), materials, floorTopY, true);
        CreateBleacherStairs("DropSteps", sectionRoot.transform, new Vector3(laneX, 0f, roomMinZ + 31.4f), 5, deckWidth, lowDeckTop, highDeckTop - lowDeckTop, 0.48f, materials, -1);
        CreateTechPlatform("LowStartPlatform", sectionRoot.transform, new Vector3(laneX, lowDeckY, roomMinZ + 32.5f), new Vector3(deckWidth, 0.35f, 2.6f), materials, floorTopY, true);
        CreateTechPlatform("LandingPlatform", sectionRoot.transform, new Vector3(laneX, lowDeckY, roomMinZ + 37.15f), new Vector3(deckWidth, 0.35f, 1.1f), materials, floorTopY, true);
        CreateTechPlatform("FinishPlatform", sectionRoot.transform, new Vector3(laneX, highDeckY, roomMinZ + 39.1f), new Vector3(deckWidth, 0.35f, 1.7f), materials, floorTopY, true);
        CreateBleacherStairs("ReturnSteps", sectionRoot.transform, new Vector3(laneX + 1.9f, 0f, roomMinZ + 30.55f), 5, 1.3f, floorTopY, lowDeckTop - floorTopY, 0.56f, materials);
        CreateRotatedWorldNote("Section4Note", signageRoot, new Vector3(noteX, lowDeckTop + 1.05f, roomMinZ + 32.55f), Quaternion.Euler(0f, -90f, 0f), new Vector3(2.25f, 1.05f, 0.18f), "for -> bridge\nwhile -> lift", materials, fontAsset);

        GameObject bridgeRoot = new GameObject("Bridge_Final");
        bridgeRoot.transform.SetParent(sectionRoot.transform, false);
        bridgeRoot.transform.position = new Vector3(laneX, lowDeckTop - 0.18f, roomMinZ + 33.8f);
        ForBridgeController bridgeController = bridgeRoot.AddComponent<ForBridgeController>();
        Transform[] segments = CreateBridgeSegments(bridgeRoot.transform, 3, 1f, materials.Bridge);
        ConfigureForBridge(bridgeController, segments, 0.16f, 0.08f, new Vector3(0f, -2f, 0f));

        GameObject liftRoot = new GameObject("LiftPlatform_Final");
        liftRoot.transform.SetParent(sectionRoot.transform, false);
        liftRoot.transform.position = new Vector3(laneX, lowDeckTop - 0.35f, roomMinZ + 38.45f);
        CreateVisualCube("LiftFrame", liftRoot.transform, new Vector3(0f, 1.08f, 0f), new Vector3(2.05f, 2.16f, 1.7f), materials.DarkBackdrop);
        CreateVisualCube("LiftGlowLeft", liftRoot.transform, new Vector3(-0.78f, 1.08f, 0f), new Vector3(0.08f, 2f, 1.45f), materials.Glow);
        CreateVisualCube("LiftGlowRight", liftRoot.transform, new Vector3(0.78f, 1.08f, 0f), new Vector3(0.08f, 2f, 1.45f), materials.Glow);
        GameObject platform = CreateCube("PlatformVisual", liftRoot.transform, new Vector3(0f, 0.175f, 0f), new Vector3(1.65f, 0.35f, 1.45f), materials.Panel);

        WhileLiftController liftController = liftRoot.AddComponent<WhileLiftController>();
        ConfigureWhileLift(liftController, platform.transform, highDeckTop - lowDeckTop, 0.5f, 0.18f, 0.12f);

        TerminalIndicator indicator = CreateRoomTerminalDisplay(displaysRoot, "Terminal_Final_Display", new Vector3(displayX, lowDeckTop + 1.22f, roomMinZ + 32.35f), Quaternion.Euler(0f, 90f, 0f), materials, fontAsset);
        CombinedCycleTerminal terminal = terminalRoot.AddComponent<CombinedCycleTerminal>();
        ConfigureCombinedRoomTerminal(terminal, bridgeController, liftController, indicator, "\u0424\u0438\u043d\u0430\u043b / FOR + WHILE", 3, "while lift < finish");

        CreateRoomInteractionZone(zonesRoot, "Terminal_Final_Zone", new Vector3(leftTerminalX + 0.95f, lowDeckTop + 1.2f, roomMinZ + 32.35f), new Vector3(2.4f, 2.7f, 2.3f), terminal);

        CreateVisualCube("FinishArchLeft", sectionRoot.transform, new Vector3(laneX - 1.55f, highDeckTop + 1.15f, roomMinZ + 39.85f), new Vector3(0.22f, 2.2f, 0.22f), materials.Structure);
        CreateVisualCube("FinishArchRight", sectionRoot.transform, new Vector3(laneX + 1.55f, highDeckTop + 1.15f, roomMinZ + 39.85f), new Vector3(0.22f, 2.2f, 0.22f), materials.Structure);
        CreateVisualCube("FinishArchTop", sectionRoot.transform, new Vector3(laneX, highDeckTop + 2.15f, roomMinZ + 39.85f), new Vector3(3.2f, 0.18f, 0.22f), materials.Glow);

        GameObject finishTriggerObject = new GameObject("FinishTrigger", typeof(BoxCollider), typeof(LevelFinishTrigger));
        finishTriggerObject.transform.SetParent(gameplayRoot, false);
        finishTriggerObject.transform.position = new Vector3(laneX, highDeckTop + 1.05f, roomMaxZ - 0.85f);
        BoxCollider finishCollider = finishTriggerObject.GetComponent<BoxCollider>();
        finishCollider.isTrigger = true;
        finishCollider.size = new Vector3(deckWidth, 2.3f, 1.8f);

        LevelFinishTrigger finishTrigger = finishTriggerObject.GetComponent<LevelFinishTrigger>();
        SetObjectReference(finishTrigger, "screenFadePlayerLock", screenFadePlayerLock);
        SetObjectReference(finishTrigger, "finishCanvasGroup", finishCanvasGroup);
        SetFloat(finishTrigger, "fadeDuration", 0.35f);
        SetFloat(finishTrigger, "overlayAlphaWhenComplete", 0.12f);
    }

    private static void ConfigureRoomPlayer(GameObject playerRoot, float laneX, float lowDeckTop, float roomMinZ)
    {
        playerRoot.transform.SetParent(null);
        playerRoot.transform.SetPositionAndRotation(new Vector3(laneX, lowDeckTop + 1.05f, roomMinZ + 1.1f), Quaternion.identity);
    }

    private static GameObject PrepareRoomTerminal(string[] candidateNames, string targetName, Vector3 worldPosition)
    {
        Transform terminalTransform = null;
        foreach (string candidateName in candidateNames)
        {
            terminalTransform = FindSceneObject(candidateName);
            if (terminalTransform != null)
            {
                break;
            }
        }

        if (terminalTransform == null)
        {
            throw new InvalidOperationException($"Terminal was not found for {targetName}.");
        }

        GameObject terminalRoot = terminalTransform.gameObject;
        terminalRoot.name = targetName;
        terminalRoot.transform.position = worldPosition;

        DestroyTerminalBehaviour<ForTerminal>(terminalRoot);
        DestroyTerminalBehaviour<WhileTerminal>(terminalRoot);
        DestroyTerminalBehaviour<CombinedCycleTerminal>(terminalRoot);
        DestroyTerminalBehaviour<TerminalIndicator>(terminalRoot);

        Transform existingTrigger = terminalRoot.transform.Find("InteractionTriggerZone");
        if (existingTrigger != null)
        {
            UnityEngine.Object.DestroyImmediate(existingTrigger.gameObject);
        }

        return terminalRoot;
    }

    private static void DestroyTerminalBehaviour<T>(GameObject target) where T : Component
    {
        foreach (T component in target.GetComponents<T>())
        {
            UnityEngine.Object.DestroyImmediate(component);
        }
    }

    private static UiBundle CreateRoomUi(Transform fadeCanvasTransform, TMP_FontAsset fontAsset)
    {
        RectTransform lessonCard = CreatePanel("LessonCard", fadeCanvasTransform, new Vector2(0.03f, 0.76f), new Vector2(0.31f, 0.95f), new Color(0.97f, 0.97f, 0.98f, 0.96f));
        CreatePanel("Accent", lessonCard, new Vector2(0f, 0f), new Vector2(0.04f, 1f), new Color(0.47f, 0.34f, 0.92f, 1f));
        CreateText("LessonTitle", lessonCard, new Vector2(0.08f, 0.62f), new Vector2(0.95f, 0.92f), Vector2.zero, Vector2.zero, "\u0423\u0440\u043e\u043a 4: \u0426\u0438\u043a\u043b\u044b", fontAsset, 30, new Color(0.14f, 0.16f, 0.2f, 1f), TextAlignmentOptions.Left);
        CreateText("LessonBody", lessonCard, new Vector2(0.08f, 0.14f), new Vector2(0.95f, 0.6f), Vector2.zero, Vector2.zero, "FOR \u043f\u043e\u043a\u0430\u0437\u044b\u0432\u0430\u0435\u0442 \u0444\u0438\u043a\u0441\u0438\u0440\u043e\u0432\u0430\u043d\u043d\u043e\u0435 \u0447\u0438\u0441\u043b\u043e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u0439.\nWHILE \u043f\u043e\u0432\u0442\u043e\u0440\u044f\u0435\u0442 \u0434\u0435\u0439\u0441\u0442\u0432\u0438\u0435, \u043f\u043e\u043a\u0430 \u0443\u0441\u043b\u043e\u0432\u0438\u0435 \u0438\u0441\u0442\u0438\u043d\u043d\u043e.", fontAsset, 18, new Color(0.48f, 0.5f, 0.56f, 1f), TextAlignmentOptions.TopLeft);

        RectTransform progressCard = CreatePanel("ProgressCard", fadeCanvasTransform, new Vector2(0.77f, 0.84f), new Vector2(0.96f, 0.94f), new Color(0.97f, 0.97f, 0.98f, 0.96f));
        CreateText("ProgressTitle", progressCard, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.88f), Vector2.zero, Vector2.zero, "\u0423\u0440\u043e\u043a 4 \u0438\u0437 5", fontAsset, 20, new Color(0.14f, 0.16f, 0.2f, 1f), TextAlignmentOptions.Left);
        CreateText("ProgressBody", progressCard, new Vector2(0.08f, 0.14f), new Vector2(0.92f, 0.5f), Vector2.zero, Vector2.zero, "\u041a\u043e\u043c\u043d\u0430\u0442\u0430-\u043f\u0440\u043e\u0442\u043e\u0442\u0438\u043f \u043f\u043e Python-\u0446\u0438\u043a\u043b\u0430\u043c", fontAsset, 14, new Color(0.48f, 0.5f, 0.56f, 1f), TextAlignmentOptions.Left);

        RectTransform promptRoot = CreatePanel("InteractionPrompt", fadeCanvasTransform, new Vector2(0.29f, 0.04f), new Vector2(0.71f, 0.14f), new Color(0.98f, 0.98f, 0.99f, 0.98f));
        CreatePanel("PromptAccent", promptRoot, new Vector2(0f, 0f), new Vector2(0.03f, 1f), new Color(0.26f, 0.76f, 0.34f, 1f));
        CreateText("PromptTitle", promptRoot, new Vector2(0.07f, 0.52f), new Vector2(0.93f, 0.9f), Vector2.zero, Vector2.zero, "\u0412\u0437\u0430\u0438\u043c\u043e\u0434\u0435\u0439\u0441\u0442\u0432\u0438\u0435", fontAsset, 20, new Color(0.14f, 0.16f, 0.2f, 1f), TextAlignmentOptions.Left);
        TextMeshProUGUI promptText = CreateText("PromptText", promptRoot, new Vector2(0.07f, 0.12f), new Vector2(0.93f, 0.56f), Vector2.zero, Vector2.zero, "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0438\u0441\u043f\u043e\u043b\u044c\u0437\u043e\u0432\u0430\u0442\u044c \u0442\u0435\u0440\u043c\u0438\u043d\u0430\u043b", fontAsset, 16, new Color(0.43f, 0.45f, 0.51f, 1f), TextAlignmentOptions.Left);
        promptRoot.gameObject.SetActive(false);

        RectTransform finishRoot = CreatePanel("Level4FinishPanel", fadeCanvasTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Color(0.97f, 0.97f, 0.98f, 0.98f), new Vector2(620f, 260f));
        CanvasGroup finishCanvasGroup = finishRoot.gameObject.AddComponent<CanvasGroup>();
        finishCanvasGroup.alpha = 0f;
        finishCanvasGroup.blocksRaycasts = false;
        finishCanvasGroup.interactable = false;

        CreatePanel("FinishAccent", finishRoot, new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.88f), new Color(0.26f, 0.76f, 0.34f, 1f));
        CreateText("FinishTitle", finishRoot, new Vector2(0.1f, 0.54f), new Vector2(0.9f, 0.78f), Vector2.zero, Vector2.zero, "\u0423\u0440\u043e\u0432\u0435\u043d\u044c \u043f\u0440\u043e\u0439\u0434\u0435\u043d", fontAsset, 40, new Color(0.14f, 0.16f, 0.2f, 1f), TextAlignmentOptions.Center);
        CreateText("FinishBody", finishRoot, new Vector2(0.12f, 0.26f), new Vector2(0.88f, 0.5f), Vector2.zero, Vector2.zero, "\u0412\u044b \u043a\u043e\u0440\u043e\u0442\u043a\u043e \u043f\u0440\u043e\u0448\u043b\u0438 \u0432\u0441\u0435 4 \u0441\u0435\u043a\u0446\u0438\u0438 \u0438 \u0443\u0432\u0438\u0434\u0435\u043b\u0438 \u0440\u0430\u0437\u043d\u0438\u0446\u0443 \u043c\u0435\u0436\u0434\u0443 for \u0438 while.", fontAsset, 22, new Color(0.43f, 0.45f, 0.51f, 1f), TextAlignmentOptions.Center);

        return new UiBundle(promptRoot.gameObject, promptText, finishCanvasGroup);
    }

    private static void ConfigureRoomForTerminal(ForTerminal terminal, ForBridgeController bridgeController, TerminalIndicator indicator, string sectionTitle, int requiredCount)
    {
        SetObjectReference(terminal, "bridgeController", bridgeController);
        SetObjectReference(terminal, "terminalIndicator", indicator);
        SetString(terminal, "interactionPrompt", "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0432\u044b\u043f\u043e\u043b\u043d\u0438\u0442\u044c \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0443\u044e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u044e for");
        SetString(terminal, "sectionTitle", sectionTitle);
        SetIntegerArray(terminal, "availableCounts", new[] { requiredCount });
        SetInteger(terminal, "requiredCount", requiredCount);
        SetBool(terminal, "createInteractionTrigger", false);
        EditorUtility.SetDirty(terminal);
    }

    private static void ConfigureRoomWhileTerminal(WhileTerminal terminal, WhileLiftController liftController, TerminalIndicator indicator, string sectionTitle, string conditionLabel)
    {
        SetObjectReference(terminal, "liftController", liftController);
        SetObjectReference(terminal, "terminalIndicator", indicator);
        SetString(terminal, "interactionPrompt", "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0437\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u044c \u0446\u0438\u043a\u043b while");
        SetString(terminal, "sectionTitle", sectionTitle);
        SetString(terminal, "conditionLabel", conditionLabel);
        SetBool(terminal, "createInteractionTrigger", false);
        EditorUtility.SetDirty(terminal);
    }

    private static void ConfigureCombinedRoomTerminal(CombinedCycleTerminal terminal, ForBridgeController bridgeController, WhileLiftController liftController, TerminalIndicator indicator, string sectionTitle, int requiredCount, string conditionLabel)
    {
        SetObjectReference(terminal, "bridgeController", bridgeController);
        SetObjectReference(terminal, "liftController", liftController);
        SetObjectReference(terminal, "terminalIndicator", indicator);
        SetString(terminal, "forPrompt", "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0432\u044b\u043f\u043e\u043b\u043d\u0438\u0442\u044c \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0443\u044e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u044e for");
        SetString(terminal, "whilePrompt", "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0437\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u044c \u0446\u0438\u043a\u043b while");
        SetString(terminal, "resetPrompt", "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0441\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0441\u0435\u043a\u0446\u0438\u044e");
        SetString(terminal, "sectionTitle", sectionTitle);
        SetString(terminal, "whileConditionLabel", conditionLabel);
        SetInteger(terminal, "requiredCount", requiredCount);
        SetBool(terminal, "createInteractionTrigger", false);
        EditorUtility.SetDirty(terminal);
    }

    private static TerminalIndicator CreateRoomTerminalDisplay(Transform parent, string name, Vector3 worldPosition, Quaternion worldRotation, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        GameObject displayRoot = new GameObject(name);
        displayRoot.transform.SetParent(parent, false);
        displayRoot.transform.position = worldPosition;
        displayRoot.transform.rotation = worldRotation;

        TerminalIndicator indicator = displayRoot.AddComponent<TerminalIndicator>();

        CreateVisualCube("Frame", displayRoot.transform, Vector3.zero, new Vector3(1.6f, 1.08f, 0.12f), materials.Structure);
        GameObject screen = CreateVisualCube("Screen", displayRoot.transform, new Vector3(0f, 0f, 0.03f), new Vector3(1.42f, 0.86f, 0.04f), materials.ScreenIdle);
        GameObject idleIndicator = CreateVisualCube("IdleIndicator", displayRoot.transform, new Vector3(-0.38f, 0.51f, 0.03f), new Vector3(0.18f, 0.08f, 0.08f), materials.ScreenIdle);
        GameObject runningIndicator = CreateVisualCube("RunningIndicator", displayRoot.transform, new Vector3(-0.12f, 0.51f, 0.03f), new Vector3(0.18f, 0.08f, 0.08f), materials.ScreenRunning);
        GameObject successIndicator = CreateVisualCube("SuccessIndicator", displayRoot.transform, new Vector3(0.12f, 0.51f, 0.03f), new Vector3(0.18f, 0.08f, 0.08f), materials.ScreenSuccess);
        GameObject errorIndicator = CreateVisualCube("ErrorIndicator", displayRoot.transform, new Vector3(0.38f, 0.51f, 0.03f), new Vector3(0.18f, 0.08f, 0.08f), materials.ScreenError);

        TextMeshPro titleText = CreateWorldText("TitleText", displayRoot.transform, new Vector3(0f, 0.16f, 0.05f), "\u0422\u0435\u0440\u043c\u0438\u043d\u0430\u043b", fontAsset, 4.8f, new Color(0.13f, 0.16f, 0.2f, 1f), new Vector2(1.34f, 0.24f), TextAlignmentOptions.Center, new Vector3(0.09f, 0.09f, 0.09f));
        TextMeshPro bodyText = CreateWorldText("BodyText", displayRoot.transform, new Vector3(0f, -0.08f, 0.05f), "for i in range(2)", fontAsset, 3.8f, new Color(0.29f, 0.33f, 0.4f, 1f), new Vector2(1.3f, 0.42f), TextAlignmentOptions.Center, new Vector3(0.076f, 0.076f, 0.076f));
        TextMeshPro statusText = CreateWorldText("StatusText", displayRoot.transform, new Vector3(0f, -0.42f, 0.05f), "\u041e\u0436\u0438\u0434\u0430\u043d\u0438\u0435", fontAsset, 4f, new Color(0.26f, 0.76f, 0.34f, 1f), new Vector2(1.22f, 0.2f), TextAlignmentOptions.Center, new Vector3(0.076f, 0.076f, 0.076f));

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

        return indicator;
    }

    private static void CreateRoomInteractionZone(Transform parent, string name, Vector3 worldPosition, Vector3 size, MonoBehaviour target)
    {
        GameObject zoneObject = new GameObject(name, typeof(BoxCollider), typeof(InteractionTriggerZone));
        zoneObject.transform.SetParent(parent, false);
        zoneObject.transform.position = worldPosition;

        BoxCollider triggerCollider = zoneObject.GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = Vector3.zero;
        triggerCollider.size = size;

        InteractionTriggerZone triggerZone = zoneObject.GetComponent<InteractionTriggerZone>();
        triggerZone.SetTarget(target);
    }

    private static void CreateRoomDivider(Transform parent, string name, float centerX, float worldZ, float totalWidth, float floorTopY, float roofBottomY, float slotBottomY, float slotTopY, MaterialPalette materials)
    {
        GameObject dividerRoot = new GameObject(name);
        dividerRoot.transform.SetParent(parent, false);

        float wallHeight = roofBottomY - floorTopY;
        float wallCenterY = floorTopY + (wallHeight * 0.5f);
        float slotWidth = 3.2f;
        float sideWidth = Mathf.Max(0.5f, (totalWidth - slotWidth) * 0.5f);
        float leftCenterX = centerX - ((slotWidth * 0.5f) + (sideWidth * 0.5f));
        float rightCenterX = centerX + ((slotWidth * 0.5f) + (sideWidth * 0.5f));

        CreateCube("LeftWall", dividerRoot.transform, new Vector3(leftCenterX, wallCenterY, worldZ), new Vector3(sideWidth, wallHeight, 0.35f), materials.Structure);
        CreateCube("RightWall", dividerRoot.transform, new Vector3(rightCenterX, wallCenterY, worldZ), new Vector3(sideWidth, wallHeight, 0.35f), materials.Structure);

        float bottomHeight = Mathf.Max(0.2f, slotBottomY - floorTopY);
        CreateCube("BottomWall", dividerRoot.transform, new Vector3(centerX, floorTopY + (bottomHeight * 0.5f), worldZ), new Vector3(slotWidth, bottomHeight, 0.35f), materials.Structure);

        float topHeight = Mathf.Max(0.2f, roofBottomY - slotTopY);
        CreateCube("TopWall", dividerRoot.transform, new Vector3(centerX, slotTopY + (topHeight * 0.5f), worldZ), new Vector3(slotWidth, topHeight, 0.35f), materials.Structure);

        CreateVisualCube("GlowStrip", dividerRoot.transform, new Vector3(centerX, slotTopY + 0.02f, worldZ), new Vector3(slotWidth, 0.05f, 0.08f), materials.Glow);
    }

    private static void CreateBleacherStairs(string name, Transform parent, Vector3 start, int stepCount, float width, float baseTopY, float totalRise, float stepDepth, MaterialPalette materials, int directionZ = 1)
    {
        GameObject stairsRoot = new GameObject(name);
        stairsRoot.transform.SetParent(parent, false);

        float stepRise = totalRise / Mathf.Max(1, stepCount);
        for (int i = 0; i < stepCount; i++)
        {
            float height = stepRise * (i + 1);
            float centerY = baseTopY + (height * 0.5f);
            float centerZ = start.z + (directionZ * ((i * stepDepth) + (stepDepth * 0.5f)));
            CreateTechPlatform($"Step_{i + 1:00}", stairsRoot.transform, new Vector3(start.x, centerY, centerZ), new Vector3(width, height, stepDepth), materials, baseTopY, false);
        }
    }

    private static GameObject CreateTechPlatform(string name, Transform parent, Vector3 center, Vector3 size, MaterialPalette materials, float floorTopY, bool addSupports)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = center;

        CreateCube("Base", root.transform, Vector3.zero, size, materials.Structure);
        CreateVisualCube("Surface", root.transform, new Vector3(0f, (size.y * 0.5f) + 0.02f, 0f), new Vector3(size.x * 0.92f, 0.04f, size.z * 0.92f), materials.Panel);
        CreateVisualCube("GlowLeft", root.transform, new Vector3(-(size.x * 0.5f) + 0.04f, (size.y * 0.5f) + 0.03f, 0f), new Vector3(0.08f, 0.04f, size.z * 0.92f), materials.Glow);
        CreateVisualCube("GlowRight", root.transform, new Vector3((size.x * 0.5f) - 0.04f, (size.y * 0.5f) + 0.03f, 0f), new Vector3(0.08f, 0.04f, size.z * 0.92f), materials.Glow);

        if (!addSupports)
        {
            return root;
        }

        float supportHeight = (center.y - (size.y * 0.5f)) - floorTopY;
        if (supportHeight <= 0.2f)
        {
            return root;
        }

        float supportZ = Mathf.Min(size.z * 0.32f, 0.9f);
        float supportX = Mathf.Min(size.x * 0.3f, 0.75f);
        float supportY = -(size.y * 0.5f) - (supportHeight * 0.5f);

        CreateCube("Support_A", root.transform, new Vector3(-supportX, supportY, -supportZ), new Vector3(0.22f, supportHeight, 0.22f), materials.Structure);
        CreateCube("Support_B", root.transform, new Vector3(supportX, supportY, supportZ), new Vector3(0.22f, supportHeight, 0.22f), materials.Structure);
        return root;
    }

    private static GameObject CreateVisualCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject cube = CreateCube(name, parent, localPosition, localScale, material);
        Collider collider = cube.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.DestroyImmediate(collider);
        }

        return cube;
    }

    private static void CreateRotatedWorldNote(string name, Transform parent, Vector3 worldPosition, Quaternion worldRotation, Vector3 panelScale, string body, MaterialPalette materials, TMP_FontAsset fontAsset)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = worldPosition;
        root.transform.rotation = worldRotation;
        CreateWorldNote(root.transform, Vector3.zero, panelScale, body, materials, fontAsset);
        RemoveColliders(root);
    }

    private static Bounds CalculateWorldBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            return new Bounds(root.transform.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}
