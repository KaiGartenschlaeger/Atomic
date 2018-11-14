using Atomic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PureFreak.TileMore;
using PureFreak.TileMore.Graphics;
using PureFreak.TileMore.Input;

namespace Atomic
{
    public class GameApp : TileMoreGame
    {
        private const int GridTileSize = 64;
        private const int SidebarWidth = 340;
        private const int GridWidth = 8;
        private const int GridHeight = 10;
        private const int ScreenPadding = 45;
        private const int PreviewBoxWidth = (int)(GridTileSize * 2.5f);
        private const int PreviewBoxHeight = GridTileSize * 2;
        private const int PreviewBoxPadding = 15;
        private const int GridX = ScreenPadding;
        private const int GridY = ScreenPadding;
        private const int GridRight = GridX + GridWidth * GridTileSize + ScreenPadding;
        private const int WindowWidth = ScreenPadding + GridWidth * GridTileSize + ScreenPadding + SidebarWidth + ScreenPadding;
        private const int WindowHeight = GridHeight * GridTileSize + ScreenPadding * 2;

        private Contents _contents;
        private AtomsGrid _grid;
        private GridRenderer _renderer;
        private Atom _currentAtom;
        private Atom _nextAtom;
        private BitmapFont _font;
        private GameSession _session;

        public GameApp()
            : base(WindowWidth, WindowHeight)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _font = Content.Load<BitmapFont>("ArialRounded18pt");

            _session = new GameSession();

            _contents = new Contents();
            _contents.LoadContent(Content);

            _grid = new AtomsGrid(_contents, _session, GridTileSize, GridWidth, GridHeight);

            _currentAtom = _grid.CreateAtom();
            _nextAtom = _grid.CreateAtom();

            _renderer = new GridRenderer(_grid, _session);
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
                Mouse.Position.X >= GridX &&
                Mouse.Position.Y >= GridY &&
                Mouse.Position.X <= GridX + _grid.PixelWidth &&
                Mouse.Position.Y <= GridY + _grid.PixelHeight)
            {
                var tileX = (Mouse.Position.X - GridX) / _grid.TileSize;
                var tileY = (Mouse.Position.Y - GridY) / _grid.TileSize;

                if (_grid.SetAtom(tileX, tileY, _currentAtom))
                {
                    _currentAtom = _nextAtom;
                    _nextAtom = _grid.CreateAtom();
                }
            }

#if CHEATS_ENABLED
            var electrons = -1;

            if (Keyboard.IsKeyReleased(Keys.D1)) electrons = 1;
            else if (Keyboard.IsKeyReleased(Keys.D2)) electrons = 2;
            else if (Keyboard.IsKeyReleased(Keys.D3)) electrons = 3;
            else if (Keyboard.IsKeyReleased(Keys.D4)) electrons = 4;

            if (_currentAtom != null && electrons != -1)
                _currentAtom.Electrons = electrons;

            if (Keyboard.IsKeyReleased(Keys.R))
                _grid.Clear();
#endif

            _renderer.Update(time);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Colors.WindowBackground);

            Batch.Begin();

            // grid
            _renderer.Draw(Batch, new Vector2(GridX, GridY));

            var y = GridY + 10;

            // score
            Batch.DrawBitmapFont(_font, new Vector2(GridRight, y), $"Score:", Colors.Descriptions);
            Batch.DrawBitmapFont(_font, new Vector2(WindowWidth - ScreenPadding - 140, y), _session.Score.ToString("n0"), Colors.Texts);
            y += _font.Data.LineHeight + 15;

            Batch.DrawBitmapFont(_font, new Vector2(GridRight, y), $"Atoms:", Colors.Descriptions);
            Batch.DrawBitmapFont(_font, new Vector2(WindowWidth - ScreenPadding - 140, y), _session.Atoms.ToString("n0"), Colors.Texts);
            y += _font.Data.LineHeight + 5;

            Batch.DrawBitmapFont(_font, new Vector2(GridRight, y), $"Molecules:", Colors.Descriptions);
            Batch.DrawBitmapFont(_font, new Vector2(WindowWidth - ScreenPadding - 140, y), _session.Molecules.ToString("n0"), Colors.Texts);
            y += _font.Data.LineHeight + 5;

            // current/next atom
            y = WindowHeight - ScreenPadding - PreviewBoxHeight - _font.Data.LineHeight - 8;

            Batch.DrawBitmapFont(_font, new Vector2(GridRight, y), "Current", Colors.Descriptions);
            Batch.DrawBitmapFont(_font, new Vector2(GridRight + PreviewBoxWidth + PreviewBoxPadding, y), "Next", Colors.Descriptions);
            y += _font.Data.LineHeight + 8;

            Batch.DrawRect(new Rectangle(GridRight, y, PreviewBoxWidth, PreviewBoxHeight), 1, Colors.PreviewBorder);
            Batch.DrawRect(new Rectangle(GridRight + PreviewBoxWidth + PreviewBoxPadding, y, PreviewBoxWidth, PreviewBoxHeight), 1, Colors.PreviewBorder);

            if (_currentAtom != null)
                _currentAtom.Draw(Batch, new Vector2(GridRight + PreviewBoxWidth / 2, y + PreviewBoxHeight / 2));
            if (_nextAtom != null)
                _nextAtom.Draw(Batch, new Vector2(GridRight + PreviewBoxWidth + PreviewBoxPadding + PreviewBoxWidth / 2, y + PreviewBoxHeight / 2));

            // atom grid preview
            if (_currentAtom != null &&
                Mouse.Position.X >= GridX &&
                Mouse.Position.Y >= GridY &&
                Mouse.Position.X <= GridX + _grid.PixelWidth &&
                Mouse.Position.Y <= GridY + _grid.PixelHeight)
            {
                var tileX = (Mouse.Position.X - GridX) / _grid.TileSize;
                var tileY = (Mouse.Position.Y - GridY) / _grid.TileSize;

                if (_grid.IsValidPos(tileX, tileY) && !_grid.HasAtom(tileX, tileY))
                {
                    _currentAtom.Draw(Batch, new Vector2(
                        GridX + tileX * _grid.TileSize + _grid.TileSize / 2,
                        GridY + tileY * _grid.TileSize + _grid.TileSize / 2),
                        Color.LightGray);
                }
            }

            Batch.End();

            base.Draw(gameTime);
        }
    }
}