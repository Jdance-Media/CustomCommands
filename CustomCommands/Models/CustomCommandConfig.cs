using System.Xml.Serialization;

namespace RestoreMonarchy.CustomCommands.Models
{
    public class CustomCommandConfig
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Help { get; set; }
        [XmlAttribute]
        public int Cooldown { get; set; }

        public uint Experience { get; set; }
        [XmlArrayItem("item")]
        public ushort[] Items { get; set; }
        [XmlArrayItem("vehicle")]
        public ushort[] Vehicles { get; set; }
        [XmlArrayItem("message")]
        public CustomMessage[] Messages { get; set; }
    }
}
