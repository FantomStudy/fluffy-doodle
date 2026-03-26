using System;
using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    public static class PracticeUiFactory
    {
        public static Color32 ScreenBackground => new(236, 242, 247, 255);
        public static Color32 PanelBackground => new(255, 255, 255, 255);
        public static Color32 Accent => new(41, 128, 185, 255);
        public static Color32 AccentSoft => new(213, 234, 247, 255);
        public static Color32 Success => new(39, 174, 96, 255);
        public static Color32 Error => new(192, 57, 43, 255);
        public static Color32 Warning => new(243, 156, 18, 255);
        public static Color32 TextPrimary => new(36, 53, 71, 255);
        public static Color32 TextSecondary => new(93, 109, 126, 255);

        public static RectTransform Stretch(RectTransform rectTransform, Vector2 offsetMin, Vector2 offsetMax)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        public static GameObject CreateUIObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        public static Image CreatePanel(string name, Transform parent, Sprite sprite, Color color)
        {
            var gameObject = CreateUIObject(name, parent);
            var image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.color = color;
            return image;
        }

        public static Text CreateText(string name, Transform parent, Font font, int fontSize, TextAnchor alignment, Color color, string content)
        {
            var gameObject = CreateUIObject(name, parent);
            var text = gameObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.supportRichText = true;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = content;
            return text;
        }

        public static Button CreateButton(string name, Transform parent, Sprite sprite, Font font, string label, Color background, Color foreground, Action onClick)
        {
            var image = CreatePanel(name, parent, sprite, background);
            var button = image.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = background;
            colors.highlightedColor = Multiply(background, 1.08f);
            colors.pressedColor = Multiply(background, 0.9f);
            colors.selectedColor = background;
            colors.disabledColor = new Color(background.r * 0.8f, background.g * 0.8f, background.b * 0.8f, 0.6f);
            button.colors = colors;
            button.targetGraphic = image;
            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            var labelText = CreateText("Label", image.transform, font, 20, TextAnchor.MiddleCenter, foreground, label);
            Stretch(labelText.rectTransform, Vector2.zero, Vector2.zero);
            return button;
        }

        public static VerticalLayoutGroup AddVerticalLayout(GameObject gameObject, float spacing, TextAnchor alignment, bool controlWidth = true, bool controlHeight = true, bool forceExpandWidth = true, bool forceExpandHeight = false)
        {
            var layout = gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = new RectOffset(16, 16, 16, 16);
            layout.childAlignment = alignment;
            layout.childControlWidth = controlWidth;
            layout.childControlHeight = controlHeight;
            layout.childForceExpandWidth = forceExpandWidth;
            layout.childForceExpandHeight = forceExpandHeight;
            return layout;
        }

        public static HorizontalLayoutGroup AddHorizontalLayout(GameObject gameObject, float spacing, TextAnchor alignment, bool controlWidth = true, bool controlHeight = true, bool forceExpandWidth = true, bool forceExpandHeight = false)
        {
            var layout = gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = new RectOffset(16, 16, 16, 16);
            layout.childAlignment = alignment;
            layout.childControlWidth = controlWidth;
            layout.childControlHeight = controlHeight;
            layout.childForceExpandWidth = forceExpandWidth;
            layout.childForceExpandHeight = forceExpandHeight;
            return layout;
        }

        public static GridLayoutGroup AddGridLayout(GameObject gameObject, Vector2 cellSize, Vector2 spacing, GridLayoutGroup.Constraint constraint, int constraintCount)
        {
            var layout = gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = cellSize;
            layout.spacing = spacing;
            layout.constraint = constraint;
            layout.constraintCount = constraintCount;
            layout.childAlignment = TextAnchor.UpperLeft;
            return layout;
        }

        public static LayoutElement AddLayoutElement(GameObject gameObject, float preferredWidth = -1f, float preferredHeight = -1f, float flexibleWidth = -1f, float flexibleHeight = -1f)
        {
            var element = gameObject.AddComponent<LayoutElement>();
            if (preferredWidth >= 0f)
            {
                element.preferredWidth = preferredWidth;
            }

            if (preferredHeight >= 0f)
            {
                element.preferredHeight = preferredHeight;
            }

            if (flexibleWidth >= 0f)
            {
                element.flexibleWidth = flexibleWidth;
            }

            if (flexibleHeight >= 0f)
            {
                element.flexibleHeight = flexibleHeight;
            }

            return element;
        }

        public static Color Multiply(Color color, float multiplier)
        {
            return new Color(
                Mathf.Clamp01(color.r * multiplier),
                Mathf.Clamp01(color.g * multiplier),
                Mathf.Clamp01(color.b * multiplier),
                color.a);
        }
    }
}
