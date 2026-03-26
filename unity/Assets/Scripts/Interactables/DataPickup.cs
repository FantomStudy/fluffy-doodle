using UnityEngine;
using UnityEngine.UI;
using PythonPractice;
using PythonPractice.Levels.Variables;
using PythonPractice.Player;

namespace PythonPractice.Interactables
{
    public sealed class DataPickup : MonoBehaviour
    {
        [SerializeField]
        private RectTransform pickupRect;

        [SerializeField]
        private Text valueText;

        [SerializeField]
        private Image pickupImage;

        [SerializeField]
        private DataType dataType = DataType.Int;

        [SerializeField]
        private string valueLabel = "0";

        private Transform startParent;
        private Vector2 startPosition;
        private bool delivered;
        private bool beingCarried;

        public DataType DataType
        {
            get { return dataType; }
        }

        public string ValueLabel
        {
            get { return valueLabel; }
        }

        public bool CanInteract
        {
            get { return !delivered && !beingCarried; }
        }

        private void Awake()
        {
            if (pickupRect == null)
            {
                pickupRect = transform as RectTransform;
            }

            startParent = pickupRect.parent;
            startPosition = pickupRect.anchoredPosition;

            if (valueText != null)
            {
                valueText.text = valueLabel;
            }
        }

        public bool IsOverlapping(RectTransform actorRect)
        {
            return actorRect != null && pickupRect != null && RectTransformBoundsUtility.Overlaps(actorRect, pickupRect);
        }

        public void TryPickup(CarryItemController carrier)
        {
            if (carrier == null || !CanInteract)
            {
                return;
            }

            if (carrier.TryCarry(this))
            {
                beingCarried = true;
            }
        }

        public void AttachTo(RectTransform carryAnchor)
        {
            pickupRect.SetParent(carryAnchor, false);
            pickupRect.anchoredPosition = Vector2.zero;
        }

        public void MarkDelivered()
        {
            delivered = true;
            beingCarried = false;
            gameObject.SetActive(false);
        }

        public void ResetPickup()
        {
            delivered = false;
            beingCarried = false;
            pickupRect.SetParent(startParent, false);
            pickupRect.anchoredPosition = startPosition;
            gameObject.SetActive(true);
        }
    }
}
