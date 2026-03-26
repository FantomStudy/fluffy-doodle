using UnityEngine;
using UnityEngine.UI;
using PythonPractice;

namespace PythonPractice.Interactables
{
    public sealed class ActivationPoint : MonoBehaviour
    {
        [SerializeField]
        private RectTransform pointRect;

        [SerializeField]
        private Image pointImage;

        [SerializeField]
        private int orderIndex = 1;

        private bool activated;

        public int OrderIndex
        {
            get { return orderIndex; }
        }

        public bool Activated
        {
            get { return activated; }
        }

        private void Awake()
        {
            if (pointRect == null)
            {
                pointRect = transform as RectTransform;
            }

            RefreshVisual();
        }

        public bool IsOverlapping(RectTransform actorRect)
        {
            return actorRect != null && pointRect != null && RectTransformBoundsUtility.Overlaps(actorRect, pointRect);
        }

        public void MarkActivated()
        {
            activated = true;
            RefreshVisual();
        }

        public void ResetPoint()
        {
            activated = false;
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (pointImage != null)
            {
                pointImage.color = activated ? new Color(0.2f, 0.75f, 0.3f, 1f) : new Color(0.95f, 0.75f, 0.2f, 1f);
            }
        }
    }
}
