using System;
using TMPro;
using UnityEngine;

public class IfLogicTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Нажмите E, чтобы проверить условие";
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
    [SerializeField] private string idleStatusText = "ГОТОВО";
    [SerializeField] private string successStatusText = "ИСТИНА";
    [SerializeField] private string errorStatusText = "ЛОЖЬ";
    [SerializeField] private bool createInteractionTrigger = true;
    [SerializeField] private Vector3 triggerLocalPosition = new Vector3(0f, 1.6f, 1.8f);
    [SerializeField] private Vector3 triggerSize = new Vector3(6f, 4.6f, 6f);

    private bool isSolved;

    public string InteractionPrompt => interactionPrompt;
    public bool IsSolved => isSolved;

    private void Awake()
    {
        LocalizeDefaults();
        EnsureInteractionTrigger();
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

    private void LocalizeDefaults()
    {
        if (interactionPrompt == "Press E to validate condition")
        {
            interactionPrompt = "Нажмите E, чтобы проверить условие";
        }

        if (idleStatusText == "READY")
        {
            idleStatusText = "ГОТОВО";
        }

        if (successStatusText == "TRUE")
        {
            successStatusText = "ИСТИНА";
        }

        if (errorStatusText == "FALSE")
        {
            errorStatusText = "ЛОЖЬ";
        }

        conditionDescription = conditionDescription switch
        {
            "if (selection == Cube)" => "if (выбор == Куб)",
            "if (selection == Sphere || selection == Cylinder)" => "if (выбор == Сфера || выбор == Цилиндр)",
            "if (Room1 == TRUE && selection == Cylinder)" => "if (Комната 1 == ИСТИНА && выбор == Цилиндр)",
            "if (Door2 is open && (selection == Cube || selection == Sphere))" => "if (Дверь 2 открыта && (выбор == Куб || выбор == Сфера))",
            "if ((Room3 == TRUE && Room4 == TRUE) && (selection == Sphere || selection == Cylinder))" =>
                "if ((Комната 3 == ИСТИНА && Комната 4 == ИСТИНА) && (выбор == Сфера || выбор == Цилиндр))",
            _ => conditionDescription,
        };
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

    private void EnsureInteractionTrigger()
    {
        if (!createInteractionTrigger)
        {
            return;
        }

        Transform existingZone = transform.Find("InteractionTriggerZone");
        if (existingZone == null)
        {
            GameObject zoneObject = new GameObject("InteractionTriggerZone");
            existingZone = zoneObject.transform;
            existingZone.SetParent(transform, false);
        }

        existingZone.localPosition = triggerLocalPosition;
        existingZone.localRotation = Quaternion.identity;
        existingZone.localScale = Vector3.one;

        BoxCollider triggerCollider = existingZone.GetComponent<BoxCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = existingZone.gameObject.AddComponent<BoxCollider>();
        }

        triggerCollider.isTrigger = true;
        triggerCollider.center = Vector3.zero;
        triggerCollider.size = triggerSize;

        InteractionTriggerZone triggerZone = existingZone.GetComponent<InteractionTriggerZone>();
        if (triggerZone == null)
        {
            triggerZone = existingZone.gameObject.AddComponent<InteractionTriggerZone>();
        }

        triggerZone.SetTarget(this, interactionPrompt);
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

        foreach (IfObjectId acceptedId in acceptedObjectIds)
        {
            if (selectionManager.HasSelectedId(acceptedId))
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
