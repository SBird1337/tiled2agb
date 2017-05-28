using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tiled2agb.LTiled.Tileset
{
    /// <summary>
    /// Represents an animation of a single tile
    /// </summary>
    [XmlRoot(ElementName = "animation")]
    public class TiledAnimation
    {
        /// <summary>
        /// List of frames, tile ids and their animation duration
        /// </summary>
        [XmlElement(ElementName = "frame")]
        public List<TiledAnimationFrame> Frames { get; set; }
    }
}
