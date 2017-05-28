using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tiled2agb.LTiled.Map
{
    [XmlRoot(ElementName = "tileset")]
    public class TiledTilesetDefinition
    {
        [XmlAttribute(AttributeName = "firstgid")]
        public uint FirstGlobalId { get; set; }

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlIgnore]
        public Tileset.TiledTileset RealTileset { get; set; }
    }
}
