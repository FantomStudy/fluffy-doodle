using UnityEngine;
using PythonPractice;
using PythonPractice.Levels.Variables;
using PythonPractice.Player;

namespace PythonPractice.Interactables
{
    public sealed class DataContainer : MonoBehaviour
    {
        [SerializeField]
        private RectTransform containerRect;

        [SerializeField]
        private DataType acceptedType = DataType.Int;

        [SerializeField]
        private VariablesLevelController variablesLevelController;

        public DataType AcceptedType
        {
            get { return acceptedType; }
        }

        private void Awake()
        {
            if (containerRect == null)
            {
                containerRect = transform as RectTransform;
            }
        }

        public bool IsOverlapping(RectTransform actorRect)
        {
            return actorRect != null && containerRect != null && RectTransformBoundsUtility.Overlaps(actorRect, containerRect);
        }

        public void TryAccept(CarryItemController carrier)
        {
            if (carrier == null || !carrier.HasItem || variablesLevelController == null)
            {
                return;
            }

            DataPickup carriedPickup = carrier.CarriedPickup;
            if (carriedPickup == null)
            {
                return;
            }

            if (carriedPickup.DataType == acceptedType)
            {
                variablesLevelController.HandleCorrectDeposit(this, carriedPickup);
                carrier.RemoveCarriedItem(true);
            }
            else
            {
                variablesLevelController.HandleWrongDeposit(this, carriedPickup);
                carrier.RemoveCarriedItem(false);
            }
        }
    }
}
