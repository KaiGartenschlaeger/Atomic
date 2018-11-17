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

        private void ConnectAtoms(GridAtom source, GridAtom target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (source == target || source.Electrons <= 0 || target.Electrons <= 0)
                return;

            if (source.RightAtom == target && source.RightConnection == null)
            {
                source.Electrons--;
                source.RightConnection = target;

                target.Electrons--;
                target.LeftConnection = source;
            }
            else if (source.LeftAtom == target && source.LeftConnection == null)
            {
                source.Electrons--;
                source.LeftConnection = target;

                target.Electrons--;
                target.RightConnection = source;
            }
            else if (source.TopAtom == target && source.TopConnection == null)
            {
                source.Electrons--;
                source.TopConnection = target;

                target.Electrons--;
                target.BottomConnection = source;
            }
            else if (source.BottomAtom == target && source.BottomConnection == null)
            {
                source.Electrons--;
                source.BottomConnection = target;

                target.Electrons--;
                target.TopConnection = source;
            }
        }

        public GridAtom GetNeighbourAtomWithSmallestElectronsCount()
        {
            var result = (GridAtom)null;
            for (int i = 0; i < 4; i++) // 4 = max neighbours
            {
                if (LeftAtom != null && LeftConnection == null && LeftAtom.Electrons > 0 && (result == null || LeftAtom.Electrons < result.Electrons))
                    result = LeftAtom;

                if (RightAtom != null && RightConnection == null && RightAtom.Electrons > 0 && (result == null || RightAtom.Electrons < result.Electrons))
                    result = RightAtom;

                if (TopAtom != null && TopConnection == null && TopAtom.Electrons > 0 && (result == null || TopAtom.Electrons < result.Electrons))
                    result = TopAtom;
                if (BottomAtom != null && BottomConnection == null && BottomAtom.Electrons > 0 && (result == null || BottomAtom.Electrons < result.Electrons))
                    result = BottomAtom;

                if (result != null && result.Electrons == 1)
                    break;
            }

            return result;
        }

        public void ConnectToNeighbours()
        {
            for (int i = 0; i < 4; i++) // 4 = max neighbours
            {
                var other = GetNeighbourAtomWithSmallestElectronsCount();
                if (other != null) ConnectAtoms(this, other);

                if (other == null) break;
                if (Electrons <= 0) break;
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

        public int ConnectionsCount
        {
            get
            {
                var result = 0;
                if (TopConnection != null) result++;
                if (LeftConnection != null) result++;
                if (RightConnection != null) result++;
                if (BottomConnection != null) result++;

                return result;
            }
        }

        public GridAtom LeftConnection { get; set; }
        public GridAtom RightConnection { get; set; }
        public GridAtom BottomConnection { get; set; }
        public GridAtom TopConnection { get; set; }

        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            return $"{_gridX} {_gridY}";
        }
    }
}