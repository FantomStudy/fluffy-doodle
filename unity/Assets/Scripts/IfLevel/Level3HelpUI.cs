using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Level3HelpUI : MonoBehaviour
{
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] [TextArea(5, 12)] private string title = "КАК ИГРАТЬ";
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] [TextArea(6, 16)] private string body =
        "В каждой комнате выбери один или несколько блоков.\n\n" +
        "Нажми E у терминала, чтобы проверить условие.\n\n" +
        "Дверь откроется только тогда, когда выражение истинно.\n\n" +
        "Сначала будут простые if, потом OR, а затем комбинации AND + OR.\n\n" +
        "Если условие не выполнено, измени выбор и попробуй снова.";

    private bool isVisible;
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private PlayerInteractor playerInteractor;
    private bool cachedControllerEnabled;
    private bool cachedInteractorEnabled;
    private bool cachedCursorLocked = true;
    private bool cachedCursorInputForLook = true;
    private bool hasCachedPlayerState;

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

        CachePlayerComponents();
        RefreshTexts();
        ApplyState(false);
    }

    private void Update()
    {
        if (!WasTogglePressed())
        {
            if (isVisible)
            {
                EnforceCursorVisible();
            }

            return;
        }

        if (isVisible)
        {
            Hide();
            return;
        }

        Show();
    }

    private void OnDisable()
    {
        isVisible = false;
        ApplyState(false);
    }

    public void Show()
    {
        if (isVisible)
        {
            return;
        }

        isVisible = true;
        RefreshTexts();
        ApplyState(true);
    }

    public void Hide()
    {
        if (!isVisible && panelCanvasGroup != null && panelCanvasGroup.alpha <= 0.001f)
        {
            ApplyState(false);
            return;
        }

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

        if (openButton != null)
        {
            openButton.gameObject.SetActive(!visible);
        }

        if (visible)
        {
            LockPlayerForHelp();
            EnforceCursorVisible();
        }
        else
        {
            RestorePlayerState();
        }
    }

    private void CachePlayerComponents()
    {
        if (thirdPersonController == null)
        {
            thirdPersonController = FindFirstObjectByType<ThirdPersonController>(FindObjectsInactive.Include);
        }

        Transform playerRoot = thirdPersonController != null ? thirdPersonController.transform.root : null;

        if (starterAssetsInputs == null)
        {
            starterAssetsInputs = playerRoot != null
                ? playerRoot.GetComponentInChildren<StarterAssetsInputs>(true)
                : FindFirstObjectByType<StarterAssetsInputs>(FindObjectsInactive.Include);
        }

        if (playerInteractor == null)
        {
            playerInteractor = playerRoot != null
                ? playerRoot.GetComponentInChildren<PlayerInteractor>(true)
                : FindFirstObjectByType<PlayerInteractor>(FindObjectsInactive.Include);
        }
    }

    private void LockPlayerForHelp()
    {
        CachePlayerComponents();

        if (!hasCachedPlayerState)
        {
            cachedControllerEnabled = thirdPersonController != null && thirdPersonController.enabled;
            cachedInteractorEnabled = playerInteractor != null && playerInteractor.enabled;
            cachedCursorLocked = starterAssetsInputs == null || starterAssetsInputs.cursorLocked;
            cachedCursorInputForLook = starterAssetsInputs == null || starterAssetsInputs.cursorInputForLook;
            hasCachedPlayerState = true;
        }

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.MoveInput(Vector2.zero);
            starterAssetsInputs.LookInput(Vector2.zero);
            starterAssetsInputs.JumpInput(false);
            starterAssetsInputs.SprintInput(false);
            starterAssetsInputs.cursorLocked = false;
            starterAssetsInputs.cursorInputForLook = false;
        }

        if (playerInteractor != null)
        {
            playerInteractor.enabled = false;
        }

        if (thirdPersonController != null)
        {
            thirdPersonController.enabled = false;
        }
    }

    private void RestorePlayerState()
    {
        if (!hasCachedPlayerState)
        {
            RestoreCursor(true);
            return;
        }

        CachePlayerComponents();

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.MoveInput(Vector2.zero);
            starterAssetsInputs.LookInput(Vector2.zero);
            starterAssetsInputs.JumpInput(false);
            starterAssetsInputs.SprintInput(false);
            starterAssetsInputs.cursorLocked = cachedCursorLocked;
            starterAssetsInputs.cursorInputForLook = cachedCursorInputForLook;
        }

        if (playerInteractor != null)
        {
            playerInteractor.enabled = cachedInteractorEnabled;
        }

        if (thirdPersonController != null)
        {
            thirdPersonController.enabled = cachedControllerEnabled;
        }

        RestoreCursor(cachedCursorLocked);
        hasCachedPlayerState = false;
    }

    private static void EnforceCursorVisible()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private static void RestoreCursor(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private static bool WasTogglePressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Escape);
#else
        return false;
#endif
    }
}
