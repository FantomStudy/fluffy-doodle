using UnityEngine;
using PythonPractice;
using PythonPractice.Interactables;
using PythonPractice.Player;
using PythonPractice.UI;

namespace PythonPractice.Levels.Variables
{
    public sealed class VariablesLevelController : LevelBase
    {
        [SerializeField]
        private TopDownPlayerController playerController;

        [SerializeField]
        private CarryItemController carryItemController;

        [SerializeField]
        private DataPickup[] dataPickups;

        [SerializeField]
        private DoorController[] doorsToReset;

        [SerializeField]
        private InGameMenuUI inGameMenuUi;

        private int deliveredCount;

        protected override void Start()
        {
            base.Start();

            if (inGameMenuUi != null)
            {
                if (inGameMenuUi.RunButton != null)
                {
                    inGameMenuUi.RunButton.onClick.RemoveAllListeners();
                    inGameMenuUi.RunButton.onClick.AddListener(CheckProgress);
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
            return !isLevelFinished && deliveredCount >= GetRequiredPickupCount();
        }

        public void HandleCorrectDeposit(DataContainer container, DataPickup pickup)
        {
            deliveredCount++;
            SetStatus("Correct container. Keep sorting the remaining values.");

            if (AreCompletionConditionsMet())
            {
                CompleteLevel("Variables Sorted", "All data objects were delivered to the correct containers.");
            }
        }

        public void HandleWrongDeposit(DataContainer container, DataPickup pickup)
        {
            FailLevel("Wrong Container", "A value was delivered to the wrong data type container.");
        }

        public void ResetLevelSceneState()
        {
            ResetLevelState();
            deliveredCount = 0;

            if (playerController != null)
            {
                playerController.ResetToStart();
            }

            if (carryItemController != null)
            {
                carryItemController.ResetCarryState();
            }

            if (dataPickups != null)
            {
                for (int i = 0; i < dataPickups.Length; i++)
                {
                    if (dataPickups[i] != null)
                    {
                        dataPickups[i].ResetPickup();
                    }
                }
            }

            if (doorsToReset != null)
            {
                for (int i = 0; i < doorsToReset.Length; i++)
                {
                    if (doorsToReset[i] != null)
                    {
                        doorsToReset[i].ResetDoor();
                    }
                }
            }

            SetStatus("Carry one object at a time. Press Space or E to pick up and deliver.");
        }

        private void CheckProgress()
        {
            if (AreCompletionConditionsMet())
            {
                CompleteLevel("Variables Sorted", "All data objects were delivered to the correct containers.");
            }
            else
            {
                SetStatus("Some data objects are still waiting to be sorted.");
            }
        }

        private int GetRequiredPickupCount()
        {
            return dataPickups == null ? 0 : dataPickups.Length;
        }
    }
}
