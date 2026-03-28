using System.Collections.Generic;
using UnityEngine;

public enum LevelVariableType
{
    BridgeLength = 0,
    PlatformHeight = 1,
    PlatformAngle = 2,
}

public class VariableLevelController : MonoBehaviour
{
    private static readonly int[] BridgeValues = { 1, 2, 3 };
    private static readonly int[] PlatformHeightValues = { 1, 2, 3 };
    private static readonly int[] PlatformAngleValues = { 0, 45, 90 };

    [SerializeField] private BridgeController bridgeController;
    [SerializeField] private HeightPlatformController heightPlatformController;
    [SerializeField] private RotationPlatformController rotationPlatformController;
    [SerializeField] private TerminalUI terminalUI;
    [SerializeField] private ResultPopupUI resultPopupUI;
    [SerializeField] private ScreenFadePlayerLock screenFadePlayerLock;
    [SerializeField] [Range(1, 3)] private int defaultBridgeLength = 1;
    [SerializeField] [Range(1, 3)] private int defaultPlatformHeight = 1;
    [SerializeField] [Range(0, 90)] private int defaultPlatformAngle;

    private int bridgeLength;
    private int platformHeight;
    private int platformAngle;
    private bool levelCompleted;

    public int BridgeLength => bridgeLength;
    public int PlatformHeight => platformHeight;
    public int PlatformAngle => platformAngle;
    public bool IsBridgeCrossable => bridgeLength == 3;
    public bool IsPlatformReachable => platformHeight == 3;
    public bool IsRotationAligned => platformAngle == 90;
    public bool IsExitUnlocked => IsBridgeCrossable && IsPlatformReachable && IsRotationAligned;
    public bool IsLevelCompleted => levelCompleted;

    private void Start()
    {
        bridgeLength = Mathf.Clamp(defaultBridgeLength, 1, 3);
        platformHeight = Mathf.Clamp(defaultPlatformHeight, 1, 3);
        platformAngle = ClampPlatformAngle(defaultPlatformAngle);
        terminalUI?.RefreshActiveDisplay();
    }

    public string GetVariableName(LevelVariableType variableType)
    {
        return variableType switch
        {
            LevelVariableType.BridgeLength => "bridgeLength",
            LevelVariableType.PlatformHeight => "platformHeight",
            LevelVariableType.PlatformAngle => "platformAngle",
            _ => string.Empty,
        };
    }

    public IReadOnlyList<int> GetOptions(LevelVariableType variableType)
    {
        return variableType switch
        {
            LevelVariableType.BridgeLength => BridgeValues,
            LevelVariableType.PlatformHeight => PlatformHeightValues,
            LevelVariableType.PlatformAngle => PlatformAngleValues,
            _ => BridgeValues,
        };
    }

    public int GetCurrentValue(LevelVariableType variableType)
    {
        return variableType switch
        {
            LevelVariableType.BridgeLength => bridgeLength,
            LevelVariableType.PlatformHeight => platformHeight,
            LevelVariableType.PlatformAngle => platformAngle,
            _ => bridgeLength,
        };
    }

    public void SetVariable(LevelVariableType variableType, int value)
    {
        switch (variableType)
        {
            case LevelVariableType.BridgeLength:
                SetBridgeLength(value, false);
                break;
            case LevelVariableType.PlatformHeight:
                SetPlatformHeight(value, false);
                break;
            case LevelVariableType.PlatformAngle:
                SetPlatformAngle(value, false);
                break;
        }
    }

    public void CompleteLevel()
    {
        if (levelCompleted)
        {
            return;
        }

        levelCompleted = true;
        terminalUI?.ForceClose();
        if (LevelCompletionFlowController.TryStartCurrentLevelCompletion())
        {
            return;
        }

        if (screenFadePlayerLock != null)
        {
            screenFadePlayerLock.FadeOutAndLock();
            return;
        }

        resultPopupUI?.Show();
    }

    private void SetBridgeLength(int newValue, bool instant)
    {
        bridgeLength = Mathf.Clamp(newValue, 1, 3);

        if (bridgeController != null)
        {
            if (instant)
            {
                bridgeController.SetBridgeLengthInstant(bridgeLength);
            }
            else
            {
                bridgeController.SetBridgeLength(bridgeLength);
            }
        }

        terminalUI?.RefreshActiveDisplay();
    }

    private void SetPlatformHeight(int newValue, bool instant)
    {
        platformHeight = Mathf.Clamp(newValue, 1, 3);

        if (heightPlatformController != null)
        {
            if (instant)
            {
                heightPlatformController.SetPlatformHeightInstant(platformHeight);
            }
            else
            {
                heightPlatformController.SetPlatformHeight(platformHeight);
            }
        }

        terminalUI?.RefreshActiveDisplay();
    }

    private void SetPlatformAngle(int newValue, bool instant)
    {
        platformAngle = ClampPlatformAngle(newValue);

        if (rotationPlatformController != null)
        {
            if (instant)
            {
                rotationPlatformController.SetPlatformAngleInstant(platformAngle);
            }
            else
            {
                rotationPlatformController.SetPlatformAngle(platformAngle);
            }
        }

        terminalUI?.RefreshActiveDisplay();
    }

    private static int ClampPlatformAngle(int value)
    {
        if (value >= 90)
        {
            return 90;
        }

        if (value >= 45)
        {
            return 45;
        }

        return 0;
    }
}
