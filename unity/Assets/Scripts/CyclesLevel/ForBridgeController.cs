using System;
using System.Collections;
using UnityEngine;

public class ForBridgeController : MonoBehaviour
{
    [SerializeField] private Transform[] bridgeSegments;
    [SerializeField] private float segmentMoveDuration = 0.18f;
    [SerializeField] private float delayBetweenSegments = 0.08f;
    [SerializeField] private Vector3 hiddenLocalOffset = new Vector3(0f, -1.2f, 0f);
    [SerializeField] private bool disableCollidersWhileHidden = true;
    [SerializeField] private Vector3[] extendedLocalPositionsData;

    private Vector3[] extendedLocalPositions;
    private Collider[][] segmentColliders;
    private Coroutine buildRoutine;

    public bool IsRunning { get; private set; }
    public int SegmentCount => bridgeSegments != null ? bridgeSegments.Length : 0;
    public int LastBuiltCount { get; private set; }
    public int CurrentBuiltCount { get; private set; }

    private void Awake()
    {
        CacheSegments();
        ResetBridgeInstantly();
    }

    public void BuildBridge(int count, Action onCompleted = null)
    {
        CacheSegments();

        if (buildRoutine != null)
        {
            StopCoroutine(buildRoutine);
        }

        buildRoutine = StartCoroutine(BuildRoutine(Mathf.Clamp(count, 0, SegmentCount), onCompleted));
    }

    public void ResetBridgeInstantly()
    {
        CacheSegments();

        if (buildRoutine != null)
        {
            StopCoroutine(buildRoutine);
            buildRoutine = null;
        }

        IsRunning = false;
        LastBuiltCount = 0;
        CurrentBuiltCount = 0;

        for (int i = 0; i < SegmentCount; i++)
        {
            SetSegmentHidden(i);
        }
    }

    public void ExtendOneSegment(Action<int> onCompleted = null)
    {
        CacheSegments();

        if (IsRunning)
        {
            return;
        }

        if (CurrentBuiltCount >= SegmentCount)
        {
            onCompleted?.Invoke(CurrentBuiltCount);
            return;
        }

        if (buildRoutine != null)
        {
            StopCoroutine(buildRoutine);
        }

        buildRoutine = StartCoroutine(ExtendOneSegmentRoutine(onCompleted));
    }

    public void CaptureExtendedPositions()
    {
        if (bridgeSegments == null)
        {
            extendedLocalPositionsData = Array.Empty<Vector3>();
            extendedLocalPositions = Array.Empty<Vector3>();
            return;
        }

        extendedLocalPositionsData = new Vector3[bridgeSegments.Length];
        extendedLocalPositions = new Vector3[bridgeSegments.Length];

        for (int i = 0; i < bridgeSegments.Length; i++)
        {
            Transform segment = bridgeSegments[i];
            if (segment == null)
            {
                continue;
            }

            extendedLocalPositionsData[i] = segment.localPosition;
            extendedLocalPositions[i] = segment.localPosition;
        }
    }

    private IEnumerator BuildRoutine(int count, Action onCompleted)
    {
        IsRunning = true;

        for (int i = 0; i < SegmentCount; i++)
        {
            SetSegmentHidden(i);
        }

        yield return null;

        for (int i = 0; i < count; i++)
        {
            yield return AnimateSegment(i);
            CurrentBuiltCount = i + 1;
            LastBuiltCount = CurrentBuiltCount;

            if (delayBetweenSegments > 0f)
            {
                yield return new WaitForSeconds(delayBetweenSegments);
            }
        }

        IsRunning = false;
        buildRoutine = null;
        onCompleted?.Invoke();
    }

    private IEnumerator ExtendOneSegmentRoutine(Action<int> onCompleted)
    {
        IsRunning = true;

        int nextIndex = CurrentBuiltCount;
        yield return AnimateSegment(nextIndex);

        CurrentBuiltCount = nextIndex + 1;
        LastBuiltCount = CurrentBuiltCount;
        IsRunning = false;
        buildRoutine = null;
        onCompleted?.Invoke(CurrentBuiltCount);
    }

    private IEnumerator AnimateSegment(int index)
    {
        Transform segment = GetSegment(index);
        if (segment == null)
        {
            yield break;
        }

        Vector3 hiddenPosition = GetHiddenLocalPosition(index);
        Vector3 targetPosition = extendedLocalPositions[index];
        float duration = Mathf.Max(0.01f, segmentMoveDuration);
        float elapsed = 0f;

        SetSegmentColliders(index, true);
        segment.localPosition = hiddenPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            segment.localPosition = Vector3.Lerp(hiddenPosition, targetPosition, t);
            yield return null;
        }

        segment.localPosition = targetPosition;
    }

    private void CacheSegments()
    {
        if (bridgeSegments == null)
        {
            bridgeSegments = Array.Empty<Transform>();
        }

        if (extendedLocalPositions != null && extendedLocalPositions.Length == bridgeSegments.Length)
        {
            return;
        }

        extendedLocalPositions = new Vector3[bridgeSegments.Length];
        segmentColliders = new Collider[bridgeSegments.Length][];

        for (int i = 0; i < bridgeSegments.Length; i++)
        {
            Transform segment = bridgeSegments[i];
            if (segment == null)
            {
                continue;
            }

            bool hasStoredPosition = extendedLocalPositionsData != null
                && extendedLocalPositionsData.Length == bridgeSegments.Length;

            extendedLocalPositions[i] = hasStoredPosition
                ? extendedLocalPositionsData[i]
                : segment.localPosition;

            segmentColliders[i] = segment.GetComponentsInChildren<Collider>(true);
        }
    }

    private Transform GetSegment(int index)
    {
        if (index < 0 || index >= SegmentCount)
        {
            return null;
        }

        return bridgeSegments[index];
    }

    private Vector3 GetHiddenLocalPosition(int index)
    {
        return extendedLocalPositions[index] + hiddenLocalOffset;
    }

    private void SetSegmentHidden(int index)
    {
        Transform segment = GetSegment(index);
        if (segment == null)
        {
            return;
        }

        segment.localPosition = GetHiddenLocalPosition(index);
        SetSegmentColliders(index, !disableCollidersWhileHidden);
    }

    private void SetSegmentColliders(int index, bool enabled)
    {
        if (segmentColliders == null || index < 0 || index >= segmentColliders.Length)
        {
            return;
        }

        Collider[] colliders = segmentColliders[index];
        if (colliders == null)
        {
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = enabled;
            }
        }
    }
}
