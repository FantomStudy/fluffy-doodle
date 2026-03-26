using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.UI
{
    public sealed class ObjectivePanelUI : MonoBehaviour
    {
        [SerializeField]
        private Text objectiveText;

        [SerializeField]
        private Text statusText;

        public void SetObjective(string objective)
        {
            if (objectiveText != null)
            {
                objectiveText.text = objective;
            }
        }

        public void SetStatus(string status)
        {
            if (statusText != null)
            {
                statusText.text = status;
            }
        }
    }
}
