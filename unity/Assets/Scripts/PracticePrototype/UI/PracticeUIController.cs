using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    public sealed class PracticeUIController : MonoBehaviour
    {
        private Font _defaultFont;
        private Sprite _whiteSprite;
        private Text _titleText;
        private Text _descriptionText;
        private Text _statusText;
        private Button _runButton;
        private Button _resetButton;
        private Button _backButton;
        private Image _selectorOverlay;
        private RectTransform _selectorListRoot;

        public RectTransform GameArea { get; private set; }
        public RectTransform ControlArea { get; private set; }
        public PracticeResultPopup ResultPopup { get; private set; }

        public void Build(Sprite whiteSprite, Font defaultFont)
        {
            _whiteSprite = whiteSprite;
            _defaultFont = defaultFont;

            var canvas = gameObject.GetComponent<Canvas>() ?? gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.sortingOrder = 10;

            var scaler = gameObject.GetComponent<CanvasScaler>() ?? gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            var background = PracticeUiFactory.CreatePanel("Background", transform, _whiteSprite, PracticeUiFactory.ScreenBackground);
            PracticeUiFactory.Stretch(background.rectTransform, Vector2.zero, Vector2.zero);

            var safeRoot = PracticeUiFactory.CreateUIObject("SafeRoot", background.transform);
            PracticeUiFactory.Stretch(safeRoot.GetComponent<RectTransform>(), new Vector2(24f, 24f), new Vector2(-24f, -24f));
            var rootLayout = PracticeUiFactory.AddVerticalLayout(safeRoot, 18f, TextAnchor.UpperLeft, true, true, true, false);
            rootLayout.padding = new RectOffset(0, 0, 0, 0);

            BuildHeader(safeRoot.transform);
            BuildContent(safeRoot.transform);
            BuildFooter(safeRoot.transform);
            BuildSelector(background.transform);
            BuildPopup(background.transform);
        }

        public void BindNavigation(Action onRun, Action onReset, Action onBack)
        {
            RebindButton(_runButton, onRun);
            RebindButton(_resetButton, onReset);
            RebindButton(_backButton, onBack);
        }

        public void SetHeader(string title, string description)
        {
            _titleText.text = title;
            _descriptionText.text = description;
        }

        public void SetStatus(string status, Color color)
        {
            _statusText.text = status;
            _statusText.color = color;
        }

        public void SetRunLabel(string label)
        {
            var labelText = _runButton.GetComponentInChildren<Text>();
            if (labelText != null)
            {
                labelText.text = label;
            }
        }

        public void SetRunInteractable(bool isInteractable)
        {
            _runButton.interactable = isInteractable;
        }

        public void ClearContent()
        {
            DestroyChildren(GameArea);
            DestroyChildren(ControlArea);
        }

        public void ShowSelector(IEnumerable<PracticeLevelEntry> entries, Action<PracticeLevelId> onSelected)
        {
            DestroyChildren(_selectorListRoot);

            foreach (var entry in entries)
            {
                var card = PracticeUiFactory.CreatePanel(entry.id + "Card", _selectorListRoot, _whiteSprite, PracticeUiFactory.PanelBackground);
                PracticeUiFactory.AddLayoutElement(card.gameObject, preferredHeight: 120f);
                var layout = PracticeUiFactory.AddVerticalLayout(card.gameObject, 6f, TextAnchor.UpperLeft);
                layout.padding = new RectOffset(20, 20, 20, 20);

                var title = PracticeUiFactory.CreateText("Title", card.transform, _defaultFont, 24, TextAnchor.UpperLeft, PracticeUiFactory.TextPrimary, entry.title);
                PracticeUiFactory.AddLayoutElement(title.gameObject, preferredHeight: 36f);

                var description = PracticeUiFactory.CreateText("Description", card.transform, _defaultFont, 18, TextAnchor.UpperLeft, PracticeUiFactory.TextSecondary, entry.description);
                PracticeUiFactory.AddLayoutElement(description.gameObject, preferredHeight: 42f);

                var button = PracticeUiFactory.CreateButton("OpenButton", card.transform, _whiteSprite, _defaultFont, "Open", PracticeUiFactory.Accent, Color.white, () => onSelected?.Invoke(entry.id));
                PracticeUiFactory.AddLayoutElement(button.gameObject, preferredHeight: 42f);
            }

            _selectorOverlay.gameObject.SetActive(true);
        }

        public void HideSelector()
        {
            _selectorOverlay.gameObject.SetActive(false);
        }

        private void BuildHeader(Transform parent)
        {
            var header = PracticeUiFactory.CreatePanel("Header", parent, _whiteSprite, PracticeUiFactory.PanelBackground);
            PracticeUiFactory.AddLayoutElement(header.gameObject, preferredHeight: 140f);
            var layout = PracticeUiFactory.AddVerticalLayout(header.gameObject, 6f, TextAnchor.UpperLeft);
            layout.padding = new RectOffset(24, 24, 18, 18);

            _titleText = PracticeUiFactory.CreateText("Title", header.transform, _defaultFont, 34, TextAnchor.UpperLeft, PracticeUiFactory.TextPrimary, "Python Practice Prototype");
            PracticeUiFactory.AddLayoutElement(_titleText.gameObject, preferredHeight: 46f);

            _descriptionText = PracticeUiFactory.CreateText("Description", header.transform, _defaultFont, 20, TextAnchor.UpperLeft, PracticeUiFactory.TextSecondary, "Choose a level or open the build by route: /level1 ... /level4");
            PracticeUiFactory.AddLayoutElement(_descriptionText.gameObject, flexibleHeight: 1f);
        }

        private void BuildContent(Transform parent)
        {
            var content = PracticeUiFactory.CreateUIObject("Content", parent);
            PracticeUiFactory.AddLayoutElement(content, flexibleHeight: 1f, flexibleWidth: 1f);
            var layout = PracticeUiFactory.AddHorizontalLayout(content, 18f, TextAnchor.UpperLeft, true, true, true, true);
            layout.padding = new RectOffset(0, 0, 0, 0);

            var gamePanel = PracticeUiFactory.CreatePanel("GamePanel", content.transform, _whiteSprite, PracticeUiFactory.PanelBackground);
            PracticeUiFactory.AddLayoutElement(gamePanel.gameObject, preferredWidth: 960f, flexibleWidth: 1f, flexibleHeight: 1f);
            GameArea = gamePanel.rectTransform;
            PracticeUiFactory.AddVerticalLayout(gamePanel.gameObject, 8f, TextAnchor.UpperLeft, true, true, true, true).padding = new RectOffset(16, 16, 16, 16);

            var controlPanel = PracticeUiFactory.CreatePanel("ControlPanel", content.transform, _whiteSprite, PracticeUiFactory.PanelBackground);
            PracticeUiFactory.AddLayoutElement(controlPanel.gameObject, preferredWidth: 520f, flexibleWidth: 0f, flexibleHeight: 1f);
            ControlArea = controlPanel.rectTransform;
            PracticeUiFactory.AddVerticalLayout(controlPanel.gameObject, 10f, TextAnchor.UpperLeft, true, true, true, false).padding = new RectOffset(16, 16, 16, 16);
        }

        private void BuildFooter(Transform parent)
        {
            var footer = PracticeUiFactory.CreatePanel("Footer", parent, _whiteSprite, PracticeUiFactory.PanelBackground);
            PracticeUiFactory.AddLayoutElement(footer.gameObject, preferredHeight: 92f);
            var layout = PracticeUiFactory.AddHorizontalLayout(footer.gameObject, 12f, TextAnchor.MiddleLeft, true, true, false, false);
            layout.padding = new RectOffset(18, 18, 16, 16);

            _statusText = PracticeUiFactory.CreateText("Status", footer.transform, _defaultFont, 20, TextAnchor.MiddleLeft, PracticeUiFactory.TextSecondary, "Prototype loaded.");
            PracticeUiFactory.AddLayoutElement(_statusText.gameObject, flexibleWidth: 1f);

            _runButton = PracticeUiFactory.CreateButton("RunButton", footer.transform, _whiteSprite, _defaultFont, "Run", PracticeUiFactory.Accent, Color.white, null);
            PracticeUiFactory.AddLayoutElement(_runButton.gameObject, preferredWidth: 150f, preferredHeight: 52f);

            _resetButton = PracticeUiFactory.CreateButton("ResetButton", footer.transform, _whiteSprite, _defaultFont, "Reset", PracticeUiFactory.Warning, Color.white, null);
            PracticeUiFactory.AddLayoutElement(_resetButton.gameObject, preferredWidth: 150f, preferredHeight: 52f);

            _backButton = PracticeUiFactory.CreateButton("BackButton", footer.transform, _whiteSprite, _defaultFont, "Back", PracticeUiFactory.AccentSoft, PracticeUiFactory.TextPrimary, null);
            PracticeUiFactory.AddLayoutElement(_backButton.gameObject, preferredWidth: 150f, preferredHeight: 52f);
        }

        private void BuildSelector(Transform parent)
        {
            _selectorOverlay = PracticeUiFactory.CreatePanel("SelectorOverlay", parent, _whiteSprite, new Color(0.08f, 0.14f, 0.2f, 0.55f));
            PracticeUiFactory.Stretch(_selectorOverlay.rectTransform, Vector2.zero, Vector2.zero);

            var modal = PracticeUiFactory.CreatePanel("SelectorModal", _selectorOverlay.transform, _whiteSprite, PracticeUiFactory.ScreenBackground);
            var modalRect = modal.rectTransform;
            modalRect.anchorMin = new Vector2(0.5f, 0.5f);
            modalRect.anchorMax = new Vector2(0.5f, 0.5f);
            modalRect.sizeDelta = new Vector2(920f, 640f);
            modalRect.anchoredPosition = Vector2.zero;

            var modalLayout = PracticeUiFactory.AddVerticalLayout(modal.gameObject, 10f, TextAnchor.UpperLeft, true, true, true, false);
            modalLayout.padding = new RectOffset(24, 24, 24, 24);

            var heading = PracticeUiFactory.CreateText("Heading", modal.transform, _defaultFont, 30, TextAnchor.UpperLeft, PracticeUiFactory.TextPrimary, "Additional Python Practice");
            PracticeUiFactory.AddLayoutElement(heading.gameObject, preferredHeight: 40f);

            var subtitle = PracticeUiFactory.CreateText("Subtitle", modal.transform, _defaultFont, 19, TextAnchor.UpperLeft, PracticeUiFactory.TextSecondary, "Pick one lightweight 2D WebGL-friendly mini-level or open it directly by route.");
            PracticeUiFactory.AddLayoutElement(subtitle.gameObject, preferredHeight: 40f);

            var viewportImage = PracticeUiFactory.CreatePanel("ScrollViewport", modal.transform, _whiteSprite, new Color(1f, 1f, 1f, 0.55f));
            PracticeUiFactory.AddLayoutElement(viewportImage.gameObject, flexibleHeight: 1f);
            var mask = viewportImage.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            var scrollRect = viewportImage.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.viewport = viewportImage.rectTransform;

            var content = PracticeUiFactory.CreateUIObject("Content", viewportImage.transform);
            _selectorListRoot = content.GetComponent<RectTransform>();
            _selectorListRoot.anchorMin = new Vector2(0f, 1f);
            _selectorListRoot.anchorMax = new Vector2(1f, 1f);
            _selectorListRoot.pivot = new Vector2(0.5f, 1f);
            _selectorListRoot.anchoredPosition = Vector2.zero;
            _selectorListRoot.sizeDelta = Vector2.zero;
            var contentLayout = PracticeUiFactory.AddVerticalLayout(content, 16f, TextAnchor.UpperLeft);
            contentLayout.padding = new RectOffset(16, 16, 16, 16);
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollRect.content = _selectorListRoot;
        }

        private void BuildPopup(Transform parent)
        {
            var popupObject = PracticeUiFactory.CreateUIObject("ResultPopup", parent);
            ResultPopup = popupObject.AddComponent<PracticeResultPopup>();
            ResultPopup.Build(_whiteSprite, _defaultFont);
        }

        private static void RebindButton(Button button, Action action)
        {
            button.onClick.RemoveAllListeners();
            if (action != null)
            {
                button.onClick.AddListener(() => action());
            }
        }

        private static void DestroyChildren(RectTransform root)
        {
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                Destroy(root.GetChild(i).gameObject);
            }
        }
    }
}
