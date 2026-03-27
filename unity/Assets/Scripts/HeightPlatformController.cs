using UnityEngine;

public class HeightPlatformController : MonoBehaviour
{
    [SerializeField] private Transform platformTransform;
    [SerializeField] private float heightForOne = 0.15f;
    [SerializeField] private float heightForTwo = 0.45f;
    [SerializeField] private float heightForThree = 1f;
    [SerializeField] private float moveSpeed = 2.5f;

    private Vector3 baseLocalPosition;
    private float currentHeight;
    private float targetHeight;

    private void Awake()
    {
        if (platformTransform == null)
        {
            platformTransform = transform;
        }

        baseLocalPosition = platformTransform.localPosition;
        currentHeight = ResolveHeight(1);
        targetHeight = currentHeight;
        ApplyHeight(currentHeight);
    }

    private void Update()
    {
        if (platformTransform == null || Mathf.Approximately(currentHeight, targetHeight))
        {
            return;
        }

        currentHeight = Mathf.MoveTowards(currentHeight, targetHeight, moveSpeed * Time.deltaTime);
        ApplyHeight(currentHeight);
    }

    public void SetPlatformHeight(int platformHeight)
    {
        targetHeight = ResolveHeight(platformHeight);
    }

    public void SetPlatformHeightInstant(int platformHeight)
    {
        targetHeight = ResolveHeight(platformHeight);
        currentHeight = targetHeight;
        ApplyHeight(currentHeight);
    }

    private float ResolveHeight(int platformHeight)
    {
        return platformHeight switch
        {
            1 => heightForOne,
            2 => heightForTwo,
            _ => heightForThree,
        };
    }

    private void ApplyHeight(float height)
    {
        if (platformTransform == null)
        {
            return;
        }

        Vector3 localPosition = baseLocalPosition;
        localPosition.y = height;
        platformTransform.localPosition = localPosition;
    }
}
