using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ScreenFadePlayerLock : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private GameObject playerRoot;
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private bool unlockCursorWhenLocked = true;

    private Coroutine fadeRoutine;
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
#if ENABLE_INPUT_SYSTEM
    private PlayerInput playerInput;
#endif

    private void Awake()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }

        CachePlayerComponents();
    }

    public void FadeOutAndLock()
    {
        CachePlayerComponents();
        SetPlayerLocked(true);
        StartFade(1f);
    }

    public void FadeOutAndLock(System.Action onComplete)
    {
        CachePlayerComponents();
        SetPlayerLocked(true);
        StartFade(1f, onComplete);
    }

    public void FadeOutAndLockThenLoadScene(int buildIndex)
    {
        FadeOutAndLock(() => SceneManager.LoadScene(buildIndex));
    }

    public void FadeInAndUnlock()
    {
        CachePlayerComponents();
        StartFade(0f, UnlockPlayer);
    }

    public void SetLockedInstant(bool locked, float overlayAlpha = 1f)
    {
        CachePlayerComponents();
        SetPlayerLocked(locked);

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = locked ? overlayAlpha : 0f;
            bool blocksRaycasts = locked && overlayAlpha > 0.001f;
            fadeCanvasGroup.blocksRaycasts = blocksRaycasts;
            fadeCanvasGroup.interactable = blocksRaycasts;
        }
    }

    private void CachePlayerComponents()
    {
        if (playerRoot == null)
        {
            thirdPersonController = FindFirstObjectByType<ThirdPersonController>();
            if (thirdPersonController != null)
            {
                playerRoot = thirdPersonController.gameObject;
            }
        }

        if (playerRoot == null)
        {
            return;
        }

        if (thirdPersonController == null)
        {
            thirdPersonController = playerRoot.GetComponentInChildren<ThirdPersonController>(true);
        }

        if (starterAssetsInputs == null)
        {
            starterAssetsInputs = playerRoot.GetComponentInChildren<StarterAssetsInputs>(true);
        }

#if ENABLE_INPUT_SYSTEM
        if (playerInput == null)
        {
            playerInput = playerRoot.GetComponentInChildren<PlayerInput>(true);
        }
#endif
    }

    private void StartFade(float targetAlpha, System.Action onComplete = null)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, onComplete));
    }

    private IEnumerator FadeRoutine(float targetAlpha, System.Action onComplete)
    {
        if (fadeCanvasGroup == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;
        fadeCanvasGroup.blocksRaycasts = targetAlpha > 0f;
        fadeCanvasGroup.interactable = targetAlpha > 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = fadeDuration <= 0f ? 1f : elapsed / fadeDuration;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha > 0f;
        fadeCanvasGroup.interactable = targetAlpha > 0f;
        fadeRoutine = null;
        onComplete?.Invoke();
    }

    private void SetPlayerLocked(bool locked)
    {
        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.MoveInput(Vector2.zero);
            starterAssetsInputs.LookInput(Vector2.zero);
            starterAssetsInputs.JumpInput(false);
            starterAssetsInputs.SprintInput(false);
            starterAssetsInputs.enabled = !locked;
        }

#if ENABLE_INPUT_SYSTEM
        if (playerInput != null)
        {
            playerInput.enabled = !locked;
        }
#endif

        if (thirdPersonController != null)
        {
            thirdPersonController.enabled = !locked;
        }

        if (unlockCursorWhenLocked)
        {
            Cursor.visible = locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private void UnlockPlayer()
    {
        SetPlayerLocked(false);
    }
}
