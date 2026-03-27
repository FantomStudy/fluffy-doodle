using System;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Level3AssetPuzzleBuilder
{
    private const string GeneratedRootName = "Level3AssetPuzzle";
    private const string LegacyPrimitiveRootName = "Level3Prototype";
    private const string ScenePath = "Assets/Scenes/Level3.unity";
    private const string GlowMaterialPath = "Assets/Materials/BridgeGlow.mat";
    private const string FontAssetPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
    private const string GeneratedMaterialFolder = "Assets/Materials/IfLevel";
    private const string ObjectMaterialPath = GeneratedMaterialFolder + "/IfObjectNeutral.mat";
    private const string IdleMaterialPath = GeneratedMaterialFolder + "/IfIdle.mat";
    private const string SuccessMaterialPath = GeneratedMaterialFolder + "/IfSuccess.mat";
    private const string ErrorMaterialPath = GeneratedMaterialFolder + "/IfError.mat";

    private const string RoomWidePath = "Assets/Models/kenney_modular-space-kit_1.0/room-wide.fbx";
    private const string RoomWideVariationPath = "Assets/Models/kenney_modular-space-kit_1.0/room-wide-variation.fbx";
    private const string RoomLargePath = "Assets/Models/kenney_modular-space-kit_1.0/room-large.fbx";
    private const string RoomLargeVariationPath = "Assets/Models/kenney_modular-space-kit_1.0/room-large-variation.fbx";
    private const string RoomSmallVariationPath = "Assets/Models/kenney_modular-space-kit_1.0/room-small-variation.fbx";
    private const string GateFramePath = "Assets/Models/kenney_modular-space-kit_1.0/gate.fbx";
    private const string GateDoorPath = "Assets/Models/kenney_modular-space-kit_1.0/gate-door.fbx";
    private const string PedestalPath = "Assets/Models/stylized-pedestal/source/Pedestal.fbx";
    private const string BridgeFloorPath = "Assets/Models/kenney_modular-space-kit_1.0/template-floor-big.fbx";
    private const string TerminalPath = "Assets/Models/sci-fi_terminal_low_poly.glb";

    private static readonly Vector3 GeneratedRootWorldPosition = new Vector3(0f, 0f, 120f);
    private static readonly Vector3 PlayerStartLocalPosition = new Vector3(0f, 1.4f, -7.5f);

    private const float RoomSpacing = 19f;
    private const float RoomWidth = 14f;
    private const float RoomDepth = 18f;
    private const float RoomHeight = 4f;

    [MenuItem("Tools/Level3/Build 5 Puzzle Rooms From Assets")]
    public static void BuildFivePuzzleRooms()
    {
        EnsureRoomsBuiltInOpenLevel3(true);
        Debug.Log("Level3 asset puzzle rooms built in the current scene.");
    }

    [MenuItem("Tools/Level3/Bake 5 Puzzle Rooms Into Level3 Scene Asset")]
    public static void BuildLevel3SceneAsset()
    {
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        if (sceneAsset == null)
        {
            throw new InvalidOperationException("Level3 scene was not found.");
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        EnsureRoomsBuiltInOpenLevel3(true);
        Debug.Log("Level3 scene asset was baked and saved.");
    }

    public static bool NeedsBuildInOpenScene()
    {
        if (!IsLevel3SceneOpen())
        {
            return false;
        }

        GameObject generatedRoot = FindSceneObject(GeneratedRootName);
        GameObject legacyRoot = FindSceneObject(LegacyPrimitiveRootName);
        return generatedRoot == null || legacyRoot != null;
    }

    public static void EnsureRoomsBuiltInOpenLevel3(bool saveScene)
    {
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        if (sceneAsset == null)
        {
            throw new InvalidOperationException("Level3 scene was not found.");
        }

        if (!IsLevel3SceneOpen())
        {
            throw new InvalidOperationException("Open Level3 scene before building puzzle rooms.");
        }

        Level3SceneUiTools.AddPuzzleUiToCurrentScene();
        BuildInOpenScene();

        var activeScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(activeScene);
        AssetDatabase.SaveAssets();

        if (saveScene)
        {
            EditorSceneManager.SaveScene(activeScene);
        }
    }

    private static bool IsLevel3SceneOpen()
    {
        return string.Equals(EditorSceneManager.GetActiveScene().path, ScenePath, StringComparison.OrdinalIgnoreCase);
    }

    private static void BuildInOpenScene()
    {
        EnsureGeneratedMaterialFolder();

        TMP_FontAsset fontAsset = LoadRequiredAsset<TMP_FontAsset>(FontAssetPath);
        Material neutralObjectMaterial = CreateOrUpdateMaterial(ObjectMaterialPath, new Color(0.77f, 0.82f, 0.91f), 0.3f, 0.05f, Color.black);
        Material selectedObjectMaterial = LoadRequiredAsset<Material>(GlowMaterialPath);
        Material idleMaterial = CreateOrUpdateMaterial(IdleMaterialPath, new Color(0.1f, 0.34f, 0.46f), 0.62f, 0.08f, new Color(0.22f, 1.05f, 1.55f));
        Material successMaterial = CreateOrUpdateMaterial(SuccessMaterialPath, new Color(0.13f, 0.74f, 0.28f), 0.72f, 0.1f, new Color(0.25f, 1.8f, 0.45f));
        Material errorMaterial = CreateOrUpdateMaterial(ErrorMaterialPath, new Color(0.77f, 0.19f, 0.22f), 0.6f, 0.05f, new Color(1.9f, 0.18f, 0.18f));

        ScreenFadePlayerLock screenFadePlayerLock = FindRequiredObject<ScreenFadePlayerLock>("ScreenFadePlayerLock was not found in the open scene.");
        ThirdPersonController playerController = FindRequiredObject<ThirdPersonController>("A ThirdPersonController player is required in Level3.");
        GameObject playerRoot = playerController.transform.root.gameObject;

        RemoveGeneratedRoot();

        GameObject generatedRoot = new GameObject(GeneratedRootName);
        generatedRoot.transform.position = GeneratedRootWorldPosition;
        playerRoot.transform.SetPositionAndRotation(generatedRoot.transform.TransformPoint(PlayerStartLocalPosition), Quaternion.identity);

        string[] roomPaths =
        {
            RoomWidePath,
            RoomWideVariationPath,
            RoomLargePath,
            RoomSmallVariationPath,
            RoomLargeVariationPath,
        };

        List<RoomBuildResult> rooms = new List<RoomBuildResult>();
        for (int i = 0; i < roomPaths.Length; i++)
        {
            RoomBuildResult room = CreateRoom(
                generatedRoot.transform,
                roomPaths[i],
                i,
                fontAsset,
                neutralObjectMaterial,
                selectedObjectMaterial);
            rooms.Add(room);
        }

        CreateTransitionBridges(generatedRoot.transform, rooms);
        ConfigureTerminalLogic(rooms, idleMaterial, successMaterial, errorMaterial);
        CreateFinishZone(generatedRoot.transform, rooms[^1], screenFadePlayerLock, fontAsset);
    }

    private static GameObject InstantiateModel(string assetPath, Transform parent, string name)
    {
        GameObject asset = LoadRequiredAsset<GameObject>(assetPath);
        GameObject instance = PrefabUtility.InstantiatePrefab(asset) as GameObject;
        if (instance == null)
        {
            throw new InvalidOperationException($"Could not instantiate model asset: {assetPath}");
        }

        instance.name = name;
        instance.transform.SetParent(parent, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        return instance;
    }

    private static void FitModelToWidth(GameObject model, float targetWidth)
    {
        Bounds bounds = CalculateRendererBounds(model);
        float baseWidth = Mathf.Max(bounds.size.x, bounds.size.z);
        if (baseWidth <= 0.001f)
        {
            return;
        }

        float scale = targetWidth / baseWidth;
        model.transform.localScale = Vector3.one * scale;
    }

    private static void FitModelToHeight(GameObject model, float targetHeight)
    {
        Bounds bounds = CalculateRendererBounds(model);
        if (bounds.size.y <= 0.001f)
        {
            return;
        }

        float scale = targetHeight / bounds.size.y;
        model.transform.localScale = Vector3.one * scale;
    }

    private static void FitModelToFootprint(GameObject model, float targetWidth, float targetDepth)
    {
        Bounds bounds = CalculateRendererBounds(model);
        Vector3 scale = model.transform.localScale;

        if (bounds.size.x > 0.001f)
        {
            scale.x *= targetWidth / bounds.size.x;
        }

        if (bounds.size.z > 0.001f)
        {
            scale.z *= targetDepth / bounds.size.z;
        }

        model.transform.localScale = scale;
    }

    private static void PlaceModelOnGround(GameObject model)
    {
        Bounds bounds = CalculateRendererBounds(model);
        model.transform.position += Vector3.down * bounds.min.y;
    }

    private static Bounds CalculateRendererBounds(GameObject root)
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

    private static void AddBoundsCollider(GameObject root)
    {
        Bounds bounds = CalculateRendererBounds(root);
        BoxCollider colliderComponent = root.GetComponent<BoxCollider>();
        if (colliderComponent == null)
        {
            colliderComponent = root.AddComponent<BoxCollider>();
        }

        Vector3 localCenter = root.transform.InverseTransformPoint(bounds.center);
        Vector3 lossyScale = root.transform.lossyScale;
        Vector3 localSize = new Vector3(
            lossyScale.x == 0f ? bounds.size.x : bounds.size.x / Mathf.Abs(lossyScale.x),
            lossyScale.y == 0f ? bounds.size.y : bounds.size.y / Mathf.Abs(lossyScale.y),
            lossyScale.z == 0f ? bounds.size.z : bounds.size.z / Mathf.Abs(lossyScale.z));

        colliderComponent.center = localCenter;
        colliderComponent.size = localSize;
    }

    private static TextMeshPro CreateWorldText(
        string name,
        Transform parent,
        Vector3 localPosition,
        string content,
        TMP_FontAsset fontAsset,
        float fontSize,
        TextAlignmentOptions alignment,
        Color color,
        float uniformScale)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        textObject.transform.localPosition = localPosition;
        textObject.transform.localRotation = Quaternion.identity;
        textObject.transform.localScale = Vector3.one * uniformScale;

        TextMeshPro text = textObject.AddComponent<TextMeshPro>();
        text.text = content;
        text.font = fontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.rectTransform.sizeDelta = new Vector2(8f, 3f);
        return text;
    }

    private static GameObject CreateBoxCollider(Transform parent, string name, Vector3 localCenter, Vector3 size)
    {
        GameObject colliderObject = new GameObject(name);
        colliderObject.transform.SetParent(parent, false);
        colliderObject.transform.localPosition = Vector3.zero;
        colliderObject.transform.localRotation = Quaternion.identity;
        colliderObject.transform.localScale = Vector3.one;

        BoxCollider boxCollider = colliderObject.AddComponent<BoxCollider>();
        boxCollider.center = localCenter;
        boxCollider.size = size;
        return colliderObject;
    }

    private static RectTransform CreatePanel(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Color color,
        Vector2 sizeDelta)
    {
        GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image));
        panelObject.layer = LayerMask.NameToLayer("UI") >= 0 ? LayerMask.NameToLayer("UI") : 5;
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        UnityEngine.UI.Image image = panelObject.GetComponent<UnityEngine.UI.Image>();
        image.color = color;
        return rectTransform;
    }

    private static TMP_Text CreateCanvasText(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        string content,
        TMP_FontAsset fontAsset,
        int fontSize,
        Color color,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = LayerMask.NameToLayer("UI") >= 0 ? LayerMask.NameToLayer("UI") : 5;
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.font = fontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private static void RemoveGeneratedRoot()
    {
        GameObject existingRoot = FindSceneObject(GeneratedRootName);
        if (existingRoot != null)
        {
            UnityEngine.Object.DestroyImmediate(existingRoot);
        }

        GameObject legacyRoot = FindSceneObject(LegacyPrimitiveRootName);
        if (legacyRoot != null)
        {
            UnityEngine.Object.DestroyImmediate(legacyRoot);
        }

        GameObject existingFinishPanel = FindSceneObject("Level3FinishPanel");
        if (existingFinishPanel != null)
        {
            UnityEngine.Object.DestroyImmediate(existingFinishPanel);
        }

        GameObject legacyFinishPanel = FindSceneObject("FinishPanel");
        if (legacyFinishPanel != null)
        {
            UnityEngine.Object.DestroyImmediate(legacyFinishPanel);
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

    private static T FindRequiredObject<T>(string errorMessage) where T : UnityEngine.Object
    {
        T instance = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (instance == null)
        {
            throw new InvalidOperationException(errorMessage);
        }

        return instance;
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

    private static RoomBuildResult CreateRoom(
        Transform parent,
        string roomAssetPath,
        int roomIndex,
        TMP_FontAsset fontAsset,
        Material neutralObjectMaterial,
        Material selectedObjectMaterial)
    {
        GameObject roomRoot = new GameObject($"PuzzleRoom_{roomIndex + 1}");
        roomRoot.transform.SetParent(parent, false);
        roomRoot.transform.localPosition = new Vector3(0f, 0f, roomIndex * RoomSpacing);

        SelectionManager selectionManager = new GameObject("SelectionManager").AddComponent<SelectionManager>();
        selectionManager.transform.SetParent(roomRoot.transform, false);

        GameObject roomVisual = InstantiateModel(roomAssetPath, roomRoot.transform, "RoomVisual");
        FitModelToWidth(roomVisual, RoomWidth);
        PlaceModelOnGround(roomVisual);

        CreateRoomColliders(roomRoot.transform);
        CreateRoomLabel(roomRoot.transform, fontAsset, roomIndex + 1);

        float[] pedestalXs = { -3.2f, 0f, 3.2f };
        PrimitiveType[] primitiveTypes = { PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Cylinder };
        IfObjectId[] objectIds = { IfObjectId.Cube, IfObjectId.Sphere, IfObjectId.Cylinder };

        for (int i = 0; i < pedestalXs.Length; i++)
        {
            CreatePedestalSelection(
                roomRoot.transform,
                selectionManager,
                pedestalXs[i],
                -2.1f,
                primitiveTypes[i],
                objectIds[i],
                neutralObjectMaterial,
                selectedObjectMaterial);
        }

        SimpleDoor door = CreateDoorAssembly(roomRoot.transform);
        IfLogicTerminal terminal = CreateTerminalAssembly(roomRoot.transform, fontAsset, door);

        return new RoomBuildResult
        {
            RoomRoot = roomRoot,
            SelectionManager = selectionManager,
            Door = door,
            Terminal = terminal,
        };
    }

    private static void CreateRoomColliders(Transform roomRoot)
    {
        GameObject colliderRoot = new GameObject("RoomColliders");
        colliderRoot.transform.SetParent(roomRoot, false);

        const float doorwayWidth = 4.2f;
        float sideWidth = (RoomWidth - 1f - doorwayWidth) * 0.5f;
        float sideOffset = (doorwayWidth * 0.5f) + (sideWidth * 0.5f);

        CreateBoxCollider(colliderRoot.transform, "FloorCollider", new Vector3(0f, -0.1f, 0f), new Vector3(RoomWidth - 1f, 0.4f, RoomDepth));
        CreateBoxCollider(colliderRoot.transform, "LeftWallCollider", new Vector3(-(RoomWidth * 0.5f), RoomHeight * 0.5f, -0.5f), new Vector3(0.35f, RoomHeight, RoomDepth - 2f));
        CreateBoxCollider(colliderRoot.transform, "RightWallCollider", new Vector3(RoomWidth * 0.5f, RoomHeight * 0.5f, -0.5f), new Vector3(0.35f, RoomHeight, RoomDepth - 2f));
        CreateBoxCollider(colliderRoot.transform, "BackWallLeftCollider", new Vector3(-sideOffset, RoomHeight * 0.5f, -(RoomDepth * 0.5f)), new Vector3(sideWidth, RoomHeight, 0.35f));
        CreateBoxCollider(colliderRoot.transform, "BackWallRightCollider", new Vector3(sideOffset, RoomHeight * 0.5f, -(RoomDepth * 0.5f)), new Vector3(sideWidth, RoomHeight, 0.35f));
        CreateBoxCollider(colliderRoot.transform, "ExitFloorCollider", new Vector3(0f, -0.1f, RoomDepth * 0.5f), new Vector3(3.8f, 0.4f, 3f));
    }

    private static void CreateRoomLabel(Transform roomRoot, TMP_FontAsset fontAsset, int roomNumber)
    {
        CreateWorldText(
            "RoomLabel",
            roomRoot,
            new Vector3(0f, 3.05f, -(RoomDepth * 0.5f) + 1.1f),
            $"ROOM {roomNumber}",
            fontAsset,
            5.5f,
            TextAlignmentOptions.Center,
            new Color(0.72f, 0.97f, 1f, 1f),
            0.18f);
    }

    private static void CreatePedestalSelection(
        Transform roomRoot,
        SelectionManager selectionManager,
        float localX,
        float localZ,
        PrimitiveType primitiveType,
        IfObjectId objectId,
        Material neutralObjectMaterial,
        Material selectedObjectMaterial)
    {
        GameObject pedestalRoot = new GameObject(objectId + "Pedestal");
        pedestalRoot.transform.SetParent(roomRoot, false);
        pedestalRoot.transform.localPosition = new Vector3(localX, 0f, localZ);

        GameObject pedestal = InstantiateModel(PedestalPath, pedestalRoot.transform, "PedestalVisual");
        FitModelToHeight(pedestal, 1.35f);
        PlaceModelOnGround(pedestal);

        GameObject block = GameObject.CreatePrimitive(primitiveType);
        block.name = objectId + "Block";
        block.transform.SetParent(pedestalRoot.transform, false);
        block.transform.localPosition = new Vector3(0f, 1.35f, 0f);
        block.transform.localScale = primitiveType switch
        {
            PrimitiveType.Cylinder => new Vector3(0.8f, 0.6f, 0.8f),
            PrimitiveType.Sphere => new Vector3(0.95f, 0.95f, 0.95f),
            _ => new Vector3(0.9f, 0.9f, 0.9f),
        };

        Renderer blockRenderer = block.GetComponent<Renderer>();
        if (blockRenderer != null)
        {
            blockRenderer.sharedMaterial = neutralObjectMaterial;
        }

        SelectableObject selectableObject = block.AddComponent<SelectableObject>();
        SetEnum(selectableObject, "objectId", (int)objectId);
        SetObjectReference(selectableObject, "selectionManager", selectionManager);
        SetString(selectableObject, "interactionPrompt", $"Press E to select {objectId}");
        SetObjectReference(selectableObject, "objectRenderer", blockRenderer);
        SetObjectReference(selectableObject, "defaultMaterial", neutralObjectMaterial);
        SetObjectReference(selectableObject, "selectedMaterial", selectedObjectMaterial);
    }

    private static SimpleDoor CreateDoorAssembly(Transform roomRoot)
    {
        GameObject doorRoot = new GameObject("DoorAssembly");
        doorRoot.transform.SetParent(roomRoot, false);
        doorRoot.transform.localPosition = new Vector3(0f, 0f, 7.3f);

        GameObject frame = InstantiateModel(GateFramePath, doorRoot.transform, "DoorFrameVisual");
        FitModelToHeight(frame, 4.2f);
        PlaceModelOnGround(frame);

        GameObject panel = InstantiateModel(GateDoorPath, doorRoot.transform, "DoorPanelVisual");
        FitModelToHeight(panel, 3.35f);
        PlaceModelOnGround(panel);
        AddBoundsCollider(panel);

        SimpleDoor door = doorRoot.AddComponent<SimpleDoor>();
        SetObjectReference(door, "doorTransform", panel.transform);
        SetVector3(door, "openLocalOffset", new Vector3(0f, 3.8f, 0f));
        SetFloat(door, "openSpeed", 3.2f);
        return door;
    }

    private static IfLogicTerminal CreateTerminalAssembly(Transform roomRoot, TMP_FontAsset fontAsset, SimpleDoor door)
    {
        GameObject terminalRoot = new GameObject("LogicTerminal");
        terminalRoot.transform.SetParent(roomRoot, false);
        terminalRoot.transform.localPosition = new Vector3(4.2f, 0f, 2.6f);
        terminalRoot.transform.localRotation = Quaternion.Euler(0f, -32f, 0f);

        GameObject terminalModel = InstantiateModel(TerminalPath, terminalRoot.transform, "TerminalVisual");
        FitModelToHeight(terminalModel, 2.2f);
        PlaceModelOnGround(terminalModel);

        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        indicator.name = "TerminalIndicator";
        indicator.transform.SetParent(terminalRoot.transform, false);
        indicator.transform.localPosition = new Vector3(0f, 1.58f, 0.34f);
        indicator.transform.localScale = new Vector3(0.85f, 0.12f, 0.08f);

        BoxCollider interactionCollider = terminalRoot.AddComponent<BoxCollider>();
        interactionCollider.center = new Vector3(0f, 1.1f, 0f);
        interactionCollider.size = new Vector3(1.8f, 2.2f, 1.6f);

        TextMeshPro conditionText = CreateWorldText(
            "ConditionText",
            terminalRoot.transform,
            new Vector3(0f, 2.2f, 0.42f),
            string.Empty,
            fontAsset,
            3.8f,
            TextAlignmentOptions.Center,
            Color.white,
            0.12f);

        TextMeshPro statusText = CreateWorldText(
            "StatusText",
            terminalRoot.transform,
            new Vector3(0f, 1.78f, 0.42f),
            "READY",
            fontAsset,
            4.5f,
            TextAlignmentOptions.Center,
            new Color(0.72f, 0.97f, 1f, 1f),
            0.15f);

        IfLogicTerminal terminal = terminalRoot.AddComponent<IfLogicTerminal>();
        SetObjectReference(terminal, "controlledDoor", door);
        SetString(terminal, "interactionPrompt", "Press E to validate condition");
        SetObjectReference(terminal, "statusRenderer", indicator.GetComponent<Renderer>());
        SetObjectReference(terminal, "conditionText", conditionText);
        SetObjectReference(terminal, "statusText", statusText);
        SetString(terminal, "idleStatusText", "READY");
        SetString(terminal, "successStatusText", "TRUE");
        SetString(terminal, "errorStatusText", "FALSE");
        return terminal;
    }

    private static void ConfigureTerminalLogic(List<RoomBuildResult> rooms, Material idleMaterial, Material successMaterial, Material errorMaterial)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            IfLogicTerminal terminal = rooms[i].Terminal;
            SetObjectReference(terminal, "idleMaterial", idleMaterial);
            SetObjectReference(terminal, "successMaterial", successMaterial);
            SetObjectReference(terminal, "errorMaterial", errorMaterial);
            SetConditionDescription(terminal, GetConditionDescription(i));
            ConfigureConditions(terminal, rooms, i);
        }
    }

    private static void CreateTransitionBridges(Transform parent, List<RoomBuildResult> rooms)
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            float fromZ = rooms[i].RoomRoot.transform.localPosition.z + (RoomDepth * 0.5f);
            float toZ = rooms[i + 1].RoomRoot.transform.localPosition.z - (RoomDepth * 0.5f);
            float bridgeLength = Mathf.Max(2.4f, toZ - fromZ + 0.8f);

            GameObject bridgeRoot = new GameObject($"Bridge_{i + 1}_{i + 2}");
            bridgeRoot.transform.SetParent(parent, false);
            bridgeRoot.transform.localPosition = new Vector3(0f, 0f, (fromZ + toZ) * 0.5f);

            GameObject bridge = InstantiateModel(BridgeFloorPath, bridgeRoot.transform, "BridgeFloorVisual");
            FitModelToFootprint(bridge, 4.6f, bridgeLength);
            PlaceModelOnGround(bridge);
            AddBoundsCollider(bridge);
        }
    }

    private static string GetConditionDescription(int roomIndex)
    {
        return roomIndex switch
        {
            0 => "if (selection == Cube)",
            1 => "if (selection == Sphere || selection == Cylinder)",
            2 => "if (Room1 == TRUE && selection == Cylinder)",
            3 => "if (Door2 is open && (selection == Cube || selection == Sphere))",
            4 => "if ((Room3 == TRUE && Room4 == TRUE) && (selection == Sphere || selection == Cylinder))",
            _ => "if (selection == Cube)",
        };
    }

    private static void ConfigureConditions(IfLogicTerminal terminal, List<RoomBuildResult> rooms, int roomIndex)
    {
        SerializedObject serializedObject = new SerializedObject(terminal);
        SerializedProperty allConditions = serializedObject.FindProperty("allConditions");
        SerializedProperty anyConditions = serializedObject.FindProperty("anyConditions");

        switch (roomIndex)
        {
            case 0:
                allConditions.arraySize = 1;
                anyConditions.arraySize = 0;
                ConfigureSelectionCondition(allConditions.GetArrayElementAtIndex(0), rooms[0].SelectionManager, false, IfObjectId.Cube);
                break;

            case 1:
                allConditions.arraySize = 0;
                anyConditions.arraySize = 2;
                ConfigureSelectionCondition(anyConditions.GetArrayElementAtIndex(0), rooms[1].SelectionManager, false, IfObjectId.Sphere);
                ConfigureSelectionCondition(anyConditions.GetArrayElementAtIndex(1), rooms[1].SelectionManager, false, IfObjectId.Cylinder);
                break;

            case 2:
                allConditions.arraySize = 2;
                anyConditions.arraySize = 0;
                ConfigureTerminalCondition(allConditions.GetArrayElementAtIndex(0), rooms[0].Terminal, true, false);
                ConfigureSelectionCondition(allConditions.GetArrayElementAtIndex(1), rooms[2].SelectionManager, false, IfObjectId.Cylinder);
                break;

            case 3:
                allConditions.arraySize = 1;
                anyConditions.arraySize = 2;
                ConfigureDoorCondition(allConditions.GetArrayElementAtIndex(0), rooms[1].Door, true, false);
                ConfigureSelectionCondition(anyConditions.GetArrayElementAtIndex(0), rooms[3].SelectionManager, false, IfObjectId.Cube);
                ConfigureSelectionCondition(anyConditions.GetArrayElementAtIndex(1), rooms[3].SelectionManager, false, IfObjectId.Sphere);
                break;

            case 4:
                allConditions.arraySize = 2;
                anyConditions.arraySize = 2;
                ConfigureTerminalCondition(allConditions.GetArrayElementAtIndex(0), rooms[2].Terminal, true, false);
                ConfigureTerminalCondition(allConditions.GetArrayElementAtIndex(1), rooms[3].Terminal, true, false);
                ConfigureSelectionCondition(anyConditions.GetArrayElementAtIndex(0), rooms[4].SelectionManager, false, IfObjectId.Sphere);
                ConfigureSelectionCondition(anyConditions.GetArrayElementAtIndex(1), rooms[4].SelectionManager, false, IfObjectId.Cylinder);
                break;
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigureSelectionCondition(SerializedProperty conditionProperty, SelectionManager selectionManager, bool invert, params IfObjectId[] acceptedIds)
    {
        conditionProperty.FindPropertyRelative("sourceType").enumValueIndex = (int)IfLogicSourceType.Selection;
        conditionProperty.FindPropertyRelative("selectionManager").objectReferenceValue = selectionManager;

        SerializedProperty acceptedIdsProperty = conditionProperty.FindPropertyRelative("acceptedObjectIds");
        acceptedIdsProperty.arraySize = acceptedIds.Length;
        for (int i = 0; i < acceptedIds.Length; i++)
        {
            acceptedIdsProperty.GetArrayElementAtIndex(i).enumValueIndex = (int)acceptedIds[i];
        }

        conditionProperty.FindPropertyRelative("requiredTerminal").objectReferenceValue = null;
        conditionProperty.FindPropertyRelative("requiredDoor").objectReferenceValue = null;
        conditionProperty.FindPropertyRelative("invertResult").boolValue = invert;
    }

    private static void ConfigureTerminalCondition(SerializedProperty conditionProperty, IfLogicTerminal requiredTerminal, bool solved, bool invert)
    {
        conditionProperty.FindPropertyRelative("sourceType").enumValueIndex = (int)IfLogicSourceType.TerminalSolved;
        conditionProperty.FindPropertyRelative("selectionManager").objectReferenceValue = null;
        conditionProperty.FindPropertyRelative("acceptedObjectIds").arraySize = 0;
        conditionProperty.FindPropertyRelative("requiredTerminal").objectReferenceValue = requiredTerminal;
        conditionProperty.FindPropertyRelative("requiredTerminalSolved").boolValue = solved;
        conditionProperty.FindPropertyRelative("requiredDoor").objectReferenceValue = null;
        conditionProperty.FindPropertyRelative("invertResult").boolValue = invert;
    }

    private static void ConfigureDoorCondition(SerializedProperty conditionProperty, SimpleDoor requiredDoor, bool opened, bool invert)
    {
        conditionProperty.FindPropertyRelative("sourceType").enumValueIndex = (int)IfLogicSourceType.DoorOpened;
        conditionProperty.FindPropertyRelative("selectionManager").objectReferenceValue = null;
        conditionProperty.FindPropertyRelative("acceptedObjectIds").arraySize = 0;
        conditionProperty.FindPropertyRelative("requiredTerminal").objectReferenceValue = null;
        conditionProperty.FindPropertyRelative("requiredDoor").objectReferenceValue = requiredDoor;
        conditionProperty.FindPropertyRelative("requiredDoorOpened").boolValue = opened;
        conditionProperty.FindPropertyRelative("invertResult").boolValue = invert;
    }

    private static void SetConditionDescription(IfLogicTerminal terminal, string description)
    {
        SerializedObject serializedObject = new SerializedObject(terminal);
        serializedObject.FindProperty("conditionDescription").stringValue = description;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        terminal.RefreshTexts();
    }

    private static void CreateFinishZone(Transform parent, RoomBuildResult lastRoom, ScreenFadePlayerLock screenFadePlayerLock, TMP_FontAsset fontAsset)
    {
        CanvasGroup finishCanvasGroup = CreateFinishPanel(fontAsset);

        GameObject finishRoot = new GameObject("Level3FinishTrigger");
        finishRoot.transform.SetParent(parent, false);
        finishRoot.transform.localPosition = lastRoom.RoomRoot.transform.localPosition + new Vector3(0f, 0f, 10.5f);

        BoxCollider triggerCollider = finishRoot.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = new Vector3(0f, 1.2f, 0f);
        triggerCollider.size = new Vector3(4f, 2.4f, 3f);

        LevelFinishTrigger finishTrigger = finishRoot.AddComponent<LevelFinishTrigger>();
        SetObjectReference(finishTrigger, "screenFadePlayerLock", screenFadePlayerLock);
        SetObjectReference(finishTrigger, "finishCanvasGroup", finishCanvasGroup);
        SetFloat(finishTrigger, "fadeDuration", 0.25f);
        SetFloat(finishTrigger, "overlayAlphaWhenComplete", 0.12f);

        CreateBoxCollider(finishRoot.transform, "FinishFloor", new Vector3(0f, -0.1f, 0f), new Vector3(4f, 0.4f, 4f));
        CreateWorldText(
            "FinishLabel",
            finishRoot.transform,
            new Vector3(0f, 2.2f, 0f),
            "EXIT",
            fontAsset,
            5f,
            TextAlignmentOptions.Center,
            new Color(0.72f, 0.97f, 1f, 1f),
            0.18f);
    }

    private static CanvasGroup CreateFinishPanel(TMP_FontAsset fontAsset)
    {
        GameObject fadeCanvas = FindSceneObject("FadeCanvas");
        if (fadeCanvas == null)
        {
            throw new InvalidOperationException("FadeCanvas was not found in the open scene.");
        }

        Transform existing = fadeCanvas.transform.Find("Level3FinishPanel");
        if (existing != null)
        {
            UnityEngine.Object.DestroyImmediate(existing.gameObject);
        }

        RectTransform root = CreatePanel(
            "Level3FinishPanel",
            fadeCanvas.transform,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Color(0.97f, 0.96f, 1f, 0.97f),
            new Vector2(540f, 260f));

        CanvasGroup canvasGroup = root.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        CreatePanel(
            "FinishAccent",
            root,
            new Vector2(0.06f, 0.88f),
            new Vector2(0.94f, 0.94f),
            new Color(0.93f, 0.9f, 0.99f, 1f),
            Vector2.zero);

        CreateCanvasText(
            "FinishTitle",
            root,
            new Vector2(0.08f, 0.6f),
            new Vector2(0.92f, 0.82f),
            "LOGIC COMPLETE",
            fontAsset,
            32,
            new Color(0.21f, 0.18f, 0.31f, 1f),
            TextAlignmentOptions.Center);

        CreateCanvasText(
            "FinishBody",
            root,
            new Vector2(0.1f, 0.3f),
            new Vector2(0.9f, 0.54f),
            "All conditions evaluated to TRUE.",
            fontAsset,
            22,
            new Color(0.37f, 0.34f, 0.46f, 1f),
            TextAlignmentOptions.Center);

        return canvasGroup;
    }

    private sealed class RoomBuildResult
    {
        public GameObject RoomRoot;
        public SelectionManager SelectionManager;
        public SimpleDoor Door;
        public IfLogicTerminal Terminal;
    }
}
