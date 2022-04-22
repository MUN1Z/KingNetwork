using KingNetwork.Client;
using KingNetwork.Shared.Enums;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using KingNetwork.SimpleExample.Shared;
using System;
using System.Threading;

namespace KingNetwork.SimpleExample.Client
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
                _networkListenerType = NetworkListenerType.RUDP;

                var client = new KingClient();
                client.MessageReceivedHandler = OnMessageReceived;
                client.Connect("127.0.0.1", 7171, _networkListenerType);

                if (client.HasConnected)
                {
                    Console.WriteLine("client.HasConnected");
                }

                new Thread(() =>
                {
                    Thread.Sleep(5000);

                    using (var buffer = KingBufferWriter.Create())
                    {
                        if (_networkListenerType != NetworkListenerType.WSText)
                            buffer.Write(MyPackets.PacketOne);

                        if(_networkListenerType == NetworkListenerType.RUDP)
                        {
                            buffer.Write("Testinho1 - Reliable");
                            client.SendMessage(buffer, RudpMessageType.Reliable);

                            buffer.Reset();

                            buffer.Write(MyPackets.PacketOne);
                            buffer.Write("Testinho2 - Unreliable");
                            client.SendMessage(buffer, RudpMessageType.Unreliable);
                        }
                        else
                        {
                            buffer.Write("Testinho1");
                            client.SendMessage(buffer);
                        }
                    }
                }).Start();

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
        /// <param name="reader">The king buffer reader from received message.</param>
        private static void OnMessageReceived(IKingBufferReader reader)
        {
            try
            {
                if (_networkListenerType == NetworkListenerType.WSText)
                    Console.WriteLine($"OnMessageReceived: {reader.ReadString()}");
                else
                    switch (reader.ReadMessagePacket<MyPackets>())
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
