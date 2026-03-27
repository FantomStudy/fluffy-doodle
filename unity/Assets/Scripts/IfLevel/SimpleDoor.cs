using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openLocalOffset = new Vector3(0f, 3.4f, 0f);
    [SerializeField] private float openSpeed = 2.8f;
    [SerializeField] private bool startOpened;
    [SerializeField] private bool disableCollidersWhenOpened = true;

    private Vector3 closedLocalPosition;
    private Vector3 openedLocalPosition;
    private Collider[] cachedColliders;
    private bool collidersDisabled;
    private bool isOpen;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (doorTransform == null)
        {
            doorTransform = transform;
        }

        closedLocalPosition = doorTransform.localPosition;
        openedLocalPosition = closedLocalPosition + openLocalOffset;
        cachedColliders = GetComponentsInChildren<Collider>(true);
        isOpen = startOpened;
        doorTransform.localPosition = isOpen ? openedLocalPosition : closedLocalPosition;

        if (isOpen)
        {
            DisableCollidersIfNeeded();
        }
    }

    private void Update()
    {
        if (doorTransform == null)
        {
            return;
        }

        Vector3 targetPosition = isOpen ? openedLocalPosition : closedLocalPosition;
        doorTransform.localPosition = Vector3.MoveTowards(
            doorTransform.localPosition,
            targetPosition,
            openSpeed * Time.deltaTime);

        if (isOpen && !collidersDisabled && Vector3.Distance(doorTransform.localPosition, openedLocalPosition) <= 0.01f)
        {
            DisableCollidersIfNeeded();
        }
    }

    public void Open()
    {
        isOpen = true;
    }

    private void DisableCollidersIfNeeded()
    {
        if (collidersDisabled || !disableCollidersWhenOpened || cachedColliders == null)
        {
            return;
        }

        collidersDisabled = true;
        foreach (Collider colliderComponent in cachedColliders)
        {
            if (colliderComponent != null)
            {
                colliderComponent.enabled = false;
            }
        }
    }
}
