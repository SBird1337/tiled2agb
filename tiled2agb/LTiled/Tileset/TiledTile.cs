using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.LTiled.Common;

namespace tiled2agb.LTiled.Tileset
{
    /// <summary>
    /// Represents a single tile of a tileset
    /// </summary>
    [XmlRoot(ElementName = "tile")]
    public class TiledTile
    {
        /// <summary>
        /// Animation state of a single tile
        /// </summary>
        [XmlElement(ElementName = "animation")]
        public TiledAnimation Animation { get; set; }

        /// <summary>
        /// Tile ID
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public uint Id { get; set; }

        /// <summary>
        /// Custom Properties of the tile
        /// </summary>
        [XmlElement(ElementName = "properties")]
        public TiledProperties Properties { get; set; }

        /// <summary>
        /// Type of the tile (Property)
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }
}
