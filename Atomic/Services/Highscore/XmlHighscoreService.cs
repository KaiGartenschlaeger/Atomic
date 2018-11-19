using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Atomic.Services.Highscore
{
    public class XmlHighscoreService : IHighscoreService
    {
        #region Fields

        private readonly string _directoryPath;
        private readonly string _filePath;

        private Highscore[] _items;

        #endregion

        #region Constructor

        public XmlHighscoreService()
        {
            _directoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                AppConstants.AppDataDirectoryName);

            _filePath = Path.Combine(_directoryPath, AppConstants.HighscoreFilename + ".xml");

            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            _items = new Highscore[0];
        }

        #endregion

        #region Methods

        public void Load()
        {
            var result = new List<Highscore>();

            if (File.Exists(_filePath))
            {
                var doc = new XmlDocument();
                doc.Load(_filePath);

                foreach (XmlNode nodeHighscore in
                    doc.SelectNodes("./Highscores/Highscore"))
                {
                    var item = new Highscore();
                    item.User = nodeHighscore["User"].InnerText;
                    item.Time = TimeSpan.FromSeconds(Convert.ToDouble(nodeHighscore["Time"].InnerText));
                    item.Score = Convert.ToInt32(nodeHighscore["Score"].InnerText);
                    item.Atoms = Convert.ToInt32(nodeHighscore["Atoms"].InnerText);
                    item.Molecules = Convert.ToInt32(nodeHighscore["Molecules"].InnerText);

                    result.Add(item);
                }
            }

            _items = result.OrderByDescending(i => i.Score).ToArray();
        }

        public void Save()
        {
            using (var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument(true);

                writer.WriteStartElement("Highscores");
                writer.WriteAttributeString("Version", "1");
                writer.WriteAttributeString("Date", DateTime.UtcNow.ToString());

                foreach (var item in _items.Take(AppConstants.TopHighscorePlaces))
                {
                    writer.WriteStartElement("Highscore");
                    writer.WriteElementString("User", item.User);
                    writer.WriteElementString("Time", item.Time.TotalSeconds.ToString());
                    writer.WriteElementString("Score", item.Score.ToString());
                    writer.WriteElementString("Atoms", item.Atoms.ToString());
                    writer.WriteElementString("Molecules", item.Molecules.ToString());
                    writer.WriteEndElement(); // highscore
                }

                writer.WriteEndElement(); // highscores

                writer.WriteEndDocument();
            }
        }

        public int Add(string user, TimeSpan time, int score, int atoms, int molecules)
        {
            var list = new List<Highscore>(_items);

            var index = -1;
            var item = (Highscore)null;

            if (score > 0)
            {
                item = new Highscore
                {
                    User = user,
                    Time = time,
                    Score = score,
                    Atoms = atoms,
                    Molecules = molecules
                };

                list.Add(item);
                list = list.OrderByDescending(h => h.Score).ToList();

                index = list.FindIndex(i => i == item);

                _items = list.ToArray();
            }

            return index;
        }

        public Highscore[] Items
        {
            get { return _items; }
        }

        #endregion
    }
}