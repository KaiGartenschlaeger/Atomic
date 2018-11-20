using System.Xml.Serialization;

namespace Atomic.Services.Settings.Entities
{
    public class AudioSettingsElement
    {
        [XmlElement("EffectsVolume")]
        public byte EffectsVolume { get; set; }
    }
}