using Microsoft.Xna.Framework;
using PureFreak.TileMore;
using System;
using System.Collections.Generic;

namespace Atomic.Entities
{
    public class AtomsGrid
    {
        private readonly Contents _contents;
        private readonly GameSession _session;
        private readonly int _tileSize;
        private readonly int _width;
        private readonly int _height;
        private readonly GridAtom[,] _atoms;

        public AtomsGrid(Contents contents, GameSession session, int cellSize, int width, int height)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            _contents = contents;
            _session = session;
            _tileSize = cellSize;
            _width = width;
            _height = height;
            _atoms = new GridAtom[width, height];
        }

        public Atom CreateAtom(int? electrons = null)
        {
            if (electrons.HasValue && (electrons < 0 || electrons > 4))
                throw new ArgumentException("Electrons must be a value between 1 and 4");

            if (!electrons.HasValue)
                electrons = RandomHelper.Between(new Range<int>(1, 4));

            return new Atom(_contents, electrons.Value);
        }

        public bool SetAtom(int gridX, int gridY, Atom atom)
        {
            if (atom == null)
                throw new ArgumentNullException(nameof(atom));

            if (IsValidPos(gridX, gridY) && !HasAtom(gridX, gridY))
            {
                _atoms[gridX, gridY] = new GridAtom(this, gridX, gridY, atom.Electrons);

                AtomAdded(_atoms[gridX, gridY]);

                return true;
            }

            return false;
        }

        public void Clear()
        {
            for (int gridX = 0; gridX < _width; gridX++)
            {
                for (int gridY = 0; gridY < _height; gridY++)
                {
                    _atoms[gridX, gridY] = null;
                }
            }
        }

        private void AtomAdded(GridAtom addedAtom)
        {
            for (int gridX = addedAtom.GridX - 1; gridX < addedAtom.GridX + 1; gridX++)
            {
                for (int gridY = addedAtom.GridY - 1; gridY < addedAtom.GridY + 1; gridY++)
                {
                    var atom = GetAtom(gridX, gridY);
                    if (atom != null) atom.RefreshNeighbours();
                }
            }

            CheckMolecule(addedAtom);
        }

        private bool CheckMolecule(GridAtom startAtom)
        {
            if (startAtom.Electrons > 0)
                return false;

            var moleculeAtoms = new HashSet<Point>();
            var openList = new Stack<Point>();
            var closedList = new HashSet<Point>();

            openList.Push(startAtom.GridPos);
            while (openList.Count > 0)
            {
                var currentPos = openList.Pop();
                var currentAtom = GetAtom(currentPos.X, currentPos.Y);

                if (currentAtom.Electrons > 0)
                    return false;

                closedList.Add(currentPos);
                moleculeAtoms.Add(currentPos);

                if (currentAtom.LeftConnection != null && !closedList.Contains(currentAtom.LeftConnection.GridPos))
                    openList.Push(currentAtom.LeftConnection.GridPos);
                if (currentAtom.RightConnection != null && !closedList.Contains(currentAtom.RightConnection.GridPos))
                    openList.Push(currentAtom.RightConnection.GridPos);
                if (currentAtom.TopConnection != null && !closedList.Contains(currentAtom.TopConnection.GridPos))
                    openList.Push(currentAtom.TopConnection.GridPos);
                if (currentAtom.BottomConnection != null && !closedList.Contains(currentAtom.BottomConnection.GridPos))
                    openList.Push(currentAtom.BottomConnection.GridPos);
            }

            MoleculeCompleted(moleculeAtoms);
            return true;
        }

        private void MoleculeCompleted(HashSet<Point> moleculeAtoms)
        {
            var count = 0;
            var points = 1;

            foreach (var gridPos in moleculeAtoms)
            {
                count++;
                if (count > 3) points++;

                var atom = _atoms[gridPos.X, gridPos.Y];
                atom.IsCompleted = true;

                _session.Score += points;
            }

            _session.Atoms += moleculeAtoms.Count;
            _session.Molecules++;
        }

        public GridAtom GetAtom(int gridX, int gridY)
        {
            if (IsValidPos(gridX, gridY))
                return _atoms[gridX, gridY];

            return null;
        }

        public bool HasAtom(int gridX, int gridY)
        {
            return GetAtom(gridX, gridY) != null;
        }

        public bool IsValidPos(int gridX, int gridY)
        {
            return
                gridX >= 0 &&
                gridY >= 0 &&
                gridX < _width &&
                gridY < _height;
        }

        public int TileSize
        {
            get { return _tileSize; }
        }

        public GridAtom[,] Atoms
        {
            get { return _atoms; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int PixelWidth
        {
            get { return _width * _tileSize; }
        }

        public int PixelHeight
        {
            get { return _height * _tileSize; }
        }

        public Contents Contents
        {
            get { return _contents; }
        }
    }
}