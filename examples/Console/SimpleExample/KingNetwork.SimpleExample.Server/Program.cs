using KingNetwork.Server;
using System;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using KingNetwork.SimpleExample.Shared;

namespace KingNetwork.SimpleExample.Server
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
                server.OnMessageReceivedHandler = OnMessageReceived;
                server.Start();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of message received from client in server.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer from received message.</param>
        private static void OnMessageReceived(IClient client, IKingBuffer kingBuffer)
        {
            try
            {
                switch (kingBuffer.ReadMessagePacket<MyPackets>())
                {
                    case MyPackets.PacketOne:
                        Console.WriteLine($"OnMessageReceived PacketOne from {client.Id}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
