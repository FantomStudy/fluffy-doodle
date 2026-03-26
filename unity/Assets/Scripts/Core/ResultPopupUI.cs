using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice
{
    public sealed class ResultPopupUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject rootPanel;

        [SerializeField]
        private Text titleText;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private SceneLoader sceneLoader;

        public bool IsVisible
        {
            get { return rootPanel != null && rootPanel.activeSelf; }
        }

        private void Awake()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(HandleRestart);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(HandleBackToMenu);
            }

            Hide();
        }

        public void Show(bool success, string title, string message)
        {
            if (rootPanel != null)
            {
                rootPanel.SetActive(true);
            }

            if (titleText != null)
            {
                titleText.text = title;
                titleText.color = success ? new Color(0.17f, 0.55f, 0.27f) : new Color(0.72f, 0.18f, 0.18f);
            }

            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        public void Hide()
        {
            if (rootPanel != null)
            {
                rootPanel.SetActive(false);
            }
        }

        private void HandleRestart()
        {
            if (sceneLoader != null)
            {
                sceneLoader.ReloadCurrentScene();
            }
        }

        private void HandleBackToMenu()
        {
            if (sceneLoader != null)
            {
                sceneLoader.LoadBootstrap();
            }
        }
    }
}
