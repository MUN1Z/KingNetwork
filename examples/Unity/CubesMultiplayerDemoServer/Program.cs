using CubesMultiplayerDemoShared;
using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using System;
using System.Collections.Generic;

namespace CubesMultiplayerDemoServer
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {

        private Dictionary<IClient, NetworkPlayer> _networkPlayersDictionary;
        private KingServer _server;

        public void Run()
        {
            try
            {
                _networkPlayersDictionary = new Dictionary<IClient, NetworkPlayer>();

                _server = new KingServer();
                _server.OnMessageReceivedHandler = OnMessageReceived;
                _server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// This method is responsible for main execution of console application.
        /// </summary>
        /// <param name="args">The string args received by parameters.</param>
        static void Main(string[] args)
        {
            try
            {
                Program program = new Program();
                program.Run();

                Console.ReadKey();
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
        private void OnMessageReceived(IClient client, KingBuffer kingBuffer)
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
