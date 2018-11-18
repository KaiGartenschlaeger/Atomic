using Microsoft.Xna.Framework.Content;

namespace Atomic.Services.Sounds
{
    public interface ISoundsManager
    {
        void LoadContent(ContentManager content);
        void PlaySound(SoundName name);

        /// <summary>
        /// Volume value in percent (0% - 100%)
        /// </summary>
        byte Volume { get; set; }
    }
}