using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tiled2agb.Compiler;
using tiled2agb.LTiled.Map;
using tiled2agb.LTiled.Tileset;
using tiled2agb.Properties;

namespace tiled2agb.LTiled.Extensions
{
    public static class PrimitiveExtensions
    {
        public static uint TileGetRelativeId(this uint tile, List<TiledTilesetDefinition> tilesets, CompilerContext context)
        {
            TiledTilesetDefinition tileset = tilesets.GetTilesetByGlobalTileId(tile);
            if (tileset == null)
            {
                context.ExitError("could not find tileset for id {0} in collection", tile);
                return 0;
            }
            return tile - tileset.FirstGlobalId;
        }

        public static uint TileGetIntProperty(this uint tile, List<TiledTilesetDefinition> tilesets, CompilerContext context, string propertyName, bool errorOnNotFound, uint defaultValue = 0)
        {
            TiledTilesetDefinition tileset = tilesets.GetTilesetByGlobalTileId(tile);
            uint relativeTile = tile.TileGetRelativeId(tilesets, context);
            TiledTile chosenTile = tileset.RealTileset.Tiles.FirstOrDefault(t => t.Id == relativeTile);
            if (chosenTile == null)
            {
                if (errorOnNotFound)
                {
                    context.ExitError("could not find {0} on tile {1}", propertyName, tile);
                }
                context.PushWarning("tile {0}  does not have a {1}, assuming {2}", tile, propertyName, defaultValue);
                return defaultValue;
            }
            if (chosenTile.Properties.PropertyList == null)
            {
                if (errorOnNotFound)
                {
                    context.ExitError("could not find {0} on tile {1}", propertyName, tile);
                }
                context.PushWarning("tile {0}  does not have a {1}, assuming {2}", tile, propertyName, defaultValue);
                return defaultValue;
            }
            if (!chosenTile.Properties.PropertyList.Exists(prop => prop.Name == propertyName))
            {
                if (errorOnNotFound)
                {
                    context.ExitError("could not find {0} on tile {1}", propertyName, tile);
                }
                context.PushWarning("tile {0}  does not have a {1}, assuming {2}", tile, propertyName, defaultValue);
                return defaultValue;
            }
            return chosenTile.Properties.RetrieveFormattedInt(propertyName, context, chosenTile);
        }
    }
}
