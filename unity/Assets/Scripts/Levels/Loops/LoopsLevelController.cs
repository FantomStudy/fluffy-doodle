using System.Collections.Generic;
using UnityEngine;
using PythonPractice;
using PythonPractice.Interactables;
using PythonPractice.Levels.Algorithms;
using PythonPractice.Player;
using PythonPractice.UI;

namespace PythonPractice.Levels.Loops
{
    public sealed class LoopsLevelController : LevelBase
    {
        [SerializeField]
        private GridRobotController gridRobotController;

        [SerializeField]
        private CrystalPickup[] crystalPickups;

        [SerializeField]
        private HazardZone[] hazardZones;

        [SerializeField]
        private GoalZone goalZone;

        [SerializeField]
        private LoopSequenceUI loopSequenceUi;

        [SerializeField]
        private InGameMenuUI inGameMenuUi;

        private readonly List<LoopCommandType> loopCommands = new List<LoopCommandType>();

        protected override void Start()
        {
            base.Start();

            if (inGameMenuUi != null)
            {
                if (inGameMenuUi.RunButton != null)
                {
                    inGameMenuUi.RunButton.onClick.RemoveAllListeners();
                    inGameMenuUi.RunButton.onClick.AddListener(RunCommands);
                }

                if (inGameMenuUi.ResetButton != null)
                {
                    inGameMenuUi.ResetButton.onClick.RemoveAllListeners();
                    inGameMenuUi.ResetButton.onClick.AddListener(ResetLevelSceneState);
                }
            }

            ResetLevelSceneState();
        }

        public override bool AreCompletionConditionsMet()
        {
            return !isLevelFinished && AreAllCrystalsCollected();
        }

        public void AddCommand(LoopCommandType commandType)
        {
            if (isLevelFinished || gridRobotController == null || gridRobotController.IsRunning)
            {
                return;
            }

            loopCommands.Add(commandType);
            RefreshSequenceUi();
        }

        public void RunCommands()
        {
            if (isLevelFinished || loopCommands.Count == 0 || gridRobotController == null || gridRobotController.IsRunning)
            {
                return;
            }

            List<RobotCommandType> expandedCommands = BuildExpandedCommandList();
            if (expandedCommands == null || expandedCommands.Count == 0)
            {
                FailLevel("Loop Error", "Repeat blocks must be followed by a movement or turn command.");
                return;
            }

            if (inGameMenuUi != null)
            {
                inGameMenuUi.SetButtonsInteractable(false);
            }

            SetStatus("The robot is running the loop program.");
            gridRobotController.RunCommands(expandedCommands, HandleRobotStepCompleted, HandleRobotRunCompleted, HandleRobotFailed);
        }

        public void ResetLevelSceneState()
        {
            ResetLevelState();
            loopCommands.Clear();

            if (gridRobotController != null)
            {
                gridRobotController.ResetRobot();
            }

            if (crystalPickups != null)
            {
                for (int i = 0; i < crystalPickups.Length; i++)
                {
                    if (crystalPickups[i] != null)
                    {
                        crystalPickups[i].ResetCrystal();
                    }
                }
            }

            if (inGameMenuUi != null)
            {
                inGameMenuUi.SetButtonsInteractable(true);
            }

            SetStatus("Use repeat blocks to make the route shorter.");
            RefreshSequenceUi();
        }

        private void HandleRobotStepCompleted()
        {
            if (CheckHazards())
            {
                return;
            }

            CollectCrystals();

            if (goalZone != null && goalZone.IsOverlapping(gridRobotController.RobotRect) && AreAllCrystalsCollected())
            {
                gridRobotController.StopExecution();
                CompleteLevel("Loop Complete", "The robot collected every crystal and reached the finish.");
                return;
            }

            SetStatus(AreAllCrystalsCollected() ? "All crystals collected. Reach the finish." : "Keep collecting the remaining crystals.");
        }

        private void HandleRobotRunCompleted()
        {
            if (inGameMenuUi != null)
            {
                inGameMenuUi.SetButtonsInteractable(true);
            }

            if (!isLevelFinished)
            {
                SetStatus("Program finished. Compact the loop or adjust the route.");
            }
        }

        private void HandleRobotFailed(string message)
        {
            if (gridRobotController != null)
            {
                gridRobotController.StopExecution();
            }

            if (inGameMenuUi != null)
            {
                inGameMenuUi.SetButtonsInteractable(true);
            }

            FailLevel("Loop Failed", message);
        }

        private List<RobotCommandType> BuildExpandedCommandList()
        {
            List<RobotCommandType> expanded = new List<RobotCommandType>();

            for (int i = 0; i < loopCommands.Count; i++)
            {
                LoopCommandType current = loopCommands[i];
                int repeatCount = GetRepeatCount(current);
                if (repeatCount > 0)
                {
                    if (i >= loopCommands.Count - 1 || GetRepeatCount(loopCommands[i + 1]) > 0)
                    {
                        return null;
                    }

                    RobotCommandType repeatedCommand = ConvertToRobotCommand(loopCommands[i + 1]);
                    for (int repeatIndex = 0; repeatIndex < repeatCount; repeatIndex++)
                    {
                        expanded.Add(repeatedCommand);
                    }

                    i++;
                    continue;
                }

                expanded.Add(ConvertToRobotCommand(current));
            }

            return expanded;
        }

        private void CollectCrystals()
        {
            if (crystalPickups == null)
            {
                return;
            }

            for (int i = 0; i < crystalPickups.Length; i++)
            {
                if (crystalPickups[i] != null)
                {
                    crystalPickups[i].TryCollect(gridRobotController.RobotRect);
                }
            }
        }

        private bool CheckHazards()
        {
            if (hazardZones == null)
            {
                return false;
            }

            for (int i = 0; i < hazardZones.Length; i++)
            {
                if (hazardZones[i] != null && hazardZones[i].IsOverlapping(gridRobotController.RobotRect))
                {
                    FailLevel(hazardZones[i].FailTitle, hazardZones[i].FailMessage);
                    return true;
                }
            }

            return false;
        }

        private bool AreAllCrystalsCollected()
        {
            if (crystalPickups == null || crystalPickups.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < crystalPickups.Length; i++)
            {
                if (crystalPickups[i] != null && !crystalPickups[i].IsCollected)
                {
                    return false;
                }
            }

            return true;
        }

        private void RefreshSequenceUi()
        {
            if (loopSequenceUi != null)
            {
                loopSequenceUi.Refresh(loopCommands);
            }
        }

        private static RobotCommandType ConvertToRobotCommand(LoopCommandType commandType)
        {
            switch (commandType)
            {
                case LoopCommandType.TurnLeft:
                    return RobotCommandType.TurnLeft;
                case LoopCommandType.TurnRight:
                    return RobotCommandType.TurnRight;
                default:
                    return RobotCommandType.MoveForward;
            }
        }

        private static int GetRepeatCount(LoopCommandType commandType)
        {
            switch (commandType)
            {
                case LoopCommandType.Repeat2:
                    return 2;
                case LoopCommandType.Repeat3:
                    return 3;
                case LoopCommandType.Repeat4:
                    return 4;
                default:
                    return 0;
            }
        }
    }
}
