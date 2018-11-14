using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using System;

namespace Atomic.Entities
{
    public class Atom
    {
        private readonly Contents _contents;
        private int _electronsCount;
        private float _scale = 1f;
        private float _rotation = 0f;

        public Atom(Contents contents, int electrons)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (electrons < 0 || electrons > 4)
                throw new ArgumentException("Electrons must be a value between 0 and 4");

            _contents = contents;
            _electronsCount = electrons;
        }

        public void Update(GameTime time)
        {
            Rotation += time.ElapsedSeconds() * Angle.PiOver2;
        }

        public void Draw(SpriteBatch batch, Vector2 pos, Color? color = null)
        {
            if (!color.HasValue)
            {
                switch (_electronsCount)
                {
                    case 0:
                        color = Color.LightGreen;
                        break;
                    case 1:
                        color = Color.Green;
                        break;
                    case 2:
                        color = Color.Yellow;
                        break;
                    case 3:
                        color = Color.Blue;
                        break;
                    case 4:
                        color = Color.Red;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            var region = _contents.AtomRegions[_electronsCount];

            batch.Draw(
                region.Atlas.Textures[region.TextureIndex],
                pos,
                region.Source,
                color.Value,
                Rotation,
                region.Origin,
                Scale,
                SpriteEffects.None,
                1f);
        }

        public int Electrons
        {
            get { return _electronsCount; }
            set { _electronsCount = MathI.Clamp(value, 0, 4); }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value % Angle.TwoPi; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = MathF.Clamp(value, 0f, 1f); }
        }
    }
}