using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.Compiler;
using tiled2agb.LTiled.Tileset;

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

        public void LoadTileset(string mapPath, CompilerContext context)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TiledTileset));
            try
            {
                RealTileset = ((TiledTileset)serializer.
                    Deserialize(new FileStream(
                        Path.Combine(mapPath, Source), FileMode.Open, FileAccess.Read)));
            }
            catch (Exception ex)
            {
                context.ExitError("could not deserialize {0} as tileset: {1}", ex, Source, ex.Message);
            }
        }
    }
}
