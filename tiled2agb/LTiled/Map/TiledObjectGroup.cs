using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.LTiled.Common;

namespace tiled2agb.LTiled.Map
{
    [XmlRoot(ElementName = "objectgroup")]
    public class TiledObjectGroup
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "properties")]
        public TiledProperties Properties { get; set; }

        [XmlElement(ElementName = "object")]
        public List<TiledObject> Objects { get; set; }
    }
}
