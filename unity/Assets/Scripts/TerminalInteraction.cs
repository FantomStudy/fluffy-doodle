using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerInRange = true;
        SetPromptVisible(false);
        terminalUI?.ShowFor(this);
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
        SetPromptVisible(false);
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
}
