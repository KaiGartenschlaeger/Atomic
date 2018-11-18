using Atomic.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore.Input;
using System;

namespace Atomic.UI
{
    public interface IUIManager
    {
        #region Methods

        UIElement FindElement(string name);

        TElement Create<TElement>()
            where TElement : UIElement;

        TElement Create<TElement>(Action<TElement> configure)
            where TElement : UIElement;

        void Update(GameTime time, IMouseManager mouse, IKeyboardManager keyboard);

        void Draw(SpriteBatch batch);

        #endregion

        #region Properties

        UISkin Skin { get; }

        bool DebugRects { get; set; }

        #endregion
    }
}