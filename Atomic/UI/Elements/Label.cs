using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using System;

namespace Atomic.UI.Elements
{
    public class Label : UIElement
    {
        #region Fields

        private string _text;
        private Vector2 _textSize;
        private int _customWidth;
        private int _customHeight;

        #endregion

        #region Constructor

        public Label(IUIManager manager)
            : base(manager)
        {
            Text = string.Empty;
        }

        #endregion

        #region Methods

        protected override void OnDraw(SpriteBatch batch)
        {
            if (AutoSize)
            {
                batch.DrawBitmapFont(Manager.Skin.DefaultFont, Pos, _text,
                    color: IsHovered ? ColorHovered : Color);
            }
            else
            {
                float x, y;
                switch (TextAlignment)
                {
                    //case TextAlignment.TopLeft:
                    //    break;
                    //case TextAlignment.TopCenter:
                    //    break;
                    //case TextAlignment.TopRight:
                    //    break;

                    case TextAlignment.Left:
                        x = Pos.X;
                        y = Pos.Y + Height / 2 - _textSize.Y / 2;
                        break;

                    case TextAlignment.Center:
                        x = Pos.X + Width / 2 - _textSize.X / 2;
                        y = Pos.Y + Height / 2 - _textSize.Y / 2;
                        break;

                    case TextAlignment.Right:
                        x = Pos.X + Width - _textSize.X;
                        y = Pos.Y + Height / 2 - _textSize.Y / 2;
                        break;

                    //case TextAlignment.BottomLeft:
                    //    break;
                    //case TextAlignment.BottomCenter:
                    //    break;
                    //case TextAlignment.BottomRight:
                    //    break;

                    default:
                        throw new NotImplementedException();
                }

                batch.DrawBitmapFont(Manager.Skin.DefaultFont, new Vector2(x, y), _text,
                    color: IsHovered ? ColorHovered : Color);
            }
        }

        #endregion

        #region Properties

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value ?? string.Empty;
                _textSize = Manager.Skin.DefaultFont.MeasureString(_text);
            }
        }

        [SkinValue]
        public bool AutoSize
        {
            get; set;
        }

        public override int Width
        {
            get
            {
                if (AutoSize) return (int)_textSize.X;
                return _customWidth;
            }
            set
            {
                if (!AutoSize) _customWidth = MathI.Clamp(value, 0, int.MaxValue);
            }
        }

        public override int Height
        {
            get
            {
                if (AutoSize) return (int)_textSize.Y;
                return _customHeight;
            }
            set
            {
                if (!AutoSize) _customHeight = MathI.Clamp(value, 0, int.MaxValue);
            }
        }

        [SkinValue]
        public TextAlignment TextAlignment { get; set; }

        [SkinValue]
        public Color Color { get; set; }

        [SkinValue]
        public Color ColorHovered { get; set; }

        #endregion
    }
}
