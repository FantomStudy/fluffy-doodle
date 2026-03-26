using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.UI
{
    public sealed class InGameMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Button runButton;

        [SerializeField]
        private Button resetButton;

        [SerializeField]
        private Button backButton;

        public Button RunButton
        {
            get { return runButton; }
        }

        public Button ResetButton
        {
            get { return resetButton; }
        }

        public Button BackButton
        {
            get { return backButton; }
        }

        public void SetButtonsInteractable(bool value)
        {
            if (runButton != null)
            {
                runButton.interactable = value;
            }

            if (resetButton != null)
            {
                resetButton.interactable = value;
            }

            if (backButton != null)
            {
                backButton.interactable = value;
            }
        }
    }
}
