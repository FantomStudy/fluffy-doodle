using UnityEngine;

public class SelectableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private IfObjectId objectId = IfObjectId.Cube;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private string interactionPrompt;
    [SerializeField] private GameObject selectedVisualRoot;
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private bool useColorOverride = true;
    [SerializeField] private Color defaultColor = new Color(0.82f, 0.14f, 0.14f, 1f);
    [SerializeField] private Color selectedColor = new Color(0.15f, 0.82f, 0.22f, 1f);
    [SerializeField] private Color defaultEmission = new Color(0.55f, 0.08f, 0.08f, 1f);
    [SerializeField] private Color selectedEmission = new Color(0.1f, 0.95f, 0.18f, 1f);
    [SerializeField] private bool createPedestalTrigger = true;
    [SerializeField] private Vector3 triggerLocalPosition = new Vector3(0f, 0.95f, 0f);
    [SerializeField] private Vector3 triggerSize = new Vector3(2.6f, 2.4f, 2.6f);

    private MaterialPropertyBlock propertyBlock;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    public IfObjectId ObjectId => objectId;
    public string InteractionPrompt => GetDefaultPrompt();

    private void Awake()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>(true);
        }

        if ((targetRenderers == null || targetRenderers.Length == 0) && objectRenderer == null)
        {
            targetRenderers = GetComponentsInChildren<Renderer>(true);
        }

        if (useColorOverride)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        EnsurePedestalTrigger();
        SetSelected(false);
    }

    public void Interact(PlayerInteractor interactor)
    {
        selectionManager?.ToggleSelection(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedVisualRoot != null)
        {
            selectedVisualRoot.SetActive(isSelected);
        }

        if (useColorOverride)
        {
            ApplyColorState(isSelected);
            return;
        }

        Material targetMaterial = isSelected && selectedMaterial != null ? selectedMaterial : defaultMaterial;
        if (targetMaterial == null)
        {
            return;
        }

        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            foreach (Renderer rendererComponent in targetRenderers)
            {
                if (rendererComponent != null)
                {
                    rendererComponent.sharedMaterial = targetMaterial;
                }
            }

            return;
        }

        if (objectRenderer != null)
        {
            objectRenderer.sharedMaterial = targetMaterial;
        }
    }

    private void ApplyColorState(bool isSelected)
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        Color color = isSelected ? selectedColor : defaultColor;
        Color emission = isSelected ? selectedEmission : defaultEmission;

        Renderer[] renderers = targetRenderers != null && targetRenderers.Length > 0
            ? targetRenderers
            : new[] { objectRenderer };

        if (renderers == null || renderers.Length == 0)
        {
            return;
        }

        foreach (Renderer rendererComponent in renderers)
        {
            if (rendererComponent == null)
            {
                continue;
            }

            propertyBlock.Clear();
            propertyBlock.SetColor(BaseColorId, color);
            propertyBlock.SetColor(ColorId, color);
            propertyBlock.SetColor(EmissionColorId, emission);
            rendererComponent.SetPropertyBlock(propertyBlock);
        }
    }

    private void EnsurePedestalTrigger()
    {
        if (!createPedestalTrigger)
        {
            return;
        }

        Transform zoneParent = transform.parent != null ? transform.parent : transform;
        Transform existingZone = zoneParent.Find("InteractionTriggerZone");
        if (existingZone == null)
        {
            GameObject zoneObject = new GameObject("InteractionTriggerZone");
            existingZone = zoneObject.transform;
            existingZone.SetParent(zoneParent, false);
        }

        existingZone.localPosition = triggerLocalPosition;
        existingZone.localRotation = Quaternion.identity;
        existingZone.localScale = Vector3.one;

        BoxCollider triggerCollider = existingZone.GetComponent<BoxCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = existingZone.gameObject.AddComponent<BoxCollider>();
        }

        triggerCollider.isTrigger = true;
        triggerCollider.size = triggerSize;
        triggerCollider.center = Vector3.zero;

        InteractionTriggerZone triggerZone = existingZone.GetComponent<InteractionTriggerZone>();
        if (triggerZone == null)
        {
            triggerZone = existingZone.gameObject.AddComponent<InteractionTriggerZone>();
        }

        triggerZone.SetTarget(this);
    }

    private string GetDefaultPrompt()
    {
        bool isSelected = selectionManager != null && selectionManager.IsSelected(this);
        string objectName = GetDisplayName();
        return isSelected
            ? $"Нажмите E, чтобы снять выбор: {objectName}"
            : $"Нажмите E, чтобы выбрать: {objectName}";
    }

    private string GetDisplayName()
    {
        return objectId switch
        {
            IfObjectId.Cube => "Куб",
            IfObjectId.Sphere => "Сфера",
            IfObjectId.Cylinder => "Цилиндр",
            _ => "Блок",
        };
    }
}
