using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] private Transform bridgeVisual;
    [SerializeField] private float lengthForOne = 1f;
    [SerializeField] private float lengthForTwo = 2f;
    [SerializeField] private float lengthForThree = 3.45f;
    [SerializeField] private float animationSpeed = 4f;

    private float currentLength;
    private float targetLength;
    private Vector3 bridgeStartLocalPosition;
    private float bridgeStartLength;

    private void Awake()
    {
        if (bridgeVisual == null)
        {
            return;
        }

        bridgeStartLocalPosition = bridgeVisual.localPosition;
        bridgeStartLength = Mathf.Max(0.001f, bridgeVisual.localScale.z);
        currentLength = bridgeStartLength;
        targetLength = currentLength;
    }

    private void Update()
    {
        if (bridgeVisual == null || Mathf.Approximately(currentLength, targetLength))
        {
            return;
        }

        currentLength = Mathf.MoveTowards(currentLength, targetLength, animationSpeed * Time.deltaTime);
        ApplyLength(currentLength);
    }

    public void SetBridgeLength(int bridgeLength)
    {
        targetLength = ResolveLength(bridgeLength);
    }

    public void SetBridgeLengthInstant(int bridgeLength)
    {
        targetLength = ResolveLength(bridgeLength);
        currentLength = targetLength;
        ApplyLength(currentLength);
    }

    private float ResolveLength(int bridgeLength)
    {
        return bridgeLength switch
        {
            1 => lengthForOne,
            2 => lengthForTwo,
            _ => lengthForThree,
        };
    }

    private void ApplyLength(float length)
    {
        if (bridgeVisual == null)
        {
            return;
        }

        Vector3 scale = bridgeVisual.localScale;
        scale.z = length;
        bridgeVisual.localScale = scale;

        Vector3 localPosition = bridgeStartLocalPosition;
        localPosition.z += (length - bridgeStartLength) * 0.5f;
        bridgeVisual.localPosition = localPosition;
    }
}
