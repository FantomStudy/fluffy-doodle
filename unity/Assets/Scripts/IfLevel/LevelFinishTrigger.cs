using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] private CanvasGroup finishCanvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float overlayAlphaWhenComplete = 0.12f;

    private bool isCompleted;

    private void Awake()
    {
        ApplyState(0f, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCompleted)
        {
            return;
        }

        if (other.GetComponentInParent<CharacterController>() == null)
        {
            return;
        }

        isCompleted = true;

        if (screenFadePlayerLock != null)
        {
            screenFadePlayerLock.FadeOutAndLock();
        }
    }

    private void ApplyState(float alpha, bool visible)
    {
        if (finishCanvasGroup == null)
        {
            return;
        }

        finishCanvasGroup.alpha = alpha;
        finishCanvasGroup.blocksRaycasts = visible;
        finishCanvasGroup.interactable = visible;
    }
}
