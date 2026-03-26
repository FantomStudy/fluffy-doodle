using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    internal sealed class VariablesCollectorLevel : PracticeLevelBase
    {
        private readonly CollectibleData[] _items =
        {
            new(new Vector2Int(1, 3), "5", "int"),
            new(new Vector2Int(3, 3), "\"cat\"", "str"),
            new(new Vector2Int(5, 3), "3.5", "float"),
            new(new Vector2Int(1, 1), "7", "int"),
            new(new Vector2Int(3, 1), "\"hi\"", "str"),
            new(new Vector2Int(5, 1), "2.0", "float")
        };

        private readonly VariableObjective[] _objectives =
        {
            new("Collect only int values.", item => item.Type == "int"),
            new("Collect the exact value 5.", item => item.Label == "5"),
            new("Collect only strings.", item => item.Type == "str")
        };

        private readonly HashSet<int> _collectedIndices = new();
        private GridBoardView _board;
        private Text _objectiveText;
        private Text _progressText;
        private int _objectiveIndex;
        private Vector2Int _playerPosition;

        public override PracticeLevelId LevelId => PracticeLevelId.Level2;
        public override string Title => "Level 2 - Variables and Data Types";
        public override string Description => "Move around the arena and collect objects matching the requested type or value.";

        public override void Build(PracticeRuntimeContext context)
        {
            base.Build(context);

            var layout = PracticeUiFactory.AddVerticalLayout(Root.gameObject, 10f, TextAnchor.UpperCenter, true, true, true, false);
            layout.padding = new RectOffset(0, 0, 0, 0);

            _objectiveText = CreateSectionTitle(Root, string.Empty);
            _progressText = CreateBodyText(Root, string.Empty);
            PracticeUiFactory.AddLayoutElement(_progressText.gameObject, preferredHeight: 46f);

            var boardHost = PracticeUiFactory.CreateUIObject("BoardHost", Root);
            PracticeUiFactory.AddLayoutElement(boardHost, preferredHeight: 420f);
            var boardLayout = PracticeUiFactory.AddHorizontalLayout(boardHost, 0f, TextAnchor.MiddleCenter, false, true, false, false);
            boardLayout.padding = new RectOffset(0, 0, 8, 8);

            _board = new GridBoardView(boardHost.transform, Context.WhiteSprite, Context.DefaultFont, 7, 5, new Vector2(82f, 82f));
            BuildControlPanel();
            ResetLevel();
        }

        public override void ResetLevel()
        {
            _collectedIndices.Clear();
            _playerPosition = new Vector2Int(0, 2);
            UpdateTexts();
            UpdateBoard();
            Context.UI.SetStatus("Move with arrow keys or the UI buttons. Wrong pickup fails immediately.", PracticeUiFactory.TextSecondary);
        }

        public override void RunLevel()
        {
            if (HasCompletedObjective())
            {
                Context.ShowResult(new PracticeResultData(true, "Success", "All required values were collected for this variable task."));
                return;
            }

            Context.UI.SetStatus("Not complete yet. Keep collecting the correct values.", PracticeUiFactory.Warning);
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                TryMove(Vector2Int.up);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                TryMove(Vector2Int.down);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                TryMove(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                TryMove(Vector2Int.right);
            }
        }

        private void BuildControlPanel()
        {
            var controlRoot = Context.UI.ControlArea;

            var objectiveHeader = CreateSectionTitle(controlRoot, "Objective");
            PracticeUiFactory.AddLayoutElement(objectiveHeader.gameObject, preferredHeight: 30f);

            var objectiveTabs = PracticeUiFactory.CreateUIObject("ObjectiveTabs", controlRoot);
            var tabsLayout = PracticeUiFactory.AddVerticalLayout(objectiveTabs, 8f, TextAnchor.UpperLeft, true, true, true, false);
            tabsLayout.padding = new RectOffset(0, 0, 0, 0);
            PracticeUiFactory.AddLayoutElement(objectiveTabs, preferredHeight: 160f);

            for (var i = 0; i < _objectives.Length; i++)
            {
                var objectiveIndex = i;
                var button = PracticeUiFactory.CreateButton("ObjectiveButton", objectiveTabs.transform, Context.WhiteSprite, Context.DefaultFont, _objectives[i].Label, PracticeUiFactory.AccentSoft, PracticeUiFactory.TextPrimary, () =>
                {
                    _objectiveIndex = objectiveIndex;
                    ResetLevel();
                });

                PracticeUiFactory.AddLayoutElement(button.gameObject, preferredHeight: 42f);
            }

            var moveHeader = CreateSectionTitle(controlRoot, "Move");
            PracticeUiFactory.AddLayoutElement(moveHeader.gameObject, preferredHeight: 30f);

            var movePad = PracticeUiFactory.CreateUIObject("MovePad", controlRoot);
            PracticeUiFactory.AddLayoutElement(movePad, preferredHeight: 190f);
            var moveLayout = PracticeUiFactory.AddVerticalLayout(movePad, 8f, TextAnchor.MiddleCenter, true, true, true, false);
            moveLayout.padding = new RectOffset(60, 60, 0, 0);

            var up = PracticeUiFactory.CreateButton("UpButton", movePad.transform, Context.WhiteSprite, Context.DefaultFont, "Up", PracticeUiFactory.Accent, Color.white, () => TryMove(Vector2Int.up));
            PracticeUiFactory.AddLayoutElement(up.gameObject, preferredHeight: 40f, preferredWidth: 140f);

            var middle = PracticeUiFactory.CreateUIObject("Middle", movePad.transform);
            var middleLayout = PracticeUiFactory.AddHorizontalLayout(middle, 8f, TextAnchor.MiddleCenter, true, true, true, false);
            middleLayout.padding = new RectOffset(0, 0, 0, 0);
            PracticeUiFactory.AddLayoutElement(middle, preferredHeight: 42f);

            var left = PracticeUiFactory.CreateButton("LeftButton", middle.transform, Context.WhiteSprite, Context.DefaultFont, "Left", PracticeUiFactory.Accent, Color.white, () => TryMove(Vector2Int.left));
            PracticeUiFactory.AddLayoutElement(left.gameObject, preferredHeight: 40f);

            var right = PracticeUiFactory.CreateButton("RightButton", middle.transform, Context.WhiteSprite, Context.DefaultFont, "Right", PracticeUiFactory.Accent, Color.white, () => TryMove(Vector2Int.right));
            PracticeUiFactory.AddLayoutElement(right.gameObject, preferredHeight: 40f);

            var down = PracticeUiFactory.CreateButton("DownButton", movePad.transform, Context.WhiteSprite, Context.DefaultFont, "Down", PracticeUiFactory.Accent, Color.white, () => TryMove(Vector2Int.down));
            PracticeUiFactory.AddLayoutElement(down.gameObject, preferredHeight: 40f, preferredWidth: 140f);

            var legend = CreateBodyText(controlRoot, "Blue = player, green = correct pickups, red = wrong type. Keyboard arrows also work in WebGL.");
            PracticeUiFactory.AddLayoutElement(legend.gameObject, preferredHeight: 82f);
        }

        private void TryMove(Vector2Int delta)
        {
            var next = _playerPosition + delta;
            if (next.x < 0 || next.x > 6 || next.y < 0 || next.y > 4)
            {
                Context.UI.SetStatus("Arena edge reached.", PracticeUiFactory.Warning);
                return;
            }

            _playerPosition = next;
            EvaluateCollection();
            UpdateBoard();
        }

        private void EvaluateCollection()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                if (_collectedIndices.Contains(i) || _items[i].Position != _playerPosition)
                {
                    continue;
                }

                if (!_objectives[_objectiveIndex].Matches(_items[i]))
                {
                    Context.UI.SetStatus("Wrong item collected.", PracticeUiFactory.Error);
                    Context.ShowResult(new PracticeResultData(false, "Type Error", $"You picked {_items[i].Label}, but the task is: {_objectives[_objectiveIndex].Label}"));
                    return;
                }

                _collectedIndices.Add(i);
                UpdateTexts();
                Context.UI.SetStatus($"Collected {_items[i].Label}", PracticeUiFactory.Success);

                if (HasCompletedObjective())
                {
                    Context.ShowResult(new PracticeResultData(true, "Success", "All matching values were collected without mistakes."));
                }
                return;
            }
        }

        private bool HasCompletedObjective()
        {
            var needed = 0;
            var collected = 0;
            for (var i = 0; i < _items.Length; i++)
            {
                if (_objectives[_objectiveIndex].Matches(_items[i]))
                {
                    needed++;
                    if (_collectedIndices.Contains(i))
                    {
                        collected++;
                    }
                }
            }

            return needed > 0 && needed == collected;
        }

        private void UpdateTexts()
        {
            _objectiveText.text = _objectives[_objectiveIndex].Label;
            var needed = 0;
            var collected = 0;
            for (var i = 0; i < _items.Length; i++)
            {
                if (_objectives[_objectiveIndex].Matches(_items[i]))
                {
                    needed++;
                    if (_collectedIndices.Contains(i))
                    {
                        collected++;
                    }
                }
            }

            _progressText.text = $"Progress: {collected}/{needed} matching objects collected.";
        }

        private void UpdateBoard()
        {
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 7; x++)
                {
                    _board.SetCell(x, y, new Color32(246, 249, 251, 255), string.Empty, PracticeUiFactory.TextPrimary);
                }
            }

            for (var i = 0; i < _items.Length; i++)
            {
                if (_collectedIndices.Contains(i))
                {
                    continue;
                }

                var item = _items[i];
                var matches = _objectives[_objectiveIndex].Matches(item);
                var color = matches ? new Color32(220, 245, 226, 255) : new Color32(250, 231, 231, 255);
                _board.SetCell(item.Position.x, item.Position.y, color, item.Label, PracticeUiFactory.TextPrimary);
            }

            _board.SetCell(_playerPosition.x, _playerPosition.y, new Color32(52, 152, 219, 255), "P", Color.white);
        }

        private readonly struct CollectibleData
        {
            public CollectibleData(Vector2Int position, string label, string type)
            {
                Position = position;
                Label = label;
                Type = type;
            }

            public Vector2Int Position { get; }
            public string Label { get; }
            public string Type { get; }
        }

        private readonly struct VariableObjective
        {
            private readonly Func<CollectibleData, bool> _predicate;

            public VariableObjective(string label, Func<CollectibleData, bool> predicate)
            {
                Label = label;
                _predicate = predicate;
            }

            public string Label { get; }

            public bool Matches(CollectibleData data)
            {
                return _predicate.Invoke(data);
            }
        }
    }
}
