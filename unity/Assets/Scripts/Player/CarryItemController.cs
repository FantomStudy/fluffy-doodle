using UnityEngine;
using PythonPractice.Interactables;

namespace PythonPractice.Player
{
    public sealed class CarryItemController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform playerRect;

        [SerializeField]
        private RectTransform carryAnchor;

        [SerializeField]
        private DataPickup[] dataPickups;

        [SerializeField]
        private DataContainer[] dataContainers;

        private bool inputLocked;
        private DataPickup carriedPickup;

        public RectTransform PlayerRect
        {
            get { return playerRect; }
        }

        public bool HasItem
        {
            get { return carriedPickup != null; }
        }

        public DataPickup CarriedPickup
        {
            get { return carriedPickup; }
        }

        private void Awake()
        {
            if (playerRect == null)
            {
                playerRect = transform as RectTransform;
            }
        }

        private void Update()
        {
            if (inputLocked || playerRect == null)
            {
                return;
            }

            if (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.E))
            {
                return;
            }

            if (carriedPickup != null)
            {
                TryPlaceIntoContainer();
                return;
            }

            TryPickupNearbyData();
        }

        public void SetInputLocked(bool isLocked)
        {
            inputLocked = isLocked;
        }

        public void ResetCarryState()
        {
            inputLocked = false;
            if (carriedPickup != null)
            {
                carriedPickup.ResetPickup();
                carriedPickup = null;
            }
        }

        public bool TryCarry(DataPickup pickup)
        {
            if (carriedPickup != null || pickup == null)
            {
                return false;
            }

            carriedPickup = pickup;
            pickup.AttachTo(carryAnchor);
            return true;
        }

        public void RemoveCarriedItem(bool markDelivered)
        {
            if (carriedPickup == null)
            {
                return;
            }

            if (markDelivered)
            {
                carriedPickup.MarkDelivered();
            }
            else
            {
                carriedPickup.ResetPickup();
            }

            carriedPickup = null;
        }

        private void TryPickupNearbyData()
        {
            if (dataPickups == null)
            {
                return;
            }

            for (int i = 0; i < dataPickups.Length; i++)
            {
                if (dataPickups[i] != null && dataPickups[i].CanInteract && dataPickups[i].IsOverlapping(playerRect))
                {
                    dataPickups[i].TryPickup(this);
                    return;
                }
            }
        }

        private void TryPlaceIntoContainer()
        {
            if (dataContainers == null)
            {
                return;
            }

            for (int i = 0; i < dataContainers.Length; i++)
            {
                if (dataContainers[i] != null && dataContainers[i].IsOverlapping(playerRect))
                {
                    dataContainers[i].TryAccept(this);
                    return;
                }
            }
        }
    }
}
