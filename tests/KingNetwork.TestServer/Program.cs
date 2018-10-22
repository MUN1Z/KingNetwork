using KingNetwork.Server;
using System;

namespace KingNetwork.TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new KingServer("127.0.0.1", 7171);

            server.Start();

            Console.ReadLine();
        }
    }
}
