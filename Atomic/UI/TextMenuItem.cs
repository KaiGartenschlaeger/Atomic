using Microsoft.Xna.Framework;
using PureFreak.TileMore;
using System;
using System.Diagnostics;

namespace Atomic.UI
{
    [DebuggerDisplay("{_text}")]
    public class TextMenuItem
    {
        #region Fields

        private readonly TextMenu _menu;
        private string _name;
        private string _text;
        private Vector2 _textSize;
        private bool _enabled;

        #endregion

        #region Constructor

        public TextMenuItem(TextMenu menu, string text)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException(nameof(text));

            _menu = menu;

            _text = text;
            _textSize = _menu.Font.MeasureString(text);

            _enabled = true;
        }

        #endregion

        #region Methods

        public Rectangle GetScreenRect(Vector2 pos)
        {
            return new Rectangle(
                (int)(pos.X),
                (int)(pos.Y),
                (int)(_textSize.X),
                (int)(_textSize.Y));
        }

        public void RaiseClicked()
        {
            if (Clicked != null) Clicked();
        }

        #endregion

        #region Events

        public event Action Clicked;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(nameof(Text));

                _text = value;
                _textSize = _menu.Font.MeasureString(value);
            }
        }

        public Padding Margin { get; set; }

        public bool IsHovered { get; set; }

        public bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        #endregion
    }
}