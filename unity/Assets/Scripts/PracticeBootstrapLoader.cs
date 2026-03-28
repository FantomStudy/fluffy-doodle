using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PracticeBootstrapLoader : MonoBehaviour
{
    [SerializeField] private string queryParameterName = "scene";
    [SerializeField] private string defaultSceneName = "Level1";
    [SerializeField] private float minimumLoadingDelay = 0.1f;

    private IEnumerator Start()
    {
        yield return null;

        string requestedScene = PracticeLaunchOptions.TryGetQueryParameter(queryParameterName, out string queryValue)
            ? queryValue
            : string.Empty;

        string targetSceneName = ResolveTargetSceneName(requestedScene);
        if (minimumLoadingDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(minimumLoadingDelay);
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private string ResolveTargetSceneName(string requestedScene)
    {
        string mappedScene = MapSceneAlias(requestedScene);
        if (!string.IsNullOrWhiteSpace(mappedScene) && Application.CanStreamedLevelBeLoaded(mappedScene))
        {
            return mappedScene;
        }

        if (Application.CanStreamedLevelBeLoaded(defaultSceneName))
        {
            return defaultSceneName;
        }

        return "Level1";
    }

    private static string MapSceneAlias(string rawScene)
    {
        if (string.IsNullOrWhiteSpace(rawScene))
        {
            return string.Empty;
        }

        string normalized = rawScene.Trim().Replace("-", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        return normalized switch
        {
            "if" => "Level1",
            "level1" => "Level1",
            "variables" => "Level2",
            "level2" => "Level2",
            "assets" => "Level3",
            "puzzle" => "Level3",
            "level3" => "Level3",
            "cycles" => "Level4",
            "level4" => "Level4",
            _ => TryMapDirectLevelName(rawScene, out string directSceneName) ? directSceneName : string.Empty,
        };
    }

    private static bool TryMapDirectLevelName(string rawScene, out string sceneName)
    {
        sceneName = string.Empty;
        if (string.IsNullOrWhiteSpace(rawScene))
        {
            return false;
        }

        string trimmed = rawScene.Trim();
        if (trimmed.StartsWith("Level", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(trimmed.Substring("Level".Length), out int levelNumber) &&
            levelNumber > 0)
        {
            sceneName = $"Level{levelNumber}";
            return true;
        }

        return false;
    }
}

public static class PracticeLaunchOptions
{
    public static bool IsMobileLaunch()
    {
        return TryGetQueryParameter("mobile", out string value) && IsEnabledValue(value);
    }

    public static bool TryGetQueryParameter(string parameterName, out string value)
    {
        value = string.Empty;
        if (string.IsNullOrWhiteSpace(parameterName) || string.IsNullOrWhiteSpace(Application.absoluteURL))
        {
            return false;
        }

        if (!Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out Uri absoluteUri))
        {
            return false;
        }

        string query = absoluteUri.Query;
        if (string.IsNullOrWhiteSpace(query))
        {
            return false;
        }

        string[] pairs = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (string pair in pairs)
        {
            string[] parts = pair.Split('=', 2);
            if (parts.Length == 0)
            {
                continue;
            }

            string key = Uri.UnescapeDataString(parts[0]);
            if (!string.Equals(key, parameterName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
            return true;
        }

        return false;
    }

    public static bool TryGetAbsoluteUrlOrigin(out string origin)
    {
        origin = string.Empty;
        if (string.IsNullOrWhiteSpace(Application.absoluteURL))
        {
            return false;
        }

        if (!Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out Uri absoluteUri))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(absoluteUri.Host))
        {
            return false;
        }

        origin = absoluteUri.GetLeftPart(UriPartial.Authority);
        return true;
    }

    public static bool TryBuildAbsoluteUrl(string rawValue, out string absoluteUrl)
    {
        absoluteUrl = string.Empty;
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return false;
        }

        string trimmed = rawValue.Trim();
        if (Uri.TryCreate(trimmed, UriKind.Absolute, out Uri absoluteUri))
        {
            absoluteUrl = absoluteUri.ToString();
            return true;
        }

        if (!trimmed.StartsWith("/", StringComparison.Ordinal) || !TryGetAbsoluteUrlOrigin(out string origin))
        {
            return false;
        }

        absoluteUrl = origin.TrimEnd('/') + trimmed;
        return true;
    }

    private static bool IsEnabledValue(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return false;
        }

        switch (rawValue.Trim().ToLowerInvariant())
        {
            case "1":
            case "true":
            case "yes":
            case "on":
                return true;
            default:
                return false;
        }
    }
}
