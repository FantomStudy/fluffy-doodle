using UnityEngine;
using UnityEngine.UI;
using PythonPractice;
using PythonPractice.Player;

namespace PythonPractice.Interactables
{
    public sealed class LaserTrap : MonoBehaviour
    {
        [SerializeField]
        private RectTransform laserRect;

        [SerializeField]
        private Image laserImage;

        [SerializeField]
        private LevelBase levelBase;

        [SerializeField]
        private TopDownPlayerController playerController;

        [SerializeField]
        private bool startActive = true;

        [SerializeField]
        private string failMessage = "A laser trap hit the player.";

        private bool isActive;

        private void Awake()
        {
            if (laserRect == null)
            {
                laserRect = transform as RectTransform;
            }

            isActive = startActive;
            RefreshVisual();
        }

        private void Update()
        {
            if (!isActive || levelBase == null || levelBase.IsLevelFinished || playerController == null || playerController.PlayerRect == null)
            {
                return;
            }

            if (RectTransformBoundsUtility.Overlaps(playerController.PlayerRect, laserRect))
            {
                levelBase.FailLevel("Laser Triggered", failMessage);
            }
        }

        public void SetActiveState(bool value)
        {
            isActive = value;
            RefreshVisual();
        }

        public void ResetTrap()
        {
            isActive = startActive;
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (laserImage != null)
            {
                laserImage.color = isActive ? new Color(0.9f, 0.2f, 0.2f, 0.9f) : new Color(0.5f, 0.5f, 0.5f, 0.25f);
            }
        }
    }
}
