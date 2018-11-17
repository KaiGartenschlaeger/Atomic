using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using System;

namespace Atomic.Entities
{
    public class GridRenderer
    {
        private readonly AtomsGrid _grid;
        private readonly GameSession _session;

        public GridRenderer(AtomsGrid grid, GameSession session)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            _grid = grid;
            _session = session;
        }

        public void Update(GameTime time)
        {
            for (int gridX = 0; gridX < _grid.Width; gridX++)
            {
                for (int gridY = 0; gridY < _grid.Height; gridY++)
                {
                    var atom = _grid.Atoms[gridX, gridY];
                    if (atom != null)
                    {
                        atom.Update(time);

                        if (atom.IsCompleted)
                        {
                            atom.Scale -= time.ElapsedSeconds() * 0.7f;
                            if (atom.Scale <= 0.2f)
                                _grid.Atoms[gridX, gridY] = null;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch batch, Vector2 pos)
        {
            for (int gridX = 0; gridX < _grid.Width; gridX++)
            {
                if (gridX > 0)
                {
                    // vertical lines
                    batch.DrawLine(
                        new Vector2(
                            pos.X + gridX * _grid.TileSize,
                            pos.Y),
                        new Vector2(
                            pos.X + gridX * _grid.TileSize,
                            pos.Y + _grid.PixelHeight),
                        1,
                        AppColors.GridCellBorder);
                }

                for (int gridY = 0; gridY < _grid.Height; gridY++)
                {
                    if (gridX == 0 && gridY > 0)
                    {
                        // horizontal lines
                        batch.DrawLine(
                            new Vector2(
                                pos.X,
                                pos.Y + gridY * _grid.TileSize),
                            new Vector2(
                                pos.X + _grid.PixelWidth,
                                pos.Y + gridY * _grid.TileSize),
                            1,
                            AppColors.GridCellBorder);
                    }

                    var atom = _grid.Atoms[gridX, gridY];
                    if (atom != null)
                    {
                        // atom
                        RenderConnections(batch, pos, atom);
                        RenderAtom(batch,
                            pos + new Vector2(
                                gridX * _grid.TileSize + _grid.TileSize / 2,
                                gridY * _grid.TileSize + _grid.TileSize / 2),
                            atom);
                    }
                }
            }

            batch.DrawRect(pos, new Size(_grid.PixelWidth, _grid.PixelHeight), 4, AppColors.GridBorder);
        }

        private void RenderConnections(SpriteBatch batch, Vector2 pos, GridAtom atom)
        {
            if (atom.LeftConnection != null)
            {
                batch.DrawTextureAtlasRegion(_grid.Contents.HConnection,
                    new Vector2(
                        pos.X + atom.GridX * _grid.TileSize,
                        pos.Y + atom.GridY * _grid.TileSize + _grid.TileSize / 2));
            }
            if (atom.TopConnection != null)
            {
                batch.DrawTextureAtlasRegion(_grid.Contents.VConnection,
                    new Vector2(
                        pos.X + atom.GridX * _grid.TileSize + _grid.TileSize / 2,
                        pos.Y + atom.GridY * _grid.TileSize));
            }
        }

        private void RenderAtom(SpriteBatch batch, Vector2 pos, Atom atom)
        {
            atom.Draw(batch, pos);
        }
    }
}