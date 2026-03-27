using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPopupUI : MonoBehaviour
{
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backButton;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private string restartSceneName = "Level2";
    [SerializeField] private string backSceneName = "Level1";

    private Coroutine fadeRoutine;

    private void Awake()
    {
        ApplyState(0f, false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToPreviousLevel);
        }
    }

    public void Show()
    {
        if (screenFadePlayerLock != null)
        {
            screenFadePlayerLock.SetLockedInstant(true, 0f);
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = fadeDuration <= 0f ? 1f : elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        ApplyState(1f, true);
        fadeRoutine = null;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(restartSceneName);
    }

    private void BackToPreviousLevel()
    {
        SceneManager.LoadScene(backSceneName);
    }

    private void ApplyState(float alpha, bool visible)
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
}
