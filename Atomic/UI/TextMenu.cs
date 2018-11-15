using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore.Graphics;
using PureFreak.TileMore.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atomic.UI
{
    [DebuggerDisplay("Count = {_items.Count}")]
    public class TextMenu
    {
        #region Fields

        private readonly BitmapFont _font;
        private readonly List<TextMenuItem> _items;

        #endregion

        #region Constructor

        public TextMenu(BitmapFont font)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            _font = font;
            _items = new List<TextMenuItem>();

            Padding = 3f;

            Color = Color.Gray;
            HoverColor = Color.White;
        }

        #endregion

        #region Methods

        public void Update(GameTime time, IMouseManager mouse)
        {
            var currentPos = Pos;
            foreach (var item in _items)
            {
                currentPos = currentPos.Offset(item.Margin.Left, item.Margin.Top);

                var rect = item.GetScreenRect(currentPos);
                var isHovered = rect.Contains(mouse.Position);

                item.IsHovered = isHovered;

                if (item.IsHovered && mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (ItemClicked != null)
                        ItemClicked(item);

                    item.RaiseClicked();
                }

                currentPos = currentPos.Offset(0f, _font.Data.LineHeight + Padding + item.Margin.Bottom);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            var currentPos = Pos;
            foreach (var item in _items)
            {
                currentPos = currentPos.Offset(item.Margin.Left, item.Margin.Top);

                batch.DrawBitmapFont(_font, currentPos, item.Text, item.IsHovered ? HoverColor : Color);

                currentPos = currentPos.Offset(0f, _font.Data.LineHeight + Padding + item.Margin.Bottom);
            }
        }

        public TextMenuItem CreateItem(string text)
        {
            var item = new TextMenuItem(this, text);
            _items.Add(item);

            return item;
        }

        #endregion

        #region Events

        public event Action<TextMenuItem> ItemClicked;

        #endregion

        #region Properties

        public BitmapFont Font
        {
            get { return _font; }
        }

        public Color Color { get; set; }

        public Color HoverColor { get; set; }

        public float Padding { get; set; }

        public Vector2 Pos { get; set; }

        #endregion
    }
}