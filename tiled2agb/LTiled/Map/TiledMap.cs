using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.LTiled.Tileset;
using tiled2agb.Properties;
using tiled2agb.LTiled.Extensions;
using tiled2agb.Compiler;

namespace tiled2agb.LTiled.Map
{
    /// <summary>
    /// Represents a tiled map
    /// </summary>
    [XmlRoot(ElementName = "map")]
    public class TiledMap
    {
        [XmlElement(ElementName = "tileset")]
        public List<TiledTilesetDefinition> TilesetDefinitions { get; set; }

        [XmlElement(ElementName = "layer")]
        public List<TiledLayer> Layers { get; set; }

        [XmlElement(ElementName = "objectgroup")]
        public List<TiledObjectGroup> ObjectGroups { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "orientation")]
        public string Orientation { get; set; }

        [XmlAttribute(AttributeName = "renderorder")]
        public string RenderOrder { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public uint Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public uint Height { get; set; }

        [XmlAttribute(AttributeName = "tilewidth")]
        public uint Tilewidth { get; set; }

        [XmlAttribute(AttributeName = "tileheight")]
        public uint Tileheight { get; set; }

        [XmlAttribute(AttributeName = "nextobjectid")]
        public uint Nextobjectid { get; set; }

        public void LoadTilesets(string mapPath, CompilerContext context)
        {
            foreach (TiledTilesetDefinition def in TilesetDefinitions)
            {
                def.LoadTileset(mapPath, context);
            }
        }
    }
}
