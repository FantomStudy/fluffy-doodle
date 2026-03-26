using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.UI
{
    public sealed class LevelHeaderUI : MonoBehaviour
    {
        [SerializeField]
        private Text titleText;

        [SerializeField]
        private Text subtitleText;

        public void SetHeader(string title, string subtitle)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (subtitleText != null)
            {
                subtitleText.text = subtitle;
            }
        }
    }
}
