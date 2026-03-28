using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed class LevelCompletionApiClient : MonoBehaviour
{
    private const string DefaultLocalhostBaseUrl = "http://localhost";

    [SerializeField] private string overrideBaseUrl = string.Empty;
    [SerializeField] private int timeoutSeconds = 5;

    private bool webGlRequestInProgress;
    private string webGlResponseJson;
    private string webGlError;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void PostLevelCompletionWebRequest(
        string gameObjectName,
        string successCallbackName,
        string errorCallbackName,
        string url,
        string bodyJson);
#endif

    public IEnumerator CompleteLevel(
        string levelId,
        Action<LevelCompletionResponse> onSuccess,
        Action<string> onError)
    {
        if (string.IsNullOrWhiteSpace(levelId))
        {
            onError?.Invoke("Level id is empty.");
            yield break;
        }

        string url = BuildCompleteLevelUrl(levelId);
        string requestBody = JsonUtility.ToJson(new LevelCompletionRequest());

#if UNITY_WEBGL && !UNITY_EDITOR
        yield return SendWebGlRequest(url, requestBody);

        if (!string.IsNullOrWhiteSpace(webGlError))
        {
            onError?.Invoke(webGlError);
            yield break;
        }

        if (TryParseResponse(webGlResponseJson, out LevelCompletionResponse webGlResponse, out string webGlParseError))
        {
            onSuccess?.Invoke(webGlResponse);
            yield break;
        }

        onError?.Invoke(webGlParseError);
#else
        yield return SendUnityWebRequest(url, requestBody, onSuccess, onError);
#endif
    }

    private string BuildCompleteLevelUrl(string levelId)
    {
        string baseUrl = ResolveBaseUrl();
        string escapedLevelId = UnityWebRequest.EscapeURL(levelId);
        return $"{baseUrl.TrimEnd('/')}/game/levels/{escapedLevelId}/complete";
    }

    private string ResolveBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(overrideBaseUrl))
        {
            return overrideBaseUrl.TrimEnd('/');
        }

        if (TryGetAbsoluteUrlOrigin(out string origin))
        {
            return origin;
        }

        return DefaultLocalhostBaseUrl;
    }

    private static bool TryGetAbsoluteUrlOrigin(out string origin)
    {
        origin = null;
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

#if UNITY_WEBGL && !UNITY_EDITOR
    private IEnumerator SendWebGlRequest(string url, string requestBody)
    {
        webGlRequestInProgress = true;
        webGlResponseJson = null;
        webGlError = null;

        PostLevelCompletionWebRequest(
            gameObject.name,
            nameof(OnWebGlRequestSucceeded),
            nameof(OnWebGlRequestFailed),
            url,
            requestBody);

        float startedAt = Time.realtimeSinceStartup;
        while (webGlRequestInProgress)
        {
            if (Time.realtimeSinceStartup - startedAt >= Mathf.Max(1, timeoutSeconds))
            {
                webGlRequestInProgress = false;
                webGlError = $"Level completion request timed out after {timeoutSeconds} seconds.";
                break;
            }

            yield return null;
        }
    }

    public void OnWebGlRequestSucceeded(string responseJson)
    {
        webGlResponseJson = responseJson;
        webGlError = null;
        webGlRequestInProgress = false;
    }

    public void OnWebGlRequestFailed(string errorMessage)
    {
        webGlResponseJson = null;
        webGlError = string.IsNullOrWhiteSpace(errorMessage)
            ? "Level completion request failed."
            : errorMessage;
        webGlRequestInProgress = false;
    }
#endif

    private IEnumerator SendUnityWebRequest(
        string url,
        string requestBody,
        Action<LevelCompletionResponse> onSuccess,
        Action<string> onError)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(requestBody);

        using UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(bodyBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = Mathf.Max(1, timeoutSeconds);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(BuildUnityWebRequestError(request));
            yield break;
        }

        if (request.responseCode < 200 || request.responseCode >= 300)
        {
            onError?.Invoke(BuildUnityWebRequestError(request));
            yield break;
        }

        if (TryParseResponse(request.downloadHandler.text, out LevelCompletionResponse response, out string parseError))
        {
            onSuccess?.Invoke(response);
            yield break;
        }

        onError?.Invoke(parseError);
    }

    private static bool TryParseResponse(
        string responseJson,
        out LevelCompletionResponse response,
        out string error)
    {
        response = null;
        error = null;

        if (string.IsNullOrWhiteSpace(responseJson))
        {
            error = "Backend returned an empty response.";
            return false;
        }

        try
        {
            response = JsonUtility.FromJson<LevelCompletionResponse>(responseJson);
        }
        catch (Exception exception)
        {
            error = $"Failed to parse level completion response: {exception.Message}";
            return false;
        }

        if (response == null)
        {
            error = "Backend returned an unreadable response.";
            return false;
        }

        if (!response.success || response.data == null)
        {
            error = string.IsNullOrWhiteSpace(response.message)
                ? "Backend reported an unsuccessful level completion."
                : response.message;
            return false;
        }

        return true;
    }

    private static string BuildUnityWebRequestError(UnityWebRequest request)
    {
        string responseText = request.downloadHandler?.text;
        if (!string.IsNullOrWhiteSpace(responseText))
        {
            return $"HTTP {request.responseCode}: {responseText}";
        }

        if (!string.IsNullOrWhiteSpace(request.error))
        {
            return $"HTTP {request.responseCode}: {request.error}";
        }

        return $"HTTP {request.responseCode}: Level completion request failed.";
    }
}
