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
        [XmlIgnore]
        public List<TiledTileset> Tilesets { get; set; }

        [XmlIgnore]
        public List<uint> MapTiles { get; set; }

        [XmlIgnore]
        public List<uint> CollisionTiles { get; set; }

        [XmlElement(ElementName = "tileset")]
        public List<TiledTilesetDefinition> TilesetDefinitions { get; set; }

        [XmlElement(ElementName = "layer")]
        public List<TiledLayer> Layers { get; set; }

        [XmlElement(ElementName = "objectgroup")]
        public List<TiledObjectGroup> ObjectGroup { get; set; }

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

        public void LoadData(string mapPath, CompilerContext context)
        {
            LoadTileData(context);
            LoadTilesets(mapPath, context);
        }

        public void LoadTileData(CompilerContext context)
        {
            /* we do not need layers without properties, they will be ignored anyways */
            List<TiledLayer> flaggedRemove = new List<TiledLayer>();
            foreach (TiledLayer layer in Layers)
            {
                if (layer.Properties == null)
                {
                    flaggedRemove.Add(layer);
                    //TODO: Add Warning to warning stack
                }
            }
            foreach (TiledLayer removeLayer in flaggedRemove)
            {
                Layers.Remove(removeLayer);
            }
            TiledLayer collisionLayer = Layers.GetObjectByMatchingProperties(Resources.STR_PROPERTY_LTYPE_NAME, Resources.STR_PROPERTY_LTYPE_COLLISION);
            TiledLayer mapLayer = Layers.GetObjectByMatchingProperties(Resources.STR_PROPERTY_LTYPE_NAME, Resources.STR_PROPERTY_LTYPE_MAP);

            if (collisionLayer == null)
            {
                //TODO: Actual error handler, as compiler object
                Console.WriteLine("HELP ME AN ERROR OCCURED: NO_COLLISION_MAP");
                return;
            }
            if (mapLayer == null)
            {
                //TODO: Actual error handler, as compiler object
                Console.WriteLine("HELP ME AN ERROR OCCURED: NO_BACKGROUND_MAP");
                return;
            }
            if (mapLayer.Data.Encoding != Resources.STR_ENCODING_CSV)
            {
                Console.WriteLine("HELP ME AN ERROR OCCURED: NO_CSV");
                return;
            }
            if (collisionLayer.Data.Encoding != Resources.STR_ENCODING_CSV)
            {
                Console.WriteLine("HELP ME AN ERROR OCCURED: NO_CSV");
                return;
            }

            MapTiles = new List<uint>();
            CollisionTiles = new List<uint>();

            AddDataToMapList(MapTiles, mapLayer.Data.MapDataText);
            AddDataToMapList(CollisionTiles, collisionLayer.Data.MapDataText);


        }

        public void LoadTilesets(string mapPath,CompilerContext context)
        {
            Tilesets = new List<TiledTileset>();
            XmlSerializer serializer = new XmlSerializer(typeof(TiledTileset));
            for (int i = 0; i < TilesetDefinitions.Count; ++i)
            {
                try
                {
                    TilesetDefinitions[i].RealTileset = ((TiledTileset)serializer.
                        Deserialize(new FileStream(
                            Path.Combine(mapPath, TilesetDefinitions[i].Source), FileMode.Open, FileAccess.Read)));
                }
                catch (Exception ex)
                {
                    context.ExitError("could not deserialize {0} as tileset", TilesetDefinitions[i].Source);
                }

            }
        }

        private void AddDataToMapList(List<uint> mapList, string data)
        {
            string[] lines = data.Split('\n');
            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i] != string.Empty)
                {
                    string[] tiles = lines[i].Split(',');
                    for (int j = 0; j < tiles.Length; ++j)
                    {
                        if(tiles[j] != string.Empty)
                            mapList.Add(Convert.ToUInt32(tiles[j].Trim()));
                    }
                }
            }
        }
    }
}
