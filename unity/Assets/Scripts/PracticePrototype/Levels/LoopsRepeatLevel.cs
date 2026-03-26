using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    internal sealed class LoopsRepeatLevel : PracticeLevelBase
    {
        private readonly List<LoopBlock> _blocks = new();
        private readonly Vector2Int[] _crystals = { new(1, 1), new(2, 1), new(3, 1) };
        private readonly HashSet<int> _collected = new();
        private GridBoardView _board;
        private Text _sequenceText;
        private Vector2Int _robotPosition;
        private RobotDirection _direction;
        private Coroutine _runRoutine;

        public override PracticeLevelId LevelId => PracticeLevelId.Level4;
        public override string Title => "Level 4 - Loops";
        public override string Description => "Use repeat blocks so a repeated route is solved more compactly than writing every move manually.";

        public override void Build(PracticeRuntimeContext context)
        {
            base.Build(context);

            var layout = PracticeUiFactory.AddVerticalLayout(Root.gameObject, 10f, TextAnchor.UpperCenter, true, true, true, false);
            layout.padding = new RectOffset(0, 0, 0, 0);

            var title = CreateSectionTitle(Root, "Collect the 3 crystals, then reach the goal using as few blocks as possible.");
            PracticeUiFactory.AddLayoutElement(title.gameObject, preferredHeight: 34f);

            var tip = CreateBodyText(Root, "Hint: one repeat block is more compact than many single forward steps.");
            PracticeUiFactory.AddLayoutElement(tip.gameObject, preferredHeight: 44f);

            var boardHost = PracticeUiFactory.CreateUIObject("BoardHost", Root);
            PracticeUiFactory.AddLayoutElement(boardHost, preferredHeight: 420f);
            var boardLayout = PracticeUiFactory.AddHorizontalLayout(boardHost, 0f, TextAnchor.MiddleCenter, false, true, false, false);
            boardLayout.padding = new RectOffset(0, 0, 8, 8);
            _board = new GridBoardView(boardHost.transform, Context.WhiteSprite, Context.DefaultFont, 6, 4, new Vector2(90f, 90f));

            BuildControlPanel();
            ResetLevel();
        }

        public override void ResetLevel()
        {
            StopRunRoutine();
            IsBusy = false;
            _blocks.Clear();
            _collected.Clear();
            _robotPosition = new Vector2Int(0, 1);
            _direction = RobotDirection.Right;
            UpdateSequenceText();
            UpdateBoard();
            Context.UI.SetStatus("Build a short repeat-based solution, then run it.", PracticeUiFactory.TextSecondary);
        }

        public override void RunLevel()
        {
            if (IsBusy)
            {
                return;
            }

            if (_blocks.Count == 0)
            {
                Context.UI.SetStatus("Add at least one block.", PracticeUiFactory.Warning);
                return;
            }

            _runRoutine = Context.Bootstrap.StartCoroutine(PlayBlocks());
        }

        private void BuildControlPanel()
        {
            var controlRoot = Context.UI.ControlArea;

            var heading = CreateSectionTitle(controlRoot, "Blocks");
            PracticeUiFactory.AddLayoutElement(heading.gameObject, preferredHeight: 30f);

            var palette = PracticeUiFactory.CreateUIObject("Palette", controlRoot);
            PracticeUiFactory.AddLayoutElement(palette, preferredHeight: 220f);
            var layout = PracticeUiFactory.AddVerticalLayout(palette, 8f, TextAnchor.UpperLeft, true, true, true, false);
            layout.padding = new RectOffset(0, 0, 0, 0);

            AddBlockButton(palette.transform, "Forward", CommandType.Forward, 1);
            AddBlockButton(palette.transform, "Repeat 2x Forward", CommandType.Forward, 2);
            AddBlockButton(palette.transform, "Repeat 3x Forward", CommandType.Forward, 3);
            AddBlockButton(palette.transform, "Turn Right", CommandType.TurnRight, 1);
            AddBlockButton(palette.transform, "Turn Left", CommandType.TurnLeft, 1);

            var sequenceHeader = CreateSectionTitle(controlRoot, "Program");
            PracticeUiFactory.AddLayoutElement(sequenceHeader.gameObject, preferredHeight: 30f);

            _sequenceText = CreateBodyText(controlRoot, string.Empty);
            PracticeUiFactory.AddLayoutElement(_sequenceText.gameObject, preferredHeight: 150f, flexibleHeight: 1f);

            var clearButton = PracticeUiFactory.CreateButton("ClearBlocks", controlRoot, Context.WhiteSprite, Context.DefaultFont, "Clear Blocks", PracticeUiFactory.Warning, Color.white, () =>
            {
                if (IsBusy)
                {
                    return;
                }

                _blocks.Clear();
                UpdateSequenceText();
            });

            PracticeUiFactory.AddLayoutElement(clearButton.gameObject, preferredHeight: 44f);
        }

        private void AddBlockButton(Transform parent, string label, CommandType command, int repeatCount)
        {
            var button = PracticeUiFactory.CreateButton(label + "Button", parent, Context.WhiteSprite, Context.DefaultFont, label, PracticeUiFactory.Accent, Color.white, () =>
            {
                if (IsBusy)
                {
                    return;
                }

                if (_blocks.Count >= 5)
                {
                    Context.UI.SetStatus("Keep the loop solution compact: max 5 blocks.", PracticeUiFactory.Warning);
                    return;
                }

                _blocks.Add(new LoopBlock(command, repeatCount));
                UpdateSequenceText();
            });

            PracticeUiFactory.AddLayoutElement(button.gameObject, preferredHeight: 38f);
        }

        private IEnumerator PlayBlocks()
        {
            IsBusy = true;
            Context.UI.SetStatus("Running repeat blocks.", PracticeUiFactory.Accent);

            foreach (var block in _blocks)
            {
                for (var i = 0; i < block.RepeatCount; i++)
                {
                    var failed = ApplyBlockCommand(block.Command, out var failureMessage);
                    UpdateBoard();

                    if (failed)
                    {
                        IsBusy = false;
                        Context.UI.SetStatus(failureMessage, PracticeUiFactory.Error);
                        Context.ShowResult(new PracticeResultData(false, "Loop Failed", failureMessage));
                        yield break;
                    }

                    CollectCrystalAtCurrentPosition();
                    UpdateBoard();

                        if (_robotPosition == new Vector2Int(3, 2) && _collected.Count == _crystals.Length)
                    {
                        IsBusy = false;
                        Context.UI.SetStatus("Loop solution succeeded.", PracticeUiFactory.Success);
                        Context.ShowResult(new PracticeResultData(true, "Success", "The repeat blocks collected every crystal and reached the goal."));
                        yield break;
                    }

                    yield return new WaitForSeconds(0.34f);
                }
            }

            IsBusy = false;
            Context.UI.SetStatus("Program ended before completing the route.", PracticeUiFactory.Warning);
            Context.ShowResult(new PracticeResultData(false, "Incomplete", "The program finished, but not all crystals were collected or the goal was missed."));
        }

        private bool ApplyBlockCommand(CommandType command, out string failureMessage)
        {
            failureMessage = string.Empty;

            if (command == CommandType.TurnLeft)
            {
                _direction = (RobotDirection)(((int)_direction + 3) % 4);
                return false;
            }

            if (command == CommandType.TurnRight)
            {
                _direction = (RobotDirection)(((int)_direction + 1) % 4);
                return false;
            }

            var target = _robotPosition + DirectionToVector(_direction);
            if (target.x < 0 || target.x > 5 || target.y < 0 || target.y > 3)
            {
                failureMessage = "The robot moved out of the allowed route.";
                return true;
            }

            _robotPosition = target;
            return false;
        }

        private void CollectCrystalAtCurrentPosition()
        {
            for (var i = 0; i < _crystals.Length; i++)
            {
                if (_crystals[i] == _robotPosition)
                {
                    _collected.Add(i);
                }
            }
        }

        private void UpdateSequenceText()
        {
            if (_blocks.Count == 0)
            {
                _sequenceText.text = "No blocks yet.\nSuggested compact idea:\n1. Repeat 3x Forward\n2. Turn Left\n3. Forward";
                return;
            }

            var lines = new List<string>();
            for (var i = 0; i < _blocks.Count; i++)
            {
                lines.Add($"{i + 1}. {_blocks[i]}");
            }

            _sequenceText.text = string.Join("\n", lines);
        }

        private void UpdateBoard()
        {
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 6; x++)
                {
                    var color = new Color32(245, 248, 250, 255);
                    var label = string.Empty;
                    var labelColor = PracticeUiFactory.TextPrimary;

                    if (x == 3 && y == 2)
                    {
                        color = new Color32(220, 245, 226, 255);
                        label = "G";
                        labelColor = PracticeUiFactory.Success;
                    }

                    for (var i = 0; i < _crystals.Length; i++)
                    {
                        if (_crystals[i].x == x && _crystals[i].y == y && !_collected.Contains(i))
                        {
                            color = new Color32(240, 236, 187, 255);
                            label = "*";
                            labelColor = PracticeUiFactory.Warning;
                        }
                    }

                    if (_robotPosition.x == x && _robotPosition.y == y)
                    {
                        color = new Color32(52, 152, 219, 255);
                        label = "R" + DirectionSymbol(_direction);
                        labelColor = Color.white;
                    }

                    _board.SetCell(x, y, color, label, labelColor);
                }
            }
        }

        private void StopRunRoutine()
        {
            if (_runRoutine != null)
            {
                Context.Bootstrap.StopCoroutine(_runRoutine);
                _runRoutine = null;
            }
        }

        private static Vector2Int DirectionToVector(RobotDirection direction)
        {
            return direction switch
            {
                RobotDirection.Up => new Vector2Int(0, 1),
                RobotDirection.Right => new Vector2Int(1, 0),
                RobotDirection.Down => new Vector2Int(0, -1),
                _ => new Vector2Int(-1, 0)
            };
        }

        private static string DirectionSymbol(RobotDirection direction)
        {
            return direction switch
            {
                RobotDirection.Up => "^",
                RobotDirection.Right => ">",
                RobotDirection.Down => "v",
                _ => "<"
            };
        }

        private readonly struct LoopBlock
        {
            public LoopBlock(CommandType command, int repeatCount)
            {
                Command = command;
                RepeatCount = repeatCount;
            }

            public CommandType Command { get; }
            public int RepeatCount { get; }

            public override string ToString()
            {
                if (RepeatCount <= 1)
                {
                    return Command switch
                    {
                        CommandType.Forward => "Forward",
                        CommandType.TurnLeft => "Turn Left",
                        _ => "Turn Right"
                    };
                }

                return Command switch
                {
                    CommandType.Forward => $"Repeat {RepeatCount}x Forward",
                    CommandType.TurnLeft => $"Repeat {RepeatCount}x Turn Left",
                    _ => $"Repeat {RepeatCount}x Turn Right"
                };
            }
        }
    }
}
