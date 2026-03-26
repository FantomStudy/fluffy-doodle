using UnityEngine;
using UnityEngine.UI;

namespace PythonPractice.Interactables
{
    public sealed class DoorController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform doorRect;

        [SerializeField]
        private Image doorImage;

        [SerializeField]
        private bool startOpen;

        [SerializeField]
        private bool requiresKey;

        [SerializeField]
        private Color closedColor = new Color(0.2f, 0.25f, 0.35f, 1f);

        [SerializeField]
        private Color openColor = new Color(0.2f, 0.7f, 0.3f, 0.35f);

        private bool hasKey;
        private bool isOpen;

        public RectTransform DoorRect
        {
            get { return doorRect; }
        }

        public bool IsBlocking
        {
            get { return !isOpen; }
        }

        private void Awake()
        {
            if (doorRect == null)
            {
                doorRect = transform as RectTransform;
            }

            isOpen = startOpen;
            RefreshVisual();
        }

        public void Open()
        {
            if (requiresKey && !hasKey)
            {
                return;
            }

            isOpen = true;
            RefreshVisual();
        }

        public void Close()
        {
            isOpen = false;
            RefreshVisual();
        }

        public void UnlockWithKey()
        {
            hasKey = true;
            Open();
        }

        public void ResetDoor()
        {
            hasKey = false;
            isOpen = startOpen;
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (doorImage != null)
            {
                doorImage.color = isOpen ? openColor : closedColor;
            }
        }
    }
}
