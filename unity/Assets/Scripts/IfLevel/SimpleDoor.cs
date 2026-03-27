using System.Collections;
using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openLocalOffset = new Vector3(0f, 3.4f, 0f);
    [SerializeField] private float openSpeed = 2.8f;
    [SerializeField] private bool startOpened;
    [SerializeField] private bool disableCollidersWhenOpened = true;
    [SerializeField] private bool isFinalDoor;
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] private int nextSceneBuildIndex = -1;
    [SerializeField] private float fadeDelayOnOpen = 0.1f;

    private Vector3 closedLocalPosition;
    private Vector3 openedLocalPosition;
    private Collider[] cachedColliders;
    private bool collidersDisabled;
    private bool isOpen;
    private bool finalFadeTriggered;

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
        CacheFadePlayerLock();

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
        if (isOpen)
        {
            return;
        }

        isOpen = true;

        if (isFinalDoor && !finalFadeTriggered)
        {
            finalFadeTriggered = true;
            StartCoroutine(FadeAfterOpenRoutine());
        }
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

    private IEnumerator FadeAfterOpenRoutine()
    {
        if (fadeDelayOnOpen > 0f)
        {
            yield return new WaitForSeconds(fadeDelayOnOpen);
        }

        CacheFadePlayerLock();
        if (screenFadePlayerLock == null)
        {
            yield break;
        }

        if (nextSceneBuildIndex >= 0)
        {
            screenFadePlayerLock.FadeOutAndLockThenLoadScene(nextSceneBuildIndex);
            yield break;
        }

        screenFadePlayerLock.FadeOutAndLock();
    }

    private void CacheFadePlayerLock()
    {
        if (screenFadePlayerLock == null)
        {
            screenFadePlayerLock = FindFirstObjectByType<ScreenFadePlayerLock>();
        }
    }
}
