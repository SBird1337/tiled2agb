using System;
using System.IO;
using System.Xml.Serialization;
using tiled2agb.LTiled.Tileset;
using tiled2agb.LTiled.Map;
using tiled2agb.Compiler;
using tiled2agb.Properties;

namespace tiled2agb
{
    class Program
    {
        static void Main(string[] args)
        {
            string mappath = @"C:\Users\Philipp\Desktop\sots tilesets\";
            XmlSerializer serializer = new XmlSerializer(typeof(TiledMap));
            TiledMapCompiler compiler = new TiledMapCompiler();
            string output = "";
            try
            {
                output = (compiler.CompileMap(mappath + "alabastia.tmx"));
            }
            catch (CompilerErrorException ex)
            {
                Console.WriteLine(Resources.STR_PREFIX_CONSOLE + "error: " + compiler.Context.CompilerError);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: unhandled exception: {0}", ex.Message);
            }
            File.WriteAllText(Path.Combine(@"D:\cygwin\home\Philipp\sots\source_of_the_sovereign\src\test", "alabastia.S"), output);
            Console.ReadLine();
        }
    }
}
