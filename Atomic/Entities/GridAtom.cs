using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Atomic.Entities
{
    [DebuggerDisplay("{_gridX} {_gridY}")]
    public class GridAtom : Atom
    {
        private readonly AtomsGrid _grid;
        private readonly int _gridX;
        private readonly int _gridY;

        public GridAtom(AtomsGrid grid, int gridX, int gridY, int electrons)
            : base(grid.Contents, electrons)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            _grid = grid;
            _gridX = gridX;
            _gridY = gridY;
        }

        public GridAtom GetAvailableNeighbourAtom()
        {
            if (LeftAtom != null && LeftAtom.Electrons > 0 && LeftAtom.RightConnection == null)
                return LeftAtom;
            else if (RightAtom != null && RightAtom.Electrons > 0 && RightAtom.LeftConnection == null)
                return RightAtom;
            else if (TopAtom != null && TopAtom.Electrons > 0 && TopAtom.BottomConnection == null)
                return TopAtom;
            else if (BottomAtom != null && BottomAtom.Electrons > 0 && BottomAtom.TopConnection == null)
                return BottomAtom;

            return null;
        }

        public bool HasConnectionTo(GridAtom atom)
        {
            if (atom == null)
                throw new ArgumentNullException(nameof(atom));

            return
                LeftConnection == atom ||
                RightConnection == atom ||
                TopConnection == atom ||
                BottomConnection == atom;
        }

        public void RefreshNeighbours()
        {
            if (Electrons > 0 && LeftAtom != null && LeftAtom.Electrons > 0 && LeftConnection != LeftAtom)
            {
                LeftConnection = LeftAtom;
                Electrons--;

                LeftAtom.RightConnection = this;
                LeftAtom.Electrons--;
            }
            if (Electrons > 0 && RightAtom != null && RightAtom.Electrons > 0 && RightConnection != RightAtom)
            {
                RightConnection = RightAtom;
                Electrons--;

                RightAtom.LeftConnection = this;
                RightAtom.Electrons--;
            }
            if (Electrons > 0 && TopAtom != null && TopAtom.Electrons > 0 && TopConnection != TopAtom)
            {
                TopConnection = TopAtom;
                Electrons--;

                TopAtom.BottomConnection = this;
                TopAtom.Electrons--;
            }
            if (Electrons > 0 && BottomAtom != null && BottomAtom.Electrons > 0 && BottomConnection != BottomAtom)
            {
                BottomConnection = BottomAtom;
                Electrons--;

                BottomAtom.TopConnection = this;
                BottomAtom.Electrons--;
            }
        }

        public int GridX
        {
            get { return _gridX; }
        }

        public int GridY
        {
            get { return _gridY; }
        }

        public Point GridPos
        {
            get { return new Point(_gridX, _gridY); }
        }

        public GridAtom LeftAtom
        {
            get { return _grid.GetAtom(_gridX - 1, _gridY); }
        }

        public GridAtom RightAtom
        {
            get { return _grid.GetAtom(_gridX + 1, _gridY); }
        }

        public GridAtom TopAtom
        {
            get { return _grid.GetAtom(_gridX, _gridY - 1); }
        }

        public GridAtom BottomAtom
        {
            get { return _grid.GetAtom(_gridX, _gridY + 1); }
        }

        public GridAtom LeftConnection { get; private set; }
        public GridAtom RightConnection { get; private set; }
        public GridAtom BottomConnection { get; private set; }
        public GridAtom TopConnection { get; private set; }

        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            return $"{_gridX} {_gridY}";
        }
    }
}