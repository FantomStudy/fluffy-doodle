using System;
using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    internal sealed class ConditionsRuleLevel : PracticeLevelBase
    {
        private readonly Vector2Int[] _walls =
        {
            new(5, 0), new(4, 4), new(0, 3)
        };

        private GridBoardView _board;
        private Text _sensorText;
        private Vector2Int _robotPosition;
        private RobotDirection _direction;
        private CommandType _selectedAction;

        public override PracticeLevelId LevelId => PracticeLevelId.Level3;
        public override string Title => "Level 3 - Conditions";
        public override string Description => "Observe what is in front of the robot and choose the correct action using a simple if rule.";

        public override void Build(PracticeRuntimeContext context)
        {
            base.Build(context);

            var layout = PracticeUiFactory.AddVerticalLayout(Root.gameObject, 10f, TextAnchor.UpperCenter, true, true, true, false);
            layout.padding = new RectOffset(0, 0, 0, 0);

            var ruleText = CreateSectionTitle(Root, "Rule: if the cell ahead is blocked, turn left. Otherwise move forward.");
            PracticeUiFactory.AddLayoutElement(ruleText.gameObject, preferredHeight: 60f);

            _sensorText = CreateBodyText(Root, string.Empty);
            PracticeUiFactory.AddLayoutElement(_sensorText.gameObject, preferredHeight: 48f);

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
            _robotPosition = new Vector2Int(0, 0);
            _direction = RobotDirection.Right;
            _selectedAction = CommandType.Forward;
            UpdateBoard();
            UpdateSensors();
            Context.UI.SetStatus("Pick the correct action, then press Run to apply the if-rule.", PracticeUiFactory.TextSecondary);
        }

        public override void RunLevel()
        {
            var expected = IsForwardBlocked() ? CommandType.TurnLeft : CommandType.Forward;
            if (_selectedAction != expected)
            {
                Context.UI.SetStatus("That action does not match the rule for this situation.", PracticeUiFactory.Error);
                Context.ShowResult(new PracticeResultData(false, "Wrong Branch", "The chosen action does not satisfy the current if-condition."));
                return;
            }

            ApplyAction(_selectedAction);
            UpdateBoard();
            UpdateSensors();

            if (_robotPosition == new Vector2Int(1, 1))
            {
                Context.UI.SetStatus("Goal reached with correct condition checks.", PracticeUiFactory.Success);
                Context.ShowResult(new PracticeResultData(true, "Success", "You used the condition correctly across the full route."));
                return;
            }

            Context.UI.SetStatus($"Correct step: {FormatCommand(_selectedAction)}", PracticeUiFactory.Success);
        }

        private void BuildControlPanel()
        {
            var controlRoot = Context.UI.ControlArea;

            var title = CreateSectionTitle(controlRoot, "Choose Action");
            PracticeUiFactory.AddLayoutElement(title.gameObject, preferredHeight: 30f);

            var buttonsHost = PracticeUiFactory.CreateUIObject("ActionButtons", controlRoot);
            PracticeUiFactory.AddLayoutElement(buttonsHost, preferredHeight: 180f);
            var layout = PracticeUiFactory.AddVerticalLayout(buttonsHost, 8f, TextAnchor.UpperLeft, true, true, true, false);
            layout.padding = new RectOffset(0, 0, 0, 0);

            AddChoiceButton(buttonsHost.transform, "Move Forward", CommandType.Forward);
            AddChoiceButton(buttonsHost.transform, "Turn Left", CommandType.TurnLeft);
            AddChoiceButton(buttonsHost.transform, "Turn Right", CommandType.TurnRight);

            var tip = CreateBodyText(controlRoot, "Only one action is valid each turn.\nSensor hint updates after every move.");
            PracticeUiFactory.AddLayoutElement(tip.gameObject, preferredHeight: 82f);
        }

        private void AddChoiceButton(Transform parent, string label, CommandType action)
        {
            var button = PracticeUiFactory.CreateButton(label + "Button", parent, Context.WhiteSprite, Context.DefaultFont, label, PracticeUiFactory.AccentSoft, PracticeUiFactory.TextPrimary, () =>
            {
                _selectedAction = action;
                Context.UI.SetStatus($"Selected: {FormatCommand(action)}. Press Run to confirm.", PracticeUiFactory.Accent);
            });

            PracticeUiFactory.AddLayoutElement(button.gameObject, preferredHeight: 40f);
        }

        private void ApplyAction(CommandType action)
        {
            if (action == CommandType.TurnLeft)
            {
                _direction = (RobotDirection)(((int)_direction + 3) % 4);
                return;
            }

            if (action == CommandType.TurnRight)
            {
                _direction = (RobotDirection)(((int)_direction + 1) % 4);
                return;
            }

            _robotPosition += DirectionToVector(_direction);
        }

        private bool IsForwardBlocked()
        {
            var target = _robotPosition + DirectionToVector(_direction);
            if (target.x < 0 || target.x > 6 || target.y < 0 || target.y > 4)
            {
                return true;
            }

            foreach (var wall in _walls)
            {
                if (wall == target)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateSensors()
        {
            _sensorText.text = $"Front sensor: {(IsForwardBlocked() ? "BLOCKED" : "FREE")} | Selected action: {FormatCommand(_selectedAction)}";
        }

        private void UpdateBoard()
        {
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 7; x++)
                {
                    var background = new Color32(246, 249, 251, 255);
                    var label = string.Empty;
                    var labelColor = PracticeUiFactory.TextPrimary;

                    if (Array.Exists(_walls, wall => wall.x == x && wall.y == y))
                    {
                        background = new Color32(127, 140, 141, 255);
                        label = "W";
                        labelColor = Color.white;
                    }

                    if (x == 1 && y == 1)
                    {
                        background = new Color32(220, 245, 226, 255);
                        label = "G";
                        labelColor = PracticeUiFactory.Success;
                    }

                    if (_robotPosition.x == x && _robotPosition.y == y)
                    {
                        background = new Color32(52, 152, 219, 255);
                        label = "R" + DirectionSymbol(_direction);
                        labelColor = Color.white;
                    }

                    _board.SetCell(x, y, background, label, labelColor);
                }
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

        private static string FormatCommand(CommandType action)
        {
            return action switch
            {
                CommandType.Forward => "Forward",
                CommandType.TurnLeft => "Turn Left",
                _ => "Turn Right"
            };
        }
    }
}
