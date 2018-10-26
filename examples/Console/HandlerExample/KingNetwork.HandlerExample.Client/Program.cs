using KingNetwork.Client;
using System;
using KingNetwork.HandlerExample.Client.PacketHandlers;
using KingNetwork.HandlerExample.Shared;

namespace KingNetwork.HandlerExample.Client
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
                var client = new KingClient();
                client.PutHandler<MyPacketHandlerOne, MyPackets>(MyPackets.PacketOne);
                client.Connect("127.0.0.1");

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
