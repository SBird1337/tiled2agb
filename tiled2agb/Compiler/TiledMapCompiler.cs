using System;
using System.Collections.Generic;
using System.Text;
using tiled2agb.Properties;
using tiled2agb.LTiled.Map;
using System.Xml.Serialization;
using System.IO;
using map2agblib.Map;
using tiled2agb.LTiled.Extensions;

namespace tiled2agb.Compiler
{
    public class TiledMapCompiler
    {
        public CompilerContext Context { get; set; }

        public TiledMapCompiler()
        {
            Context = new CompilerContext();
        }

        public string CompileMap(string mapPath)
        {
            MainMap map;
            BorderMap borderMap;
            try
            {
                string mapDirectory = Path.GetDirectoryName(mapPath);
                XmlSerializer serializer = new XmlSerializer(typeof(TiledMap));

                FileStream mapStream = new FileStream(mapPath, FileMode.Open, FileAccess.Read);
                TiledMap tiledMain = (TiledMap)serializer.Deserialize(mapStream);
                mapStream.Close();
                mapStream.Dispose();
                map = new MainMap(tiledMain, mapDirectory, Context);

                string borderMapPath = Path.Combine(mapDirectory, map.MapLayer.Properties.RetrieveString(Resources.STR_BORDER_MAP_FILE, Context, map.MapLayer));

                /* Load the map representing the border block */


                FileStream borderStream = new FileStream(borderMapPath, FileMode.Open, FileAccess.Read);
                TiledMap tiledBorder = (TiledMap)serializer.Deserialize(borderStream);
                borderStream.Close();
                borderStream.Dispose();
                borderMap = new BorderMap(tiledBorder, Context, mapDirectory);
            }
            catch (CompilerErrorException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                Context.ExitError("could not deserialize map file(s): {0}", ex, ex.Message);
                return null;
            }
            
            /* Tiled Map is loaded, we need to create libmap2agb maps now */
            MapHeader header = new MapHeader();
            header.ShowName = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_SHOW_NAME, Context, map.MapLayer);
            header.Music = (ushort)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_MUSIC, Context, map.MapLayer);
            header.Weather = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_WEATHER, Context, map.MapLayer);
            header.Type = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_MAPTYPE, Context, map.MapLayer);
            header.BattleStyle = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_BATTLETYPE, Context, map.MapLayer);
            header.Flash = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_CAVE, Context, map.MapLayer);
            header.Name = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_NAME, Context, map.MapLayer);
            header.Index = (ushort)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_INDEX, Context, map.MapLayer);
            header.Unknown = (ushort)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_UNKNOWN, Context, map.MapLayer);

            header.Footer = new MapFooter();
            header.Footer.FirstTilesetInternal = 0x082D49B8;
            header.Footer.SecondTilesetInternal = 0x082D49D0;
            header.Footer.Width = map.Child.Width;
            header.Footer.Height = map.Child.Height;


            /* Build the border block */
            header.Footer.BorderBlock = Fill2DMap(borderMap.BorderMapTiles, borderMap.Child.Width, borderMap.Child.Height, borderMap.Child);

            header.Footer.BorderHeight = (byte)borderMap.Child.Height;
            header.Footer.BorderWidth = (byte)borderMap.Child.Width;

            /* Build the map block */
            header.Footer.MapBlock = Fill2DMap(map.MapTiles, map.Child.Width, map.Child.Height, map.Child);
            FillCollisionMap(map.CollisionTiles, map.Child.Width, map.Child.Height, map.Child, header.Footer.MapBlock);
            Context.ExitCode = CompilerExitCode.EXIT_SUCCESS;

            StringBuilder sb = new StringBuilder();
            MapHeaderCompile(Context, header, sb, mapPath);
            return sb.ToString();
        }
        private ushort[][] Fill2DMap(List<uint> tiles, uint width, uint height, TiledMap baseMap)
        {
            ushort[][] output = new ushort[height][];
            for (int y = 0; y < height; ++y)
            {
                output[y] = new ushort[width];
                for (int x = 0; x < width; ++x)
                {
                    uint tile = tiles[(int)(y * height + x)].TileGetRelativeId(baseMap.TilesetDefinitions, Context);
                    if (tile > 0xFFFF)
                    {
                        Context.ExitError("relative tile id is too big for output map: {0}", tile);
                    }
                    output[y][x] = (ushort)(tile);
                }
            }
            return output;
        }

        private void FillCollisionMap(List<uint> collisionTiles, uint width, uint height, TiledMap baseMap, ushort[][] blockInput)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    uint tile = collisionTiles[(int)(y * height + x)];
                    uint collisionData = tile.TileGetIntProperty(baseMap.TilesetDefinitions, Context, Resources.STR_COL_DATA, false, 0);
                    if (collisionData > 0x3F)
                    {
                        Context.ExitError("collision id is too big for output map: {0}", collisionData);
                    }
                    blockInput[y][x] |= (ushort)(collisionData << 0xA);
                }
            }
        }

        private void MapHeaderCompile(CompilerContext context, MapHeader header, StringBuilder builder, string mapFile)
        {
            /* header */
            string file = "file: " + mapFile;
            string prog = "converted using tiled2agb";
            string date = "converted on " + DateTime.Now.ToShortDateString();
            int len = Math.Max(Math.Max(file.Length, date.Length), prog.Length) + 25;
            builder.AppendLine(new string('@', len));
            builder.AppendLine(("@" + new string(' ', len - 2) + "@"));
            builder.Append("@" + new string(' ', (len - file.Length - 2) / 2));
            builder.Append(file + new string(' ', (int)Math.Ceiling((decimal)(len - file.Length - 2) / 2)));
            builder.AppendLine("@");

            builder.Append("@" + new string(' ', (len - prog.Length - 2) / 2));
            builder.Append(prog + new string(' ', (int)Math.Ceiling((decimal)(len - prog.Length - 2) / 2)));
            builder.AppendLine("@");

            builder.Append("@" + new string(' ', (len - date.Length - 2) / 2));
            builder.Append(date + new string(' ', (int)Math.Ceiling((decimal)(len - date.Length - 2) / 2)));
            builder.AppendLine("@");

            builder.AppendLine("@" + new string(' ', len - 2) + "@");
            builder.AppendLine(new string('@', len));
            builder.AppendLine();

            string baseSymbol = Path.GetFileNameWithoutExtension(mapFile);
            builder.AppendLine("@@@  SECTION: MAPHEADER  @@@");
            builder.AppendLine();

            builder.AppendLine(".align 2");
            builder.AppendLine(".global mapheader_" + baseSymbol);
            builder.AppendLine("mapheader_" + baseSymbol);

            builder.AppendLine("\t.word mapfooter_" + baseSymbol);

            //TODO: Events
            builder.AppendLine("\t.word " + (0x083B4C9C).ToString("X8"));

            //TODO: Scripts
            builder.AppendLine("\t.word " + (0x081653C2).ToString("X8"));

            //TODO: Connections
            builder.AppendLine("\t.word " + (0x08352690).ToString("X8"));

            builder.AppendLine("\t.hword " + header.Music.ToString("X4"));
            builder.AppendLine("\t.hword " + header.Index.ToString("X4"));
            builder.AppendLine("\t.byte " + header.Name.ToString("X2"));
            builder.AppendLine("\t.byte " + header.Flash.ToString("X2"));
            builder.AppendLine("\t.byte " + header.Weather.ToString("X2"));
            builder.AppendLine("\t.byte " + header.Type.ToString("X2"));
            builder.AppendLine("\t.hword " + header.Unknown.ToString("X4"));
            builder.AppendLine("\t.byte " + header.ShowName.ToString("X2"));
            builder.AppendLine("\t.byte " + header.BattleStyle.ToString("X2"));
            builder.AppendLine();

            MapFooterCompile(context, header.Footer, builder, baseSymbol);
        }

        private void MapFooterCompile(CompilerContext context, MapFooter footer, StringBuilder builder, string baseSymbol)
        {

            builder.AppendLine("@@@  SECTION: MAPFOOTER  @@@");
            builder.AppendLine();
            builder.AppendLine(".align 2");
            builder.AppendLine(".global mapfooter_" + baseSymbol);
            builder.AppendLine("mapfooter_" + baseSymbol + ":");
            builder.AppendLine("\t.word " + footer.Width.ToString("X8"));
            builder.AppendLine("\t.word " + footer.Height.ToString("X8"));
            builder.AppendLine("\t.word mapborderblocks_" + baseSymbol);
            builder.AppendLine("\t.word mapblocks_" + baseSymbol);
            builder.AppendLine("\t.word " + (0x082D49B8).ToString("X8"));
            builder.AppendLine("\t.word " + (0x082D49D0).ToString("X8"));
            builder.AppendLine("\t.byte " + footer.BorderWidth.ToString());
            builder.AppendLine("\t.byte " + footer.BorderHeight.ToString());
            builder.AppendLine("\t.hword " + footer.Padding);


            builder.AppendLine("@@@  SECTION: MAPBLOCKS  @@@");
            builder.AppendLine(".align 2");
            builder.AppendLine();
            builder.AppendLine(".global mapblocks_" + baseSymbol);
            builder.AppendLine("mapblocks_" + baseSymbol + ":");

            BlocksArrayCompile(context, builder, baseSymbol, footer.MapBlock);

            builder.AppendLine("@@@  SECTION: BORDERBLOCKS  @@@");
            builder.AppendLine(".align 2");
            builder.AppendLine();
            builder.AppendLine(".global mapborderblocks_" + baseSymbol);
            builder.AppendLine("mapborderblocks_" + baseSymbol + ":");

            BlocksArrayCompile(context, builder, baseSymbol, footer.BorderBlock);
        }

        private void BlocksArrayCompile(CompilerContext context, StringBuilder builder, string baseSymbol, ushort[][] blocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                for (int j = 0; j < blocks[i].Length; j++)
                {
                    List<ushort> currentRow = new List<ushort>();
                    currentRow.AddRange(blocks[i]);
                    builder.Append("\t.hword ");
                    builder.AppendLine(string.Join(",", currentRow.ConvertAll(m => m.ToString("X4"))));
                }
            }
        }
    }
}
