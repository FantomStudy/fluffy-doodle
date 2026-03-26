using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice
{
    public sealed class SimpleUIButton : MonoBehaviour
    {
        public enum ButtonAction
        {
            None = 0,
            LoadScene = 1,
            ReloadCurrentScene = 2,
            LoadBootstrap = 3,
            QuitApplication = 4
        }

        [SerializeField]
        private Button button;

        [SerializeField]
        private SceneLoader sceneLoader;

        [SerializeField]
        private ButtonAction buttonAction = ButtonAction.None;

        [SerializeField]
        private string sceneName;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(Execute);
            }
        }

        private void Execute()
        {
            if (sceneLoader == null)
            {
                return;
            }

            switch (buttonAction)
            {
                case ButtonAction.LoadScene:
                    sceneLoader.LoadScene(sceneName);
                    break;
                case ButtonAction.ReloadCurrentScene:
                    sceneLoader.ReloadCurrentScene();
                    break;
                case ButtonAction.LoadBootstrap:
                    sceneLoader.LoadBootstrap();
                    break;
                case ButtonAction.QuitApplication:
                    sceneLoader.QuitApplication();
                    break;
            }
        }
    }
}
