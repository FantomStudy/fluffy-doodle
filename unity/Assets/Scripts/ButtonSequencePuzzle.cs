using System.Collections.Generic;
using UnityEngine;

public class ButtonSequencePuzzle : MonoBehaviour
{
    [SerializeField] private PressureButton[] buttons;
    [SerializeField] private int[] correctOrder = { 0, 1, 2 };
    [SerializeField] private SequenceDoor targetDoor;
    [SerializeField] private bool resetOnWrongButton = true;

    private readonly List<int> currentOrder = new();
    private bool isSolved;

    private void Start()
    {
        ResetPuzzle();
    }

    public void NotifyButtonPressed(PressureButton button)
    {
        if (button == null || isSolved)
        {
            return;
        }

        currentOrder.Add(button.ButtonIndex);

        int currentStep = currentOrder.Count - 1;
        if (currentStep >= correctOrder.Length || currentOrder[currentStep] != correctOrder[currentStep])
        {
            if (resetOnWrongButton)
            {
                ResetPuzzle();
            }

            return;
        }

        if (currentOrder.Count == correctOrder.Length)
        {
            isSolved = true;
            MarkButtonsSolved();
            if (targetDoor != null)
            {
                targetDoor.Open();
            }
        }
    }

    public void ResetPuzzle()
    {
        currentOrder.Clear();
        isSolved = false;

        if (buttons == null)
        {
            return;
        }

        foreach (PressureButton button in buttons)
        {
            if (button != null)
            {
                button.ResetButton();
            }
        }
    }

    private void MarkButtonsSolved()
    {
        if (buttons == null)
        {
            return;
        }

        foreach (PressureButton button in buttons)
        {
            if (button != null)
            {
                button.MarkSolved();
            }
        }
    }
}
