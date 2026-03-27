using TMPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private float interactionDistance = 4.5f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string defaultPrompt = "Press E to interact";

    private IInteractable currentInteractable;

    private void Awake()
    {
        SetPromptVisible(false, defaultPrompt);
    }

    private void Update()
    {
        ResolveCamera();
        UpdateCurrentInteractable();

        if (currentInteractable != null && WasInteractPressed())
        {
            currentInteractable.Interact(this);
        }
    }

    private void OnDisable()
    {
        currentInteractable = null;
        SetPromptVisible(false, defaultPrompt);
    }

    private void ResolveCamera()
    {
        if (interactionCamera == null)
        {
            interactionCamera = Camera.main;
        }

        if (interactionCamera == null)
        {
            interactionCamera = FindFirstObjectByType<Camera>();
        }
    }

    private void UpdateCurrentInteractable()
    {
        if (interactionCamera == null)
        {
            currentInteractable = null;
            SetPromptVisible(false, defaultPrompt);
            return;
        }

        Ray ray = interactionCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionMask, QueryTriggerInteraction.Ignore))
        {
            currentInteractable = null;
            SetPromptVisible(false, defaultPrompt);
            return;
        }

        if (!TryGetInteractable(hit.collider, out IInteractable interactable))
        {
            currentInteractable = null;
            SetPromptVisible(false, defaultPrompt);
            return;
        }

        currentInteractable = interactable;
        string prompt = string.IsNullOrWhiteSpace(interactable.InteractionPrompt) ? defaultPrompt : interactable.InteractionPrompt;
        SetPromptVisible(true, prompt);
    }

    private void SetPromptVisible(bool visible, string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
        }

        if (promptRoot != null && promptRoot.activeSelf != visible)
        {
            promptRoot.SetActive(visible);
        }
    }

    private static bool TryGetInteractable(Collider targetCollider, out IInteractable interactable)
    {
        MonoBehaviour[] behaviours = targetCollider.GetComponentsInParent<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IInteractable candidate && behaviour.isActiveAndEnabled)
            {
                interactable = candidate;
                return true;
            }
        }

        interactable = null;
        return false;
    }

    private static bool WasInteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.E);
#else
        return false;
#endif
    }
}
