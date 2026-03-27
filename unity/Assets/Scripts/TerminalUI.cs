using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalUI : MonoBehaviour
{
    [SerializeField] private VariableLevelController levelController;
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text expressionText;
    [SerializeField] private TMP_Text selectedValueText;
    [SerializeField] private GameObject optionListRoot;
    [SerializeField] private Button selectedValueButton;
    [SerializeField] private Button optionOneButton;
    [SerializeField] private Button optionTwoButton;
    [SerializeField] private Button optionThreeButton;
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine fadeRoutine;
    private TerminalInteraction activeTerminal;
    private TMP_Text optionOneText;
    private TMP_Text optionTwoText;
    private TMP_Text optionThreeText;

    public bool IsVisible => canvasGroup != null && canvasGroup.interactable;
    public TerminalInteraction ActiveTerminal => activeTerminal;

    private void Awake()
    {
        SetVisible(false, true);
        CacheOptionTexts();

        if (selectedValueButton != null)
        {
            selectedValueButton.onClick.AddListener(ToggleOptionList);
        }

        optionOneButton?.onClick.AddListener(() => SelectValue(0));
        optionTwoButton?.onClick.AddListener(() => SelectValue(1));
        optionThreeButton?.onClick.AddListener(() => SelectValue(2));
    }

    public void ShowFor(TerminalInteraction terminalInteraction)
    {
        if (terminalInteraction == null)
        {
            return;
        }

        activeTerminal = terminalInteraction;
        RefreshActiveDisplay();
        SetVisible(true, false);
    }

    public void RefreshActiveDisplay()
    {
        if (activeTerminal == null || levelController == null)
        {
            return;
        }

        LevelVariableType variableType = activeTerminal.VariableType;
        IReadOnlyList<int> options = levelController.GetOptions(variableType);
        UpdateExpression(variableType);
        UpdateOptionTexts(options);
        UpdateSelectedValue(levelController.GetCurrentValue(variableType));
    }

    public void Hide()
    {
        SetVisible(false, false);
        activeTerminal = null;
    }

    public void ForceClose()
    {
        SetVisible(false, true);
        activeTerminal = null;
    }

    private void SetVisible(bool visible, bool instant)
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        if (screenFadePlayerLock != null)
        {
            screenFadePlayerLock.SetLockedInstant(visible, 0f);
        }

        activeTerminal?.NotifyTerminalVisibilityChanged(visible);
        SetOptionListVisible(false);

        if (instant)
        {
            ApplyCanvasState(visible ? 1f : 0f, visible);
            return;
        }

        fadeRoutine = StartCoroutine(FadeRoutine(visible));
    }

    private IEnumerator FadeRoutine(bool visible)
    {
        float target = visible ? 1f : 0f;
        float start = canvasGroup.alpha;
        float elapsed = 0f;

        if (visible)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = fadeDuration <= 0f ? 1f : elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }

        ApplyCanvasState(target, visible);
        fadeRoutine = null;
    }

    private void ApplyCanvasState(float alpha, bool visible)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }

    private void ToggleOptionList()
    {
        if (optionListRoot == null)
        {
            return;
        }

        optionListRoot.SetActive(!optionListRoot.activeSelf);
    }

    private void SelectValue(int optionIndex)
    {
        if (activeTerminal == null || levelController == null)
        {
            return;
        }

        IReadOnlyList<int> options = levelController.GetOptions(activeTerminal.VariableType);
        if (optionIndex < 0 || optionIndex >= options.Count)
        {
            return;
        }

        int value = options[optionIndex];
        levelController.SetVariable(activeTerminal.VariableType, value);
        UpdateSelectedValue(value);
        SetOptionListVisible(false);
    }

    private void UpdateSelectedValue(int value)
    {
        if (selectedValueText != null)
        {
            selectedValueText.text = value.ToString();
        }
    }

    private void SetOptionListVisible(bool visible)
    {
        if (optionListRoot != null)
        {
            optionListRoot.SetActive(visible);
        }
    }

    private void CacheOptionTexts()
    {
        optionOneText = ResolveButtonText(optionOneButton);
        optionTwoText = ResolveButtonText(optionTwoButton);
        optionThreeText = ResolveButtonText(optionThreeButton);
    }

    private void UpdateExpression(LevelVariableType variableType)
    {
        if (expressionText != null && levelController != null)
        {
            expressionText.text = levelController.GetVariableName(variableType) + " =";
        }
    }

    private void UpdateOptionTexts(IReadOnlyList<int> options)
    {
        SetOption(optionOneButton, optionOneText, options, 0);
        SetOption(optionTwoButton, optionTwoText, options, 1);
        SetOption(optionThreeButton, optionThreeText, options, 2);
    }

    private static TMP_Text ResolveButtonText(Button button)
    {
        return button != null ? button.GetComponentInChildren<TMP_Text>(true) : null;
    }

    private static void SetOption(Button button, TMP_Text label, IReadOnlyList<int> options, int index)
    {
        bool hasValue = options != null && index < options.Count;

        if (button != null)
        {
            button.gameObject.SetActive(hasValue);
        }

        if (hasValue && label != null)
        {
            label.text = options[index].ToString();
        }
    }
}
