using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LevelCompletionFlowController : MonoBehaviour
{
    private static LevelCompletionFlowController instance;

    private LevelCompletionApiClient apiClient;
    private LevelCompletionPopupUI popupUI;
    private ScreenFadePlayerLock screenFadePlayerLock;
    private SceneLevelDescriptor currentSceneLevel;
    private bool isCompleting;

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

        StartCoroutine(CompleteLevelRoutine(nextSceneBuildIndex));
        return true;
    }

    private IEnumerator CompleteLevelRoutine(int nextSceneBuildIndex)
    {
        isCompleting = true;

        bool fadeCompleted = false;
        screenFadePlayerLock.FadeOutAndLock(() => fadeCompleted = true);
        while (!fadeCompleted)
        {
            yield return null;
        }

        popupUI.ShowLoading(currentSceneLevel.LevelId);

        LevelCompletionResponse response = null;
        string error = null;

        yield return apiClient.CompleteLevel(
            currentSceneLevel.LevelId,
            success => response = success,
            failure => error = failure);

        if (response != null)
        {
            ShowSuccessPopup(response, nextSceneBuildIndex);
        }
        else
        {
            ShowFallbackPopup(error, nextSceneBuildIndex);
        }

        isCompleting = false;
    }

    private void ShowSuccessPopup(LevelCompletionResponse response, int nextSceneBuildIndex)
    {
        LevelCompletionResponseData responseData = response.data;
        string resolvedLevelId = string.IsNullOrWhiteSpace(responseData.levelId)
            ? currentSceneLevel.LevelId
            : responseData.levelId;

        string message = responseData.wasCompleted || (responseData.awardedStars <= 0 && responseData.awardedExp <= 0)
            ? "Повторное прохождение: награды уже были получены ранее."
            : "Новые награды добавлены в профиль.";

        string footer = $"Всего: {responseData.currentStars} stars, {responseData.currentExp} exp";
        LevelCompletionPopupAction primaryAction = BuildPrimaryAction(nextSceneBuildIndex);
        LevelCompletionPopupAction secondaryAction = BuildSecondaryAction(nextSceneBuildIndex);

        popupUI.Show(new LevelCompletionPopupState
        {
            Title = "Уровень пройден",
            LevelId = resolvedLevelId,
            Message = message,
            StarsText = responseData.awardedStars.ToString(),
            ExpText = responseData.awardedExp.ToString(),
            Footer = footer,
            PrimaryButtonText = primaryAction.Label,
            PrimaryAction = primaryAction.Callback,
            SecondaryButtonText = secondaryAction.Label,
            SecondaryAction = secondaryAction.Callback,
        });
    }

    private void ShowFallbackPopup(string error, int nextSceneBuildIndex)
    {
        LevelCompletionPopupAction primaryAction = BuildPrimaryAction(nextSceneBuildIndex);
        LevelCompletionPopupAction secondaryAction = BuildSecondaryAction(nextSceneBuildIndex);

        popupUI.Show(new LevelCompletionPopupState
        {
            Title = "Уровень пройден",
            LevelId = currentSceneLevel.LevelId,
            Message = BuildFallbackMessage(error),
            StarsText = "--",
            ExpText = "--",
            Footer = string.Empty,
            PrimaryButtonText = primaryAction.Label,
            PrimaryAction = primaryAction.Callback,
            SecondaryButtonText = secondaryAction.Label,
            SecondaryAction = secondaryAction.Callback,
        });
    }

    private LevelCompletionPopupAction BuildPrimaryAction(int nextSceneBuildIndex)
    {
        if (nextSceneBuildIndex >= 0)
        {
            return new LevelCompletionPopupAction("Далее", () => LoadScene(nextSceneBuildIndex));
        }

        return new LevelCompletionPopupAction("Заново", ReloadCurrentScene);
    }

    private LevelCompletionPopupAction BuildSecondaryAction(int nextSceneBuildIndex)
    {
        if (nextSceneBuildIndex >= 0)
        {
            return new LevelCompletionPopupAction("Заново", ReloadCurrentScene);
        }

        if (currentSceneLevel.HasPreviousScene)
        {
            return new LevelCompletionPopupAction("Назад", () => LoadScene(currentSceneLevel.PreviousSceneName));
        }

        return LevelCompletionPopupAction.None;
    }

    private void ReloadCurrentScene()
    {
        popupUI.HideInstant();
        SceneManager.LoadScene(currentSceneLevel.SceneName);
    }

    private void LoadScene(int buildIndex)
    {
        popupUI.HideInstant();
        SceneManager.LoadScene(buildIndex);
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return;
        }

        popupUI.HideInstant();
        SceneManager.LoadScene(sceneName);
    }

    private static string BuildFallbackMessage(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            return "Уровень пройден, но награды не удалось загрузить.";
        }

        string trimmedError = error.Trim();
        if (trimmedError.Length > 140)
        {
            trimmedError = trimmedError.Substring(0, 140) + "...";
        }

        return $"Уровень пройден, но награды не удалось загрузить.\n{trimmedError}";
    }

    private readonly struct LevelCompletionPopupAction
    {
        public static readonly LevelCompletionPopupAction None = new LevelCompletionPopupAction(null, null);

        public LevelCompletionPopupAction(string label, Action callback)
        {
            Label = label;
            Callback = callback;
        }

        public string Label { get; }
        public Action Callback { get; }
    }

    private readonly struct SceneLevelDescriptor
    {
        public SceneLevelDescriptor(bool isSupported, string sceneName, string levelId, string previousSceneName)
        {
            IsSupported = isSupported;
            SceneName = sceneName;
            LevelId = levelId;
            PreviousSceneName = previousSceneName;
        }

        public bool IsSupported { get; }
        public string SceneName { get; }
        public string LevelId { get; }
        public string PreviousSceneName { get; }
        public bool HasPreviousScene => !string.IsNullOrWhiteSpace(PreviousSceneName);

        public static SceneLevelDescriptor FromScene(Scene scene)
        {
            if (!TryParseLevelNumber(scene.name, out int levelNumber))
            {
                return new SceneLevelDescriptor(false, scene.name, string.Empty, string.Empty);
            }

            if (levelNumber < 1 || levelNumber > 3)
            {
                return new SceneLevelDescriptor(false, scene.name, string.Empty, string.Empty);
            }

            string previousSceneName = levelNumber > 1 ? $"Level{levelNumber - 1}" : string.Empty;
            return new SceneLevelDescriptor(
                true,
                scene.name,
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

            return int.TryParse(sceneName.Substring("Level".Length), out levelNumber);
        }
    }
}
