using UnityEngine;
using PythonPractice;
using PythonPractice.Interactables;
using PythonPractice.Player;
using PythonPractice.UI;

namespace PythonPractice.Levels.IfLogic
{
    public sealed class IfLevelController : LevelBase
    {
        [SerializeField]
        private TopDownPlayerController playerController;

        [SerializeField]
        private GoalZone goalZone;

        [SerializeField]
        private HazardZone[] hazardZones;

        [SerializeField]
        private DoorController[] doorControllers;

        [SerializeField]
        private PressureButton[] pressureButtons;

        [SerializeField]
        private LaserTrap[] laserTraps;

        [SerializeField]
        private SensorTrigger[] sensorTriggers;

        [SerializeField]
        private KeyPickup[] keyPickups;

        [SerializeField]
        private InGameMenuUI inGameMenuUi;

        private void Update()
        {
            if (isLevelFinished || playerController == null || playerController.PlayerRect == null)
            {
                return;
            }

            if (CheckHazards())
            {
                return;
            }

            if (goalZone != null && goalZone.IsOverlapping(playerController.PlayerRect))
            {
                CompleteLevel("If Logic Cleared", "The player used buttons, sensors and keys to reach the exit.");
            }
        }

        protected override void Start()
        {
            base.Start();

            if (inGameMenuUi != null)
            {
                if (inGameMenuUi.RunButton != null)
                {
                    inGameMenuUi.RunButton.onClick.RemoveAllListeners();
                    inGameMenuUi.RunButton.onClick.AddListener(ShowRuleHint);
                }

                if (inGameMenuUi.ResetButton != null)
                {
                    inGameMenuUi.ResetButton.onClick.RemoveAllListeners();
                    inGameMenuUi.ResetButton.onClick.AddListener(ResetLevelSceneState);
                }
            }

            ResetLevelSceneState();
        }

        public void ResetLevelSceneState()
        {
            ResetLevelState();

            if (playerController != null)
            {
                playerController.ResetToStart();
            }

            ResetDoors();
            ResetButtons();
            ResetLasers();
            ResetSensors();
            ResetKeys();

            SetStatus("Observe the rule chain: if a button is pressed, a door opens; if a sensor triggers, the trap changes.");
        }

        private void ShowRuleHint()
        {
            if (!isLevelFinished)
            {
                SetStatus("Follow the chain in order: press the button, take the key, trigger the sensor, then cross the safe path.");
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
                if (hazardZones[i] != null && hazardZones[i].IsOverlapping(playerController.PlayerRect))
                {
                    FailLevel(hazardZones[i].FailTitle, hazardZones[i].FailMessage);
                    return true;
                }
            }

            return false;
        }

        private void ResetDoors()
        {
            if (doorControllers == null)
            {
                return;
            }

            for (int i = 0; i < doorControllers.Length; i++)
            {
                if (doorControllers[i] != null)
                {
                    doorControllers[i].ResetDoor();
                }
            }
        }

        private void ResetButtons()
        {
            if (pressureButtons == null)
            {
                return;
            }

            for (int i = 0; i < pressureButtons.Length; i++)
            {
                if (pressureButtons[i] != null)
                {
                    pressureButtons[i].ResetButton();
                }
            }
        }

        private void ResetLasers()
        {
            if (laserTraps == null)
            {
                return;
            }

            for (int i = 0; i < laserTraps.Length; i++)
            {
                if (laserTraps[i] != null)
                {
                    laserTraps[i].ResetTrap();
                }
            }
        }

        private void ResetSensors()
        {
            if (sensorTriggers == null)
            {
                return;
            }

            for (int i = 0; i < sensorTriggers.Length; i++)
            {
                if (sensorTriggers[i] != null)
                {
                    sensorTriggers[i].ResetSensor();
                }
            }
        }

        private void ResetKeys()
        {
            if (keyPickups == null)
            {
                return;
            }

            for (int i = 0; i < keyPickups.Length; i++)
            {
                if (keyPickups[i] != null)
                {
                    keyPickups[i].ResetKey();
                }
            }
        }
    }
}
