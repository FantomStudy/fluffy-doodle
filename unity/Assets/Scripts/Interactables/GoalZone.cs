using UnityEngine;
using PythonPractice;

namespace PythonPractice.Interactables
{
    public sealed class GoalZone : MonoBehaviour
    {
        [SerializeField]
        private RectTransform zoneRect;

        public RectTransform ZoneRect
        {
            get { return zoneRect; }
        }

        private void Awake()
        {
            if (zoneRect == null)
            {
                zoneRect = transform as RectTransform;
            }
        }

        public bool IsOverlapping(RectTransform actorRect)
        {
            return actorRect != null && zoneRect != null && RectTransformBoundsUtility.Overlaps(actorRect, zoneRect);
        }
    }
}
