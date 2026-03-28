using UnityEngine;

public class ForTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private ForBridgeController bridgeController;
    [SerializeField] private TerminalIndicator terminalIndicator;
    [SerializeField] private string interactionPrompt = "Нажмите E, чтобы выполнить следующую итерацию for";
    [SerializeField] private string sectionTitle = "FOR";
    [SerializeField] private int[] availableCounts = { 1, 2, 3 };
    [SerializeField] private int requiredCount = 2;
    [SerializeField] private bool createInteractionTrigger = true;
    [SerializeField] private Vector3 triggerLocalPosition = new Vector3(0f, 1.5f, 1.9f);
    [SerializeField] private Vector3 triggerSize = new Vector3(4.2f, 3.4f, 4.2f);

    public string InteractionPrompt => interactionPrompt;

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
                "Мост не подключен");
            return;
        }

        if (bridgeController.IsRunning)
        {
            return;
        }

        if (bridgeController.CurrentBuiltCount >= requiredCount)
        {
            terminalIndicator?.ApplyState(
                TerminalIndicatorState.Success,
                sectionTitle,
                BuildBodyText(),
                $"Цикл завершён: {requiredCount} из {requiredCount}");
            return;
        }

        int nextIteration = bridgeController.CurrentBuiltCount + 1;
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Running,
            sectionTitle,
            BuildBodyText(),
            $"Итерация {nextIteration} из {requiredCount}");

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
                ? $"Готово: {builtCount} из {requiredCount}"
                : $"Прогресс: {builtCount} из {requiredCount}");
    }

    private void RefreshIdleState()
    {
        terminalIndicator?.ApplyState(
            TerminalIndicatorState.Idle,
            sectionTitle,
            BuildBodyText(),
            $"Нужно итераций: {requiredCount}");
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
        if (interactionPrompt == "Press E to advance the for loop"
            || interactionPrompt.Contains("переключить число")
            || interactionPrompt.Contains("выбрать длину")
            || interactionPrompt.Contains("выдвинуть финальный мост"))
        {
            interactionPrompt = "Нажмите E, чтобы выполнить следующую итерацию for";
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
}
