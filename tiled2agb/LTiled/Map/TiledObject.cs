using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.LTiled.Common;

namespace tiled2agb.LTiled.Map
{
    [XmlRoot(ElementName = "object")]
    public class TiledObject
    {
        [XmlElement(ElementName = "properties")]
        public TiledProperties Properties { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public uint Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "gid")]
        public uint GlobalIdentifier { get; set; }

        [XmlAttribute(AttributeName = "x")]
        public uint X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public uint Y { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public uint Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public uint Height { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }
}
