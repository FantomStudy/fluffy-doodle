using UnityEngine;

public class CombinedCycleTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private ForBridgeController bridgeController;
    [SerializeField] private WhileLiftController liftController;
    [SerializeField] private TerminalIndicator terminalIndicator;
    [SerializeField] private string forPrompt = "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0432\u044b\u043f\u043e\u043b\u043d\u0438\u0442\u044c \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0443\u044e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u044e for";
    [SerializeField] private string whilePrompt = "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0437\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u044c \u0446\u0438\u043a\u043b while";
    [SerializeField] private string resetPrompt = "\u041d\u0430\u0436\u043c\u0438\u0442\u0435 E, \u0447\u0442\u043e\u0431\u044b \u0441\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0441\u0435\u043a\u0446\u0438\u044e";
    [SerializeField] private string sectionTitle = "\u0424\u0418\u041d\u0410\u041b / FOR + WHILE";
    [SerializeField] private string whileConditionLabel = "while lift < finish";
    [SerializeField] private int requiredCount = 3;
    [SerializeField] private bool createInteractionTrigger = true;
    [SerializeField] private Vector3 triggerLocalPosition = new Vector3(0f, 1.5f, 1.9f);
    [SerializeField] private Vector3 triggerSize = new Vector3(4.2f, 3.4f, 4.2f);

    public string InteractionPrompt => GetInteractionPrompt();

    private void Awake()
    {
        requiredCount = Mathf.Max(1, requiredCount);
        EnsureInteractionTrigger();
        RefreshIdleState();
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (bridgeController == null || liftController == null)
        {
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Error,
                sectionTitle,
                "for -> bridge\nwhile -> lift",
                "\u0421\u0435\u043a\u0446\u0438\u044f \u043d\u0435 \u043d\u0430\u0441\u0442\u0440\u043e\u0435\u043d\u0430");
            return;
        }

        if (bridgeController.IsRunning || liftController.IsRunning)
        {
            return;
        }

        if (liftController.IsComplete)
        {
            ResetCycle();
            return;
        }

        if (bridgeController.CurrentBuiltCount < requiredCount)
        {
            int nextIteration = bridgeController.CurrentBuiltCount + 1;
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Running,
                sectionTitle,
                BuildForBodyText(),
                $"\u0418\u0442\u0435\u0440\u0430\u0446\u0438\u044f {nextIteration} \u0438\u0437 {requiredCount}");

            bridgeController.ExtendOneSegment(HandleBridgeStepCompleted);
            return;
        }

        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildWhileBodyText(0f, liftController.TargetHeight),
            "\u0417\u0430\u043f\u0443\u0441\u043a \u0446\u0438\u043a\u043b\u0430 while");

        liftController.RunLift(HandleLiftStep, HandleLiftCompleted);
    }

    private void HandleBridgeStepCompleted(int builtCount)
    {
        if (builtCount >= requiredCount)
        {
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Success,
                sectionTitle,
                BuildForBodyText(),
                "\u041c\u043e\u0441\u0442 \u0433\u043e\u0442\u043e\u0432. \u0417\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u0435 while");
            return;
        }

        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildForBodyText(),
            $"\u041f\u0440\u043e\u0433\u0440\u0435\u0441\u0441: {builtCount} \u0438\u0437 {requiredCount}");
    }

    private void HandleLiftStep(float currentHeight, float targetHeight)
    {
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildWhileBodyText(currentHeight, targetHeight),
            $"\u0412\u044b\u0441\u043e\u0442\u0430: {currentHeight:0.0} / {targetHeight:0.0}");
    }

    private void HandleLiftCompleted()
    {
        float targetHeight = liftController != null ? liftController.TargetHeight : 0f;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Success,
            sectionTitle,
            BuildWhileBodyText(targetHeight, targetHeight),
            "\u0421\u0435\u043a\u0446\u0438\u044f \u0437\u0430\u0432\u0435\u0440\u0448\u0435\u043d\u0430");
    }

    private void RefreshIdleState()
    {
        if (liftController != null && liftController.IsComplete)
        {
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Success,
                sectionTitle,
                BuildWhileBodyText(liftController.TargetHeight, liftController.TargetHeight),
                "\u0421\u0435\u043a\u0446\u0438\u044f \u0437\u0430\u0432\u0435\u0440\u0448\u0435\u043d\u0430");
            return;
        }

        if (bridgeController != null && bridgeController.CurrentBuiltCount >= requiredCount)
        {
            float targetHeight = liftController != null ? liftController.TargetHeight : 0f;
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Idle,
                sectionTitle,
                BuildWhileBodyText(0f, targetHeight),
                "\u041c\u043e\u0441\u0442 \u0433\u043e\u0442\u043e\u0432. \u0417\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u0435 while");
            return;
        }

        int builtCount = bridgeController != null ? bridgeController.CurrentBuiltCount : 0;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildForBodyText(),
            builtCount > 0
                ? $"\u041f\u0440\u043e\u0433\u0440\u0435\u0441\u0441: {builtCount} \u0438\u0437 {requiredCount}"
                : $"\u041d\u0443\u0436\u043d\u043e \u0438\u0442\u0435\u0440\u0430\u0446\u0438\u0439: {requiredCount}");
    }

    private string BuildForBodyText()
    {
        return $"for i in range({requiredCount}):\n    extend_bridge()";
    }

    private string BuildWhileBodyText(float currentHeight, float targetHeight)
    {
        return $"{whileConditionLabel}\n{currentHeight:0.0} < {targetHeight:0.0}";
    }

    private string GetInteractionPrompt()
    {
        if (liftController != null && liftController.IsComplete)
        {
            return resetPrompt;
        }

        if (bridgeController != null && bridgeController.CurrentBuiltCount >= requiredCount)
        {
            return whilePrompt;
        }

        return forPrompt;
    }

    private void ResetCycle()
    {
        bridgeController?.ResetBridgeInstantly();
        liftController?.ResetLiftInstantly();
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildForBodyText(),
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
