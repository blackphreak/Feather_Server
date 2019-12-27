using Feather_Server.Database;
using Feather_Server.ServerRelated;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Feather_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            AssemblyDescriptionAttribute releaseDate = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute));

            Console.WriteLine($"============== FeatherServer ==============\r\n  + Build: {version} [{releaseDate.Description}]\r\n\r\n");
            Console.WriteLine("Note: Type \"exit\" to exit.\r\n");

            // set encoder
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Lib.textEncoder = Encoding.GetEncoding("GBK");

            Console.WriteLine($"[!] Preparing Database Connection...");

            Lib.lastUID = DB2.GetInstance().getLastInsertedID("Hero", "heroID");
            Lib.lastItemUID = (uint)DB2.GetInstance().getLastInsertedID("UniqueItem", "ItemUID");
            Console.WriteLine($"[*] Last Usable HeroID : [{Lib.lastUID++}]");
            Console.WriteLine($"[*] Last Usable ItemUID: [{Lib.lastItemUID++}]");

            Console.WriteLine("[!] Registering Listener ...");

            Thread t = new Thread(delegate ()
            {
                Server srv = new Server("0.0.0.0", 6011);
            });
            t.Start();

            Console.WriteLine("[+] Everythings Ready!");

            do
            {
                Console.Write("# > ");
                consoleCmdHandler.handle(Console.ReadLine());
            } while (!Lib.endSrv);
        }
    }
}
