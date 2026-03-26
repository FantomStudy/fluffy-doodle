using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.Levels.Algorithms
{
    public sealed class CommandButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private AlgorithmsLevelController levelController;

        [SerializeField]
        private RobotCommandType commandType = RobotCommandType.MoveForward;

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
            if (levelController != null)
            {
                levelController.AddCommand(commandType);
            }
        }
    }
}
