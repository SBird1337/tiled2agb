using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using tiled2agb.Compiler;
using tiled2agb.LTiled.Extensions;
using tiled2agb.LTiled.Tileset;
using tiled2agb.Properties;
using tiled2agb.LTiled.Common.Utility;

namespace tiled2agb.LTiled.Map
{
    public class MainMap
    {
        public MainMap(TiledMap map, string mapPath, CompilerContext context)
        {
            this.Child = map;
            LoadData(mapPath, context);
        }

        public TiledMap Child { get; set; }

        public List<TiledTileset> Tilesets { get; set; }

        public List<uint> MapTiles { get; set; }

        public List<uint> CollisionTiles { get; set; }

        public TiledLayer MapLayer { get; set; }

        public TiledLayer CollisionLayer { get; set; }

        public void LoadTileData(CompilerContext context)
        {
            /* we do not need layers without properties, they will be ignored anyways */
            List<TiledLayer> flaggedRemove = new List<TiledLayer>();
            foreach (TiledLayer layer in Child.Layers)
            {
                if (layer.Properties == null)
                {
                    flaggedRemove.Add(layer);
                    //TODO: Add Warning to warning stack
                }
            }
            foreach (TiledLayer removeLayer in flaggedRemove)
            {
                Child.Layers.Remove(removeLayer);
            }
            TiledLayer collisionLayer = Child.Layers.GetObjectByMatchingProperties(Resources.STR_PROPERTY_LTYPE_NAME, Resources.STR_PROPERTY_LTYPE_COLLISION, context);
            TiledLayer mapLayer = Child.Layers.GetObjectByMatchingProperties(Resources.STR_PROPERTY_LTYPE_NAME, Resources.STR_PROPERTY_LTYPE_MAP, context);

            if (collisionLayer == null)
            {
                context.ExitError("could not find collision layer, specify custom property '{0}' as '{1}'", Resources.STR_PROPERTY_LTYPE_NAME, Resources.STR_PROPERTY_LTYPE_COLLISION);
            }
            if (mapLayer == null)
            {
                context.ExitError("could not find map layer, specify custom property '{0}' as '{1}'", Resources.STR_PROPERTY_LTYPE_NAME, Resources.STR_PROPERTY_LTYPE_MAP);
            }
            if (mapLayer.Data.Encoding != Resources.STR_ENCODING_CSV)
            {
                context.ExitError("encoding of map layer is not csv");
            }
            if (collisionLayer.Data.Encoding != Resources.STR_ENCODING_CSV)
            {
                context.ExitError("encoding of collision layer is not csv");
            }

            MapTiles = new List<uint>();
            CollisionTiles = new List<uint>();

            Csv.AddDataToMapList(MapTiles, mapLayer.Data.MapDataText, context);
            Csv.AddDataToMapList(CollisionTiles, collisionLayer.Data.MapDataText, context);

            MapLayer = mapLayer;
            CollisionLayer = collisionLayer;
        }

        public void LoadData(string mapPath, CompilerContext context)
        {
            LoadTileData(context);
            Child.LoadTilesets(mapPath, context);
        }

    }
}
