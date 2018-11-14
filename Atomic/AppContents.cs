using Microsoft.Xna.Framework.Content;
using PureFreak.TileMore.Graphics;

namespace Atomic
{
    public class AppContents
    {
        private BitmapFont _defaultFont;
        private TextureAtlasRegion[] _atomRegions;
        private TextureAtlasRegion _hConnection;
        private TextureAtlasRegion _vConnection;

        public void LoadContent(ContentManager content)
        {
            var atlas = content.Load<TextureAtlas>("Atoms");

            _atomRegions = new TextureAtlasRegion[5];
            for (int i = 0; i < _atomRegions.Length; i++)
            {
                _atomRegions[i] = atlas.GetRegion("Atom" + i);
            }

            _hConnection = atlas.GetRegion("HConnection");
            _vConnection = atlas.GetRegion("VConnection");

            _defaultFont = content.Load<BitmapFont>("ArialRounded18pt");
        }

        public TextureAtlasRegion[] AtomRegions
        {
            get { return _atomRegions; }
        }

        public TextureAtlasRegion HConnection
        {
            get { return _hConnection; }
        }

        public TextureAtlasRegion VConnection
        {
            get { return _vConnection; }
        }

        public BitmapFont DefaultFont
        {
            get { return _defaultFont; }
        }
    }
}