using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {
        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        [Header("Interact Button")]
        [SerializeField] private string interactButtonName = "UI_Virtual_Button_Interact";
        [SerializeField] private Vector2 interactButtonAnchoredPosition = new Vector2(-285f, 177f);
        [SerializeField] private string interactButtonLabel = "E";
        [SerializeField] private int interactButtonFontSize = 56;

        private global::PlayerInteractor playerInteractor;
        private global::UIVirtualButton interactButton;

        private void OnEnable()
        {
            EnsureBindings();
            EnsureInteractButton();
        }

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.MoveInput(virtualMoveDirection);
            }
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.LookInput(virtualLookDirection);
            }
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.JumpInput(virtualJumpState);
            }
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.SprintInput(virtualSprintState);
            }
        }

        public void VirtualInteractClick()
        {
            EnsureBindings();
            if (starterAssetsInputs != null && !starterAssetsInputs.enabled)
            {
                return;
            }

            playerInteractor?.TryInteract();
        }

        private void EnsureBindings()
        {
            if (starterAssetsInputs == null)
            {
                starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>(true);
            }

            if (playerInteractor == null && starterAssetsInputs != null)
            {
                playerInteractor = starterAssetsInputs.GetComponentInParent<global::PlayerInteractor>(true);
            }

            if (playerInteractor == null)
            {
                playerInteractor = GetComponentInParent<global::PlayerInteractor>(true);
            }
        }

        private void EnsureInteractButton()
        {
            if (interactButton == null)
            {
                Transform existingButtonTransform = transform.Find(interactButtonName);
                if (existingButtonTransform != null)
                {
                    interactButton = existingButtonTransform.GetComponent<global::UIVirtualButton>();
                }
            }

            if (interactButton == null)
            {
                interactButton = CreateOrCloneInteractButton();
            }

            if (interactButton == null)
            {
                return;
            }

            RectTransform rectTransform = interactButton.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = interactButtonAnchoredPosition;
            }

            interactButton.name = interactButtonName;
            interactButton.buttonStateOutputEvent = new global::UIVirtualButton.BoolEvent();
            interactButton.buttonClickOutputEvent = new global::UIVirtualButton.Event();
            interactButton.buttonClickOutputEvent.AddListener(VirtualInteractClick);

            Transform iconTransform = interactButton.transform.Find("Image_Icon");
            if (iconTransform != null)
            {
                iconTransform.gameObject.SetActive(false);
            }

            Text labelText = interactButton.GetComponentInChildren<Text>(true);
            if (labelText == null)
            {
                GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
                labelObject.transform.SetParent(interactButton.transform, false);
                labelText = labelObject.GetComponent<Text>();

                RectTransform labelRect = labelObject.GetComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.pivot = new Vector2(0.5f, 0.5f);
                labelRect.offsetMin = new Vector2(12f, 12f);
                labelRect.offsetMax = new Vector2(-12f, -12f);
            }

            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = interactButtonFontSize;
            labelText.fontStyle = FontStyle.Bold;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.black;
            labelText.supportRichText = false;
            labelText.text = interactButtonLabel;
        }

        private global::UIVirtualButton CreateOrCloneInteractButton()
        {
            global::UIVirtualButton templateButton = FindTemplateButton();
            if (templateButton == null)
            {
                GameObject fallbackButtonObject = new GameObject(
                    interactButtonName,
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image),
                    typeof(global::UIVirtualButton));

                fallbackButtonObject.transform.SetParent(transform, false);

                RectTransform rectTransform = fallbackButtonObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(200f, 200f);
                rectTransform.anchoredPosition = interactButtonAnchoredPosition;

                Image image = fallbackButtonObject.GetComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 0.35f);

                return fallbackButtonObject.GetComponent<global::UIVirtualButton>();
            }

            global::UIVirtualButton clonedButton = Instantiate(templateButton, transform);
            clonedButton.transform.SetAsLastSibling();
            return clonedButton;
        }

        private global::UIVirtualButton FindTemplateButton()
        {
            global::UIVirtualButton[] buttons = GetComponentsInChildren<global::UIVirtualButton>(true);
            foreach (global::UIVirtualButton button in buttons)
            {
                if (button != null && button != interactButton)
                {
                    return button;
                }
            }

            return null;
        }
    }

}
