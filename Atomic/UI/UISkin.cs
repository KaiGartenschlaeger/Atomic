using Atomic.UI.Elements;
using Microsoft.Xna.Framework;
using PureFreak.TileMore;
using PureFreak.TileMore.Graphics;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Atomic.UI
{
    public class UISkin
    {
        private readonly BitmapFont _defaultFont;
        private readonly Dictionary<KeyValuePair<Type, string>, object> _values;

        public UISkin(BitmapFont defaultFont)
        {
            if (defaultFont == null)
                throw new ArgumentNullException(nameof(defaultFont));

            _defaultFont = defaultFont;
            _values = new Dictionary<KeyValuePair<Type, string>, object>();

            SetDefaults();
        }

        private void SetDefaults()
        {
            SetValue<Label, Color>(e => e.Color, Color.Gray);
            SetValue<Label, Color>(e => e.ColorHovered, Color.White);
            SetValue<Label, bool>(e => e.AutoSize, true);
            SetValue<Label, TextAlignment>(e => e.TextAlignment, TextAlignment.Left);
        }

        public void SetValue<TElement, TValue>(Expression<Func<TElement, TValue>> memberLamda, TValue value)
            where TElement : UIElement
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                var attr = property.GetCustomAttribute<SkinValue>();

                if (property != null && property.SetMethod != null && attr != null)
                {
                    _values[new KeyValuePair<Type, string>(typeof(TElement), property.Name)] = value;
                }
            }
        }

        public void SetElementValues<TElement>(TElement element)
            where TElement : UIElement
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var type = element.GetType();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<SkinValue>();
                if (attr != null)
                {
                    var key = new KeyValuePair<Type, string>(typeof(TElement), property.Name);
                    if (_values.TryGetValue(key, out object value))
                    {
                        property.SetValue(element, value);
                    }
                }
            }
        }

        public BitmapFont DefaultFont
        {
            get { return _defaultFont; }
        }
    }
}