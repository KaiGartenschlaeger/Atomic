using Atomic.Entities;
using System;
using System.IO;
using System.Text;

namespace Atomic.Services
{
    public class SaveGameService
    {
        public string GetSaveDirectory()
        {
            var saveDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Atomics");

            return saveDirectory;
        }

        public string GetSaveFilePath(string filename)
        {
            return Path.Combine(GetSaveDirectory(), filename);
        }

        public string GetSaveFilePathLastGame()
        {
            return GetSaveFilePath("LastGame.dat");
        }

        public bool HasLastGame()
        {
            return File.Exists(GetSaveFilePathLastGame());
        }

        public void SaveGame(GameSession session, AtomsGrid grid, Atom currentAtom, Atom nextAtom)
        {
            // todo: move currentAtom and nextAtom to GameSession

            var saveDirectory = GetSaveDirectory();

            var dirInfo = new DirectoryInfo(saveDirectory);
            if (!dirInfo.Exists) dirInfo.Create();

            var filePath = GetSaveFilePathLastGame();

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(new[] { 'A', 'S', 'G' });  // identify
                writer.Write((byte)1);                  // version

                writer.Write(session.Time);

                writer.Write(session.Score);
                writer.Write(session.Atoms);
                writer.Write(session.Molecules);

                writer.Write((byte)currentAtom.Electrons);
                writer.Write((byte)nextAtom.Electrons);

                for (int gridX = 0; gridX < grid.Width; gridX++)
                {
                    for (int gridY = 0; gridY < grid.Height; gridY++)
                    {
                        var atom = grid.GetAtom(gridX, gridY);
                        if (atom != null)
                        {
                            writer.Write((byte)atom.Electrons);
                            writer.Write(atom.LeftConnection != null);
                            writer.Write(atom.TopConnection != null);
                            writer.Write(atom.RightConnection != null);
                            writer.Write(atom.BottomConnection != null);
                        }
                        else
                        {
                            writer.Write((byte)0);
                        }
                    }
                }
            }
        }

        public void LoadGame(GameSession session, AtomsGrid grid)
        {
            // todo: move currentAtom and nextAtom to GameSession

            throw new NotImplementedException();
        }
    }
}