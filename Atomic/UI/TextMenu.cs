using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore.Graphics;
using PureFreak.TileMore.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            ColorDisabled = Color.DarkGray;
            ColorHover = Color.White;
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

                if (item.IsHovered && mouse.IsButtonPressed(MouseButton.Left) && item.IsEnabled)
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

                var color = Color;
                if (!item.IsEnabled) color = ColorDisabled;
                else if (item.IsHovered) color = ColorHover;

                batch.DrawBitmapFont(_font, currentPos, item.Text, color);

                currentPos = currentPos.Offset(0f, _font.Data.LineHeight + Padding + item.Margin.Bottom);
            }
        }

        public TextMenuItem CreateItem(string text)
        {
            return CreateItem(text, null);
        }

        public TextMenuItem CreateItem(string text, string name)
        {
            var item = new TextMenuItem(this, text);
            _items.Add(item);

            item.Name = name;
            if (item.Name == null) item.Name = text.Replace(' ', '_').ToLowerInvariant();

            return item;
        }

        #endregion

        #region Events

        public event Action<TextMenuItem> ItemClicked;

        #endregion

        #region Properties

        public TextMenuItem GetItem(string name)
        {
            return _items.FirstOrDefault(i => i.Name == name);
        }

        public BitmapFont Font
        {
            get { return _font; }
        }

        public Color Color { get; set; }

        public Color ColorHover { get; set; }

        public Color ColorDisabled { get; set; }

        public float Padding { get; set; }

        public Vector2 Pos { get; set; }

        #endregion
    }
}