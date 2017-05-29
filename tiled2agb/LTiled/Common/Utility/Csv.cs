using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tiled2agb.Compiler;

namespace tiled2agb.LTiled.Common.Utility
{
    public static class Csv
    {
        public static void AddDataToMapList(List<uint> mapList, string data, CompilerContext context)
        {
            string[] lines = data.Split('\n');
            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i] != string.Empty)
                {
                    string[] tiles = lines[i].Split(',');
                    for (int j = 0; j < tiles.Length; ++j)
                    {
                        if (tiles[j] != string.Empty)
                        {
                            mapList.Add(Convert.ToUInt32(tiles[j].Trim()));
                        }
                            
                    }
                }
            }
        }
    }
}
