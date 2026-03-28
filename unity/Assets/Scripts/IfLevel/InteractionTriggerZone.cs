using UnityEngine;

public class InteractionTriggerZone : MonoBehaviour
{
    [SerializeField] private MonoBehaviour targetBehaviour;
    [SerializeField] private string promptOverride;

    public Vector3 WorldPosition => transform.position;

    public bool TryGetInteractable(out IInteractable interactable)
    {
        if (targetBehaviour is IInteractable candidate && targetBehaviour.isActiveAndEnabled)
        {
            interactable = candidate;
            return true;
        }

        interactable = null;
        return false;
    }

    public string GetPrompt()
    {
        if (!string.IsNullOrWhiteSpace(promptOverride))
        {
            return promptOverride;
        }

        return TryGetInteractable(out IInteractable interactable) ? interactable.InteractionPrompt : string.Empty;
    }

    public void SetTarget(MonoBehaviour target, string prompt = null)
    {
        targetBehaviour = target;
        promptOverride = prompt ?? string.Empty;
    }

    private void OnTriggerEnter(Collider other)
    {
        RegisterInteractor(other);
    }

    private void OnTriggerStay(Collider other)
    {
        RegisterInteractor(other);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInteractor interactor = FindInteractor(other);
        interactor?.UnregisterZone(this);
    }

    private void RegisterInteractor(Collider other)
    {
        PlayerInteractor interactor = FindInteractor(other);
        interactor?.RegisterZone(this);
    }

    private static PlayerInteractor FindInteractor(Collider other)
    {
        if (other == null)
        {
            return null;
        }

        PlayerInteractor interactor = other.GetComponentInParent<PlayerInteractor>();
        if (interactor != null)
        {
            return interactor;
        }

        Transform root = other.transform.root;
        if (root == null)
        {
            return null;
        }

        return root.GetComponentInChildren<PlayerInteractor>(true);
    }
}
