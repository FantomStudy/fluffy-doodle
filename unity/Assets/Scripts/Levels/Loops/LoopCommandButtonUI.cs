using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.Levels.Loops
{
    public sealed class LoopCommandButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private LoopsLevelController loopsLevelController;

        [SerializeField]
        private LoopCommandType commandType = LoopCommandType.MoveForward;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            if (loopsLevelController != null)
            {
                loopsLevelController.AddCommand(commandType);
            }
        }
    }
}
