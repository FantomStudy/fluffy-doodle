using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LevelCompletionFlowController : MonoBehaviour
{
    private const float AutoRedirectDelaySeconds = 5f;
    private const string RedirectQueryParameterName = "redirect";
    private const int DefaultRedirectPort = 5173;
    private const string DefaultRedirectPath = "/courses";

    private static LevelCompletionFlowController instance;

    private LevelCompletionApiClient apiClient;
    private LevelCompletionPopupUI popupUI;
    private ScreenFadePlayerLock screenFadePlayerLock;
    private SceneLevelDescriptor currentSceneLevel;
    private bool isCompleting;
    private Coroutine redirectRoutine;

    public static bool TryStartCurrentLevelCompletion(int nextSceneBuildIndex = -1)
    {
        EnsureInstance();
        return instance != null && instance.TryStartCompletion(nextSceneBuildIndex);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        instance = FindFirstObjectByType<LevelCompletionFlowController>();
        if (instance != null)
        {
            instance.Initialize();
            return;
        }

        GameObject controllerObject = new GameObject("LevelCompletionFlowController");
        instance = controllerObject.AddComponent<LevelCompletionFlowController>();
        instance.Initialize();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    private void OnDestroy()
    {
        if (instance != this)
        {
            return;
        }

        StopAutoRedirect();
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        instance = null;
    }

    private void Initialize()
    {
        if (instance != null && instance != this)
        {
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        apiClient = GetComponent<LevelCompletionApiClient>();
        if (apiClient == null)
        {
            apiClient = gameObject.AddComponent<LevelCompletionApiClient>();
        }

        popupUI = GetComponent<LevelCompletionPopupUI>();
        if (popupUI == null)
        {
            popupUI = gameObject.AddComponent<LevelCompletionPopupUI>();
        }

        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        RefreshSceneContext();
        popupUI.HideInstant();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        isCompleting = false;
        StopAutoRedirect();
        RefreshSceneContext();
        popupUI?.HideInstant();
    }

    private void RefreshSceneContext()
    {
        currentSceneLevel = SceneLevelDescriptor.FromScene(SceneManager.GetActiveScene());
        screenFadePlayerLock = FindFirstObjectByType<ScreenFadePlayerLock>();
    }

    private bool TryStartCompletion(int nextSceneBuildIndex)
    {
        RefreshSceneContext();

        if (isCompleting || !currentSceneLevel.IsSupported || screenFadePlayerLock == null || apiClient == null || popupUI == null)
        {
            return false;
        }

        StopAutoRedirect();
        StartCoroutine(CompleteLevelRoutine(nextSceneBuildIndex));
        return true;
    }

    private IEnumerator CompleteLevelRoutine(int nextSceneBuildIndex)
    {
        isCompleting = true;

        bool fadeCompleted = false;
        bool requestCompleted = false;
        bool loadingShown = false;
        LevelCompletionResponse response = null;
        string error = null;

        screenFadePlayerLock.FadeOutAndLock(() => fadeCompleted = true);

        StartCoroutine(apiClient.CompleteLevel(
            currentSceneLevel.LevelId,
            success =>
            {
                response = success;
                requestCompleted = true;
            },
            failure =>
            {
                error = failure;
                requestCompleted = true;
            }));

        while (!fadeCompleted || !requestCompleted)
        {
            if (fadeCompleted && !requestCompleted && !loadingShown)
            {
                popupUI.ShowLoading(currentSceneLevel.LevelId);
                loadingShown = true;
            }

            yield return null;
        }

        if (response != null)
        {
            ShowCompletionPopup(response, nextSceneBuildIndex);
        }
        else
        {
            ShowFallbackPopup(error, nextSceneBuildIndex);
        }

        isCompleting = false;
    }

    private void ShowCompletionPopup(LevelCompletionResponse response, int nextSceneBuildIndex)
    {
        LevelCompletionResponseData responseData = response.data;
        string levelLabel = ResolveLevelLabel(responseData?.levelId);
        bool alreadyCompleted = !response.success ||
            responseData == null ||
            responseData.wasCompleted ||
            (responseData.awardedStars <= 0 && responseData.awardedExp <= 0);

        int awardedStars = responseData != null ? Mathf.Max(0, responseData.awardedStars) : 0;
        int awardedExp = responseData != null ? Mathf.Max(0, responseData.awardedExp) : 0;

        string message = alreadyCompleted
            ? $"Вы уже проходили уровень {levelLabel}. Награда больше не выдаётся."
            : $"Вы прошли уровень {levelLabel} и получили +{awardedStars} stars и +{awardedExp} exp.";

        popupUI.Show(new LevelCompletionPopupState
        {
            Title = "Уровень пройден",
            LevelId = levelLabel,
            Message = message,
            StarsText = alreadyCompleted ? "+0" : $"+{awardedStars}",
            ExpText = alreadyCompleted ? "+0" : $"+{awardedExp}",
            Footer = BuildAutoRedirectFooter(),
        });

        StartAutoRedirect();
    }

    private void ShowFallbackPopup(string error, int nextSceneBuildIndex)
    {
        popupUI.Show(new LevelCompletionPopupState
        {
            Title = "Уровень пройден",
            LevelId = currentSceneLevel.LevelLabel,
            Message = BuildFallbackMessage(error),
            StarsText = "--",
            ExpText = "--",
            Footer = BuildAutoRedirectFooter(),
        });

        StartAutoRedirect();
    }

    private static string BuildAutoRedirectFooter()
    {
        return $"Автоматический переход через {Mathf.RoundToInt(AutoRedirectDelaySeconds)} секунд...";
    }

    private void StartAutoRedirect()
    {
        StopAutoRedirect();
        redirectRoutine = StartCoroutine(AutoRedirectRoutine());
    }

    private void StopAutoRedirect()
    {
        if (redirectRoutine == null)
        {
            return;
        }

        StopCoroutine(redirectRoutine);
        redirectRoutine = null;
    }

    private IEnumerator AutoRedirectRoutine()
    {
        yield return new WaitForSecondsRealtime(AutoRedirectDelaySeconds);

        string redirectUrl = ResolveRedirectUrl();
        redirectRoutine = null;

        if (string.IsNullOrWhiteSpace(redirectUrl))
        {
            Debug.LogWarning("Level completion redirect target is not configured yet.");
            yield break;
        }

        Application.OpenURL(redirectUrl);
    }

    private static string ResolveRedirectUrl()
    {
        string queryRedirectUrl;
        if (PracticeLaunchOptions.TryGetQueryParameter(RedirectQueryParameterName, out string redirectValue) &&
            PracticeLaunchOptions.TryBuildAbsoluteUrl(redirectValue, out queryRedirectUrl))
        {
            return queryRedirectUrl;
        }

        if (TryBuildDefaultRedirectUrl(out string defaultRedirectUrl))
        {
            return defaultRedirectUrl;
        }

        return string.Empty;
    }

    private static bool TryBuildDefaultRedirectUrl(out string redirectUrl)
    {
        redirectUrl = string.Empty;
        if (string.IsNullOrWhiteSpace(Application.absoluteURL))
        {
            return false;
        }

        if (!Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out Uri currentUri) ||
            string.IsNullOrWhiteSpace(currentUri.Host))
        {
            return false;
        }

        UriBuilder builder = new UriBuilder(currentUri.Scheme, currentUri.Host, DefaultRedirectPort, DefaultRedirectPath);
        redirectUrl = builder.Uri.ToString();
        return true;
    }

    private string ResolveLevelLabel(string responseLevelId)
    {
        string normalized = NormalizeLevelLabel(responseLevelId);
        if (!string.IsNullOrWhiteSpace(normalized))
        {
            return normalized;
        }

        return currentSceneLevel.LevelLabel;
    }

    private static string NormalizeLevelLabel(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        if (rawValue.StartsWith("level_", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(rawValue.Substring("level_".Length), out int backendLevelNumber))
        {
            return backendLevelNumber.ToString();
        }

        if (rawValue.StartsWith("Level", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(rawValue.Substring("Level".Length), out int sceneLevelNumber))
        {
            return sceneLevelNumber.ToString();
        }

        return rawValue.Trim();
    }

    private static string BuildFallbackMessage(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            return "Не удалось получить ответ от сервера. Попробуйте ещё раз.";
        }

        string trimmedError = error.Trim();
        if (trimmedError.Length > 140)
        {
            trimmedError = trimmedError.Substring(0, 140) + "...";
        }

        return $"Не удалось подтвердить награду на сервере.\n{trimmedError}";
    }

    private readonly struct SceneLevelDescriptor
    {
        public SceneLevelDescriptor(bool isSupported, string sceneName, int levelNumber, string levelId, string previousSceneName)
        {
            IsSupported = isSupported;
            SceneName = sceneName;
            LevelNumber = levelNumber;
            LevelId = levelId;
            PreviousSceneName = previousSceneName;
        }

        public bool IsSupported { get; }
        public string SceneName { get; }
        public int LevelNumber { get; }
        public string LevelId { get; }
        public string PreviousSceneName { get; }
        public string LevelLabel => LevelNumber > 0 ? LevelNumber.ToString() : string.Empty;

        public static SceneLevelDescriptor FromScene(Scene scene)
        {
            if (!TryParseLevelNumber(scene.name, out int levelNumber))
            {
                return new SceneLevelDescriptor(false, scene.name, 0, string.Empty, string.Empty);
            }

            string previousSceneName = levelNumber > 1 ? $"Level{levelNumber - 1}" : string.Empty;
            return new SceneLevelDescriptor(
                true,
                scene.name,
                levelNumber,
                $"level_{levelNumber}",
                previousSceneName);
        }

        private static bool TryParseLevelNumber(string sceneName, out int levelNumber)
        {
            levelNumber = 0;
            if (string.IsNullOrWhiteSpace(sceneName) || !sceneName.StartsWith("Level", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return int.TryParse(sceneName.Substring("Level".Length), out levelNumber) && levelNumber > 0;
        }
    }
}
