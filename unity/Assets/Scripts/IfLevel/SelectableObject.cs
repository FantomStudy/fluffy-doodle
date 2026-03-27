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

    public IfObjectId ObjectId => objectId;
    public string InteractionPrompt => string.IsNullOrWhiteSpace(interactionPrompt)
        ? $"Press E to select {objectId}"
        : interactionPrompt;

    private void Awake()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>(true);
        }

        SetSelected(false);
    }

    public void Interact(PlayerInteractor interactor)
    {
        selectionManager?.SetSelection(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedVisualRoot != null)
        {
            selectedVisualRoot.SetActive(isSelected);
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
}
