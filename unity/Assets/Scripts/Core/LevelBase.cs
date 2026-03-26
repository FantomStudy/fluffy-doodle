using UnityEngine;
using PythonPractice.UI;

namespace PythonPractice
{
    public abstract class LevelBase : MonoBehaviour
    {
        [SerializeField]
        private string levelTitle = "Python Practice";

        [SerializeField]
        [TextArea(2, 4)]
        private string objectiveText = "Complete the objective.";

        [SerializeField]
        protected LevelResultController levelResultController;

        [SerializeField]
        private LevelHeaderUI levelHeaderUI;

        [SerializeField]
        private ObjectivePanelUI objectivePanelUI;

        protected bool isLevelFinished;

        public bool IsLevelFinished
        {
            get { return isLevelFinished; }
        }

        protected virtual void Start()
        {
            ApplyStaticUi();
        }

        protected void ApplyStaticUi()
        {
            if (levelHeaderUI != null)
            {
                levelHeaderUI.SetHeader(levelTitle, "Additional Python mini-practice");
            }

            if (objectivePanelUI != null)
            {
                objectivePanelUI.SetObjective(objectiveText);
            }
        }

        protected void SetStatus(string statusText)
        {
            if (objectivePanelUI != null)
            {
                objectivePanelUI.SetStatus(statusText);
            }
        }

        public virtual bool AreCompletionConditionsMet()
        {
            return !isLevelFinished;
        }

        public virtual void ResetLevelState()
        {
            isLevelFinished = false;
            ApplyStaticUi();

            if (levelResultController != null)
            {
                levelResultController.HideResult();
                levelResultController.SetGameplayLocked(false);
            }
        }

        public void CompleteLevel(string title, string message)
        {
            if (isLevelFinished)
            {
                return;
            }

            isLevelFinished = true;
            if (levelResultController != null)
            {
                levelResultController.ShowSuccess(title, message);
            }
        }

        public void FailLevel(string title, string message)
        {
            if (isLevelFinished)
            {
                return;
            }

            isLevelFinished = true;
            if (levelResultController != null)
            {
                levelResultController.ShowFailure(title, message);
            }
        }
    }
}
