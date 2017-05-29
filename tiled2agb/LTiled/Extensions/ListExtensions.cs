using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tiled2agb.Compiler;
using tiled2agb.LTiled.Common;
using tiled2agb.LTiled.Map;
using tiled2agb.LTiled.Tileset;

namespace tiled2agb.LTiled.Extensions
{
    public static class ListExtensions
    {
        public static T GetObjectByMatchingProperties<T>(this List<T> container, string propertyName, string propertyValue, CompilerContext context) where T : IPropertyContainer
        {
            List<IPropertyContainer> properties = container.ConvertAll(x => (IPropertyContainer)x);
            if (properties.Exists(prop => prop.Properties == null))
            {
                context.ExitError("could not find property {0} in {1}, while looking for {2} - property list is empty", propertyName, typeof(T).Name, propertyValue);
            }
            return container.FirstOrDefault(obj => obj.Properties.PropertyList.Exists(prop => prop.Name == propertyName && prop.Value == propertyValue));
        }

        public static TiledTilesetDefinition GetTilesetByGlobalTileId(this List<TiledTilesetDefinition> tilesets, uint tileId)
        {
            return tilesets.FirstOrDefault(set => set.FirstGlobalId <= tileId && set.FirstGlobalId + set.RealTileset.Count > tileId);
        }


    }
}
