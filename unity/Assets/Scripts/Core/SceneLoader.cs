using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace PythonPractice
{
    public sealed class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string bootstrapSceneName = "BootstrapScene";

#if ENABLE_INPUT_SYSTEM
        [SerializeField]
        private InputSystemUIInputModule inputSystemUiInputModule;
#endif

        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            if (inputSystemUiInputModule != null)
            {
                inputSystemUiInputModule.AssignDefaultActions();
            }
#endif
        }

        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadBootstrap()
        {
            LoadScene(bootstrapSceneName);
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}
