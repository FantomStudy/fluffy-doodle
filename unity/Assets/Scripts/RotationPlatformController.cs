using UnityEngine;

public class RotationPlatformController : MonoBehaviour
{
    [SerializeField] private Transform platformTransform;
    [SerializeField] private float angleForZero = 0f;
    [SerializeField] private float angleForFortyFive = 45f;
    [SerializeField] private float angleForNinety = 90f;
    [SerializeField] private float rotationSpeed = 120f;

    private Vector3 baseEulerAngles;
    private float currentAngle;
    private float targetAngle;

    private void Awake()
    {
        if (platformTransform == null)
        {
            platformTransform = transform;
        }

        baseEulerAngles = platformTransform.localEulerAngles;
        currentAngle = baseEulerAngles.y;
        targetAngle = currentAngle;
    }

    private void Update()
    {
        if (platformTransform == null || Mathf.Approximately(currentAngle, targetAngle))
        {
            return;
        }

        currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        ApplyAngle(currentAngle);
    }

    public void SetPlatformAngle(int platformAngle)
    {
        targetAngle = ResolveAngle(platformAngle);
    }

    public void SetPlatformAngleInstant(int platformAngle)
    {
        targetAngle = ResolveAngle(platformAngle);
        currentAngle = targetAngle;
        ApplyAngle(currentAngle);
    }

    private float ResolveAngle(int platformAngle)
    {
        return platformAngle switch
        {
            >= 90 => angleForNinety,
            >= 45 => angleForFortyFive,
            _ => angleForZero,
        };
    }

    private void ApplyAngle(float angle)
    {
        if (platformTransform == null)
        {
            return;
        }

        Vector3 eulerAngles = baseEulerAngles;
        eulerAngles.y = angle;
        platformTransform.localRotation = Quaternion.Euler(eulerAngles);
    }
}
