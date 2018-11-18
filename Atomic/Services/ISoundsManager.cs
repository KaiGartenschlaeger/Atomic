using Microsoft.Xna.Framework.Content;

namespace Atomic.Services
{
    public interface ISoundsManager
    {
        void LoadContent(ContentManager content);

        void PlaySound(SoundName name);
    }
}