using System;
using System.IO;
using System.Xml.Serialization;
using tiled2agb.LTiled.Tileset;
using tiled2agb.LTiled.Map;
namespace tiled2agb
{
    class Program
    {
        static void Main(string[] args)
        {
            TiledMap map;
            string mappath = @"C:\Users\Philipp\Desktop\sots tilesets\";
            XmlSerializer serializer = new XmlSerializer(typeof(TiledMap));
            try
            {
                map = (TiledMap)serializer.Deserialize(new FileStream(mappath+ "alabastia.tmx", FileMode.Open, FileAccess.Read));
                map.LoadData(mappath);
            }
            catch (Exception ex)
            {
                Console.ReadLine();
            }
            Console.ReadLine();
        }
    }
}
