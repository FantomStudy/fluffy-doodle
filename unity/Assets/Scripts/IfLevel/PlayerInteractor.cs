using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private bool useRaycastFallback;
    [SerializeField] private float interactionDistance = 4.5f;
    [SerializeField] private float interactionRadius = 0.35f;
    [SerializeField] private float interactionAssistDepth = 1.1f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string defaultPrompt = "Нажмите E для взаимодействия";

    private IInteractable currentInteractable;
    private readonly List<InteractionTriggerZone> nearbyZones = new List<InteractionTriggerZone>();
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];

    private void Awake()
    {
        if (defaultPrompt == "Press E to interact")
        {
            defaultPrompt = "Нажмите E для взаимодействия";
        }

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
        if (TryResolveTriggeredInteractable(out IInteractable zoneInteractable, out string zonePrompt))
        {
            currentInteractable = zoneInteractable;
            SetPromptVisible(true, string.IsNullOrWhiteSpace(zonePrompt) ? defaultPrompt : zonePrompt);
            return;
        }

        if (!useRaycastFallback)
        {
            currentInteractable = null;
            SetPromptVisible(false, defaultPrompt);
            return;
        }

        ResolveCamera();
        if (interactionCamera == null)
        {
            currentInteractable = null;
            SetPromptVisible(false, defaultPrompt);
            return;
        }

        Ray ray = interactionCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!TryResolveInteractable(ray, out IInteractable interactable))
        {
            currentInteractable = null;
            SetPromptVisible(false, defaultPrompt);
            return;
        }

        currentInteractable = interactable;
        string prompt = string.IsNullOrWhiteSpace(interactable.InteractionPrompt) ? defaultPrompt : interactable.InteractionPrompt;
        SetPromptVisible(true, prompt);
    }

    private bool TryResolveTriggeredInteractable(out IInteractable interactable, out string prompt)
    {
        interactable = null;
        prompt = defaultPrompt;

        if (nearbyZones.Count == 0)
        {
            return false;
        }

        float bestDistanceSqr = float.MaxValue;
        InteractionTriggerZone bestZone = null;
        Vector3 playerPosition = transform.position;

        for (int i = nearbyZones.Count - 1; i >= 0; i--)
        {
            InteractionTriggerZone zone = nearbyZones[i];
            if (zone == null)
            {
                nearbyZones.RemoveAt(i);
                continue;
            }

            if (!zone.TryGetInteractable(out IInteractable candidate))
            {
                continue;
            }

            float distanceSqr = (zone.WorldPosition - playerPosition).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                bestZone = zone;
                interactable = candidate;
            }
        }

        if (bestZone == null || interactable == null)
        {
            return false;
        }

        prompt = bestZone.GetPrompt();
        return true;
    }

    private bool TryResolveInteractable(Ray ray, out IInteractable interactable)
    {
        int hitCount = interactionRadius > 0.001f
            ? Physics.SphereCastNonAlloc(ray, interactionRadius, hitBuffer, interactionDistance, interactionMask, QueryTriggerInteraction.Collide)
            : Physics.RaycastNonAlloc(ray, hitBuffer, interactionDistance, interactionMask, QueryTriggerInteraction.Collide);

        if (hitCount <= 0)
        {
            interactable = null;
            return false;
        }

        Array.Sort(hitBuffer, 0, hitCount, RaycastHitDistanceComparer.Instance);

        float nearestDistance = -1f;
        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = hitBuffer[i].collider;
            if (hitCollider == null || IsSelfCollider(hitCollider))
            {
                continue;
            }

            if (nearestDistance < 0f)
            {
                nearestDistance = hitBuffer[i].distance;
            }

            if (hitBuffer[i].distance > nearestDistance + interactionAssistDepth)
            {
                break;
            }

            if (TryGetInteractable(hitCollider, out interactable))
            {
                return true;
            }
        }

        interactable = null;
        return false;
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

    private bool IsSelfCollider(Collider targetCollider)
    {
        return targetCollider != null && targetCollider.transform.root == transform.root;
    }

    private void OnTriggerEnter(Collider other)
    {
        RegisterZoneFromCollider(other);
    }

    private void OnTriggerStay(Collider other)
    {
        RegisterZoneFromCollider(other);
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionTriggerZone zone = other.GetComponent<InteractionTriggerZone>();
        if (zone == null)
        {
            zone = other.GetComponentInParent<InteractionTriggerZone>();
        }

        if (zone != null)
        {
            nearbyZones.Remove(zone);
        }
    }

    public void RegisterZone(InteractionTriggerZone zone)
    {
        if (zone != null && !nearbyZones.Contains(zone))
        {
            nearbyZones.Add(zone);
        }
    }

    public void UnregisterZone(InteractionTriggerZone zone)
    {
        if (zone != null)
        {
            nearbyZones.Remove(zone);
        }
    }

    private void RegisterZoneFromCollider(Collider other)
    {
        InteractionTriggerZone zone = other.GetComponent<InteractionTriggerZone>();
        if (zone == null)
        {
            zone = other.GetComponentInParent<InteractionTriggerZone>();
        }

        RegisterZone(zone);
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

    private sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public static readonly RaycastHitDistanceComparer Instance = new RaycastHitDistanceComparer();

        public int Compare(RaycastHit x, RaycastHit y)
        {
            return x.distance.CompareTo(y.distance);
        }
    }
}

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
