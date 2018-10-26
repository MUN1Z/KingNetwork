using KingNetwork.Server;
using System;
using KingNetwork.HandlerExample.Server.PacketHandlers;
using KingNetwork.HandlerExample.Shared;

namespace KingNetwork.HandlerExample.Server
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {
        /// <summary>
        /// This method is responsible for main execution of console application.
        /// </summary>
        /// <param name="args">The string args received by parameters.</param>
        static void Main(string[] args)
        {
            try
            {
                var server = new KingServer();
                server.PutHandler<MyPacketHandlerOne, MyPackets>(MyPackets.PacketOne);
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
