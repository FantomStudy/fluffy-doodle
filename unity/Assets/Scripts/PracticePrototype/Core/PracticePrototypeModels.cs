using System;
using UnityEngine;

namespace PracticePrototype
{
    public enum PracticeLevelId
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4
    }

    public enum RobotDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum CommandType
    {
        Forward = 0,
        TurnLeft = 1,
        TurnRight = 2
    }

    [Serializable]
    public sealed class PracticeLevelEntry
    {
        public PracticeLevelId id;
        public string route;
        public string title;
        [TextArea(2, 4)]
        public string description;
    }

    public readonly struct PracticeResultData
    {
        public PracticeResultData(bool success, string title, string message)
        {
            Success = success;
            Title = title;
            Message = message;
        }

        public bool Success { get; }
        public string Title { get; }
        public string Message { get; }
    }

    public sealed class PracticeRuntimeContext
    {
        public PracticeGameBootstrap Bootstrap { get; set; }
        public PracticeUIController UI { get; set; }
        public Sprite WhiteSprite { get; set; }
        public Font DefaultFont { get; set; }
        public Action<PracticeResultData> ShowResult { get; set; }
        public Action ShowSelector { get; set; }
        public Action<PracticeLevelId> LoadLevel { get; set; }
    }

    public interface IPracticeLevel
    {
        PracticeLevelId LevelId { get; }
        string Title { get; }
        string Description { get; }

        void Build(PracticeRuntimeContext context);
        void Show();
        void Hide();
        void ResetLevel();
        void RunLevel();
        void Tick();
        void Dispose();
    }
}
