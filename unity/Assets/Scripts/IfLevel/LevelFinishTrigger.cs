using System.Collections;
using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] private CanvasGroup finishCanvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float overlayAlphaWhenComplete = 0.12f;

    private Coroutine fadeRoutine;
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
            screenFadePlayerLock.SetLockedInstant(true, overlayAlphaWhenComplete);
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        if (finishCanvasGroup == null)
        {
            yield break;
        }

        finishCanvasGroup.blocksRaycasts = true;
        finishCanvasGroup.interactable = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = fadeDuration <= 0f ? 1f : elapsed / fadeDuration;
            finishCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        ApplyState(1f, true);
        fadeRoutine = null;
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
