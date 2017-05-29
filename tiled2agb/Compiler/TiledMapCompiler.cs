using System;
using System.Collections.Generic;
using System.Text;
using tiled2agb.Properties;
using tiled2agb.LTiled.Map;
using System.Xml.Serialization;
using System.IO;
using map2agblib.Map;
using tiled2agb.LTiled.Extensions;
using map2agblib.Map.Event;

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
            header.Light = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_LIGHT, Context, map.MapLayer);
            header.BattleStyle = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_BATTLETYPE, Context, map.MapLayer);
            header.Cave = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_CAVE, Context, map.MapLayer);
            header.Name = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_NAME, Context, map.MapLayer);
            header.Index = (ushort)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_INDEX, Context, map.MapLayer);
            header.Unknown = (byte)map.MapLayer.Properties.RetrieveFormattedInt(Resources.STR_HEAD_UNKNOWN, Context, map.MapLayer);
            bool escapeRope = map.MapLayer.Properties.RetrieveBoolean(Resources.STR_HEAD_ESCAPEROPE, Context, map.MapLayer);
            bool canDig = map.MapLayer.Properties.RetrieveBoolean(Resources.STR_HEAD_DIG, Context, map.MapLayer);
            header.EscapeRope = (byte)((escapeRope ? (1 << 1) : 0) | (canDig ? 1 : 0));

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

            EventHeader mapEventHeader = new EventHeader();
            TiledObjectGroup eventGroup = map.Child.ObjectGroups.GetObjectByMatchingProperties(Resources.STR_OBJLAYER_NAME, Resources.STR_OBJLAYER_EVENT, Context);
            if (eventGroup == null)
                Context.ExitError("could not find entity layer in object layers");
            foreach (TiledObject obj in eventGroup.Objects)
            {
                if (obj.GlobalIdentifier.TileGetTileObject(map.Child.TilesetDefinitions, Context).Type == Resources.STR_EVTTYPE_PERSON)
                {
                    EventEntityPerson person = new EventEntityPerson();
                    person.Id = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_NR, Context, obj, false, 0);
                    person.Picture = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_IMG, Context, obj, false, 0);
                    person.Field2 = (byte)(obj.Properties.RetrieveBoolean(Resources.STR_EVT_P_RIVAL, Context, obj, false, false) ? 1 : 0);
                    person.Field3 = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_F3, Context, obj, false, 0);
                    person.X = (short)(obj.X / 16);
                    person.Y = (short)(obj.Y / 16);
                    person.Height = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_HEIGHT, Context, obj, false, 0);
                    person.Behaviour = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_RUNBEHAV, Context, obj, false, 0);
                    person.Movement = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_MOVEMENT, Context, obj, false, 0);
                    person.FieldB = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_FB, Context, obj, false, 0);
                    person.IsTrainer = (byte)(obj.Properties.RetrieveBoolean(Resources.STR_EVT_P_TRAINER, Context, obj, false, false) ? 1 : 0);
                    person.FieldD = (byte)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_FD, Context, obj, false, 0);
                    person.AlertRadius = (ushort)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_SIGHT, Context, obj, false, 0);
                    person.Script = obj.Properties.RetrieveString(Resources.STR_EVT_SCRIPT, Context, obj, false, "");
                    person.Flag = (ushort)obj.Properties.RetrieveFormattedInt(Resources.STR_EVT_P_FLAG, Context, obj, false, 0x200);
                    person.Padding = 0x0000;

                    mapEventHeader.Persons.Add(person);
                }
            }

            header.Events = mapEventHeader;
            StringBuilder sb = new StringBuilder();
            MapHeaderCompile(Context, header, sb, mapPath);

            Context.ExitCode = CompilerExitCode.EXIT_SUCCESS;

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
                    uint tile = tiles[(int)(y * width + x)].TileGetRelativeId(baseMap.TilesetDefinitions, Context);
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
                    uint tile = collisionTiles[(int)(y * width + x)];
                    uint collisionData = tile.TileGetFormattedInt(baseMap.TilesetDefinitions, Context, Resources.STR_COL_DATA, false);
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
            string author = "program created by Sturmvogel";
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

            builder.Append("@" + new string(' ', (len - author.Length - 2) / 2));
            builder.Append(author + new string(' ', (int)Math.Ceiling((decimal)(len - author.Length - 2) / 2)));
            builder.AppendLine("@");

            builder.AppendLine("@" + new string(' ', len - 2) + "@");
            builder.AppendLine(new string('@', len));
            builder.AppendLine();

            string baseSymbol = Path.GetFileNameWithoutExtension(mapFile);
            builder.AppendLine("@@@  SECTION: MAPHEADER  @@@");
            builder.AppendLine();

            builder.AppendLine(".align 2");
            builder.AppendLine(".global mapheader_" + baseSymbol);
            builder.AppendLine("mapheader_" + baseSymbol + ":");

            builder.AppendLine("\t.word mapfooter_" + baseSymbol);

            //TODO: Events
            builder.AppendLine("\t.word mapevents_" + baseSymbol);

            //TODO: Scripts
            builder.AppendLine("\t.word " + "0x" + (0x0816545A).ToString("X8"));

            //TODO: Connections
            builder.AppendLine("\t.word " + "0x" + (0x0835276C).ToString("X8"));

            builder.AppendLine("\t.hword " + "0x" + header.Music.ToString("X4") + " @music");
            builder.AppendLine("\t.hword " + "0x" + header.Index.ToString("X4") + " @index");
            builder.AppendLine("\t.byte " + "0x" + header.Name.ToString("X2") + " @name");
            builder.AppendLine("\t.byte " + "0x" + header.Cave.ToString("X2") + " @cave");
            builder.AppendLine("\t.byte " + "0x" + header.Weather.ToString("X2") + " @weather");
            builder.AppendLine("\t.byte " + "0x" + header.Light.ToString("X2") + " @light");
            builder.AppendLine("\t.byte " + "0x" + header.Unknown.ToString("X2") + " @unknown");
            builder.AppendLine("\t.hword " + "0x" + header.ShowName.ToString("X4") + " @showName");
            builder.AppendLine("\t.byte " + "0x" + header.BattleStyle.ToString("X2") + " @battleStyle");
            builder.AppendLine();

            MapFooterCompile(context, header.Footer, builder, baseSymbol);
            MapEventsCompile(context, header.Events, builder, baseSymbol);
        }

        private void MapFooterCompile(CompilerContext context, MapFooter footer, StringBuilder builder, string baseSymbol)
        {

            builder.AppendLine("@@@  SECTION: MAPFOOTER  @@@");
            builder.AppendLine();
            builder.AppendLine(".align 2");
            builder.AppendLine(".global mapfooter_" + baseSymbol);
            builder.AppendLine("mapfooter_" + baseSymbol + ":");
            builder.AppendLine("\t.word " + "0x" + footer.Width.ToString("X8"));
            builder.AppendLine("\t.word " + "0x" + footer.Height.ToString("X8"));
            builder.AppendLine("\t.word mapborderblocks_" + baseSymbol);
            builder.AppendLine("\t.word mapblocks_" + baseSymbol);
            builder.AppendLine("\t.word " + "0x" + (0x082D4A94).ToString("X8"));
            builder.AppendLine("\t.word " + "0x" + (0x082D4AAC).ToString("X8"));
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
                List<ushort> currentRow = new List<ushort>();
                currentRow.AddRange(blocks[i]);
                builder.Append("\t.hword ");
                builder.AppendLine(string.Join(",", currentRow.ConvertAll(m => "0x" + m.ToString("X4"))));
            }
        }

        private void MapEventsCompile(CompilerContext context, EventHeader events, StringBuilder builder, string baseSymbol)
        {
            builder.AppendLine("@@@  SECTION: MAPFOOTER  @@@");
            builder.AppendLine();
            builder.AppendLine(".align 2");
            builder.AppendLine(".global mapevents_" + baseSymbol);
            builder.AppendLine("mapevents_" + baseSymbol + ":");

            builder.AppendFormat(".byte\t0x{0}, 0x{1}, 0x{2}, 0x{3}{4}", events.Persons.Count.ToString("X2"),
                events.Warps.Count.ToString("X2"), events.ScriptTriggers.Count.ToString("X2"), events.Signs.Count.ToString("X2"), Environment.NewLine);
            builder.AppendLine(".word \t" + (events.Persons.Count > 0 ? ("mapevents_persons_" + baseSymbol) : (0x0).ToString("X8")));
            builder.AppendLine(".word \t" + (events.Warps.Count > 0 ? ("mapevents_warps_" + baseSymbol) : (0x0).ToString("X8")));
            builder.AppendLine(".word \t" + (events.ScriptTriggers.Count > 0 ? ("mapevents_triggers_" + baseSymbol) : (0x0).ToString("X8")));
            builder.AppendLine(".word \t" + (events.Signs.Count > 0 ? ("mapevents_signs_" + baseSymbol) : (0x0).ToString("X8")));
            builder.AppendLine();

            if (events.Persons.Count > 0)
            {

                /* compile persons */
                builder.AppendLine("@@@ SECTION: PERSON EVENTS @@@");
                builder.AppendLine(".align 2");
                builder.AppendLine("mapevents_persons_" + baseSymbol + ":");
                for(int i = 0; i < events.Persons.Count; ++i)
                {
                    builder.AppendLine("@//new structure");
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].Id.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].Picture.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].Field2.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].Field3.ToString("X2"));
                    builder.AppendLine("\t.hword " + "0x" + events.Persons[i].X.ToString("X4"));
                    builder.AppendLine("\t.hword " + "0x" + events.Persons[i].Y.ToString("X4"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].Height.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].Behaviour.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].Movement.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].FieldB.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].IsTrainer.ToString("X2"));
                    builder.AppendLine("\t.byte " + "0x" + events.Persons[i].FieldD.ToString("X2"));
                    builder.AppendLine("\t.hword " + "0x" + events.Persons[i].AlertRadius.ToString("X4"));
                    builder.AppendLine("\t.word " + events.Persons[i].Script);
                    builder.AppendLine("\t.hword " + "0x" + events.Persons[i].Flag.ToString("X4"));
                    builder.AppendLine("\t.hword " + "0x" + events.Persons[i].Padding.ToString("X4"));
                }
            }
        }
    }
}
