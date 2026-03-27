using UnityEngine;

public class IfTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private IfObjectId correctObjectId = IfObjectId.Sphere;
    [SerializeField] private SimpleDoor door;
    [SerializeField] private string interactionPrompt = "Press E to run if check";
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Material idleScreenMaterial;
    [SerializeField] private Material successScreenMaterial;
    [SerializeField] private Material errorScreenMaterial;
    [SerializeField] private GameObject idleStateRoot;
    [SerializeField] private GameObject successStateRoot;
    [SerializeField] private GameObject errorStateRoot;

    public string InteractionPrompt => interactionPrompt;

    private void Awake()
    {
        ApplyState(TerminalState.Idle);
    }

    public void Interact(PlayerInteractor interactor)
    {
        bool hasCorrectSelection = selectionManager != null
            && selectionManager.HasSelection
            && selectionManager.CurrentSelectedId == correctObjectId;

        if (hasCorrectSelection)
        {
            ApplyState(TerminalState.Success);
            door?.Open();
            return;
        }

        ApplyState(TerminalState.Error);
    }

    private void ApplyState(TerminalState state)
    {
        if (screenRenderer != null)
        {
            screenRenderer.sharedMaterial = state switch
            {
                TerminalState.Success when successScreenMaterial != null => successScreenMaterial,
                TerminalState.Error when errorScreenMaterial != null => errorScreenMaterial,
                _ => idleScreenMaterial,
            };
        }

        if (idleStateRoot != null)
        {
            idleStateRoot.SetActive(state == TerminalState.Idle);
        }

        if (successStateRoot != null)
        {
            successStateRoot.SetActive(state == TerminalState.Success);
        }

        if (errorStateRoot != null)
        {
            errorStateRoot.SetActive(state == TerminalState.Error);
        }
    }

    private enum TerminalState
    {
        Idle = 0,
        Success = 1,
        Error = 2,
    }
}
