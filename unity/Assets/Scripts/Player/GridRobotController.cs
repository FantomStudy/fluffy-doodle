using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonPractice;
using PythonPractice.Interactables;
using PythonPractice.Levels.Algorithms;

namespace PythonPractice.Player
{
    public sealed class GridRobotController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform robotRect;

        [SerializeField]
        private RectTransform playArea;

        [SerializeField]
        private RectTransform[] wallRects;

        [SerializeField]
        private DoorController[] doorControllers;

        [SerializeField]
        private float stepDistance = 72f;

        [SerializeField]
        private float stepDuration = 0.22f;

        [SerializeField]
        private RobotDirection startDirection = RobotDirection.Right;

        private Coroutine activeRoutine;
        private Vector2 startPosition;
        private RobotDirection currentDirection;

        public RectTransform RobotRect
        {
            get { return robotRect; }
        }

        public bool IsRunning
        {
            get { return activeRoutine != null; }
        }

        private void Awake()
        {
            if (robotRect == null)
            {
                robotRect = transform as RectTransform;
            }

            if (robotRect != null)
            {
                startPosition = robotRect.anchoredPosition;
            }

            currentDirection = startDirection;
            ApplyDirectionVisual();
        }

        public void ResetRobot()
        {
            StopExecution();

            if (robotRect != null)
            {
                robotRect.anchoredPosition = startPosition;
            }

            currentDirection = startDirection;
            ApplyDirectionVisual();
        }

        public void StopExecution()
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                activeRoutine = null;
            }
        }

        public void RunCommands(IList<RobotCommandType> commands, Action onStepCompleted, Action onCompleted, Action<string> onFailed)
        {
            if (activeRoutine != null || commands == null || commands.Count == 0)
            {
                return;
            }

            activeRoutine = StartCoroutine(RunRoutine(commands, onStepCompleted, onCompleted, onFailed));
        }

        private IEnumerator RunRoutine(IList<RobotCommandType> commands, Action onStepCompleted, Action onCompleted, Action<string> onFailed)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                RobotCommandType command = commands[i];

                if (command == RobotCommandType.MoveForward)
                {
                    Vector2 targetPosition = robotRect.anchoredPosition + DirectionToVector(currentDirection) * stepDistance;
                    if (!RectTransformBoundsUtility.Contains(playArea, robotRect, targetPosition))
                    {
                        activeRoutine = null;
                        if (onFailed != null)
                        {
                            onFailed.Invoke("The robot left the allowed grid.");
                        }
                        yield break;
                    }

                    if (OverlapsStaticWalls(targetPosition) || OverlapsClosedDoors(targetPosition))
                    {
                        activeRoutine = null;
                        if (onFailed != null)
                        {
                            onFailed.Invoke("The robot collided with an obstacle.");
                        }
                        yield break;
                    }

                    yield return MoveTo(targetPosition);
                }
                else if (command == RobotCommandType.TurnLeft)
                {
                    currentDirection = (RobotDirection)(((int)currentDirection + 3) % 4);
                    ApplyDirectionVisual();
                    yield return new WaitForSeconds(stepDuration * 0.6f);
                }
                else if (command == RobotCommandType.TurnRight)
                {
                    currentDirection = (RobotDirection)(((int)currentDirection + 1) % 4);
                    ApplyDirectionVisual();
                    yield return new WaitForSeconds(stepDuration * 0.6f);
                }

                if (onStepCompleted != null)
                {
                    onStepCompleted.Invoke();
                }
            }

            activeRoutine = null;
            if (onCompleted != null)
            {
                onCompleted.Invoke();
            }
        }

        private IEnumerator MoveTo(Vector2 targetPosition)
        {
            Vector2 start = robotRect.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < stepDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / stepDuration);
                robotRect.anchoredPosition = Vector2.Lerp(start, targetPosition, t);
                yield return null;
            }

            robotRect.anchoredPosition = targetPosition;
        }

        private bool OverlapsStaticWalls(Vector2 targetPosition)
        {
            if (wallRects == null)
            {
                return false;
            }

            for (int i = 0; i < wallRects.Length; i++)
            {
                if (wallRects[i] != null && RectTransformBoundsUtility.Overlaps(robotRect, targetPosition, wallRects[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private bool OverlapsClosedDoors(Vector2 targetPosition)
        {
            if (doorControllers == null)
            {
                return false;
            }

            for (int i = 0; i < doorControllers.Length; i++)
            {
                if (doorControllers[i] != null && doorControllers[i].IsBlocking &&
                    RectTransformBoundsUtility.Overlaps(robotRect, targetPosition, doorControllers[i].DoorRect))
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyDirectionVisual()
        {
            if (robotRect == null)
            {
                return;
            }

            float rotation = 0f;
            switch (currentDirection)
            {
                case RobotDirection.Up:
                    rotation = 0f;
                    break;
                case RobotDirection.Right:
                    rotation = -90f;
                    break;
                case RobotDirection.Down:
                    rotation = 180f;
                    break;
                case RobotDirection.Left:
                    rotation = 90f;
                    break;
            }

            robotRect.localEulerAngles = new Vector3(0f, 0f, rotation);
        }

        private static Vector2 DirectionToVector(RobotDirection direction)
        {
            switch (direction)
            {
                case RobotDirection.Up:
                    return Vector2.up;
                case RobotDirection.Right:
                    return Vector2.right;
                case RobotDirection.Down:
                    return Vector2.down;
                default:
                    return Vector2.left;
            }
        }
    }
}
