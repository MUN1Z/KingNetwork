using KingNetwork.Server;
using System;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using KingNetwork.SimpleExample.Shared;
using System.Text;

namespace KingNetwork.SimpleExample.Server
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {
        private static NetworkListenerType _networkListenerType;

        /// <summary>
        /// This method is responsible for main execution of console application.
        /// </summary>
        /// <param name="args">The string args received by parameters.</param>
        static void Main(string[] args)
        {
            try
            {
                _networkListenerType = NetworkListenerType.WSText;

                var server = new KingServer();
                server.OnMessageReceivedHandler = OnMessageReceived;
                server.Start(_networkListenerType);

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
        private static void OnMessageReceived(IClient client, KingBufferReader kingBuffer)
        {
            try
            {
                if (_networkListenerType == NetworkListenerType.WSText)
                {
                    Console.WriteLine($"OnMessageReceived from {client.Id}");
                    
                    //string text = kingBuffer.ReadAllText();
                    string text = kingBuffer.ReadString();
                    Console.WriteLine(text);

                    var writer = KingBufferWriter.Create();

                    //writer.Write((byte)1);
                    writer.Write("Testinho2");

                    client.SendMessage(writer);
                }
                else
                {
                    switch (kingBuffer.ReadMessagePacket<MyPackets>())
                    {
                        case MyPackets.PacketOne:
                            Console.WriteLine($"OnMessageReceived PacketOne from {client.Id}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
