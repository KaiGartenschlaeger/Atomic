using System;
using System.IO;
using System.Xml;

namespace Atomic.Services.SaveGames
{
    public class XmlSaveGameService : ISaveGameService
    {
        #region Consts

        private byte MaxSupportedVersion = 2;

        #endregion

        #region Fields

        private readonly string _directoryPath;

        #endregion

        #region Constructor

        public XmlSaveGameService()
        {
            _directoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppConstants.AppDataDirectoryName);

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
            using (var writer = XmlWriter.Create(GetFilePath(filename), new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument(true);

                writer.WriteStartElement("SaveGame");
                writer.WriteAttributeString("Version", MaxSupportedVersion.ToString());
                writer.WriteAttributeString("Date", DateTime.UtcNow.ToString());

                writer.WriteStartElement("State");
                writer.WriteElementString("Time", data.ElapsedTime.TotalSeconds.ToString());

                writer.WriteElementString("Score", data.Score.ToString());
                writer.WriteElementString("Atoms", data.Atoms.ToString());
                writer.WriteElementString("Molecules", data.Molecules.ToString());

                writer.WriteElementString("AddedAtoms", data.AddedAtoms.ToString());

                writer.WriteElementString("Current", data.CurrentAtom.ToString());
                writer.WriteElementString("Next", data.NextAtom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Grid");
                writer.WriteAttributeString("W", data.GridData.GetLength(0).ToString());
                writer.WriteAttributeString("H", data.GridData.GetLength(1).ToString());

                for (int gridY = 0; gridY < data.GridData.GetLength(1); gridY++)
                {
                    writer.WriteStartElement("Data");

                    for (int gridX = 0; gridX < data.GridData.GetLength(0); gridX++)
                    {
                        writer.WriteStartElement("Atom");

                        var atom = data.GridData[gridX, gridY];
                        if (atom != null)
                        {
                            writer.WriteAttributeString("E", atom.Electrons.ToString());
                            writer.WriteAttributeString("L", atom.ConnectedLeft.ToString());
                            writer.WriteAttributeString("T", atom.ConnectedTop.ToString());
                            writer.WriteAttributeString("R", atom.ConnectedRight.ToString());
                            writer.WriteAttributeString("B", atom.ConnectedBottom.ToString());
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement(); // Data
                }

                writer.WriteEndElement(); // Grid

                writer.WriteEndElement(); // SaveGame
                writer.WriteEndDocument();
            }
        }

        public SaveGameData LoadGame(string filename)
        {
            var doc = new XmlDocument();
            doc.Load(GetFilePath(filename));

            var result = new SaveGameData();

            var nodeSaveGame = doc["SaveGame"];
            var version = Convert.ToByte(nodeSaveGame.Attributes["Version"].InnerText);
            if (version < 1 || version > MaxSupportedVersion)
                throw new NotSupportedException("File version is not supported");

            var nodeState = nodeSaveGame["State"];
            result.ElapsedTime = TimeSpan.FromSeconds(Convert.ToDouble(nodeState["Time"].InnerText));
            result.Score = Convert.ToInt32(nodeState["Score"].InnerText);
            result.Atoms = Convert.ToInt32(nodeState["Atoms"].InnerText);
            result.Molecules = Convert.ToInt32(nodeState["Molecules"].InnerText);

            if (version >= 2)
                result.AddedAtoms = result.Molecules = Convert.ToInt32(nodeState["AddedAtoms"].InnerText);

            result.CurrentAtom = Convert.ToInt32(nodeState["Current"].InnerText);
            result.NextAtom = Convert.ToInt32(nodeState["Next"].InnerText);

            var nodeGrid = nodeSaveGame["Grid"];

            var gridWidth = Convert.ToInt32(nodeGrid.Attributes["W"].InnerText);
            var gridHeight = Convert.ToInt32(nodeGrid.Attributes["H"].InnerText);

            result.GridData = new SaveGameGridData[gridWidth, gridHeight];

            var nodesData = nodeGrid.SelectNodes("./Data");
            for (int gridY = 0; gridY < nodesData.Count; gridY++)
            {
                var nodeData = nodesData[gridY];

                var nodesAtom = nodeData.SelectNodes("./Atom");
                for (int gridX = 0; gridX < nodesAtom.Count; gridX++)
                {
                    var nodeAtom = nodesAtom[gridX];
                    if (nodeAtom.Attributes["E"] != null)
                    {
                        result.GridData[gridX, gridY] = new SaveGameGridData
                        {
                            Electrons = Convert.ToInt32(nodeAtom.Attributes["E"].InnerText),
                            ConnectedLeft = Convert.ToBoolean(nodeAtom.Attributes["L"].InnerText),
                            ConnectedTop = Convert.ToBoolean(nodeAtom.Attributes["T"].InnerText),
                            ConnectedRight = Convert.ToBoolean(nodeAtom.Attributes["R"].InnerText),
                            ConnectedBottom = Convert.ToBoolean(nodeAtom.Attributes["B"].InnerText)
                        };
                    }
                }
            }

            return result;
        }

        public void DeleteSaveGame(string filename)
        {
            var path = GetFilePath(filename);
            if (File.Exists(path)) File.Delete(path);
        }

        #endregion
    }
}