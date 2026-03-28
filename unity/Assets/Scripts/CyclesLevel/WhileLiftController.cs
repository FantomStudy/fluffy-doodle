using System;
using System.Collections;
using UnityEngine;

public class WhileLiftController : MonoBehaviour
{
    [SerializeField] private Transform platformTransform;
    [SerializeField] private float targetHeight = 2.4f;
    [SerializeField] private float stepHeight = 0.8f;
    [SerializeField] private float stepMoveDuration = 0.22f;
    [SerializeField] private float stepDelay = 0.12f;

    private Vector3 startLocalPosition;
    private Coroutine liftRoutine;

    public bool IsRunning { get; private set; }
    public bool IsComplete { get; private set; }
    public float CurrentHeight { get; private set; }
    public float TargetHeight => Mathf.Max(0f, targetHeight);

    private void Awake()
    {
        if (platformTransform == null)
        {
            platformTransform = transform;
        }

        startLocalPosition = platformTransform.localPosition;
        ResetLiftInstantly();
    }

    public void RunLift(Action<float, float> onStep = null, Action onCompleted = null)
    {
        if (IsRunning)
        {
            return;
        }

        if (IsComplete)
        {
            onCompleted?.Invoke();
            return;
        }

        if (liftRoutine != null)
        {
            StopCoroutine(liftRoutine);
        }

        liftRoutine = StartCoroutine(LiftRoutine(onStep, onCompleted));
    }

    public void ResetLiftInstantly()
    {
        if (liftRoutine != null)
        {
            StopCoroutine(liftRoutine);
            liftRoutine = null;
        }

        CurrentHeight = 0f;
        IsRunning = false;
        IsComplete = false;

        if (platformTransform != null)
        {
            platformTransform.localPosition = startLocalPosition;
        }
    }

    private IEnumerator LiftRoutine(Action<float, float> onStep, Action onCompleted)
    {
        IsRunning = true;

        float safeStepHeight = Mathf.Max(0.05f, stepHeight);
        float safeTargetHeight = TargetHeight;
        float safeMoveDuration = Mathf.Max(0.01f, stepMoveDuration);

        while (CurrentHeight + 0.001f < safeTargetHeight)
        {
            float nextHeight = Mathf.Min(CurrentHeight + safeStepHeight, safeTargetHeight);
            Vector3 from = startLocalPosition + Vector3.up * CurrentHeight;
            Vector3 to = startLocalPosition + Vector3.up * nextHeight;
            float elapsed = 0f;

            while (elapsed < safeMoveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / safeMoveDuration);
                platformTransform.localPosition = Vector3.Lerp(from, to, t);
                yield return null;
            }

            platformTransform.localPosition = to;
            CurrentHeight = nextHeight;
            onStep?.Invoke(CurrentHeight, safeTargetHeight);

            if (stepDelay > 0f && CurrentHeight + 0.001f < safeTargetHeight)
            {
                yield return new WaitForSeconds(stepDelay);
            }
        }

        IsRunning = false;
        IsComplete = true;
        liftRoutine = null;
        onCompleted?.Invoke();
    }
}
