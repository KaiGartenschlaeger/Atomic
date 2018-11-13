using Atomic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using PureFreak.TileMore.Input;

namespace Atomic
{
    public class GameApp : TileMoreGame
    {
        private Contents _contents;
        private AtomsGrid _grid;
        private GridRenderer _renderer;
        private Atom _currentAtom;
        private Atom _nextAtom;
        private SpriteFont _font;

#if DEBUG
        private int? _electrons = null;
#else
        private int? _electrons = null;
#endif

        public GameApp()
            : base(1024, 768)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Default");

            _contents = new Contents();
            _contents.LoadContent(Content);

            _grid = new AtomsGrid(_contents, 64, 10, 10);

            _currentAtom = _grid.CreateAtom(_electrons);
            _nextAtom = _grid.CreateAtom(_electrons);

            _renderer = new GridRenderer(_grid);
        }

        protected override void Update(GameTime time)
        {
            base.Update(time);

            if (_currentAtom != null)
                _currentAtom.Update(time);
            if (_nextAtom != null)
                _nextAtom.Update(time);

            if (_currentAtom != null &&
                Mouse.IsButtonPressed(MouseButton.Left) &&
                Mouse.Position.X >= 50 &&
                Mouse.Position.Y >= 50 &&
                Mouse.Position.X <= 50 + _grid.Width &&
                Mouse.Position.Y <= 50 + _grid.Height)
            {
                var tileX = (Mouse.Position.X - 50) / _grid.TileSize;
                var tileY = (Mouse.Position.Y - 50) / _grid.TileSize;

                if (_grid.SetAtom(tileX, tileY, _currentAtom))
                {
                    _currentAtom = _nextAtom;
                    _nextAtom = _grid.CreateAtom(_electrons);
                }
            }

            _renderer.Update(time);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Batch.Begin();
            _renderer.Draw(Batch, new Vector2(50, 50));

            Batch.DrawString(_font, "Next atoms:", new Vector2(50 + _grid.Width + 50, 75), Color.White);
            Batch.DrawRect(new Vector2(50 + _grid.Width + 50, 100), new Size(120, 220), 2, Color.Gray);

            if (_currentAtom != null &&
                Mouse.Position.X >= 50 &&
                Mouse.Position.Y >= 50 &&
                Mouse.Position.X <= 50 + _grid.Width &&
                Mouse.Position.Y <= 50 + _grid.Height)
            {
                var tileX = (Mouse.Position.X - 50) / _grid.TileSize;
                var tileY = (Mouse.Position.Y - 50) / _grid.TileSize;

                if (_grid.IsValidPos(tileX, tileY) && !_grid.HasAtom(tileX, tileY))
                {
                    _currentAtom.Draw(Batch, new Vector2(
                        50 + tileX * _grid.TileSize + _grid.TileSize / 2,
                        50 + tileY * _grid.TileSize + _grid.TileSize / 2),
                        Color.LightGray);
                }
            }

            var posY = 165;
            if (_currentAtom != null)
            {
                _currentAtom.Draw(Batch, new Vector2(50 + _grid.Width + 110, posY));
                posY += 85;
            }
            if (_nextAtom != null)
            {
                _nextAtom.Draw(Batch, new Vector2(50 + _grid.Width + 110, posY));
            }

            Batch.End();

            base.Draw(gameTime);
        }
    }
}