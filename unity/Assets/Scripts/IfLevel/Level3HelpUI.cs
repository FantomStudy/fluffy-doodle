using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Level3HelpUI : MonoBehaviour
{
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] [TextArea(5, 12)] private string title = "HOW TO PLAY";
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] [TextArea(6, 16)] private string body =
        "Choose one block in each room.\n\n" +
        "Walk to the terminal and press E to validate the condition.\n\n" +
        "Doors open only when the statement is TRUE.\n\n" +
        "The rooms get harder: first simple if checks, then OR, then AND + OR combinations.";
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;

    private bool isVisible;

    private void Awake()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(Show);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        RefreshTexts();
        ApplyState(false);
    }

    private void Update()
    {
        if (WasTogglePressed())
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    public void Show()
    {
        isVisible = true;
        RefreshTexts();
        ApplyState(true);
    }

    public void Hide()
    {
        isVisible = false;
        ApplyState(false);
    }

    private void RefreshTexts()
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (bodyText != null)
        {
            bodyText.text = body;
        }
    }

    private void ApplyState(bool visible)
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = visible ? 1f : 0f;
            panelCanvasGroup.blocksRaycasts = visible;
            panelCanvasGroup.interactable = visible;
        }

        if (screenFadePlayerLock != null)
        {
            screenFadePlayerLock.SetLockedInstant(visible, 0f);
        }
        else
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private bool WasTogglePressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (toggleKey == KeyCode.F1 && Keyboard.current.f1Key.wasPressedThisFrame)
            {
                return true;
            }

            if (toggleKey == KeyCode.H && Keyboard.current.hKey.wasPressedThisFrame)
            {
                return true;
            }
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(toggleKey);
#else
        return false;
#endif
    }
}
