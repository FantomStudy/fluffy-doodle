using UnityEngine;
using UnityEngine.UI;

namespace PracticePrototype
{
    internal sealed class GridBoardView
    {
        private readonly CellView[,] _cells;
        private readonly int _width;
        private readonly int _height;

        public GridBoardView(Transform parent, Sprite sprite, Font font, int width, int height, Vector2 cellSize)
        {
            _width = width;
            _height = height;
            _cells = new CellView[width, height];

            var container = PracticeUiFactory.CreatePanel("Board", parent, sprite, new Color(1f, 1f, 1f, 0f));
            PracticeUiFactory.AddLayoutElement(container.gameObject, preferredWidth: width * (cellSize.x + 8f), preferredHeight: height * (cellSize.y + 8f));
            PracticeUiFactory.AddGridLayout(container.gameObject, cellSize, new Vector2(6f, 6f), GridLayoutGroup.Constraint.FixedColumnCount, width);

            for (var row = height - 1; row >= 0; row--)
            {
                for (var column = 0; column < width; column++)
                {
                    var cell = PracticeUiFactory.CreatePanel($"Cell_{column}_{row}", container.transform, sprite, Color.white);
                    var text = PracticeUiFactory.CreateText("Label", cell.transform, font, 24, TextAnchor.MiddleCenter, PracticeUiFactory.TextPrimary, string.Empty);
                    PracticeUiFactory.Stretch(text.rectTransform, Vector2.zero, Vector2.zero);
                    _cells[column, row] = new CellView(cell, text);
                }
            }
        }

        public void SetCell(int x, int y, Color background, string label, Color labelColor)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return;
            }

            var cell = _cells[x, y];
            cell.Image.color = background;
            cell.Label.text = label;
            cell.Label.color = labelColor;
        }

        private readonly struct CellView
        {
            public CellView(Image image, Text label)
            {
                Image = image;
                Label = label;
            }

            public Image Image { get; }
            public Text Label { get; }
        }
    }
}
