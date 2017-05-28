using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.LTiled.Common;

namespace tiled2agb.LTiled.Map
{
    [XmlRoot(ElementName = "layer")]
    public class TiledLayer : IPropertyContainer
    {
        [XmlElement(ElementName = "data")]
        public TiledMapData Data { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "properties")]
        public TiledProperties Properties { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public uint Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public uint Height { get; set; }

        [XmlAttribute(AttributeName = "opacity")]
        public float Opacity { get; set; }
    }
}
