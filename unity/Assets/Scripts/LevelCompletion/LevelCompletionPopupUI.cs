using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelCompletionPopupUI : MonoBehaviour
{
    private const float FadeDuration = 0.2f;

    private Canvas popupCanvas;
    private CanvasGroup popupCanvasGroup;
    private Text titleText;
    private Text levelText;
    private Text messageText;
    private Text starsValueText;
    private Text expValueText;
    private Text footerText;
    private Button primaryButton;
    private Text primaryButtonText;
    private Button secondaryButton;
    private Text secondaryButtonText;
    private Coroutine fadeRoutine;

    public void ShowLoading(string levelId)
    {
        Show(new LevelCompletionPopupState
        {
            Title = "Завершение уровня",
            LevelId = levelId,
            Message = "Сохраняем прохождение на сервере...",
            StarsText = "--",
            ExpText = "--",
            Footer = string.Empty,
        });
    }

    public void Show(LevelCompletionPopupState popupState)
    {
        EnsureUi();
        ApplyState(popupState);
        SetVisible(true);
    }

    public void HideInstant()
    {
        if (popupCanvas == null || popupCanvasGroup == null)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        popupCanvasGroup.alpha = 0f;
        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;
        popupCanvas.gameObject.SetActive(false);
    }

    private void EnsureUi()
    {
        if (popupCanvas != null)
        {
            return;
        }

        Font uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject canvasObject = CreateUiObject("LevelCompletionPopupCanvas", transform);
        popupCanvas = canvasObject.AddComponent<Canvas>();
        popupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        popupCanvas.sortingOrder = 5000;
        canvasObject.AddComponent<GraphicRaycaster>();

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        popupCanvasGroup = canvasObject.AddComponent<CanvasGroup>();
        popupCanvasGroup.alpha = 0f;
        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;

        GameObject backdropObject = CreateUiObject("Backdrop", canvasObject.transform);
        Image backdropImage = backdropObject.AddComponent<Image>();
        backdropImage.color = new Color(0f, 0f, 0f, 0.78f);
        StretchToFullScreen(backdropObject.GetComponent<RectTransform>());

        GameObject panelObject = CreateUiObject("Panel", backdropObject.transform);
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.12f, 0.18f, 0.98f);
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(720f, 420f);

        titleText = CreateText("Title", panelObject.transform, uiFont, 38, FontStyle.Bold, TextAnchor.UpperCenter);
        SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -38f), new Vector2(620f, 48f));

        levelText = CreateText("LevelText", panelObject.transform, uiFont, 28, FontStyle.Bold, TextAnchor.UpperCenter);
        levelText.color = new Color(0.87f, 0.92f, 1f, 1f);
        SetRect(levelText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(620f, 40f));

        messageText = CreateText("MessageText", panelObject.transform, uiFont, 23, FontStyle.Normal, TextAnchor.UpperCenter);
        messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
        messageText.verticalOverflow = VerticalWrapMode.Overflow;
        messageText.color = new Color(0.8f, 0.86f, 0.95f, 1f);
        SetRect(messageText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -145f), new Vector2(600f, 72f));

        CreateRewardRow(
            panelObject.transform,
            uiFont,
            "StarsRow",
            new Color(0.96f, 0.8f, 0.18f, 1f),
            "stars",
            new Vector2(0f, -230f),
            out starsValueText);

        CreateRewardRow(
            panelObject.transform,
            uiFont,
            "ExpRow",
            new Color(0.2f, 0.86f, 0.58f, 1f),
            "exp",
            new Vector2(0f, -290f),
            out expValueText);

        footerText = CreateText("FooterText", panelObject.transform, uiFont, 20, FontStyle.Normal, TextAnchor.MiddleCenter);
        footerText.color = new Color(0.67f, 0.75f, 0.87f, 1f);
        SetRect(footerText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 86f), new Vector2(620f, 28f));

        primaryButton = CreateButton(
            panelObject.transform,
            uiFont,
            "PrimaryButton",
            new Vector2(-130f, 40f),
            new Color(0.23f, 0.5f, 0.94f, 1f),
            out primaryButtonText);

        secondaryButton = CreateButton(
            panelObject.transform,
            uiFont,
            "SecondaryButton",
            new Vector2(130f, 40f),
            new Color(0.27f, 0.31f, 0.39f, 1f),
            out secondaryButtonText);

        canvasObject.SetActive(false);
    }

    private void ApplyState(LevelCompletionPopupState popupState)
    {
        string levelLabel = FormatLevelLabel(popupState.LevelId);

        titleText.text = string.IsNullOrWhiteSpace(popupState.Title)
            ? "Уровень пройден"
            : popupState.Title;

        bool showLevel = !string.IsNullOrWhiteSpace(levelLabel);
        levelText.gameObject.SetActive(showLevel);
        levelText.text = showLevel ? $"Уровень {levelLabel} пройден" : string.Empty;

        messageText.text = popupState.Message ?? string.Empty;
        starsValueText.text = popupState.StarsText ?? "--";
        expValueText.text = popupState.ExpText ?? "--";

        bool hasFooter = !string.IsNullOrWhiteSpace(popupState.Footer);
        footerText.gameObject.SetActive(hasFooter);
        footerText.text = hasFooter ? popupState.Footer : string.Empty;

        ConfigureButton(primaryButton, primaryButtonText, popupState.PrimaryButtonText, popupState.PrimaryAction);
        ConfigureButton(secondaryButton, secondaryButtonText, popupState.SecondaryButtonText, popupState.SecondaryAction);
    }

    private static string FormatLevelLabel(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        string trimmed = rawValue.Trim();
        if (trimmed.StartsWith("level_", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(trimmed.Substring("level_".Length), out int backendLevelNumber))
        {
            return backendLevelNumber.ToString();
        }

        if (trimmed.StartsWith("Level", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(trimmed.Substring("Level".Length), out int sceneLevelNumber))
        {
            return sceneLevelNumber.ToString();
        }

        return trimmed;
    }

    private void SetVisible(bool visible)
    {
        if (popupCanvas == null || popupCanvasGroup == null)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        popupCanvas.gameObject.SetActive(true);
        fadeRoutine = StartCoroutine(FadeRoutine(visible ? 1f : 0f));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = popupCanvasGroup.alpha;
        float elapsed = 0f;

        popupCanvasGroup.blocksRaycasts = targetAlpha > 0.001f;
        popupCanvasGroup.interactable = targetAlpha > 0.001f;

        while (elapsed < FadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = FadeDuration <= 0f ? 1f : elapsed / FadeDuration;
            popupCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        popupCanvasGroup.alpha = targetAlpha;
        popupCanvasGroup.blocksRaycasts = targetAlpha > 0.001f;
        popupCanvasGroup.interactable = targetAlpha > 0.001f;

        if (targetAlpha <= 0.001f)
        {
            popupCanvas.gameObject.SetActive(false);
        }

        fadeRoutine = null;
    }

    private static void CreateRewardRow(
        Transform parent,
        Font font,
        string name,
        Color iconColor,
        string label,
        Vector2 anchoredPosition,
        out Text valueText)
    {
        GameObject rowObject = CreateUiObject(name, parent);
        RectTransform rowRect = rowObject.GetComponent<RectTransform>();
        SetRect(rowRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), anchoredPosition, new Vector2(560f, 48f));

        GameObject iconObject = CreateUiObject("Icon", rowObject.transform);
        Image iconImage = iconObject.AddComponent<Image>();
        iconImage.color = iconColor;
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        SetRect(iconRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(32f, 0f), new Vector2(28f, 28f));

        Text labelText = CreateText("Label", rowObject.transform, font, 24, FontStyle.Bold, TextAnchor.MiddleLeft);
        labelText.color = Color.white;
        labelText.text = label;
        SetRect(labelText.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(64f, 0f), new Vector2(180f, 36f));

        valueText = CreateText("Value", rowObject.transform, font, 24, FontStyle.Bold, TextAnchor.MiddleRight);
        valueText.color = Color.white;
        SetRect(valueText.rectTransform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-20f, 0f), new Vector2(180f, 36f));
    }

    private static Button CreateButton(
        Transform parent,
        Font font,
        string name,
        Vector2 anchoredPosition,
        Color backgroundColor,
        out Text labelText)
    {
        GameObject buttonObject = CreateUiObject(name, parent);
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = backgroundColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        ColorBlock colors = button.colors;
        colors.normalColor = backgroundColor;
        colors.highlightedColor = backgroundColor * 1.08f;
        colors.pressedColor = backgroundColor * 0.92f;
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0.5f);
        button.colors = colors;

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        SetRect(buttonRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f), anchoredPosition, new Vector2(220f, 58f));

        labelText = CreateText("Label", buttonObject.transform, font, 24, FontStyle.Bold, TextAnchor.MiddleCenter);
        labelText.color = Color.white;
        StretchToParent(labelText.rectTransform, new Vector2(12f, 8f));

        return button;
    }

    private static void ConfigureButton(Button button, Text labelText, string label, Action action)
    {
        if (button == null || labelText == null)
        {
            return;
        }

        bool isVisible = action != null && !string.IsNullOrWhiteSpace(label);
        button.gameObject.SetActive(isVisible);
        button.onClick.RemoveAllListeners();

        if (!isVisible)
        {
            return;
        }

        labelText.text = label;
        button.onClick.AddListener(() => action.Invoke());
    }

    private static Text CreateText(
        string name,
        Transform parent,
        Font font,
        int fontSize,
        FontStyle fontStyle,
        TextAnchor alignment)
    {
        GameObject textObject = CreateUiObject(name, parent);
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = Color.white;
        text.supportRichText = false;
        return text;
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void StretchToFullScreen(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static void StretchToParent(RectTransform rectTransform, Vector2 padding)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = padding;
        rectTransform.offsetMax = -padding;
    }

    private static void SetRect(
        RectTransform rectTransform,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
    }
}

public sealed class LevelCompletionPopupState
{
    public string Title;
    public string LevelId;
    public string Message;
    public string StarsText;
    public string ExpText;
    public string Footer;
    public string PrimaryButtonText;
    public Action PrimaryAction;
    public string SecondaryButtonText;
    public Action SecondaryAction;
}
