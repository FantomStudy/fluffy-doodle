using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TerminalInteraction : MonoBehaviour
{
    [SerializeField] private TerminalUI terminalUI;
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private LevelVariableType variableType;

    private bool playerInRange;

    public LevelVariableType VariableType => variableType;

    private void Awake()
    {
        SetPromptVisible(false);
    }

    private void Update()
    {
        if (!playerInRange || terminalUI == null)
        {
            return;
        }

        if (WasInteractPressed())
        {
            if (terminalUI.IsVisible && terminalUI.ActiveTerminal == this)
            {
                terminalUI.Hide();
            }
            else
            {
                terminalUI.ShowFor(this);
            }
        }
        else if (terminalUI.IsVisible && terminalUI.ActiveTerminal == this && WasCancelPressed())
        {
            terminalUI.Hide();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerInRange = true;
        SetPromptVisible(terminalUI == null || !terminalUI.IsVisible);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerInRange = false;
        SetPromptVisible(false);

        if (terminalUI != null && terminalUI.IsVisible && terminalUI.ActiveTerminal == this)
        {
            terminalUI.Hide();
        }
    }

    public void NotifyTerminalVisibilityChanged(bool visible)
    {
        SetPromptVisible(playerInRange && !visible);
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptRoot != null)
        {
            promptRoot.SetActive(visible);
        }
    }

    private static bool IsPlayer(Collider other)
    {
        return other.GetComponentInParent<CharacterController>() != null;
    }

    private static bool WasInteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }

    private static bool WasCancelPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}
