using UnityEngine;

public class WhileTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private WhileLiftController liftController;
    [SerializeField] private TerminalIndicator terminalIndicator;
    [SerializeField] private string interactionPrompt = "Нажмите E, чтобы запустить цикл while";
    [SerializeField] private string sectionTitle = "WHILE";
    [SerializeField] private string conditionLabel = "while высота < цель";
    [SerializeField] private bool createInteractionTrigger = true;
    [SerializeField] private Vector3 triggerLocalPosition = new Vector3(0f, 1.5f, 1.9f);
    [SerializeField] private Vector3 triggerSize = new Vector3(4.2f, 3.4f, 4.2f);

    public string InteractionPrompt => interactionPrompt;

    private void Awake()
    {
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
                "Подъёмник не подключен");
            return;
        }

        if (liftController.IsRunning)
        {
            return;
        }

        if (liftController.IsComplete)
        {
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Success,
                sectionTitle,
                BuildBodyText(liftController.TargetHeight, liftController.TargetHeight),
                "Условие уже ложное");
            return;
        }

        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildBodyText(0f, liftController.TargetHeight),
            "Запуск цикла");

        liftController.RunLift(HandleStep, HandleCompleted);
    }

    private void HandleStep(float currentHeight, float targetHeight)
    {
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildBodyText(currentHeight, targetHeight),
            $"Высота: {currentHeight:0.0} / {targetHeight:0.0}");
    }

    private void HandleCompleted()
    {
        float targetHeight = liftController != null ? liftController.TargetHeight : 0f;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Success,
            sectionTitle,
            BuildBodyText(targetHeight, targetHeight),
            "Цель достигнута");
    }

    private void RefreshIdleState()
    {
        float targetHeight = liftController != null ? liftController.TargetHeight : 0f;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildBodyText(0f, targetHeight),
            targetHeight > 0f
                ? $"Цель: {targetHeight:0.0}"
                : "Ожидание");
    }

    private string BuildBodyText(float currentHeight, float targetHeight)
    {
        return $"{conditionLabel}\n{currentHeight:0.0} < {targetHeight:0.0}";
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
}
