using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.Levels.Loops
{
    public sealed class LoopSequenceUI : MonoBehaviour
    {
        [SerializeField]
        private Text sequenceText;

        public void Refresh(IList<LoopCommandType> commands)
        {
            if (sequenceText == null)
            {
                return;
            }

            if (commands == null || commands.Count == 0)
            {
                sequenceText.text = "Program is empty.";
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

            sequenceText.text = builder.ToString();
        }
    }
}
