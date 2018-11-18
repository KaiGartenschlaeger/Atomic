using System;
using System.IO;

namespace Atomic.Services.SaveGames
{
    public class BinSaveGameService : ISaveGameService
    {
        #region Consts

        private string IdentityString = "ASG";
        private byte MaxSupportedVersion = 1;

        #endregion

        #region Fields

        private readonly string _directoryPath;

        #endregion

        #region Constructor

        public BinSaveGameService()
        {
            _directoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Atomics");

            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);
        }

        #endregion

        #region Private methods

        private string GetFilePath(string filename)
        {
            var extension = Path.GetExtension(filename);
            if (extension != ".xml") filename += ".xml";

            return Path.Combine(_directoryPath, filename);
        }

        #endregion

        #region Public methods

        public bool HasSaveGame(string filename)
        {
            return File.Exists(GetFilePath(filename));
        }

        public void SaveGame(string filename, SaveGameData data)
        {
            using (var stream = new FileStream(GetFilePath(filename), FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream))
            {
                foreach (var ch in IdentityString)
                {
                    writer.Write(ch);
                }

                writer.Write(MaxSupportedVersion);

                writer.Write(data.ElapsedTime.TotalSeconds);
                writer.Write(data.Score);
                writer.Write((ushort)data.Atoms);
                writer.Write((ushort)data.Molecules);
                writer.Write((byte)data.CurrentAtom);
                writer.Write((byte)data.NextAtom);

                writer.Write((ushort)data.GridData.GetLength(0));
                writer.Write((ushort)data.GridData.GetLength(1));

                for (int gridX = 0; gridX < data.GridData.GetLength(0); gridX++)
                {
                    for (int gridY = 0; gridY < data.GridData.GetLength(1); gridY++)
                    {
                        var atom = data.GridData[gridX, gridY];
                        if (atom != null)
                        {
                            writer.Write((byte)atom.Electrons);
                            writer.Write(atom.ConnectedLeft);
                            writer.Write(atom.ConnectedTop);
                            writer.Write(atom.ConnectedRight);
                            writer.Write(atom.ConnectedBottom);
                        }
                        else
                        {
                            writer.Write((byte)0);
                        }
                    }
                }
            }
        }

        public SaveGameData LoadGame(string filename)
        {
            using (var stream = new FileStream(GetFilePath(filename), FileMode.Open, FileAccess.Read, FileShare.None))
            using (var reader = new BinaryReader(stream))
            {
                foreach (var ch in IdentityString)
                {
                    if (reader.ReadChar() != ch)
                        throw new Exception("Invalid file");
                }

                var version = reader.ReadByte();
                if (version < 1 || version > MaxSupportedVersion)
                    throw new NotSupportedException("Unsupported file version");

                var result = new SaveGameData();
                result.ElapsedTime = TimeSpan.FromSeconds(reader.ReadDouble());
                result.Score = reader.ReadInt32();
                result.Atoms = reader.ReadUInt16();
                result.Molecules = reader.ReadUInt16();
                result.CurrentAtom = reader.ReadByte();
                result.NextAtom = reader.ReadByte();

                var gridWidth = reader.ReadUInt16();
                var gridHeight = reader.ReadUInt16();

                result.GridData = new SaveGameGridData[gridWidth, gridHeight];
                for (int gridX = 0; gridX < gridWidth; gridX++)
                {
                    for (int gridY = 0; gridY < gridHeight; gridY++)
                    {
                        var electrons = reader.ReadByte();
                        if (electrons > 0)
                        {
                            result.GridData[gridX, gridY] = new SaveGameGridData
                            {
                                Electrons = electrons,
                                ConnectedLeft = reader.ReadBoolean(),
                                ConnectedTop = reader.ReadBoolean(),
                                ConnectedRight = reader.ReadBoolean(),
                                ConnectedBottom = reader.ReadBoolean()
                            };
                        }
                    }
                }

                return result;
            }
        }

        #endregion
    }
}