using KingNetwork.Example.Server.PacketHandlers;
using KingNetwork.Example.Shared.PacketHandlers;
using KingNetwork.Server;
using KingNetwork.Shared;
using System;
using System.Threading;

namespace KingNetwork.Example.TestServer
{
    class Program
    {
        private KingServer _server;

        public void Run()
        {
            try
            {
                _server = new KingServer(7171);

                _server.PutHandler<MyPacketHandlerOne>(MyPackets.Default);
                _server.PutHandler<MyPacketHandlerOne>(MyPackets.MyTestPacketOne);

                new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(2000);

                        var message = new byte[ConnectionSettings.MAX_MESSAGE_BUFFER];
                        message[0] = 1;

                        _server.SendMessageToAll(message);
                    }
                }).Start();

                _server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void Main(string[] args)
        {

            var program = new Program();
            program.Run();

            Console.ReadKey();
        }
    }
}
