using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public uint RetrieveFormattedInt(string propertyName, CompilerContext context, object caller, bool errorOnFailure = true, uint defaultValue = 0)
        {
            return RetrieveProperty(propertyName, context, caller, s => (uint)s.ToLong(CultureInfo.InvariantCulture), errorOnFailure, defaultValue);
        }

        public string RetrieveString(string propertyName, CompilerContext context, object caller, bool errorOnFailure = true, string defaultValue = "")
        {
            return RetrieveProperty(propertyName, context, caller, s => s, errorOnFailure, defaultValue);
        }

        public bool RetrieveBoolean(string propertyName, CompilerContext context, object caller, bool errorOnFailure = true, bool defaultValue = false)
        {
            return RetrieveProperty(propertyName, context, caller, s => Convert.ToBoolean(s), errorOnFailure, defaultValue);
        }

        public T RetrieveProperty<T>(string propertyName, CompilerContext context, object caller, Func<string, T> conversion, bool errorOnFailure = true, T defaultValue = default(T))
        {
            TiledProperty output = PropertyList.FirstOrDefault(prop => prop.Name == propertyName);
            if (output == null)
            {
                if (errorOnFailure)
                {
                    context.ExitError("could not find property {0}, in {1} object", propertyName, caller.GetType().Name);
                    Debug.Assert(false);
                }
                else
                {
                    context.PushWarning("could not find property {0} in {1} object, assuming {2}", propertyName, caller.GetType().Name, defaultValue.ToString());
                    return defaultValue;
                }
            }
            return conversion(output.Value);
        }
    }
}
