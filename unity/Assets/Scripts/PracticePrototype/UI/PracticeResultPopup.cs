using System;
using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    public sealed class PracticeResultPopup : MonoBehaviour
    {
        private Text _titleText;
        private Text _messageText;
        private Button _restartButton;
        private Button _backButton;

        public void Build(Sprite whiteSprite, Font font)
        {
            var overlay = PracticeUiFactory.CreatePanel("Overlay", transform, whiteSprite, new Color(0f, 0f, 0f, 0.35f));
            PracticeUiFactory.Stretch(overlay.rectTransform, Vector2.zero, Vector2.zero);

            var window = PracticeUiFactory.CreatePanel("Window", overlay.transform, whiteSprite, PracticeUiFactory.PanelBackground);
            var windowRect = window.rectTransform;
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.sizeDelta = new Vector2(480f, 280f);
            windowRect.anchoredPosition = Vector2.zero;

            var layout = PracticeUiFactory.AddVerticalLayout(window.gameObject, 12f, TextAnchor.UpperCenter);
            layout.padding = new RectOffset(24, 24, 24, 24);

            _titleText = PracticeUiFactory.CreateText("Title", window.transform, font, 28, TextAnchor.MiddleCenter, PracticeUiFactory.TextPrimary, string.Empty);
            PracticeUiFactory.AddLayoutElement(_titleText.gameObject, preferredHeight: 46f);

            _messageText = PracticeUiFactory.CreateText("Message", window.transform, font, 19, TextAnchor.UpperCenter, PracticeUiFactory.TextSecondary, string.Empty);
            PracticeUiFactory.AddLayoutElement(_messageText.gameObject, preferredHeight: 110f, flexibleHeight: 1f);

            var buttonsRow = PracticeUiFactory.CreateUIObject("Buttons", window.transform);
            var buttonsLayout = PracticeUiFactory.AddHorizontalLayout(buttonsRow, 12f, TextAnchor.MiddleCenter, true, true, true, false);
            buttonsLayout.padding = new RectOffset(0, 0, 8, 0);
            PracticeUiFactory.AddLayoutElement(buttonsRow, preferredHeight: 62f);

            _restartButton = PracticeUiFactory.CreateButton("RestartButton", buttonsRow.transform, whiteSprite, font, "Restart", PracticeUiFactory.Accent, Color.white, null);
            _backButton = PracticeUiFactory.CreateButton("BackButton", buttonsRow.transform, whiteSprite, font, "Back", PracticeUiFactory.AccentSoft, PracticeUiFactory.TextPrimary, null);

            gameObject.SetActive(false);
        }

        public void Show(PracticeResultData result, Action onRestart, Action onBack)
        {
            _titleText.text = result.Title;
            _titleText.color = result.Success ? PracticeUiFactory.Success : PracticeUiFactory.Error;
            _messageText.text = result.Message;

            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(() => onRestart?.Invoke());

            _backButton.onClick.RemoveAllListeners();
            _backButton.onClick.AddListener(() => onBack?.Invoke());

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
