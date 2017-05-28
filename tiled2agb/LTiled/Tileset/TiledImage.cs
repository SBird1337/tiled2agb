using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tiled2agb.LTiled.Tileset
{

    /// <summary>
    /// Represents an external image of a tileset
    /// </summary>
    [XmlRoot(ElementName = "image")]
    public class TiledImage
    {
        /// <summary>
        /// Height of the image in pixels
        /// </summary>
        [XmlAttribute(AttributeName = "height")]
        public uint Height { get; set; }

        /// <summary>
        /// Path to the image
        /// </summary>
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        /// <summary>
        /// Width of the image in pixels
        /// </summary>
        [XmlAttribute(AttributeName = "width")]
        public string Width { get; set; }
    }
}
