using log4net.Config;
using Server.CommonLib.MglServerConfig;
using System;

namespace Server
{
    class Program
    {
        private static MglServer server = new MglServer();
        static void Main(string[] args)
        {                        
            server.InitConfig();
            server.CreateServer();

            var flag = server.Start();            
            if(flag)
            {
                Console.WriteLine("ok");
            }
            else
            {
                Console.WriteLine("bad");
            }
                    
            Console.ReadLine();
        }
    }
}
