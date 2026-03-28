using UnityEngine;

public class SequenceDoor : MonoBehaviour
{
    [SerializeField] private Vector3 openLocalOffset = new(0f, 2.5f, 0f);
    [SerializeField] private float openSpeed = 2.5f;
    [SerializeField] private bool disableCollidersWhenOpened = true;

    [SerializeField] private bool isFinalDoor = false;
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] private int nextSceneBuildIndex = -1;

    private Vector3 closedLocalPosition;
    private Vector3 openedLocalPosition;
    private Collider[] cachedColliders;
    private bool isOpen;
    private bool collidersDisabled;

    private void Awake()
    {
        closedLocalPosition = transform.localPosition;
        openedLocalPosition = closedLocalPosition + openLocalOffset;
        cachedColliders = GetComponentsInChildren<Collider>(true);
    }

    private void Update()
    {
        if (!isOpen)
        {
            return;
        }

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            openedLocalPosition,
            openSpeed * Time.deltaTime);

        if (!collidersDisabled &&
            disableCollidersWhenOpened &&
            Vector3.Distance(transform.localPosition, openedLocalPosition) <= 0.01f)
        {
            collidersDisabled = true;
            foreach (Collider colliderComponent in cachedColliders)
            {
                colliderComponent.enabled = false;
            }
        }
    }

    public void Open()
    {
        isOpen = true;
        if (isFinalDoor)
        {
            if (LevelCompletionFlowController.TryStartCurrentLevelCompletion(nextSceneBuildIndex))
            {
                return;
            }

            if (screenFadePlayerLock == null)
            {
                return;
            }

            if (nextSceneBuildIndex >= 0)
            {
                screenFadePlayerLock.FadeOutAndLockThenLoadScene(nextSceneBuildIndex);
                return;
            }

            screenFadePlayerLock.FadeOutAndLock();
        }
    }
}
