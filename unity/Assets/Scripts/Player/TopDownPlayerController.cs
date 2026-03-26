using UnityEngine;
using PythonPractice;
using PythonPractice.Interactables;

namespace PythonPractice.Player
{
    public sealed class TopDownPlayerController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform playerRect;

        [SerializeField]
        private RectTransform playArea;

        [SerializeField]
        private RectTransform[] wallRects;

        [SerializeField]
        private DoorController[] doorControllers;

        [SerializeField]
        private float moveSpeed = 260f;

        private bool inputLocked;
        private Vector2 startPosition;

        public RectTransform PlayerRect
        {
            get { return playerRect; }
        }

        private void Awake()
        {
            if (playerRect == null)
            {
                playerRect = transform as RectTransform;
            }

            if (playerRect != null)
            {
                startPosition = playerRect.anchoredPosition;
            }
        }

        private void Update()
        {
            if (inputLocked || playerRect == null || playArea == null)
            {
                return;
            }

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            if (input.sqrMagnitude > 0f)
            {
                TryMove(playerRect.anchoredPosition + input * moveSpeed * Time.unscaledDeltaTime);
            }
        }

        public void SetInputLocked(bool isLocked)
        {
            inputLocked = isLocked;
        }

        public void ResetToStart()
        {
            if (playerRect != null)
            {
                playerRect.anchoredPosition = startPosition;
            }

            inputLocked = false;
        }

        private void TryMove(Vector2 targetPosition)
        {
            if (!RectTransformBoundsUtility.Contains(playArea, playerRect, targetPosition))
            {
                return;
            }

            if (OverlapsStaticWalls(targetPosition) || OverlapsClosedDoors(targetPosition))
            {
                return;
            }

            playerRect.anchoredPosition = targetPosition;
        }

        private bool OverlapsStaticWalls(Vector2 targetPosition)
        {
            if (wallRects == null)
            {
                return false;
            }

            for (int i = 0; i < wallRects.Length; i++)
            {
                if (wallRects[i] != null && RectTransformBoundsUtility.Overlaps(playerRect, targetPosition, wallRects[i]))
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
                    RectTransformBoundsUtility.Overlaps(playerRect, targetPosition, doorControllers[i].DoorRect))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
