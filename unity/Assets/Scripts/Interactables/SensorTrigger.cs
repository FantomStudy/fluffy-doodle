using UnityEngine;
using PythonPractice;
using PythonPractice.Player;

namespace PythonPractice.Interactables
{
    public sealed class SensorTrigger : MonoBehaviour
    {
        [SerializeField]
        private RectTransform sensorRect;

        [SerializeField]
        private TopDownPlayerController playerController;

        [SerializeField]
        private LaserTrap[] laserTraps;

        [SerializeField]
        private DoorController[] doorsToOpen;

        [SerializeField]
        private DoorController[] doorsToClose;

        [SerializeField]
        private bool activateLasers = true;

        [SerializeField]
        private bool triggerOnce = true;

        private bool hasTriggered;

        private void Awake()
        {
            if (sensorRect == null)
            {
                sensorRect = transform as RectTransform;
            }
        }

        private void Update()
        {
            if (playerController == null || playerController.PlayerRect == null || sensorRect == null)
            {
                return;
            }

            if (RectTransformBoundsUtility.Overlaps(playerController.PlayerRect, sensorRect))
            {
                if (!hasTriggered || !triggerOnce)
                {
                    Activate();
                }

                hasTriggered = true;
            }
        }

        public void ResetSensor()
        {
            hasTriggered = false;
        }

        private void Activate()
        {
            if (laserTraps != null)
            {
                for (int i = 0; i < laserTraps.Length; i++)
                {
                    if (laserTraps[i] != null)
                    {
                        laserTraps[i].SetActiveState(activateLasers);
                    }
                }
            }

            if (doorsToOpen != null)
            {
                for (int i = 0; i < doorsToOpen.Length; i++)
                {
                    if (doorsToOpen[i] != null)
                    {
                        doorsToOpen[i].Open();
                    }
                }
            }

            if (doorsToClose != null)
            {
                for (int i = 0; i < doorsToClose.Length; i++)
                {
                    if (doorsToClose[i] != null)
                    {
                        doorsToClose[i].Close();
                    }
                }
            }
        }
    }
}
