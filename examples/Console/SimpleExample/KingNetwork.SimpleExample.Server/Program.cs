using KingNetwork.Server;
using System;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using KingNetwork.SimpleExample.Shared;
using KingNetwork.Shared.Interfaces;

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
                server.OnClientConnectedHandler = OnClientConnectedHandler;
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
        /// <param name="reader">The king buffer reader from received message.</param>
        private static void OnMessageReceived(IClient client, IKingBufferReader reader)
        {
            try
            {
                if (_networkListenerType == NetworkListenerType.WSText)
                {
                    Console.WriteLine($"OnMessageReceived from {client.Id}");
                    
                    string text = reader.ReadString();
                    Console.WriteLine($"Message: {text}");

                    var writer = KingBufferWriter.Create();

                    writer.Write("Testinho2");

                    client.SendMessage(writer);
                }
                else
                {
                    switch (reader.ReadMessagePacket<MyPackets>())
                    {
                        case MyPackets.PacketOne:
                            Console.WriteLine($"OnMessageReceived PacketOne from {client.Id}");
                            Console.WriteLine($"Message: {reader.ReadString()}");

                            var writer = KingBufferWriter.Create();

                            writer.Write(MyPackets.PacketOne);
                            writer.Write("Testinho2");

                            client.SendMessage(writer);

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        private static void OnClientConnectedHandler(IClient client)
        {
            try
            {
                Console.WriteLine($"OnClientConnectedHandler from {client.IpAddress}");

                var writer = KingBufferWriter.Create();

                writer.Write("Testinho2");

                client.SendMessage(writer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
