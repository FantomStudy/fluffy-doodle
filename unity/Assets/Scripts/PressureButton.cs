using UnityEngine;

public class PressureButton : MonoBehaviour
{
    private static readonly Color DefaultLightColor = Color.red;
    private static readonly Color SolvedLightColor = Color.green;

    [SerializeField] private ButtonSequencePuzzle puzzle;
    [SerializeField] private int buttonIndex;
    [SerializeField] private Light controlledLight;
    [SerializeField] private Transform pressVisual;
    [SerializeField] private BoxCollider activationCollider;
    [SerializeField] private float pressedDepth = 0.03f;
    [SerializeField] private float visualMoveSpeed = 8f;
    [SerializeField] private float playerHeightTolerance = 1.6f;
    [SerializeField] private GameObject namedPlayer;

    private Transform playerRoot;
    private Collider playerCollider;
    private Vector3 releasedLocalPosition;
    private Vector3 pressedLocalPosition;
    private bool isPressed;
    private bool wasPlayerOnButton;

    public int ButtonIndex => buttonIndex;
    public bool IsPressed => isPressed;

    private void Awake()
    {
        if (pressVisual == null)
        {
            pressVisual = transform;
        }

        if (activationCollider == null)
        {
            activationCollider = GetComponentInChildren<BoxCollider>();
        }

        releasedLocalPosition = pressVisual.localPosition;
        pressedLocalPosition = releasedLocalPosition + Vector3.down * pressedDepth;

        SetVisualState(false, true);
        SetLightState(false);
        SetLightColor(DefaultLightColor);
        TryFindPlayer();
    }

    private void Update()
    {
        if (playerRoot == null)
        {
            TryFindPlayer();
        }

        bool isPlayerOnButton = IsPlayerStandingOnButton();
        if (isPlayerOnButton && !wasPlayerOnButton && !isPressed)
        {
            Press();
        }

        wasPlayerOnButton = isPlayerOnButton;

        Vector3 targetLocalPosition = isPressed ? pressedLocalPosition : releasedLocalPosition;
        pressVisual.localPosition = Vector3.Lerp(
            pressVisual.localPosition,
            targetLocalPosition,
            Time.deltaTime * visualMoveSpeed);
    }

    public void ResetButton()
    {
        isPressed = false;
        SetLightState(false);
        SetLightColor(DefaultLightColor);
        SetVisualState(false, false);
    }

    public void MarkSolved()
    {
        isPressed = true;
        SetLightState(true);
        SetLightColor(SolvedLightColor);
        SetVisualState(true, false);
    }

    private void Press()
    {
        isPressed = true;
        SetLightState(true);
        SetVisualState(true, false);

        if (puzzle != null)
        {
            puzzle.NotifyButtonPressed(this);
        }
    }

    private void SetVisualState(bool pressed, bool snap)
    {
        if (pressVisual == null)
        {
            return;
        }

        Vector3 targetLocalPosition = pressed ? pressedLocalPosition : releasedLocalPosition;
        if (snap)
        {
            pressVisual.localPosition = targetLocalPosition;
        }
    }

    private void SetLightState(bool enabledState)
    {
        if (controlledLight != null)
        {
            controlledLight.enabled = enabledState;
        }
    }

    private void SetLightColor(Color lightColor)
    {
        if (controlledLight != null)
        {
            controlledLight.color = lightColor;
        }
    }

    private bool IsPlayerStandingOnButton()
    {
        if (activationCollider == null || playerRoot == null)
        {
            return false;
        }

        if (playerCollider == null)
        {
            playerCollider = playerRoot.GetComponentInChildren<Collider>();
            if (playerCollider == null)
            {
                return false;
            }
        }

        Bounds buttonBounds = activationCollider.bounds;
        Bounds playerBounds = playerCollider.bounds;
        buttonBounds.Expand(new Vector3(0.05f, playerHeightTolerance, 0.05f));

        return buttonBounds.Intersects(playerBounds);
    }

    private void TryFindPlayer()
    {
        if (namedPlayer != null)
        {
            playerRoot = namedPlayer.transform;
            playerCollider = namedPlayer.GetComponentInChildren<CharacterController>();
            if (playerCollider == null)
            {
                playerCollider = namedPlayer.GetComponentInChildren<Collider>();
            }

            if (playerCollider != null)
            {
                playerRoot = playerCollider.transform;
            }

            return;
        }

        CharacterController characterController = FindFirstObjectByType<CharacterController>();
        if (characterController != null)
        {
            playerRoot = characterController.transform;
            playerCollider = characterController;
        }
    }
}
