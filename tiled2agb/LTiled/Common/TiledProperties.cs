using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tiled2agb.Compiler;
using tiled2agb.LTiled.Extensions;

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

        public uint RetrieveFormattedInt(string propertyName, CompilerContext context, object caller)
        {
            TiledProperty output = PropertyList.FirstOrDefault(prop => prop.Name == propertyName);
            if (output == null)
            {
                context.ExitError("could not find property {0}, in {1} object", propertyName, caller.GetType().Name);
                return 0;
            }
            return (uint)output.Value.ToLong(CultureInfo.InvariantCulture);
        }

        public string RetrieveString(string propertyName, CompilerContext context, object caller)
        {
            TiledProperty output = PropertyList.FirstOrDefault(prop => prop.Name == propertyName);
            if (output == null)
            {
                context.ExitError("could not find property {0}, in {1} object", propertyName, caller.GetType().Name);
                return null;
            }
            return output.Value;
        }
    }
}
