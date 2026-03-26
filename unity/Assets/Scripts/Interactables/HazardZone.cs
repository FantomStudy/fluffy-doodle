using UnityEngine;
using PythonPractice;

namespace PythonPractice.Interactables
{
    public sealed class HazardZone : MonoBehaviour
    {
        [SerializeField]
        private RectTransform hazardRect;

        [SerializeField]
        private string failTitle = "Hazard Hit";

        [SerializeField]
        private string failMessage = "You touched a hazard.";

        public string FailTitle
        {
            get { return failTitle; }
        }

        public string FailMessage
        {
            get { return failMessage; }
        }

        private void Awake()
        {
            if (hazardRect == null)
            {
                hazardRect = transform as RectTransform;
            }
        }

        public bool IsOverlapping(RectTransform actorRect)
        {
            return actorRect != null && hazardRect != null && RectTransformBoundsUtility.Overlaps(actorRect, hazardRect);
        }
    }
}
