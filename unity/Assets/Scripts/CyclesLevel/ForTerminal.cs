using UnityEngine;

public class ForTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private ForBridgeController bridgeController;
    [SerializeField] private TerminalIndicator terminalIndicator;
    [SerializeField] private string interactionPrompt = "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0432\u044b\u043f\u043e\u043b\u043d\u0438\u0442\u044c \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0443\u044e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u044e for";
    [SerializeField] private string sectionTitle = "FOR";
    [SerializeField] private int[] availableCounts = { 1, 2, 3 };
    [SerializeField] private int requiredCount = 2;
    [SerializeField] private bool createInteractionTrigger = true;
    [SerializeField] private Vector3 triggerLocalPosition = new Vector3(0f, 1.5f, 1.9f);
    [SerializeField] private Vector3 triggerSize = new Vector3(4.2f, 3.4f, 4.2f);

    public string InteractionPrompt => GetInteractionPrompt();

    private void Awake()
    {
        LocalizeDefaults();
        SanitizeCounts();
        EnsureInteractionTrigger();
        RefreshIdleState();
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (bridgeController == null)
        {
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Error,
                sectionTitle,
                "for i in range(?):\n    extend_bridge()",
                "\u041c\u043e\u0441\u0442 \u043d\u0435 \u043f\u043e\u0434\u043a\u043b\u044e\u0447\u0435\u043d");
            return;
        }

        if (bridgeController.IsRunning)
        {
            return;
        }

        if (bridgeController.CurrentBuiltCount >= requiredCount)
        {
            ResetCycle();
            return;
        }

        int nextIteration = bridgeController.CurrentBuiltCount + 1;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildBodyText(),
            $"\u0418\u0442\u0435\u0440\u0430\u0446\u0438\u044f {nextIteration} \u0438\u0437 {requiredCount}");

        bridgeController.ExtendOneSegment(HandleBridgeStepCompleted);
    }

    private void HandleBridgeStepCompleted(int builtCount)
    {
        bool success = builtCount >= requiredCount;
        terminalIndicator?.ApplyState(
            success ? TerminalIndicatorState.Success : TerminalIndicatorState.Idle,
            sectionTitle,
            BuildBodyText(),
            success
                ? $"\u0413\u043e\u0442\u043e\u0432\u043e: {builtCount} \u0438\u0437 {requiredCount}"
                : $"\u041f\u0440\u043e\u0433\u0440\u0435\u0441\u0441: {builtCount} \u0438\u0437 {requiredCount}");
    }

    private void RefreshIdleState()
    {
        int builtCount = bridgeController != null ? bridgeController.CurrentBuiltCount : 0;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildBodyText(),
            builtCount > 0
                ? $"\u041f\u0440\u043e\u0433\u0440\u0435\u0441\u0441: {builtCount} \u0438\u0437 {requiredCount}"
                : $"\u041d\u0443\u0436\u043d\u043e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u0439: {requiredCount}");
    }

    private string BuildBodyText()
    {
        return $"for i in range({requiredCount}):\n    extend_bridge()";
    }

    private void SanitizeCounts()
    {
        requiredCount = Mathf.Max(1, requiredCount);

        if (availableCounts == null || availableCounts.Length == 0)
        {
            availableCounts = new[] { requiredCount };
            return;
        }

        for (int i = 0; i < availableCounts.Length; i++)
        {
            availableCounts[i] = Mathf.Max(1, availableCounts[i]);
        }
    }

    private void LocalizeDefaults()
    {
        if (string.IsNullOrWhiteSpace(interactionPrompt) || interactionPrompt.Contains("Press E"))
        {
            interactionPrompt = "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0432\u044b\u043f\u043e\u043b\u043d\u0438\u0442\u044c \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0443\u044e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u044e for";
        }
    }

    private string GetInteractionPrompt()
    {
        if (bridgeController != null && bridgeController.CurrentBuiltCount >= requiredCount)
        {
            return "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0441\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0446\u0438\u043a\u043b for";
        }

        return interactionPrompt;
    }

    private void ResetCycle()
    {
        bridgeController?.ResetBridgeInstantly();
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildBodyText(),
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
