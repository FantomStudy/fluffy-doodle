using UnityEngine;

namespace PythonPractice
{
    public sealed class LevelResultController : MonoBehaviour
    {
        [SerializeField]
        private ResultPopupUI resultPopupUi;

        [SerializeField]
        private Behaviour[] behavioursToDisable;

        public bool IsShowingResult
        {
            get { return resultPopupUi != null && resultPopupUi.IsVisible; }
        }

        public void ShowSuccess(string title, string message)
        {
            SetGameplayLocked(true);
            if (resultPopupUi != null)
            {
                resultPopupUi.Show(true, title, message);
            }
        }

        public void ShowFailure(string title, string message)
        {
            SetGameplayLocked(true);
            if (resultPopupUi != null)
            {
                resultPopupUi.Show(false, title, message);
            }
        }

        public void HideResult()
        {
            if (resultPopupUi != null)
            {
                resultPopupUi.Hide();
            }
        }

        public void SetGameplayLocked(bool isLocked)
        {
            if (behavioursToDisable == null)
            {
                return;
            }

            for (int i = 0; i < behavioursToDisable.Length; i++)
            {
                if (behavioursToDisable[i] != null)
                {
                    behavioursToDisable[i].enabled = !isLocked;
                }
            }
        }
    }
}
