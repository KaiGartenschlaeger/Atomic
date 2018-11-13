using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using System;

namespace Atomic.Entities
{
    public class GridRenderer
    {
        private readonly AtomsGrid _grid;

        public GridRenderer(AtomsGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            _grid = grid;
        }

        public void Update(GameTime time)
        {
            for (int x = 0; x < _grid.TilesWidth; x++)
            {
                for (int y = 0; y < _grid.TilesHeight; y++)
                {
                    if (_grid.Atoms[x, y] != null)
                        _grid.Atoms[x, y].Update(time);
                }
            }
        }

        public void Draw(SpriteBatch batch, Vector2 pos)
        {
            batch.DrawRect(pos, new Size(_grid.Width, _grid.Height), 4, Color.Gray);

            for (int x = 0; x < _grid.TilesWidth; x++)
            {
                if (x > 0)
                    batch.DrawLine(
                        new Vector2(
                            pos.X + x * _grid.TileSize,
                            pos.Y),
                        new Vector2(
                            pos.X + x * _grid.TileSize,
                            pos.Y + _grid.Height),
                        1,
                        Color.Gray);

                for (int y = 0; y < _grid.TilesHeight; y++)
                {
                    if (x == 0)
                    {
                        batch.DrawLine(
                            new Vector2(
                                pos.X,
                                pos.Y + y * _grid.TileSize),
                            new Vector2(
                                pos.X + _grid.Width,
                                pos.Y + y * _grid.TileSize),
                            1,
                            Color.Gray);
                    }

                    var atom = _grid.Atoms[x, y];
                    if (atom != null)
                    {
                        // connections
                        if (atom.LeftConnection != null)
                        {
                            batch.DrawTextureAtlasRegion(_grid.Contents.HConnection,
                                new Vector2(
                                    pos.X + x * _grid.TileSize,
                                    pos.Y + y * _grid.TileSize + _grid.TileSize / 2));
                        }
                        if (atom.RightConnection != null)
                        {
                            batch.DrawTextureAtlasRegion(_grid.Contents.HConnection,
                                new Vector2(
                                    pos.X + x * _grid.TileSize + _grid.TileSize,
                                    pos.Y + y * _grid.TileSize + _grid.TileSize / 2));
                        }
                        if (atom.TopConnection != null)
                        {
                            batch.DrawTextureAtlasRegion(_grid.Contents.VConnection,
                                new Vector2(
                                    pos.X + x * _grid.TileSize + _grid.TileSize / 2,
                                    pos.Y + y * _grid.TileSize));
                        }
                        if (atom.BottomConnection != null)
                        {
                            batch.DrawTextureAtlasRegion(_grid.Contents.VConnection,
                                new Vector2(
                                    pos.X + x * _grid.TileSize + _grid.TileSize / 2,
                                    pos.Y + y * _grid.TileSize + _grid.TileSize));
                        }

                        // atom
                        RenderAtom(batch,
                            pos + new Vector2(
                                x * _grid.TileSize + _grid.TileSize / 2,
                                y * _grid.TileSize + _grid.TileSize / 2),
                            atom);
                    }
                }
            }
        }

        private void RenderAtom(SpriteBatch batch, Vector2 pos, Atom atom)
        {
            atom.Draw(batch, pos);
        }
    }
}