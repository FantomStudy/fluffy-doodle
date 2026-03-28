using TMPro;
using UnityEngine;

public class TerminalIndicator : MonoBehaviour
{
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material runningMaterial;
    [SerializeField] private Material successMaterial;
    [SerializeField] private Material errorMaterial;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject idleStateRoot;
    [SerializeField] private GameObject runningStateRoot;
    [SerializeField] private GameObject successStateRoot;
    [SerializeField] private GameObject errorStateRoot;

    public void ApplyState(TerminalIndicatorState state, string title = null, string body = null, string status = null)
    {
        if (screenRenderer != null)
        {
            screenRenderer.sharedMaterial = GetMaterial(state);
        }

        if (titleText != null && title != null)
        {
            titleText.text = title;
        }

        if (bodyText != null && body != null)
        {
            bodyText.text = body;
        }

        if (statusText != null && status != null)
        {
            statusText.text = status;
        }

        if (idleStateRoot != null)
        {
            idleStateRoot.SetActive(state == TerminalIndicatorState.Idle);
        }

        if (runningStateRoot != null)
        {
            runningStateRoot.SetActive(state == TerminalIndicatorState.Running);
        }

        if (successStateRoot != null)
        {
            successStateRoot.SetActive(state == TerminalIndicatorState.Success);
        }

        if (errorStateRoot != null)
        {
            errorStateRoot.SetActive(state == TerminalIndicatorState.Error);
        }
    }

    private Material GetMaterial(TerminalIndicatorState state)
    {
        return state switch
        {
            TerminalIndicatorState.Running when runningMaterial != null => runningMaterial,
            TerminalIndicatorState.Success when successMaterial != null => successMaterial,
            TerminalIndicatorState.Error when errorMaterial != null => errorMaterial,
            _ => idleMaterial,
        };
    }
}

public enum TerminalIndicatorState
{
    Idle = 0,
    Running = 1,
    Success = 2,
    Error = 3,
}
