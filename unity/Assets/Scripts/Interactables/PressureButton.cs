using UnityEngine;
using UnityEngine.UI;
using PythonPractice;
using PythonPractice.Player;

namespace PythonPractice.Interactables
{
    public sealed class PressureButton : MonoBehaviour
    {
        [SerializeField]
        private RectTransform buttonRect;

        [SerializeField]
        private TopDownPlayerController playerController;

        [SerializeField]
        private DoorController[] doorsToOpen;

        [SerializeField]
        private GameObject[] objectsToActivate;

        [SerializeField]
        private GameObject[] objectsToDeactivate;

        [SerializeField]
        private bool activateOnce = true;

        [SerializeField]
        private Image buttonImage;

        [SerializeField]
        private Color idleColor = new Color(0.95f, 0.8f, 0.2f, 1f);

        [SerializeField]
        private Color pressedColor = new Color(0.4f, 0.8f, 0.3f, 1f);

        private bool wasPressed;
        private bool[] initialObjectsToActivateStates;
        private bool[] initialObjectsToDeactivateStates;

        private void Awake()
        {
            if (buttonRect == null)
            {
                buttonRect = transform as RectTransform;
            }

            initialObjectsToActivateStates = CaptureStates(objectsToActivate);
            initialObjectsToDeactivateStates = CaptureStates(objectsToDeactivate);
            RefreshVisual();
        }

        private void Update()
        {
            if (playerController == null || playerController.PlayerRect == null || buttonRect == null)
            {
                return;
            }

            bool isPressedNow = RectTransformBoundsUtility.Overlaps(playerController.PlayerRect, buttonRect);
            if (isPressedNow && (!wasPressed || !activateOnce))
            {
                Trigger();
            }

            wasPressed = isPressedNow || (activateOnce && wasPressed);
            RefreshVisual();
        }

        public void ResetButton()
        {
            wasPressed = false;
            RestoreStates(objectsToActivate, initialObjectsToActivateStates);
            RestoreStates(objectsToDeactivate, initialObjectsToDeactivateStates);
            RefreshVisual();
        }

        private void Trigger()
        {
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

            SetObjectsActive(objectsToActivate, true);
            SetObjectsActive(objectsToDeactivate, false);
        }

        private void RefreshVisual()
        {
            if (buttonImage != null)
            {
                buttonImage.color = wasPressed ? pressedColor : idleColor;
            }
        }

        private static void SetObjectsActive(GameObject[] objects, bool value)
        {
            if (objects == null)
            {
                return;
            }

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].SetActive(value);
                }
            }
        }

        private static bool[] CaptureStates(GameObject[] objects)
        {
            if (objects == null)
            {
                return null;
            }

            bool[] states = new bool[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                states[i] = objects[i] != null && objects[i].activeSelf;
            }

            return states;
        }

        private static void RestoreStates(GameObject[] objects, bool[] states)
        {
            if (objects == null || states == null)
            {
                return;
            }

            int count = Mathf.Min(objects.Length, states.Length);
            for (int i = 0; i < count; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].SetActive(states[i]);
                }
            }
        }
    }
}
