using Atomic.Services.SaveGames;
using Microsoft.Xna.Framework;
using PureFreak.TileMore;
using System;
using System.Collections.Generic;

namespace Atomic.Entities
{
    /// <summary>
    /// Represents an grid with atoms.
    /// </summary>
    public class AtomsGrid
    {
        private readonly AppContents _contents;
        private readonly GameSession _session;
        private readonly int _tileSize;
        private readonly int _width;
        private readonly int _height;
        private readonly GridAtom[,] _atoms;

        public AtomsGrid(AppContents contents, GameSession session, int cellSize, int width, int height)
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

        /// <summary>
        /// Creates a new atom with given electrons count.
        /// </summary>
        public Atom CreateAtom(int? electrons = null)
        {
            if (electrons.HasValue && (electrons < 0 || electrons > 4))
                throw new ArgumentException("Electrons must be a value between 1 and 4");

            if (!electrons.HasValue)
                electrons = RandomHelper.Between(new Range<int>(1, 4));

            return new Atom(_contents, electrons.Value);
        }

        /// <summary>
        /// Tries to create an atom with given electrons count at the given grid position.
        /// </summary>
        public void SetAtom(int gridX, int gridY, int electrons)
        {
            SetAtom(gridX, gridY, CreateAtom(electrons));
        }

        /// <summary>
        /// Tries to set the given atom to the given grid position.
        /// </summary>
        public bool SetAtom(int gridX, int gridY, Atom atom)
        {
            if (atom == null)
                throw new ArgumentNullException(nameof(atom));

            if (CanSet(gridX, gridY, atom))
            {
                _atoms[gridX, gridY] = new GridAtom(this, gridX, gridY, atom.Electrons);
                AtomAdded(_atoms[gridX, gridY]);
                return true;
            }

            return false;
        }

        public void FromSaveGame(SaveGameGridData[,] gridData)
        {
            if (gridData == null)
                throw new ArgumentNullException(nameof(gridData));

            var width = gridData.GetLength(0);
            var height = gridData.GetLength(1);

            if (width != _width || height != _height)
                throw new ArgumentException("Given grid data array has invalid dimensions.");

            Clear();
            for (int gridX = 0; gridX < width; gridX++)
            {
                for (int gridY = 0; gridY < height; gridY++)
                {
                    var data = gridData[gridX, gridY];
                    if (data != null)
                    {
                        _atoms[gridX, gridY] = new GridAtom(this, gridX, gridY, data.Electrons);
                    }
                }
            }

            for (int gridX = 0; gridX < width; gridX++)
            {
                for (int gridY = 0; gridY < height; gridY++)
                {
                    var data = gridData[gridX, gridY];
                    if (data != null)
                    {
                        var atom = _atoms[gridX, gridY];

                        if (data.ConnectedLeft)
                        {
                            atom.LeftConnection = atom.LeftAtom;
                            atom.LeftAtom.RightConnection = atom;
                        }
                        if (data.ConnectedTop)
                        {
                            atom.TopConnection = atom.TopAtom;
                            atom.TopAtom.BottomConnection = atom;
                        }
                        if (data.ConnectedRight)
                        {
                            atom.RightConnection = atom.RightAtom;
                            atom.RightAtom.LeftConnection = atom;
                        }
                        if (data.ConnectedBottom)
                        {
                            atom.BottomConnection = atom.BottomAtom;
                            atom.BottomAtom.TopConnection = atom;
                        }
                    }
                }
            }
        }

        public void RemoveAtom(int gridX, int gridY)
        {
            if (!IsValidPos(gridX, gridY))
                return;

            var atom = GetAtom(gridX, gridY);
            if (atom == null)
                return;

            var left = atom.LeftAtom;
            if (left != null && left.RightConnection != null)
            {
                left.RightConnection = null;
                left.Electrons++;
            }

            var top = atom.TopAtom;
            if (top != null && top.BottomConnection != null)
            {
                top.BottomConnection = null;
                top.Electrons++;
            }

            var right = atom.RightAtom;
            if (right != null && right.LeftConnection != null)
            {
                right.LeftConnection = null;
                right.Electrons++;
            }

            var bottom = atom.BottomAtom;
            if (bottom != null && bottom.TopConnection != null)
            {
                bottom.TopConnection = null;
                bottom.Electrons++;
            }

            _atoms[gridX, gridY] = null;
        }

        private bool CanResolveElectron(int gridX, int gridY)
        {
            var atom = GetAtom(gridX, gridY);
            if (atom != null)
                return atom.Electrons > 0;

            return IsValidPos(gridX, gridY);
        }

        /// <summary>
        /// Checks if the given atom can be set to the given grid position.
        /// </summary>
        public bool CanSet(int gridX, int gridY, Atom atom)
        {
            if (atom == null)
                throw new ArgumentNullException(nameof(atom));

            if (!IsValidPos(gridX, gridY) || HasAtom(gridX, gridY))
                return false;

            //var electronsToResolve = atom.Electrons;
            //if (CanResolveElectron(gridX - 1, gridY)) electronsToResolve--;
            //if (CanResolveElectron(gridX + 1, gridY)) electronsToResolve--;
            //if (CanResolveElectron(gridX, gridY - 1)) electronsToResolve--;
            //if (CanResolveElectron(gridX, gridY + 1)) electronsToResolve--;
            //return electronsToResolve < 1;

            return true;
        }

        /// <summary>
        /// Clears the whole grid (Removes all atoms and connections).
        /// </summary>
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
            if (addedAtom == null)
                throw new ArgumentNullException(nameof(addedAtom));

            addedAtom.ConnectToNeighbours();

            for (int gridX = addedAtom.GridX - 1; gridX < addedAtom.GridX + 1; gridX++)
            {
                for (int gridY = addedAtom.GridY - 1; gridY < addedAtom.GridY + 1; gridY++)
                {
                    var atom = GetAtom(gridX, gridY);
                    if (atom != null && atom != addedAtom)
                    {
                        atom.ConnectToNeighbours();
                    }
                }
            }

            CheckMolecule(addedAtom);
        }

        /// <summary>
        /// Checks if a molecule is complete (has no more electrons) and can be removed.
        /// </summary>
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

            RemoveMoleculeAtoms(moleculeAtoms);
            return true;
        }

        private void RemoveMoleculeAtoms(HashSet<Point> moleculeAtoms)
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

        /// <summary>
        /// Returns an grid atom or null, if no atom on given position is available.
        /// </summary>
        public GridAtom GetAtom(int gridX, int gridY)
        {
            if (IsValidPos(gridX, gridY))
                return _atoms[gridX, gridY];

            return null;
        }

        /// <summary>
        /// Returns true if an atom is available at the given grid position.
        /// </summary>
        public bool HasAtom(int gridX, int gridY)
        {
            return GetAtom(gridX, gridY) != null;
        }

        public bool IsValidPos(int gridX, int gridY)
        {
            return
                gridX >= 0 && gridX < _width &&
                gridY >= 0 && gridY < _height;
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

        public AppContents Contents
        {
            get { return _contents; }
        }
    }
}