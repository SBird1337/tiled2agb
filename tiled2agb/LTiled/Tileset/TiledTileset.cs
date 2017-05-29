using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.Compiler;
using tiled2agb.LTiled.Common;

namespace tiled2agb.LTiled.Tileset
{
    /// <summary>
    /// A data model for a Tiled tileset, including its properties and animations
    /// </summary>
    [XmlRoot(ElementName = "tileset")]
    public class TiledTileset
    {
        /// <summary>
        /// Columns of the tileset
        /// </summary>
        [XmlAttribute(AttributeName = "columns")]
        public uint Columns { get; set; }

        /// <summary>
        /// Tileset graphic to work with
        /// </summary>
        [XmlElement(ElementName = "image")]
        public TiledImage Image { get; set; }

        /// <summary>
        /// Name of the Tileset
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The custom properties associated with the Tileset
        /// </summary>
        [XmlElement(ElementName = "properties")]
        public TiledProperties Properties { get; set; }

        /// <summary>
        /// A list of the actual Tiles
        /// </summary>
        [XmlElement(ElementName = "tile")]
        public List<TiledTile> Tiles { get; set; }

        /// <summary>
        /// Number of tiles in the Tileset
        /// </summary>
        [XmlAttribute(AttributeName = "tilecount")]
        public uint Count { get; set; }

        /// <summary>
        /// Height (in tiles) of the Tileset
        /// </summary>
        [XmlAttribute(AttributeName = "tileheight")]
        public uint TileHeight { get; set; }

        /// <summary>
        /// Width (in tiles) of the Tileset
        /// </summary>
        [XmlAttribute(AttributeName = "tilewidth")]
        public uint TileWidth { get; set; }

    }
}
