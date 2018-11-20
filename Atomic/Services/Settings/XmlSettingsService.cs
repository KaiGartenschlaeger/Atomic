using Atomic.Services.Settings.Entities;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Atomic.Services.Settings
{
    public class XmlSettingsService : ISettingsService
    {
        #region Fields

        private readonly string _directoryPath;
        private readonly string _filePath;
        private readonly XmlSerializer _serializer;

        private SettingsElement _settings;

        #endregion

        #region Constructor

        public XmlSettingsService()
        {
            _directoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppConstants.AppDataDirectoryName);

            _filePath = Path.Combine(_directoryPath, "Settings.xml");

            _serializer = new XmlSerializer(typeof(SettingsElement));

            SetDefaults();
        }

        #endregion

        #region Private methods

        private void SetDefaults()
        {
            _settings = new SettingsElement();
            _settings.Audio = new AudioSettingsElement();
            _settings.Audio.EffectsVolume = 100;
        }

        #endregion

        #region Public methods

        public void Load()
        {
            if (!File.Exists(_filePath)) return;

            using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                _settings = (SettingsElement)_serializer.Deserialize(stream);
            }
        }

        public void Save()
        {
            using (var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
            {
                _serializer.Serialize(writer, _settings);
            }
        }

        #endregion

        #region Properties

        public SettingsElement Settings
        {
            get { return _settings; }
        }

        #endregion
    }
}