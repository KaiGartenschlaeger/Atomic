using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using PureFreak.TileMore;
using System;
using System.Collections.Generic;

namespace Atomic.Services.Sounds
{
    public class SoundsManager : ISoundsManager
    {
        #region Fields

        private readonly Dictionary<SoundName, SoundEffect> _loadedEffects;
        private byte _volume;

        #endregion

        #region Constructor

        public SoundsManager()
        {
            _loadedEffects = new Dictionary<SoundName, SoundEffect>();
            _volume = 100;
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
                var volume = (float)_volume / 100;
                effect.Play(volume, 0f, 0f);
            }
        }

        #endregion

        #region Properties

        public byte Volume
        {
            get { return _volume; }
            set
            {
                _volume = (byte)MathI.Clamp(value, 0, 100);
            }
        }

        #endregion
    }
}