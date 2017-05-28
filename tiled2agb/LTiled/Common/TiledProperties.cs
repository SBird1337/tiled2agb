using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tiled2agb.LTiled.Common
{
    /// <summary>
    /// Represents a properties object of a tileset, map or object
    /// </summary>
    [XmlRoot(ElementName = "properties")]
    public class TiledProperties
    {
        /// <summary>
        /// The actual property list
        /// </summary>
        [XmlElement(ElementName = "property")]
        public List<TiledProperty> PropertyList { get; set; }
    }
}
