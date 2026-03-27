using System;
using StarterAssets;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Level3SceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Level3.unity";
    private const string FloorMaterialPath = "Assets/Materials/BlackOutside.mat";
    private const string StructureMaterialPath = "Assets/Materials/TerminalAccent.mat";
    private const string GlowMaterialPath = "Assets/Materials/BridgeGlow.mat";
    private const string FontAssetPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
    private const string GeneratedMaterialFolder = "Assets/Materials/IfLevel";
    private const string ObjectMaterialPath = GeneratedMaterialFolder + "/IfObjectNeutral.mat";
    private const string DoorMaterialPath = GeneratedMaterialFolder + "/IfDoor.mat";
    private const string SuccessMaterialPath = GeneratedMaterialFolder + "/IfSuccess.mat";
    private const string ErrorMaterialPath = GeneratedMaterialFolder + "/IfError.mat";

    [MenuItem("Tools/Legacy/Build Level3 Primitive Prototype (Deprecated)")]
    public static void BuildLevel3()
    {
        EditorUtility.DisplayDialog(
            "Deprecated Tool",
            "Этот старый builder собирает примитивный прототип и больше не подходит для текущего Level3.\n\nИспользуй новые non-destructive инструменты из меню Tools/Level3.",
            "OK");
    }

    private static void BuildOpenScene()
    {
        Material floorMaterial = LoadRequiredAsset<Material>(FloorMaterialPath);
        Material structureMaterial = LoadRequiredAsset<Material>(StructureMaterialPath);
        Material glowMaterial = LoadRequiredAsset<Material>(GlowMaterialPath);
        TMP_FontAsset fontAsset = LoadRequiredAsset<TMP_FontAsset>(FontAssetPath);

        EnsureGeneratedMaterialFolder();
        Material objectMaterial = CreateOrUpdateMaterial(ObjectMaterialPath, new Color(0.78f, 0.82f, 0.88f), 0.3f, 0.05f, Color.black);
        Material doorMaterial = CreateOrUpdateMaterial(DoorMaterialPath, new Color(0.11f, 0.18f, 0.24f), 0.55f, 0.2f, Color.black);
        Material successMaterial = CreateOrUpdateMaterial(SuccessMaterialPath, new Color(0.12f, 0.76f, 0.28f), 0.72f, 0.1f, new Color(0.2f, 1.8f, 0.45f));
        Material errorMaterial = CreateOrUpdateMaterial(ErrorMaterialPath, new Color(0.78f, 0.18f, 0.22f), 0.58f, 0.05f, new Color(2f, 0.18f, 0.18f));

        ThirdPersonController playerController = FindRequiredObject<ThirdPersonController>("A ThirdPersonController player is required in Level3.");
        GameObject playerRoot = playerController.transform.root.gameObject;
        playerRoot.transform.SetParent(null);
        playerRoot.transform.SetPositionAndRotation(new Vector3(0f, 1.5f, -10.75f), Quaternion.identity);

        GameObject fadeCanvasObject = FindRequiredSceneObject("FadeCanvas");
        ScreenFadePlayerLock screenFadePlayerLock = fadeCanvasObject.GetComponent<ScreenFadePlayerLock>();
        if (screenFadePlayerLock == null)
        {
            throw new InvalidOperationException("FadeCanvas must contain ScreenFadePlayerLock.");
        }

        RemoveExistingPrototypeRoots(playerRoot, fadeCanvasObject);
        RemoveExistingPlayerInteractor();
        CleanupFadeCanvas(fadeCanvasObject.transform);
        EnsureEventSystem();

        SetObjectReference(screenFadePlayerLock, "playerRoot", playerRoot);

        PromptBundle promptBundle = CreateInteractionPrompt(fadeCanvasObject.transform, fontAsset);
        CanvasGroup finishCanvasGroup = CreateFinishPanel(fadeCanvasObject.transform, fontAsset);
        ConfigurePlayerInteractor(playerRoot, promptBundle);

        GameObject prototypeRoot = new GameObject("Level3Prototype");

        CreateEnvironment(prototypeRoot.transform, floorMaterial, structureMaterial, glowMaterial);

        SelectionManager selectionManager = CreateSelectionManager(prototypeRoot.transform);
        CreateSelectionZone(prototypeRoot.transform, selectionManager, structureMaterial, objectMaterial, glowMaterial);

        SimpleDoor door = CreateDoor(prototypeRoot.transform, structureMaterial, doorMaterial);
        CreateTerminal(
            prototypeRoot.transform,
            selectionManager,
            door,
            structureMaterial,
            glowMaterial,
            successMaterial,
            errorMaterial,
            IfObjectId.Sphere);

        CreateFinishZone(prototypeRoot.transform, structureMaterial, glowMaterial, finishCanvasGroup, screenFadePlayerLock);
    }

    private static void CreateEnvironment(Transform parent, Material floorMaterial, Material structureMaterial, Material glowMaterial)
    {
        CreateCube("MainFloor", parent, new Vector3(0f, -0.25f, 2.5f), new Vector3(8f, 0.5f, 31f), floorMaterial);
        CreateCube("SelectionPlate", parent, new Vector3(0f, 0.02f, -0.5f), new Vector3(6.8f, 0.04f, 8f), structureMaterial);
        CreateCube("PostDoorPlate", parent, new Vector3(0f, 0.02f, 14f), new Vector3(4.6f, 0.04f, 7f), structureMaterial);
        CreateCube("LeftWall", parent, new Vector3(-4.25f, 2f, 2.5f), new Vector3(0.5f, 4f, 31f), floorMaterial);
        CreateCube("RightWall", parent, new Vector3(4.25f, 2f, 2.5f), new Vector3(0.5f, 4f, 31f), floorMaterial);
        CreateCube("BackWall", parent, new Vector3(0f, 2f, -13f), new Vector3(8.5f, 4f, 0.5f), floorMaterial);
        CreateCube("EndWall", parent, new Vector3(0f, 2f, 18f), new Vector3(8.5f, 4f, 0.5f), floorMaterial);
        CreateCube("Roof", parent, new Vector3(0f, 4.25f, 2.5f), new Vector3(8.5f, 0.5f, 31f), floorMaterial);
        CreateCube("GlowStripLeft", parent, new Vector3(-3.55f, 0.05f, 2.5f), new Vector3(0.12f, 0.04f, 29f), glowMaterial);
        CreateCube("GlowStripRight", parent, new Vector3(3.55f, 0.05f, 2.5f), new Vector3(0.12f, 0.04f, 29f), glowMaterial);
        CreateCube("SelectionFrameTop", parent, new Vector3(0f, 3.4f, -4.5f), new Vector3(6.5f, 0.25f, 0.35f), structureMaterial);
        CreateCube("SelectionFrameLeft", parent, new Vector3(-3.1f, 1.9f, -4.5f), new Vector3(0.25f, 2.8f, 0.35f), structureMaterial);
        CreateCube("SelectionFrameRight", parent, new Vector3(3.1f, 1.9f, -4.5f), new Vector3(0.25f, 2.8f, 0.35f), structureMaterial);
        CreateCube("AfterDoorFrameTop", parent, new Vector3(0f, 3.4f, 11.8f), new Vector3(3.9f, 0.25f, 0.35f), structureMaterial);
        CreateCube("AfterDoorFrameLeft", parent, new Vector3(-1.85f, 1.9f, 11.8f), new Vector3(0.25f, 2.8f, 0.35f), structureMaterial);
        CreateCube("AfterDoorFrameRight", parent, new Vector3(1.85f, 1.9f, 11.8f), new Vector3(0.25f, 2.8f, 0.35f), structureMaterial);
    }

    private static SelectionManager CreateSelectionManager(Transform parent)
    {
        GameObject managerObject = new GameObject("SelectionManager");
        managerObject.transform.SetParent(parent, false);
        return managerObject.AddComponent<SelectionManager>();
    }

    private static void CreateSelectionZone(
        Transform parent,
        SelectionManager selectionManager,
        Material pedestalMaterial,
        Material objectMaterial,
        Material selectedMaterial)
    {
        CreateSelectableObject(parent, selectionManager, "SelectableCube", IfObjectId.Cube, PrimitiveType.Cube, new Vector3(-2.4f, 0f, -0.8f), pedestalMaterial, objectMaterial, selectedMaterial);
        CreateSelectableObject(parent, selectionManager, "SelectableSphere", IfObjectId.Sphere, PrimitiveType.Sphere, new Vector3(0f, 0f, -0.8f), pedestalMaterial, objectMaterial, selectedMaterial);
        CreateSelectableObject(parent, selectionManager, "SelectableCylinder", IfObjectId.Cylinder, PrimitiveType.Cylinder, new Vector3(2.4f, 0f, -0.8f), pedestalMaterial, objectMaterial, selectedMaterial);
    }

    private static void CreateSelectableObject(
        Transform parent,
        SelectionManager selectionManager,
        string name,
        IfObjectId objectId,
        PrimitiveType primitiveType,
        Vector3 localPosition,
        Material pedestalMaterial,
        Material objectMaterial,
        Material selectedMaterial)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.localPosition = localPosition;

        SelectableObject selectableObject = root.AddComponent<SelectableObject>();

        CreateCube("Pedestal", root.transform, new Vector3(0f, 0.6f, 0f), new Vector3(1.2f, 1.2f, 1.2f), pedestalMaterial);
        CreateCube("PedestalTrim", root.transform, new Vector3(0f, 1.13f, 0f), new Vector3(1.4f, 0.08f, 1.4f), pedestalMaterial);
        GameObject selectionRing = CreatePrimitive("SelectionRing", PrimitiveType.Cylinder, root.transform, new Vector3(0f, 1.18f, 0f), new Vector3(0.9f, 0.04f, 0.9f), selectedMaterial);
        selectionRing.SetActive(false);

        Vector3 objectScale = primitiveType switch
        {
            PrimitiveType.Cylinder => new Vector3(0.75f, 0.55f, 0.75f),
            PrimitiveType.Sphere => new Vector3(0.92f, 0.92f, 0.92f),
            _ => new Vector3(0.9f, 0.9f, 0.9f),
        };

        float objectHeight = primitiveType == PrimitiveType.Cylinder ? 1.72f : 1.64f;
        GameObject objectVisual = CreatePrimitive("ObjectVisual", primitiveType, root.transform, new Vector3(0f, objectHeight, 0f), objectScale, objectMaterial);
        CreateCube("BackAccent", root.transform, new Vector3(0f, 1.9f, -0.82f), new Vector3(1.1f, 1.1f, 0.15f), pedestalMaterial);

        SetEnum(selectableObject, "objectId", (int)objectId);
        SetObjectReference(selectableObject, "selectionManager", selectionManager);
        SetString(selectableObject, "interactionPrompt", $"Press E to select {objectId}");
        SetObjectReference(selectableObject, "selectedVisualRoot", selectionRing);
        SetObjectReference(selectableObject, "objectRenderer", objectVisual.GetComponent<Renderer>());
        SetObjectReference(selectableObject, "defaultMaterial", objectMaterial);
        SetObjectReference(selectableObject, "selectedMaterial", selectedMaterial);
    }

    private static SimpleDoor CreateDoor(Transform parent, Material frameMaterial, Material doorMaterial)
    {
        CreateCube("DoorFrameLeft", parent, new Vector3(-1.65f, 1.75f, 8.6f), new Vector3(0.35f, 3.5f, 0.6f), frameMaterial);
        CreateCube("DoorFrameRight", parent, new Vector3(1.65f, 1.75f, 8.6f), new Vector3(0.35f, 3.5f, 0.6f), frameMaterial);
        CreateCube("DoorFrameTop", parent, new Vector3(0f, 3.42f, 8.6f), new Vector3(3.65f, 0.35f, 0.6f), frameMaterial);

        GameObject doorRoot = new GameObject("DoorRoot");
        doorRoot.transform.SetParent(parent, false);
        doorRoot.transform.localPosition = new Vector3(0f, 0f, 8.6f);

        GameObject doorVisual = CreateCube("DoorVisual", doorRoot.transform, new Vector3(0f, 1.6f, 0f), new Vector3(2.4f, 3.2f, 0.35f), doorMaterial);
        SimpleDoor door = doorRoot.AddComponent<SimpleDoor>();
        SetObjectReference(door, "doorTransform", doorVisual.transform);
        SetVector3(door, "openLocalOffset", new Vector3(0f, 3.6f, 0f));
        SetFloat(door, "openSpeed", 3.2f);
        return door;
    }

    private static void CreateTerminal(
        Transform parent,
        SelectionManager selectionManager,
        SimpleDoor door,
        Material frameMaterial,
        Material idleMaterial,
        Material successMaterial,
        Material errorMaterial,
        IfObjectId correctObjectId)
    {
        GameObject terminalRoot = new GameObject("IfTerminal");
        terminalRoot.transform.SetParent(parent, false);
        terminalRoot.transform.localPosition = new Vector3(2.7f, 0f, 6.95f);
        terminalRoot.transform.localRotation = Quaternion.Euler(0f, -24f, 0f);

        IfTerminal ifTerminal = terminalRoot.AddComponent<IfTerminal>();
        CreateCube("Base", terminalRoot.transform, new Vector3(0f, 0.18f, 0f), new Vector3(1.15f, 0.36f, 0.9f), frameMaterial);
        CreateCube("Stand", terminalRoot.transform, new Vector3(0f, 0.95f, 0f), new Vector3(0.22f, 1.45f, 0.22f), frameMaterial);
        GameObject screen = CreateCube("Screen", terminalRoot.transform, new Vector3(0f, 1.82f, 0.18f), new Vector3(1.4f, 0.82f, 0.14f), idleMaterial);
        CreateCube("ScreenHood", terminalRoot.transform, new Vector3(0f, 2.23f, 0.05f), new Vector3(1.05f, 0.12f, 0.45f), frameMaterial);

        GameObject idleIndicator = CreateCube("IdleIndicator", terminalRoot.transform, new Vector3(0f, 2.45f, 0.14f), new Vector3(0.22f, 0.12f, 0.12f), idleMaterial);
        GameObject successIndicator = CreateCube("SuccessIndicator", terminalRoot.transform, new Vector3(-0.32f, 2.45f, 0.14f), new Vector3(0.22f, 0.12f, 0.12f), successMaterial);
        GameObject errorIndicator = CreateCube("ErrorIndicator", terminalRoot.transform, new Vector3(0.32f, 2.45f, 0.14f), new Vector3(0.22f, 0.12f, 0.12f), errorMaterial);
        GameObject targetPreview = CreateTargetPreview(terminalRoot.transform, correctObjectId, idleMaterial);
        targetPreview.name = "TargetPreview";

        SetObjectReference(ifTerminal, "selectionManager", selectionManager);
        SetEnum(ifTerminal, "correctObjectId", (int)correctObjectId);
        SetObjectReference(ifTerminal, "door", door);
        SetString(ifTerminal, "interactionPrompt", $"Press E to check if selected == {correctObjectId}");
        SetObjectReference(ifTerminal, "screenRenderer", screen.GetComponent<Renderer>());
        SetObjectReference(ifTerminal, "idleScreenMaterial", idleMaterial);
        SetObjectReference(ifTerminal, "successScreenMaterial", successMaterial);
        SetObjectReference(ifTerminal, "errorScreenMaterial", errorMaterial);
        SetObjectReference(ifTerminal, "idleStateRoot", idleIndicator);
        SetObjectReference(ifTerminal, "successStateRoot", successIndicator);
        SetObjectReference(ifTerminal, "errorStateRoot", errorIndicator);
    }

    private static GameObject CreateTargetPreview(Transform parent, IfObjectId correctObjectId, Material material)
    {
        PrimitiveType primitiveType = correctObjectId switch
        {
            IfObjectId.Cube => PrimitiveType.Cube,
            IfObjectId.Cylinder => PrimitiveType.Cylinder,
            _ => PrimitiveType.Sphere,
        };

        Vector3 scale = primitiveType == PrimitiveType.Cylinder
            ? new Vector3(0.22f, 0.18f, 0.22f)
            : new Vector3(0.28f, 0.28f, 0.28f);

        return CreatePrimitive("Preview", primitiveType, parent, new Vector3(0f, 2.78f, 0.18f), scale, material);
    }

    private static void CreateFinishZone(
        Transform parent,
        Material structureMaterial,
        Material glowMaterial,
        CanvasGroup finishCanvasGroup,
        ScreenFadePlayerLock screenFadePlayerLock)
    {
        CreateCube("FinishPad", parent, new Vector3(0f, 0.08f, 15.45f), new Vector3(3f, 0.16f, 3f), structureMaterial);
        CreateCube("FinishPadGlow", parent, new Vector3(0f, 0.16f, 15.45f), new Vector3(2.2f, 0.05f, 2.2f), glowMaterial);

        GameObject finishRoot = new GameObject("LevelFinishTrigger");
        finishRoot.transform.SetParent(parent, false);
        finishRoot.transform.localPosition = new Vector3(0f, 0f, 15.45f);

        BoxCollider triggerCollider = finishRoot.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(3.2f, 2.4f, 3.2f);
        triggerCollider.center = new Vector3(0f, 1.2f, 0f);

        LevelFinishTrigger finishTrigger = finishRoot.AddComponent<LevelFinishTrigger>();
        SetObjectReference(finishTrigger, "screenFadePlayerLock", screenFadePlayerLock);
        SetObjectReference(finishTrigger, "finishCanvasGroup", finishCanvasGroup);
        SetFloat(finishTrigger, "fadeDuration", 0.25f);
        SetFloat(finishTrigger, "overlayAlphaWhenComplete", 0.12f);
    }

    private static void ConfigurePlayerInteractor(GameObject playerRoot, PromptBundle promptBundle)
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
        SetFloat(interactor, "interactionDistance", 4.5f);
        SetLayerMask(interactor, "interactionMask", ~0);
        SetObjectReference(interactor, "promptRoot", promptBundle.Root);
        SetObjectReference(interactor, "promptText", promptBundle.Text);
        SetString(interactor, "defaultPrompt", "Press E to interact");
    }

    private static PromptBundle CreateInteractionPrompt(Transform parent, TMP_FontAsset fontAsset)
    {
        RectTransform root = CreatePanel(
            "InteractionPrompt",
            parent,
            new Vector2(0.33f, 0.06f),
            new Vector2(0.67f, 0.12f),
            new Color(0.04f, 0.08f, 0.11f, 0.92f));

        TextMeshProUGUI text = CreateText(
            "PromptText",
            root,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            Vector2.zero,
            Vector2.zero,
            "Press E to interact",
            fontAsset,
            24,
            new Color(0.72f, 0.97f, 1f, 1f),
            TextAlignmentOptions.Center);

        root.gameObject.SetActive(false);
        return new PromptBundle(root.gameObject, text);
    }

    private static CanvasGroup CreateFinishPanel(Transform parent, TMP_FontAsset fontAsset)
    {
        RectTransform root = CreatePanel(
            "FinishPanel",
            parent,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Color(0.05f, 0.09f, 0.12f, 0.94f),
            new Vector2(560f, 240f));

        CanvasGroup canvasGroup = root.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        CreatePanel(
            "FinishAccent",
            root,
            new Vector2(0.08f, 0.84f),
            new Vector2(0.92f, 0.89f),
            new Color(0.12f, 0.84f, 1f, 0.95f));

        CreateText(
            "FinishTitle",
            root,
            new Vector2(0.1f, 0.5f),
            new Vector2(0.9f, 0.78f),
            Vector2.zero,
            Vector2.zero,
            "IF COMPLETE",
            fontAsset,
            38,
            Color.white,
            TextAlignmentOptions.Center);

        CreateText(
            "FinishSubtitle",
            root,
            new Vector2(0.12f, 0.22f),
            new Vector2(0.88f, 0.46f),
            Vector2.zero,
            Vector2.zero,
            "Correct choice unlocked the door.",
            fontAsset,
            24,
            new Color(0.78f, 0.89f, 0.95f, 1f),
            TextAlignmentOptions.Center);

        return canvasGroup;
    }

    private static void RemoveExistingPrototypeRoots(GameObject playerRoot, GameObject fadeCanvasObject)
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        foreach (GameObject root in activeScene.GetRootGameObjects())
        {
            if (root == playerRoot || root == fadeCanvasObject)
            {
                continue;
            }

            if (root.GetComponentInChildren<Camera>(true) != null)
            {
                continue;
            }

            if (root.GetComponentInChildren<Light>(true) != null)
            {
                continue;
            }

            if (root.GetComponentInChildren<EventSystem>(true) != null)
            {
                continue;
            }

            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static void RemoveExistingPlayerInteractor()
    {
        PlayerInteractor[] interactors = UnityEngine.Object.FindObjectsByType<PlayerInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PlayerInteractor interactor in interactors)
        {
            UnityEngine.Object.DestroyImmediate(interactor);
        }
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

    private static void EnsureEventSystem()
    {
        EventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
        }
    }

    private static void EnsureGeneratedMaterialFolder()
    {
        if (!AssetDatabase.IsValidFolder(GeneratedMaterialFolder))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "IfLevel");
        }
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
        material.globalIlluminationFlags = emissionColor.maxColorComponent > 0.001f
            ? MaterialGlobalIlluminationFlags.RealtimeEmissive
            : MaterialGlobalIlluminationFlags.EmissiveIsBlack;

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

    private static GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        return CreatePrimitive(name, PrimitiveType.Cube, parent, localPosition, localScale, material);
    }

    private static GameObject CreatePrimitive(string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject primitive = GameObject.CreatePrimitive(primitiveType);
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
        text.enableWordWrapping = true;
        return text;
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

    private static T LoadRequiredAsset<T>(string path) where T : UnityEngine.Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            throw new InvalidOperationException($"Required asset was not found: {path}");
        }

        return asset;
    }

    private static T FindRequiredObject<T>(string errorMessage) where T : UnityEngine.Object
    {
        T instance = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (instance == null)
        {
            throw new InvalidOperationException(errorMessage);
        }

        return instance;
    }

    private static GameObject FindRequiredSceneObject(string name)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform transform in transforms)
        {
            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }

        throw new InvalidOperationException($"Scene object was not found: {name}");
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

    private static void SetEnum(UnityEngine.Object target, string propertyName, int value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).enumValueIndex = value;
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
}
