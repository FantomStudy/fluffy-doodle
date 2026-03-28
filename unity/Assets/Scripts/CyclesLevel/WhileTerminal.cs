using UnityEngine;

public class WhileTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private WhileLiftController liftController;
    [SerializeField] private TerminalIndicator terminalIndicator;
    [SerializeField] private string interactionPrompt = "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0437\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u044c \u0446\u0438\u043a\u043b while";
    [SerializeField] private string sectionTitle = "WHILE";
    [SerializeField] private string conditionLabel = "while \u0432\u044b\u0441\u043e\u0442\u0430 < \u0446\u0435\u043b\u044c";
    [SerializeField] private bool createInteractionTrigger = true;
    [SerializeField] private Vector3 triggerLocalPosition = new Vector3(0f, 1.5f, 1.9f);
    [SerializeField] private Vector3 triggerSize = new Vector3(4.2f, 3.4f, 4.2f);

    public string InteractionPrompt => GetInteractionPrompt();

    private void Awake()
    {
        LocalizeDefaults();
        EnsureInteractionTrigger();
        RefreshIdleState();
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (liftController == null)
        {
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Error,
                sectionTitle,
                BuildBodyText(0f, 1f),
                "\u041f\u043e\u0434\u044a\u0451\u043c\u043d\u0438\u043a \u043d\u0435 \u043f\u043e\u0434\u043a\u043b\u044e\u0447\u0435\u043d");
            return;
        }

        if (liftController.IsRunning)
        {
            return;
        }

        if (liftController.IsComplete)
        {
            ResetCycle();
            return;
        }

        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildBodyText(0f, liftController.TargetHeight),
            "\u0417\u0430\u043f\u0443\u0441\u043a \u0446\u0438\u043a\u043b\u0430");

        liftController.RunLift(HandleStep, HandleCompleted);
    }

    private void HandleStep(float currentHeight, float targetHeight)
    {
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildBodyText(currentHeight, targetHeight),
            $"\u0412\u044b\u0441\u043e\u0442\u0430: {currentHeight:0.0} / {targetHeight:0.0}");
    }

    private void HandleCompleted()
    {
        float targetHeight = liftController != null ? liftController.TargetHeight : 0f;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Success,
            sectionTitle,
            BuildBodyText(targetHeight, targetHeight),
            "\u0426\u0435\u043b\u044c \u0434\u043e\u0441\u0442\u0438\u0433\u043d\u0443\u0442\u0430");
    }

    private void RefreshIdleState()
    {
        float currentHeight = liftController != null ? liftController.CurrentHeight : 0f;
        float targetHeight = liftController != null ? liftController.TargetHeight : 0f;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildBodyText(currentHeight, targetHeight),
            currentHeight > 0f
                ? $"\u0422\u0435\u043a\u0443\u0449\u0430\u044f \u0432\u044b\u0441\u043e\u0442\u0430: {currentHeight:0.0}"
                : targetHeight > 0f
                    ? $"\u0426\u0435\u043b\u044c: {targetHeight:0.0}"
                    : "\u041e\u0436\u0438\u0434\u0430\u043d\u0438\u0435");
    }

    private string BuildBodyText(float currentHeight, float targetHeight)
    {
        return $"{conditionLabel}\n{currentHeight:0.0} < {targetHeight:0.0}";
    }

    private void LocalizeDefaults()
    {
        if (string.IsNullOrWhiteSpace(interactionPrompt) || interactionPrompt.Contains("Press E"))
        {
            interactionPrompt = "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0437\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u044c \u0446\u0438\u043a\u043b while";
        }
    }

    private string GetInteractionPrompt()
    {
        if (liftController != null && liftController.IsComplete)
        {
            return "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0441\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0446\u0438\u043a\u043b while";
        }

        return interactionPrompt;
    }

    private void ResetCycle()
    {
        liftController?.ResetLiftInstantly();
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildBodyText(0f, liftController != null ? liftController.TargetHeight : 0f),
            "\u0421\u0431\u0440\u043e\u0441 \u0432\u044b\u043f\u043e\u043b\u043d\u0435\u043d");
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

        triggerZone.SetTarget(this);
    }
}
