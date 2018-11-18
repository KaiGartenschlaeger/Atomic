using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Atomic.Services.Sounds
{
    public class SoundsManager : ISoundsManager
    {
        #region Fields

        private readonly Dictionary<SoundName, SoundEffect> _loadedEffects;

        #endregion

        #region Constructor

        public SoundsManager()
        {
            _loadedEffects = new Dictionary<SoundName, SoundEffect>();
        }

        #endregion

        #region Methods

        public void LoadContent(ContentManager content)
        {
            _loadedEffects.Clear();

            foreach (var value in Enum.GetValues(typeof(SoundName)))
            {
                var name = (SoundName)value;

                _loadedEffects.Add(name, content.Load<SoundEffect>("Sounds/" + name.ToString()));
            }
        }

        public void PlaySound(SoundName name)
        {
            if (_loadedEffects.TryGetValue(name, out SoundEffect effect))
            {
                effect.Play();
            }
        }

        #endregion
    }
}