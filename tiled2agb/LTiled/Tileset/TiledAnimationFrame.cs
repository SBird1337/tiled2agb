using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tiled2agb.LTiled.Tileset
{
    /// <summary>
    /// Represents a single animation frame of a TiledAnimation
    /// </summary>
    [XmlRoot(ElementName = "frame")]
    public class TiledAnimationFrame
    {
        /// <summary>
        /// Duration of the Frame in milliseconds
        /// </summary>
        [XmlAttribute(AttributeName = "duration")]
        public uint Duration { get; set; }

        /// <summary>
        /// Tile ID of the current frame to display
        /// </summary>
        [XmlAttribute(AttributeName = "tileid")]
        public uint Tileid { get; set; }
    }
}
