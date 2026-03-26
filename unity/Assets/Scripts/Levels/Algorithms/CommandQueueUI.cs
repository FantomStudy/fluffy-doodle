using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.Levels.Algorithms
{
    public sealed class CommandQueueUI : MonoBehaviour
    {
        [SerializeField]
        private Text queueText;

        public void Refresh(IList<RobotCommandType> commands)
        {
            if (queueText == null)
            {
                return;
            }

            if (commands == null || commands.Count == 0)
            {
                queueText.text = "Queue is empty.";
                return;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < commands.Count; i++)
            {
                builder.Append(i + 1);
                builder.Append(". ");
                builder.Append(commands[i].ToString());
                if (i < commands.Count - 1)
                {
                    builder.Append('\n');
                }
            }

            queueText.text = builder.ToString();
        }
    }
}
