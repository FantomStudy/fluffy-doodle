using UnityEngine;
using UnityEngine.UI;
using PythonPractice;
using PythonPractice.Player;

namespace PythonPractice.Interactables
{
    public sealed class KeyPickup : MonoBehaviour
    {
        [SerializeField]
        private RectTransform keyRect;

        [SerializeField]
        private TopDownPlayerController playerController;

        [SerializeField]
        private DoorController[] doorsToUnlock;

        [SerializeField]
        private Image keyImage;

        private bool collected;

        private void Awake()
        {
            if (keyRect == null)
            {
                keyRect = transform as RectTransform;
            }
        }

        private void Update()
        {
            if (collected || playerController == null || playerController.PlayerRect == null || keyRect == null)
            {
                return;
            }

            if (RectTransformBoundsUtility.Overlaps(playerController.PlayerRect, keyRect))
            {
                collected = true;

                if (doorsToUnlock != null)
                {
                    for (int i = 0; i < doorsToUnlock.Length; i++)
                    {
                        if (doorsToUnlock[i] != null)
                        {
                            doorsToUnlock[i].UnlockWithKey();
                        }
                    }
                }

                if (keyImage != null)
                {
                    keyImage.enabled = false;
                }
            }
        }

        public void ResetKey()
        {
            collected = false;
            if (keyImage != null)
            {
                keyImage.enabled = true;
            }
        }
    }
}
