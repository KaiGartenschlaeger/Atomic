using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore.Input;
using System;

namespace Atomic.UI.Elements
{
    public abstract class UIElement
    {
        private readonly IUIManager _manager;
        private string _name;

        public UIElement(IUIManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            _manager = manager;
        }

        public void Update(GameTime time, IMouseManager mouse, IKeyboardManager keyboard)
        {
            IsHovered = ScreenRect.Contains(mouse.Position);
            if (IsHovered)
            {
                if (mouse.IsButtonPressed(MouseButton.Left))
                {
                    OnMouseClick(MouseButton.Left);
                    if (Clicked != null) Clicked();
                    if (MouseClicked != null) MouseClicked(MouseButton.Left);
                }
                if (mouse.IsButtonPressed(MouseButton.Middle))
                {
                    OnMouseClick(MouseButton.Middle);
                    if (MouseClicked != null) MouseClicked(MouseButton.Middle);
                }
                if (mouse.IsButtonPressed(MouseButton.Right))
                {
                    OnMouseClick(MouseButton.Right);
                    if (MouseClicked != null) MouseClicked(MouseButton.Right);
                }
            }

            OnUpdate(time, mouse, keyboard);
        }

        public void Draw(SpriteBatch batch)
        {
            if (Manager.DebugRects)
            {
                batch.DrawRect(ScreenRect, color: Color.LightBlue);
            }

            OnDraw(batch);
        }

        protected virtual void OnUpdate(GameTime time, IMouseManager mouse, IKeyboardManager keyboard)
        {
        }

        protected virtual void OnDraw(SpriteBatch batch)
        {
        }

        protected virtual void OnMouseClick(MouseButton button)
        {
        }

        public event Action Clicked;

        public event Action<MouseButton> MouseClicked;

        public IUIManager Manager
        {
            get { return _manager; }
        }

        public Vector2 Pos { get; set; }

        public virtual Rectangle ScreenRect
        {
            get
            {
                return new Rectangle(
                    (int)Pos.X,
                    (int)Pos.Y,
                    Width,
                    Height);
            }
        }

        public virtual int Width { get; set; }

        public virtual int Height { get; set; }

        public bool IsHovered { get; private set; }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}