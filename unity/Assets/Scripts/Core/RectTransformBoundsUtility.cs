using UnityEngine;

namespace PythonPractice
{
    public static class RectTransformBoundsUtility
    {
        public static Rect GetSiblingSpaceRect(RectTransform rectTransform)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 min = rectTransform.anchoredPosition - Vector2.Scale(size, rectTransform.pivot);
            return new Rect(min, size);
        }

        public static Rect GetSiblingSpaceRect(RectTransform rectTransform, Vector2 anchoredPosition)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 min = anchoredPosition - Vector2.Scale(size, rectTransform.pivot);
            return new Rect(min, size);
        }

        public static bool Overlaps(RectTransform first, RectTransform second)
        {
            return GetSiblingSpaceRect(first).Overlaps(GetSiblingSpaceRect(second));
        }

        public static bool Overlaps(RectTransform movingRect, Vector2 anchoredPosition, RectTransform obstacle)
        {
            return GetSiblingSpaceRect(movingRect, anchoredPosition).Overlaps(GetSiblingSpaceRect(obstacle));
        }

        public static bool Contains(RectTransform container, RectTransform child, Vector2 anchoredPosition)
        {
            Rect containerRect = new Rect(-Vector2.Scale(container.rect.size, container.pivot), container.rect.size);
            Rect childRect = GetSiblingSpaceRect(child, anchoredPosition);
            return containerRect.xMin <= childRect.xMin &&
                   containerRect.xMax >= childRect.xMax &&
                   containerRect.yMin <= childRect.yMin &&
                   containerRect.yMax >= childRect.yMax;
        }
    }
}
