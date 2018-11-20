using System.Xml.Serialization;

namespace Atomic.Services.Settings.Entities
{
    [XmlRoot("Settings")]
    public class SettingsElement
    {
        [XmlElement("Audio")]
        public AudioSettingsElement Audio { get; set; }
    }
}