using System;
using TMPro;
using UnityEngine;

public class IfLogicTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Press E to validate condition";
    [SerializeField] [TextArea(2, 4)] private string conditionDescription = "if (selection == Cube)";
    [SerializeField] private SimpleDoor controlledDoor;
    [SerializeField] private IfLogicCondition[] allConditions;
    [SerializeField] private IfLogicCondition[] anyConditions;
    [SerializeField] private Renderer statusRenderer;
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material successMaterial;
    [SerializeField] private Material errorMaterial;
    [SerializeField] private TMP_Text conditionText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private string idleStatusText = "READY";
    [SerializeField] private string successStatusText = "TRUE";
    [SerializeField] private string errorStatusText = "FALSE";

    private bool isSolved;

    public string InteractionPrompt => interactionPrompt;
    public bool IsSolved => isSolved;

    private void Awake()
    {
        RefreshTexts();
        ApplyVisualState(TerminalVisualState.Idle);
    }

    public void Interact(PlayerInteractor interactor)
    {
        bool passed = EvaluateAllConditions() && EvaluateAnyConditions();
        if (passed)
        {
            isSolved = true;
            controlledDoor?.Open();
            ApplyVisualState(TerminalVisualState.Success);
            return;
        }

        ApplyVisualState(TerminalVisualState.Error);
    }

    public void RefreshTexts()
    {
        if (conditionText != null)
        {
            conditionText.text = conditionDescription;
        }

        if (statusText != null)
        {
            statusText.text = idleStatusText;
        }
    }

    private bool EvaluateAllConditions()
    {
        if (allConditions == null || allConditions.Length == 0)
        {
            return true;
        }

        foreach (IfLogicCondition condition in allConditions)
        {
            if (condition == null)
            {
                continue;
            }

            if (!condition.Evaluate())
            {
                return false;
            }
        }

        return true;
    }

    private bool EvaluateAnyConditions()
    {
        if (anyConditions == null || anyConditions.Length == 0)
        {
            return true;
        }

        foreach (IfLogicCondition condition in anyConditions)
        {
            if (condition != null && condition.Evaluate())
            {
                return true;
            }
        }

        return false;
    }

    private void ApplyVisualState(TerminalVisualState visualState)
    {
        if (statusRenderer != null)
        {
            statusRenderer.sharedMaterial = visualState switch
            {
                TerminalVisualState.Success when successMaterial != null => successMaterial,
                TerminalVisualState.Error when errorMaterial != null => errorMaterial,
                _ => idleMaterial,
            };
        }

        if (statusText != null)
        {
            statusText.text = visualState switch
            {
                TerminalVisualState.Success => successStatusText,
                TerminalVisualState.Error => errorStatusText,
                _ => idleStatusText,
            };
        }
    }

    private enum TerminalVisualState
    {
        Idle = 0,
        Success = 1,
        Error = 2,
    }
}

[Serializable]
public class IfLogicCondition
{
    [SerializeField] private IfLogicSourceType sourceType = IfLogicSourceType.Selection;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private IfObjectId[] acceptedObjectIds;
    [SerializeField] private IfLogicTerminal requiredTerminal;
    [SerializeField] private bool requiredTerminalSolved = true;
    [SerializeField] private SimpleDoor requiredDoor;
    [SerializeField] private bool requiredDoorOpened = true;
    [SerializeField] private bool invertResult;

    public bool Evaluate()
    {
        bool result = sourceType switch
        {
            IfLogicSourceType.Selection => EvaluateSelection(),
            IfLogicSourceType.TerminalSolved => EvaluateTerminal(),
            IfLogicSourceType.DoorOpened => EvaluateDoor(),
            _ => false,
        };

        return invertResult ? !result : result;
    }

    private bool EvaluateSelection()
    {
        if (selectionManager == null || acceptedObjectIds == null || acceptedObjectIds.Length == 0)
        {
            return false;
        }

        IfObjectId selectedId = selectionManager.CurrentSelectedId;
        foreach (IfObjectId acceptedId in acceptedObjectIds)
        {
            if (selectedId == acceptedId)
            {
                return true;
            }
        }

        return false;
    }

    private bool EvaluateTerminal()
    {
        return requiredTerminal != null && requiredTerminal.IsSolved == requiredTerminalSolved;
    }

    private bool EvaluateDoor()
    {
        return requiredDoor != null && requiredDoor.IsOpen == requiredDoorOpened;
    }
}

public enum IfLogicSourceType
{
    Selection = 0,
    TerminalSolved = 1,
    DoorOpened = 2,
}
