using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tiled2agb.LTiled.Map
{
    [XmlRoot(ElementName = "data")]
    public class TiledMapData
    {
        [XmlAttribute(AttributeName = "encoding")]
        public string Encoding { get; set; }

        [XmlText]
        public string MapDataText { get; set; }
    }
}
