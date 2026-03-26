using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    internal sealed class AlgorithmSequenceLevel : PracticeLevelBase
    {
        private readonly AlgorithmPuzzle[] _puzzles =
        {
            new("Direct path", "Send the robot straight to the goal.", 5, 3, new Vector2Int(0, 1), RobotDirection.Right, new Vector2Int(4, 1), Array.Empty<Vector2Int>()),
            new("One turn", "Reach the goal after turning once.", 4, 4, new Vector2Int(0, 0), RobotDirection.Up, new Vector2Int(2, 3), Array.Empty<Vector2Int>()),
            new("Obstacle", "Avoid the wall and route around it.", 5, 5, new Vector2Int(0, 2), RobotDirection.Right, new Vector2Int(4, 2), new[] { new Vector2Int(2, 2), new Vector2Int(2, 3), new Vector2Int(2, 1) })
        };

        private readonly List<CommandType> _commands = new();
        private GridBoardView _board;
        private Text _subPuzzleText;
        private Text _legendText;
        private Text _sequencePreview;
        private int _puzzleIndex;
        private Vector2Int _robotPosition;
        private RobotDirection _direction;
        private Coroutine _runRoutine;

        public override PracticeLevelId LevelId => PracticeLevelId.Level1;
        public override string Title => "Level 1 - Algorithms and Sequence";
        public override string Description => "Build a short command list so the robot reaches the target on a simple grid.";

        public override void Build(PracticeRuntimeContext context)
        {
            base.Build(context);

            var layout = PracticeUiFactory.AddVerticalLayout(Root.gameObject, 10f, TextAnchor.UpperCenter, true, true, true, false);
            layout.padding = new RectOffset(0, 0, 0, 0);

            _subPuzzleText = CreateSectionTitle(Root, string.Empty);
            _legendText = CreateBodyText(Root, string.Empty);
            PracticeUiFactory.AddLayoutElement(_legendText.gameObject, preferredHeight: 52f);

            var boardHost = PracticeUiFactory.CreateUIObject("BoardHost", Root);
            PracticeUiFactory.AddLayoutElement(boardHost, preferredHeight: 420f);
            var boardHostLayout = PracticeUiFactory.AddHorizontalLayout(boardHost, 0f, TextAnchor.MiddleCenter, false, true, false, false);
            boardHostLayout.padding = new RectOffset(0, 0, 8, 8);

            _board = new GridBoardView(boardHost.transform, Context.WhiteSprite, Context.DefaultFont, 5, 5, new Vector2(92f, 92f));
            BuildControlPanel();
            ResetLevel();
        }

        public override void ResetLevel()
        {
            StopRunRoutine();
            IsBusy = false;
            _commands.Clear();
            var puzzle = _puzzles[_puzzleIndex];
            _robotPosition = puzzle.Start;
            _direction = puzzle.Direction;
            UpdateSequencePreview();
            UpdateBoard();
            Context.UI.SetStatus("Build a command sequence, then run it step by step.", PracticeUiFactory.TextSecondary);
        }

        public override void RunLevel()
        {
            if (IsBusy)
            {
                return;
            }

            if (_commands.Count == 0)
            {
                Context.UI.SetStatus("Add at least one command before running.", PracticeUiFactory.Warning);
                return;
            }

            _runRoutine = Context.Bootstrap.StartCoroutine(PlayCommands());
        }

        private void BuildControlPanel()
        {
            var controlRoot = Context.UI.ControlArea;

            var heading = CreateSectionTitle(controlRoot, "Command Builder");
            PracticeUiFactory.AddLayoutElement(heading.gameObject, preferredHeight: 34f);

            var puzzleTabs = PracticeUiFactory.CreateUIObject("PuzzleTabs", controlRoot);
            var tabsLayout = PracticeUiFactory.AddHorizontalLayout(puzzleTabs, 10f, TextAnchor.MiddleLeft, true, true, true, false);
            tabsLayout.padding = new RectOffset(0, 0, 0, 0);
            PracticeUiFactory.AddLayoutElement(puzzleTabs, preferredHeight: 54f);

            for (var i = 0; i < _puzzles.Length; i++)
            {
                var puzzleIndex = i;
                var button = PracticeUiFactory.CreateButton("PuzzleButton", puzzleTabs.transform, Context.WhiteSprite, Context.DefaultFont, $"Puzzle {i + 1}", PracticeUiFactory.AccentSoft, PracticeUiFactory.TextPrimary, () =>
                {
                    _puzzleIndex = puzzleIndex;
                    ResetLevel();
                });

                PracticeUiFactory.AddLayoutElement(button.gameObject, preferredHeight: 44f);
            }

            var paletteTitle = CreateSectionTitle(controlRoot, "Palette");
            PracticeUiFactory.AddLayoutElement(paletteTitle.gameObject, preferredHeight: 30f);

            var palette = PracticeUiFactory.CreateUIObject("Palette", controlRoot);
            PracticeUiFactory.AddLayoutElement(palette, preferredHeight: 140f);
            var paletteLayout = PracticeUiFactory.AddVerticalLayout(palette, 8f, TextAnchor.UpperLeft, true, true, true, false);
            paletteLayout.padding = new RectOffset(0, 0, 0, 0);

            AddCommandButton(palette.transform, "Forward", CommandType.Forward);
            AddCommandButton(palette.transform, "Turn Left", CommandType.TurnLeft);
            AddCommandButton(palette.transform, "Turn Right", CommandType.TurnRight);

            var sequenceTitle = CreateSectionTitle(controlRoot, "Sequence");
            PracticeUiFactory.AddLayoutElement(sequenceTitle.gameObject, preferredHeight: 30f);

            _sequencePreview = CreateBodyText(controlRoot, string.Empty);
            PracticeUiFactory.AddLayoutElement(_sequencePreview.gameObject, preferredHeight: 120f, flexibleHeight: 1f);

            var clearButton = PracticeUiFactory.CreateButton("ClearButton", controlRoot, Context.WhiteSprite, Context.DefaultFont, "Clear Sequence", PracticeUiFactory.Warning, Color.white, () =>
            {
                if (IsBusy)
                {
                    return;
                }

                _commands.Clear();
                UpdateSequencePreview();
                Context.UI.SetStatus("Sequence cleared.", PracticeUiFactory.TextSecondary);
            });

            PracticeUiFactory.AddLayoutElement(clearButton.gameObject, preferredHeight: 46f);
        }

        private void AddCommandButton(Transform parent, string label, CommandType command)
        {
            var button = PracticeUiFactory.CreateButton(label + "Button", parent, Context.WhiteSprite, Context.DefaultFont, label, PracticeUiFactory.Accent, Color.white, () =>
            {
                if (IsBusy)
                {
                    return;
                }

                if (_commands.Count >= 8)
                {
                    Context.UI.SetStatus("This prototype keeps the sequence short: max 8 commands.", PracticeUiFactory.Warning);
                    return;
                }

                _commands.Add(command);
                UpdateSequencePreview();
            });

            PracticeUiFactory.AddLayoutElement(button.gameObject, preferredHeight: 38f);
        }

        private IEnumerator PlayCommands()
        {
            IsBusy = true;
            Context.UI.SetStatus("Robot is running the sequence.", PracticeUiFactory.Accent);

            for (var i = 0; i < _commands.Count; i++)
            {
                ExecuteCommand(_commands[i], out var failed, out var message);
                UpdateBoard();

                if (failed)
                {
                    IsBusy = false;
                    Context.UI.SetStatus(message, PracticeUiFactory.Error);
                    Context.ShowResult(new PracticeResultData(false, "Sequence Failed", message));
                    yield break;
                }

                if (_robotPosition == _puzzles[_puzzleIndex].Goal)
                {
                    IsBusy = false;
                    Context.UI.SetStatus("Goal reached.", PracticeUiFactory.Success);
                    Context.ShowResult(new PracticeResultData(true, "Success", "The robot reached the goal with your algorithm."));
                    yield break;
                }

                Context.UI.SetStatus($"Step {i + 1}/{_commands.Count}: {FormatCommand(_commands[i])}", PracticeUiFactory.Accent);
                yield return new WaitForSeconds(0.38f);
            }

            IsBusy = false;
            Context.UI.SetStatus("The robot stopped before the goal.", PracticeUiFactory.Warning);
            Context.ShowResult(new PracticeResultData(false, "Not Yet", "The sequence finished, but the robot did not reach the target."));
        }

        private void ExecuteCommand(CommandType command, out bool failed, out string message)
        {
            failed = false;
            message = string.Empty;

            if (command == CommandType.TurnLeft)
            {
                _direction = (RobotDirection)(((int)_direction + 3) % 4);
                return;
            }

            if (command == CommandType.TurnRight)
            {
                _direction = (RobotDirection)(((int)_direction + 1) % 4);
                return;
            }

            var next = _robotPosition + DirectionToVector(_direction);
            var puzzle = _puzzles[_puzzleIndex];
            if (next.x < 0 || next.x >= puzzle.Width || next.y < 0 || next.y >= puzzle.Height)
            {
                failed = true;
                message = "The robot moved outside the grid.";
                return;
            }

            foreach (var obstacle in puzzle.Obstacles)
            {
                if (obstacle == next)
                {
                    failed = true;
                    message = "The robot collided with an obstacle.";
                    return;
                }
            }

            _robotPosition = next;
        }

        private void UpdateBoard()
        {
            var puzzle = _puzzles[_puzzleIndex];
            _subPuzzleText.text = $"{puzzle.Name}: {puzzle.Description}";
            _legendText.text = "Legend: S=start, G=goal, X=wall, R=robot. Use simple turns plus forward movement.";

            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    var background = new Color32(245, 248, 250, 255);
                    var label = string.Empty;
                    var labelColor = PracticeUiFactory.TextPrimary;

                    if (x >= puzzle.Width || y >= puzzle.Height)
                    {
                        background = new Color32(224, 229, 233, 255);
                    }
                    else if (Array.Exists(puzzle.Obstacles, obstacle => obstacle.x == x && obstacle.y == y))
                    {
                        background = new Color32(149, 165, 166, 255);
                        label = "X";
                        labelColor = Color.white;
                    }
                    else if (puzzle.Goal.x == x && puzzle.Goal.y == y)
                    {
                        background = new Color32(214, 245, 221, 255);
                        label = "G";
                        labelColor = PracticeUiFactory.Success;
                    }
                    else if (puzzle.Start.x == x && puzzle.Start.y == y)
                    {
                        background = new Color32(231, 240, 253, 255);
                        label = "S";
                    }

                    if (_robotPosition.x == x && _robotPosition.y == y)
                    {
                        background = new Color32(52, 152, 219, 255);
                        label = "R" + DirectionLabel(_direction);
                        labelColor = Color.white;
                    }

                    _board.SetCell(x, y, background, label, labelColor);
                }
            }
        }

        private void UpdateSequencePreview()
        {
            if (_commands.Count == 0)
            {
                _sequencePreview.text = "No commands yet.\nTip: direct path = Forward, Forward, ...";
                return;
            }

            var lines = new List<string>();
            for (var i = 0; i < _commands.Count; i++)
            {
                lines.Add($"{i + 1}. {FormatCommand(_commands[i])}");
            }

            _sequencePreview.text = string.Join("\n", lines);
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

        private static string FormatCommand(CommandType command)
        {
            return command switch
            {
                CommandType.Forward => "Forward",
                CommandType.TurnLeft => "Turn Left",
                _ => "Turn Right"
            };
        }

        private static string DirectionLabel(RobotDirection direction)
        {
            return direction switch
            {
                RobotDirection.Up => "^",
                RobotDirection.Right => ">",
                RobotDirection.Down => "v",
                _ => "<"
            };
        }

        private readonly struct AlgorithmPuzzle
        {
            public AlgorithmPuzzle(string name, string description, int width, int height, Vector2Int start, RobotDirection direction, Vector2Int goal, Vector2Int[] obstacles)
            {
                Name = name;
                Description = description;
                Width = width;
                Height = height;
                Start = start;
                Direction = direction;
                Goal = goal;
                Obstacles = obstacles;
            }

            public string Name { get; }
            public string Description { get; }
            public int Width { get; }
            public int Height { get; }
            public Vector2Int Start { get; }
            public RobotDirection Direction { get; }
            public Vector2Int Goal { get; }
            public Vector2Int[] Obstacles { get; }
        }
    }
}
