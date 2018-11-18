using Atomic.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atomic.UI
{
    public class UIManager : IUIManager
    {
        #region Fields

        private readonly UISkin _skin;
        private readonly List<UIElement> _elements;

        #endregion

        #region Constructor

        public UIManager(UISkin skin)
        {
            if (skin == null)
                throw new ArgumentNullException(nameof(skin));

            _skin = skin;
            _elements = new List<UIElement>();
        }

        #endregion

        #region Methods

        public TElement Create<TElement>()
            where TElement : UIElement
        {
            var element = (TElement)Activator.CreateInstance(typeof(TElement), this);
            _elements.Add(element);

            _skin.SetElementValues(element);

            return element;
        }

        public TElement Create<TElement>(Action<TElement> configure)
            where TElement : UIElement
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var element = Create<TElement>();
            configure(element);

            return element;
        }

        public UIElement FindElement(string name)
        {
            return _elements.FirstOrDefault(e => e.Name == name);
        }

        public void Update(GameTime time, IMouseManager mouse, IKeyboardManager keyboard)
        {
            foreach (var element in _elements)
            {
                element.Update(time, mouse, keyboard);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();

            foreach (var element in _elements)
            {
                element.Draw(batch);
            }

            batch.End();
        }

        #endregion

        #region Properties

        public UISkin Skin
        {
            get { return _skin; }
        }

        public bool DebugRects { get; set; }

        #endregion
    }
}
