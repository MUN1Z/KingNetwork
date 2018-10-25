using KingNetwork.Example.Server.PacketHandlers;
using KingNetwork.Example.Shared.PacketHandlers;
using KingNetwork.Server;
using System;

namespace KingNetwork.Example.TestServer
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {
        /// <summary>
        /// This method is responsible for main execution of console application.
        /// </summary>
        /// <param name="args">The string args receiveds by parameters.</param>
        static void Main(string[] args)
        {
            try
            {
                var server = new KingServer(7171);
                server.PutHandler<MyPacketHandlerOne>(MyPackets.MyTestPacketOne);
                server.Start();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
