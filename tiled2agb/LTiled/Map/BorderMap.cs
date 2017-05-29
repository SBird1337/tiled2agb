using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tiled2agb.Compiler;
using tiled2agb.LTiled.Common.Utility;
using tiled2agb.LTiled.Extensions;
using tiled2agb.Properties;

namespace tiled2agb.LTiled.Map
{
    public class BorderMap
    {
        public TiledMap Child { get; set; }
        public List<uint> BorderMapTiles { get; set; }
        public TiledLayer BorderLayer { get; set; }

        public BorderMap(TiledMap map, CompilerContext context, string mapPath)
        {
            Child = map;
            BorderMapTiles = new List<uint>();

            BorderLayer = Child.Layers.GetObjectByMatchingProperties(Resources.STR_PROPERTY_LTYPE_NAME, Resources.STR_PROPERTY_LTYPE_BORDER, context);
            if (BorderLayer == null)
                context.ExitError("could not find border layer in border map file");

            foreach (TiledTilesetDefinition def in Child.TilesetDefinitions)
            {
                def.LoadTileset(mapPath, context);
            }
            Csv.AddDataToMapList(BorderMapTiles, BorderLayer.Data.MapDataText, context);
        }
    }
}
