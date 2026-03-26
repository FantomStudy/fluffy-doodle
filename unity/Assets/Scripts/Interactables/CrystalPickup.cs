using UnityEngine;
using UnityEngine.UI;
using PythonPractice;

namespace PythonPractice.Interactables
{
    public sealed class CrystalPickup : MonoBehaviour
    {
        [SerializeField]
        private RectTransform crystalRect;

        [SerializeField]
        private Image crystalImage;

        private bool collected;

        private void Awake()
        {
            if (crystalRect == null)
            {
                crystalRect = transform as RectTransform;
            }
        }

        public bool IsCollected
        {
            get { return collected; }
        }

        public bool TryCollect(RectTransform actorRect)
        {
            if (collected || actorRect == null || crystalRect == null)
            {
                return false;
            }

            if (!RectTransformBoundsUtility.Overlaps(actorRect, crystalRect))
            {
                return false;
            }

            collected = true;
            if (crystalImage != null)
            {
                crystalImage.enabled = false;
            }

            return true;
        }

        public void ResetCrystal()
        {
            collected = false;
            if (crystalImage != null)
            {
                crystalImage.enabled = true;
            }
        }
    }
}
