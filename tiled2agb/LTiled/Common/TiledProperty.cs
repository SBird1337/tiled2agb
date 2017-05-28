using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tiled2agb.LTiled.Common
{
    /// <summary>
    /// Represents a single property, its type, name and value
    /// </summary>
    [XmlRoot(ElementName = "property")]
    public class TiledProperty
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Datatype of the Property
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Value of the property, interpretation depends on data type
        /// </summary>
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}
