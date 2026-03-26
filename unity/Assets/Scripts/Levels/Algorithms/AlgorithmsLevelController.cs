using System.Collections.Generic;
using UnityEngine;
using PythonPractice;
using PythonPractice.Interactables;
using PythonPractice.Player;
using PythonPractice.UI;

namespace PythonPractice.Levels.Algorithms
{
    public sealed class AlgorithmsLevelController : LevelBase
    {
        [SerializeField]
        private GridRobotController gridRobotController;

        [SerializeField]
        private ActivationPoint[] activationPoints;

        [SerializeField]
        private DoorController exitDoor;

        [SerializeField]
        private GoalZone goalZone;

        [SerializeField]
        private HazardZone[] hazardZones;

        [SerializeField]
        private CommandQueueUI commandQueueUi;

        [SerializeField]
        private InGameMenuUI inGameMenuUi;

        [SerializeField]
        private int maxCommands = 8;

        private readonly List<RobotCommandType> commandQueue = new List<RobotCommandType>();
        private int nextActivationIndex = 1;

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
            return !isLevelFinished && AllPointsActivated();
        }

        public void AddCommand(RobotCommandType commandType)
        {
            if (isLevelFinished || gridRobotController == null || gridRobotController.IsRunning)
            {
                return;
            }

            if (commandQueue.Count >= maxCommands)
            {
                SetStatus("Command limit reached.");
                return;
            }

            commandQueue.Add(commandType);
            RefreshQueueUi();
        }

        public void ClearCommands()
        {
            commandQueue.Clear();
            RefreshQueueUi();
        }

        public void RunCommands()
        {
            if (commandQueue.Count == 0 || gridRobotController == null || gridRobotController.IsRunning || isLevelFinished)
            {
                return;
            }

            SetStatus("Robot is executing the sequence.");
            if (inGameMenuUi != null)
            {
                inGameMenuUi.SetButtonsInteractable(false);
            }

            gridRobotController.RunCommands(commandQueue, HandleRobotStepCompleted, HandleRobotRunCompleted, HandleRobotFailed);
        }

        public void ResetLevelSceneState()
        {
            ResetLevelState();
            commandQueue.Clear();
            nextActivationIndex = 1;

            if (gridRobotController != null)
            {
                gridRobotController.ResetRobot();
            }

            if (activationPoints != null)
            {
                for (int i = 0; i < activationPoints.Length; i++)
                {
                    if (activationPoints[i] != null)
                    {
                        activationPoints[i].ResetPoint();
                    }
                }
            }

            if (exitDoor != null)
            {
                exitDoor.ResetDoor();
            }

            if (inGameMenuUi != null)
            {
                inGameMenuUi.SetButtonsInteractable(true);
            }

            SetStatus("Activate points 1 -> 2 -> 3, then reach the goal.");
            RefreshQueueUi();
        }

        private void HandleRobotStepCompleted()
        {
            if (gridRobotController == null || isLevelFinished)
            {
                return;
            }

            if (CheckHazards())
            {
                return;
            }

            CheckActivationPoints();

            if (goalZone != null && goalZone.IsOverlapping(gridRobotController.RobotRect) && AllPointsActivated())
            {
                gridRobotController.StopExecution();
                CompleteLevel("Algorithm Complete", "The robot activated every point in order and reached the goal.");
                return;
            }

            SetStatus(AllPointsActivated() ? "All points active. Reach the goal." : "Follow the activation order.");
        }

        private void HandleRobotRunCompleted()
        {
            if (inGameMenuUi != null)
            {
                inGameMenuUi.SetButtonsInteractable(true);
            }

            if (!isLevelFinished)
            {
                SetStatus("Sequence finished. Adjust the commands and try again.");
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

            FailLevel("Sequence Failed", message);
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

        private void CheckActivationPoints()
        {
            if (activationPoints == null)
            {
                return;
            }

            for (int i = 0; i < activationPoints.Length; i++)
            {
                ActivationPoint point = activationPoints[i];
                if (point == null || point.Activated || !point.IsOverlapping(gridRobotController.RobotRect))
                {
                    continue;
                }

                if (point.OrderIndex != nextActivationIndex)
                {
                    if (gridRobotController != null)
                    {
                        gridRobotController.StopExecution();
                    }

                    FailLevel("Wrong Order", "Activation points must be visited in order 1 -> 2 -> 3.");
                    return;
                }

                point.MarkActivated();
                nextActivationIndex++;

                if (AllPointsActivated() && exitDoor != null)
                {
                    exitDoor.Open();
                }
            }
        }

        private bool AllPointsActivated()
        {
            if (activationPoints == null || activationPoints.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < activationPoints.Length; i++)
            {
                if (activationPoints[i] != null && !activationPoints[i].Activated)
                {
                    return false;
                }
            }

            return true;
        }

        private void RefreshQueueUi()
        {
            if (commandQueueUi != null)
            {
                commandQueueUi.Refresh(commandQueue);
            }
        }
    }
}
