using System.Xml.Serialization;

namespace RestoreMonarchy.CustomCommands.Models
{
    public class CustomMessage
    {
        [XmlAttribute]
        public string Text { get; set; }
        [XmlAttribute]
        public string Color { get; set; }
        [XmlAttribute]
        public string IconUrl { get; set; }
    }
}
