using KingNetwork.Client;
using KingNetwork.Shared;
using KingNetwork.SimpleExample.Shared;
using System;

namespace KingNetwork.SimpleExample.Client
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
                client.MessageReceivedHandler = OnMessageReceived;
                client.Connect("127.0.0.1");

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of message received from server in client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer from received message.</param>
        private static void OnMessageReceived(KingBuffer kingBuffer)
        {
            try
            {
                switch (kingBuffer.ReadMessagePacket<MyPackets>())
                {
                    case MyPackets.PacketOne:
                        Console.WriteLine("OnMessageReceived for PacketOne");
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
